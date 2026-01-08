# REQ-20260108-001:FR-001 — Load Application Configuration

# Design Considerations
- Use IConfigurationBuilder to load from JSON and UserSecrets.
- Ensure environment-specific files if needed.

# Data Flow
1. Program.cs builds configuration.
2. Loads appsettings.json.
3. Loads UserSecrets.
4. Configuration ready.

# Affected Components (Projects, Services, Classes)
- Program.cs
- appsettings.json

# Dependencies
- Microsoft.Extensions.Configuration
- Microsoft.Extensions.Configuration.Json
- Microsoft.Extensions.Configuration.UserSecrets

# Implementation Steps
1. Add packages.
2. In Program.cs, create builder and build config.
3. Use config in app.