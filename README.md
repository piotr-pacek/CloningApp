# CloningApp

A simple C# console application that synchronizes two folders: **source** and **replica**.  
The synchronization ensures that the replica folder always matches the content of the source folder.

## Features

- One-way synchronization: **replica** is updated to exactly match **source**
- Supports recursive synchronization (subdirectories included)
- Detects changes using **MD5 hash comparison**
- Copies new and updated files
- Deletes files and directories that no longer exist in the source
- Logs operations (to both console and log file)
- Synchronization runs periodically with user-defined interval

## Requirements

- .NET 6.0 (or later)

## Usage

Build the project, then run the executable with 4 arguments:

    CloningApp.exe [source_path] [replica_path] [interval_seconds] [log_file_path]

Example:

    CloningApp.exe "C:\Source" "D:\Replica" 10 "C:\Logs\sync.log"

This will:
- Synchronize `D:\Replica` with `C:\Source` every 10 seconds
- Write logs both to console and to `C:\Logs\sync.log`

## Error Handling

- Application exits if source directory is missing
- Errors during file copy or deletion are logged without stopping synchronization
- Interval must be a positive integer

## Logging

Every synchronization action is logged, e.g.:

    Copied file from C:\Source\example.txt to D:\Replica\example.txt
    Replaced file D:\Replica\data.json with C:\Source\data.json
    Deleted file D:\Replica\old.txt
    Deleted directory D:\Replica\Obsolete

## Notes

- Synchronization is periodic (based on the interval argument)
- Only built-in .NET libraries are used (no third-party sync libraries)
- Hashing is done with MD5 to detect file changes
