## Source page 1
Cursed Table — Game
Design Document
Nov 10, 2026  ·  @Naughty
1. Agent rules, locked decisions and pitch
This document is the only source of truth for building Cursed Table, a Unity 6 WebGL
puzzle game for the 2026 Unity Game Jam (theme: Lucky Number). Follow it literally.
Where it is silent, take the simplest option and log the choice in
Docs/OPEN_QUESTIONS.md . Priority tags: [MUST] the game is broken without it,
[SHOULD] build after every MUST passes, [COULD] only if time remains.
1.1 Rules for the building agent
1. Read the whole document before touching the project. Build in the order of section 12
and run each milestone's acceptance tests before starting the next.
2. Do not invent features, assets, packages or APIs. If something is missing, write a one-
line question in Docs/OPEN_QUESTIONS.md , take the simplest default and continue.
3. Allowed packages only: Universal RP, Input System, Unity UI (uGUI) with TextMeshPro,
Unity Test Framework. No DOTween, Cinemachine, Addressables, 2D Animation,
Timeline or Asset Store code. Write the small Tween helper from section 11 instead.
4. Check the Unity version first through the MCP and write it into Docs/CHANGELOG.md .
Expected: Unity 6 LTS (6000.x) with URP. If it differs, stop and log it.
5. After every script write: wait for compilation, read the Console, fix every error and
warning you caused. Never continue with compile errors.
6. Every tunable number lives in ScriptableObjects ( GameConfig , DifficultyTable ,
LuckTable ). No magic numbers in code.
7. Placeholders first: Unity primitives and flat-colour sprites using the exact names in
sections 7, 8 and 11, so real art drops in later without code changes. Never generate,
download or import art or audio. A missing Sprite or AudioClip must never throw; skip
silently.
8. WebGL rules: no System.Threading  or Task.Run , no System.IO  file access, no
Application.Quit  (hide Quit on WebGL), long work runs in coroutines that yield,
saves use PlayerPrefs only. Audio starts only after the first click on the Click to Enter
screen.
Cursed Table — Game Design Document
Page 1 of 62

## Source page 2
9. Use the class, method, enum and event names in section 11 exactly. Do not rename
files or members.
10. Use only the key bindings in section 6. Mouse is the primary input.
11. Cache references in Awake . No FindObjectOfType , no GetComponent  or string
lookups per frame, no allocations per frame in Update  (no LINQ, no string
concatenation).
12. UI tweens use unscaled time, because Time.timeScale  is 0 while paused.
13. At every milestone capture a Game View screenshot at 1920x1080 and compare it
with the layout and safe-zone rules in section 6. Fix before moving on.
14. Debug keys F1 to F6 exist only inside #if UNITY_EDITOR || DEVELOPMENT_BUILD .
15. Do not change Project Settings except what sections 7 and 12 list.
1.2 Locked decisions
Topic Decision
Engine Unity 6 LTS, URP, C#, new Input System, uGUI +
TextMeshPro
Target WebGL in a desktop browser (itch.io), mouse and
keyboard
Orientation Landscape 16:9, reference resolution 1920x1080
Look Stylised 3D casino, 2D layered-sprite story scenes
Ammo Every chip is used once; each result returns to the
hand as a new chip
Operators + − × ÷ on positive integers, exact division only
Core meter Luck from −15 to +15 replaces lives
Hint One per run, costs 2 luck
Story Prologue, 6 dream beats, ending choice (Leave or
Stay), endless mode
Art and audio Placeholders now, named slots for real assets later
Saves PlayerPrefs
Language English only
Cursed Table — Game Design Document
Page 2 of 62

## Source page 3
1.3 Pitch
A heartbroken, drunk man sits at a roulette table in a casino that feeds on despair. Reach
the target number using the chips in your hand. Every number carries a real superstition: 7
and 108 bless you, 4 and 13 curse you. The room itself shows your luck: warm and clear
when blessed, red and choking when cursed. Hit −15 and the house keeps you. Hit +15 and
the casino lets you see what became of the person who broke you. Climb back from the
brink and the house pays double.
1.4 Selling points (what the jam page and the first 30 seconds must show)
1. Superstition is the physics. 15 culturally real numbers change your luck. The shortest
route to the target is not always the safe one.
2. The room is the meter. Lighting, fog, camera sway, music and the dealer's mask all
change with luck.
3. Fight back for double. Survive the red zone and the next dream is doubled.
4. Two visual languages. Cold, oppressive 3D casino against warm, painterly 2D dreams.
5. Short and finishable. Rounds last about 40 seconds. The first dream arrives in about 4
minutes.
1.5 Theme and voting mapping
Topic Decision
Working title Cursed Table (also the name of the casino)
Jam asks for Delivered by
Number with cultural, religious or symbolic
meaning
LuckTable: 15 numbers, each with a one-line
Codex blurb (section 3.5)
Luck, fate or superstition in mechanics Spending tagged numbers changes Luck; the
meter changes timer speed and what you
can see
...in art CasinoDirector changes light, fog, post-
processing and mask by luck
...in sound Three music stems crossfaded by luck,
heartbeat in the red zone (section 9)
Cursed Table — Game Design Document
Page 3 of 62

## Source page 4
2. Story
The story is told in fragments, never in exposition. The player is drunk and heartbroken;
the casino feeds on that grief and tempts him to sit down and give up. Every time he fights
back the house pays out, and the payout is a dream in which he sees what became of his
wife. The dreams start as satisfying and end as hollow, so the arc is: revenge does not
heal, and walking away does.
2.1 Cast (no names are ever spoken or shown)
Jam asks for Delivered by
Narrative Design Prologue, haunts, six dream beats, Leave or
Stay ending
Fun Factor Short rounds, rubber-band difficulty, risk
against reward routes
Uniqueness Numerology as mechanic; heartbreak as
stakes
Character Look Rules
The Hero (the
player)
Mid-30s, rumpled dark suit,
loosened tie, wedding ring on the
left hand, bottle in the prologue.
Seen as a silhouette or from
behind; the face appears only as a
window reflection in dream beat 6.
Never named. Speaks only as
whispered captions.
His wife Long dark hair, mustard-yellow
coat (so she reads in rain and
neon), carries a bouquet in the
prologue.
Never named. Always referred to
as 'she'.
The other man Tall, dark coat. Face is never shown: back to
camera or cropped by the frame.
Cursed Table — Game Design Document
Page 4 of 62

## Source page 5
2.2 Prologue script [MUST]
Plays once on first launch (about 60 s). Holding Space for 1 s skips it at any time (the hint
'Hold Space to skip' appears after 3 s), and on later launches a Skip button also appears.
The hero's whisper captions use the 'whisper' style: italic, off-white, typewriter 12
characters per second, a faint horizontal wobble. Voice lines are optional; captions are
always shown.
Character Look Rules
The Dealer Black waistcoat, white gloves, a
brass ball in one hand, faceless
head wearing a porcelain mask
with swappable expressions: Calm,
Grin, Crack.
Calls the Hero 'friend'. Never tells
him to hurt himself; he tempts him
to stay, sit and let go.
Seq Time Visual Motion Audio Caption
P0 until
click
Black screen, text 'Click
to Enter'
none none until clicked,
then audio
unlocks
none
P1 0-6 s Black, then rain-
streaked street at night
fades up
slow push-in 1.00
to 1.06
rain loop fades in,
soft heartbeat
'Why did she do
that...'
P2 6-16 s Panel Bouquet: hero
silhouette blurred in the
left foreground; far
right, his wife takes a
bouquet from the other
man (back to camera).
Shallow focus: hero is
out of focus, the couple
sharp
parallax drift, rain
overlay, wife's hair
sway
muffled street,
distant laugh
none
P3 16-24 s Panel Ring: close-up of
his left hand, ring, bottle
neck in the other hand
push-in, bottle
wobble
heartbeat, glass
clink
'I loved her...
unconditionally.'
Cursed Table — Game Design Document
Page 5 of 62

## Source page 6
After P8 the Tutorial starts (section 5).
2.3 Haunts (casino flashbacks) [COULD, except C1 and C2 in the prologue which
are MUST]
Three 2D flashes the casino throws at the Hero: C1 Dance (warm), C2 Laugh (cold), C3
Empty Bed (grey, her side of the bed untouched). In play: the first time Luck drops below
−6 in a cycle, play one random Haunt as a full-screen overlay for 1.5 s at 35 percent alpha,
desaturated. It never blocks input and never pauses the timer. At most one Haunt per
round and one per 20 s. A whisper line from section 2.5 plays with it.
2.4 The Dealer's mask
Mask states are three sprites on a quad: Calm (Luck 0 to 10), Grin (Luck below 0), Crack
(Luck above 10 and after dream beat 6). In Crack state a thin crack line is visible and the
left eye is open wider. Swap with a 0.15 s crossfade.
Seq Time Visual Motion Audio Caption
P4 24-32
s
Panel Door: hero from
behind stumbling
toward a neon sign THE
CURSED TABLE where
the 7 flickers and one
letter is dead; he pushes
the door
neon flicker, rain door creak, neon
buzz
none
P5 32-38
s
Cut to the 3D casino,
camera fades in at the
table, camera sway on
camera fades from
black
casino room tone,
muffled chatter
none
P6 38-42
s
Haunt C1 'Dance': warm
overexposed 2D flash
over the 3D room: the
two of them dancing at
a wedding
3 s dissolve in and
out
strings sting 'We were happy...
weren't we?'
P7 42-46
s
Haunt C2 'Laugh': cold
flash: she laughs, head
thrown back, next to the
faceless man
2.5 s sting drops a half-
tone
'Stop.'
P8 46-60
s
Dealer enters the 3D
frame, mask Calm
dealer breathing
idle
room tone, low
drone
dealer lines D1 and
D2 (section 2.5)
Cursed Table — Game Design Document
Page 6 of 62

## Source page 7
2.5 Dealer and whisper lines
All lines live in DealerLines  (ScriptableObject), grouped by situation. Pick randomly,
never repeat the same line within 5 uses of that situation. Dealer lines are gold, hero
whispers are off-white italic.
Situation Lines
D1 Intro 'Sit, friend. You look like you need a table.'
D2 Intro 'One wheel. One rule. Reach the number and the
house is kind.'
Round start 'Place your chips.' / 'The ball is in the air.' / 'Let us
see what you do with it.'
Win 'The wheel allows it.' / 'Lucky. Or are you?' /
'Again.'
Win under 40 percent of the time limit
used
'Quick hands.'
Lucky number spent 'Fortune likes you tonight.' / 'A kind number.
Enjoy it.'
Cursed number spent 'Ah. That one bites.' / 'Fate is not refundable.'
Timeout 'Too slow. The house is patient.' / 'The clock does
not wait.'
Luck first below −10 in a cycle 'Sit back. Stay a while. Everyone does.'
Defiance 'You are still here. Interesting.'
Dream armed 'The wheel wants you to see something.'
Idle 20 s with no click 'Tick, tock.'
Death 'The house always collects.'
After dream beat 6 'You are not what I feed on any more.'
Whispers (Haunts and low Luck) 'Stay.' / 'Sit. Rest. Let the wheel decide.' / 'You do
not have to carry this.' / 'One more spin.'
Cursed Table — Game Design Document
Page 7 of 62

## Source page 8
2.6 Dream beats [MUST]
A dream is a sequence of three 2D panels (about 4 s each, 12 s in total) shown inside an
iris that opens from the screen centre (the 'Second Sight'). Each panel has a hero caption.
After the last panel the iris closes, Luck is set to 2 and play resumes. Placeholder art for
each panel is a flat colour with the panel ID in large text.
No graphic injury is ever shown. Beat 4 is implied by light and sound only.
2.7 Double reward (fight back) [MUST]
Brink flag: set the moment Luck is at or below −10, cleared when a dream finishes or
on death.
Defiance: when the Brink flag is set and Luck climbs back to 0 or above, fire Defiance
once per cycle: banner 'YOU REFUSED', Luck +2, and score multiplier ×2 until the next
dream.
Beat Title Panel A Panel B Panel C Hero caption
1 The Door Rain-lit hallway: the
other man lifts a
suitcase, she stands
in the doorway light
Close-up of her hand
slipping from the
door frame; the door
shuts
She sinks to the
floor; the bouquet
from the prologue
lies in a bin
'Gone. Just
like that.'
2 The Empty
Purse
Kitchen table at
night, bills, a closed
laptop
A banking screen
showing a large 0
A pawn-shop
counter: she slides a
necklace across
'Nothing left
to hold on to.'
3 The
Photograph
She holds the
wedding photo,
thumb on his face
A phone screen: 'I am
so sorry' is typed,
then deleted letter
by letter
Forehead against a
rain-streaked
window
'She is sorry.
Now.'
4 The
Crossing
Night, red pedestrian
light, she runs
holding a phone
Headlights bloom to
full white, tyre
screech
Pure white frame
that slowly fades to
black, silence
'No... I never
wanted...'
5 The Ward Hospital corridor,
strip lights
Her hand on a bed
sheet, monitor line,
bare ring finger
Empty visitor chair,
a cheap bunch of
flowers in a plastic
cup
'Who sits
with her?'
6 The Empty
Chair
Same ward, his
reflection in the
window beside her
bed
The reflection sits
down in the chair
Her hand moves
slightly; he looks at
his own ring and
does not reach out
'This does not
hurt less.'
Cursed Table — Game Design Document
Page 8 of 62

