# REQ-20260108-001:FR-005 — Resize JPEG Images Iteratively if Size Exceeds Threshold

# Design Considerations
- Resize by reducing width/height by percentage.
- Loop until size <= target or max iterations.
- Use original for each resize.

# Data Flow
1. Load JPEG.
2. Get size.
3. If > target, resize, save temp, check size, repeat.
4. Save final.

# Affected Components (Projects, Services, Classes)
- ImageProcessor

# Dependencies
- Image library

# Implementation Steps
1. Implement resize loop.