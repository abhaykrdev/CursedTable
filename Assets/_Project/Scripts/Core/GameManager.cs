using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace HollowSeven {
public sealed class GameManager : MonoBehaviour {
    public GameConfig Config;
    public DifficultyTable Difficulty;
    public LuckTable LuckTable;
    public TMP_FontAsset Font;
    public AudioClip HeroLaughVoice;
    public AudioClip CasinoAmbience;
    public Canvas HudCanvas, StoryCanvas, OverlayCanvas;
    public GameObject Casino;
    public CasinoDirector Director;
    public LuckManager Luck { get; private set; }
    public RoundController Round { get; private set; }
    public HudController Hud { get; private set; }
    public AudioManager Audio { get; private set; }
    public StoryPanel Story { get; private set; }
    public TutorialController Tutorial { get; private set; }
    public GameState State { get; private set; } = GameState.Boot;
    public bool Paused { get; private set; }
    public bool Endless { get; private set; }
    public bool InTutorial => State == GameState.Tutorial;
    public int Score, RoundIndex = 1, Streak, Hints = 1, RoundsWon, BestStreak, ContinueCount;
    public bool AllowUndo => !InTutorial || Tutorial.AllowUndo;
    public bool AllowHint => !InTutorial || Tutorial.AllowHint;
    public bool AllowChip(int index) => !InTutorial || Tutorial.AllowChip(index);
    public bool AllowOperator(OpKind op) => !InTutorial || Tutorial.AllowOperator(op);
    private System.Random _rng;
    private Coroutine _flow;
    private RectTransform _menu;
    private bool _skipPrologue, _inSubmenu, _rewardPopup;

    private void Awake() {
        Time.timeScale = 1;
        _rng = new System.Random(Config.seed);
        UIFactory.Font = Font;
        LevelGenerator.Configure(Difficulty, Config);
        Luck = gameObject.AddComponent<LuckManager>(); Luck.Configure(Config, LuckTable);
        Round = gameObject.AddComponent<RoundController>(); Round.Game = this;
        Audio = gameObject.AddComponent<AudioManager>(); Audio.Game = this; Audio.AMB_RoomTone=CasinoAmbience;
        UIFactory.Sound = Audio.PlaySfx;
        Hud = HudCanvas.gameObject.AddComponent<HudController>(); Hud.Build(this, Font);
        Story = StoryCanvas.gameObject.AddComponent<StoryPanel>(); Story.Game = this;
        Tutorial = gameObject.AddComponent<TutorialController>(); Tutorial.Game = this;
        gameObject.AddComponent<InputRouter>().Game = this;
        if (Director != null) Director.Game = this;
    }
    private void OnEnable() { GameEvents.RunEnded += Death; GameEvents.DefianceFired += Defiance; GameEvents.LuckValueSpent += Spent; }
    private void OnDisable() { GameEvents.RunEnded -= Death; GameEvents.DefianceFired -= Defiance; GameEvents.LuckValueSpent -= Spent; Time.timeScale = 1; }
    private void Start() {
        var viewport = gameObject.AddComponent<ViewportLayout>(); viewport.Camera = Camera.main;
        viewport.Canvases = new[] { HudCanvas.GetComponent<CanvasScaler>(), StoryCanvas.GetComponent<CanvasScaler>(), OverlayCanvas.GetComponent<CanvasScaler>() };
        Luck.ResetForRun(false); Hud.Visible(false); ShowBoot();
    }
    public void ChangeState(GameState next) { var old = State; State = next; GameEvents.RaiseStateChanged(old, next); }
    private void ClearMenu() {
        foreach (Transform child in OverlayCanvas.transform) { child.gameObject.SetActive(false); Destroy(child.gameObject); }
        _menu = null;
    }
    private RectTransform Menu(string eyebrow, string title, string subtitle) {
        ClearMenu();
        var shade = UIFactory.Panel(OverlayCanvas.transform, "MenuShade", 0, 0, 1920, 1080, new Color(.015f,.012f,.025f,.88f)); shade.raycastTarget = true;
        _menu = UIFactory.Rect(OverlayCanvas.transform, "Menu", 0, 0, 1920, 1080);
        UIFactory.Shape(_menu,"OuterFrame",0,0,1800,960,ShapeGraphic.Shape.Frame,new Color(.69f,.55f,.24f,.35f),1);
        UIFactory.Text(_menu,"Eyebrow",eyebrow,0,352,1400,60,24,UIFactory.Gold);
        UIFactory.Text(_menu,"Title",title,0,250,1500,160,90,UIFactory.Bone);
        UIFactory.Text(_menu,"Subtitle",subtitle,0,130,1250,90,27,UIFactory.Brass);
        UIFactory.Panel(_menu,"Rule",0,70,520,2,UIFactory.Brass);
        return _menu;
    }
    private Button MenuButton(string name,string label,float y,Action action) {
        return UIFactory.Button(_menu,name,label,0,y,440,62,()=>action());
    }
    private void Focus(Button button) { if (EventSystem.current != null) EventSystem.current.SetSelectedGameObject(button.gameObject); }
    private void ContentNote() { UIFactory.Text(_menu,"ContentNote","This game deals with heartbreak, drinking and despair.\nIf you are struggling, please talk to someone you trust.",0,-474,1500,60,20,UIFactory.Hex("9B9292")); }
    public void ShowBoot() {
        ChangeState(GameState.Boot); Menu("THE HOUSE HAS BEEN EXPECTING YOU","CURSED TABLE","Win a hand. See what she never told you.");
        Focus(MenuButton("Enter","TAKE A SEAT",-90,Enter)); ContentNote();
    }
    public void Enter() { Audio.Unlock(); if (!SaveService.PrologueSeen) StartPrologue(); else MainMenu(); }
    public void MainMenu() {
        _rewardPopup=false; _inSubmenu=false; CancelFlow(); Tutorial.Stop(); Round.Stop(); Paused=false; Time.timeScale=1;
        Hud.Visible(false); Story.Hide(); ChangeState(GameState.MainMenu);
        Menu("THE HOUSE IS WAITING","CURSED TABLE","BEST  "+PlayerPrefs.GetInt("ln_highScore").ToString("00000")+"     /     "+SaveService.Rank(PlayerPrefs.GetInt("ln_bestRounds")));
        Focus(MenuButton("Play",SaveService.EndingSeen?"STORY REPLAY":"SIT AT THE TABLE",-10,()=>{if(SaveService.EndingSeen)SaveService.Set("ln_beatsSeen",0);StartStoryRun();}));
        float y=-100;
        if(SaveService.EndingSeen) { MenuButton("Endless","ENDLESS",y,()=>StartRun(false,true)); y-=70; }
        MenuButton("Memories","MEMORIES & CODEX",y,ShowCollection); MenuButton("Settings","SETTINGS",y-70,ShowSettings);
        MenuButton("Opening","REPLAY OPENING",y-140,StartPrologue);
        UIFactory.Button(_menu,"NewGame","NEW GAME",-650,-360,290,62,ConfirmNewGame);ContentNote();
    }
    private void ConfirmNewGame(){
        Menu("START OVER","NEW GAME?","This clears story progress, unlocked memories, Codex and records.\nAudio settings stay as they are.");
        Focus(MenuButton("CancelReset","KEEP MY PROGRESS",-50,MainMenu));
        MenuButton("ConfirmReset","RESET & START AGAIN",-145,()=>{SaveService.ResetProgress();StartPrologue();});
    }
    public void StartStoryRun() { if(!SaveService.PrologueSeen)StartPrologue(); else if(!SaveService.TutorialDone)StartTutorial(); else StartRun(false,false); }
    private void CancelFlow() { if(Audio!=null)Audio.StopVoice(); if(_flow!=null) { StopCoroutine(_flow); _flow=null; } }
    private void StartPrologue() {
        CancelFlow(); Tutorial.Stop(); Round.Stop(); Paused=false; Time.timeScale=1; _rewardPopup=false; _inSubmenu=false;
        ClearMenu(); Hud.Visible(false); ChangeState(GameState.Prologue); _skipPrologue=false; _flow=StartCoroutine(Prologue());
    }
    private IEnumerator Prologue() {
        for(int i=0;i<StoryScript.Opening.Length;i++){if(_skipPrologue)break;var shot=StoryScript.Opening[i];yield return StoryBeat(shot.Id,shot.Line,shot.Place,shot.Seconds,true,shot.Speaker,OpeningVoice(i));}
        Story.Hide();SaveService.Set("ln_prologueSeen",1);_flow=null;StartTutorial();
    }
    private IEnumerator StoryBeat(string id,string caption,string heading,float seconds,bool skippable=false,string speaker="HERO",string voice=null) {
        Story.Show(id,caption,heading,Mathf.Max(seconds,Audio.VoiceLength(voice)),speaker,voice);int before=ContinueCount;float entered=Time.time;
        while(true){
            if(skippable&&_skipPrologue){Audio.StopVoice();break;}
            if(ContinueCount>before&&Time.time-entered>.25f){before=ContinueCount;if(!Story.TextComplete)Story.RevealText();else if(!Audio.VoiceBusy)break;}
            yield return null;
        }
    }
    private static string OpeningVoice(int i){return "vo_open_"+(i+1).ToString("00")+ (i==1?"_wife":i==7?"_dealer_door":i==9||i==10?"_dealer":"_hero");}
    private static string FirstVoice(int i){return "vo_first_"+(i+1).ToString("00")+(i==1?"_wife":i==2?"_boyfriend":i==6?"_hero_laugh":i==7?"_dealer":"_hero");}
    public System.Collections.IEnumerator WaitForVoice(){while(Audio.VoiceBusy)yield return null;}
    public void SkipPrologue() { if(State==GameState.Prologue)_skipPrologue=true; }
    public void StartTutorial() {
        _rewardPopup=false; Audio.Unlock(); CancelFlow(); Round.Stop(); Paused=false; Time.timeScale=1;
        if(Director!=null)Director.ResetPresentation(); ClearMenu(); Story.Hide(); ChangeState(GameState.Tutorial);
        Hud.Visible(true); Score=0; RoundIndex=1; Hints=1; Luck.Tutorial=true; Luck.ResetForRun(false); Tutorial.Begin();
    }
    public void SkipTutorial() { if(!InTutorial)return; Tutorial.Stop(); SaveService.Set("ln_tutorialDone",1); StartRun(false,false); }
    public void TutorialFinished() { PrepareRun(false,false); ChangeState(GameState.RoundResolve); _flow=StartCoroutine(FirstReward()); }
    private IEnumerator FirstReward() {
        Hud.SetTeachingStage(3); Hud.Skip.gameObject.SetActive(false); Hud.Lesson.text="THE HOUSE KEEPS ITS PROMISE";
        Hud.Equation.text="YOUR FIRST VISION"; Hud.Say("You won. Now look at what your luck has bought.",Audio.VoiceLength("vo_dealer_first_vision")+.1f); Audio.PlayVoice("vo_dealer_first_vision");if(Director!=null)Director.Speak(Hud.Caption.text,Audio.VoiceLength("vo_dealer_first_vision"));
        Hud.LuckAnimationSeconds=1.8f; Luck.SetLuck(Config.firstRunBonus); Hud.ShowBanner("+15 LUCK",UIFactory.Green);
        yield return new WaitForSeconds(2.4f);yield return WaitForVoice(); Hud.LuckAnimationSeconds=.65f;
        ChangeState(GameState.Dream); Hud.Visible(false); Audio.PlaySfx(SfxId.SFX_DreamOpen); GameEvents.RaiseDreamStarted(1);
        for(int i=0;i<StoryScript.FirstMemory.Length;i++){var shot=StoryScript.FirstMemory[i];yield return StoryBeat(shot.Id,shot.Line,shot.Place,shot.Seconds,false,shot.Speaker,FirstVoice(i));}
        Story.Hide(); Audio.PlaySfx(SfxId.SFX_DreamClose); SaveService.Beat(1);
        Menu("THE DEALER LEANS CLOSER","LUCK REWARDS YOU.\nUNTIL IT GETS YOU.","+15 reveals a memory. −15 lets the house keep you.\nYou saw her lose him. Play to discover what happened next.");
        _menu.Find("Title").GetComponent<TextMeshProUGUI>().fontSize=64;
        Audio.PlayVoice("vo_dealer_luck_rewards");if(Director!=null)Director.Speak("Luck rewards you. Until it gets you.",Audio.VoiceLength("vo_dealer_luck_rewards"));
        _rewardPopup=true; int before=ContinueCount; Focus(MenuButton("AcceptLuck","DEAL THE NEXT HAND",-110,Continue));
        UIFactory.Text(_menu,"LuckRule","THE VISION SPENDS YOUR LUCK · RETURN WITH +"+Config.postDreamLuck.ToString("0"),0,-230,1300,60,23,UIFactory.Gold);
        yield return WaitForVoice();before=ContinueCount;yield return new WaitUntil(()=>ContinueCount>before); _rewardPopup=false; ClearMenu();
        SaveService.Set("ln_tutorialDone",1); Luck.FinishDream(Config.postDreamLuck); GameEvents.RaiseDreamEnded();
        Hud.SetTeachingStage(0); Hud.Visible(true); _flow=null; NextRound();
    }
    public void StartRun(bool bonus,bool endless) { PrepareRun(bonus,endless); NextRound(); }
    private void PrepareRun(bool bonus,bool endless) {
        _rewardPopup=false; _inSubmenu=false; if(Director!=null)Director.ResetPresentation();
        CancelFlow(); Round.Stop(); Tutorial.Stop(); ClearMenu(); Story.Hide(); Paused=false; Time.timeScale=1; Endless=endless;
        Score=0; RoundIndex=1; Streak=0; Hints=1; RoundsWon=0; BestStreak=0; Luck.Tutorial=false; Luck.CurseMultiplier=1;
        Hud.LuckAnimationSeconds=.65f; Luck.ResetForRun(bonus); Hud.SetTeachingStage(0); Hud.Lesson.text=""; Hud.Skip.gameObject.SetActive(false); Hud.Visible(true);
    }
    public void NextRound() { CancelFlow(); _flow=StartCoroutine(Intro()); }
    private IEnumerator Intro() {
        ChangeState(GameState.RoundIntro); Round.Stop(); Hud.Visible(true); Hud.RefreshStats(); Hud.Say("Place your chips.");
        if(Director!=null)Director.Spin(); Audio.PlaySfx(SfxId.SFX_WheelSpin); Level level=null; float start=Time.time;
        yield return LevelGenerator.GenerateRoutine(RoundIndex,_rng,l=>level=l);
        while(Time.time-start<Config.roundIntroSeconds||Audio.VoiceBusy)yield return null;
        if(level.Rule!=RoundRule.Standard){string key=level.Rule==RoundRule.NoAddition?"vo_dealer_rule_noadd":level.Rule==RoundRule.DoubleCurses?"vo_dealer_rule_doublecurse":"vo_dealer_rule_payout";Hud.Say(RoundRules.Label(level.Rule),Audio.VoiceLength(key)+.1f);Audio.PlayVoice(key);if(Director!=null)Director.Speak(Hud.Caption.text,Audio.VoiceLength(key));yield return WaitForVoice();}
        if(Endless)Luck.CurseMultiplier=Mathf.Min(Config.endlessCurseCap,1+Config.endlessCurseStep*Mathf.Floor((RoundIndex-1)/5f));
        ChangeState(GameState.RoundActive); Round.Begin(level); Audio.PlaySfx(SfxId.SFX_ChipDeal); _flow=null;
    }
    public void ResolveRound(bool won) { if(InTutorial){if(!won)Tutorial.Timeout();return;} if(State!=GameState.RoundActive)return; _flow=StartCoroutine(Resolve(won)); }
    private IEnumerator Resolve(bool won) {
        ChangeState(GameState.RoundResolve); Round.Stop();yield return WaitForVoice();
        if(won) {
            if(!Round.CursedSpent)Streak++; BestStreak=Mathf.Max(BestStreak,Streak); RoundsWon++;
            var result=ScoreCalculator.Calculate(Config,Difficulty.ForRound(RoundIndex),Round.Level.TimeLimit-Round.Remaining,Round.Level.TimeLimit,Streak,Luck.DefianceActive);
            float earnedLuck=result.WinLuck*Round.Level.WinLuckMultiplier;
            Score+=(int)result.Score; Luck.Add(earnedLuck); if(State==GameState.Death)yield break;
            Hud.ShowBanner(Streak>=3?"CLEAN STREAK  /  "+Streak:"THE WHEEL ALLOWS IT");
            Hud.Say("+"+result.Score.ToString("0")+" POINTS    /    +"+earnedLuck.ToString("0.0")+" LUCK");
            Audio.PlaySfx(SfxId.SFX_Win);Audio.PlayVoice("vo_dealer_wheel_allows");if(Director!=null)Director.Speak("The wheel allows it.",Audio.VoiceLength("vo_dealer_wheel_allows")); GameEvents.RaiseRoundWon(result);
        } else {
            Streak=0; Luck.Add(Config.timeoutLuck*(Luck.CurseMultiplier/Round.Level.CurseMultiplier)); if(State==GameState.Death)yield break;
            Audio.PlayVoice("vo_dealer_too_slow");if(Director!=null)Director.Speak("Too slow.",Audio.VoiceLength("vo_dealer_too_slow"));Hud.ShowBanner("TOO SLOW",UIFactory.Red); Audio.PlaySfx(SfxId.SFX_Timeout); GameEvents.RaiseRoundFailed(RoundFailReason.Timeout);
        }
        Hud.RefreshStats(); yield return new WaitForSeconds(Config.resolveSeconds);yield return WaitForVoice(); if(State==GameState.Death)yield break;
        RoundIndex++;
        if(Luck.DreamArmed) {
            if(Endless){Score+=Config.windfallScore;Hints=Mathf.Min(3,Hints+1);Luck.FinishDream(Config.windfallLuck);Hud.ShowBanner("WINDFALL  +500");}
            else {_flow=StartCoroutine(Dream());yield break;}
        }
        _flow=null; NextRound();
    }
    private IEnumerator Dream() {
        ChangeState(GameState.Dream); Hud.Visible(false); int beat=SaveService.NextBeat(); if(beat>6){ShowEnding();yield break;}
        int count=Luck.BrinkFlag?2:1; if(count==2)Hud.ShowBanner("FOUGHT BACK: DOUBLE VISION");
        for(int n=beat;n<=Mathf.Min(6,beat+count-1);n++) {
            GameEvents.RaiseDreamStarted(n); Audio.PlaySfx(SfxId.SFX_DreamOpen);
            for(int panel=0;panel<3;panel++)yield return StoryBeat("D"+n+(char)('A'+panel),StoryPanel.DreamCaption(n,panel),"SECOND SIGHT  /  "+n+" OF 6",Config.panelSeconds,false,"HERO","vo_mem"+n+"_"+(char)('a'+panel)+"_hero");
            SaveService.Beat(n); Audio.PlaySfx(SfxId.SFX_DreamClose);
        }
        Story.Hide(); Luck.FinishDream(Config.postDreamLuck); GameEvents.RaiseDreamEnded();
        if(SaveService.NextBeat()>6){ShowEnding();yield break;} _flow=null; Hud.Visible(true); NextRound();
    }
    public void ShowEnding() {CancelFlow();_flow=StartCoroutine(EndingDialogue());}
    private IEnumerator EndingDialogue(){
        ChangeState(GameState.Ending);Hud.Visible(false);
        yield return StoryBeat("P8","You are not what I feed on any more.","THE DEALER",6,false,"THE DEALER","vo_end_01_dealer");
        yield return StoryBeat("P8","Some things cannot be won back. You can still walk away.","THE DOOR IS OPEN",8,false,"THE DEALER","vo_end_02_dealer");
        EndingChoices();_flow=null;
    }
    private void EndingChoices() {
        ChangeState(GameState.Ending); SaveService.Set("ln_endingSeen",1); Story.Hide(); Hud.Visible(false); if(Director!=null)Director.RevealEnding();
        Menu("YOU ARE NOT WHAT I FEED ON ANY MORE","THE DOOR IS OPEN","Some things cannot be won back. You can still walk away.");
        Focus(MenuButton("Leave","LEAVE",-45,()=>{_flow=StartCoroutine(Leave());}));
        MenuButton("Stay","STAY  /  ENDLESS",-145,()=>{ClearMenu();Endless=true;Hints=1;Luck.FinishDream(0);Hud.Visible(true);NextRound();});
    }
    private IEnumerator Leave() {
        ClearMenu();yield return StoryBeat("E2","This does not hurt less. But the rain has stopped.","MORNING",6,false,"HERO","vo_end_03_hero"); Story.Hide(); SaveService.Record(Score,RoundsWon);
        Menu("WALKED AWAY","THE END","SCORE  "+Score+"     /     ROUNDS  "+RoundsWon);
        Focus(MenuButton("Again","PLAY AGAIN",-60,()=>{SaveService.Set("ln_beatsSeen",0);StartRun(false,false);})); MenuButton("Menu","MAIN MENU",-160,MainMenu);
    }
    private void Death() {
        if(InTutorial||State==GameState.Death)return; CancelFlow(); Round.Stop(); ChangeState(GameState.Death); Paused=false; Time.timeScale=1; Hud.Visible(false);
        SaveService.Record(Score,RoundsWon); Audio.PlaySfx(SfxId.SFX_Death);
        Menu("THE HOUSE ALWAYS COLLECTS",SaveService.Rank(RoundsWon),"SCORE  "+Score+"    /    ROUNDS  "+RoundsWon+"    /    DREAMS  "+SaveService.BeatCount()+" OF 6");
        Audio.PlayVoice("vo_end_death_dealer");if(Director!=null)Director.Speak("The house always collects.",Audio.VoiceLength("vo_end_death_dealer"));Focus(MenuButton("Retry","SIT DOWN AGAIN  [SPACE]",-60,()=>{if(!Audio.VoiceBusy)StartRun(false,Endless);})); MenuButton("Menu","MAIN MENU",-160,MainMenu);
    }
    private void Defiance(){if(Hud!=null)Hud.ShowBanner("YOU REFUSED  /  ×2");}
    private void Spent(int value,LuckTag tag,float delta) {
        if(Hud==null)return; int index=LuckTable.Index(value),mask=PlayerPrefs.GetInt("ln_codexMask");
        if(index>=0&&(mask&(1<<index))==0){SaveService.Set("ln_codexMask",mask|(1<<index));if(!InTutorial)StartCoroutine(CodexCard(value,LuckTable.entries[index].codexBlurb));}
        if(delta<0&&Director!=null)Director.Shake();
    }
    private IEnumerator CodexCard(int value,string text){Hud.Codex.text="<color=#E8C26A>"+value+"</color>  "+text;yield return new WaitForSeconds(3);Hud.Codex.text="";}
    public void TogglePause() {
        if(_rewardPopup||State==GameState.Boot||State==GameState.MainMenu||State==GameState.Death||State==GameState.Ending)return;
        if(Paused){Paused=false;Time.timeScale=1;ClearMenu();return;}
        Paused=true;Time.timeScale=0;Menu("THE WHEEL CAN WAIT","PAUSED","");Focus(MenuButton("Resume","RESUME",-30,TogglePause));MenuButton("Settings","SETTINGS",-130,ShowSettings);MenuButton("Menu","MAIN MENU",-230,MainMenu);
    }
    public void Continue(){if(Paused||_inSubmenu)return;ContinueCount++;if(State==GameState.Death&&!Audio.VoiceBusy)StartRun(false,Endless);}
    public void ShowSettings() {
        _inSubmenu=true;Menu("MAKE YOURSELF COMFORTABLE","SETTINGS","Music includes casino ambience. Sound Effects includes dialogue.");
        SettingsSlider("MASTER",10,Audio.Master,Audio.SetMaster);SettingsSlider("MUSIC",-80,Audio.Music,Audio.SetMusic);SettingsSlider("SOUND EFFECTS",-170,Audio.Sfx,Audio.SetSfx);
        MenuButton("Mute",Audio.Muted?"UNMUTE [M]":"MUTE [M]",-280,()=>{Audio.ToggleMute();ShowSettings();});MenuButton("Back","BACK",-380,BackFromSubmenu);
    }
    private void SettingsSlider(string label,float y,float value,Action<float> changed) {
        UIFactory.Text(_menu,label,label,-280,y,260,50,24,UIFactory.Bone);
        var hit=UIFactory.Panel(_menu,label+"Slider",160,y,420,48,Color.clear);hit.raycastTarget=true;
        var r=hit.rectTransform;var slider=r.gameObject.AddComponent<Slider>();
        UIFactory.Panel(r,"Track",0,0,420,4,UIFactory.Brass);
        var fillArea=UIFactory.Rect(r,"FillArea",0,0,420,4);
        var fill=UIFactory.Panel(fillArea,"Fill",0,0,0,0,UIFactory.Gold);slider.fillRect=fill.rectTransform;
        var handle=UIFactory.Shape(r,"Handle",0,0,16,26,ShapeGraphic.Shape.Diamond,UIFactory.Gold);handle.raycastTarget=true;
        slider.handleRect=handle.rectTransform;slider.targetGraphic=handle;slider.minValue=0;slider.maxValue=1;slider.value=value;
        var percentage=UIFactory.Text(_menu,label+"Value",Mathf.RoundToInt(value*100)+"%",440,y,110,48,24,UIFactory.Bone);
        slider.onValueChanged.AddListener(v=>{changed(v);percentage.text=Mathf.RoundToInt(v*100)+"%";});
    }
    private void BackFromSubmenu(){_inSubmenu=false;if(Paused){Paused=false;Time.timeScale=1;TogglePause();}else MainMenu();}
    private void ShowCollection() {
        _inSubmenu=true;Menu("WHAT THE HOUSE REMEMBERS","MEMORIES & CODEX","Dreams seen: "+SaveService.BeatCount()+" / 6");int mask=PlayerPrefs.GetInt("ln_codexMask");string content="";
        for(int i=0;i<LuckTable.entries.Length;i++)if((mask&(1<<i))!=0)content+=LuckTable.entries[i].value+"  —  "+LuckTable.entries[i].codexBlurb+"\n";
        if(content=="")content="Spend a marked number to discover its story.";UIFactory.Text(_menu,"Collection",content,0,-145,1300,360,25,UIFactory.Bone);MenuButton("Back","BACK",-400,BackFromSubmenu);
    }
    public void DebugDream(){if(InTutorial)SkipTutorial();Luck.SetLuck(15);if(State==GameState.RoundActive)Round.DebugWin();}
}
}
