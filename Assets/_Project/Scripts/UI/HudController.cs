using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
namespace HollowSeven { public sealed class HudController:MonoBehaviour {
    public RectTransform Root;public TMP_FontAsset Font;public GameManager Game;
    public TextMeshProUGUI Target,Timer,Stats,LuckLabel,Equation,Caption,Banner,Lesson,HintLabel,Codex;
    public ShapeGraphic TimerRing;public Image LuckNegative,LuckPositive;public RectTransform LuckPointer;public ChipView[] Chips;public Button[] Operators;public Button Undo,Hint,Pause,Skip;
    public CanvasGroup Group;private Coroutine _banner,_caption;private float _shownLuck;private int _lastSecond=-1;
    public float LuckAnimationSeconds=.65f;
    private float _luckFrom,_luckTarget,_luckElapsed=10,_tickAt,_pulse;private bool _luckAudible;
    private TextMeshProUGUI _luckDelta,_nextMemory,_roundRule,_fog;private RectTransform _meter;private ShapeGraphic _meterHalo;
    private void OnEnable(){GameEvents.LuckChanged+=OnLuckChanged;}
    private void OnDisable(){GameEvents.LuckChanged-=OnLuckChanged;}
    private void OnLuckChanged(float oldValue,float newValue){
        _luckFrom=_shownLuck;_luckTarget=newValue;_luckElapsed=0;_tickAt=0;
        _luckAudible=Game!=null&&Group!=null&&Group.alpha>.5f&&!Game.Paused&&Mathf.Abs(newValue-oldValue)>.01f&&
            (Game.State==GameState.Tutorial||Game.State==GameState.RoundActive||Game.State==GameState.RoundResolve);
        if(!_luckAudible)return;
        _pulse=1;Game.Audio.PlaySfx(SfxId.SFX_LuckPulse);
        if(_luckDelta!=null){float delta=newValue-oldValue;_luckDelta.text=(delta>0?"+":"")+delta.ToString("0.#")+" LUCK";_luckDelta.color=delta>=0?UIFactory.Green:UIFactory.Red;}
    }

