# SPCC design review — blue, white and yellow

This is a set of 25 proposed interface pictures based on the current source-code navigation. It preserves the Windows desktop product and academic administration context. All names, identifiers, counts and records are synthetic. These samples do not add working features to the application.

Open `index.html` for the complete gallery. Each screen is supplied as a 1600 × 1000 PNG plus an editable SVG. `overview.png` is the contact sheet. `manifest.json` contains a short, screen-specific critique and recommendation for every sample.

## Palette and visual rules

| Color | Hex | Purpose |
| --- | --- | --- |
| Institutional navy | `#12335B` | Sidebar, headings, primary actions |
| Link blue | `#245B93` | Links, focus and secondary interaction |
| Warm gold | `#E7B641` | Small brand accents, selected navigation marker, attention actions |
| White | `#FFFFFF` | Forms, record tables, document previews |
| Cool neutral | `#F4F6F8` | Workspace background |
| Body ink | `#203247` | Main text |
| Muted ink | `#5B6B7B` | Supporting text |
| Subtle border | `#D8E0E8` | Form boundaries and row separation |
| Soft yellow | `#FFF5D8` | Pending requirements and review notices |

Use navy text on yellow, and white text on dark blue. Yellow is an accent with a job; it should not become the default button color or fill the whole workspace. Green and red appear only when a status requires them, always with a written label. Blue, white and yellow remain the identity palette.

Use Segoe UI, sentence case, a compact 29 px page title, 14–15 px main text, and 12–13 px secondary labels at the mockup scale. Use 8 / 16 / 24 / 32 px spacing, small 3 px corners, thin horizontal table rules and flat surfaces. Keep native window controls, a visible keyboard focus ring and one main action for each task. Do not rely on hover to reveal essential commands.

## The changes that matter most

1. **Reduce the sidebar.** The registrar currently has many long “Generate…” buttons. Replace them with Workspace, Students, Enrollment, Grades, Evaluation and Reports. Put report types inside Reports. Keep Sign out at the bottom, away from record actions.
2. **Give each role a relevant workspace.** Registrar staff need incomplete records and review queues. Coordinators need pending registrations and teaching assignments within their program. Faculty need assigned classes and grade completion. Derive these scopes from authenticated access rules.
3. **Keep the selected student visible.** Student number, name, program and year level should stay on screen across profile, enrollment, grades and credentials. Avoid repeatedly asking users to retype the same identifiers.
4. **Use records and grids as the main content.** The application is an academic records tool. Large decorative charts, giant greetings, arbitrary activity graphs and repeated metric cards would displace useful rows.
5. **Organize student entry.** Use Identity, Contact & address, Prior education and Credentials. Validate required fields in place, calculate age, preserve edits after failure, and confirm success only after the full transaction commits.
6. **Make enrollment flexible.** Replace the fixed eight-subject field groups with a subject grid. Show term, section, units, schedule and conflicts together. Save draft and Confirm enrollment should have distinct meanings if that workflow is implemented.
7. **Make grade entry deliberate.** Edit a class grid, keep calculated fields read-only, flag missing values, and show unsaved changes and stale-record conflicts. Never turn a blank grade into zero. Store decimals; format only for display.
8. **Treat output as documents.** Reports should have a filter stage and a separate print preview. Use the school’s approved margins, headers, signatories and pagination when implementing official documents. Sample report pictures are not valid documents.
9. **Replace the admin gate with controlled access.** “Request staff access” is the proposed sign-up route. Administrators approve requests or invite staff, assign roles and program scope, and record the change. Public sign-up must not grant registrar or coordinator privileges.
10. **Design recovery states.** No matches, loading, invalid input, save failure, success and concurrent changes need explicit text and a next action. Keep error messages beside the affected task and preserve user input.

## Screen coverage and original-form mapping

