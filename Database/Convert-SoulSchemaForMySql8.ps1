param(
    [Parameter(Mandatory = $true)]
    [string]$SourceDump,
    [string]$OutputPath = (Join-Path $PSScriptRoot 'MySQL8\soul.mysql8.sql'),
    [string]$Database = 'soul'
)

$ErrorActionPreference = 'Stop'

if ($Database -notmatch '^[A-Za-z0-9_]+$') {
    throw "Invalid database name: $Database"
}

$sourcePath = (Resolve-Path -LiteralPath $SourceDump).Path
$sourceEncoding = [Text.Encoding]::GetEncoding(936)
$sourceSql = [IO.File]::ReadAllText($sourcePath, $sourceEncoding)

# The preserved dump is CP936. Decode it once, then emit explicit UTF-8 for
# MySQL 8 while retaining its table definitions and data.
$convertedSql = $sourceSql -replace `
    'DEFAULT\s+CHARSET=(?:utf8|gb2312)', `
    'DEFAULT CHARACTER SET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci'

$header = @"
-- Generated from the immutable preserved soul.sql dump.
-- Target: MySQL 8, UTF-8 input and utf8mb4 tables.
SET NAMES utf8mb4;
CREATE DATABASE IF NOT EXISTS ``$Database``
  CHARACTER SET utf8mb4
  COLLATE utf8mb4_0900_ai_ci;
USE ``$Database``;

"@

$footer = @"

SET FOREIGN_KEY_CHECKS=1;
"@

$outputDirectory = Split-Path -Parent $OutputPath
[IO.Directory]::CreateDirectory($outputDirectory) | Out-Null
$utf8WithoutBom = New-Object Text.UTF8Encoding($false)
[IO.File]::WriteAllText($OutputPath, $header + $convertedSql.TrimEnd() + $footer, $utf8WithoutBom)

Write-Output "Generated MySQL 8 schema: $OutputPath"
