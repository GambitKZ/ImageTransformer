# REQ-20260108-001:FR-001 — Load Application Configuration

# User Story
As a user, I want the application to load configuration from appsettings.json and UserSecrets so that settings are configurable and secure.

# Acceptance Criteria (Given / When / Then)
- Given the application is starting
- When configuration is loaded
- Then appsettings.json and UserSecrets are read and merged

# Test Outline
1. Start the application.
2. Verify configuration values are loaded.
3. Verify UserSecrets override if set.

# Rationale
This allows customization and security for settings.