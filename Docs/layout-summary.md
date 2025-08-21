# App.vue CSS: layout summary & key points

## Summary
- Header (menu) is intended to sit fixed at the top of the viewport.
- Tabs are centered inside the header using a centered container (.menu-bar / .tab-list).
- Main content sits below the fixed header via `padding-top: var(--header-height)`.
- `.main-content` is a centered two-column flex container: left = NutritionTracker, right = FoodLog.
- Responsive rule stacks columns vertically on small screens.

## Summary (what the current CSS does)
- :root defines layout variables (content width, header height).
- header is fixed to the top (position: fixed) and sized with --header-height.
- .menu-bar centers the tabs horizontally inside the header.
- .tab-list is a flex row constrained by max-width so tabs don’t overflow.
- main has top padding equal to header height so content starts below the fixed header.
- .main-content is a centered two-column flex container (left = NutritionTracker, right = FoodLog).
- Responsive styles stack columns under 900px.

## Key points to ensure desired layout (menu top, tracker left, foodlog right)
1. Header
   - Make header a top-level fixed element:
     - `position: fixed; top:0; left:0; right:0; height: var(--header-height); z-index: 1000;`
   - Header must not be inside any ancestor with `transform`, `filter`, `perspective` or `position` other than `static` (those break fixed positioning).
   - Center tabs by constraining `.tab-list` with `max-width` and using `justify-content: center`.

2. Main content
   - Push content below fixed header: `main { padding-top: var(--header-height); }`.
   - Two-column layout: `.main-content { display:flex; gap:...; max-width: var(--content-max-width); margin: 0 auto; }`
   - Columns: use flex sizes:
     - Equal: `.left-panel, .right-panel { flex:1 }`
     - Fixed ratio: `.left-panel { flex: 0 0 48% } .right-panel { flex: 0 0 52% }`

3. Responsiveness
   - Stack columns on small screens: `@media (max-width: 900px) { .main-content { flex-direction: column } }`
   - Make `.tab-list` scrollable if tabs overflow: `overflow-x: auto`.

4. Implementation / cleanup tips
   - Remove `scoped` from the `<style>` block if CSS variables or header positioning need to apply globally.
   - Avoid `.tab-list { width: 100% }` if you want centered tabs — use `width: auto; max-width: ...; justify-content: center`.
   - Ensure main layout is centered with `max-width` and `margin: 0 auto`.
   - If header still appears inside left column, inspect DOM ancestors for `transform`/`position` and move header outside the constrained wrapper.

## Minimal CSS checklist to paste into App.vue
- `:root { --header-height:64px; --content-max-width:1200px; }`
- `header { position: fixed; top:0; left:0; right:0; height:var(--header-height); z-index:1000; }`
- `main { padding-top:var(--header-height); }`
- `.main-content { display:flex; max-width:var(--content-max-width); margin:0 auto; gap:1.5rem; }`
- `.left-panel { flex:0 0 48% } .right-panel { flex:0 0 52% }`
- `@media (max-width:900px) { .main-content { flex