## Source page 9
Double Vision: when a dream plays with the Brink flag set, play beat N and then beat
N+1 back to back, with the banner 'FOUGHT BACK: DOUBLE VISION'. If N is 6 only
beat 6 plays.
2.8 Ending and Leave or Stay [MUST]
After beat 6 the Ending plays: E1 Dawn at the Table: the Dealer's mask is cracked, the
wheel is still, a door at the far end of the room glows warm. Two buttons appear: Leave and
Stay.
Leave: E2 Morning Street: sunrise, the rain has stopped, he walks away. Then an end
card with final score, rounds, rank title, and buttons Play Again and Main Menu.
Stay [SHOULD]: starts Endless mode (section 10.3). The door closes and the room
returns to Neutral.
2.9 Tone and content note
Themes are heartbreak, drinking and despair. Keep everything stylised and never graphic.
The casino tempts him to stop fighting but never tells him to harm himself. The title screen
footer reads: 'This game deals with heartbreak, drinking and despair. If you are struggling,
please talk to someone you trust.' Do not add religious imagery to the cursed casino; the
numbers are explained respectfully in the Codex (section 3.5).
3. Core gameplay
A run starts at round 1 and ends on death or when the player chooses Leave. A round is
one puzzle: reach the target with the chips in your hand. Story progress (which dream
beats were seen) persists across runs in PlayerPrefs; everything else resets each run.
3.1 One round, step by step [MUST]
1. RoundIntro (2.5 s): the wheel spins fast, the ball drops, a Dealer line plays. The
TARGET number rolls up from 0 to its value over 0.8 s. The chips pop in one by one
(0.25 s each, 0.06 s stagger). The timer is not running yet.
2. RoundActive: the timer starts when the last chip finishes popping. The player
combines two chips with an operator. The two chips are removed and one result chip
is appended at the right end of the hand.
3. The round is won the moment any chip in the hand equals the target.
4. The round is lost when the timer reaches 0 (timeout) or the player quits.
Cursed Table — Game Design Document
Page 9 of 62

## Source page 10
5. RoundResolve (1.6 s): win shows a merge flash on the winning chip, floating text for
luck and score; timeout dims the screen red for 0.4 s.
6. Then in this order: death check (Luck ≤ −15), armed dream check (section 3.7), next
round.
3.2 Selecting chips and operators [MUST]
The hand holds 3 to 6 chips, shown left to right. Chip index 1 is the leftmost.
If the commit is invalid (section 3.3) both chips shake for 0.2 s, the dud sound plays and
the selection stays (chip A and operator remain). A valid commit plays the merge
animation: both chips slide to each other and fuse over 0.3 s into the result chip, which
pops in at the right end.
Equation preview (EquationPreview, section 6): shows A , then A op , then A op B = R
while B is hovered or committed. For subtraction and division the larger number is always
written first. Under Fog of Despair (Luck ≤ −10) R is shown as ?  and the real value
appears only after commit.
Current
selection
Player action Result
Nothing Click a chip Chip A selected (scale 1.12, glow ring)
Chip A Click the same chip Deselect
Chip A Click another chip Selection moves to that chip
Chip A Click an operator Operator chosen and highlighted
Chip A +
operator
Click another operator Operator changes
Chip A +
operator
Click a different chip
(B)
Try to commit A op B
Chip A +
operator
Click chip A again Ignored
Nothing Click an operator Ignored; operator button shakes 0.15 s
Anything Right-click or Esc Clear selection (Esc pauses only if nothing is
selected)
Cursed Table — Game Design Document
Page 10 of 62

## Source page 11
3.3 Operation rules [MUST]
All values are positive integers. Cap: 9999.
Subtraction and division are order independent for the player: the engine always puts the
larger number first.
3.4 Dead end, Undo and Hint [MUST]
Dead end: the hand holds exactly one chip and it is not the target. Show the banner
'DEAD END - Undo (Z)' and pulse the Undo button. The timer keeps running. There is
no luck penalty.
Undo: free and unlimited. It pops one step off the undo stack and plays the merge in
reverse. It does not refund luck: a luck value that already fired this round stays fired.
Hint: one per run, costs 2 luck. It is blocked, with the message 'Too risky', if it would
take Luck to −15 or below. It highlights chip A, the operator and chip B of the next step
of the shortest safe solution from the current hand for 4 s. If no solution exists from
the current hand, it pulses the Undo button instead. The Hint button shows a badge '1'
and greys out after use. Hints in the tutorial are free (section 5).
3.5 Luck table [MUST]
Stored in the LuckTable  ScriptableObject. A value is spent when it is used as an operand
in a committed operation, or when it is produced as the winning target chip. Each value
fires at most once per round (tracked in firedThisRound , cleared at round start),
including after Undo.
Operator Result Valid only when
Add a + b result ≤ 9999
Subtract larger − smaller a ≠ b (result at least 1)
Multiply a × b neither is 1 and result ≤ 9999
Divide larger ÷ smaller smaller is not 1 and larger mod smaller = 0
Value Tag Luck Codex blurb (shown once on first spend, 3 s card)
3 Lucky +1 Third time lucky: a lucky number in many folk
traditions.
Cursed Table — Game Design Document
Page 11 of 62

## Source page 12
Chip visuals: Lucky, Double lucky and Jackpot chips show a green marker top right (1, 2 or
3 small pips); Cursed and Doom chips show a red marker (1 or 3 pips). Untagged chips
have no marker. Tags are always visible, including in the red zone.
Value Tag Luck Codex blurb (shown once on first spend, 3 s card)
7 Lucky +1 Lucky in many Western traditions and the classic
slot-machine symbol.
8 Lucky +1 In Chinese, its sound resembles the word for
prosperity.
9 Lucky +1 In Chinese, its sound resembles the word for long-
lasting.
77 Double lucky +2 Double sevens: fortune twice over.
88 Double lucky +2 Double prosperity in Chinese numerology.
108 Jackpot +3 Sacred in Hinduism and Buddhism; prayer bead
strings commonly have 108 beads.
777 Jackpot +3 The classic slot-machine jackpot.
786 Jackpot +3 In South Asian Islamic numerology, the value
associated with the opening words of the
Bismillah; widely seen as auspicious.
888 Jackpot +3 Triple prosperity in Chinese numerology.
4 Cursed −1 In parts of East Asia, its sound resembles the word
for death.
13 Cursed −1 Triskaidekaphobia: unlucky in many Western
traditions.
17 Cursed −1 In Italy it is unlucky: XVII rearranges to VIXI, 'I have
lived'.
49 Cursed −1 In Japanese, 4 and 9 can be read as 'to suffer'.
666 Doom −3 In Christian tradition, linked with the Number of
the Beast.
Cursed Table — Game Design Document
Page 12 of 62

## Source page 13
3.6 Luck meter [MUST]
Luck is a float clamped to −15 and +15. It starts at 0 each run, or at 5 for the first run right
after the tutorial ( firstRunBonus ). It is displayed with one decimal ('LUCK +3.5').
Luck changes
The meter value animates with a 0.4 s ease. Each change spawns a floating number above
the meter (rises 80 px in 0.8 s, fades), green for plus, red for minus.
Luck State HUD colour Effects
below 0 Cursed red #E0243C Timer drains ×(1 + 0.03 × abs(Luck)), up to ×1.45 at
−15. Dealer mask Grin. Heartbeat audio from −5. At
−10 or lower: Fog of Despair (result preview
hidden).
0 to 10 Neutral white #F4F1EA Normal. Dealer mask Calm.
above
10
Blessed green
#3DDC84
Timer drains ×0.85. Second Sight: after 6 s with no
action the next step of the shortest safe solution
shimmers on the chips (free, repeatable). Dealer
mask Crack only after beat 6, otherwise Calm.
Event Change
Round won +1 + 2 × speedFactor (between +1 and +3, formula
in section 4.5)
Lucky, Double lucky, Jackpot spent +1, +2, +3
Cursed, Doom spent −1, −3
Timeout −3
Hint used −2
Defiance +2
Dream finished Luck is set to 2
Tutorial finished (first run only) start Luck 5
Cursed Table — Game Design Document
Page 13 of 62

## Source page 14
3.7 Death and dream triggers [MUST]
Death: at Luck ≤ −15, at any moment, including mid-round. Stop the timer, disable
input, play the Death sequence (section 6.8). The run ends.
Dream armed: the moment Luck reaches +15 set dreamArmed = true . It stays armed
even if Luck later falls. The dream plays after the current round resolves (win or
timeout), before the next round starts. In Endless mode this becomes the Windfall
(section 10.3).
Which beat: the lowest beat number not yet seen. With the Brink flag set, play that
beat and the next one (section 2.7).
3.8 Game states [MUST]
State Enters from What happens Leaves to
Boot app start Click to Enter screen,
audio unlock, load save
MainMenu
MainMenu Boot, Death, Ending Play, Memories, Codex,
Settings
Prologue, Tutorial,
RoundIntro
Prologue Play when not seen Script from section 2.2 Tutorial
Tutorial Prologue, or Play when
not done
Lessons L1 to L7 (section
5)
RoundIntro
RoundIntro Tutorial,
RoundResolve, Dream,
Death retry
Section 3.1 step 1 RoundActive
RoundActive RoundIntro Timer running, input
enabled
RoundResolve,
Death, Paused
RoundResolve RoundActive Section 3.1 step 5 Dream, RoundIntro,
Death
Dream RoundResolve Section 2.6 and 2.7 RoundIntro, Ending
Death any luck ≤ −15 Section 6.8 RoundIntro (Retry),
MainMenu
Ending after beat 6 Section 2.8 MainMenu,
RoundIntro
(Endless)
Cursed Table — Game Design Document
Page 14 of 62

## Source page 15
Skipping: if the prologue was seen, Play goes straight to Tutorial or RoundIntro. If the
tutorial was done, Play goes straight to RoundIntro.
4. Level generator, solver and formulas
Every level is built forward from the hand (so a solution always exists) and then verified
by a solver that also proves a safe route exists: a solution that never spends a Cursed or
Doom value. Nothing impossible can ever reach the player. [MUST]
4.1 Difficulty tiers ( DifficultyTable )
The tier is chosen by the round number within the run. Tutorial rounds use hand-written
levels and are not generated.
Big values are {25, 50, 75, 100} . Generator steps are capped at chips − 1. Intermediate
values are capped at 9999. Jackpot target sets: tier 4 {108} , tiers 5 and 6 {108, 777,
786, 888} ; tiers 1 to 3 have none. Tier 1 has no Cursed dealt chips (4 is removed from its
pool) because a 2-step route on 3 chips must spend every chip.
State Enters from What happens Leaves to
Paused RoundActive and
others
Time.timeScale = 0 ,
pause menu
previous state
Tier Rounds Chips Target
range
Operators Chip pool Generator
steps
Min safe-
route
length
Base
points
B
Penalty r
(points
per s)
1 1-4 3 8-30 Add, Sub 1-10
without 4
2 2 5 0.25
2 5-9 4 15-150 Add, Sub,
Mul
1-10 2-3 2 7 0.30
3 10-15 4-5 20-300 all four 1-12 3-4 3 9 0.35
4 16-22 5 50-500 all four 1-12 plus
one big
3-4 3 11 0.40
5 23-30 6 100-
999
all four 1-12 plus
up to two
bigs
4-5 3 13 0.45
6 31 and
up
6 100-
999
all four as tier 5 4-5 3 15 0.50
Cursed Table — Game Design Document
Page 15 of 62

## Source page 16
Tier 6 time pressure: Ts = max(6, 10 − 0.25 × (round − 30))  seconds. All other tiers
use Ts = 10 .
4.2 Data types
public enum OpKind { Add, Sub, Mul, Div }
public readonly struct Step { public int A, B, Result; public OpKind Op; }
public sealed class Level {
    public int[] StartChips; public int Target; public int Tier;
    public Step[] SafeSolution;      // never null for an accepted level
    public Step[] ShortestSolution;  // may equal SafeSolution
    public float TimeLimit;          // seconds, section 4.5
    public bool IsJackpotRound;
}
Operation.TryApply(int x, int y, OpKind op, out int result, out int left, out
int right)  implements section 3.3. For Sub and Div left  is the larger value, right  the
smaller. For Add and Mul left = x  and right = y .
4.3 Generator
Use a seeded System.Random  (seed stored in Level  logs for reproducing bugs).
Generation is spread over frames: the caller wraps each attempt in a coroutine and yields
between attempts, because it runs during RoundResolve and RoundIntro.
Level Generate(int roundIndex, Random rng):
  tier = DifficultyTable.ForRound(roundIndex)
  wantJackpot = tier.jackpotTargets.Length > 0 AND rng.NextDouble() < 0.20
  for attempt = 1 .. 400:
    count  = rng.Next(tier.chipsMin, tier.chipsMax + 1)
    chips  = DrawChips(tier, count, rng)
    steps  = rng.Next(tier.stepsMin, Min(tier.stepsMax, count - 1) + 1)
    pool   = copy of chips
    repeat steps times:
      pick two different indices i, j at random from pool
      for op in tier.operators in random order:
        if Operation.TryApply(pool[i], pool[j], op, out r, ...) AND r <= 
