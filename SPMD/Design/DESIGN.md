---
name: Clinical Precision
colors:
  surface: '#f7f9fb'
  surface-dim: '#d8dadc'
  surface-bright: '#f7f9fb'
  surface-container-lowest: '#ffffff'
  surface-container-low: '#f2f4f6'
  surface-container: '#eceef0'
  surface-container-high: '#e6e8ea'
  surface-container-highest: '#e0e3e5'
  on-surface: '#191c1e'
  on-surface-variant: '#444651'
  inverse-surface: '#2d3133'
  inverse-on-surface: '#eff1f3'
  outline: '#757682'
  outline-variant: '#c5c5d3'
  surface-tint: '#4059aa'
  primary: '#00236f'
  on-primary: '#ffffff'
  primary-container: '#1e3a8a'
  on-primary-container: '#90a8ff'
  inverse-primary: '#b6c4ff'
  secondary: '#505f76'
  on-secondary: '#ffffff'
  secondary-container: '#d0e1fb'
  on-secondary-container: '#54647a'
  tertiary: '#222a3e'
  on-tertiary: '#ffffff'
  tertiary-container: '#384055'
  on-tertiary-container: '#a4acc5'
  error: '#ba1a1a'
  on-error: '#ffffff'
  error-container: '#ffdad6'
  on-error-container: '#93000a'
  primary-fixed: '#dce1ff'
  primary-fixed-dim: '#b6c4ff'
  on-primary-fixed: '#00164e'
  on-primary-fixed-variant: '#264191'
  secondary-fixed: '#d3e4fe'
  secondary-fixed-dim: '#b7c8e1'
  on-secondary-fixed: '#0b1c30'
  on-secondary-fixed-variant: '#38485d'
  tertiary-fixed: '#dae2fd'
  tertiary-fixed-dim: '#bec6e0'
  on-tertiary-fixed: '#131b2e'
  on-tertiary-fixed-variant: '#3f465c'
  background: '#f7f9fb'
  on-background: '#191c1e'
  surface-variant: '#e0e3e5'
typography:
  headline-lg:
    fontFamily: Inter
    fontSize: 32px
    fontWeight: '700'
    lineHeight: '1.2'
    letterSpacing: -0.02em
  headline-lg-mobile:
    fontFamily: Inter
    fontSize: 24px
    fontWeight: '700'
    lineHeight: '1.2'
    letterSpacing: -0.02em
  headline-md:
    fontFamily: Inter
    fontSize: 24px
    fontWeight: '600'
    lineHeight: '1.3'
    letterSpacing: -0.01em
  headline-sm:
    fontFamily: Inter
    fontSize: 20px
    fontWeight: '600'
    lineHeight: '1.4'
    letterSpacing: -0.01em
  body-lg:
    fontFamily: Inter
    fontSize: 16px
    fontWeight: '400'
    lineHeight: '1.6'
    letterSpacing: -0.01em
  body-md:
    fontFamily: Inter
    fontSize: 14px
    fontWeight: '400'
    lineHeight: '1.5'
    letterSpacing: 0em
  label-md:
    fontFamily: Inter
    fontSize: 12px
    fontWeight: '600'
    lineHeight: '1'
    letterSpacing: 0.02em
  code-md:
    fontFamily: jetbrainsMono
    fontSize: 14px
    fontWeight: '400'
    lineHeight: '1.5'
rounded:
  sm: 0.25rem
  DEFAULT: 0.5rem
  md: 0.75rem
  lg: 1rem
  xl: 1.5rem
  full: 9999px
spacing:
  base: 4px
  xs: 4px
  sm: 8px
  md: 16px
  lg: 24px
  xl: 40px
  grid_columns: '12'
  gutter: 24px
  margin_desktop: 32px
  margin_mobile: 16px
---

## Brand & Style
This design system focuses on high-stakes reliability and cognitive clarity for healthcare professionals. The brand personality is authoritative yet empathetic—balancing the rigid precision of clinical data with a soft, approachable interface that reduces user fatigue. 

The aesthetic follows a **Corporate Modern** approach with minimalist leanings. It prioritizes information density without clutter, using a "flat plus" philosophy: primarily flat surfaces defined by surgical 1px outlines, punctuated by extremely soft elevation to denote interactivity and hierarchy. The goal is to evoke a sense of sterile cleanliness and technical sophistication.

## Colors
The palette is anchored by a deep Navy Primary (`#1e3a8a`), symbolizing stability and institutional trust. To modernize the interface, we utilize a tiered Neutral system based on Slate and Gray scales. Surfaces use extremely subtle off-whites (`#f8fafc`) to reduce screen glare during long shifts.

Semantic colors (Success, Error, Warning) are executed with a "Tint & Tone" strategy. Backgrounds are high-lightness, low-saturation pastels to indicate state without visual alarm, while the foreground text uses high-contrast, bold versions of the same hue to ensure AA/AAA accessibility and immediate recognition.

## Typography
This design system utilizes **Inter** exclusively to leverage its exceptional legibility in data-heavy environments. To achieve a premium, technical feel, tracking (letter-spacing) is tightened on headlines and large body text. 

Hierarchy is established through weight and color rather than drastic size shifts. Use `SemiBold` (600) for sub-headers and `Bold` (700) for primary headings. Labels use a slightly increased letter-spacing and uppercase styling to differentiate metadata from patient data.

## Layout & Spacing
The spacing logic follows a strict 4px/8px baseline grid to ensure mathematical alignment across complex medical charts and dashboards. 

A **Fluid Grid** model is used for the main content area, allowing the UI to expand on wide medical monitors while maintaining 24px gutters to prevent information crowding. For data-dense views, such as lab results or patient lists, the `md` (16px) spacing unit should be the primary container padding to maximize "at-a-glance" data visibility.

## Elevation & Depth
Depth is handled with restraint to maintain the professional, flat aesthetic. This design system avoids heavy drop shadows in favor of **Low-Contrast Outlines**.

1.  **Level 0 (Base):** Flat surfaces using the neutral background color.
2.  **Level 1 (Surface):** White containers with a 1px border in `#e2e8f0`. No shadow.
3.  **Level 2 (Interactive/Overlay):** Used for cards or dropdowns. 1px border plus a soft, diffused shadow: `0px 4px 12px rgba(30, 58, 138, 0.05)`. The shadow is slightly tinted with the primary navy to keep it integrated with the brand.

## Shapes
To soften the clinical "coldness" of the system without losing professional rigor, we use a **Rounded** shape language. 

Standard UI components like buttons and input fields utilize a 0.5rem (8px) corner radius. This provides a modern, friendly touch that feels more ergonomic and accessible than sharp corners, while remaining structured enough for enterprise software.

## Components
- **Buttons:** Primary buttons are solid Navy (`#1e3a8a`) with white text. Secondary buttons use a 1px outline of `#cbd5e1` with primary-colored text.
- **Input Fields:** Use a 1px border in `#cbd5e1`. On focus, the border transitions to the primary navy with a subtle 2px outer glow in 10% primary color.
- **Status Chips:** These must use the semantic backgrounds defined in the Color section. Text should be `Bold` and the background should have a 100px (pill) radius for quick distinction from rectangular data cells.
- **Cards:** White background, 1px border in `#e2e8f0`. Use `lg` (24px) padding for general content and `md` (16px) for data-dense widgets.
- **Data Tables:** Remove vertical grid lines. Use 1px horizontal dividers in `#f1f5f9`. Header cells should use `label-md` typography with a subtle gray background.
- **Checkboxes/Radios:** Use the primary navy for the selected state. Ensure the hit target is at least 44px for touch-interface compatibility in mobile clinical environments.