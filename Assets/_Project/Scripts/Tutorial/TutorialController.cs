using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
namespace HollowSeven { public sealed class TutorialController:MonoBehaviour {
    public GameManager Game;public bool AllowUndo{get;private set;}public bool AllowHint{get;private set;}private int[] _chips;private OpKind[] _ops;private Coroutine _routine;
    public bool AllowChip(int i)=>_chips==null||Array.IndexOf(_chips,i)>=0;public bool AllowOperator(OpKind op)=>_ops==null||Array.IndexOf(_ops,op)>=0;
    public void Begin(){Stop();_routine=StartCoroutine(ShortLessons());}
    public void Stop(){if(_routine!=null)StopCoroutine(_routine);_routine=null;if(Game.Round!=null)Game.Round.TutorialWon=null;_chips=null;_ops=null;}
    public void Timeout(){}
    private IEnumerator ShortLessons(){
        StartLesson(1,new[]{5,6},11);Game.Hud.SetTeachingStage(1);Game.Hud.Skip.gameObject.SetActive(true);Game.Hud.Lesson.text="YOUR FIRST HAND";AllowUndo=false;AllowHint=false;
        yield return Guide("Make 11.",0,OpKind.Add,1);Game.Hud.Say("That's it. Two chips become one.",1.3f);Game.Audio.PlaySfx(SfxId.SFX_Win);yield return new WaitForSeconds(1.1f);yield return Game.WaitForVoice();
        StartLesson(2,new[]{2,5,6},32);Game.Hud.SetTeachingStage(2);Game.Hud.Skip.gameObject.SetActive(true);Game.Hud.Lesson.text="BUILD ON YOUR RESULT";AllowUndo=false;
        yield return Guide("Make 32. Start with 5 × 6.",1,OpKind.Mul,2);yield return Guide("Now use the 30. Add the 2.",0,OpKind.Add,1);Game.Hud.Say("Results are chips too.",1.1f);Game.Audio.PlaySfx(SfxId.SFX_Win);yield return new WaitForSeconds(1);yield return Game.WaitForVoice();
        StartLesson(3,new[]{7,5,2,4},37);Game.Hud.SetTeachingStage(3);Game.Hud.Skip.gameObject.SetActive(true);Game.Hud.Lesson.text="MAKE YOUR OWN LUCK";AllowUndo=false;
        yield return Guide("The green 7 gives Luck. Multiply it by 5.",0,OpKind.Mul,1);yield return Guide("Add 2 to reach 37. Leave the cursed 4 alone.",0,OpKind.Add,2);
        Game.Hud.Say("You don't need every chip. Just the right ones.",1.8f);Game.Audio.PlaySfx(SfxId.SFX_Win);yield return new WaitForSeconds(1.8f);yield return Game.WaitForVoice();Game.Hud.SetTeachingStage(0);Game.TutorialFinished();
    }
    private IEnumerator Guide(string caption,int a,OpKind op,int b){_chips=new[]{a,b};_ops=new[]{op};int before=Game.Round.Commits;int last=-99;float start=Time.time;Game.Hud.Say(caption,Mathf.Max(3,Game.Audio.VoiceLength(VoiceLibrary.ForLine(caption))));
        while(Game.Round.Commits==before){if(Game.Round.Busy){yield return null;continue;}int stage=Game.Round.Selected<0?0:!Game.Round.SelectedOp.HasValue?1:2;if(stage!=last){last=stage;ClearHighlight();Game.Hud.Caption.text=caption;string symbol=new[]{"+","−","×","÷"}[(int)op];if(stage==0){Game.Hud.Equation.text="Click the "+Game.Round.Puzzle.Hand[a]+" chip";Game.Hud.Chips[a].Glow.gameObject.SetActive(true);}else if(stage==1){Game.Hud.Equation.text="Choose "+symbol;Game.Hud.Operators[(int)op].transform.localScale=Vector3.one*1.12f;}else{int other=Game.Round.Selected==a?b:a;Game.Hud.Equation.text="Click the "+Game.Round.Puzzle.Hand[other]+" chip";Game.Hud.Chips[other].Glow.gameObject.SetActive(true);}}
            if(Time.time-start>8&&stage==0){Game.Hud.Chips[a].Rect.localScale=Vector3.one*(1+.035f*Mathf.Sin(Time.time*5));}yield return null;
        }ClearHighlight();_chips=Array.Empty<int>();_ops=Array.Empty<OpKind>();yield return Game.WaitForVoice();_chips=null;_ops=null;yield return null;
    }
    private void StartLesson(int n,int[] hand,int target){AllowUndo=false;AllowHint=false;_chips=null;_ops=null;Game.Hud.Lesson.text="HAND "+n+" / 3";Game.Round.TutorialWon=()=>{};Game.Round.Begin(new Level{StartChips=hand,Target=target,Tier=1,Operators=Solver.All,TimeLimit=60},false);}
    private void ClearHighlight(){foreach(var chip in Game.Hud.Chips)chip.Glow.gameObject.SetActive(false);foreach(var op in Game.Hud.Operators)op.transform.localScale=Vector3.one;}
} }
