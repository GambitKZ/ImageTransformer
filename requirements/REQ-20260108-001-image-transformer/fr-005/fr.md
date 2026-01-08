# REQ-20260108-001:FR-005 — Resize JPEG Images Iteratively if Size Exceeds Threshold

# User Story
As a user, I want JPEGs resized if too large, iteratively until size ok.

# Acceptance Criteria (Given / When / Then)
- Given JPEG
- When size > target
- Then resize by step %, check again, repeat

# Test Outline
1. Large JPEG -> resized until <= target.

# Rationale
Compresses large images.