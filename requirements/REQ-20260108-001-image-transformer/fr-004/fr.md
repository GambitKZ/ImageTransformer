# REQ-20260108-001:FR-004 — Convert PNG Images to JPEG Based on Size Threshold

# User Story
As a user, I want PNG images converted to JPEG if above threshold, else copied.

# Acceptance Criteria (Given / When / Then)
- Given PNG image
- When processing
- Then if size >= threshold, convert to JPEG
- Then if size < threshold, copy as-is

# Test Outline
1. PNG > threshold -> converted.
2. PNG < threshold -> copied.

# Rationale
Optimizes PNGs.