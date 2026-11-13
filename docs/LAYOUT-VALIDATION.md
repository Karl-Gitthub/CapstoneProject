# Navigation and report layout verification

The approved layout revision fixes two issues found in the desktop walkthrough: clipped navigation captions at the current Windows scaling, and shortened student/course identifiers in report grids and printed tables.

## Changes

- Navigation buttons measure wrapped text at the current font and available width, retaining their minimum height and growing when a caption needs another line. Sidebar scrolling and role permissions are preserved.
- Report and evaluation grids size columns to their content. Narrow windows can scroll horizontally instead of hiding identifier text behind ellipses. CSV contents and grade calculations are unchanged.
- Printed columns use measured content widths. Headers and values wrap without ellipsis trimming. Row height follows the wrapped content; normal rows stay together and oversized rows continue across pages. The internal-copy footer remains separate from the body. Invalid print areas produce a layout message.

## Verification on 13 November 2026

The complete isolated `scripts/Test.ps1` run exited successfully with **249 checks passed**: 206 application checks and 43 prerequisite/setup/demo/profile/visible-window checks. The application count includes nine new layout checks covering both long navigation captions at simulated 100% and 200% font/width scales, minimum-size report identifiers, measured print columns, wrapped multi-page evaluations, oversized-row continuation and empty reports. These simulated cases do not constitute an OS-scaling matrix.

A separate Release build succeeded. Its actual administrator and registrar windows were inspected at the workstation's current Windows scaling: **Password assistance** and **Registration approvals** displayed completely on two lines. The BSBA report screen displayed the full UAT student number and both full course codes, alongside the saved averages and equivalents. Both test workspaces signed out normally.

The Release application's PrintDocument was also exercised with .NET PreviewPrintController against the configured printer driver. BSIT and BSBA copy-of-grades and curriculum-evaluation reports each produced one native preview page. All four proof images were inspected: complete identifiers, course descriptions, saved outcomes and internal-copy footers remained visible. This rendered previews without sending physical print jobs. Proofs are local under ignored `artifacts/layout/print-proofs/`.

All five presentation-demo database SHA-256 hashes matched their pre-verification values. Main data was not used. Only the two existing unused-field warnings in the legacy faculty form remain.

## Remaining acceptance work

The two observed layout defects are resolved in this revision. Actual staff/adviser sign-off, a full interactive login walkthrough, a broader OS-scaling and keyboard-accessibility matrix, second-computer installation and physical printing remain pending. The earlier automation difficulty targeting an owned Print preview Close control is not a demonstrated application defect; native preview generation was verified separately here. Follow the [UAT guide](UAT.md) and [release-readiness constraints](READINESS.md).
