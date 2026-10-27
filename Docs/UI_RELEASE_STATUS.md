# UI and Release Status Report

Audit date: 2026-10-28  
Branch: `phase3-polish`  
Starting HEAD: `f41e4a3`  
Working tree: clean at audit.  
Unity project version: `6000.6.2f1`.

| Area | Audit finding | Plan/status |
|---|---|---|
| Existing graded gameplay | Left/right, one straight punch, one-step recoil, 5-hit dizzy, drain-only end, no health/K.O./timer, and player-isolated input are implemented and documented. | Preserve and rerun rubric checks. |
| Screen foundation | Runtime canvas provides title, roster, three stages, controls/settings, fight, result, and pause. Buttons currently use torn sprites; exit quits directly; text has no English mode; controls screen is text-heavy. | Redesign incrementally; add input-consumption and exit flow. |
| Maps | Current build exposes 3 arenas × 3 times, with moving clouds, traffic, lamps, and floor shimmer. | Fix Palengke to morning; Looban/Plaza to night; stop traffic/cloud/floor motion and remove time choice. |
| Existing test/build support | Unity 6000.6.2f1, 65 rule checks, EditMode/PlayMode tests, four-resolution portable QA and package scripts exist. | Extend/execute; report only runs actually completed. |
| Publishing | Project handoff warns that public reference-photo history needs owner review. | Inspect repository visibility and tracked paths before any external write. Never include reference-photo files. |
