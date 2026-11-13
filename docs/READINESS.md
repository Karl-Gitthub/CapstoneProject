# Release readiness

The application supports a controlled local demonstration on a Windows machine with the documented prerequisites. General institutional deployment is not signed off. The installer and Azure deployment remain separate future work.

## Current launch and editing safeguards

| Area | Behaviour |
|---|---|
| Main entry | Double-click `Start Main.cmd`. A new installation creates sample databases, asks for administrator credentials, then opens login. Cancel exits without creating an account or opening login. Existing accounts go directly to login. |
| Demo entry | Double-click `Start Demo.cmd`. Its active `data` folder and session marker determine demo mode, including ordinary/direct executable entry. Demo windows hold the reset guard from startup until exit. |
| Demo generation | Fresh data is generated and migrated in a staging folder before activation under the exclusive guard. Reset archives the previous demo without changing working data. |
| Prerequisites | Launch checks Windows/x64, PowerShell version, ACE registration, build/targeting tools, writable locations, database access and backup path length. Failed checks give recovery guidance before setup. |
| Build configuration | Public launchers use Release by default; `-Configuration Debug` is available. The existing working-data directory is preserved across configurations. |
| Grade entry | The loaded class is labelled. A pending class selection blocks editing, save and print until opened. Declining a discard restores selection and edits; accepting discards only unsaved changes. Save validates and reloads the loaded class. |

No default account or password is supplied. Administrator creation retains the one-time transactional bootstrap checks. A direct executable launch still needs an initialized selected data directory and its runtime prerequisites; use the CMD launchers for normal setup.

## Verification

On 13 November 2026, an isolated source snapshot was verified with these passing checks:

- 197 application checks, including first-run form completion/cancellation, grade-grid interactions, all 19 approved role navigation entries, prohibited routes, direct service denials, profile access and minimum-size navigation. Administrator has no academic access; only faculty save grades; only coordinators maintain teaching assignments.
- Eight prerequisite/path checks, including missing provider discovery, read-only/locked files and long paths.
- Nine database setup checks.
- 12 demo/reset safety checks.
- Ten fictional-profile checks covering complete form data, account/grade preservation and rollback on a failed refresh.
- Four visible-window checks using Release builds, including ordinary and direct demo entry.

Total: **240 passed across the application suite and script checks**. Six explicitly provisioned local demo accounts also authenticated successfully; their generated credentials remain outside Git. See [role access](ROLE-ACCESS.md) for the workflow. Only the two existing unused-field compiler warnings in the legacy faculty form remain.

The first script run stalled in a native ACE/OLE migration process after schema writes; only that verified test child was stopped. All nine setup checks subsequently passed in a fresh independent run without intervention. The sample-profile fixture was updated to use separate registrar, coordinator and faculty accounts, and all ten profile checks passed on rerun. All four visible-window checks then passed. These were separate successful verification runs, not one uninterrupted full-suite pass; the native migration stall remains an observation for release rehearsal.

These checks use disposable data. They do not certify every Windows configuration, printer, real OS DPI setting, screen reader, large dataset, disk failure or power-loss scenario. Human acceptance rehearsal and a clean second-computer check remain necessary before an external handoff.

The separate [acceptance rehearsal](UAT.md) subsequently passed 27 service-workflow checks for BSIT and BSBA, with saved-grade CSV read-back verification and a nonempty-fixture rejection check. Application behavior was unchanged. The 26-case manual UAT register and acceptance-owner sign-off are prepared but remain pending; automated rehearsal does not establish user acceptance.

## Remaining work before real school use

1. Add validated foreign keys, required fields, uniqueness and lookup indexes to the workflow tables through a backed-up/reconciled migration. Current database enforcement still permits orphan enrollment lines through direct writes.
2. Correct report filters, long-text print layouts and access-denied export handling. Official transcript/GWA templates require approved school rules.
3. Resolve previously exposed credentials and separately decide on coordinated Git-history cleanup. Current tracked source excludes the old data files; old revisions still contain them.
4. Add durable authentication controls, idle locking, deliberate privileged approvals and a secure administrator recovery process.
5. Verify migration/restore failure handling, off-device backup retention and richer audit history. Local database owners can change files outside application checks.
6. Confirm academic lifecycle rules for amendments, withdrawals, prerequisites, grading locks/corrections and curriculum versions.
7. Establish release automation, volume measurements and trusted central authorization before shared institutional operation. The current per-machine Access deployment is not the planned Azure API.

Database migrations, report changes, credential rotation, history cleanup, installers and cloud resources were not changed by the launch-readiness update.
