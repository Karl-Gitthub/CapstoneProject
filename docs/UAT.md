# User acceptance testing

Status: **Prepared for user testing. Human acceptance is pending.**

On 13 November 2026, the supplementary automated rehearsal passed all **27 checks**, covering BSIT and BSBA through saved grades, evaluation, CSV generation and audit events. Both exported files were independently read back and checked. A separate negative run rejected a populated fixture with exit code 1 and preserved the account/student database hashes. The final full rehearsal exited successfully. This uses the application baseline from commit `433d6f3`; this revision adds the rehearsal harness and documentation, without changing application behavior. The 26 manual cases below remain Not run.

Automated checks verify calculations, permissions and saved records. UAT asks the intended staff and adviser whether the workflow is correct, understandable and usable for their work. Only those users can accept it. This exercise covers the local Windows demonstration, not institutional deployment or Azure hosting.

## People and test record

Name the adviser or acceptance owner, a registrar representative, a BSIT/BSBA coordinator representative and a faculty representative. One person may rehearse several roles; record when an actual role representative has not participated.

Copy [UAT-CHECKLIST.csv](UAT-CHECKLIST.csv) to a new local results file under ignored `artifacts/uat/<session>/`. Keep the template unchanged. Record the commit, Windows version, display scaling, data/demo name, date and tester names. Each case starts as **Not run**, even where automation already covers related behavior. Use **Passed**, **Failed**, **Blocked** or **Not applicable** only after the tester records the actual result. Explain blocked and not-applicable cases.

For failures, record the case ID, steps, expected and actual result, severity and a screenshot or log path using fictional data only. Do not put account passwords in screenshots, issue reports or Git. Keep signed acceptance evidence local or in the school's approved document location.

## Start the rehearsal

1. Finish pending work and close the existing application. Open **Start Demo.cmd** from the repository folder. Confirm the **DEMO · SAMPLE DATA** indicator before changing any records.
2. Use the explicitly provisioned `demo.admin`, `demo.registrar`, `demo.bsit`, `demo.bsba`, `demo.faculty.it` and `demo.faculty.ba` accounts. This machine's passwords are in local `artifacts/demo/presentation/ROLE-ACCOUNTS.md`. Fresh downloads have no default accounts. Avoid resetting the presentation demo; reset removes its active accounts.
3. Use new student numbers `UAT-BSIT-<session>-01` and `UAT-BSBA-<session>-01`, with a short unique session such as `20261113A`. Never reuse existing records for destructive cases. Suggested fictional names: Miguel Antonio Dela Cruz and Alyssa Mae Reyes; year level 2; birth date 15 February 2006; contact email ending in `@example.invalid`. Label additional records as rehearsal data and use masked phone numbers.
4. For each program, use its existing `SAMPLE-<program>-201` and `SAMPLE-<program>-202` courses and the matching `SAMPLE-FAC-BSIT` or `SAMPLE-FAC-BSBA` faculty profile. Use a unique section such as `UAT-IT-A` or `UAT-BA-A`, academic year **2026-2027**, **First semester**. Existing faculty or section schedules may already occupy a slot: select a free time rather than changing an earlier class to make the test pass. Rejection of a genuine conflict is expected.

## Run the academic workflow for each program

| Step | Sign in as | Action and expected result |
|---|---|---|
| 1 | Registrar | Open Students, add the fictional student and save. Reopen it and verify identity, contact and credential details. It enters Pending program review. |
| 2 | Matching coordinator | Open Pending students and review the student. Request a correction with a clear note. Registrar sees the correction request and edits the details. Saving returns it to program review. |
| 3 | Registrar, then matching coordinator | Registrar cannot give final approval before forwarding. Coordinator reviews and selects Approve and forward. The record enters Pending registrar review. |
| 4 | Registrar | Open Registration approvals, review and approve. The status becomes Approved. |
| 5 | Matching coordinator | Open Faculty loading. Assign the two courses to the matching faculty, same section and term, using adjacent free time slots such as 09:00–10:30 and 10:30–12:00 on M,W. A deliberate overlapping assignment must be rejected. |
| 6 | Matching coordinator | Open Enrollment drafts, load the approved student and term, select both classes and save draft. There is no Confirm enrollment action. Drafts alone must not create faculty grade rows. |
| 7 | Registrar | Open Enrollment, load the same student and term, verify the two selections and confirm. It becomes Confirmed; the assigned faculty now sees the student in the two classes. |
| 8 | Matching faculty | Open the first class in Grade entry. Save prelim only, then reopen it: remaining periods and the computed average remain incomplete. Enter midterm and final, save, and reopen again. Verify the expected values below. |
| 9 | Matching faculty | In the second class, verify that -1 and 101 are rejected without changing saved grades. Then save 0 for all three periods: zero is a valid grade, not a blank. Verify read-only Computed grades. |
| 10 | Registrar | Open Evaluation for this student. Verify the saved course results. In Reports choose Copy of grades, specify the exact student, year and semester, and load saved records. Open Print / export report, inspect Print preview and export CSV to the local UAT session folder. Verify student, course, term and grade values after reopening the file. Reports must be labelled internal copies. |
| 11 | Administrator | Open Audit history and verify the registration, enrollment and grade actions from the rehearsal. The administrator must have no academic navigation. |

