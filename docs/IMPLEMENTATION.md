# Desktop foundation — implementation and validation

Source baseline: commit `0289ea4818abe94f3b93d1b591c41dd27b8b9d95`. Work is isolated on `codex/winforms-foundation`; the unrelated Deskflow application outside this checkout is unchanged.

## Changes

The original build failed on six absent image resources. Their broken resource entries and project references were removed without fabricating institutional assets. The application retains .NET Framework 4.7.2 and the original forms/project copies.

Login now uses a parameterized username lookup, verifies the credential separately and derives the role from storage. Legacy plaintext credentials are upgraded to salted PBKDF2-SHA256 after successful sign-in. Five failed UI attempts impose a short local cooldown. This is local-client behavior and does not replace server-side authentication, lockout or authorization.

Student registration uses the inspected Access schema, including the actual credential column names. The original empty-command replacement, premature success notification and unused binary photo conversion were removed. Five inserts run in a transaction; the existing student identifier primary key remains intact. Required names/ID, dates, program, field length and email are validated. Input remains available after failure.

The user selected decimal averages. The calculator retains equal weighting and the original lower bounds: 96, 94, 92, 89, 87, 84, 80, 78 and 75. A result of 95.67 therefore converts to 1.25, while 96.00 converts to 1.00. Calculations use decimal; Access persists the average as DOUBLE, with normal binary floating-point storage limits. The UI displays two decimals without rounding before conversion.

Grade retrieval accommodates ACE's restriction on joining Memo columns: short-text join expressions are followed by a full identifier comparison. Updates identify the selected enrollment through `course_id`, compare the original grade values and reject stale writes. The other semester's enrollment remains unchanged.

The login and student workspace use restrained typography, navy branding, native window controls and table-based layouts. Student entry reuses existing field controls in organized tabs. A read-only synthetic workspace preview is explicitly separate from authenticated database access.

## Validation

On 12 November 2026, the x64 build and all **51 regression checks passed** on this machine with the installed ACE provider. The login, student workspace and student-entry renders were inspected. The grade join incompatibility found during initial implementation is fixed and covered by the passing database round-trip tests.

Run `scripts/Test.ps1` under 64-bit Windows PowerShell with ACE installed. Tests use the compiled production services, not copied calculation logic. Disposable database copies and isolated verification builds are written under ignored `artifacts/` directories. Original database files are not staged or modified.

Coverage includes decimal calculation and conversion boundaries; invalid grades; date/email/program validation; password hashing and legacy upgrade; quoted names; injection-shaped login/search input; program filtering; duplicate IDs; rollback after a failure at the final related insert; decimal persistence; enrollment scoping; and concurrency rejection.

UI rendering checks materialize the controls and reject blank renders at simulated scaling factors of 1, 1.25, 1.5 and 2. Actual Windows DPI changes, comprehensive keyboard interaction, and every legacy screen have not been verified. Live computer-use verification was interrupted by the user during the initial implementation.

## Screens

- [Original login after missing-resource repair](screenshots/before-login.png)
- [Updated login](screenshots/login-1.png)
- [Student workspace using synthetic records](screenshots/students-1.png)
- [Student entry](screenshots/add-student.png)

## Remaining work

Azure sign-in configuration is deferred at the user's request. No API, Azure resources, cloud connectivity or SQL migration has been claimed. Legacy screens still need query parameterization, permission checks, navigation cleanup and workflow tests. Account provisioning and photo storage remain unavailable. Never describe the complete application as production-ready based on this subset of tests.
