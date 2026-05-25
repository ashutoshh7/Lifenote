---
name: Luminal Noir
colors:
  surface: '#131313'
  surface-dim: '#131313'
  surface-bright: '#393939'
  surface-container-lowest: '#0e0e0e'
  surface-container-low: '#1b1b1b'
  surface-container: '#1f1f1f'
  surface-container-high: '#2a2a2a'
  surface-container-highest: '#353535'
  on-surface: '#e2e2e2'
  on-surface-variant: '#bccbb9'
  inverse-surface: '#e2e2e2'
  inverse-on-surface: '#303030'
  outline: '#869585'
  outline-variant: '#3d4a3d'
  surface-tint: '#53e076'
  primary: '#53e076'
  on-primary: '#003914'
  primary-container: '#1db954'
  on-primary-container: '#004118'
  inverse-primary: '#006e2d'
  secondary: '#c8c6c5'
  on-secondary: '#313030'
  secondary-container: '#4a4949'
  on-secondary-container: '#bab8b7'
  tertiary: '#c8c6c5'
  on-tertiary: '#303030'
  tertiary-container: '#a2a1a0'
  on-tertiary-container: '#383838'
  error: '#ffb4ab'
  on-error: '#690005'
  error-container: '#93000a'
  on-error-container: '#ffdad6'
  primary-fixed: '#72fe8f'
  primary-fixed-dim: '#53e076'
  on-primary-fixed: '#002108'
  on-primary-fixed-variant: '#005320'
  secondary-fixed: '#e5e2e1'
  secondary-fixed-dim: '#c8c6c5'
  on-secondary-fixed: '#1c1b1b'
  on-secondary-fixed-variant: '#474646'
  tertiary-fixed: '#e4e2e1'
  tertiary-fixed-dim: '#c8c6c5'
  on-tertiary-fixed: '#1b1c1c'
  on-tertiary-fixed-variant: '#474746'
  background: '#131313'
  on-background: '#e2e2e2'
  surface-variant: '#353535'
typography:
  display-lg:
    fontFamily: Inter
    fontSize: 48px
    fontWeight: '700'
    lineHeight: 56px
    letterSpacing: -0.02em
  display-lg-mobile:
    fontFamily: Inter
    fontSize: 36px
    fontWeight: '700'
    lineHeight: 42px
    letterSpacing: -0.02em
  headline-lg:
    fontFamily: Inter
    fontSize: 32px
    fontWeight: '600'
    lineHeight: 40px
    letterSpacing: -0.01em
  headline-md:
    fontFamily: Inter
    fontSize: 24px
    fontWeight: '600'
    lineHeight: 32px
    letterSpacing: -0.01em
  body-lg:
    fontFamily: Inter
    fontSize: 18px
    fontWeight: '400'
    lineHeight: 28px
    letterSpacing: '0'
  body-md:
    fontFamily: Inter
    fontSize: 16px
    fontWeight: '400'
    lineHeight: 24px
    letterSpacing: '0'
  label-md:
    fontFamily: Inter
    fontSize: 14px
    fontWeight: '500'
    lineHeight: 20px
    letterSpacing: 0.05em
  label-sm:
    fontFamily: Inter
    fontSize: 12px
    fontWeight: '500'
    lineHeight: 16px
    letterSpacing: 0.03em
rounded:
  sm: 0.25rem
  DEFAULT: 0.5rem
  md: 0.75rem
  lg: 1rem
  xl: 1.5rem
  full: 9999px
spacing:
  unit: 8px
  container-padding-mobile: 20px
  container-padding-desktop: 48px
  gutter: 24px
  stack-sm: 8px
  stack-md: 16px
  stack-lg: 32px
  stack-xl: 64px
---

## Brand & Style
This design system embodies a premium, high-fidelity aesthetic that merges the precision of minimalist Swiss design with the immersive, dark-mode-first philosophy of modern media platforms. It is tailored for high-end SaaS, lifestyle, and media applications where content is the protagonist.

