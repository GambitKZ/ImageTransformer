# REQ-20260108-001:FR-006 — Save Processed Images to Output Folder

# Design Considerations
- Use same name or add suffix.
- Ensure output folder exists.

# Data Flow
1. After processing, save to output path.

# Affected Components (Projects, Services, Classes)
- File system

# Dependencies
- System.IO

# Implementation Steps
1. Create output dir if needed.
2. Save file.