# Local validation — 2026-11-13

Unity MCP access restored; refresh/import and live compilation succeeded. Existing editor tests passed 11/11, and the original runtime check passed 31 assertions before voice integration. The WindowsReview player build succeeded (0 errors; 520 package/compiler/shader warnings). Standalone first-time inspection started, then the user took control and subsequently reported that the game seemed fine. This is not a claim that automated standalone end-to-end verification completed.

Voice integration is a subsequent revision: see VOICE_VALIDATION.md and VOICE_TRANSITIONS.md for its live playback results, and VOICE_INTEGRATION.md for behavior.

## Checks completed without driving Unity
- Compiled every runtime C# source with Roslyn against the actual Unity/project reference assemblies: PASS, no diagnostics. This is an offline compilation check, not a Unity import or player build.
- Executed the actual operation, solver, generator, rules and puzzle-state sources in an isolated .NET harness. Only ScriptableObject and its authoring attribute were stubbed; game algorithms were not replaced.
- 1,000 seeded levels, 25 seeds for each round 1–40: all safe solutions replayed to the target; all operations obeyed the round's operator list; no cursed operands in safe solutions; minimum safe lengths respected; zero fallback levels.
- Every No Addition fallback separately solved, including the highest tier.
- Early rounds had no addition-only subset solution. Using a subset of chips remains valid: 5 × 2 wins target 10 with 3 unused.
- Timer budgets exactly matched the configured formula and the 0.75 multiplier from tier 2 onward.

Observed time ranges (seconds): tier 1: 35; tier 2: 26.25–33.75; tiers 3–4: 33.75–41.25; tier 5: 33.75–48.75; tier 6 (rounds 31–40): 28.125–45. These are numerical checks, not human pacing evidence. The timer reduction remains provisional pending human playtesting.

## Original follow-up checklist (historical; status above)
1. Refresh/import and check compilation, warnings, missing scripts and metadata.
2. Inspect 2D home, café, rear alley, breakup and hero shots at 1280×720 and 1024×768. Confirm captions fit and manual reveal/advance/hold-Space behavior works.
3. Inspect the upper-left target panel, dealer face, house-rule label and persistent fog explanation during actual play.
4. Run existing editor tests unchanged, plus current runtime flow checks. Longer/manual story timing may require manual validation rather than rewriting the user's existing tests.
5. Build a Windows player to Builds/WindowsReview, then launch that executable. Use the new in-game New Game confirmation to clear only the player application's save, preserving the editor save.
6. Play opening → three tutorial hands → +15 reward → breakup → hero reaction → warning → normal play. Check P pause, resize, safe hints, first modifier round, -10 fog, death/retry and reset cancellation.
7. Web/browser fullscreen and persistence need a separate Web build check before itch submission.

Existing editor test scripts and unused ITHappy assets were left unchanged. Six inspection PNGs and their sidecars were archived to Docs/inspection-reference. The unused SampleScene and its .meta were removed after confirming it was absent from the build scene list.
