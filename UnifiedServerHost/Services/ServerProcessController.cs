using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows.Media;
using Eudemons.UnifiedServer.Models;

namespace Eudemons.UnifiedServer.Services;

public sealed class ServerProcessController : INotifyPropertyChanged, IDisposable
{
    private readonly string _runtimeRoot;
    private readonly Action<ServerProcessController, string, bool> _log;
    private Process? _process;
    private string _status = "Stopped";
    private bool _isRunning;
    private bool _isBusy;

    public ServerProcessController(
        ServerDefinition definition,
        string runtimeRoot,
        Action<ServerProcessController, string, bool> log)
    {
        Definition = definition;
        _runtimeRoot = runtimeRoot;
        _log = log;
        AccentBrush = new SolidColorBrush(
            (Color)ColorConverter.ConvertFromString(definition.Color));
        AccentBrush.Freeze();
        RefreshProcessState(logDiscovery: false);
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    public ServerDefinition Definition { get; }
    public string Id => Definition.Id;
    public string DisplayName => Definition.DisplayName;
    public Brush AccentBrush { get; }

    public string Status
    {
        get => _status;
        private set
        {
            if (_status == value)
            {
                return;
            }

            _status = value;
            OnPropertyChanged();
        }
    }

    public bool IsRunning
    {
        get => _isRunning;
        private set
        {
            if (_isRunning == value)
            {
                return;
            }

            _isRunning = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(CanStart));
            OnPropertyChanged(nameof(CanStop));
        }
    }

