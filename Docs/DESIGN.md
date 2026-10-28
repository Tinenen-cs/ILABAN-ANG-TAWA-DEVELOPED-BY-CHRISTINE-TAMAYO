# ILABAN ANG TAWA - current design

Replaces historical academy/fantasy notes. UPDATE2_REQUEST.md is authoritative.

Two local players; eight original fictional adult chibi fighters; Palengke morning, Looban night and Plaza night. Painted gold lettering, cream text, warm rounded wood/brass/capiz. Native 1280x720 logical canvas with anchored/scaled UI and background-cover adaptation through ultrawide.

Opening -> title -> both-player confirmations -> stage -> first-to-three match. Controls provide language/reactions/volume. Exit asks Oo/Hindi, default Hindi. Notices trap input. Earlier drain wins show automatic banners; third win shows winner/replay/select/exit. Esc pause has five options.

DuelModel owns left/right movement, one straight punch shown as weapon strike, common reach 190, body gap 112, movement 210, startup .12s/cycle .36s, one 52-unit step per hit, five-hit dizzy for two seconds, streak resets and instant drain results. No jump, health, stamina, K.O., timer or second attack. Input maps isolate keyboard/gamepad/USB slots.

AcademyGame renders one full-body image per fighter: six strike phases; four idle/walk/hurt/dizzy/fall/win/lose frames. Canvas 896x512, fixed feet-center pivot and ground 512. Baked weapons, imported head/grip/tip anchors, five amber/red streak segments, three win medals, head-owned bubbles and separate visible dizzy stars.

Only restrained sky wisps, leaves, lamps and flags animate behind combat. No fight vehicles or floor layers. Correctly facing jeepney appears only in the opening. See credits/audio/evidence; physical input/audio/performance and clean-PC checks remain manual.
