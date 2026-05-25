---
name: Technical Luxury
colors:
  surface: '#0b1326'
  surface-dim: '#0b1326'
  surface-bright: '#31394d'
  surface-container-lowest: '#060e20'
  surface-container-low: '#131b2e'
  surface-container: '#171f33'
  surface-container-high: '#222a3d'
  surface-container-highest: '#2d3449'
  on-surface: '#dae2fd'
  on-surface-variant: '#c5c5d9'
  inverse-surface: '#dae2fd'
  inverse-on-surface: '#283044'
  outline: '#8e8fa2'
  outline-variant: '#444656'
  surface-tint: '#bbc3ff'
  primary: '#bbc3ff'
  on-primary: '#001d93'
  primary-container: '#3d5afe'
  on-primary-container: '#f1f0ff'
  inverse-primary: '#2848ee'
  secondary: '#cdbdff'
  on-secondary: '#370096'
  secondary-container: '#5203d5'
  on-secondary-container: '#c0acff'
  tertiary: '#00e475'
  on-tertiary: '#003918'
  tertiary-container: '#007f3e'
  on-tertiary-container: '#c8ffcf'
  error: '#ffb4ab'
  on-error: '#690005'
  error-container: '#93000a'
  on-error-container: '#ffdad6'
  primary-fixed: '#dee0ff'
  primary-fixed-dim: '#bbc3ff'
  on-primary-fixed: '#000f5d'
  on-primary-fixed-variant: '#002ccd'
  secondary-fixed: '#e8deff'
  secondary-fixed-dim: '#cdbdff'
  on-secondary-fixed: '#20005f'
  on-secondary-fixed-variant: '#4f00d0'
  tertiary-fixed: '#62ff96'
  tertiary-fixed-dim: '#00e475'
  on-tertiary-fixed: '#00210b'
  on-tertiary-fixed-variant: '#005226'
  background: '#0b1326'
  on-background: '#dae2fd'
  surface-variant: '#2d3449'
typography:
  display-lg:
    fontFamily: Hanken Grotesk
    fontSize: 48px
    fontWeight: '700'
    lineHeight: 56px
    letterSpacing: -0.02em
  display-lg-mobile:
    fontFamily: Hanken Grotesk
    fontSize: 32px
    fontWeight: '700'
    lineHeight: 40px
    letterSpacing: -0.01em
  headline-md:
    fontFamily: Hanken Grotesk
    fontSize: 24px
    fontWeight: '600'
    lineHeight: 32px
  body-lg:
    fontFamily: Inter
    fontSize: 18px
    fontWeight: '400'
    lineHeight: 28px
  body-md:
    fontFamily: Inter
    fontSize: 16px
    fontWeight: '400'
    lineHeight: 24px
  label-sm:
    fontFamily: JetBrains Mono
    fontSize: 12px
    fontWeight: '500'
    lineHeight: 16px
    letterSpacing: 0.05em
rounded:
  sm: 0.125rem
  DEFAULT: 0.25rem
  md: 0.375rem
  lg: 0.5rem
  xl: 0.75rem
  full: 9999px
spacing:
  base: 8px
  container-max: 1280px
  gutter: 24px
  margin-mobile: 16px
  margin-desktop: 40px
---

## Brand & Style

This design system embodies "Technical Luxury"—a synthesis of high-performance engineering and high-end aesthetic refinement. The brand personality is precise, sophisticated, and authoritative, targeting a professional audience that values efficiency wrapped in a premium experience. 

The design style is **Modern Corporate** with a heavy influence from **Minimalism** and **Glassmorphism**. It prioritizes extreme clarity, generous white space, and subtle depth through translucent layers. The emotional response should be one of "effortless power"—the UI feels fast, reliable, and meticulously crafted.

## Colors

The palette is anchored by "Cyber Cobalt," a deep, high-chroma blue that signals intelligence and premium performance. This primary color is supported by a suite of tonally aligned accents: a muted Electric Purple, a crisp Spring Green for success states, and a sophisticated Burnt Orange for warnings.

The default mode is **Dark**. The background architecture utilizes deep charcoal and navy neutrals to provide a high-contrast canvas for the vibrant primary accents. Surface colors are derived from the neutral base with varying levels of opacity to create a sense of optical depth.

## Typography

The typography strategy leverages three distinct typefaces to reinforce the technical luxury narrative:
- **Hanken Grotesk** (Headlines): Sharp, contemporary, and authoritative. Used for high-level information hierarchy.
- **Inter** (Body): Highly legible and neutral, ensuring that complex data remains accessible.
- **JetBrains Mono** (Labels/Data): Used sparingly for metadata, status labels, and technical values to inject a sense of "precision engineering."

Scales are tight and controlled. Large display type uses slight negative letter-spacing to appear more "locked-in" and architectural.

## Layout & Spacing

The layout follows a **Fixed Grid** model on desktop, centering content within a 1280px container to maintain visual focus. On smaller viewports, it transitions to a fluid system.

A 12-column grid is utilized for desktop layouts, while mobile collapses to a single column with 16px side margins. The spacing rhythm is strictly based on an 8px linear scale (8, 16, 24, 32, 48, 64). Internal component padding should be generous to maintain the "luxury" feel of uncrowded interfaces.

## Elevation & Depth

Depth is communicated through **Glassmorphism** and **Tonal Layering**. Rather than traditional heavy shadows, this design system uses:
1.  **Backdrop Blurs:** High-elevation elements (modals, navigation bars) use a 20px blur with a 60% opaque neutral fill.
2.  **Inner Glows:** A subtle 1px top-border (stroke) with 10% white opacity is applied to cards to simulate a light source from above.
3.  **Tonal Offsets:** Background is the darkest tier, while interactive cards sit one "step" lighter in the neutral scale.

Shadows, when used, are "Ambient Shadows"—extremely diffused (32px+ blur), low opacity (15%), and tinted with the primary Cyber Cobalt hex to prevent a "dirty" look.

## Shapes

The shape language is **Soft (1)**. Corner radii are kept small (4px for small components, 8px for cards) to maintain a disciplined, "technical" appearance. Fully rounded "pill" shapes are reserved exclusively for status indicators and specific secondary tags to distinguish them from primary structural elements.

## Components

### Buttons
Primary buttons use a solid Cyber Cobalt fill with white text. Hover states should introduce a subtle glow effect rather than a simple color shift. Secondary buttons use a ghost style with a 1px border.

### Cards
Cards are the primary container. They should feature a subtle 1px border of `neutral-700` and a very slight background tint. In dark mode, card surfaces should be `#1E293B` or similar.

### Inputs
Input fields are dark-filled with a 1px bottom border that transforms into a full Cyber Cobalt glow on focus. Labels use the JetBrains Mono "label-sm" style for a technical feel.

### Chips & Tags
Chips use low-saturation versions of the accent colors (Green, Purple, Orange) with high-saturation text to ensure legibility while remaining tonally aligned with the premium blue.

### Navigation
The sidebar or top-nav should utilize the backdrop blur effect, ensuring it feels like a layer floating above the content rather than a static block.