#Requires -Version 5.1
<#
.SYNOPSIS
    Install script for getmajorcolors on Windows.
.DESCRIPTION
    Downloads the latest release binary from GitHub and places it on PATH.
#>

param(
    [string]$Repo = "tenhauser/getmajorcolor",
    [string]$InstallDir = "$env:LOCALAPPDATA\getmajorcolors"
)

$ErrorActionPreference = "Stop"

$arch = if ([Environment]::Is64BitOperatingSystem) { "x64" } else { "arm64" }
$asset = "getmajorcolors-win-$arch.zip"
$url = "https://github.com/$Repo/releases/latest/download/$asset"

$tempDir = Join-Path $env:TEMP ([System.Guid]::NewGuid().ToString())
New-Item -ItemType Directory -Path $tempDir -Force | Out-Null
New-Item -ItemType Directory -Path $InstallDir -Force | Out-Null

try {
    $zipPath = Join-Path $tempDir $asset
    Write-Host "Downloading $asset..."
    Invoke-RestMethod -Uri $url -OutFile $zipPath

    Write-Host "Extracting..."
    Expand-Archive -Path $zipPath -DestinationPath $InstallDir -Force

    $userPath = [Environment]::GetEnvironmentVariable("Path", "User")
    if ($userPath -notlike "*$InstallDir*") {
        [Environment]::SetEnvironmentVariable("Path", "$userPath;$InstallDir", "User")
        Write-Host "Added $InstallDir to user PATH. Restart your terminal to use getmajorcolors."
    }

    Write-Host "Installed to $InstallDir\getmajorcolors.exe"
}
finally {
    Remove-Item -Recurse -Force $tempDir -ErrorAction SilentlyContinue
}
