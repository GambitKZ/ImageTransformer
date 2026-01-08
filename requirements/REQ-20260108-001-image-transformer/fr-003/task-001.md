# Task 001: Create IImageScanner Interface

## Objective
Define a contract for image scanning functionality that will be implemented and injected throughout the application.

## Deliverables
- Create `IImageScanner` interface in a new file
- Define method signatures for folder scanning and image discovery
- Include XML doc comments explaining the contract

## Implementation Details
1. Create `src/ImageTransformer.Console/Services/IImageScanner.cs`
2. Define interface with:
   - `Task<IEnumerable<string>> ScanAsync(string inputPath)` - Returns list of discovered image file paths
   - Support for .png, .jpg, .jpeg extensions
3. Add XML documentation for the interface and method

## Acceptance Criteria
- [ ] IImageScanner interface created with ScanAsync method
- [ ] Method returns full file paths of discovered images
- [ ] XML doc comments present and clear
- [ ] Interface is in Services namespace
