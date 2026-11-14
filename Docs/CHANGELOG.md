# Build log — Cursed Table

## 2026-11-10 — Foundation and puzzle verification

**Done:** Created project folders, runtime/editor/test assemblies, Game scene, three overlay canvases, Input System EventSystem, data assets and core puzzle rules. Unity version verified through MCP: 6000.6.8f1; URP 17.6.0. Imported Unity's bundled TMP essentials. Recorded user decisions in DECISIONS.md; copied source text into DESIGN.md.

**Tests:** PASS: 11 EditMode tests covering operation validity, solver safety and allowed operators, duplicate-chip Undo, scoring example, once-per-round Luck and death/dream triggers. PASS: 3,000 seeded puzzles, 500 in every tier, safe paths replayed, no forbidden operators or cursed operands, target bounds and minimum safe length verified. Zero fallback levels. Average generation 0.06–6.24 ms; worst 77.13 ms. Details: SOLVER_TESTS.md.

**Screenshot:** screens/boot-ui.png; screens/play-first.png (initial visual checks).

**Deviations:** User owns Web platform setup, so no platform switch or publishing-setting overwrite. Unity 6.6 replaces the document's LTS expectation. Primitive casino objects and procedural story layers replace flat image placeholders per the user's latest direction. Related core and luck code compiled together to resolve the original test-order dependency. Existing external MCP analyzer warnings are excluded from game-code warning counts.

**Next:** Full runtime verification, tutorial/ending flows, responsive layout, visual polish, and asset selection shortlist if needed.

## 2026-11-10 — Opening, pacing and presentation revision
Rebuilt the opening as short layered scenes, reduced onboarding to three interactive hands, and shortened round transitions. Added contextual chip/operator guidance, native button feedback, original procedural audio, quieter atmospheric lighting, curtains, table details, mist, brass chip inlays and clearer target hierarchy. All casino props remain separate objects.

Fixed a merge-animation/tutorial index race, blocked Undo during prescribed tutorial moves, suppressed tutorial codex interruptions, stopped rain when leaving a story shot, reset the ending mask on a new run, and fixed stale-frame artifacts outside the 16:9 viewport. Added Replay Opening to the menu.

Validation: core tests and 3,000-puzzle coverage remain recorded above. Full runtime flow covers three guided hands, normal play, Undo, safe hints, death/retry, Double Vision, ending and Endless. See RUNTIME_TESTS.md. Small-screen screenshot: screens/layout-800x600-verified.png. Unity asset-reference checker passes (364 files, zero errors/warnings) after removing nine unused template test subassets; original profile backed up under Docs/backups.

Remaining limits: no Web player build/browser test (user is configuring Web), no final recorded soundtrack or character assets, no measured target-device frame-time budget. The visual result is still custom prototype geometry. External art selection remains with the user.

## 2026-11-11 — Emotional hook and first reward
Expanded the illustrated opening with a rear-casino scene and explicit story context. Kept 2D cutscenes after user feedback; removed the temporary 3D story component. Added three separate breakup shots, the +15 first reward, acknowledged dealer warning, and next-memory progress on the HUD. Later memory panels now have distinct captions rather than repeating one sentence.

Added subdued Luck pulse/count sounds, meter halo and gain/loss text, immediate pooled-source mute, and procedural dealer speech gestures. Imported the user-selected ITHappy art subset and placed four individual slot-machine props in the casino. No third-party gameplay scripts or demo settings imported.

Runtime regression now includes first reward/vision/warning/return, memory persistence, dealer gesture/rest, and generated audio bounds/fades. Latest results: RUNTIME_TESTS.md. Audio tests establish sample integrity; final perceptual balance needs playtesting on the user's speakers/headphones. Web setup remains user-owned.

Added the requested 2D hero reaction after the breakup: close-up, growing smile, moving mouth/brows/head and staged LUCK / NUMBERS / laughter text. Voice playback slot supports pause/mute/skip; no spoken recording is assigned yet.

Final runtime check: 31 assertions passed, including the new first-reward flow, dealer acting and Luck audio sample checks. Asset-reference validation: 1,009 files, zero errors/warnings. Reviewed 2D alley, breakup and hero reaction screenshots, plus casino gameplay. Fixed the next-memory label being covered by the target panel.

