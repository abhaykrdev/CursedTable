using System;
using System.Collections;
using UnityEngine;
namespace HollowSeven { public sealed class RoundController:MonoBehaviour {
    public GameManager Game;public PuzzleState Puzzle{get;private set;}public Level Level{get;private set;}public int Selected{get;private set;}=-1;public OpKind? SelectedOp{get;private set;}
    public float Remaining{get;private set;}public bool Active{get;private set;}public bool Busy{get;private set;}public int Commits{get;private set;}public int Undos{get;private set;}public int HintUses{get;private set;}
    public bool CursedSpent{get;private set;}private float _idle;private bool _timerEnabled=true;public Action TutorialWon;
    public void Begin(Level level,bool timer=true){Level=level;Game.Luck.CurseMultiplier=(Game.Endless?Mathf.Min(Game.Config.endlessCurseCap,1+Game.Config.endlessCurseStep*Mathf.Floor((Game.RoundIndex-1)/5f)):1)*level.CurseMultiplier;Game.Hud.SetRoundRule(level.Rule);Puzzle=new PuzzleState(level.StartChips,level.Target,level.Operators);Remaining=level.TimeLimit;_timerEnabled=timer;Selected=-1;SelectedOp=null;Busy=false;Active=true;_idle=0;CursedSpent=false;Commits=0;Undos=0;HintUses=0;Game.Luck.BeginRound();Game.Hud.Target.SetText("{0}",level.Target);Game.Hud.ShowHand(Puzzle,true);Game.Hud.SetOperators(level.Operators);Game.Hud.Selection(-1,null);Game.Hud.RefreshStats();GameEvents.RaiseRoundStarted(level);}
    public void Stop(){Active=false;Busy=false;StopAllCoroutines();}
    public void Clear(){Selected=-1;SelectedOp=null;Game.Hud.Selection(Selected,SelectedOp);}
    public void SelectChip(int index){if(!CanInput()||index<0||index>=Puzzle.Hand.Count||!Game.AllowChip(index))return;_idle=0;if(Selected<0||!SelectedOp.HasValue){Selected=Selected==index?-1:index;Game.Hud.Selection(Selected,SelectedOp);Game.Audio.PlaySfx(SfxId.SFX_ChipSelect);return;}if(index==Selected)return;
        int a=Selected;if(!Puzzle.TryCommit(a,index,SelectedOp.Value,out var step)){Game.Hud.Shake(a);Game.Hud.Shake(index);Game.Audio.PlaySfx(SfxId.SFX_Invalid);return;}StartCoroutine(Merge(a,index,step));}
    public void SelectOperator(OpKind op){if(!CanInput()||Array.IndexOf(Level.Operators,op)<0||!Game.AllowOperator(op))return;_idle=0;if(Selected<0){Game.Hud.Say("Choose a chip first.");return;}SelectedOp=op;Game.Hud.Selection(Selected,SelectedOp);Game.Audio.PlaySfx(SfxId.SFX_OperatorSelect);}
    private bool CanInput()=>Active&&!Busy&&!Game.Paused&&(Game.State==GameState.RoundActive||Game.State==GameState.Tutorial);
    private IEnumerator Merge(int a,int b,Step step){Busy=true;var ar=Game.Hud.Chips[a].Rect;var br=Game.Hud.Chips[b].Rect;var ap=ar.anchoredPosition;var bp=br.anchoredPosition;var mid=(ap+bp)/2;Game.Audio.PlaySfx(SfxId.SFX_Merge);
        for(float t=0;t<Game.Config.mergeSeconds;t+=Time.deltaTime){float u=t/Game.Config.mergeSeconds;ar.anchoredPosition=Vector2.Lerp(ap,mid,u);br.anchoredPosition=Vector2.Lerp(bp,mid,u);ar.localScale=br.localScale=Vector3.one*Mathf.Lerp(1,.5f,u);yield return null;}
        Commits++;bool cursed=LuckTable.IsCursed(step.A)||LuckTable.IsCursed(step.B);CursedSpent|=cursed;if(cursed)Game.Streak=0;
        Game.Luck.SpendOperation(step,Puzzle.ContainsTarget);GameEvents.RaiseOperationCommitted(step);if(Game.State==GameState.Death){Busy=false;yield break;}Clear();Game.Hud.ShowHand(Puzzle,true);Busy=false;
        if(Puzzle.ContainsTarget){Active=false;if(Game.InTutorial){TutorialWon?.Invoke();yield break;}Game.ResolveRound(true);}else if(Puzzle.IsDeadEnd){Game.Hud.ShowBanner("DEAD END  /  UNDO (Z)",UIFactory.Red);Game.Audio.PlaySfx(SfxId.SFX_DeadEnd);}
    }
    public void Undo(){if(!CanInput()||!Game.AllowUndo)return;if(!Puzzle.Undo())return;Undos++;Clear();Game.Hud.ShowHand(Puzzle,true);Game.Audio.PlaySfx(SfxId.SFX_Undo);GameEvents.RaiseUndoPerformed();_idle=0;}
    public void Hint(bool free=false){if(!CanInput()||(!free&&!Game.AllowHint))return;if(!free&&!Game.InTutorial&&(Game.Hints<1||Game.Luck.Luck-Game.Config.hintLuckCost<=Game.Config.luckMin)){Game.Hud.Say("Too risky.");return;}
        var solution=Solver.Solve(Puzzle.Snapshot(),Puzzle.Target,Game.Config.liveNodeBudget,Level.Operators);
        if(!free&&!Game.InTutorial){Game.Hints--;Game.Luck.Add(-Game.Config.hintLuckCost);}HintUses++;Game.Hud.RefreshStats();_idle=0;
        if(solution.SafeShortest==null||solution.SafeShortest.Length==0){Game.Hud.ShowBanner("UNDO TO FIND A SAFE ROUTE");Tween.Run(this,.4f,t=>Game.Hud.Undo.transform.localScale=Vector3.one*(1+.08f*Mathf.Sin(t*Mathf.PI)));return;}
        Game.Hud.Highlight(solution.SafeShortest[0]);Game.Audio.PlaySfx(SfxId.SFX_Hint);
    }
    private void Update(){if(!Active||Game.Paused||Game.State!=GameState.RoundActive&&Game.State!=GameState.Tutorial)return;_idle+=Time.deltaTime;
        if(_timerEnabled){float luck=Game.Luck.Luck;float rate=luck<0?1+Game.Config.timerCursedPerLuck*-luck:luck>10?Game.Config.timerBlessedMult:1;Remaining=Mathf.Max(0,Remaining-Time.deltaTime*rate);if(Remaining<=0){Active=false;Game.ResolveRound(false);return;}}
        if(!Busy&&Game.Luck.Luck>10&&_idle>=Game.Config.secondSightIdleSeconds){Hint(true);_idle=0;}
    }
    public void DebugWin(){if(!Active)return;Active=false;Game.ResolveRound(true);}
} }
