using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.IO;
using System.Net.NetworkInformation;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using Eudemons.UnifiedServer.Models;
using Eudemons.UnifiedServer.Services;

namespace Eudemons.UnifiedServer;

public partial class MainWindow : Window, INotifyPropertyChanged
{
    private const int MaximumLogEntries = 10_000;
    private static readonly Regex ChildTimestampPattern = new(
        @"^\s*\d{1,4}[/.-]\d{1,2}[/.-]\d{1,4}\s+\d{1,2}:\d{2}:\d{2}(?:\s*[AP]M)?\s+",
        RegexOptions.Compiled | RegexOptions.IgnoreCase);
    private readonly ObservableCollection<LogEntry> _logs = [];
    private readonly ICollectionView _logView;
    private bool _isClosing;
    private bool _databaseReady;
    private bool _startAllInProgress;
    private string _overallStatusText = "All servers stopped";
    private Brush _overallStatusBrush = Brushes.Gray;

    public MainWindow()
    {
        InitializeComponent();

        RuntimeRoot = FindRuntimeRoot();
        RuntimePathText = $"Runtime  {RuntimeRoot}";

        Servers =
        [
            CreateServer("ACC", "Account Server", "AccServer.exe", "AccServer", "exit", "#58C7D6", 8000, 1),
            CreateServer("DB", "Database Server", "DBServer.exe", "DBServer", "exit", "#69CF8E", 1500, 2),
            CreateServer("LOGIN", "Login Server", "LoginServer.exe", "LoginServer", "exit", "#E8B85A", 8001, 3),
            CreateServer("MAP", "Map Server", "MapServer.exe", "MapServer", "exit", "#D27BC4", 8002, 4)
        ];

        foreach (var server in Servers)
        {
            server.PropertyChanged += ServerOnPropertyChanged;
        }

        _logView = CollectionViewSource.GetDefaultView(_logs);
        _logView.Filter = FilterLog;
        _logs.CollectionChanged += LogsOnCollectionChanged;

        DataContext = this;
        ServerFilter.ItemsSource = new[] { "All servers" }
            .Concat(Servers.Select(server => server.DisplayName));
        ServerFilter.SelectedIndex = 0;
        CommandTarget.SelectedIndex = 0;

        AddHostLog($"Runtime directory resolved to {RuntimeRoot}.", false);
        ValidateRuntime();
        Loaded += MainWindow_Loaded;
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    public ObservableCollection<ServerProcessController> Servers { get; }
    public ICollectionView LogView => _logView;
    public string RuntimeRoot { get; }
    public string RuntimePathText { get; }
    public string LogCountText => $"{_logs.Count:N0} log lines";

    public string OverallStatusText
    {
        get => _overallStatusText;
        private set
        {
            _overallStatusText = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(OverallStatusText)));
        }
    }

    public Brush OverallStatusBrush
    {
        get => _overallStatusBrush;
        private set
        {
            _overallStatusBrush = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(OverallStatusBrush)));
        }
    }

    private ServerProcessController CreateServer(
        string id,
        string displayName,
        string executable,
        string workingDirectory,
        string stopCommand,
        string color,
        int port,
        int order)
    {
        var definition = new ServerDefinition(
            id,
            displayName,
            executable,
            workingDirectory,
            stopCommand,
            color,
            port,
            order);
        return new ServerProcessController(definition, RuntimeRoot, AddServerLog);
    }

    private static string FindRuntimeRoot()
    {
        var environmentPath = Environment.GetEnvironmentVariable("EO_SERVER_RUNTIME");
        if (!string.IsNullOrWhiteSpace(environmentPath))
        {
            return Path.GetFullPath(environmentPath);
        }

        var current = new DirectoryInfo(AppContext.BaseDirectory);
        while (current is not null)
        {
            var candidate = Path.Combine(current.FullName, "Runtime");
            if (File.Exists(Path.Combine(candidate, "GlobalConfig.ini")))
            {
                return candidate;
            }

            current = current.Parent;
        }

        return Path.GetFullPath(
            Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..", "Runtime"));
    }

    private void ValidateRuntime()
    {
        if (!File.Exists(Path.Combine(RuntimeRoot, "GlobalConfig.ini")))
        {
            AddHostLog("GlobalConfig.ini was not found in the runtime directory.", true);
        }

        foreach (var server in Servers.Where(server => !File.Exists(server.ExecutablePath)))
        {
            AddServerLog(server, $"Missing executable: {server.ExecutablePath}", true);
        }
    }

    private async void MainWindow_Loaded(object sender, RoutedEventArgs e)
    {
        Loaded -= MainWindow_Loaded;
        await StartAllAsync();
    }

    private async void StartAll_Click(object sender, RoutedEventArgs e)
    {
        await StartAllAsync();
    }

    private async Task StartAllAsync()
    {
        if (_startAllInProgress || Servers.All(server => server.IsRunning))
        {
            return;
        }

        _startAllInProgress = true;
        try
        {
            if (!await EnsureDatabaseReadyAsync())
            {
                return;
            }

            var startedServers = new List<ServerProcessController>();
            foreach (var server in Servers.OrderBy(
                         server => server.Definition.StartOrder))
            {
                if (server.IsRunning)
                {
                    continue;
                }

                if (IsPortListening(server.Definition.Port))
                {
                    AddHostLog(
                        $"Cannot start {server.DisplayName}; port {server.Definition.Port} is already in use.",
                        true);
                    await RollBackStartedServersAsync(startedServers);
                    return;
                }

                await server.StartAsync();
                if (await WaitForReadyAsync(server, TimeSpan.FromSeconds(15)))
                {
                    AddHostLog(
                        $"{server.DisplayName} is listening on port {server.Definition.Port}.",
                        false);
                    startedServers.Add(server);
                    continue;
                }

                AddHostLog(
                    $"Start All stopped because {server.DisplayName} did not become ready on port {server.Definition.Port}.",
                    true);
                await server.StopAsync(TimeSpan.FromSeconds(2));
                await RollBackStartedServersAsync(startedServers);
                return;
            }
        }
        finally
        {
            _startAllInProgress = false;
        }
    }

    private async Task<bool> EnsureDatabaseReadyAsync()
    {
        if (_databaseReady)
        {
            return true;
        }

        OverallStatusText = "Checking MySQL 8";
        OverallStatusBrush = Brushes.Goldenrod;
        AddHostLog("Checking the configured MySQL 8 database.", false);

        try
        {
            var result = await Task.Run(
                () => DatabaseBootstrapper.EnsureReady(RuntimeRoot));
            _databaseReady = true;
            AddHostLog(result.Message, false);
            return true;
        }
        catch (Exception exception)
        {
            OverallStatusText = "Database unavailable";
            OverallStatusBrush = Brushes.IndianRed;
            AddHostLog($"Database startup check failed: {exception.Message}", true);
            return false;
        }
    }

    private static async Task<bool> WaitForReadyAsync(
        ServerProcessController server,
        TimeSpan timeout)
    {
        var deadline = DateTime.UtcNow + timeout;
        while (DateTime.UtcNow < deadline)
        {
            if (!server.IsRunning)
            {
                return false;
            }

            if (IsPortListening(server.Definition.Port))
            {
                return true;
            }

            await Task.Delay(250);
        }

        return false;
    }

    private static bool IsPortListening(int port)
    {
        return IPGlobalProperties
            .GetIPGlobalProperties()
            .GetActiveTcpListeners()
            .Any(endpoint => endpoint.Port == port);
    }

    private static async Task RollBackStartedServersAsync(
        IEnumerable<ServerProcessController> servers)
    {
        foreach (var server in servers.Reverse())
        {
            await server.StopAsync(TimeSpan.FromSeconds(5));
        }
    }

    private async void StopAll_Click(object sender, RoutedEventArgs e)
    {
        await StopAllAsync();
    }

    private async void StartServer_Click(object sender, RoutedEventArgs e)
    {
        if (((Button)sender).Tag is ServerProcessController server)
        {
            await server.StartAsync();
        }
    }

    private async void StopServer_Click(object sender, RoutedEventArgs e)
    {
        if (((Button)sender).Tag is ServerProcessController server)
        {
            await server.StopAsync(TimeSpan.FromSeconds(5));
        }
    }

    private async Task StopAllAsync()
    {
        foreach (var server in Servers
                     .OrderByDescending(server => server.Definition.StartOrder))
        {
            await server.StopAsync(TimeSpan.FromSeconds(5));
        }
    }

    private async void SendCommand_Click(object sender, RoutedEventArgs e)
    {
        await SendCurrentCommandAsync();
    }

    private async void CommandBox_KeyDown(object sender, KeyEventArgs e)
    {
        if (e.Key == Key.Enter)
        {
            e.Handled = true;
            await SendCurrentCommandAsync();
        }
    }

    private async Task SendCurrentCommandAsync()
    {
        var command = CommandBox.Text.Trim();
        if (command.Length == 0 ||
            CommandTarget.SelectedItem is not ServerProcessController server)
        {
            return;
        }

        CommandBox.Clear();
        await server.SendCommandAsync(command);
    }

    private void ClearLogs_Click(object sender, RoutedEventArgs e)
    {
        _logs.Clear();
        AddHostLog("Console cleared.", false);
    }

    private void LogFilter_Changed(object sender, EventArgs e)
    {
        _logView?.Refresh();
    }

    private bool FilterLog(object item)
    {
        if (item is not LogEntry entry)
        {
            return false;
        }

        var selectedServer = ServerFilter?.SelectedItem as string;
        if (!string.IsNullOrWhiteSpace(selectedServer) &&
            selectedServer != "All servers" &&
            entry.ServerName != selectedServer)
        {
            return false;
        }

        var query = SearchBox?.Text.Trim();
        return string.IsNullOrWhiteSpace(query) ||
               entry.Message.Contains(query, StringComparison.OrdinalIgnoreCase) ||
               entry.ServerName.Contains(query, StringComparison.OrdinalIgnoreCase);
    }

    private void AddServerLog(
        ServerProcessController server,
        string message,
        bool isError)
    {
        var normalizedMessage = ChildTimestampPattern.Replace(message, string.Empty);
        var translatedMessage = LogTranslator.Translate(normalizedMessage);
        AddLog(new LogEntry
        {
            Timestamp = DateTime.Now,
            ServerId = server.Id,
            ServerName = server.DisplayName,
            Message = translatedMessage,
            ServerBrush = server.AccentBrush,
            MessageBrush = SelectMessageBrush(translatedMessage, isError)
        });
    }

    private void AddHostLog(string message, bool isError)
    {
        var hostBrush = new SolidColorBrush(Color.FromRgb(151, 160, 168));
        hostBrush.Freeze();
        AddLog(new LogEntry
        {
            Timestamp = DateTime.Now,
            ServerId = "HOST",
            ServerName = "Unified Host",
            Message = message,
            ServerBrush = hostBrush,
            MessageBrush = SelectMessageBrush(message, isError)
        });
    }

    private void AddLog(LogEntry entry)
    {
        if (!Dispatcher.CheckAccess())
        {
            Dispatcher.BeginInvoke(() => AddLog(entry));
            return;
        }

        _logs.Add(entry);
        while (_logs.Count > MaximumLogEntries)
        {
            _logs.RemoveAt(0);
        }
    }

    private static Brush SelectMessageBrush(string message, bool isError)
    {
        if (isError ||
            message.Contains("error", StringComparison.OrdinalIgnoreCase) ||
            message.Contains("fail", StringComparison.OrdinalIgnoreCase) ||
            message.Contains("exception", StringComparison.OrdinalIgnoreCase))
        {
            return Brushes.LightCoral;
        }

        if (message.Contains("warning", StringComparison.OrdinalIgnoreCase) ||
            message.Contains("reconnect", StringComparison.OrdinalIgnoreCase))
        {
            return Brushes.Khaki;
        }

        if (message.Contains("success", StringComparison.OrdinalIgnoreCase) ||
            message.Contains("started", StringComparison.OrdinalIgnoreCase))
        {
            return Brushes.LightGreen;
        }

        return Brushes.Gainsboro;
    }

    private void LogsOnCollectionChanged(
        object? sender,
        NotifyCollectionChangedEventArgs e)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(LogCountText)));

        if (AutoScrollCheckBox.IsChecked == true && _logs.Count > 0)
        {
            LogList.ScrollIntoView(_logs[^1]);
        }
    }

    private void ServerOnPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName is nameof(ServerProcessController.IsRunning)
            or nameof(ServerProcessController.IsBusy))
        {
            Dispatcher.BeginInvoke(UpdateOverallStatus);
        }
    }

    private void UpdateOverallStatus()
    {
        var running = Servers.Count(server => server.IsRunning);
        var busy = Servers.Count(server => server.IsBusy);

        if (busy > 0)
        {
            OverallStatusText = $"{busy} server operation(s) in progress";
            OverallStatusBrush = Brushes.Khaki;
        }
        else if (running == Servers.Count)
        {
            OverallStatusText = "All servers running";
            OverallStatusBrush = Brushes.LightGreen;
        }
        else if (running > 0)
        {
            OverallStatusText = $"{running} of {Servers.Count} servers running";
            OverallStatusBrush = Brushes.Khaki;
        }
        else
        {
            OverallStatusText = "All servers stopped";
            OverallStatusBrush = Brushes.Gray;
        }
    }

    private async void Window_Closing(object? sender, CancelEventArgs e)
    {
        if (_isClosing || Servers.All(server => !server.IsRunning))
        {
            foreach (var server in Servers)
            {
                server.Dispose();
            }

            return;
        }

        e.Cancel = true;
        var result = MessageBox.Show(
            this,
            "Stop all server processes and exit?",
            "Eudemons Unified Server",
            MessageBoxButton.YesNo,
            MessageBoxImage.Warning);
        if (result != MessageBoxResult.Yes)
        {
            return;
        }

        IsEnabled = false;
        await StopAllAsync();
        _isClosing = true;
        Close();
    }
}
