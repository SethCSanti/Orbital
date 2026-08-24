---
name: avoid-claudy-styles
description: Create focused CSS and template styling changes that avoid generic AI-looking layouts, use credible professional typography, preserve the product's visual identity, and maintain accessible responsive behavior. Use for styling, layout, visual polish, page redesign, or CSS changes.
---

# Avoid claudy styles

## Design direction

- Start from the product's audience, purpose, and existing identity before changing its layout or palette.
- Use IBM Plex Sans for headings, navigation, labels, and controls; use Source Sans 3 for body copy, metadata, and form text when the project does not already have a stronger established brand system.
- Do not use Geist Pixel or Rubik. Avoid novelty, pixel, handwritten, or overly fashionable display faces for professional work.
- Favor credible, restrained typography with clear hierarchy, comfortable measures, and sufficient weight and contrast.
- Prefer continuous page structure, practical bands, dividers, and purposeful repeated-item cards over decorative card stacks.
- Keep dark mode, light mode, hover, focus, active, disabled, selected, and error states readable when those states exist.
- Reuse CSS variables and local design tokens. Add colors only when existing tokens cannot express a required state.

## Avoid

- Generic SaaS heroes, faux editorial ticker bars, oversized italic serif headlines, extreme tracking, neon outlines, and decorative gradients.
- Glassmorphism, glow-heavy shadows, and rounded-rectangle overload. Use borders, spacing, and hierarchy before adding decoration.
- Beige, cream, or rusty-orange dominance unless it belongs to the product identity.
- Turning every section, sentence, stat, or action into its own floating card or pill.
- Inline styles unless a value is truly dynamic.
- New stylesheets when the existing stylesheet can hold the change cleanly.

## Implementation rules

1. Inspect the current templates, components, and styles before editing.
2. Identify what already communicates the product's identity and preserve it.
3. Update existing tokens and shared component styles before adding page-specific rules.
4. Add semantic class hooks when they improve reuse, responsiveness, accessibility, or maintainability.
5. Keep selectors narrow and place rules near related styles.
6. Maintain responsive behavior and prevent overflow at compact widths.
7. Preserve visible keyboard focus and honor reduced-motion preferences.
8. Honor requests for minimal or no testing; otherwise run the smallest relevant check.

## Output style

Report the exact files changed and the purpose of each change. Keep explanations compact and practical.
