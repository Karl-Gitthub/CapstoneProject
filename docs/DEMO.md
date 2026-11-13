# Student Records demonstration

This is a working local Windows Forms demonstration. It saves actual changes to a separate sample database. Azure hosting and a browser portal are not part of this release.

## Start

Double-click **Start Demo.cmd** in the repository folder, or use the PowerShell command below.

Open the repository folder in 64-bit Windows PowerShell. Visual Studio desktop build tools, the .NET Framework 4.7.2 targeting pack, and a matching 64-bit ACE provider must be installed.

```powershell
.\scripts\Start-Demo.ps1
```

On the first launch, a visible administrator setup window opens. Choose your own username and a password of 12–128 characters, then complete setup. The login screen opens afterward. If setup is canceled, the launcher exits without creating an account; run it again when ready.

The login and workspace display **DEMO · SAMPLE DATA**. Records persist between launches. Close the current demo before starting it again. There is no built-in username, password, or authentication bypass; use passwords different from those used for real accounts.

The selected data directory determines demo mode. Opening a managed session's `data` directory through the main launcher or directly through the executable also shows the demo label and holds the reset guard. Fresh generation is staged before activation, so reset/setup can safely keep its exclusive lock.

Launchers use Release builds by default and check the x64 provider, targeting pack, writable paths and database access. Use a shorter location if the path check fails. The CMD launchers apply execution-policy handling only to their PowerShell process. Build tools and ACE remain required; this is not an installer.

To prepare data without opening windows:

```powershell
.\scripts\Start-Demo.ps1 -NoLaunch
```

## Data and accounts

The default demonstration uses `artifacts/demo/presentation/data`, separate from the working application directory. The generated data contains:

| Record | Identifier |
|---|---|
| Miguel Antonio Dela Cruz, BSIT | `SAMPLE-BSIT-001` |
| Alyssa Mae Reyes, BSBA | `SAMPLE-BSBA-001` |
| Gabriel Luis Navarro, BSIT faculty | `SAMPLE-FAC-BSIT` |
| Patricia Anne Mendoza, BSBA faculty | `SAMPLE-FAC-BSBA` |
| Database Management Systems; Application Development and Emerging Technologies | `SAMPLE-BSIT-201`, `SAMPLE-BSIT-202` |
| Financial Management; Marketing Management | `SAMPLE-BSBA-201`, `SAMPLE-BSBA-202` |

Courses and student records are illustrative. No grades or enrollments are pre-approved. Creating and approving them is part of the demonstration.

All names and record details are fictional. Student profiles include demographics, birth/registration dates, addresses, prior education and document statuses. Phone numbers are masked and emails use `example.invalid`. An empty suffix means no suffix applies. School/address labels and course descriptions identify demonstration data; these are not official curriculum or school records. Select a sample student with a registrar account and open **Edit student** to see the populated tabs. New student forms remain blank for entry.

To update an existing demo's original sample profiles without resetting accounts or academic work, close its windows and run:

```powershell
powershell.exe -NoProfile -ExecutionPolicy Bypass -File .\scripts\Refresh-DemoProfiles.ps1
```

This creates a full backup under the session's `backups/profiles-...` folder, updates only the `SAMPLE-` student/faculty/course records, preserves account database bytes, saved grades, enrollment identifiers, review decisions and unrelated students, and restores the old files if an update fails. Refreshing profile details increments their record versions and writes a fixture audit entry. A future reset also generates these richer profiles. The main working database is not targeted by this refresh command.

Prepare the following role accounts once before presenting. At login, use **Request staff access**, enter sample contact details ending in `@example.invalid`, and choose a password. Sign in as administrator, open **Access requests**, select the request, and explicitly approve its role with a verification note.

| Suggested username | Approved role | Faculty identifier |
|---|---|---|
| `demo.registrar` | `REGISTRAR` | Leave blank |
| `demo.bsit` | `BSIT PROGRAM COORDINATOR` | Leave blank |
| `demo.faculty` | `FACULTY` | `SAMPLE-FAC-BSIT` |

These are suggested usernames, not supplied accounts. The administrator's account manages access; use the registrar and coordinator accounts for the academic workflow. Do this rehearsal before the presentation to avoid spending the live demonstration creating accounts.

Administrator access is limited to staff accounts, access requests, password assistance and audit history. Sign in as registrar to open **Students** and inspect Miguel's or Alyssa's completed fictional profile. Coordinators review registrations from **Pending students** and prepare **Enrollment drafts**; the registrar uses **Registration approvals** and confirms enrollment. Only assigned faculty enter grades. See [role access and workflow](ROLE-ACCESS.md).

The locally provisioned presentation accounts and generated passwords are listed in ignored `artifacts/demo/presentation/ROLE-ACCOUNTS.md`. They do not apply to the main database or fresh downloads, and a demo reset removes them from the active demo.

## Walkthrough

