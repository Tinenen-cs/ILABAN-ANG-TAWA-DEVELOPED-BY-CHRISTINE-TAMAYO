# ILABAN ANG TAWA - implementation checklist

Only verified work is checked. The original academy prototype is not yet the final submission.

## 1. Setup and naming
- [x] Inspect the existing source before changes; preserve Main.unity and asset GUIDs.
- [x] Save an external pre-upgrade source backup (Fight-Fght-backups).
- [x] Identify installed editor: Unity 6000.6.2f1; Windows standalone module present.
- [x] Reconnect local files to the existing Tinenen-cs/Fight-Fght Git history.
- [x] Create the working folder ILABAN ANG TAWA DEVELOPED BY CHRISTINE TAMAYO (original Windows-locked folder retained).
- [x] Upgrade, compile, and validate the existing game with the installed editor.
- [x] Set company to Christine Tamayo and product/window/build name to the required title.
- [x] Open the original Main scene in the installed Unity Editor.
- [x] Update repository display name while retaining the existing repository identity/history.

## 2. Character recreation
Names are proposals; reference faces must remain unchanged. Each line requires all seven animations (idle, walk, punch, hit, dizzy, fall, win), effects, and animated selection preview before completion.
- [ ] Aling Puring - pamalo; face preserved, new outfit, seven animations, effects, preview.
- [ ] Neneng - tsinelas; face preserved, new outfit, seven animations, effects, preview.
- [ ] Tala - bato; face preserved, new outfit, seven animations, effects, preview.
- [ ] Ligaya - batuta; face preserved, new outfit, seven animations, effects, preview.
- [ ] Mang Isko - lata; face preserved, new outfit, seven animations, effects, preview.
- [ ] Dodong - silya; face preserved, new outfit, seven animations, effects, preview.
- [ ] Berting - bote; face preserved, new outfit, seven animations, effects, preview.
- [ ] Kulas - walis tingting; face preserved, new outfit, seven animations, effects, preview.
- [ ] Equal punch timing, reach and knockback for every item.
- [ ] Pixel import: Point filtering, no compression; document PPU for sprite assets.

## 3. Maps (user confirmed three)
- [ ] Kalye sa Palengke: artwork, full-screen fit, imburnal, animated background, distinct music.
- [ ] Looban: artwork, full-screen fit, imburnal, animated background, distinct music.
- [ ] Plaza ng Barangay: artwork, full-screen fit, imburnal, animated background, distinct music.
- [ ] Verify 1920x1080, 1366x768, 1280x720, 2560x1440 and a 16:10 desktop.

## 4. Audio
- [ ] Separate title and selection music.
- [ ] Three original Filipino-inspired map tracks with credits.
- [ ] Move, select, confirm, back menu cues.
- [ ] Whoosh and eight item-specific impact cues.
- [ ] Walking/hit/dizzy/recovery/fall/splash/win cues synchronized.
- [ ] Source and license inventory in CREDITS.md.

## 5. Language and screens
- [ ] Exact required title: ILABAN ANG TAWA DEVELOPED BY CHRISTINE TAMAYO.
- [ ] Title, selection, stage selection, controls, pause and results reviewed in Tagalog.
- [ ] Animated character previews, items and independent confirmation.
- [ ] Scan visible strings for English; exempt only the required title and hardware key labels.

## 6. Rubric gameplay
- [ ] Shared-screen two-player movement, horizontal only; jumping impossible in every state.
- [ ] Straight punch only; guard/kick/crouch/combo/spell/power/shove disabled.
- [ ] Every landed punch moves the victim exactly one fixed backward step.
- [ ] Five consecutive landed punches cause dizzy stars/wobble with defined recovery.
- [ ] Attacker hit or broken streak resets the streak; no health/poise bars or K.O.
- [ ] Imburnal fall immediately ends the round and visibly animates into results.

## 7. Input
- [x] Existing Input System asset (upgraded by Unity to 1.20.0) and separate Player1/Player2 action maps located.
- [ ] Keyboard A/D/F and arrows/Right Ctrl/Numpad 0 isolation.
- [ ] Gamepad deadzone and exclusive device ownership.
- [ ] Generic USB controller recognition tested on physical hardware.
- [ ] Live keyboard/gamepad switching and unplug/replug without forced pause.
- [ ] Simultaneous inputs and keyboard/controller menu navigation verified.

## 8. Build and handoff
- [x] Commit and push setup checkpoint ec68355 to the existing repository; repeat for later steps.
- [ ] Windows x64 standalone with required executable filename.
- [ ] Portable folder and ZIP with README and CREDITS.
- [ ] Run from a different folder and a clean Windows PC without Unity.
- [ ] Complete TEST_PLAN.md with observed results; final bug review and fixes.

## Current continuation checkpoint
- [~] New Filipino runtime keys, full portraits and six-part procedural rigs connected; compile/visual verification pending.
- [x] Private reference folder ignored; duplicate image hashes scanned and ignored if found. No push/history rewrite.