9999:
          remove pool[i], pool[j]; add r; break
      if no operator worked: continue outer loop (next attempt)
    target = the last r added
    reject if target < tier.targetMin OR target > tier.targetMax
Cursed Table — Game Design Document
Page 16 of 62

## Source page 17
    reject if chips contains target
    reject if LuckTable.IsCursedOrDoom(target)
    reject if wantJackpot AND attempt <= 300 AND NOT 
tier.jackpotTargets.Contains(target)
    res = Solver.Solve(chips, target, nodeBudget = 150000)
    reject if res.SafeShortest == null
    reject if res.SafeShortest.Length < tier.minSafeLength
    return new Level(chips, target, tier, res.SafeShortest, res.Shortest,
                     TimeLimit = T0 + Ts * res.SafeShortest.Length)
  return DifficultyTable.FallbackLevel(tier)   // a hand-written valid level 
per tier
List<int> DrawChips(tier, count, rng):
  pool = tier.chipPool; draw with replacement; any value tagged Lucky or 
Cursed has weight 2, others weight 1
  allow at most 2 copies of any value
  if tier.bigsMax > 0: replace rng.Next(1, tier.bigsMax + 1) of the draws 
with random values from {25, 50, 75, 100}
Prefetch: generate the next level during RoundResolve and RoundIntro of the current
round and keep it in nextLevel . If it is not ready when needed, wait on the Intro
animation (never block).
4.4 Solver
Iterative deepening depth-first search over multisets of chips. The solver is synchronous
and bounded by a node budget. Mid-round calls (Hint, Second Sight) use a budget of
40000 nodes and run only on request.
SolveResult Solve(int[] chips, int target, int nodeBudget):
  Shortest     = Search(chips, target, avoidCursed = false)
  SafeShortest = Search(chips, target, avoidCursed = true)
  return { Shortest, SafeShortest, BudgetExceeded }
Step[] Search(int[] chips, int target, bool avoidCursed):
  if chips contains target: return empty array
  for depth = 1 .. chips.Length - 1:
    visited = new HashSet<ulong>()
    if Dfs(sorted copy of chips, depth, 0): return path[0 .. foundLength)
    if nodes > nodeBudget: return null
  return null
Cursed Table — Game Design Document
Page 17 of 62

## Source page 18
bool Dfs(List<int> state, int depthLeft, int pathIndex):
  nodes++ ; if nodes > nodeBudget: return false
  if depthLeft == 0: return false
  key = Hash(state sorted, depthLeft); if NOT visited.Add(key): return false
  for every index pair i < j (skip a pair whose value pair was already tried 
at this node):
    for op in Add, Sub, Mul, Div:
      if NOT Operation.TryApply(state[i], state[j], op, out r, out left, out 
right): continue
      if avoidCursed AND (LuckTable.IsCursedOrDoom(state[i]) OR 
LuckTable.IsCursedOrDoom(state[j])): continue
      path[pathIndex] = Step(left, right, op, r)
      if r == target: foundLength = pathIndex + 1; return true
      next = state without i and j, plus r, kept sorted
      if Dfs(next, depthLeft - 1, pathIndex + 1): return true
  return false
The Hint  and Second Sight  use SafeShortest  from the current hand; if it is null they
fall back to Shortest ; if that is also null they highlight the Undo button. A Cursed value is
only a problem when spent; a Cursed result that is never used does not matter.
4.5 Time limit, score and luck-from-speed
Constants: T0 = 15 s , Ts = 10 s , grace g = 2 s , B  and r  from the tier table. k  is
the length of the safe solution, so the safe route is always finishable in time. t  is the
effective elapsed time (limit minus remaining, remaining already drains at the luck-scaled
speed).
T = T_0 + T_s \cdot k
roundScore = \max\bigl(0,\; B - r \cdot \max(0,\; t - g)\bigr)
s = \operatorname{clamp}_{[0,1]}\Bigl(1 - \frac{\max(0,\; t - g)}{T - 
g}\Bigr)
winLuck = 1 + 2s
streakMult = 1 + 0.1 \cdot \min(\text{streak},\, 10)
total = roundScore \cdot streakMult \cdot defianceMult \qquad displayed = 
\operatorname{round}(100 \cdot total)
Cursed Table — Game Design Document
Page 18 of 62

## Source page 19
streak  goes up by 1 on a win in which no Cursed or Doom value was spent. It resets
to 0 on a timeout or when a Cursed or Doom value is spent.
defianceMult  is 2 while Defiance is active (section 2.7), otherwise 1.
A timeout gives score 0 and Luck −3.
Score uses the same formula you described: with B = 5, r = 0.25 and g = 2, a win at t =
10 s scores 5 − 0.25 × 8 = 3.0.
4.6 Worked example
Tier 2, safe route of 3 steps: T = 15 + 10 × 3 = 45 s , B = 7 , r = 0.30 . Solved at t =
12 s , streak 3, no Defiance:
penalty = 0.30 × (12 − 2) = 3.0, so roundScore = 7 − 3.0 = 4.0
s = 1 − 10 / 43 = 0.767, so winLuck = 1 + 2 × 0.767 = 2.53
streakMult = 1.3, total = 4.0 × 1.3 = 5.2, displayed score 520
4.7 Self-test (required before milestone M2 ends)
An Editor menu item Cursed Table/Run Solver Self Test  generates 500 levels per tier
with seeds 1 to 500 and prints a table to the Console. It must assert:
1. No exception and no null SafeSolution .
2. Replaying SafeSolution  from StartChips  with Operation.TryApply  reproduces
Target , and no step spends a Cursed or Doom value.
3. Target  is not in StartChips , is inside the tier range and is not Cursed or Doom.
4. ShortestSolution.Length  is at most SafeSolution.Length , and
SafeSolution.Length  is at least minSafeLength .
5. Fallback levels used in under 1 percent of generations; average generation time under
40 ms and maximum under 400 ms in the Editor.
5. Tutorial [MUST]
The mechanics have layers (chips, operators, results as chips, dead ends, luck tags, clock).
So the tutorial is seven short forced lessons, about 2.5 minutes in total, delivered by the
Dealer as 'the rules of the house'. Each lesson introduces exactly one idea and reveals
HUD elements gradually. Lessons use hand-written levels from TutorialLesson  assets,
so they are never random.
Cursed Table — Game Design Document
Page 19 of 62

## Source page 20
5.1 How the coach works
TutorialController  runs the lesson steps. A step has: a caption (Dealer style,
shown in the CaptionBox), a list of highlight IDs, a required action and a flag
blockOthers .
Highlight: a brass ring image placed over the element, pulsing scale 1.00 to 1.12 at 1.2
Hz. Valid IDs: Target , Luck , Timer , Undo , Hint , Chip1  to Chip6 , OpAdd , OpSub ,
OpMul , OpDiv .
blockOthers = true: every interactive element except the highlighted ones gets
interactable = false  and alpha 0.35 through a CanvasGroup. The Pause button
always stays active.
Required actions: Continue  (click anywhere or press Space), SelectChip(n) ,
SelectOperator(op) , Commit  (any valid operation), PressUndo , PressHint , Win .
If the player does nothing for 8 s on a step with a required action, the highlight ring
flashes twice.
Luck thresholds are ignored during the tutorial: no death, no dream. Luck, score and
round are reset when the tutorial ends.
The Skip Tutorial button (section 6) appears from L3 onward. Skipping sets
ln_tutorialDone = 1  and starts the first run at Luck 0.
Quitting mid-tutorial restarts it from L1 next time. ln_tutorialDone  is saved after L7
is won.
A small label 'Lesson n/7' shows under the stats panel.
5.2 The seven lessons
Tags in these hands were checked: L1 to L4 and L7 contain no tagged value, anywhere in
their solutions. L5 and L6 are built around tags on purpose.
Lesson Hand Target HUD visible Timer Luck
effects
L1 Chips and operators 5, 6 11 Target, chips,
operators
off off
L2 Results become chips 2, 5, 6 32 same off off
L3 Dead ends and Undo 11, 5, 2 27 adds Undo off off
L4 Subtract and divide 60, 12, 5 10 same off off
L5 Luck chips 7, 5, 2 37 adds Luck meter off on
Cursed Table — Game Design Document
Page 20 of 62

## Source page 21
New HUD elements fade in over 0.5 s with a 1.1 to 1.0 scale settle when a lesson first needs
them.
5.3 Step scripts (captions are final text)
L1 Chips and operators
1. 'This is your target: 11.' Highlight Target . Action: Continue.
2. 'These are your chips. Click the 5.' Highlight Chip1 , blockOthers. Action:
SelectChip(1).
3. 'Now choose plus.' Highlight OpAdd . Action: SelectOperator(Add).
4. 'Click the 6.' Highlight Chip2 . Action: Commit.
5. 'Two chips melt into one. When any chip equals the target, you win.' Auto-continue
after 2.5 s. Action: Win.
L2 Results become chips
1. 'Every chip is used once. Results become new chips.' No highlight. Action: Continue.
2. 'Multiply the 5 by the 6.' Highlight Chip2 , OpMul , Chip3 , blockOthers. Action:
Commit (only 5 × 6 is possible).
3. 'The 30 is a new chip. Add the 2 to reach 32.' Highlight the new chip and the 2 plus
OpAdd , blockOthers. Action: Win.
L3 Dead ends and Undo
1. 'Try adding things up. 11 plus 5.' Highlight Chip1 , Chip2 , OpAdd , blockOthers. Action:
Commit.
2. 'Now add the 2.' Highlight the two remaining chips and OpAdd , blockOthers. Action:
Commit.
3. 'A dead end: one chip left and it is not the target. Press Undo.' Highlight Undo . Action:
PressUndo.
4. 'Once more.' Highlight Undo . Action: PressUndo.
Lesson Hand Target HUD visible Timer Luck
effects
L6 Cursed chips and the safe
route
4, 5, 2, 6 16 same off on
L7 The clock and the Hint 10, 11, 2,
6
76 adds Timer, Score,
Hint
60 s, no
penalty
on
Cursed Table — Game Design Document
Page 21 of 62

## Source page 22
5. 'Try 11 times 2 first.' Highlight Chip1 , Chip3 , OpMul , blockOthers. Action: Commit.
6. 'Now add the 5.' Highlight the two remaining chips and OpAdd , blockOthers. Action:
Win.
L4 Subtract and divide
1. 'Division must come out even. Divide 60 by 12.' Highlight Chip1 , Chip2 , OpDiv ,
blockOthers. Action: Commit.
2. 'Subtraction and division always put the larger number first. Now add the two 5s.'
Highlight both 5 chips and OpAdd , blockOthers. Action: Win.
3. After the win, a non-blocking card for 3 s: 'Moves that do not work just shake. Nothing
is lost.'
L5 Luck chips
1. 'This is your Luck. It decides how the house treats you.' Highlight Luck . Action:
Continue.
2. 'The green marker on the 7 means it is lucky. Spending it gives you Luck.' Highlight
Chip1 . Action: Continue. The Codex card for 7 shows once.
3. 'Make 37. Seven times five, then add the two.' No highlight, free play. Action: Win.
4. 'Below 0 you are cursed. At minus 15 the house keeps you.' Highlight Luck . Action:
Continue.
5. 'Above 10 you are blessed. At plus 15 it shows you something.' Highlight Luck . Action:
Continue.
L6 Cursed chips and the safe route
1. 'The red marker on the 4 is a curse. Spending it costs Luck.' Highlight Chip1 . Action:
Continue.
2. 'There are two ways to make 16. One spends the 4. Find the one that does not.' Free
play. After 10 s with no commit, highlight Chip2 , Chip3 , OpMul  (5 × 2). Action: Win.
3. After the win: if the 4 was spent, 'That works, but it cost you. A safe route existed: 5
times 2, then plus 6.' Otherwise 'A safe route. The house notices nothing.'
L7 The clock and the Hint
1. 'Now the clock matters. Faster wins give more Luck and more points.' Highlight
Timer . Action: Continue.
Cursed Table — Game Design Document
Page 22 of 62

## Source page 23
2. 'Stuck? The Hint costs 2 Luck, once per run. Here it is free. Try it.' Highlight Hint .
Action: PressHint. The hint badge shows an infinity sign in the tutorial and is not
consumed.
3. Free play to 76 (11 times 6, then plus 10). Action: Win. If the 60 s timer runs out, show
'Again, no penalty' and restart the lesson.
4. Dealer line: 'Good. Now the wheel is real.' Then set Luck to the first-run bonus (5),
reset score and round, set ln_tutorialDone = 1 , go to RoundIntro round 1.
5.4 Data model
TutorialLesson  ScriptableObject fields: id , int[] hand , int target , float
timerSeconds  (0 means off), bool luckEffects , HudFlags show  (Luck, Timer, Score,
Undo, Hint), TutorialStep[] steps . TutorialStep  fields: string caption , string[]
highlightIds , TutorialAction action , int actionArg , bool blockOthers , float
autoContinueSeconds .
6. Screen layout and UI [MUST]
Landscape, 16:9, desktop browser. Reasons: the 3D casino table and dealer need width,
the chip tray and operators need room, and a desktop itch.io embed is landscape. Portrait
is not supported.
6.1 Canvas setup
Overlay canvases are not affected by the 3D post-processing, which is intended. Every
canvas uses Canvas Scaler: Scale With Screen Size, reference resolution 1920x1080,
Match Width Or Height = 0.5. Anchors below are Unity RectTransform anchors,
Canvas Render mode Sort
order
Holds
Canvas_HUD Screen Space -
Overlay
10 Luck meter, target, timer, stats, chips,
operators, captions, banners
Canvas_Story Screen Space -
Overlay
20 Prologue panels, dream panels, haunts,
letterbox bars, iris mask
Canvas_Overlay Screen Space -
Overlay
30 Boot, main menu, pause, death, end
card, fade-to-black
Cursed Table — Game Design Document
Page 23 of 62

