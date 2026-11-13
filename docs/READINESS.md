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

On 13 November 2026, the full `scripts/Test.ps1` run completed successfully against an isolated source snapshot:

- 206 application checks, including nine navigation/report-layout checks, first-run form completion/cancellation, grade-grid interactions, all 19 approved role navigation entries, prohibited routes, direct service denials, profile access and minimum-size navigation. Administrator has no academic access; only faculty save grades; only coordinators maintain teaching assignments.
- Eight prerequisite/path checks, including missing provider discovery, read-only/locked files and long paths.
- Nine database setup checks.
- 12 demo/reset safety checks.
- Ten fictional-profile checks covering complete form data, account/grade preservation and rollback on a failed refresh.
- Four visible-window checks using Release builds, including ordinary and direct demo entry.

Total: **249 passed in the complete run**. A separate Release build, actual administrator/registrar screen checks and four native grade/evaluation preview proofs also succeeded; see [layout verification](LAYOUT-VALIDATION.md). Six explicitly provisioned local demo accounts previously authenticated successfully; their generated credentials remain outside Git. Only the two existing unused-field compiler warnings in the legacy faculty form remain.

Earlier role-separation verification passed 240 checks across separate runs after one native ACE/OLE migration process stalled and its setup checks were rerun. The current 249-check run completed without that intervention. The earlier native stall remains an observation for release rehearsal; this layout revision did not change migration code.

These checks use disposable data. They do not certify every Windows configuration, printer, real OS DPI setting, screen reader, large dataset, disk failure or power-loss scenario. Human acceptance rehearsal and a clean second-computer check remain necessary before an external handoff.

The separate [acceptance rehearsal](UAT.md) subsequently passed 27 service-workflow checks for BSIT and BSBA, with saved-grade CSV read-back verification and a nonempty-fixture rejection check. Application behavior was unchanged. The 26-case manual UAT register and acceptance-owner sign-off are prepared but remain pending; automated rehearsal does not establish user acceptance.

## Remaining work before real school use

1. Add validated foreign keys, required fields, uniqueness and lookup indexes to the workflow tables through a backed-up/reconciled migration. Current database enforcement still permits orphan enrollment lines through direct writes.
2. Validate report filters, physical print output, official document layouts and access-denied export handling. Wrapped print content and measured identifier widths are now covered, but official transcript/GWA templates still require approved school rules.
3. Resolve previously exposed credentials and separately decide on coordinated Git-history cleanup. Current tracked source excludes the old data files; old revisions still contain them.
4. Add durable authentication controls, idle locking, deliberate privileged approvals and a secure administrator recovery process.
5. Verify migration/restore failure handling, off-device backup retention and richer audit history. Local database owners can change files outside application checks.
6. Confirm academic lifecycle rules for amendments, withdrawals, prerequisites, grading locks/corrections and curriculum versions.
7. Establish release automation, volume measurements and trusted central authorization before shared institutional operation. The current per-machine Access deployment is not the planned Azure API.

Database migrations, report changes, credential rotation, history cleanup, installers and cloud resources were not changed by the launch-readiness update.
