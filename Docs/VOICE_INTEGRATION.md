# Supplied voice integration

All 60 MP3s under Assets/_Project/Audio/Voice are referenced in the Resources/VoiceLibrary asset. Original audio files and their GUIDs are preserved, including the double-extension double-curse file.

- Opening: 12 numbered recordings matched to the 12 StoryScript.Opening entries.
- FirstMemory: 8 recordings matched to the 8 first-reward shots, including the hero laugh.
- Memories: 18 recordings matched to D1A through D6C.
- Dealer: 8 tutorial instructions/reactions, 3 house rules, first-vision announcement, Luck warning, round introduction, win, timeout, invalid selection, unsafe hint.
- Endings: two dealer lines, hero departure and death reaction.

One dedicated non-looping voice AudioSource owns speech. Voices are never PlayOneShot calls. Busy requests are rejected; incidental repeated warnings have a four-second cooldown after completion. Gameplay SFX remain independent. Music/ambience duck while voice is active. Master, Sound Effects and Mute also affect voice.

Story clicks reveal subtitles while the recording continues. Further clicks during speech are consumed, not queued. A fresh click after completion advances. The explicit hold-Space opening skip and return-to-menu action cancel playback and clean up the voice owner. Tutorial skip also cancels speech.

Tutorial actions remain playable during the instruction, but completing the action waits for that instruction before the next line/hand. The first reward, popup, round introduction, rule announcement and result transition wait for voice completion. End-of-story choices appear after their dealer lines; the final departure shot waits for its hero recording. Death retry is held until its voice ends; returning to the menu explicitly cancels it.

Voice loading has a bounded wait and readable warnings rather than a permanent scene lock if an asset fails. Pause state is tracked separately from AudioSource.isPlaying, so pausing cannot falsely signal completion. Muting does not shorten narrative timing.

Validation results: VOICE_VALIDATION.md (dedicated live playback checks; existing editor test scripts were not edited).

## Verification on 2026-11-13
- 134 normal-speed playback assertions passed: every story/ending shot resists repeated Continue input until the actual recording finishes; all 60 files decode and have non-silent finite samples; pause/resume, cancellation, tutorial sequencing and repeat cooldown pass.
- 15 transition assertions passed: full first reward/flashback/warning, muted timing, tutorial skip, all three fifth-round rules, ending choices and death/retry.
- Separate HUD caption replacement check passed after routing first-reward and house-rule captions through the owner that cancels old caption timers.
- Existing EditMode suite: 11/11 passed, unchanged.
- Unity console after voice checks: zero errors/warnings.
- Asset/reference scan: 1,138 files, zero errors/warnings.

These checks verify timing and sample integrity. Perceptual voice balance still benefits from listening on the player's own speakers/headphones. The Windows review build is separate from the earlier unvoiced player; Web setup remains user-owned.

WindowsVoiceReview/CursedTable.exe built successfully: 144.46 MB, zero errors, 502 Unity/package warnings (including Inference shader variants). Build report confirms the supplied voice assets are packed. This player was built after the final caption fix. Browser/Web playback has not been verified in this pass.