    public bool IsBusy
    {
        get => _isBusy;
        private set
        {
            if (_isBusy == value)
            {
                return;
            }

            _isBusy = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(CanStart));
            OnPropertyChanged(nameof(CanStop));
        }
    }

    public bool CanStart => !IsRunning && !IsBusy;
    public bool CanStop => IsRunning && !IsBusy;

    public string WorkingDirectory =>
        Path.Combine(_runtimeRoot, Definition.WorkingDirectoryName);

    public string ExecutablePath =>
        Path.Combine(WorkingDirectory, Definition.ExecutableName);

    public Task StartAsync()
    {
        if (IsBusy)
        {
            return Task.CompletedTask;
        }

        if (RefreshProcessState(logDiscovery: true))
        {
            return Task.CompletedTask;
        }

        IsBusy = true;
        Status = "Starting";

        try
        {
            if (!File.Exists(ExecutablePath))
            {
                throw new FileNotFoundException(
                    $"Server executable was not found: {ExecutablePath}",
                    ExecutablePath);
            }

            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            var outputEncoding = Encoding.GetEncoding(
                936,
                EncoderFallback.ReplacementFallback,
                DecoderFallback.ReplacementFallback);

            var startInfo = new ProcessStartInfo
            {
                FileName = ExecutablePath,
                WorkingDirectory = WorkingDirectory,
                UseShellExecute = false,
                CreateNoWindow = true,
                RedirectStandardInput = true,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                StandardOutputEncoding = outputEncoding,
                StandardErrorEncoding = outputEncoding
            };

            var process = new Process
            {
                StartInfo = startInfo,
                EnableRaisingEvents = true
            };

            process.OutputDataReceived += (_, args) =>
            {
                if (!string.IsNullOrWhiteSpace(args.Data))
                {
                    _log(this, args.Data, false);
                }
            };
            process.ErrorDataReceived += (_, args) =>
            {
                if (!string.IsNullOrWhiteSpace(args.Data))
                {
                    _log(this, args.Data, true);
                }
            };
            process.Exited += ProcessOnExited;

            if (!process.Start())
            {
                process.Dispose();
                throw new InvalidOperationException("Process.Start returned false.");
            }

            _process = process;
            process.BeginOutputReadLine();
            process.BeginErrorReadLine();
            IsRunning = true;
            Status = $"Running  PID {process.Id}";
            _log(this, $"Started {Definition.ExecutableName} (PID {process.Id}).", false);
        }
        catch (Exception ex)
        {
            Status = "Start failed";
            _log(this, ex.Message, true);
            DisposeProcess();
        }
        finally
        {
            IsBusy = false;
        }

        return Task.CompletedTask;
    }

    public async Task StopAsync(TimeSpan gracefulTimeout)
    {
        if (IsBusy)
        {
            return;
        }

        IsBusy = true;
        Status = "Stopping";

        try
        {
            var process = _process;
            if (IsLiveProcess(process))
            {
                _log(this, $"Sending '{Definition.StopCommand}' shutdown command.", false);
                try
                {
                    await process!.StandardInput.WriteLineAsync(Definition.StopCommand);
                    await process.StandardInput.FlushAsync();

                    using var timeout = new CancellationTokenSource(gracefulTimeout);
                    await process.WaitForExitAsync(timeout.Token);
                }
                catch (OperationCanceledException)
                {
                    _log(
                        this,
                        "Graceful shutdown timed out; terminating the exact process tree.",
                        true);
                }
                catch (InvalidOperationException)
                {
                    // The tracked process exited between the state check and write.
                }
                catch (Exception ex)
                {
                    _log(
                        this,
                        $"Graceful shutdown failed: {ex.Message}. Terminating the exact process tree.",
                        true);
                }
            }

            DisposeProcess();
            await TerminateMatchingProcessesAsync();

            var survivors = FindMatchingProcesses();
            try
            {
                if (survivors.Count == 0)
                {
                    MarkStopped();
                    _log(this, "Stop verified; no matching service process remains.", false);
                }
                else
                {
                    IsRunning = true;
                    Status = "Stop failed";
                    _log(
                        this,
                        "Stop verification failed; matching PID(s) still running: " +
                        string.Join(", ", survivors.Select(item => item.Id)),
                        true);
                }
            }
            finally
            {
                foreach (var survivor in survivors)
                {
                    survivor.Dispose();
                }
            }
        }
        finally
        {
            IsBusy = false;
        }
    }

    public async Task SendCommandAsync(string command)
    {
        var process = _process;
        if (process is null || process.HasExited)
        {
            _log(this, "Command was not sent because the server is stopped.", true);
            return;
        }

        try
        {
            await process.StandardInput.WriteLineAsync(command);
            await process.StandardInput.FlushAsync();
            _log(this, $"> {command}", false);
        }
        catch (Exception ex)
        {
            _log(this, $"Command failed: {ex.Message}", true);
        }
    }

    private void ProcessOnExited(object? sender, EventArgs e)
    {
        var exitCode = -1;
        try
        {
            exitCode = ((Process)sender!).ExitCode;
        }
        catch
        {
            // The process handle may already be disposed during application exit.
        }

        _log(this, $"Process exited with code {exitCode}.", exitCode != 0);
        if (!IsBusy)
        {
            DisposeProcess();
            RefreshProcessState(logDiscovery: false);
        }
    }

    public bool RefreshProcessState(bool logDiscovery = false)
    {
        var matches = FindMatchingProcesses();
        try
        {
            if (matches.Count == 0)
            {
                IsRunning = false;
                Status = "Stopped";
                return false;
            }

            IsRunning = true;
            Status = matches.Count == 1
                ? $"Running  PID {matches[0].Id}"
                : $"Running  {matches.Count} processes";
            if (logDiscovery)
            {
                _log(
                    this,
                    "Already running from the exact configured path; PID(s): " +
                    string.Join(", ", matches.Select(item => item.Id)) + ".",
                    false);
            }
            return true;
        }
        finally
        {
            foreach (var match in matches)
            {
                match.Dispose();
            }
        }
    }

    private async Task TerminateMatchingProcessesAsync()
    {
        var matches = FindMatchingProcesses();
        try
        {
            foreach (var match in matches)
            {
                try
                {
                    _log(
                        this,
                        $"Terminating verified {Definition.ExecutableName} PID {match.Id}.",
                        false);
                    match.Kill(entireProcessTree: true);
                }
                catch (InvalidOperationException)
                {
                    continue;
                }
                catch (Exception ex)
                {
                    _log(
                        this,
                        $"Could not terminate PID {match.Id}: {ex.Message}",
                        true);
                    continue;
                }

                try
                {
                    using var timeout = new CancellationTokenSource(
                        TimeSpan.FromSeconds(10));
                    await match.WaitForExitAsync(timeout.Token);
                }
                catch (OperationCanceledException)
                {
                    _log(
                        this,
                        $"PID {match.Id} did not exit within 10 seconds.",
                        true);
                }
                catch (InvalidOperationException)
                {
                    // The process exited before the wait began.
                }
            }
        }
        finally
        {
            foreach (var match in matches)
            {
                match.Dispose();
            }
        }
    }

    private List<Process> FindMatchingProcesses()
    {
        var matches = new List<Process>();
        var processName = Path.GetFileNameWithoutExtension(ExecutablePath);
        foreach (var process in Process.GetProcessesByName(processName))
        {
            try
            {
                if (!process.HasExited &&
                    PathsMatch(process.MainModule?.FileName, ExecutablePath))
                {
                    matches.Add(process);
                    continue;
                }
            }
            catch (InvalidOperationException)
            {
                // The process exited during discovery.
            }
            catch (Win32Exception)
            {
                // A process whose executable path cannot be verified is never a target.
            }
            catch (NotSupportedException)
            {
                // A process whose executable path cannot be verified is never a target.
            }

            process.Dispose();
        }
        return matches;
    }

    private static bool IsLiveProcess(Process? process)
    {
        if (process is null)
        {
            return false;
        }

        try
        {
            return !process.HasExited;
        }
        catch (InvalidOperationException)
        {
            return false;
        }
    }

    private static bool PathsMatch(string? first, string second)
    {
        if (string.IsNullOrWhiteSpace(first))
        {
            return false;
        }

        try
        {
            return string.Equals(
                Path.TrimEndingDirectorySeparator(Path.GetFullPath(first)),
                Path.TrimEndingDirectorySeparator(Path.GetFullPath(second)),
                StringComparison.OrdinalIgnoreCase);
        }
        catch (Exception ex) when (
            ex is ArgumentException or NotSupportedException or PathTooLongException)
        {
            return false;
        }
    }

    private void MarkStopped()
    {
        IsRunning = false;
        Status = "Stopped";
        DisposeProcess();
    }

    private void DisposeProcess()
    {
        var process = Interlocked.Exchange(ref _process, null);
        if (process is null)
        {
            return;
        }

        process.Exited -= ProcessOnExited;
        process.Dispose();
    }

    private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    public void Dispose()
    {
        DisposeProcess();
    }
}