The style is defined as **Premium Minimalism with Glassmorphic accents**. It prioritizes extreme legibility, vast negative space, and a refined sense of depth. The emotional response is one of calm, exclusivity, and technological sophistication. Depth is not communicated through heavy shadows, but through subtle layering, background blurs, and precise tonal shifts.

## Colors
The palette is rooted in "True Black" to maximize contrast on OLED displays and provide a sophisticated foundation. 

- **Primary:** An electric, vibrant green used sparingly for high-priority actions, progress indicators, and active states.
- **Neutral Stack:** Deep blacks and charcoal grays form the container hierarchy. 
- **Accents:** Functional colors (success, error, warning) should be desaturated to maintain the minimalist aesthetic, only appearing when critical feedback is required.
- **Glass Effects:** Overlays use a semi-transparent hex (`#FFFFFF` at 5-10% opacity) with a heavy background blur (20px-40px).

## Typography
The typography system relies on **Inter**, utilizing its variable weight axis to create a clear visual hierarchy. 

- **Headings:** Large headings use tight tracking (-2%) and bold weights to feel impactful.
- **Labels:** Small labels and overlines utilize increased letter spacing (up to 5%) and uppercase styling to provide a "premium metadata" feel.
- **Contrast:** High contrast is maintained between primary text (#FFFFFF) and secondary text (#B3B3B3) to guide the eye toward content and away from supplementary UI elements.

## Layout & Spacing
The layout follows a **Fluid-Fixed Hybrid** model. Content is contained within a max-width of 1440px for desktop, while mobile views utilize a flexible 4-column grid.

- **Whitespace:** Use generous vertical spacing (`stack-xl`) between major sections to allow the design to "breathe."
- **Grid:** A 12-column grid is used for desktop. 
- **Margins:** Large outer margins (48px+) are preferred on desktop to create an editorial, high-end feel.
- **Reflow:** On mobile, components should transition to full-width with reduced padding, while Maintaining large card radii to preserve the "object-like" feel of the UI elements.

## Elevation & Depth
Depth is created through a "Layered Surface" approach rather than traditional drop shadows.

1.  **Level 0 (Base):** True Black (#000000).
2.  **Level 1 (Cards/Sections):** Dark Gray (#121212) with a subtle 1px border (#FFFFFF at 5% opacity).
3.  **Level 2 (Floating/Modals):** Lighter Charcoal (#282828) with a very soft, large-radius ambient shadow (0px 20px 40px rgba(0,0,0,0.5)).
4.  **Glass Layer:** Used for fixed headers and tab bars. These use a `backdrop-filter: blur(30px)` and a semi-transparent fill to allow content to peek through during scrolls.

## Shapes
The shape language is characterized by large, friendly radii that contrast with the "serious" dark color palette.

- **Containers:** Main content cards and imagery use a large 24px-32px radius.
- **Interactive Elements:** Buttons and tags use a fully rounded "Pill" shape to signify interactivity.
- **Forms:** Inputs use a smaller 12px radius to maintain structure and alignment within the larger containers.

## Components
- **Buttons:** Primary buttons are pill-shaped, filled with the Primary Green, and use black text for maximum contrast. Secondary buttons are outlined with a 1px border (#FFFFFF at 20% opacity).
- **Input Fields:** Minimalist design with no background—only a bottom border (2px) that glows with the primary color when focused.
- **Sleek Progress Bars:** 4px height, with a dark gray track and a vibrant primary color fill. For high-end feel, add a subtle outer glow to the leading edge of the progress indicator.
- **Smooth Toggles:** Oversized pill-shaped tracks with a white circular thumb. Use a spring-based animation for the transition.
- **Cards:** No shadows; use tonal separation and a subtle inner stroke. Images within cards should always have a `5-10%` black overlay to ensure white text remains readable.
- **Tab Bars:** Positioned at the bottom (mobile) or top (desktop) with a heavy glassmorphic blur and minimal iconography.