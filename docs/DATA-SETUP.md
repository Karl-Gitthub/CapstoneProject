# Local data setup and academic correctness

The application retains WinForms and its blue, white and gold design. Fresh installations use generated sample databases. Existing working data is preserved. Azure services are planned separately.

## Grade persistence

Period values use invariant decimal strings (`8.5`) at the service/database boundary. Grid formatting and parsing use the Windows culture, without accepting thousands separators. On a German locale the user sees and enters `8,5`; the bound value and persisted value remain `8.5`. Averages retain decimals. Blank periods remain incomplete rather than zero.

Ambiguous stored strings such as `8,5` are rejected instead of being silently reinterpreted. Historical grades are not automatically rewritten. Any previously corrupted records require review against the school's source records.

## Program transfers

Grade authorization follows the student's current authoritative program, matching the existing profile access policy. Coordinators of the new program can see historical grades; the previous program loses that access. A coordinator cannot rely on a legacy academic snapshot when the primary student identity is missing. Registrars retain their existing academic-record visibility. This fixes consistency with the current policy; a different historical-access policy would need a separate decision.

## Enrollment and assignment consistency

Both workflows acquire the existing academic lock keys in the same order. Enrollment reads and validates the current offerings inside that locked transaction, before writing its header, lines, faculty assignments, and course metadata. A schedule or instructor edit cannot commit between validation and insertion. Student registration state is still held stable separately, and the existing stale-version checks and academic rollback behavior remain.

Deploy updated clients together and close older clients before upgrading. This is cooperative locking inside the application; direct database writers can bypass it. The separate Access files still do not form a distributed transaction.

## Fresh synthetic seed data

Build the application, then run from the repository root in 64-bit Windows PowerShell with the ACE provider installed:

```powershell
.\scripts\Build.ps1
.\scripts\New-SyntheticData.ps1 -OutputDirectory .\artifacts\demo-data
```

Choose a directory that does not exist. The generator refuses to overwrite any existing directory and writes a completion manifest only when successful. An interrupted directory is incomplete; choose a new destination after inspecting it.

The generator creates five new Access databases from explicit schema definitions. It never opens or copies a source database. Consequently, old records, deleted pages, stored queries, and passwords are not carried into the output. Identifiers use short text, and the student/academic legacy relationships are recreated. This is a compatible demonstration schema, not a migration of existing records.

The demonstration contains two clearly labelled sample students, two sample faculty profiles, and four example courses. Contact addresses use `example.invalid`. Students start at program review. There are **zero accounts, no default password, no grades, and no claimed official curriculum**. The temporary-registration database is empty. Workflow tables are initialized by the current executable.

To open this separate demonstration, use the freshly built executable with an explicit data directory:

```powershell
$env:CAPSTONE_DATA_DIR=(Resolve-Path .\artifacts\demo-data).Path
& .\CapstoneProject\bin\x64\Debug\CapstoneProject.exe --provision-admin
& .\CapstoneProject\bin\x64\Debug\CapstoneProject.exe
```

Create your own first-administrator credentials in the setup dialog. Existing accounts are preserved. The launch and provision scripts also accept `-DataDirectory`; pass the same directory to both scripts when using a separate demonstration.

`-SchemaOnly` produces empty base schemas without sample records or application migration. The regression runner generates these schemas and copies them into disposable fixtures, rather than copying committed account/student databases. This keeps the legacy compatibility tests while removing their dependence on exposed seed records.

## Validation and remaining exposure

The current update passed **249 checks in a complete isolated run** on 13 November 2026. Earlier targeted legacy-format probes returned the correct 49.5 average and both historical grade rows for the receiving coordinator. Sample generation and overwrite protection remain covered. The two pre-existing unused-field compiler warnings in the legacy faculty form remain. See [release readiness](READINESS.md) for coverage, the earlier native migration stall and remaining work.

Run `scripts/Test.ps1` for calculation, locale, service, transfer-authorization, transactional enrollment, competing-client, actual grid-edit, and rendering checks. UI renders do not substitute for actual OS DPI, screen-reader, or printer testing. Test-generated files and seed binaries belong under ignored `artifacts`.

Legacy database binaries and the record-data SQL export have been removed from the current tracked source. Their local copies remain ignored, and setup generates fresh sample databases instead of reading those files. Earlier Git revisions still contain the old records and password values. Any credentials that were used must be invalidated separately, and history cleanup needs a coordinated decision. Removing an old file does not revoke a copied password.

Database constraints on the newer workflow tables, durable authentication controls, richer audit records, report improvements, academic lifecycle features, and Azure identity/API deployment remain later audit items.