## Source page 24
anchoredPosition  is (x, y) with y up from the anchor, all sizes in reference pixels. Test at
1920x1080 and 1280x720 (the itch embed size).
6.2 Input bindings
Input Action
Mouse left click Select chips and operators, press buttons
Mouse right click Clear selection
1 to 6 Select chip at that position (left to right)
Q, W, E, R Operator Add, Sub, Mul, Div
Z or Backspace Undo
H Hint
Esc Clear selection if something is selected,
otherwise pause or resume
Space Continue, skip a panel, confirm, Sit Down Again
M Mute toggle
F1 to F6 (Editor and Development builds
only)
F1 force win, F2 Luck +5, F3 Luck −5, F4 arm
dream, F5 kill (Luck −15), F6 skip tutorial
Cursed Table — Game Design Document
Page 24 of 62

## Source page 25
6.3 Wireframe at 1920x1080
+----------------------------------------------------------------------+ y=0
| [StatsPanel]        [ LuckMeter  skull <-15 ..0.. +15> eye ]  [Timer] |
|  SCORE 01250         LUCK +3.5 · NEUTRAL                      ( 23 )  |
|  ROUND 07            +-----------------+                              |
|  DREAM 2/6           |  TARGET   88    |                              |
|  [Pause]             +-----------------+                              |
|  Lesson 3/7                                                           |
|  [Skip]                                                               |
|                 ===== 3D FOCAL ZONE (no UI) =====                     |
|  [Undo]         dealer mask  .  wheel and ball                [ + ]   |
|  [Hint]                                                       [ - ]   |
|                                                               [ x ]   |
|                    Caption box (dealer / whisper)             [ / ]   |
|                         8 x 8 = 64   (EquationPreview)                |
|          (8)  (10)  (4)  (7)  (12)  (5)   ChipTray                    |
+----------------------------------------------------------------------+ 
y=1080
6.4 Element placement (Canvas_HUD)
Element Anchor Pivot anchoredPosition Size Content and style
LuckMeter top-center 0.5, 1 0, −28 960 ×
48
Bar from −15 to +15, 32 px per luck
unit, zero at the centre. Children:
Bg (black 70 percent, 3 px brass
border), FillNeg (left half, Image
Filled Horizontal, origin Right,
red), FillPos (right half, origin Left,
white or green by state), tick lines
at ±10 (thin), end caps at ±15 (left:
skull placeholder square, right:
eye placeholder square), Pointer
(small triangle above the bar at
the current value)
LuckLabel top-center 0.5, 1 0, −82 600 ×
44
'LUCK +3.5 · NEUTRAL', 32 pt,
colour by state
TargetPanel top-center 0.5, 1 0, −136 420 ×
170
Dark 55 percent panel with brass
border. 'TARGET' 30 pt at the top,
value 120 pt bold below it (auto-
size, minimum 80 pt)
Cursed Table — Game Design Document
Page 25 of 62

## Source page 26
Element Anchor Pivot anchoredPosition Size Content and style
TimerPanel top-right 1, 1 −40, −40 200 ×
200
Radial ring (Image, Filled Radial
360) and remaining seconds 64
pt, whole seconds rounded up
StatsPanel top-left 0, 1 40, −40 420 ×
150
'SCORE 01250' 44 pt, 'ROUND 07'
30 pt, 'DREAM 2/6' 30 pt
('ENDLESS' in Endless mode)
PauseButton top-left 0, 1 40, −210 80 × 80 Two vertical bars icon
TutorialLabel top-left 0, 1 40, −296 300 ×
36
'Lesson n/7', 24 pt, tutorial only
SkipTutorialButton top-left 0, 1 40, −336 220 ×
56
'Skip tutorial', tutorial L3 onward
only
LeftColumn middle-left 0, 0.5 40, −20 170 ×
300
Vertical layout, spacing 40:
UndoButton 150 × 130 (top),
HintButton 150 × 130 with a badge
'1'
OperatorColumn middle-
right
1, 0.5 −40, −20 170 ×
540
Vertical layout, spacing 20: Add,
Sub, Mul, Div, each 150 × 120, label
80 pt, small key hint (Q, W, E, R)
22 pt bottom right
EquationPreview bottom-
center
0.5, 0 0, 260 800 ×
80
48 pt, bone white; invalid result
shown as red X
CaptionBox bottom-
center
0.5, 0 0, 350 1100 ×
90
Black 45 percent rounded panel,
34 pt. Dealer style: gold #E8C26A.
Whisper style: italic #D9D2C0
BannerSlot center 0.5,
0.5
0, 120 1000 ×
140
72 pt banners: 'DEAD END', 'YOU
REFUSED', 'FOUGHT BACK:
DOUBLE VISION'
CodexCard middle-left 0, 0.5 240, 120 460 ×
120
Number and one-line blurb,
shown 3 s on first spend of a
tagged value
ChipTray bottom-
center
0.5, 0 0, 40 1500 ×
200
Horizontal layout group, spacing
24, child alignment middle centre,
do not control child size; chips 180
× 180
VignetteOverlay stretch 0.5,
0.5
0, 0 full Image behind all HUD, raycast off:
red alpha 0.35 × clamp01(−Luck /
15) when cursed; green alpha 0.10
when Luck above 10
Cursed Table — Game Design Document
Page 26 of 62

## Source page 27
3D focal zone: the rectangle x 250 to 1650, y 310 to 770 (from the top left of the 1920x1080
frame) contains no UI except the transient CaptionBox, BannerSlot and CodexCard.
Acceptance: in a 1920x1080 screenshot the dealer's mask centre lies inside x 760 to 1160, y
320 to 470, and the wheel centre inside y 560 to 700.
6.5 Chip and operator look
Chip: 180 × 180 circle (placeholder: Unity built-in Knob  sprite), dark fill #1A1014, brass
ring #B08D3C, number in bone white 72 pt (auto-size down to 40 pt for four digits).
Tagged chips swap the ring to green #3DDC84 (Lucky, Double lucky, Jackpot) or red
#E0243C (Cursed, Doom), with 1, 2 or 3 small pips at the top right. Result chips get a
gold inner ring and a 0.4 s sparkle on creation.
Selected chip: scale 1.12 and a 6 px white glow ring.
Operator button: rounded square (placeholder: built-in UISprite ), fill #1E1218, brass
border. Selected: white border and fill #3A2A20.
Blocked by tutorial: alpha 0.35, no raycast.
6.6 Typography and colours
Create a dynamic TMP font asset UI_Dynamic  from Liberation Sans and verify in Play
mode that the glyphs × ÷ − · render; if one is missing fall back to ASCII x , / , - . If a serif
font is supplied later in Art/Fonts/  use it for titles only. No emoji anywhere.
Name Hex Use
Ink #07050A Backgrounds, void
Oxblood #3A0A12 Panels in cursed look
Bone #EDE6D6 Body text
Brass #B08D3C Borders, rings
Neutral white #F4F1EA Luck 0 to 10
Curse red #E0243C Luck below 0, cursed markers
Blessed green #3DDC84 Luck above 10, lucky markers
Dealer gold #E8C26A Dealer text
Whisper #D9D2C0 Hero whispers
Cursed Table — Game Design Document
Page 27 of 62

## Source page 28
6.7 UI animation spec
Element Trigger Animation Duration Easing
Chip Dealt Scale 0 to 1 with
overshoot, 0.06 s
stagger
0.25 s EaseOutBack
Chip Selected Scale to 1.12 0.12 s EaseOutQuad
Two chips Valid commit Slide to midpoint and
shrink to 0.6, then
result scales 0 to 1.15 to
1
0.2 s + 0.15
s
EaseInQuad,
EaseOutBack
Chips Invalid commit Shake x ±10 px, 4 cycles 0.2 s linear
Luck
meter
Value changes Fill lerp, colour
crossfade
0.4 s, 0.25
s
EaseOutQuad
Floating
number
Luck or score
change
Rise 80 px, fade over
the last 0.3 s, 40 pt
0.8 s EaseOutQuad
Timer ring Always Linear fill; below 30
percent remaining the
ring is red and pulses
scale 1 to 1.08 at 2 Hz
n/a n/a
Target
value
RoundIntro Number roll from 0 0.8 s EaseOutCubic
Target
value
Win Pulse 1.0 to 1.25 to 1.0,
gold
0.3 s EaseOutQuad
Banner Triggered Scale 1.4 to 1.0, hold 1.2
s, fade
0.25 + 1.2 +
0.3 s
EaseOutBack
HUD Dream,
prologue,
ending
Alpha to 0 0.3 s EaseInOutSine
3D camera Cursed spent
or timeout
Positional shake ±0.04
m
0.25 s decaying
Cursed Table — Game Design Document
Page 28 of 62

## Source page 29
6.8 Full-screen screens
Screen Layout
Boot Black. 'CURSED TABLE' 120 pt at (0, 80) from
centre; 'Click to Enter' 40 pt pulsing at (0, −120);
content note footer 24 pt, 40 px from the bottom
Main menu Static dark background with slow fog drift. Buttons
at the centre (0, −40), each 420 × 80, spacing 20, in
this order: Play, Memories [SHOULD], Codex
[SHOULD], Settings. Content note footer
as on Boot
Pause Black 70 percent overlay; Resume, Settings
(Master, Music, SFX sliders 0 to 1 and a Mute
toggle), Memories, Main Menu; buttons 420 × 80
Death 0 to 0.4 s: input off, chips fall out (scale to 0 with
rotation), screen fades to red. 0.4 to 2.0 s: Dealer
line 'The house always collects.' 64 pt gold, mask
Grin. Then a stats panel: ROUNDS, BEST STREAK,
SCORE, RANK (section 10.4), DREAMS SEEN x/6,
with buttons 'Sit Down Again' (Space) and 'Main
Menu'. Retry must start the next RoundIntro in
under 2 s
Dream Letterbox bars 135 px at top and bottom, 1920 × 810
panel area, circular iris mask opening from the
centre over 1.0 s, caption centred near the bottom
of the panel area; HUD hidden
Ending E1 3D scene, Dealer mask Crack, two buttons 360 × 90
at (−220, −200) and (220, −200) from the centre:
Leave, Stay
End card Black. 'THE END' 120 pt, final score, rounds, rank,
buttons Play Again and Main Menu
Cursed Table — Game Design Document
Page 29 of 62

## Source page 30
7. 3D casino: look and feel [MUST]
Mood: neo-noir in a decaying baroque casino. Dark velvet, tarnished brass, candle-amber
pools of light, neon in the distance, mist on the floor. Style: stylised and simple (flat-colour
primitives now, hand-painted low-poly later). The cursed feeling comes from lighting, fog,
colour, sound and camera, not from model detail. Everything beyond the camera's view is
darkness or fog.
Units are metres, Y is up, the player looks toward +Z. Everything below is built from Unity
primitives and lives under Casino3D , which is inactive until the prologue reaches P5.
7.1 Scene objects and placement
Name Primitive Position Rotation Scale Notes
Floor Cube 0, −0.05, 3 0, 0, 0 16, 0.1,
16
M_Floor_Carpet
WallBack Cube 0, 2.5, 8 0, 0, 0 16, 5,
0.2
M_Wall_Velvet
WallLeft / WallRight Cube −8 / 8, 2.5, 3 0, 0, 0 0.2, 5,
16
M_Wall_Velvet
WallFront Cube 0, 2.5, −4 0, 0, 0 16, 5,
0.2
behind the camera
Ceiling Cube 0, 5.1, 3 0, 0, 0 16, 0.2,
16
M_Ceiling
Pillar_L / Pillar_R Cylinder −4 / 4, 2.5, 5 0, 0, 0 0.8, 2.5,
0.8
M_Wood_Mahogany
Curtains_L /
Curtains_R
Cube −5 / 5, 2.4,
7.8
0, 0, 0 3, 4.8,
0.1
M_Wall_Velvet, slightly
brighter
TableBase Cylinder 0, 0.46, 0.4 0, 0, 0 0.9,
0.46,
0.9
M_Wood_Mahogany
TableTop Cylinder 0, 0.92, 0.4 0, 0, 0 3.5,
0.03,
3.5
M_Wood_Mahogany
Felt Cylinder 0, 0.952, 0.4 0, 0, 0 3.2,
0.005,
3.2
M_Felt (tinted by luck)
WheelBase Cylinder 0, 0.99, 0.4 0, 0, 0 1.2,
0.04,
1.2
M_Wood_Mahogany
Cursed Table — Game Design Document
Page 30 of 62

