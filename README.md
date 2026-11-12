# CapstoneProject — Student Records

C# Windows Forms student records, enrollment and grading application. The root solution targets **.NET Framework 4.7.2** and now uses a shared blue, white and gold workspace for registrar, coordinator, faculty and administrator accounts. Azure integration remains planned and is **not implemented or deployed**.

## Run locally

Requirements: Windows, Visual Studio 2022 or Build Tools with the .NET desktop workload and .NET Framework 4.7.2 targeting pack, Windows PowerShell 5.1, and a **64-bit Microsoft ACE OleDb provider**. Scripts build x64; the provider architecture must match.

From the repository root in Windows PowerShell:

```powershell
.\scripts\Start-Local.ps1
```

This builds a separate executable under `artifacts/runs`, preserves the five working databases in `CapstoneProject/bin/x64/Debug/Data`, applies backed-up workflow migrations, and opens login. A running older executable does not block the new build. The original repository databases are preserved; existing working copies are never replaced by setup. Close database users when applying a schema upgrade.

Use an existing account from your local database. Successful legacy logins upgrade that account's plaintext password to a salted PBKDF2 hash in the working copy. Role, program and faculty access come from the verified account. Staff can request access from login; an administrator verifies and approves the request before the chosen password becomes active.

To create the **first** administrator, the person responsible for local setup runs:

```powershell
.\scripts\Provision-Admin.ps1
```

Choose the administrator credentials in the setup window. No administrator password is embedded in source or supplied by default. This setup route refuses to create another administrator after one exists. Subsequent account changes require a signed-in administrator. Password assistance is a local review queue; it does not send reset emails. Administrators verify identity through the school's normal process, set a temporary password, and the user must change it before accessing records.

For a read-only preview using synthetic records without login or database setup:

```powershell
.\scripts\Start-Local.ps1 -Preview
```

`CAPSTONE_DATA_DIR` selects a working-copy directory for a directly launched executable. The launch script selects the existing local working directory explicitly. To migrate a different directory, pass its absolute path to `scripts/Initialize-LocalData.ps1 -DataDirectory` and pass the freshly built executable through `-ApplicationPath`. Never select source databases. Migration backups are stored beside each changed working database. To roll back, close the app, preserve the current working files separately and restore the matching pre-migration set. These are file-level backups; there is no distributed transaction across all Access files.

## Verify

For a separate demonstration with freshly generated databases and no default accounts, see [approved fixes and synthetic data setup](docs/APPROVED-FIXES.md). This preparation does not remove existing public database files or Git history.

```powershell
.\scripts\Test.ps1
```

Tests build to a separate verification directory. They exercise production services against disposable Access copies: role/program/class restrictions, registration review, related-record and enrollment rollback, schedule conflicts, faculty reassignment, account approvals, forced password changes, session invalidation, decimal and partial grades, and stale-write rejection. They also render the actual workspace pages and dialogs. `-UnitOnly` explicitly skips Access integration and UI rendering.

The suite also generates WinForms renders under `docs/screenshots` at simulated 100%, 125%, 150%, and 200% scaling. These are not a substitute for testing actual Windows display settings and keyboard workflows. See [implementation and validation notes](docs/IMPLEMENTATION.md).

## Current scope

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
