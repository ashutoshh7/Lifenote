---
name: Luminal Noir
colors:
  surface: '#131313'
  surface-dim: '#131313'
  surface-bright: '#393939'
  surface-container-lowest: '#0e0e0e'
  surface-container-low: '#1c1b1b'
  surface-container: '#201f1f'
  surface-container-high: '#2a2a2a'
  surface-container-highest: '#353534'
  on-surface: '#e5e2e1'
  on-surface-variant: '#bccbb9'
  inverse-surface: '#e5e2e1'
  inverse-on-surface: '#313030'
  outline: '#869585'
  outline-variant: '#3d4a3d'
  surface-tint: '#53e076'
  primary: '#53e076'
  on-primary: '#003914'
  primary-container: '#1db954'
  on-primary-container: '#004118'
  inverse-primary: '#006e2d'
  secondary: '#ddb8ff'
  on-secondary: '#490081'
  secondary-container: '#62259b'
  on-secondary-container: '#d1a1ff'
  tertiary: '#7bd0ff'
  on-tertiary: '#00354a'
  tertiary-container: '#16ade7'
  on-tertiary-container: '#003d54'
  error: '#ffb4ab'
  on-error: '#690005'
  error-container: '#93000a'
  on-error-container: '#ffdad6'
  primary-fixed: '#72fe8f'
  primary-fixed-dim: '#53e076'
  on-primary-fixed: '#002108'
  on-primary-fixed-variant: '#005320'
  secondary-fixed: '#f0dbff'
  secondary-fixed-dim: '#ddb8ff'
  on-secondary-fixed: '#2c0051'
  on-secondary-fixed-variant: '#62259b'
  tertiary-fixed: '#c4e7ff'
  tertiary-fixed-dim: '#7bd0ff'
  on-tertiary-fixed: '#001e2c'
  on-tertiary-fixed-variant: '#004c69'
  background: '#131313'
  on-background: '#e5e2e1'
  surface-variant: '#353534'
typography:
  display-lg:
    fontFamily: Hanken Grotesk
    fontSize: 48px
    fontWeight: '700'
    lineHeight: 56px
    letterSpacing: -0.02em
  headline-lg:
    fontFamily: Hanken Grotesk
    fontSize: 32px
    fontWeight: '600'
    lineHeight: 40px
    letterSpacing: -0.01em
  headline-lg-mobile:
    fontFamily: Hanken Grotesk
    fontSize: 24px
    fontWeight: '600'
    lineHeight: 32px
  body-md:
    fontFamily: Inter
    fontSize: 16px
    fontWeight: '400'
    lineHeight: 24px
  body-sm:
    fontFamily: Inter
    fontSize: 14px
    fontWeight: '400'
    lineHeight: 20px
  label-md:
    fontFamily: Geist
    fontSize: 12px
    fontWeight: '500'
    lineHeight: 16px
    letterSpacing: 0.05em
rounded:
  sm: 0.25rem
  DEFAULT: 0.5rem
  md: 0.75rem
  lg: 1rem
  xl: 1.5rem
  full: 9999px
spacing:
  base: 8px
  container-padding-mobile: 1rem
  container-padding-desktop: 2.5rem
  gutter: 1.5rem
  stack-gap: 1rem
---

## Brand & Style

The design system is a sophisticated, dark-mode centric interface designed for high-end digital experiences. It balances deep, obsidian tones with a vibrant, multi-accent palette to create an atmosphere of focused energy and modern elegance. 

The aesthetic is a hybrid of **Corporate Modern** and **Glassmorphism**. It leverages high-density information layouts while maintaining a premium feel through translucent layering and precise, Apple-inspired geometry. The emotional response is one of "Technical Luxury"—professional, high-performance, and visually stimulating without being overwhelming.

## Colors

This design system utilizes a "Deep Charcoal" base (#121212) with a hierarchy of vibrant accents. 

- **Primary (Vivid Green):** Used for primary actions, success states, and core branding.
- **Secondary (Light Purple):** Reserved for creative features, discovery modes, and secondary active states.
- **Tertiary (Sky Blue):** Applied to informative updates, progress tracking, and communication-related UI.
- **Quaternary (Orange):** Used for high-impact alerts, trending indicators, and energetic highlights.

Neutral surfaces should follow a tiered darkening approach:
- **Surface:** #121212 (Background)
- **Surface-Elevated:** #1E1E1E (Cards/Containers)
- **Surface-Overlay:** #2A2A2A (Tooltips/Menus)

## Typography

The typographic scale emphasizes clarity and technical precision. **Hanken Grotesk** provides a sharp, contemporary look for headlines. **Inter** handles body copy for maximum readability across various display densities. **Geist** is utilized for labels, tags, and data-driven elements to reinforce the technical, "noir" aesthetic of the design system.

## Layout & Spacing

This design system uses a 12-column fluid grid for desktop and a single-column flow for mobile. 

- **Desktop:** 24px (1.5rem) gutters with 40px (2.5rem) outer margins.
- **Mobile:** 16px (1rem) gutters and margins.
- **Rhythm:** All spacing (padding, margins, gaps) must be multiples of the 8px base unit to ensure visual harmony and vertical alignment.

## Elevation & Depth

Depth is established through **Tonal Layering** and **Glassmorphism**. 

1.  **Lower Elevation:** Flat surfaces with subtle 1px borders (#FFFFFF at 10% opacity) define the structural layout.
2.  **Middle Elevation:** Containers use a background blur (12px) and a semi-transparent fill (#1E1E1E at 80% opacity) to appear "floated" over the background.
3.  **High Elevation:** Modals and menus use a soft, deep ambient shadow (0px 20px 40px rgba(0,0,0,0.4)) combined with a subtle inner glow on the top edge to simulate Apple-like hardware precision.

## Shapes

The shape language is "Subtle & Rounded," drawing inspiration from high-end consumer hardware. 

- **Default Radius:** 0.5rem (8px) for standard components.
- **Large Containers & Search Inputs:** Use `rounded-lg` (1rem/16px) or `rounded-xl` (1.5rem/24px) for a more approachable, modern feel.
- **Interactive Elements:** Buttons and tags use consistent 0.5rem radii to maintain a professional, organized appearance.

## Components

### Search Inputs
Must use a `rounded-lg` or `rounded-xl` radius. The background should be a slightly lighter neutral (#1E1E1E) with a subtle 1px border that brightens on focus to the Primary color.

### Buttons & Chips
- **Primary:** Solid #1DB954 with white or dark text.
- **Secondary/Tertiary/Quaternary:** Use these colors for high-visibility tags or toggle active states.
- **Chips:** Small, `rounded-lg` elements. Active chips should use a subtle glow (drop-shadow) of their respective accent color.

### Progress Bars
Utilize the multi-accent palette to differentiate categories. Progress fills should have a slight linear gradient (e.g., #38BDF8 to #C084FC) for a dynamic, high-fidelity look.

### Cards
Containers should feature a 16px corner radius. Borders must be low-contrast (rgba(255,255,255,0.1)) to maintain the "Noir" aesthetic while ensuring structural definition.

### Active States
Any active navigation item or selected list item should be indicated by a 2px vertical "pill" on the leading edge using one of the four accent colors, paired with a subtle background tint of that same color at 10% opacity.