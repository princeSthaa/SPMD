---
name: Clinical Precision System
colors:
  surface: '#faf8ff'
  surface-dim: '#dad9e1'
  surface-bright: '#faf8ff'
  surface-container-lowest: '#ffffff'
  surface-container-low: '#f4f3fa'
  surface-container: '#eeedf4'
  surface-container-high: '#e9e7ef'
  surface-container-highest: '#e3e1e9'
  on-surface: '#1a1b21'
  on-surface-variant: '#444651'
  inverse-surface: '#2f3036'
  inverse-on-surface: '#f1f0f7'
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
  tertiary: '#4b1c00'
  on-tertiary: '#ffffff'
  tertiary-container: '#6e2c00'
  on-tertiary-container: '#f39461'
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
  tertiary-fixed: '#ffdbcb'
  tertiary-fixed-dim: '#ffb691'
  on-tertiary-fixed: '#341100'
  on-tertiary-fixed-variant: '#773205'
  background: '#faf8ff'
  on-background: '#1a1b21'
  surface-variant: '#e3e1e9'
typography:
  display-lg:
    fontFamily: Inter
    fontSize: 48px
    fontWeight: '700'
    lineHeight: '1.2'
    letterSpacing: -0.02em
  headline-lg:
    fontFamily: Inter
    fontSize: 32px
    fontWeight: '600'
    lineHeight: '1.3'
    letterSpacing: -0.01em
  headline-lg-mobile:
    fontFamily: Inter
    fontSize: 24px
    fontWeight: '600'
    lineHeight: '1.3'
  headline-md:
    fontFamily: Inter
    fontSize: 24px
    fontWeight: '600'
    lineHeight: '1.4'
  headline-sm:
    fontFamily: Inter
    fontSize: 20px
    fontWeight: '600'
    lineHeight: '1.4'
  body-lg:
    fontFamily: Inter
    fontSize: 18px
    fontWeight: '400'
    lineHeight: '1.6'
  body-md:
    fontFamily: Inter
    fontSize: 16px
    fontWeight: '400'
    lineHeight: '1.5'
  body-sm:
    fontFamily: Inter
    fontSize: 14px
    fontWeight: '400'
    lineHeight: '1.5'
  label-md:
    fontFamily: Inter
    fontSize: 12px
    fontWeight: '600'
    lineHeight: '1'
    letterSpacing: 0.05em
  data-mono:
    fontFamily: JetBrains Mono
    fontSize: 14px
    fontWeight: '500'
    lineHeight: '1.4'
rounded:
  sm: 0.125rem
  DEFAULT: 0.25rem
  md: 0.375rem
  lg: 0.5rem
  xl: 0.75rem
  full: 9999px
spacing:
  base: 4px
  xs: 4px
  sm: 8px
  md: 16px
  lg: 24px
  xl: 48px
  gutter: 24px
  margin-mobile: 16px
  margin-desktop: 32px
  max-width: 1440px
---

## Brand & Style

The design system is engineered for high-stakes medical environments where clarity, speed of recognition, and data integrity are paramount. The brand personality is **Precise, Reliable, and Patient-Focused**, favoring utility and trust over decorative flair.

The visual style is **Corporate / Modern**, characterized by a rigorous adherence to grid systems, high-contrast information hierarchies, and a "clinical-clean" aesthetic. It utilizes a refined SaaS-inspired approach to manage high data density without overwhelming the user. Every element is designed to evoke a sense of security and institutional authority, ensuring that clinicians and patients feel the system is a stable, error-reducing partner in healthcare.

## Colors

The palette is anchored by **Deep Clinical Blue**, providing a foundation of stability and professionalism. 

- **Primary:** A commanding navy-blue used for navigation, primary actions, and brand identification.
- **Backgrounds:** We utilize "Soft Grays" (Slate-50) to reduce eye strain during prolonged use while maintaining a "Clean White" surface for active work areas like cards and forms.
- **Status Colors:** These are non-negotiable semantic markers. **Success Green** indicates 'In Stock' or 'Validated'; **Warning Amber** flags 'Limited Supply' or 'Interaction Risks'; **Danger Red** is reserved for 'Expired' medications, 'Allergy Alerts', or 'Critical Errors'.

