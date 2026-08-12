# MySQL 8 Database

The server uses the installed MySQL 8 instance. Do not install the historical
MySQL package from the preserved files or any legacy version named in old
readme files.

`Initialize-MySql8.ps1` reads connection settings from the writable runtime
configuration, verifies that the target is MySQL 8, imports the packaged
database-neutral UTF-8/utf8mb4 schema, and reports the resulting tables. It
then applies
`MySQL8\legion.mysql8.migration.sql`, which adds durable player IDs, separate
gold/emoney contribution fields, membership uniqueness, and a leader-member
backfill required by the legion server.

Run from `Workspace/Server`:

```powershell
powershell -NoProfile -ExecutionPolicy Bypass -File `
  .\Database\Initialize-MySql8.ps1
```

The initializer refuses to replace a non-empty database unless `-Force` is
provided explicitly. Normal release setup has no dependency on `Original/` or
the development repository. To regenerate the packaged schema during
development, pass the preserved CP936 dump explicitly with `-SourceDump`; the
preserved input is never changed. The packaged schema already contains the
family, legion, and friend migrations. Those migrations run automatically
only when regenerating from an older preserved source dump.

For an existing database created before the legion update, run
`legion.mysql8.migration.sql` once against that database before deploying the
matching DBServer and MapServer binaries.

For an existing database created before wardrobe mount package support, run
`wardrobe_mount.mysql8.migration.sql` once. It changes `cq_item.postion` from
signed to unsigned `TINYINT`, matching the protocol's byte-sized package
positions and allowing the definitive client's mount package position 146.