## 2026-11-13 — Story clarity and submission checklist (validation pending)
Expanded story script, speaker/time/place labels, self-paced text, a home-departure scene and more detailed 2D character anatomy/clothing/faces. Added rotating fifth-round modifiers, more varied early targets, provisional tier-2+ timer reduction, visible fog explanation, upper-left target panel, New Game confirmation and P pause. Set default window/Web size to 1280×720 and enabled resizing. Added short itch controls line; archived six inspection PNGs; removed unused SampleScene. Existing editor test scripts and unused ITHappy assets untouched.

Offline runtime compilation and 1,000 generated-level checks passed. Unity refresh was blocked by the session's tool-approval policy, so live visual checks, player build and fresh-save playthrough remain required. See LOCAL_VALIDATION_2026-11-13.md.


## 2026-11-13 — Supplied voice recordings
Integrated all 60 user-supplied MP3s through a serialized voice library without moving or replacing originals. One non-looping source owns speech, rejects overlapping/repeated requests and ducks music/ambience. Story clicks reveal text without replaying/cutting voice; transitions wait for completion. Tutorial chip/operator stages no longer repeat the instruction. Round/rule announcements, first reward/warning, ending and death/retry now respect voice duration. Explicit skips and menu exits cancel playback. Pause resumes from the same sample; mute retains timing. Dealer gesture duration follows the recording.

Dedicated normal-speed voice QA passed 134 assertions, including every story recording under repeated Continue input and all 60 assets decoding successfully. Existing editor test scripts were left unchanged; the new VoicePlaybackChecks file contains voice-specific regression checks. See voice validation reports for subsequent transition/build results.

Final voice verification: 134 playback assertions + 15 transition assertions + caption-ownership check passed; existing EditMode suite 11/11. Final WindowsVoiceReview build succeeded with zero errors/warnings and all 60 voice files packed. See VOICE_INTEGRATION.md.

## 2026-11-13 � Cursed Table title and comment cleanup
Changed visible titles, casino signs, opening captions, product name, current documentation titles and editor menu labels to Cursed Table. Internal namespace/assembly names and historical build paths remain stable. Removed seven descriptive/history comments and shortened three technical comments using the unslop rewrite checks. Kept comments explaining solver safety, shadows and layout. No gameplay logic or test assertions changed.

The supplied opening line 07 needs a new recording of the updated title; see VOICE_REPLACEMENTS.md. Original recordings were preserved.

Renamed Windows build: Builds/Cursed Table/Cursed Table.exe, successful with zero errors. Existing 11 editor tests passed. Build reported 497 package/shader warnings. Spoken opening title replacement remains pending.

Final renamed build: build-3edf34bcc3, zero errors, 496 package/shader warnings. Pending-compilation warning resolved after refresh and rebuild.

Removed the Credits menu and page-draft attribution sections. Replay Opening moves up to fill the menu gap. Submission-page credits will be supplied manually.

Credits removal verified in the live menu and compiled runtime assembly. Build build-85819cab54 succeeded with zero errors and zero warnings.

Added menu hover/click thumps: 140 ms soft hover and 240 ms deeper click, with an 80 ms hover cooldown. Menu buttons own one click trigger; gameplay operators retain their existing selection sound. Disabled buttons do not play hover audio. Waveform bounds and fades checked, and live menu event check passed (one click sound; disabled hover silent). Included in the next build from this project.

Integrated amb_room_tone.wav as one looping casino ambience source referenced by Game.unity. Fades out during memories/exteriors, ducks under voice, lowers during pause, and follows Master/Sound Effects/Mute. Reduced synthesized background stems to make room for the quiet supplied recording. Verified clip decoding, a single source after repeated unlock, looping playback, memory fade-out and voice ducking in Play mode. Included in the next build.

Fixed audio settings: invisible 48px pointer areas accept track clicks and dragging; 4px track/fill and smaller diamond handles retain a slim appearance. Added percentages. Volume changes update active sounds, and zero silences the appropriate channels immediately. Music now controls casino/rain ambience; Sound Effects controls voice and gameplay/UI effects. Live pointer/raycast tests passed for all three sliders; master/effects/music zero checks passed. Assigned the supplied PNG as the default application icon and added Web build favicon copying/linking, verified with a generated HTML fixture. Changes are ready for the next build.
