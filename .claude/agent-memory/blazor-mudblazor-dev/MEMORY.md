# Blazor MudBlazor Dev - Agent Memory

## UX/UI Improvements Implemented

### WCAG 2.1 Accessibility (High Priority - Completed)

**Focus Indicators:**
- Added CSS variables in `fsh-theme.css`: `--fsh-focus-ring-color`, `--fsh-focus-ring-width`, `--fsh-focus-ring-offset`
- Implemented `:focus-visible` styles for all interactive elements (buttons, links, forms, cards, menu items)
- Focus ring uses primary color with 0.6 opacity, 3px width, 2px offset

**ARIA Labels and Roles:**
- FshAccountMenu: Added `role="navigation"`, `aria-label`, `aria-haspopup`, `role="menu"`, `role="menuitem"`
- FshStatCard: Added `role="region"`, `aria-label` with dynamic content, `aria-live="polite"` for values
- UsersPage/RolesPage: Added descriptive `aria-label` to all action buttons with dynamic counts

**Keyboard Navigation:**
- Created `fsh-keyboard.js` with global keyboard shortcuts handler
- Shortcuts: `/` (focus search), `Ctrl+N` (new), `Ctrl+R` (refresh), `Ctrl+S` (save), `Esc` (close/clear)
- Added skip link in PlaygroundLayout (`<a href="#main-content" class="fsh-skip-link">`)
- Skip link is visually hidden but appears on focus (WCAG requirement)

### Responsive Design (High Priority - Completed)

**Responsive Table Component:**
- Created `FshResponsiveTable<TItem>` in BuildingBlocks/Blazor.UI/Components/Data/
- Switches between DataGrid (desktop) and Card view (mobile) at 768px breakpoint
- Uses `fsh-responsive.js` for viewport detection with debounced resize listener
- Mobile cards use MudCard with clean field/value layout
- Supports pagination, empty states, and custom actions

**Responsive Navigation Drawer:**
- PlaygroundLayout now switches DrawerVariant based on viewport
- Mobile (<768px): `Temporary` variant, closed by default, overlay when open
- Desktop (≥768px): `Persistent` variant, open by default, pushes content
- Uses DotNetObjectReference for JS interop with cleanup in Dispose

## Key Patterns

**JS Interop Pattern:**
```csharp
private DotNetObjectReference<ComponentType>? _dotNetHelper;

protected override async Task OnAfterRenderAsync(bool firstRender) {
    if (firstRender) {
        _dotNetHelper = DotNetObjectReference.Create(this);
        await JS.InvokeVoidAsync("namespace.addListener", _dotNetHelper, nameof(Callback));
    }
}

[JSInvokable]
public void Callback(bool value) {
    // Handle callback
    InvokeAsync(StateHasChanged);
}

public void Dispose() {
    if (_dotNetHelper != null) {
        JS.InvokeVoidAsync("namespace.removeListener", _dotNetHelper);
        _dotNetHelper.Dispose();
    }
}
```

**CSS Custom Properties:**
- Use CSS variables for theming (e.g., `--fsh-focus-ring-color`)
- Reference MudBlazor palette with `rgba(var(--mud-palette-primary-rgb), alpha)`
- Always provide fallbacks for older browsers

**Accessibility Best Practices:**
- Use `aria-hidden="true"` for decorative icons
- Provide descriptive `aria-label` for all interactive elements
- Use semantic HTML roles (`role="main"`, `role="navigation"`, `role="region"`)
- Dynamic aria-labels should include context (e.g., counts, states)

## File Organization

**BuildingBlocks/Blazor.UI:**
- `/Components/Data/` - Data display components (tables, grids)
- `/Components/Cards/` - Card components
- `/Components/User/` - User-related UI (account menu, profile)
- `/wwwroot/css/` - Global theme CSS (fsh-theme.css)

**Playground/Playground.Blazor:**
- `/Components/Layout/` - Layout components (PlaygroundLayout, NavMenu)
- `/Components/Pages/` - Page components organized by feature
- `/wwwroot/` - Static assets and JS utilities

## Scripts to Add to App.razor

Order matters for dependencies:
1. `_framework/blazor.web.js` (Blazor framework)
2. `_content/MudBlazor/MudBlazor.min.js` (MudBlazor)
3. `fsh-theme.js` (theme management)
4. `fsh-keyboard.js` (keyboard shortcuts)
5. `fsh-responsive.js` (responsive utilities)

## Testing Checklist

**Accessibility:**
- [ ] Tab through all interactive elements (visible focus ring)
- [ ] Test with screen reader (NVDA/JAWS)
- [ ] Keyboard shortcuts work (`/`, `Ctrl+N`, `Ctrl+R`)
- [ ] Skip link appears on Tab key press

**Responsive:**
- [ ] Resize browser window across 768px breakpoint
- [ ] Mobile: Drawer closes, cards display, touch targets adequate
- [ ] Desktop: Drawer opens, DataGrid displays, proper spacing

## Known Issues

- ApiClient/Generated.cs has pre-existing type conversion errors (not related to UX/UI changes)
- These errors are in generated code and should be fixed by regenerating the API client

## Next Steps (Medium/Low Priority Tasks)

- [ ] Task #6: Color contrast WCAG AA compliance check
- [ ] Task #7: Ensure touch targets are 44x44px minimum
- [ ] Task #8: Skeleton loading components
- [ ] Task #9: Empty states with illustrations
- [ ] Task #10: Progress dialog for bulk operations
- [ ] Task #11: Standardize spacing system with CSS variables
- [ ] Task #12: Saved filter presets
- [ ] Task #13: Export data (CSV/Excel/PDF)
- [ ] Task #14: Branded error page
- [ ] Task #15: Micro-interactions and animations
