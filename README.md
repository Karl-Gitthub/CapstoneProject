# CapstoneProject — Student Records

A C# Windows Forms application for student records, enrollment and grading, maintained by [Karl-Gitthub](https://github.com/Karl-Gitthub). The root solution targets **.NET Framework 4.7.2**, with blue, white and gold workspaces for registrar, coordinator, faculty and administrator accounts. Azure integration is planned.

## Run locally

For a separate sample-data demonstration, double-click **Start Demo.cmd** or run `scripts/Start-Demo.ps1`. It opens administrator setup on the first run, then login, and keeps records under `artifacts/demo/presentation/data`. See the [demonstration walkthrough and reset instructions](docs/DEMO.md).

Requirements: Windows, Visual Studio 2022 or Build Tools with the .NET desktop workload and .NET Framework 4.7.2 targeting pack, Windows PowerShell 5.1, and a **64-bit Microsoft ACE OleDb provider**. Scripts build x64; the provider architecture must match.

Double-click **Start Main.cmd** to open the working application. From the repository root, the equivalent command is:

```powershell
powershell.exe -NoProfile -ExecutionPolicy Bypass -File .\scripts\Start-Local.ps1
```

Launchers build Release executables under `artifacts/runs` by default; use `-Configuration Debug` for a developer build. Dependency, provider, write-access and path checks run before setup. A fresh installation generates five Access databases with clearly labelled sample students, faculty and courses, opens administrator setup, and continues to login after successful setup. Canceling setup exits without creating an account. There are no default credentials.

Working data remains in `CapstoneProject/bin/x64/Debug/Data` for compatibility, including when the executable is a Release build. Existing complete databases and accounts are preserved. Incomplete sets are rejected. Close database users when applying a schema upgrade. Role, program and faculty access come from the verified account. Staff can request access from login; an administrator verifies and approves the request before the chosen password becomes active.

First-run setup is automatic. To open the guarded first-administrator setup separately:

```powershell
powershell.exe -NoProfile -ExecutionPolicy Bypass -File .\scripts\Provision-Admin.ps1
```

Choose the administrator credentials in the setup window. No administrator password is embedded in source or supplied by default. This setup route refuses to create another administrator after one exists. Subsequent account changes require a signed-in administrator. Password assistance is a local review queue; it does not send reset emails. Administrators verify identity through the school's normal process, set a temporary password, and the user must change it before accessing records.

For a read-only preview using synthetic records without login or database setup:

```powershell
.\scripts\Start-Local.ps1 -Preview
```

Use `-DataDirectory` with both `Start-Local.ps1` and `Provision-Admin.ps1` to select the same separate data directory. `CAPSTONE_DATA_DIR` selects data for a directly launched executable. Migration backups are stored beside each changed database. To roll back, close the app, preserve the current working files separately and restore the matching pre-migration set. There is no distributed transaction across Access files.

## Verify

See [data setup and correctness notes](docs/DATA-SETUP.md) and [data handling](docs/DATA-SAFETY.md). Database binaries and record exports are excluded from the current source tree. Older Git revisions still contain legacy files; password invalidation and history cleanup remain outstanding.

```powershell
.\scripts\Test.ps1
```

Tests build to a separate verification directory. They exercise production services against disposable Access copies: role/program/class restrictions, registration review, related-record and enrollment rollback, schedule conflicts, faculty reassignment, account approvals, forced password changes, session invalidation, decimal and partial grades, and stale-write rejection. They also cover first-run completion/cancellation, safe class switching, prerequisite failures, automatic demo detection, visible launch windows, and workspace/dialog renders. `-UnitOnly` explicitly skips Access integration and UI rendering.

The suite also generates WinForms renders under `docs/screenshots` at simulated 100%, 125%, 150%, and 200% scaling. These are not a substitute for testing actual Windows display settings and keyboard workflows. See [implementation and validation notes](docs/IMPLEMENTATION.md).

## Current scope

See [role access and workflow](docs/ROLE-ACCESS.md) for each staff role's responsibilities, and [release readiness](docs/READINESS.md) for the 240-check verification and remaining work before real school deployment.

For adviser and staff review, follow the [user acceptance test guide](docs/UAT.md) and complete its results checklist. `scripts/Test-Acceptance.ps1` runs a separate automated rehearsal of both programs using fresh disposable data; human acceptance remains a separate decision.

- One role-aware shell, shared sidebar, consistent fields/tables and visible selection and feedback states.
- Student search, profile, grouped create/edit forms, credential status and coordinator-to-registrar review.
- Faculty profiles, course assignments and section/instructor/student schedule-conflict checks.
- Enrollment drafts and confirmation with atomic academic-record creation. Confirmed enrollment is protected from replacement through the draft editor.
- Class-scoped grade editing, decimal calculation, partial-period saves, atomic batch updates and stale-write protection.
- Curriculum evaluation showing all recorded attempts without silently choosing a repeated-course result.
- Saved-record reports, print preview/printing and formula-safe CSV export. Reports are marked internal copies.
- Staff access approval, account disable/role changes, password assistance, forced temporary-password changes and audit history.
- Backed-up additive migrations on portable working Access databases.

Legacy forms and the nested duplicate project remain in source for reference and compatibility, but normal sign-in uses the new shell and parameterized service workflows. The old read-only `-Preview` is a separate historical student-list preview. Photo uploads, official certified transcript templates, prerequisite rules, enrollment withdrawal/amendment, email delivery and Azure identity are not implemented. A desktop account check cannot protect Access files from someone who already has direct filesystem access; protect those files and move authorization to a hosted API before real shared deployment.

The next phase is an authenticated ASP.NET Core API on Azure App Service with server-side role/assignment checks and Azure SQL through managed identity. Institutional identity configuration, SQL data migration and deployment remain future work.
