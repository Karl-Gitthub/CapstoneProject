# CapstoneProject — Student Records

C# Windows Forms student records, enrollment and grading application. The root solution targets **.NET Framework 4.7.2**. This branch preserves the desktop application and begins an incremental refactor; Azure integration is planned and is **not implemented or deployed**.

## Run locally

Requirements: Windows, Visual Studio 2022 or Build Tools with the .NET desktop workload and .NET Framework 4.7.2 targeting pack, Windows PowerShell 5.1, and a **64-bit Microsoft ACE OleDb provider**. Scripts build x64; the provider architecture must match.

From the repository root in Windows PowerShell:

```powershell
.\scripts\Start-Local.ps1
```

This builds the app, copies the five databases from `New folder` into `CapstoneProject/bin/x64/Debug/Data` only when missing, backs up and upgrades the working grading database's average column to DOUBLE, and opens login. Close running app instances before rebuilding the normal executable. The original databases are preserved. Existing working copies are never replaced by setup.

Use an existing account from your local database. Successful legacy logins upgrade that account's plaintext password to a salted PBKDF2 hash in the working copy. Role routing comes from the stored account. The desktop administrator bypass has been removed; account provisioning is unavailable in this phase.

For a read-only preview using synthetic records without login or database setup:

```powershell
.\scripts\Start-Local.ps1 -Preview
```

`CAPSTONE_DATA_DIR` can select a working-copy directory. To migrate that directory, pass its absolute path to `scripts/Initialize-LocalData.ps1 -DataDirectory`. Do not point it at the original source databases. Close database users before migration; backups are stored beside the working grading database. To roll back, close the app, preserve the current working file separately and restore the matching pre-migration backup.

## Verify

```powershell
.\scripts\Test.ps1
```

Tests build to a separate verification directory so an open app does not block them. They exercise production services against disposable Access copies, including registration rollback, parameterized input, duplicate IDs, password upgrades, decimal average persistence, semester-specific grade saves, and stale-edit rejection. No original databases are edited. `-UnitOnly` skips Access integration and UI rendering explicitly.

The suite also generates WinForms renders under `docs/screenshots` at simulated 100%, 125%, 150%, and 200% scaling. These are not a substitute for testing actual Windows display settings and keyboard workflows. See [implementation and validation notes](docs/IMPLEMENTATION.md).

## Current scope

- Redesigned login and registrar student workspace with search and program filtering.
- Student entry grouped into identity, contacts/address, prior education and credentials.
- Student creation commits all five related inserts together and reports success after commit.
- Decimal averages with the existing lower-bound conversion thresholds; a grade below 75 converts to 5.00. Equal weighting is retained. Missing inputs are rejected; an explicitly entered zero is valid.
- Grade saves use the selected `course_id` and original values for optimistic concurrency.
- Portable database paths for the active Access form connections.

Legacy enrollment, faculty/coordinator and report screens remain only partly refactored. Some retain concatenated SQL, incomplete workflows, and older navigation behavior. This branch is a local development baseline, not a completed security modernization. The nested `CapstoneProject/CapstoneProject` project is preserved but is not built by the root solution. Photo persistence is not implemented because the inspected field is text rather than image storage. Missing image resources are omitted; institution branding is retained as text.

The next phase is an authenticated ASP.NET Core API on Azure App Service with server-side role/assignment checks and Azure SQL through managed identity. Institutional identity configuration, SQL data migration and deployment remain future work.
