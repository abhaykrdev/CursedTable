# The Hollow Seven — implementation decisions

2026-11-10. Source: the user's 62-page game design document and subsequent chat decisions.

- Unity 6000.6.8f1, URP 17.6.0; use installed Unity 6.6 rather than migrating to LTS.
- Gameplay AND solver obey tier operators; tutorials use their prescribed operators.
- Blessed dealer remains Calm until beat 6; Crack is revealed at the ending.
- Hints and Second Sight never suggest a cursed route. If no safe route can be proved, offer Undo.
- First Click to Enter starts the prologue; returning launches enter the menu.
- After the ending, offer Story Replay and Endless separately. Story Replay resets story progression only when selected.
- Correct technical contradictions while preserving public names and intended mechanics.
- User owns Web platform setup. Agent must not switch target or overwrite Web publishing settings.
- Build an actual 3D casino with separate editable objects, not a flat scene image. Story layers are individual objects.
- No downloaded/imported external art or audio without the user's asset selection. Procedural geometry and UI are locally authored.
- Existing MCP package emits four Unity 6.6 compatibility analyzer warnings. Track separately from game-code warnings; do not modify the user's external MCP installation.
- Tests are sequenced with their systems: operation/solver/score at M1, luck integration at M3. This corrects M1 requesting tests for a component scheduled at M3.
- Generator has an incremental coroutine entry point, plus the specified synchronous Generate wrapper for offline tests. Live calls are bounded.

## Visual direction
Oxblood velvet, nearly black mahogany, tarnished brass, ivory mask, amber practical lighting, restrained emerald luck accents. Separate wheel sectors, brass rails, felt, ball, dealer body parts, architectural trim and lamps. Strong hierarchy: target and hand, then dealer and wheel, then room silhouette. UI is native text and geometric controls.

## Pacing revision after user playtest
- Replace the 60-second prologue with four short shots (15 seconds total): seeing her through a rainy window, the wedding ring, the casino entrance, the dealer. Click advances; holding Space skips.
- Replace seven explanation-heavy lessons with three guided hands. Teach combining, reusing results, then lucky versus cursed chips. Undo and Hint appear in normal play so guided indices cannot be invalidated.
- Round introductions and resolutions use 1.1-second beats. Suppress codex cards during guided play while retaining discoveries.
- Replay Opening is available without deleting saved progression.
- Original synthesized sound effects, three ambient layers, and rain provide temporary audio; imported clips can override them. These are locally generated sounds, not third-party recordings or final composed music.
- Preserve 16:9 composition with black bars on other aspect ratios; UI scales with the viewport. Verify 800x600 and 1280x720.

## 2026-11-11 — User-directed story and reward revision
- User rejected the brief 3D cutscene experiment. Removed its scene component and runtime implementation. Keep illustrated 2D cutscenes with individual layered shapes, parallax and rain. ITHappy assets are for the casino, not the cutscenes.
- Opening now explicitly establishes the marriage, her leaving for another man, his arrival behind the casino, and the dealer's offer. Seven readable, click-advance shots; holding Space still skips. This supersedes the earlier 15-second opening.
- Completing the three guided hands fills Luck to +15. Show a breakup vision: his wife confronts her boyfriend, he leaves with his bag, she looks at the protagonist's contact. This records memory 1.
- Return to the table with an acknowledged warning: LUCK REWARDS YOU. UNTIL IT GETS YOU. The warning explains +15 / -15 and that the vision spends Luck, returning with +2. No timer runs while the warning is open. Tutorial is marked complete after acknowledgment, so an interrupted first reveal can be replayed.
- Skipping the tutorial deliberately bypasses its reward; it does not award a free +15.
- Add a soft descending bass pulse and quiet counting ticks tied to the displayed Luck change. Short gain pulse, halo and delta accompany it. Repeated audio is bounded; mute affects playing pooled sources immediately.
- The dealer acts during rules/dialogue: subtitle-paced mouth shapes, eye changes, small head movements, hand and forearm gestures. These are procedural acting, not voice lip sync. Pose returns to rest.
- User supplied Unity_6_Casino_Free_v1.0.unitypackage. Imported selected art/animation files preserving GUIDs, excluding demo scenes and third-party gameplay/editor scripts. Removed references to excluded MonoBehaviours from imported prefab copies. Source archive is intact in Assets/Downloads. Four slot machines now decorate the 3D casino.

- After the first breakup, cut to an animated 2D close-up of the hero: a smile becomes a villainous laugh, with the user's line revealed in timed stages. Then cut back to the dealer warning. An original voice clip is still required; the available audio-generation provider reports configured=false. See HERO_LAUGH_DIRECTION.md.

## 2026-11-13 — Clarity and submission pass
- Keep layered 2D artwork. Give characters fitted clothing, articulated limbs, facial features and shading; add a domestic departure scene.
- Opening is now 12 panels with explicit time/place and speaker labels. First reveal is eight panels, distinguishing the hero's marriage breakup from his wife's later breakup with her boyfriend. A vision is observation, not proof that the player caused the event.
- Story panels wait for player input. First click/Space reveals the text; next advances. Holding Space still skips the prologue. There is no forced reading deadline.
- Every fifth round rotates No Addition, Double Curses and Lucky Payout. The generator and hints use the restricted operator set. Double Curses affects cursed-chip spending; Lucky Payout doubles win Luck. Standard rules return afterward.
- Early hands include multiplication and reject addition-only subset targets. Tier 2+ time budgets are reduced to 75%, provisionally pending real-player timing.
- Default Windows and Web canvas sizes: 1280×720. Resizable Windows window. Move target upper-left so it cannot cover the centered dealer face; persistent explanation when cursed fog hides results.
- New Game clears only game progress/records after an in-game confirmation, retaining audio preferences. P toggles pause independently of browser Esc/fullscreen behavior.
- Unity write-tool approval block currently prevents import/build/playthrough verification. See LOCAL_VALIDATION_2026-11-13.md; do not treat offline checks as a submitted-build playthrough.