    public void Build(GameManager game,TMP_FontAsset font){Game=game;Font=font;UIFactory.Font=font;Root=(RectTransform)transform;Group=gameObject.AddComponent<CanvasGroup>();
        // Pixel layout at 1920x1080, a letterboxed camera keeps the design consistent at 4:3.
        var meter=UIFactory.Panel(Root,"LuckMeter",0,488,960,48,new Color(.02f,.015f,.025f,.9f));UIFactory.Shape(meter.transform,"Border",0,0,960,48,ShapeGraphic.Shape.Frame,UIFactory.Brass,2);
        _meter=meter.rectTransform;
        _meterHalo=UIFactory.Shape(meter.transform,"RewardHalo",0,0,980,66,ShapeGraphic.Shape.Frame,new Color(.3f,.8f,.6f,0),3);
        _luckDelta=UIFactory.Text(Root,"LuckDelta","",650,486,210,46,28,UIFactory.Green);
        _nextMemory=UIFactory.Text(Root,"NextMemory","+15 REVEALS A MEMORY",0,397,850,32,19,UIFactory.Gold);
        LuckNegative=UIFactory.Panel(meter.transform,"FillNeg",-240,0,480,40,UIFactory.Red);LuckNegative.rectTransform.pivot=new Vector2(1,.5f);LuckNegative.rectTransform.anchoredPosition=Vector2.zero;
        LuckPositive=UIFactory.Panel(meter.transform,"FillPos",240,0,480,40,UIFactory.Bone);LuckPositive.rectTransform.pivot=new Vector2(0,.5f);LuckPositive.rectTransform.anchoredPosition=Vector2.zero;
        for(int i=-10;i<=10;i+=10)UIFactory.Panel(meter.transform,"Tick_"+i,i*32,0,2,48,UIFactory.Brass);
        LuckPointer=UIFactory.Shape(meter.transform,"Pointer",0,31,12,12,ShapeGraphic.Shape.Diamond,UIFactory.Gold).rectTransform;
        UIFactory.Text(meter.transform,"Min","−15",-526,0,80,40,24,UIFactory.Red);UIFactory.Text(meter.transform,"Max","+15",526,0,80,40,24,UIFactory.Green);
        LuckLabel=UIFactory.Text(Root,"LuckLabel","LUCK  0.0   /   NEUTRAL",0,436,600,44,28,UIFactory.Bone);
        var target=UIFactory.Panel(Root,"TargetPanel",-530,265,400,154,new Color(.016f,.014f,.022f,1f));UIFactory.Shape(target.transform,"Border",0,0,400,154,ShapeGraphic.Shape.Frame,UIFactory.Brass,2);UIFactory.Text(target.transform,"Heading","M A K E",0,56,390,40,22,UIFactory.Gold);Target=UIFactory.Text(target.transform,"Target","—",0,-16,400,118,100,UIFactory.Bone);Target.fontStyle=FontStyles.Bold;
        TimerRing=UIFactory.Shape(Root,"TimerRing",820,400,164,164,ShapeGraphic.Shape.Ring,UIFactory.Gold,5);Timer=UIFactory.Text(Root,"Timer","35",820,405,160,85,60,UIFactory.Bone);UIFactory.Text(Root,"TimerLabel","S E C O N D S",820,354,180,30,17,UIFactory.Brass);
        Stats=UIFactory.Text(Root,"StatsPanel","SCORE  00000\nROUND  01\nDREAM  0/6",-710,425,420,150,30,UIFactory.Bone,TextAlignmentOptions.TopLeft);
        Pause=UIFactory.Button(Root,"PauseButton","II",-880,290,80,80,()=>Game.TogglePause());
        Lesson=UIFactory.Text(Root,"TutorialLabel","",0,330,600,40,26,UIFactory.Gold,TextAlignmentOptions.Center);
        Skip=UIFactory.Button(Root,"SkipTutorialButton","SKIP",-850,176,140,48,()=>Game.SkipTutorial());Skip.gameObject.SetActive(false);
        Undo=UIFactory.Button(Root,"UndoButton","UNDO\n<size=18>Z / BACKSPACE</size>",-835,55,150,130,()=>Game.Round.Undo());
        Hint=UIFactory.Button(Root,"HintButton","HINT  1\n<size=18>H  /  −2 LUCK</size>",-835,-115,150,130,()=>Game.Round.Hint());HintLabel=Hint.GetComponentInChildren<TextMeshProUGUI>();
        Operators=new Button[4];string[] labels={"+","−","×","÷"};string[] keys={"Q","W","E","R"};
        for(int i=0;i<4;i++){int index=i;Operators[i]=UIFactory.Button(Root,"Op"+Solver.All[i],labels[i]+"<size=20>  "+keys[i]+"</size>",835,190-i*140,150,120,()=>Game.Round.SelectOperator((OpKind)index));Operators[i].GetComponentInChildren<TextMeshProUGUI>().fontSize=62;}
        Equation=UIFactory.Text(Root,"EquationPreview","Choose a chip, an operator, then another chip.",0,-240,1000,80,36,UIFactory.Bone);
        Caption=UIFactory.Text(Root,"CaptionBox","",0,-145,1100,90,30,UIFactory.Gold);
        Banner=UIFactory.Text(Root,"BannerSlot","",0,120,1000,140,62,UIFactory.Gold);Banner.fontStyle=FontStyles.Bold;
        _roundRule=UIFactory.Text(Root,"RoundRule","",-530,150,390,72,24,UIFactory.Gold);
        _fog=UIFactory.Text(Root,"FogExplanation","",-530,65,390,65,23,UIFactory.Red);
        Codex=UIFactory.Text(Root,"CodexCard","",-530,-25,390,90,23,UIFactory.Bone);
        var tray=UIFactory.Rect(Root,"ChipTray",0,-400,1500,200);Chips=new ChipView[6];
        for(int i=0;i<6;i++){int index=i;var disc=UIFactory.Shape(tray,"Chip"+(i+1),0,0,180,180,ShapeGraphic.Shape.Disc,UIFactory.Hex("17131B"));disc.raycastTarget=true;var chip=disc.gameObject.AddComponent<ChipView>();chip.Rect=disc.rectTransform;chip.Index=i;chip.Hud=this;chip.Button=disc.gameObject.AddComponent<Button>();chip.Button.targetGraphic=disc;chip.Button.onClick.AddListener(()=>Game.Round.SelectChip(index));chip.Ring=UIFactory.Shape(disc.transform,"Ring",0,0,180,180,ShapeGraphic.Shape.Ring,UIFactory.Brass,5);UIFactory.Shape(disc.transform,"InnerRing",0,0,156,156,ShapeGraphic.Shape.Ring,new Color(.69f,.55f,.24f,.3f),1);for(int mark=0;mark<8;mark++){float angle=mark*Mathf.PI/4;var inlay=UIFactory.Panel(disc.transform,"Inlay_"+mark,Mathf.Cos(angle)*81,Mathf.Sin(angle)*81,6,15,UIFactory.Hex("B8A27B"));inlay.rectTransform.localRotation=Quaternion.Euler(0,0,mark*45-90);}chip.Glow=UIFactory.Shape(disc.transform,"Selection",0,0,194,194,ShapeGraphic.Shape.Ring,UIFactory.Bone,3);chip.Glow.gameObject.SetActive(false);chip.Number=UIFactory.Text(disc.transform,"Value","7",0,4,150,100,68,UIFactory.Bone);chip.Number.fontStyle=FontStyles.Bold;chip.Number.enableAutoSizing=true;chip.Number.fontSizeMin=36;chip.Number.fontSizeMax=68;UIFactory.Text(disc.transform,"Tag","",58,62,90,35,26,UIFactory.Green);UIFactory.Text(disc.transform,"Key",(i+1).ToString(),0,-62,60,30,18,UIFactory.Brass);Chips[i]=chip;}
        UIFactory.Text(Root,"Footer","T H E   H O L L O W   S E V E N     /     E V E R Y   N U M B E R   H A S   A   P R I C E",0,-522,1500,24,15,UIFactory.Brass);
    }
    public void ShowHand(PuzzleState state,bool animate=false){int n=state.Hand.Count;for(int i=0;i<6;i++){var chip=Chips[i];chip.gameObject.SetActive(i<n);if(i>=n)continue;chip.Rect.anchoredPosition=new Vector2((i-(n-1)/2f)*204,0);chip.Bind(state.Hand[i],Game.LuckTable);chip.Select(false);if(animate){chip.Rect.localScale=Vector3.zero;Tween.Run(this,Game.Config.dealSeconds,t=>chip.Rect.localScale=Vector3.one*t,Tween.Easing.EaseOutBack);}}}
    public void SetTeachingStage(int stage){bool all=stage==0;Stats.gameObject.SetActive(all);_roundRule.gameObject.SetActive(all);_fog.gameObject.SetActive(all);Timer.gameObject.SetActive(all);TimerRing.gameObject.SetActive(all);transform.Find("TimerLabel").gameObject.SetActive(all);transform.Find("LuckMeter").gameObject.SetActive(all||stage>=3);LuckLabel.gameObject.SetActive(all||stage>=3);_nextMemory.gameObject.SetActive(all||stage>=3);_luckDelta.gameObject.SetActive(all||stage>=3);Undo.gameObject.SetActive(all);Hint.gameObject.SetActive(all);for(int i=0;i<4;i++)Operators[i].gameObject.SetActive(all||i==0||stage>=2&&i==2);if(all)Lesson.text="";}
    public void Selection(int a,OpKind? op){for(int i=0;i<6;i++)Chips[i].Select(i==a);for(int i=0;i<4;i++)Operators[i].GetComponent<Image>().color=op.HasValue&&(int)op.Value==i?UIFactory.Hex("665135"):UIFactory.Hex("1E1218");Hover(-1);}
    public void Hover(int b){var r=Game.Round;if(r.Busy)return;if(r.Puzzle==null||r.Selected<0){Equation.text="Choose a chip, an operator, then another chip.";return;}int a=r.Puzzle.Hand[r.Selected];if(!r.SelectedOp.HasValue){Equation.text=a.ToString();return;}string sign=new[]{"+","−","×","÷"}[(int)r.SelectedOp.Value];if(b<0||b==r.Selected||b>=r.Puzzle.Hand.Count){Equation.text=a+" "+sign;return;}bool valid=Operation.TryApply(a,r.Puzzle.Hand[b],r.SelectedOp.Value,out int result,out int left,out int right);Equation.text=left+" "+sign+" "+right+" = "+(valid?(Game.Luck.Luck<=-10?"?":result.ToString()):"×");Equation.color=valid?UIFactory.Bone:UIFactory.Red;}
    public void RefreshStats(){Stats.text="<size=22><color=#B08D3C>S C O R E</color></size>  "+Game.Score.ToString("00000")+"\n<size=25>ROUND  "+Game.RoundIndex.ToString("00")+"\n"+(Game.Endless?"ENDLESS":"DREAM  "+SaveService.BeatCount()+"/6")+"</size>";HintLabel.text="HINT  "+(Game.InTutorial?"∞":Game.Hints.ToString())+"\n<size=18>H  /  −2 LUCK</size>";Hint.interactable=Game.InTutorial||Game.Hints>0;}
    public void SetRoundRule(RoundRule rule){if(_roundRule!=null)_roundRule.text=RoundRules.Label(rule);}
    public void SetOperators(OpKind[] ops){for(int i=0;i<4;i++)Operators[i].interactable=Array.IndexOf(ops,(OpKind)i)>=0;}
    public void ShowBanner(string text,Color? colour=null){if(_banner!=null)StopCoroutine(_banner);_banner=StartCoroutine(BannerRoutine(text,colour??UIFactory.Gold));}
    private IEnumerator BannerRoutine(string text,Color colour){Banner.text=text;Banner.color=colour;Banner.transform.localScale=Vector3.one*1.2f;Tween.Run(this,.25f,t=>Banner.transform.localScale=Vector3.one*Mathf.Lerp(1.2f,1,t),Tween.Easing.EaseOutBack);yield return new WaitForSeconds(1.8f);Banner.text="";}
    public void Say(string text,float seconds=3){string key=VoiceLibrary.ForLine(text);if(key!=null){if(Game.Audio.VoiceBusy)return;if(Game.Audio.PlayVoice(key,true))seconds=Mathf.Max(seconds,Game.Audio.VoiceLength(key)+.1f);}if(Game.Director!=null)Game.Director.Speak(text,seconds);if(_caption!=null)StopCoroutine(_caption);_caption=StartCoroutine(SayRoutine(text,seconds));}
    private IEnumerator SayRoutine(string text,float seconds){Caption.text=text;yield return new WaitForSeconds(seconds);Caption.text="";}
    public void Shake(int index){if(index<0||index>=Chips.Length)return;var r=Chips[index].Rect;var origin=r.anchoredPosition;Tween.Run(this,.2f,t=>r.anchoredPosition=origin+Vector2.right*(Mathf.Sin(t*Mathf.PI*8)*10*(1-t)),onDone:()=>r.anchoredPosition=origin);}
    public void Highlight(Step step){var p=Game.Round.Puzzle;int a=-1,b=-1;for(int i=0;i<p.Hand.Count;i++){if(a<0&&p.Hand[i]==step.A)a=i;else if(b<0&&p.Hand[i]==step.B)b=i;}StartCoroutine(HintGlow(a,b,(int)step.Op));}
    private IEnumerator HintGlow(int a,int b,int op){float end=Time.time+Game.Config.hintSeconds;while(Time.time<end){float s=1+.06f*Mathf.Sin(Time.time*7);if(a>=0)Chips[a].Rect.localScale=Vector3.one*s;if(b>=0)Chips[b].Rect.localScale=Vector3.one*s;Operators[op].transform.localScale=Vector3.one*s;yield return null;}Operators[op].transform.localScale=Vector3.one;Selection(Game.Round.Selected,Game.Round.SelectedOp);}
    public void Visible(bool show){Group.alpha=show?1:0;Group.interactable=show;Group.blocksRaycasts=show;}
    private void Update(){if(Game==null||Game.Luck==null)return;float luck=Game.Luck.Luck;if(_fog!=null)_fog.text=luck<=-10?"CURSED FOG: results are hidden.\nYour arithmetic still works.":"";
        if(!Game.Paused){
            _luckElapsed+=Time.deltaTime;float u=Mathf.Clamp01(_luckElapsed/Mathf.Max(.1f,LuckAnimationSeconds));
            _shownLuck=Mathf.Lerp(_luckFrom,_luckTarget,1-Mathf.Pow(1-u,2));
            if(_luckAudible&&u<1&&Group.alpha>.5f&&_luckElapsed>=_tickAt){Game.Audio.PlaySfx(SfxId.SFX_LuckCount);_tickAt=_luckElapsed+.11f;}
            _pulse=Mathf.Max(0,_pulse-Time.deltaTime*1.5f);
        }
        _meter.localScale=Vector3.one*(1+.025f*_pulse*Mathf.Sin(_pulse*Mathf.PI));
        _meterHalo.color=new Color(.4f,.85f,.65f,_pulse*.65f);_meterHalo.rectTransform.localScale=Vector3.one*(1+(1-_pulse)*.08f);
        var dc=_luckDelta.color;dc.a=_pulse;_luckDelta.color=dc;
        _nextMemory.text=Game.InTutorial?"FILL THE METER. SEE WHAT SHE NEVER TOLD YOU.":Game.Endless?"+15: WINDFALL  /  −15: THE HOUSE COLLECTS":luck>=15?"MEMORY UNLOCKED":"NEXT MEMORY  "+(SaveService.BeatCount()+1)+"/6  ·  "+Mathf.Max(0,15-luck).ToString("0.#")+" LUCK TO GO";
        LuckNegative.rectTransform.sizeDelta=new Vector2(Mathf.Max(0,-_shownLuck)*32,40);LuckPositive.rectTransform.sizeDelta=new Vector2(Mathf.Max(0,_shownLuck)*32,40);LuckPositive.color=luck>10?UIFactory.Green:UIFactory.Bone;LuckPointer.anchoredPosition=new Vector2(_shownLuck*32,31);LuckLabel.SetText("LUCK  {0:1}",_shownLuck);LuckLabel.color=luck<0?UIFactory.Red:luck>10?UIFactory.Green:UIFactory.Bone;
        if(Game.Round.Level!=null){float remaining=Game.Round.Remaining;int seconds=Mathf.CeilToInt(remaining);if(seconds!=_lastSecond){_lastSecond=seconds;Timer.SetText("{0}",seconds);}TimerRing.Fill=Mathf.Clamp01(remaining/Game.Round.Level.TimeLimit);TimerRing.color=TimerRing.Fill<.3f?UIFactory.Red:UIFactory.Gold;}
    }
} }

