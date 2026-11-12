# Local data and repository setup

The source repository contains schema and sample-data generation code. Access databases, lock files, database backups, and record-data SQL exports stay outside version control. `SQLQuery2.sql` contains schema only; the previous address-record SQL export is excluded.

## Fresh installation

Run in 64-bit Windows PowerShell with the Visual Studio desktop build tools and ACE provider installed:

```powershell
.\scripts\Provision-Admin.ps1
.\scripts\Start-Local.ps1
```

Setup creates five new Access databases in `CapstoneProject/bin/x64/Debug/Data`, with two clearly labelled sample students, two faculty profiles, and four sample courses. No account or password is supplied. Choose credentials in the first-administrator dialog.

For a separate demonstration directory, use the same path in both commands:

```powershell
.\scripts\Provision-Admin.ps1 -DataDirectory .\artifacts\demo-data
.\scripts\Start-Local.ps1 -DataDirectory .\artifacts\demo-data
```

The generated curriculum is illustrative. Replace sample records through the application before using approved school data.

## Existing installation

Setup preserves a complete existing set of five working databases and applies the current backed-up migrations. It does not reset accounts or insert samples into an existing installation. Partial sets and nonempty unrelated folders are rejected. Restore matching files or choose a separate empty directory rather than mixing databases from different installations.

The working directory must be protected through Windows permissions. Desktop authorization cannot prevent a person with direct database-file write access from changing data. Backups sit beside the working files; off-device retention and a tested operational restore process still need to be established before real shared use.

## Earlier repository revisions

Fifteen legacy database files and one address-record SQL export were removed from the current tracked tree. Their local originals were preserved and are ignored. Old Git commits still contain these files. No credentials were rotated, no historical commits were rewritten, and no repository privacy setting was changed.

Any exposed credentials that were actually used need invalidation by the account owner. Historical data removal requires a coordinated history-cleanup decision; deleting a current file alone does not remove previous revisions or revoke copied passwords.

## Verification

`scripts/Test.ps1` runs the application regression suite and separate local-setup checks. Setup checks use disposable data to verify fresh initialization, zero default accounts, sample counts, preservation of existing edits, empty-directory initialization, incomplete-set rejection, unrelated-file preservation, legacy-source protection, and failure before data creation when the executable is missing.

On 12 November 2026, a clean source snapshot containing zero database binaries built successfully and passed all 143 application checks and nine setup checks (152 total). Only the two existing unused-field warnings in the legacy faculty form remained.