## Source page 31
Name Primitive Position Rotation Scale Notes
WheelRim Cylinder 0, 1.015, 0.4 0, 0, 0 1.15,
0.01,
1.15
M_Brass
WheelRotor Cylinder 0, 1.04, 0.4 0, 0, 0 0.9,
0.02,
0.9
rotates about Y; holds
Pocket_00 to Pocket_36
and Ball when resting
Pocket_00 to
Pocket_36
Cube (child of
WheelRotor)
ring, radius
0.36
face the
centre
0.07,
0.02,
0.1
00 green, then
alternating red and black
WheelHub Sphere 0, 1.08, 0.4 0, 0, 0 0.12,
0.12,
0.12
M_Brass
Ball Sphere orbit radius
0.42 around
the hub, y
1.06
n/a 0.035 M_Ball
Dealer/Body Capsule 0, 0.8, 2.5 0, 0, 0 0.5, 0.8,
0.35
M_Dealer_Cloth
Dealer/Head Sphere 0, 1.7, 2.5 0, 0, 0 0.26,
0.28,
0.26
M_Dealer_Cloth
Dealer/MaskQuad Quad 0, 1.7, 2.36 0, 0, 0 0.22,
0.28, 1
M_Mask unlit, a Quad is
visible from −Z so it
faces the camera; sprite
slots Mask_Calm,
Mask_Grin, Mask_Crack
Dealer/ArmR Capsule 0.45, 1.1, 2.2 0, 0, −60 0.12,
0.35,
0.12
M_Glove at the hand end
SlotSilhouette_01 to
_06
Cube + Quad
screen
x −7, −5, −3,
3, 5, 7 at z
7.2, y 0.9
0, 0, 0 0.8, 1.8,
0.6
screen quad blinks
between
M_Slot_ScreenA and B
SlotSilhouette_07 / _08 same ±6.6, 0.9, 4.8 0, ±25, 0 same same
Table_Far_01 / _02 Cylinder −5 / 5, 0.46,
4.5
0, 0, 0 2.6,
0.46,
2.6
only a silhouette in the
fog
NeonSign_HollowSeven Quad + TMP 3D
text
0, 3.6, 7.85 0, 0, 0 4, 1, 1 text 'THE HOLLOW
SEVEN', magenta
emission, the 7 flickers,
the letter L is dead (dark)
Cursed Table — Game Design Document
Page 31 of 62

## Source page 32
The agent must tune only the camera pitch and height (not the object positions) until the
focal-zone acceptance in section 6.4 passes. Starting values above are computed to pass.
7.2 Lights (real-time, no baking)
No directional light. At most 4 real-time lights in the scene (URP Additional Lights per
Object Limit 4).
7.3 Materials (URP Lit unless noted)
Name Primitive Position Rotation Scale Notes
Chandelier Sphere body + 6
small emissive
spheres
0, 3.9, 0.6 0, 0, 0 body
0.4
ring of bulbs radius 0.7,
M_Emissive_Amber
CameraRig empty 0, 1.4, −1.8 0, 0, 0 1 carries sway and shake
PlayerCamera Camera, child of
CameraRig
0, 0, 0 5.5, 0, 0 1 FOV 50, near 0.1, far 40,
clear colour Ink, HDR on,
FXAA
Light Type Position Rotation Range Shadows Notes
SpotTable Spot, angle
60, inner 35
0, 3.4,
0.4
90, 0, 0 7 Soft off,
atlas 1024
Key light over the wheel.
Baseline intensity is
authored so the felt reads
mid-dark; all state values
below are multipliers of that
baseline
PointDealer Point 0, 2.2,
1.6
n/a 3 none Lights the mask from the
front
PointNeon Point 0, 3.2, 7 n/a 6 none Magenta, low intensity
ChandelierLight Point 0, 3.9,
0.6
n/a 8 none Amber, flickers
Material Base colour Metallic Smoothness Emission
M_Floor_Carpet #2B0D12 0 0.15 none
M_Wall_Velvet #1B0A10 0 0.05 none
M_Ceiling #0E070B 0 0.05 none
Cursed Table — Game Design Document
Page 32 of 62

## Source page 33
Only brass and wood have strong highlights. Nothing mirrors. No textures in the first
build; a pattern texture for the carpet is [COULD].
7.4 Luck drives the room ( CasinoDirector )
Compute tCurse = clamp01(−Luck / 15)  when Luck is below 0, otherwise 0. Compute
tBless = clamp01((Luck − 10) / 5)  when Luck is above 10, otherwise 0. Every
parameter is Neutral + (Cursed − Neutral) × tCurse + (Blessed − Neutral) ×
tBless , smoothed with SmoothDamp  over 0.6 s. Post-processing uses three global
Volumes ( Vol_Neutral  weight 1, Vol_Cursed  weight tCurse, Vol_Blessed  weight
tBless) with profiles holding the values below.
Material Base colour Metallic Smoothness Emission
M_Wood_Mahogany #3B1F14 0 0.65 none
M_Felt #0F2A22 (tinted
by state)
0 0.10 none
M_Brass #B08D3C 0.9 0.65 none
M_Pocket_Red / Black /
Green
#7A1020 /
#0C0A0C /
#0E5A34
0 0.5 none
M_Ball #F2EDE0 0 0.9 none
M_Dealer_Cloth #0B090C 0 0.2 none
M_Glove #E9E4D8 0 0.2 none
M_Mask (URP Unlit) white, mask sprite n/a n/a n/a
M_Emissive_Amber #3A2A10 0 0.3 #FFB86B × 2.0
M_Neon_Magenta #200010 0 0.3 #FF2A6D × 3.0
M_Slot_ScreenA / B #101010 0 0.3 #36F0FF × 1.5 /
#FF2A6D × 1.5
Parameter Neutral Cursed (−15) Blessed (+15)
SpotTable intensity (×
baseline)
×1.0 ×0.35 ×1.35
Cursed Table — Game Design Document
Page 33 of 62

## Source page 34
Parameter Neutral Cursed (−15) Blessed (+15)
SpotTable colour #FFC77A #FF5A4A #FFE9B0
PointDealer intensity (×
baseline)
×1.0 ×1.6 ×0.8
PointDealer colour #FFF1DC #FF1F3A #FFD27A
Chandelier flicker (per
second)
0.2 1.5 0.05
Neon flicker (per second) 0.3 2.0 0.1
Fog colour #120C10 #1A0508 #2A2112
Fog density (Exponential
Squared)
0.030 0.060 0.018
Ambient colour #1A1418 #200A0E #2E2418
Felt colour #0F2A22 #2A0F14 #12332A
Mist alpha multiplier ×1.0 ×1.8 ×0.6
MistGround emitter z 3.5 1.2 4.5
Camera sway multiplier ×1.0 ×1.8 ×0.4
Wheel idle speed (deg per
s)
20 35 12
Vignette intensity 0.30 0.55 0.20
Chromatic aberration 0.10 0.45 0.05
Film grain intensity 0.25 0.50 0.15
Colour adjustments:
saturation
−10 −45 +15
Colour adjustments:
contrast
+10 +25 +5
Colour filter #FFFFFF #FFB0B0 #FFF1C8
Bloom intensity (threshold
1.0, scatter 0.6)
0.6 0.4 1.2
Cursed Table — Game Design Document
Page 34 of 62

## Source page 35
A flicker drops a light to 30 percent intensity for a random 0.06 to 0.15 s.
7.5 Fog placement
1. Global fog: RenderSettings.fog = true , Exponential Squared, colour and density
from the table. It hides the far room and makes the silhouettes (slot machines, far
tables, pillars) read as shapes.
2. MistGround: particle system at (0, 0.25, 3.5), box emission 10 × 0.3 × 8, 40 particles,
size 3 to 5 m, lifetime 20 s, velocity (0.05, 0, 0.02), alpha 0.08 to 0.14. Low mist along
the floor that separates the dealer from the background.
3. MistBack: particle system at (0, 1.7, 5.5), box emission 12 × 1.2 × 4, 24 particles, size 4
to 6 m, alpha 0.05 to 0.10. Gives depth behind the dealer. Never place mist within 0.8 m
of the mask.
4. MistNear [SHOULD]: only while Luck is −10 or lower (Fog of Despair): 12 particles of
size 1.5 m, alpha up to 0.12, drifting slowly across the table at (0, 0.9, −0.6). It fades in
and out over 1 s.
5. All mist uses a soft radial gradient sprite Art/Placeholders/SoftCircle.png  (256 ×
256, generated by an Editor script), an Unlit alpha-blended particle material, billboard
mode, soft particles off (WebGL cost). Total particles never exceed 80.
7.6 Camera: drunk sway
CameraRig applies smooth Perlin-noise offsets. Position ±0.02 m, roll ±1.2°, pitch ±0.8°,
yaw ±1.0°, noise speed 0.35 Hz, all × the sway multiplier from the table, plus a vertical bob
of ±0.01 m at 0.25 Hz. Every 6 to 10 s a 'blink': Vignette intensity spikes +0.35 for 0.15 s. At
Luck −10 or lower add a slow extra roll of ±2.5°. On a Cursed spend or a timeout the rig
shakes ±0.04 m for 0.25 s, decaying.
Parameter Neutral Cursed (−15) Blessed (+15)
Dealer mask Calm Grin Calm (Crack
after beat 6)
Heartbeat off from Luck −5: volume 0 to
0.8, 60 to 120 BPM at −15
off
Cursed Table — Game Design Document
Page 35 of 62

## Source page 36
7.7 Wheel and ball
The landing pocket is random and cosmetic. It never shows or implies the target.
7.8 Dealer
Breathing: scale Y ±1.2 percent at 0.25 Hz. Head yaw follows the ball within ±8°. Mask
swap is a 0.15 s crossfade. ArmR flicks from 0° to −70° and back over 0.6 s at the start of
RoundIntro. The dealer never walks.
7.9 URP and WebGL budget
Forward renderer, HDR on, MSAA off, FXAA on the camera, render scale 1.0. Depth
texture and opaque texture off. SSAO off. Shadows only from SpotTable: atlas 1024,
distance 12, soft shadows off.
Post-processing: Bloom, Vignette, Chromatic Aberration, Film Grain, Color
Adjustments only. No Depth of Field or Motion Blur.
Budget: under 150 draw calls, under 60k triangles, no textures except the mist sprite
and mask sprites, 60 FPS on a mid-range desktop at 1280x720 in the browser.
Quality option [SHOULD]: a Low setting that disables Bloom, Chromatic Aberration
and Film Grain, sets render scale 0.75 and turns off MistNear and MistBack.
State Behaviour
Idle WheelRotor spins at the idle speed from the table.
The ball rests in a pocket, parented to the rotor
RoundIntro Rotor at 240 deg per s, easing to idle over 2.0 s. The
ball is released on the opposite direction at 360
deg per s on radius 0.42, spirals in to radius 0.30
and settles in a random pocket at t = 2.3 s with a
drop sound. The target number roll starts at t = 1.7 s
Win SpotTable intensity ×1.5 for 0.3 s
Timeout Rotor stops in 0.2 s, mask Grin for 1.5 s
Cursed Table — Game Design Document
Page 36 of 62

## Source page 37
8. 2D sprites, animation and motion budget
All 2D story scenes (prologue, haunts, dreams, end card) are layered cut-out sprites on
Canvas_Story , animated only with transforms and sprite swaps. No skeletal rigs, no 2D
Animation package, no frame-by-frame cutscenes. The look comes from four cheap tricks:
parallax layers, a slow camera push, tiny breathing and sway on characters, and overlay
effects (rain, grain, bokeh).
8.1 Asset specification (for the human artist; the agent builds placeholders)
Placeholders (built by the agent): an Editor script PlaceholderArtGenerator  writes a
256 × 144 solid-colour PNG for every slot, with a distinct hue per slot role, into the same
folders and names. In the Editor and Development builds each StoryPanel also shows a
TMP label with its panel ID. Replacing a PNG with real art must need no code or prefab
change.
Item Specification
Background layers 2048 × 1152 PNG, RGBA, 16:9
Character and prop layers Transparent PNG, at most 1024 px tall, pivot set per
section 8.2
Overlay layers 2048 × 1152 PNG with alpha (rain tile 512 × 512
seamless)
Import Sprite (2D and UI), Pixels Per Unit 100, Filter
Bilinear, Mip Maps off, sRGB on, Max Size 2048,
Compression Normal Quality with Crunch on,
quality 50
Names <PanelId>_<slot>.png , for example
D1A_bg_far.png , D1A_man_walk.png
Location Assets/_Project/Resources/Story/<PanelId>/
Loading Resources.LoadAsync  for a sequence's panels just
before it plays; call
Resources.UnloadUnusedAssets  after it ends
Memory budget Never more than 12 story textures alive at once (a
dream has 3 panels with at most 4 layers each)
Cursed Table — Game Design Document
Page 37 of 62

