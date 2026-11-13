# Shared workspace implementation

The blue, white and yellow design is implemented as native Windows Forms screens on .NET Framework 4.7.2. `LoginForm` opens `Presentation/SchoolWorkspace` for every role. Active navigation uses the shared workspace and authenticated services.

## Working workflows

| Area | Actual behavior |
| --- | --- |
| Login | Parameterized credential lookup, role from storage, legacy-password upgrade, temporary-password gate |
| Staff sign-up | Request details and password tabs; hash stored pending administrator approval; requesting a department never grants a privileged role |
| Password assistance | Local request queue; administrator records identity verification and supplies a temporary password; no email-delivery claim |
| Registrar | Actual review queue, student search/profile/create/edit, registration approval, enrollment, grades, faculty loading, evaluation and reports |
| Coordinator | Program-scoped student review, correction/forwarding, enrollment drafts, evaluation and teaching assignments |
| Faculty | Assigned classes, scoped grade editing, partial-period saves and read-only computed-grade reports |
| Administrator | Academic workspace, student records, enrollment, grades, evaluation, faculty loading and reports; first-account setup, staff access review, role/faculty mapping, account disable, password assistance and UTC audit history |
| Student editing | Five related records saved in one transaction; optimistic version check; unsaved input retained on errors |
| Enrollment | Offered-class selection, draft versioning, duplicate/schedule checks, registration approval required for confirmation; header/lines/course records commit together |
| Assignment editing | Instructor/section/time edits preserve course and term, check schedule collisions, and update linked grade access atomically |
| Grades | Validate entered values 0–100; blanks stay incomplete; derive average/equivalent only when all periods exist; one transaction for the changed rows |
| Evaluation | Catalog requirements beside recorded attempts; repeated attempts explicitly flagged for review |
| Reports | Saved grade/roster data, paginated print preview, print dialog and escaped CSV export; internal-copy labels |

## Structure

`Presentation/Theme` owns colors and shared controls. `SchoolWorkspace` manages navigation, async loading, stale-result suppression, save feedback and unsaved-change handling. Its Students, Academic, Accounts and Reports partials keep each workflow separate. `StudentEditor`, `FormDialog` and `ReportViewer` handle their own modal lifecycle and disposal.

`SchoolService` is the authenticated workflow boundary. `AccessPolicy` checks current account status/version and role/program restrictions. `StudentStore`, `GradeBatch` and `AccountAdministration` own transaction and concurrency rules. Database commands use positional parameters and explicit types. User values never supply table or column identifiers; program catalog identifiers come from a fixed BSIT/BSBA whitelist.

Fresh installations generate sample Access databases from the schema in `scripts/New-SyntheticData.ps1`; database binaries and record exports are excluded from the current tracked source. Existing working databases remain local. `SchemaMigrator` adds version-3 workflow tables and account metadata, taking file backups first. Existing account roles are preserved. First-admin setup requires credentials chosen locally.

Registration state is locked while confirming enrollment. Teaching-assignment and account-admin writes are serialized inside their respective database transactions. The migrations themselves are not distributed transactions across files. Restore the complete matching backup set if a schema upgrade must be rolled back.

## Design decisions

- Navy `#12335B`, white surfaces and gold `#E7B641` selection markers use the proposed palette.
- Native title bars, Segoe UI, quiet separators and compact tables preserve desktop usability.
- Long toolbars wrap at the minimum window size. Student identity precedes grade columns.
- The sidebar is built from the authenticated role. Controls that are absent from navigation are still protected by service checks.
- Empty queues display actual zero counts. The running app does not import the design study's sample metrics.
- Add/edit, correction, stale-record and save errors keep the current entry available for recovery.

## Verification

On 12 November 2026, the x64 build and **126 regression/render checks passed** with the installed ACE provider. `scripts/Start-Local.ps1 -NoLaunch` also completed the real working-copy migration successfully without replacing source databases. The raw final test log is kept under the ignored verification build directory, and the summary is recorded in `WORKSPACE-VALIDATION.json`.

Run `scripts/Test.ps1`. The suite uses compiled production services and disposable copies of all required databases. It covers prior regressions plus role/program/class isolation, student edits, review transitions, schedule conflicts, draft/enrollment concurrency, late enrollment rollback, decimal/partial grades, whole-batch grade rollback, instructor reassignment, access approval, password recovery review, forced password changes, account revocation and audit writes.

The suite renders 14 actual workspace routes plus the student editor, staff access request, password change, assignment editor and report viewer. It asserts the requested route loaded, checks assignment selectors retain their actual saved values, and checks the minimum workspace size. Images are in `docs/screenshots/runtime`. All shown records are synthetic fixture data. Existing foundation renders also cover simulated 100–200% scaling; actual OS DPI changes and physical printer output have not been tested.

## Operational limits

This is a local desktop implementation. Filesystem access to the Access databases bypasses application-level account checks; those checks are not a substitute for hosted authorization. Azure API/SQL deployment and institutional identity remain separate work. Legacy SQL forms remain in source and must not be reintroduced into the signed-in routes without review.

Prerequisite enforcement, certified school document formatting, signatures/seals, document/photo upload, automated reset emails, confirmed-enrollment amendments/withdrawals and comprehensive retention policies need school-specific rules before implementation. Reports intentionally say internal copy. Historical grade-only records may lack source student details or normalized class metadata and require data reconciliation before official migration.
