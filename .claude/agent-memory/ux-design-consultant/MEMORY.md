# UX Design Consultant Memory

## Project Context
- **Framework**: Blazor WebAssembly/Server with MudBlazor component library
- **Pattern**: FSH (FullStackHero) vertical slice architecture
- **Theme System**: Custom theming with MudBlazor ThemeManager
- **Design System**: Material Design inspired with custom FSH components

## Component Architecture Discovered

### Core UI Components (BuildingBlocks/Blazor.UI)
- **FshPageHeader**: Page header with title, description, actions
- **FshStatCard**: Metric display card with icon, value, label, badge
- **FshAccountMenu**: User profile dropdown in app bar
- **FshConfirmDialog**: Confirmation dialog component
- **FshTable**: Wrapper around MudTable
- **Input Components**: FshTextField, FshCheckbox, FshSelect, FshSwitch

### Application Pages (Playground.Blazor)
- **UsersPage**: Complex data grid with filtering, bulk actions, stats
- **RolesPage**: Role management with permissions
- **DashboardPage**: Overview with stat cards and recent audits
- **SimpleLogin**: Custom login page with social buttons (disabled)
- **Theme Settings**: Live theme customization

## Design Patterns Identified

### Strengths
1. **Consistent component wrapper pattern** (Fsh* components)
2. **Scoped CSS architecture** (component.razor.css files)
3. **MudBlazor integration** with custom styling
4. **CSS custom properties** for theming (:root variables)
5. **Gradient accents** on cards and avatars
6. **Loading states** with MudProgressCircular
7. **Stat cards** with icons and badges for metrics

### Issues & Anti-patterns
1. **Inconsistent spacing** (some use pa-4, some use padding in CSS)
2. **Mixed styling approaches** (inline styles, CSS classes, MudBlazor utility classes)
3. **No loading skeletons** (only spinners)
4. **Error page lacks branding** (generic Bootstrap-style error)
5. **Hardcoded colors** in some components (should use CSS variables)
6. **Accessibility gaps**: missing ARIA labels, keyboard navigation issues
7. **Responsive design inconsistencies** (limited mobile optimization)
8. **No empty state illustrations** (only icons and text)

## UX Observations

### Navigation & Information Architecture
- Side drawer navigation with sections
- Breadcrumbs component exists but not used consistently
- Nav sections use uppercase labels (good for hierarchy)
- Missing: Search in navigation, recent items, favorites

### Data Tables & Lists
- MudDataGrid with multi-select, sorting, filtering
- Inline actions with tooltips
- Bulk action toolbar appears on selection
- Issues: No saved filters, no column customization, no export

### Forms & Validation
- EditForm with MudBlazor inputs
- Validators implemented (AbstractValidator pattern)
- Issues: No inline validation feedback, no field-level help text consistency

### Feedback & Notifications
- Snackbar for toasts
- Confirm dialogs for destructive actions
- Issues: No progress indication for long operations, no undo capability

## Accessibility Concerns
1. Color contrast not verified (especially gradients)
2. Focus indicators inconsistent
3. Screen reader support incomplete
4. Keyboard navigation not fully implemented
5. Missing skip links
6. Form labels not always properly associated

## Performance Considerations
- StreamRendering used on dashboard
- Lazy loading not evident
- Images not optimized (no lazy loading, no srcset)
- No virtualization for long lists

## Mobile/Responsive Issues
- Breakpoints defined but inconsistent usage
- Tables not responsive-friendly (no card view for mobile)
- Modals may overflow on small screens
- Touch targets may be too small (<44px)

## Links to Detailed Notes
- [accessibility-audit.md](accessibility-audit.md) - WCAG compliance findings
- [responsive-design.md](responsive-design.md) - Mobile/tablet optimization
- [component-patterns.md](component-patterns.md) - Reusable patterns catalog