## Source page 38
8.2 Layer model
Every panel is a StoryPanel  prefab with fixed slots in this order (back to front): bg_far ,
bg_mid , subject_1 , subject_2 , fx_mid , fg , overlay_rain , overlay_grain , tint .
Unused slots are inactive. A StoryLayer  component on each slot has: parallax ,
breathAmp , breathPeriod , swayDeg , swayPeriod , driftPxPerSec , phaseOffset .
Camera push (Ken Burns): a virtual camera per panel goes from zoomStart  to zoomEnd
(default 1.00 to 1.08) and pans from panStart  to panEnd  (default −30 px to +30 px on X)
over the panel duration, EaseInOutSine. Each layer's position is −cameraOffset ×
parallax .
Timing: each panel lasts 4 s by default, with a 0.4 s crossfade into the next. The caption
starts at 0.6 s with the whisper typewriter and stays until the panel ends. The prologue
panels use the durations in section 2.2.
Slot role Parallax Motion
bg_far 0.15 none
bg_mid 0.40 none
subject (character), pivot at the feet 1.00 Breathing: scale Y ±1.5 percent over
3.2 s. Sway ±1.0° over 5 s
hair or cloth (separate sprite, pivot at
the attachment point)
1.00 Sway ±3° over 2.4 s, random phase
face (expression sprite slot) follows
subject
Swap on cue. Blink: every 4 to 6 s
swap to eyes-closed for 0.12 s, only
if a closed sprite exists
fg (bokeh, rain streaks, branches) 1.80 Drift 20 px per s
overlay_rain none RawImage UV scroll (0, −1.8) per s,
alpha 0.35
overlay_grain none Random UV offset every 0.08 s,
alpha 0.08
tint none Full-screen colour multiply: beats 1
to 3 #FFE8D0 at alpha 0.15 (warm),
beats 4 to 6 #C8D8FF at alpha 0.15
(cool)
Cursed Table — Game Design Document
Page 38 of 62

## Source page 39
8.3 Character sprite slots
Characters are made of a few separate sprites so they can breathe and sway. No lip-sync,
no walk cycles except the two-frame run.
8.4 Panel recipes
Character Slots
Hero hero_back  (silhouette from behind), hero_side ,
hero_hand_ring  (close-up), hero_bottle ,
hero_blur  (blurred copy for P2),
hero_reflection  (face visible, only for D6)
Wife wife_stand , wife_sit , wife_floor , wife_bed ,
wife_run_a , wife_run_b , wife_hand ,
wife_hair_back  (sways separately), heads:
wife_head_neutral , wife_head_sad ,
wife_head_shock , wife_head_smile
Other man man_back , man_walk  (with suitcase). Never a face
Props bouquet , necklace , photo , phone_screen ,
bank_screen , neon_sign , neon_seven , door ,
headlights , monitor_line , flowers_cup ,
suitcase
Panel Slots Motion beyond the defaults
P1 Street bg_far city, bg_mid shopfronts,
overlay_rain, overlay_grain
Push-in 1.00 to 1.06
P2 Bouquet bg_far, bg_mid lamps, subject_1
wife_stand with wife_hair_back,
subject_2 man_back (still), fg
hero_blur at left, rain
At 5 s hero_blur crossfades to a
sharp hero_side (focus pull). Hero
swaying ±1.5°
P3 Ring bg_far bokeh, subject_1
hero_hand_ring, subject_2
hero_bottle
Push-in 1.00 to 1.12. Bottle wobble
±2° over 1.2 s
Cursed Table — Game Design Document
Page 39 of 62

## Source page 40
Panel Slots Motion beyond the defaults
P4 Door bg_far, bg_mid, subject_1 hero_back,
fx_mid neon_sign and neon_seven,
door
Hero bob ±6 px at 1.4 Hz.
neon_seven alpha flickers 0.4 to 1.0
every 0.06 to 0.2 s. At 6 s the door
squishes in X 1 to 0.2 over 0.5 s
(pivot left)
C1 Dance bg_far warm bokeh, two figure
silhouettes
Rotate ±4° over 3 s. Additive white
vignette. 0.5 s dissolve in and out.
No rain
C2 Laugh bg_far, wife_head_smile, man_back
cropped at the edge
Head bob ±2 px at 8 Hz. Cold tint
#C8D8FF alpha 0.25
C3 Empty
Bed
bg_far bed, fx_mid curtain Curtain sway ±3° over 4 s.
Everything else still
D1A hallway, man_walk, wife_stand man_walk moves right 40 px over 4
s. Light flicker overlay
D1B door frame, wife_hand, door wife_hand slides down 60 px over 2
s. At 2.5 s door shakes 6 px once
and a 0.1 s dark flash
D1C room, wife_floor, bouquet in bin Breathing. Window rain overlay
D2A kitchen, wife_sit Lamp sway ±1°. Slow push
D2B bank_screen, wife_hand The 0 on screen pulses scale 1.0 to
1.1 every 1.2 s. wife_hand taps once
D2C pawn counter bg, necklace, fg
counter
necklace slides right 200 px over 2 s
D3A wife_sit, photo photo tilts ±1°. A tear sprite falls 120
px and fades at 2.5 s
D3B phone_screen with a TMP text Typewriter 'I am so sorry' at 10
characters per s, then backspace at
8 characters per s. Cursor blinks at
1 Hz
D3C window with rain, wife_sit Rain UV scroll 2.5 per s. Breathing
Cursed Table — Game Design Document
Page 40 of 62

## Source page 41
8.5 Iris (Second Sight) and captions
The dream sits inside a circular iris: a Mask  with a circle sprite scaled from 0 to 2200 px
over 1.0 s (EaseOutCubic) to open and back to 0 over 0.8 s (EaseInCubic) to close.
Captions use the whisper style and sit near the bottom of the 1920 × 810 panel area.
8.6 What moves and what stays still
Rule: at most 6 continuously moving things in the 3D scene at once, and nothing moves in
the outer 20 percent of the frame except mist and light flicker. Movement is for the wheel,
the dealer, the light and the air; the room is still.
Panel Slots Motion beyond the defaults
D4A street, traffic light (red), wife_run_a
and wife_run_b
Swap at 6 fps. Moves left to right
300 px over 4 s. Rain
D4B headlights additive Scale 0.2 to 3.0 over 1.5 s. Screen
shake 10 px at the end. Screech
sound
D4C pure white White fades to black over 3.5 s.
Nothing else
D5A corridor One strip light flickers at a time
every 0.8 s. Push 1.00 to 1.15
D5B wife_bed, monitor_line monitor_line UV scroll 0.6 per s.
Hand breathing
D5C chair, flowers_cup Slow push only
D6A window night city, bed,
hero_reflection at alpha 0.5
Reflection breathing
D6B same hero_reflection moves down 80 px
into the chair over 2.5 s
D6C wife_hand, hero_hand_ring wife_hand moves ±4 px once. Push
1.00 to 1.15 toward the ring
E2 Morning
Street
sky gradient, hero_back hero_back walks away and shrinks
1.0 to 0.7 over 6 s. Additive flare
grows. No rain. Fade to the end card
Cursed Table — Game Design Document
Page 41 of 62

## Source page 42
8.7 Cheap look boosters [SHOULD]
1. A warm rim-light copy of each subject: the same sprite tinted #FFB86B, offset 4 px
toward the light, additive, alpha 0.4.
2. Blur the bg_far  layer in the source art and keep subjects sharp, to fake depth of
field.
3. Keep one silhouette colour per character in every panel so the viewer always
recognises her yellow coat and his dark suit.
4. Always keep the grain overlay on; it hides compression and unifies mixed art.
9. Audio
Audio carries half of the curse. Music is one layered track whose mix follows Luck, so the
player hears the meter before reading it. The agent builds the whole system with empty
AudioClip slots; it must work silently when slots are empty and play correctly when clips
are dropped in.
Area Moves Stays still
3D casino
[MUST]
Wheel rotor, ball, camera sway,
dealer breathing and head turn,
mask swap, mist drift, felt and
light colour changes
Floor, walls, ceiling, pillars, curtains,
table, far tables, dealer body, slot
machine bodies
3D casino
[SHOULD]
Chandelier and neon flicker,
dealer arm flick
Everything else
3D casino
[COULD]
Slot screens blink at random 0.8 to
2.5 s
n/a
2D scenes Parallax, camera push, breathing,
hair sway, blink, rain, grain, bokeh
drift, scripted prop moves from
the recipes
The painted backgrounds and
props not listed in a recipe
UI Only on events (section 6.7); the
timer ring and the luck pointer are
the only constant motion
Frames, labels, buttons at rest
Cursed Table — Game Design Document
Page 42 of 62

## Source page 43
9.1 System rules [MUST]
1. AudioManager  owns all playback. Other scripts call AudioManager.PlaySfx(SfxId) ,
PlayVoice(VoiceId) , SetMusicState(...) . A null clip does nothing and logs
nothing.
2. No AudioMixer, no mixer snapshots, no AudioLowPassFilter or other audio effect
components. Support for these on WebGL is limited. Buses are plain float scalars in
code: Master, Music, SFX, Voice, Ambience, each 0 to 1, saved in PlayerPrefs. Defaults:
Master 1.0, Music 0.7, SFX 0.9, Voice 1.0, Ambience 0.6. Final volume = clip volume ×
bus × Master.
3. Audio starts only after the Click to Enter click (browser autoplay rule). The
AudioManager creates its sources on that click.
4. SFX use a pool of 12 AudioSource s. At most 3 instances of the same SFX at once.
Default pitch variation ±3 percent unless a sound is flagged fixed pitch below.
5. Import settings: Vorbis quality 0.4; SFX and ambience mono, music stereo; short SFX
(under 1 s) Decompress On Load, music and ambience Streaming, everything else
Compressed In Memory.
6. Mute (M key or Settings) sets Master to 0 and shows a small speaker-off icon in the
pause menu. While paused, Music and Ambience are multiplied by 0.4.
9.2 Layered music [MUST structure, clips optional]
Three stems with the same length and tempo play together from the start of the first
RoundIntro, started with AudioSettings.dspTime  and PlayScheduled  so they stay in
sync. Volumes follow Luck (smoothed 0.6 s), using tCurse  and tBless  from section 7.4.
Other tracks: MUS_Menu  (title and main menu), MUS_Dream  (warm, plays during dreams;
casino stems fade to 0 over 0.5 s and return over 1.0 s afterwards), MUS_Ending  (Leave
ending, hopeful).
Stem Content Volume
MUS_Base Slow noir loop: low piano or upright bass, soft brush
drums, distant room
1 − 0.4 × tCurse
MUS_Tension Detuned drone, low strings, irregular pulse tCurse
MUS_Shimmer Warm pad, bell tones, high strings tBless
Cursed Table — Game Design Document
Page 43 of 62

## Source page 44
9.3 Ambience
9.4 Sound effects
ID Where Behaviour
AMB_Rain Prologue P1 to P4, dream panels
with rain
Fades in and out with the panel
AMB_RoomTone In the casino Always on, low murmur and
ventilation
AMB_Heartbeat In the casino Off above Luck −5. From −5 to −15
volume rises 0 to 0.8 and tempo 60
to 120 BPM (pitch stays fixed)
AMB_NeonBuzz At the neon sign (3D source,
spatial blend 1, min distance 2, max
12)
Tied to neon flicker: volume drops
with each flicker
ID Trigger Description
SFX_ChipDeal Each chip pops in Short clay tick, pitch +2 percent per
chip (fixed pitch)
SFX_ChipSelect Chip selected Soft chip click
SFX_OperatorSelect Operator chosen Dry mechanical tick
SFX_Merge Valid commit Two chips clack together, short rising
swish
SFX_Invalid Invalid commit Muted dud thump
SFX_Undo Undo Reversed swish
SFX_TargetRoll Target number rolls Fast ticking, 0.8 s
SFX_WheelSpin RoundIntro Rotor whirr, loop, fades with speed
SFX_BallRattle RoundIntro Ball on track, loop for 2.3 s
SFX_BallDrop Ball settles Single bounce and settle
Cursed Table — Game Design Document
Page 44 of 62

## Source page 45
ID Trigger Description
SFX_Win Round won Short rising stinger. Pitch ×(1 + 0.05 ×
min(streak, 10))
SFX_LuckUp Luck rises Soft chime, pitch higher for bigger
gains
SFX_Jackpot Jackpot value spent Full bright chord with coin shimmer
SFX_Curse Cursed value spent Low knell plus a quiet whisper tail
SFX_Doom Doom value (666)
spent
Deep hit and sub drop
SFX_Timeout Timer reaches 0 Buzzer, wheel brakes
SFX_TimerTick Each second of the
last 5 s
Clock tick (fixed pitch)
SFX_Hint Hint or Second Sight Soft glass shimmer
SFX_DeadEnd Dead end Low thud
SFX_Banner Banner appears Heavy hit
SFX_DreamOpen Iris opens Whoosh into a warm swell
SFX_DreamClose Iris closes Reverse swell
SFX_Haunt Haunt overlay Reversed strings, 1.5 s
SFX_MaskCrack Mask becomes
Crack
Porcelain crack
SFX_Death Death sequence Low hit and chips scattering
SFX_DoorCreak P4 and E2 Wooden door creak
SFX_Screech D4B Tyre screech, cut to silence
SFX_UIClick  /
SFX_UIHover
Menu buttons Neutral clicks
Cursed Table — Game Design Document
Page 45 of 62

