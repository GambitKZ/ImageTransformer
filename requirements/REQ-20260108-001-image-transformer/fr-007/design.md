# REQ-20260108-001:FR-007 — Display Processing Progress and Summary

# Design Considerations
- Use Spectre.Console Progress for bar.
- Collect stats for summary.

# Data Flow
1. Start progress.
2. Update per image.
3. End with summary.

# Affected Components (Projects, Services, Classes)
- Program.cs

# Dependencies
- Spectre.Console

# Implementation Steps
1. Wrap processing in progress context.
2. Display summary.