1. **Review registration.** Sign in as the BSIT coordinator. Open **Pending students**, select `SAMPLE-BSIT-001`, review the profile, add a note, and choose **Approve and forward**. Explain that this account cannot approve a student outside BSIT.
2. **Approve registration.** Sign in as the registrar. Open **Registration approvals**, review the same student, and complete **Approve registration**. Show the updated review status.
3. **Assign classes.** As coordinator, open **Faculty loading** and create two BSIT offerings for academic year `2026-2027`, **First semester**, section `2A`, and faculty `SAMPLE-FAC-BSIT`. Use day codes `M,W`: course 201 at `09:00–10:30`, course 202 at `10:30–12:00`. A conflicting instructor or section schedule should be rejected.
4. **Enroll the student.** As coordinator, open **Enrollment drafts**, enter the student and matching year/semester, load the classes, select both offerings and save a draft. Sign in as registrar, open **Enrollment**, load that student and term, review the selected classes, then confirm. Show that enrollment requires approved registration and rejects overlapping classes.
5. **Enter grades.** As faculty, open **My classes**, choose the appropriate class, then **Grade entry**. Enter only prelim and save to show an incomplete average. Then enter prelim `95.5`, midterm `96`, and final `95`. Use your Windows decimal separator when entering values. Save: the average is approximately `95.50`, with converted grade `1.25`.
6. **Show reporting.** As registrar, open **Evaluation** for the student, then **Reports**. Select the student, year, and semester, load saved records, and preview or export a copy of grades. Reports remain internal copies.
7. **Show traceability.** As administrator, open **Audit history** and show the registration, enrollment, grade, and account actions created during the demonstration.

Keep each step's role visible. In grade entry, **Loaded class** identifies the editing table. Choosing a different class disables editing, saving and printing until **Open class** is clicked. Declining the discard prompt restores the old selection and retains unsaved grades. Print/export uses saved records; save changes first.

## Reset for another rehearsal

Close every demo login, administrator, and workspace window, then run:

```powershell
.\scripts\Reset-Demo.ps1
.\scripts\Start-Demo.ps1
```

Reset first generates a complete replacement. It then moves the previous demo data into `artifacts/demo/presentation/backups/<timestamp-and-id>` and activates the new sample set. Accounts and edits from the old demo remain in that archive. The next launch asks for new administrator credentials, and role accounts need to be prepared again.

Reset refuses an open demo, an unrecognized session, a path outside the dedicated session, or a symbolic link/junction. There is no automatic deletion of backups and no change to the working application database. A failed replacement-generation step leaves the current demo active. If an I/O error prevents rollback, preserve both directories and restore the archived set after resolving the file access problem.

For independent rehearsals, pass the same name to both commands:

```powershell
.\scripts\Start-Demo.ps1 -Name rehearsal
.\scripts\Reset-Demo.ps1 -Name rehearsal
```

Names contain only letters, numbers, hyphens, or underscores. Each name selects its own folder under `artifacts/demo`.

## Open the demo database

Close the demo, open Microsoft Access, and browse to `artifacts/demo/presentation/data`. The five `.accdb` files have the same purposes as the working databases described in [data handling](DATA-SAFETY.md). Use the application for ordinary edits so validation and audit records are applied. Do not manually replace demo files while a demo is running.

All generated databases, accounts, archives, lock files, and builds remain under ignored `artifacts`. Nothing in this workflow uploads student data or creates Azure resources.

## Verification

The subsequent navigation/report-layout revision passed all 249 checks in one complete isolated run. Actual Release screens showed both long navigation captions and full report identifiers, and native previews were rendered for both programs' grades and evaluations without printing. See [layout verification](LAYOUT-VALIDATION.md). Close the running application and reopen **Start Demo.cmd** to build the revised screens with the existing demo data.

The simpler role separation passed 197 application checks and 43 script checks across separate successful runs on 13 November 2026. All six locally provisioned demo accounts authenticated. See [release readiness](READINESS.md) for the setup-process stall and successful fresh rerun; this was not one uninterrupted suite run.

The earlier administrator-navigation update passed 212 checks. That broad academic menu has been superseded by the simpler role separation documented in [role access](ROLE-ACCESS.md). Close the running demo and reopen **Start Demo.cmd** to build the updated screens; existing credentials and records are retained.

The earlier fictional-profile update passed 202 checks, including ten checks for complete form data, preservation of accounts/enrollments/grades/review decisions, and rollback on a failed refresh. All four populated student tabs were rendered from the current demo, and the identity/contact layouts were inspected.

On 12 November 2026, the launch-readiness update passed 159 application checks, eight prerequisite/path checks, nine local-setup checks, 12 demo/reset checks, and four real visible-window checks: **192 total**. Coverage includes first-run completion/cancellation, existing-account preservation, automatic demo detection through ordinary/direct entry, class-switch save/discard behavior, and reset protection. Grade-entry and demo-login renders were inspected. The two existing unused-field warnings in the legacy faculty form remain; actual Windows DPI, printer hardware, and Azure deployment were not part of this verification. See [release readiness](READINESS.md) for remaining work.
