# REQ-20260108-001:FR-004 — Convert PNG Images to JPEG Based on Size Threshold

# Design Considerations
- Use image library to load PNG and save as JPEG.
- Check file size before.

# Data Flow
1. For PNG, get file size.
2. If >= threshold, load image, save as JPEG.
3. Else, copy file.

# Affected Components (Projects, Services, Classes)
- ImageProcessor

# Dependencies
- NetVips or ImageMagick.NET

# Implementation Steps
1. Choose library.
2. Implement conversion logic.