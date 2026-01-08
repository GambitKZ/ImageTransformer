# REQ-20260108-001:FR-002 — Display Processing Progress with File Details

# Design Considerations
- Use Spectre.Console Progress for the overall bar.
- Use Status or Markup to show current file and per-file results.
- Format result messages clearly, including original size, actions taken, and final size.

# Data Flow
1. Start processing after configuration.
2. Initialize progress bar with total files.
3. For each file, update status to show current file.
4. After processing, display result message.
5. Update progress bar.

# Affected Components (Projects, Services, Classes)
- Program.cs or main processing loop

# Dependencies
- Spectre.Console

# Implementation Steps
1. Install Spectre.Console.
2. Wrap the processing loop in AnsiConsole.Progress.
3. Use context.AddTask for the bar.
4. Use AnsiConsole.Markup for status and results.