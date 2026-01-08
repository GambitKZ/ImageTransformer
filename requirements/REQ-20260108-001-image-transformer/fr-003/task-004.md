# Task 004: Add Folder Validation

## Objective
Validate that the input folder exists and is accessible before attempting to scan for images.

## Deliverables
- Add input folder validation logic
- Enhance configuration validation
- Provide meaningful error messages to user

## Implementation Details
1. Add validation in Program.cs after configuration is loaded:
   - Check if input folder path exists using `Directory.Exists()`
   - Check if folder is readable (attempt minimal access)
   - Display error and exit gracefully if validation fails
2. Validation timing:
   - Occurs after configuration loading (FR-001)
   - Occurs before user is prompted to start (FR-002)
3. Error messages:
   - "Input folder not found: {path}"
   - "Input folder is not accessible: {path}"

## Acceptance Criteria
- [ ] Input folder existence validated before scanning
- [ ] Input folder accessibility checked
- [ ] User receives clear error message if folder missing or inaccessible
- [ ] Application exits gracefully on validation failure
- [ ] Configuration provides input path from appsettings.json