Expected calculation examples:

| Course | Prelim | Midterm | Final | Average | Equivalent | Evaluation |
|---|---:|---:|---:|---:|---:|---|
| First | 95.5 | 96 | 95 | 95.5 | 1.25 | Passed |
| Second | 0 | 0 | 0 | 0 | 5.00 | Not passed |

Use the Windows decimal separator when entering grades. If the school disputes an equivalent or averaging rule, record a requirements issue; do not silently change the expected result or treat it as accepted policy.

## Check role boundaries and desktop behavior

- Administrator: account/access/password tools and audit history only. No Students, Enrollment, Grades, Evaluation or Faculty loading.
- Registrar: student editing, registration approval, enrollment, evaluation and reports. No grade-entry or faculty-loading page.
- Coordinators: own program only; student details read-only; decisions start from their review queue; no final enrollment confirmation, report portal or grade saving.
- Faculty: assigned class rosters and grades only; computed grades read-only; no full student personal records or other instructor's class.
- Sign out between roles. Verify the role and username on every workspace; reopening an application must require sign-in.
- Test a pending grade change followed by choosing a different class. Save/print must wait until the selected class is opened. Decline the discard prompt and verify the original edits remain; accept it only with disposable edits.
- Check the minimum window size, keyboard Tab order, readable labels, error messages and sidebar access. Test actual Windows display scaling and a second laptop separately. Print preview availability depends on the installed printer configuration; record a missing printer as blocked, not passed. Physical printing needs a separate user check.

## Automated rehearsal

From the repository folder, run:

```powershell
powershell.exe -NoProfile -ExecutionPolicy Bypass -File .\scripts\Test-Acceptance.ps1
```

The script builds Release and creates fresh schema-only fixtures under a new ignored `artifacts/acceptance/<run>/` directory. It explicitly approves six fictional staff accounts through the account services and rehearses both program workflows through the compiled services. It produces `rehearsal.log`, `rehearsal-errors.log`, `evidence/acceptance-results.json` and two internal-grade CSV examples. Runner execution has a two-minute timeout; a timed-out or nonzero run fails even if partial evidence exists. An empty fixture and a synthetic manifest are required before any rehearsal writes.

This is a service-level rehearsal. It does not click through the desktop, validate printer output, replace the existing regression suite or constitute human acceptance. Its passwords are disposable fixture credentials and do not apply to the presentation demo.

## Acceptance decision

Do not approve while a required case is failed, blocked or not run. Resolve defects and repeat affected cases; repeat permission checks after any access-rule change. Record which optional cases were deferred and why. The acceptance owner decides whether the demonstrated local workflow meets the agreed requirements.

Record the decision in the local results copy:

| Field | To be completed by actual users |
|---|---|
| Tested commit and session | |
| Registrar / coordinator / faculty reviewers | |
| Adviser or acceptance owner | |
| Required cases passed and remaining issues | |
| School grading and workflow rules confirmed | |
| Decision: Accepted / Changes required / Deferred | |
| Name, signature or approved confirmation, and date | |

Even an accepted demonstration is not authorization for real school deployment. Database constraints, submitted-grade correction rules, backup/recovery rehearsal, the previously observed native migration stall, second-computer installation and Azure remain covered by the separate [release-readiness work](READINESS.md).