## Source page 46
9.5 Voice (optional)
Every Dealer and whisper line in section 2.5 has a VoiceId  slot. If a clip is missing the
caption is shown alone. Dealer: low, calm, slightly amused, close microphone. Hero
whispers: breathy, close, a little unsteady, with a gentle low-pass and light chorus applied
in the source audio (not in Unity). Dealer voice plays over the Voice bus and ducks Music
by 3 dB while a line plays.
9.6 Luck and audio summary
10. Retention, endless mode and pacing
The goal is a 5 to 10 minute session that a jam voter finishes the first dream of, plus a
reason to come back. Everything in this section is built from systems already specified;
the only new content is Endless mode, ranks and the optional Fortune Cards.
10.1 Hooks
Luck What the player hears
Below 0 Tension stem rises, base thins, heartbeat from −5,
dealer mask Grin, curse knell on each cursed spend
0 to 10 Base stem and room tone, calm
Above 10 Shimmer stem rises, bright chimes on lucky spends
Dream Casino stems fade out, MUS_Dream  fades in, rain
only where the panel has rain
Hook How it works Priority
One more round A round lasts about 40 s. Win, a Dealer line and the
next target arrive within 4 s
MUST
Risk against reward The shortest route may spend a cursed number;
the safe route is longer. Lucky numbers give a
bigger payout for a longer route
MUST
Cursed Table — Game Design Document
Page 46 of 62

## Source page 47
10.2 Pacing of a first session
Hook How it works Priority
Rubber band Blessed: slower timer and Second Sight. Cursed:
faster timer and hidden previews. Winning helps
you, losing hurts you, so the game stays tense
MUST
Story curiosity Each dream ends on a question (who sits with
her?) and the next one is one cycle away
MUST
Fight back for double Surviving the red zone doubles the next dream and
doubles score until it plays
MUST
Visible stakes The room changes with Luck, so the player feels
the danger without reading a number
MUST
Instant retry After death, Space starts the next round in under 2
s. Dream beats seen are kept
MUST
Collection Codex of 15 numbers, Memories gallery of the 6
beats and the ending, rank titles
SHOULD
Endless mode After the ending, Stay keeps the casino open with
escalating curses
SHOULD
Fortune Cards One collectible card per Windfall in Endless mode COULD
Haunts Random 1.5 s flashbacks at low Luck for dread COULD
Time What the player experiences
0:00 Click to Enter. The prologue starts within 1 s.
Holding Space for 1 s skips it at any time (hint text
'Hold Space to skip' appears after 3 s)
0:00 to 1:00 Prologue: rain, whispers, the bouquet, the ring, the
neon sign, two haunts, the Dealer
1:00 to 3:30 Tutorial L1 to L7
3:30 to 6:30 First rounds starting at Luck 5; the player reaches
+15 after about four good rounds and sees dream
beat 1
Cursed Table — Game Design Document
Page 47 of 62

## Source page 48
10.3 Endless mode (Stay) [SHOULD]
Starts when the player picks Stay after the ending ( ln_endingSeen = 1 ). After that
every new run is Endless. Memories can replay the beats.
The round counter and score continue from the story run, so tiers and Ts  decay carry
on. Luck is set to 0, the hint count to 1 and the Brink flag is cleared.
No dream beats. The HUD shows 'ENDLESS' in place of the dream counter.
Windfall: when Luck reaches +15 the Windfall is armed like a dream. After the round
resolves: banner 'WINDFALL', score +500 (displayed units), Luck set to 5, hints +1
(maximum 3). Defiance and Double Vision do not apply, but Defiance still gives its ×2
score until the next Windfall.
Curse escalation: cursedMult = min(2.0, 1 + 0.05 × floor(endlessRounds /
5)) . It multiplies the Luck loss from Cursed spends, Doom spends and timeouts.
Fortune Card [COULD]: each Windfall shows one card (a framed 2D panel, 3 s) from
the list below, marks it collected in ln_cardsMask , and the Memories screen shows
collected cards.
Fortune Card lines (12): 'The wheel remembers who stays.' / 'Seven is only lucky to those
who believe.' / 'Four never meant to hurt anyone.' / 'Every ball lands somewhere.' / 'Luck is
a borrowed coat.' / 'The house is only as big as your fear.' / 'Count what you have, not what
you lost.' / 'Nobody wins the whole night.' / 'The door is always unlocked.' / 'Rain ends. Even
here.' / 'A cursed number is still just a number.' / 'Sit down, or stand up. The wheel does not
care.'
10.4 Rank titles
Shown on the death screen and the end card, based on rounds won in the run. The best
rank is saved.
Time What the player experiences
6:30 to 9:00 Tier 2 puzzles; first Cursed chips in hands; the first
brush with the red zone is likely
9:00 onward Beats 2 to 6 about every 3 to 4 minutes. A full story
playthrough is about 20 minutes
Cursed Table — Game Design Document
Page 48 of 62

## Source page 49
10.5 Anti-frustration rules [MUST]
1. A safe route exists in every round and the time limit is sized to it.
2. Undo is free and unlimited.
3. In story mode no single event costs more than 4 Luck (the worst case is spending 666
and a Cursed value in one operation).
4. Death keeps story progress; only the run resets.
5. The first run after the tutorial starts at Luck 5.
6. The Dealer line after a Timeout or Death never mocks the player's feelings, only the
clock and the house.
10.6 Jam page plan [SHOULD]
Tagline: 'Reach the number. Mind the superstitions. The house is watching.'
Five screenshots, in this order: (1) the red, cursed table at Luck −12 with the mask
Grin; (2) the green, blessed table at Luck +12; (3) a dream panel inside its iris; (4) a
hand with a green lucky chip and a red cursed chip; (5) the mask Crack and the
glowing door at the ending.
First sentence of the description: 'You are drunk, heartbroken and sitting at a
roulette table that feeds on despair.' Then list the controls and the 15 numbers.
Disclose AI use on the submission page, and add a content note for heartbreak,
drinking and despair.
Rounds won in the run Title
1 to 4 Drifter
5 to 9 Regular
10 to 19 Gambler
20 to 29 High Roller
30 to 44 House Favourite
45 or more Unbreakable
Chose Leave Walked Away (special title, always shown on the
Leave end card)
Cursed Table — Game Design Document
Page 49 of 62

## Source page 50
11. Project structure, scripts and API [MUST]
One scene ( Game.unity ) holds everything: _Managers , Casino3D , Canvas_HUD ,
Canvas_Story , Canvas_Overlay , a GlobalVolumes  object and an EventSystem with the
Input System UI module. Namespace for all runtime code: HollowSeven . Build Settings
contain only Game .
11.1 Folder tree
Docs/                      DESIGN.md  OPEN_QUESTIONS.md  CHANGELOG.md   
(project root, outside Assets)
Assets/_Project/
  Scenes/Game.unity
  Scripts/                 HollowSeven.Runtime.asmdef
    Core/        GameManager GameConfig GameEvents SaveService Tween
    Puzzle/      Operation Step Level PuzzleState LevelGenerator Solver 
DifficultyTable
                 RoundController ScoreCalculator RoundTimer
    Luck/        LuckTable LuckManager
    Story/       StoryData DreamBeat PrologueController DreamController 
StoryPanel StoryLayer
                 HauntController DealerLines EndingController
    Tutorial/    TutorialLesson TutorialController CoachOverlay
    UI/          HudController LuckMeterView TargetView TimerView StatsView 
ChipView ChipTrayView
                 OperatorBarView EquationPreviewView CaptionView BannerView 
FloatingText CodexCard
                 MainMenuView PauseMenuView DeathScreenView EndCardView 
SettingsView MemoriesView CodexView
    Presentation/ CasinoDirector CameraRig WheelController DealerController 
MistController
                 FlickerLight SlotBlink
    Audio/       AudioManager MusicLayers SfxId
    Input/       InputRouter
    Editor/      HollowSeven.Editor.asmdef  PlaceholderArtGenerator 
SolverSelfTest ProjectSetup
    Tests/EditMode/  HollowSeven.Tests.asmdef  OperationTests SolverTests 
LuckTests ScoreTests
  Data/          GameConfig.asset  DifficultyTable.asset  LuckTable.asset  
DealerLines.asset
                 StoryData.asset  Tutorial/L1..L7.asset  
Story/Beat1..Beat6.asset
Cursed Table — Game Design Document
Page 50 of 62

## Source page 51
  Prefabs/       UI/ChipView  UI/OperatorButton  UI/FloatingText  
UI/BannerView  UI/CodexCard
                 Story/StoryPanel
  Resources/Story/<PanelId>/     (section 8.1)
  Art/           Placeholders/  Fonts/  Sprites/ (Mask_Calm, Mask_Grin, 
Mask_Crack, SoftCircle)
  Audio/         Music/  Sfx/  Ambience/  Voice/
  Materials/     M_*.mat   (section 7.3)
  Settings/      URP-WebGL.asset  URP-WebGL-Renderer.asset  Vol_Neutral  
Vol_Cursed  Vol_Blessed
Assembly definitions: Runtime references Unity.InputSystem, Unity.TextMeshPro,
UnityEngine.UI and the URP runtime. Editor references Runtime (Editor platform only).
Tests reference Runtime and the Unity Test Framework.
11.2 Public API (use these names exactly)
namespace HollowSeven {
  public enum GameState { Boot, MainMenu, Prologue, Tutorial, RoundIntro, 
RoundActive,
                          RoundResolve, Dream, Death, Ending }
  public enum LuckState { Cursed, Neutral, Blessed }
  public enum LuckTag   { None, Lucky, DoubleLucky, Jackpot, Cursed, Doom }
  public enum OpKind    { Add, Sub, Mul, Div }
  public enum RoundFailReason { Timeout }
  public readonly struct RoundResult { public float Time, Score, WinLuck; 
public int Streak; }
  public static class GameEvents {
    public static event System.Action<GameState, GameState> StateChanged;     
// old, new
    public static event System.Action<float, float>         LuckChanged;      
// old, new
    public static event System.Action<LuckState>            LuckStateChanged;
    public static event System.Action<int, LuckTag, float>  LuckValueSpent;   
// value, tag, delta
    public static event System.Action<Level>                RoundStarted;
    public static event System.Action<Step>                 
OperationCommitted;
    public static event System.Action                       UndoPerformed;
    public static event System.Action<RoundResult>          RoundWon;
Cursed Table — Game Design Document
Page 51 of 62

## Source page 52
    public static event System.Action<RoundFailReason>      RoundFailed;
    public static event System.Action                       DreamArmed;
    public static event System.Action<int>                  DreamStarted;     
// beat number 1..6
    public static event System.Action                       DreamEnded;
    public static event System.Action                       DefianceFired;
    public static event System.Action                       RunEnded;
    // each event has a matching static Raise method, e.g. 
GameEvents.RaiseLuckChanged(old, new)
  }
  public sealed class LuckManager : MonoBehaviour {
    public float Luck { get; }  public LuckState State { get; }
    public bool BrinkFlag { get; }  public bool DreamArmed { get; }  public 
bool DefianceActive { get; }
    public void ResetForRun(bool firstRunAfterTutorial);
    public void Add(float delta);                 // clamps, raises events, 
checks death, arms dream
    public void SetLuck(float value);             // no clamp events beyond 
normal; used by dreams
    public void Spend(int value);                 // applies LuckTable once 
per value per round
    public void BeginRound();                     // clears firedThisRound
  }
  public sealed class PuzzleState {
    public IReadOnlyList<int> Hand { get; }  public int Target { get; }
    public bool IsDeadEnd { get; }  public bool ContainsTarget { get; }
    public bool TryCommit(int indexA, int indexB, OpKind op, out Step step);
    public bool Undo();
  }
  public static class Operation {
    public static bool TryApply(int x, int y, OpKind op, out int result, out 
int left, out int right);
  }
  public static class Solver {
    public static SolveResult Solve(int[] chips, int target, int nodeBudget);
  }
  public static class LevelGenerator {
    public static Level Generate(int roundIndex, System.Random rng);   // 
section 4.3
Cursed Table — Game Design Document
Page 52 of 62

## Source page 53
  }
  public static class Tween {
    public enum Easing { Linear, EaseOutQuad, EaseInQuad, EaseInOutSine, 
EaseOutBack, EaseOutCubic, EaseInCubic }
    public static float Ease(Easing e, float t);
    public static Coroutine Run(MonoBehaviour owner, float duration, 
System.Action<float> onUpdate,
                                Easing easing = Easing.Linear, System.Action 
onDone = null, bool unscaled = true);
  }
}
11.3 Script responsibilities
Script Responsibility
GameManager Owns GameState , runs the state machine
of section 3.8, wires managers, handles run
start and end, Pause
GameConfig ScriptableObject with every tunable (11.4)
SaveService PlayerPrefs wrapper for the keys in 11.5;
calls PlayerPrefs.Save()  on writes that
matter
RoundController Starts a round from a Level , owns
PuzzleState , RoundTimer , win and
timeout detection, prefetches the next
level in a coroutine, computes
RoundResult  with ScoreCalculator
RoundTimer Counts remaining time, drains at the luck-
scaled speed, exposes Remaining ,
Elapsed , raises timeout
LuckManager Section 3.5 to 3.7 rules and events
DreamController Picks the beat(s), runs the iris, panels and
captions, then returns control
PrologueController , EndingController ,
HauntController
Section 2 sequences, driven by data
Cursed Table — Game Design Document
Page 53 of 62