| Existing area or form | Sample pictures |
| --- | --- |
| `LoginForm` | 01 Login |
| `UserFormDirectory` | 02 Request staff access; 23 Staff access administration |
| Password recovery, not currently implemented | 03 Password recovery |
| `RegistrarDirectoryForm` | 04 Registrar workspace; 05 Student directory; 06 Student profile |
| `StudentInformationSheet` | 07 Identity; 08 Contact & address; 09 Prior education; 10 Credentials |
| `RegFormDirectoryRegistrar` and `RegFormDirectoryProgramcoodinator` | 11 Enrollment; 13 Pending registration review |
| `ProgramCoordinatorDirectoryForm` | 12 Coordinator workspace; 13 Registration review |
| `AddFacultyDetailandLoading` | 14 Faculty details and loading |
| `FacultyDirectoryForm` | 15 Faculty workspace; 16 Grade entry; 20 Computed grades |
| `RegistrarGradeFormDirectory` and `FacultyGradeFormDirectory` | 16 Grade entry with role-appropriate scope |
| `Evaluationform` | 17 Curriculum evaluation |
| `RegistrarGCGDirectoryForm` | 18 Reports; 19 Copy of grades |
| `CopyofComputedGradesDirectory` | 20 Computed class grades |
| `AlphaListDirectoryForm` | 21 Alphalist preview |
| `RegistrarTORDirectoryForm` | 22 Transcript preview |
| Shared sidebar, button, field and status rules | 24 Navigation and interaction states |
| Shared empty, save-failure, stale-record and success patterns | 25 Feedback states |

The nested duplicate project is represented through the same workflow families rather than generating duplicate pictures for identical screens. Faculty profile and assignment editing are shown together in sample 14, but should be separate sections with distinct permissions in the implementation.

## Navigation by role

**Registrar:** Workspace → Students → selected student → profile/enrollment/grades/credentials. Reports → choose document → provide student or class context → preview → print/export.

**Coordinator:** My workspace → Pending students → registration review → request correction or approve. Enrollment, Evaluation and Faculty loading stay scoped to the assigned program.

**Faculty:** My workspace → My classes → selected class → Grade entry or Computed grades. No editable instructor-username field should control access.

**Administrator, proposed:** Staff accounts → invite/manage account; Access requests → review request; Roles & permissions → assign controlled scope. Changes need server-side authorization and audit records.

## Existing versus proposed behavior

The running branch already has a redesigned login, student search/program filtering, grouped student entry, transactional registration and tested decimal grading services. This design study proposes a coherent visual extension across the remaining screens.

Access requests, email recovery, account invitations, approvals, dashboard queues, document export, review notes and a complete permissions administration area are proposed workflows. The pictures do not prove those features exist. Do not present their sample counts as live database measurements. The exact institutional sign-up and identity-provider flow remains to be configured.

## Implementation order

First align the shared palette, typography, navigation and fields. Next apply the student record shell and the enrollment/grade grids. Then redesign the coordinator and faculty workspaces around their permission scopes. Add the report preview system after the source data and document templates are verified. Implement account requests and password recovery only when the identity service and approval rules are defined.

Validate contrast, keyboard navigation, 100–200% Windows display scaling, field error placement and long names. Check that grids retain useful columns at the minimum window size. These static images were rendered and visually reviewed as samples; interactive behavior and actual WinForms DPI behavior need testing during implementation.

## Artifact checks

All 25 PNGs were checked for the expected 1600 × 1000 resolution and a nontrivial file size; all 25 SVGs were parsed as XML. The contact sheet was inspected as a whole, along with full-size samples of login, registrar work, credentials, grade entry, evaluation, reports and feedback states. Eight intended text/background color pairs meet a 4.5:1 contrast threshold; see `validation.json`. This checks the chosen color pairs, not every possible future rendered state.

To regenerate with Node.js and local Google Chrome, run from this folder:

```powershell
node generate.cjs
.\render.ps1
node overview.cjs
.\render.ps1 -OverviewOnly
node validate.cjs
```
