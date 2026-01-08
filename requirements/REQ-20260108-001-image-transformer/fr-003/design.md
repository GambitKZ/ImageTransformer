# REQ-20260108-001:FR-003 — Scan Input Folder for Images

# Design Considerations
- Use Directory.EnumerateFiles with extensions.
- Handle subfolders if needed (but probably not).

# Data Flow
1. Get input path from config.
2. Enumerate files.
3. Store list.

# Affected Components (Projects, Services, Classes)
- ImageProcessor or Program.cs

# Dependencies
- System.IO

# Implementation Steps
1. Read config for input path.
2. Use Directory.GetFiles.