## Source page 54
11.4 ScriptableObject fields
GameConfig : startLuck 0 , firstRunBonus 5 , luckMin −15 , luckMax 15 ,
blessedThreshold 10 , brinkThreshold −10 , hauntThreshold −6 , postDreamLuck 2 ,
timeoutLuck −3 , hintLuckCost 2 , winLuckBase 1 , winLuckSpeed 2 ,
timerCursedPerLuck 0.03 , timerBlessedMult 0.85 , baseTime 15 , perStepTime 10 ,
grace 2 , streakStep 0.1 , streakMax 10 , defianceLuck 2 , defianceScoreMult 2 ,
secondSightIdleSeconds 6 , windfallScore 500 , windfallLuck 5 , endlessCurseStep
0.05 , endlessCurseCap 2.0 , roundIntroSeconds 2.5 , resolveSeconds 1.6 ,
targetRollSeconds 0.8 , valueCap 9999 , genNodeBudget 150000 , liveNodeBudget
40000 .
DifficultyTable : one entry per tier with firstRound , chipsMin , chipsMax , stepsMin ,
stepsMax , targetMin , targetMax , operators , chipPool , removedFromPool , bigsMax ,
jackpotTargets , minSafeLength , basePoints , penaltyPerSecond , plus a hand-written
Script Responsibility
TutorialController Section 5, reads TutorialLesson  assets,
blocks and highlights UI
HudController Shows or hides HUD groups, subscribes to
GameEvents  and updates the views
ChipTrayView , ChipView ,
OperatorBarView
Build chips from the hand, handle clicks and
the selection table of section 3.2, forward
commits to RoundController
CasinoDirector Section 7.4: applies luck-driven values to
lights, fog, volumes, felt and mist
CameraRig Section 7.6 sway and shake
WheelController , DealerController Sections 7.7 and 7.8
AudioManager , MusicLayers Section 9
InputRouter Single place that reads the Input System
and calls UI and game actions (section 6.2)
PlaceholderArtGenerator ,
SolverSelfTest , ProjectSetup  (Editor)
Placeholder PNGs, the self test of section
4.7, and a menu command that sets Player
and quality settings from section 12.3
Cursed Table — Game Design Document
Page 54 of 62

## Source page 55
fallbackLevel .
LuckTable : list of { value, tag, delta, codexBlurb }  in the order of section 3.5 (this
order is the Codex bit index). Helpers: GetTag(int) , GetDelta(int) ,
IsCursedOrDoom(int) .
StoryData : DreamBeat[6]  with title , panelIds[3] , captions[3] , plus the ending
panels. DealerLines : one string array per situation in section 2.5.
11.5 Save keys (PlayerPrefs)
11.6 Code conventions
1. One public type per file, file name equals type name. Private fields _camelCase  with
[SerializeField] private . No public fields except plain data types.
2. Subscribe to GameEvents  in OnEnable , unsubscribe in OnDisable .
Key Type Meaning
ln_prologueSeen int 0 or 1 Prologue played once
ln_tutorialDone int 0 or 1 Tutorial completed or skipped
ln_beatsSeen int
bitmask
Bit 0 to 5 for beats 1 to 6
ln_endingSeen int 0 or 1 Ending reached (turns on
Endless runs)
ln_highScore int Best displayed score
ln_bestRounds int Most rounds won in one run
ln_codexMask int
bitmask
One bit per LuckTable row
ln_cardsMask int
bitmask
Collected Fortune Cards
ln_volMaster , ln_volMusic , ln_volSfx ,
ln_volVoice , ln_volAmb
float Bus volumes
ln_muted , ln_quality int Mute flag, quality level (0 low, 1
normal)
Cursed Table — Game Design Document
Page 55 of 62

## Source page 56
3. Cache references in Awake . Prefer events over polling in Update .
4. All timed sequences are coroutines. Use WaitForSecondsRealtime  where
Time.timeScale  may be 0.
5. No Resources.Load  outside StoryPanel  loading. No Find  calls at runtime.
6. Every serialized reference that must be set is checked once in Awake  with a single
clear Debug.LogError .
11.7 Unit tests (Unity Test Framework, EditMode)
12. Build order, acceptance tests and submission
Build strictly in milestone order. After each milestone the agent appends a report to
Docs/CHANGELOG.md  in this exact shape: Done (what was built), Tests (each acceptance
line with pass or fail), Screenshot (path), Deviations (anything not as specified, and why),
Next. A failed acceptance line blocks the next milestone.
Test Assertions
OperationTests 8 − 8 invalid. 6 ÷ 4 invalid. 12 ÷ 3 = 4. 1 × 5 invalid.
9999 + 1 invalid. 9 − 4 returns left 9, right 4
SolverTests Hand 5, 6 target 11: shortest length 1. Hand 11, 5, 2
target 27: shortest length 2 and a safe route. Hand
4, 5, 2, 6 target 16: safe route length 2 that never
spends 4
LuckTests Spending 7 twice in one round gives +1 once. After
BeginRound  it fires again. Luck −15 raises death.
Luck reaching 15 sets DreamArmed  and it stays set
after Luck drops
ScoreTests Section 4.6 example gives displayed score 520 and
winLuck 2.53 (±0.01). With t = g the winLuck is 3.
With t = T it is 1
Cursed Table — Game Design Document
Page 56 of 62

## Source page 57
12.1 Milestones and acceptance tests
Milestone Build Acceptance (all must pass)
M0 Setup Verify Unity version and URP, create
folders, asmdefs, Game.unity  with
the three canvases, EventSystem,
run ProjectSetup  (12.3) and
PlaceholderArtGenerator
Console has 0 errors and 0
warnings. Folder tree matches
11.1. CHANGELOG.md  records the
Unity version
M1 Puzzle core Operation , Step , Level ,
PuzzleState , Solver ,
LevelGenerator ,
DifficultyTable , LuckTable  data,
EditMode tests, SolverSelfTest
All tests in 11.7 pass. Self test
(4.7) passes for all 6 tiers
M2 Round loop on
grey UI
HUD per 6.4 with chips, operators,
target, timer, stats, Undo and Hint;
selection per 3.2; win, timeout, dead
end; score
In Play mode: a generated level
is solved by clicking only.
Invalid moves shake and
change nothing. Dead end
banner and Undo work. A
timeout starts the next round.
Screenshot at 1920x1080
matches 6.4 positions within 4
px
M3 Luck system LuckManager , meter view, states
and effects on the timer, tag
markers on chips, floating numbers,
Codex card, Brink, Defiance, death
and dream arming, debug keys
F2 and F3 move the meter and
colours match 3.6. Spending 7
twice in a round gives +1 once.
Luck −15 raises RunEnded .
Luck +15 arms the dream and it
stays armed after Luck drops.
Timer drain matches ×1.45 at
−15 and ×0.85 above 10
(measure over 10 s)
Cursed Table — Game Design Document
Page 57 of 62

## Source page 58
Milestone Build Acceptance (all must pass)
M4 3D casino All of section 7: objects, lights, fog,
mist, volumes, CasinoDirector ,
CameraRig , wheel, dealer
Screenshots at Luck −12, 0 and
+12 are saved to
Docs/screens/  and differ
visibly in light colour, fog and
vignette. Focal-zone test of 6.4
passes. 0 errors in the Console.
Frame time under 16 ms in the
Editor Game view at 1280x720
M5 Story and flow Boot, main menu, prologue,
DreamController  with placeholder
panels, death screen, ending, end
card
Full flow works: Boot, Play,
prologue, (F6 skips the
tutorial), a round, F4 plays beat
1, beats 1 to 6 in order, ending,
Leave end card. Death flow: F5
shows the death screen and
Space restarts in under 2 s
with beats kept. Double Vision:
F3 to −10 or below, then F2 to 0
or above fires Defiance, then
F4 plays two beats
M6 Tutorial TutorialController ,
CoachOverlay , L1 to L7
A tester completes L1 to L7
using only highlighted
elements. Skip button appears
from L3. Quitting mid-tutorial
restarts at L1.
ln_tutorialDone  is set after
L7. First run starts at Luck 5
M7 Audio AudioManager , MusicLayers , all
SFX and voice hooks with empty
slots
No errors with every slot
empty. Every SfxId  in 9.4 has
at least one call site. Bus
volumes and mute persist after
a restart. Music stem volumes
follow Luck (checked in the
Inspector)
M8 Polish and
SHOULD items
Juice from 6.7, Codex and Memories
views, Endless mode, Low quality,
look boosters, Haunts if time
remains
Each item is switchable off and
the game still passes M5 and
M6 tests
Cursed Table — Game Design Document
Page 58 of 62

## Source page 59
12.2 Schedule (IST, deadline Saturday 14 November 2026, 17:30 IST = 12:00 UTC)
The jam recommends being done by the end of the seventh day (for India, Thursday 12
November at 00:00) and using the rest as a buffer. Late submissions are not accepted.
12.3 Project settings (the ProjectSetup  menu command applies these)
Platform: WebGL. Scenes in build: Game  only.
Player: company and product HollowSeven , colour space Linear, default canvas 1280
× 720, run in background on, Active Input Handling set to Input System Package
(New).
WebGL publishing: compression Gzip with Decompression Fallback on, exceptions
None or explicitly thrown only for the release build, managed stripping Low, name
files as hashes off. If the itch.io page fails to load the build, switch compression to
Disabled and re-upload.
Graphics: the URP asset from section 7.9 assigned in Graphics and in both quality
levels ( Normal , Low ). VSync on.
Milestone Build Acceptance (all must pass)
M9 WebGL build Build, run from a local web server,
test in Chrome and Firefox
Checklist in 12.4 passes
M10 Submission itch.io page and jam submission Checklist in 12.5 passes
When Target
Tuesday 10 Nov M0 to M3
Wednesday 11 Nov M4, M5, M6
Thursday 12 Nov, 00:00 All MUST items done (jam's recommended finish)
Thursday 12 Nov M7, M8 (SHOULD items), real art and audio if
available
Friday 13 Nov M9 build and browser tests, M10 itch page and
submission
Saturday 14 Nov Fixes only. No new features. Final upload no later
than 12:00 IST to leave a margin
Cursed Table — Game Design Document
Page 59 of 62

## Source page 60
Audio: default Vorbis import per section 9.1.
Embed on itch.io: viewport 1280 × 720, fullscreen button on, mobile friendly off.
12.4 WebGL test checklist (M9)
1. Build size is under 40 MB with placeholders and the page loads in under 15 s on a
normal connection.
2. No errors in the browser console (F12) from load to a complete playthrough.
3. Audio starts after Click to Enter. Mute and volume sliders work.
4. Fullscreen works and the HUD stays inside the screen at 1366 × 768, 1920 × 1080, 1280
× 720 and 800 × 600.
5. Reload keeps the saves from 11.5.
6. At least 50 FPS at 1280 × 720 on the test machine. If not, make Low  the default and re-
test.
7. Mouse clicks and every key in 6.2 work in the browser; the page does not scroll on
Space.
8. The browser build passes the M5 and M6 flows (debug keys are not available in
release; use a Development build to test them).
12.5 Submission checklist (M10, from the jam's rules page)
Submit through the jam page on itch.io before Saturday 14 November, 17:30 IST. Late
submissions are not allowed and files must not change after the deadline or during
judging.
The game is made in Unity and is original, made during the jam.
The page has a playable build, a short description, the controls (section 6.2), a credit
list for all assets and fonts (including the default Liberation Sans font, and any asset
packs) and the team members.
Mark the WebGL build as playable in the browser. WebGL is also required for prize
eligibility. Upload as a ZIP (preferred), under 1 GB. All files needed to play are inside
the build.
Cursed Table — Game Design Document
Page 60 of 62

## Source page 61
Best Student Game needs every team member to hold a valid Unity Student Plan
licence. Check the official contest rules linked on the jam page.
Add the content note about heartbreak, drinking and despair to the description.
12.6 If time runs short: cut in this order
1. Haunts
2. Fortune Cards
3. Memories view
4. Codex view (keep the in-game Codex cards)
5. Low quality option
6. MistNear
7. Endless mode
8. Music layers (use one looping track)
9. Rim-light look boosters
Never cut: the tutorial, the luck meter and its effects on the room, the six dream beats,
the death screen, the ending, the safe-route guarantee.
12.7 Main risks
Risk Mitigation
Browser frame rate drops with post-
processing and fog
Real-time lights capped at 4, particles at 80,
Low  quality, test in the browser at M4 and
not only at M9
Solver hitch during play Generation runs during transitions; live
solver budget 40000 nodes and only on
request
Missing glyphs for × ÷ − Dynamic TMP font check in 6.6, ASCII
fallback
Audio blocked by the browser Click to Enter screen unlocks it before any
sound
The agent drifts from the design Rules in 1.1, one milestone at a time,
acceptance tests, deviations logged in
CHANGELOG.md
Cursed Table — Game Design Document
Page 61 of 62

## Source page 62
12.8 Definition of done
A new player opens the itch.io page, clicks Click to Enter, sees the prologue, finishes the
tutorial in about 2.5 minutes, reaches the first dream in about 4 more minutes, can lose
and retry in under 2 s, and can finish all six dreams and the ending in about 20 minutes,
with the room visibly and audibly reacting to Luck all the way.
Risk Mitigation
Scope growth Priority tags and the cut order above
Cursed Table — Game Design Document
Page 62 of 62