## Typography

This design system uses **Inter** for all UI elements to ensure maximum legibility and a neutral, professional tone. Inter’s tall x-height and distinct character shapes are essential for distinguishing similar-looking drug names or dosage numbers.

For specialized medical data—such as NDC codes, serial numbers, or dosage timestamps—a monospaced font (**JetBrains Mono**) is used to ensure tabular alignment and character clarity. 

**Hierarchy Rules:**
- Use **Headline-SM** for section headers within medical records.
- Use **Label-MD** in uppercase for field descriptions to distinguish them from user-entered data.
- Ensure all body text maintains at least a 4.5:1 contrast ratio against backgrounds to meet accessibility standards.

## Layout & Spacing

The layout utilizes a **Fixed Grid** model on desktop (12 columns) and a fluid 4-column grid on mobile devices. 

- **Data Density:** Spacing is tighter than consumer apps to allow for comprehensive medication lists and patient history views without excessive scrolling. Use the `md` (16px) unit for standard gaps between elements.
- **Grouping:** Use `lg` (24px) spacing to separate distinct medical modules (e.g., separating 'Current Prescriptions' from 'Pharmacy Logistics').
- **Reflow:** On mobile, sidebars collapse into a bottom navigation bar or a hamburger menu to prioritize the content area. Cards should span the full width of the mobile viewport minus the `margin-mobile`.

## Elevation & Depth

To maintain a clinical feel, elevation is used sparingly to define hierarchy rather than create depth for its own sake.

- **Low-Contrast Outlines:** Most containers use a 1px border (#E2E8F0) to define boundaries. This provides a "structured" look that suggests organization and precision.
- **Subtle Shadows:** For floating elements like dropdowns, modals, or active medication cards, use a very soft, diffused shadow. Avoid harsh, dark shadows; instead, use a 10% opacity slate-tinted shadow to lift the element off the background surface.
- **Tonal Layers:** Deepest depth is represented by the light gray background. Active work surfaces (cards) are pure white. Hover states should use a subtle light-blue tint (#EFF6FF) rather than a shadow change to indicate interactivity.

## Shapes

The design system employs a **Soft** shape language. 

- **Standard Radius:** 0.25rem (4px) for input fields, buttons, and small UI components. This provides a professional, "fitted" look that is cleaner than sharp corners but more serious than highly rounded ones.
- **Large Radius:** 0.5rem (8px) for cards and modals to create a distinct containerization of patient data.
- **Pill Shapes:** Reserved exclusively for status tags (e.g., "In Stock", "Active") to make them immediately distinguishable from actionable buttons.

## Components

### Buttons
- **Primary:** Solid Deep Clinical Blue with white text. High-contrast, used for "Confirm Order" or "Dispense."
- **Secondary:** Outlined Blue with white background. Used for "Add New Medication."
- **Danger:** Solid Red. Used for "Stop Prescription."

### Input Fields
- Use a 1px border (#CBD5E1). In focus state, use a 2px Primary Blue border with a soft blue outer glow. 
- Validation states must be clearly marked with an icon (Checkmark for Success, Exclamation for Error) to assist color-blind users.

### Cards
- White background, 1px border, 0.5rem corner radius.
- Headers within cards should have a light gray bottom border to separate patient identification from the medication list.

### Status Chips (Pill-shaped)
- **In Stock:** Light green background with dark green text.
- **Allergy Alert:** Light red background with dark red text.
- **Limited Supply:** Light amber background with dark amber text.

### Data Tables
- Use zebra-striping (F8FAFC) for rows in high-density lists.
- Column headers must be sticky when scrolling through long medication histories.
- Use the `data-mono` typography for dosage quantities (e.g., "500mg").