using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
namespace HollowSeven.Editor { public static class RuntimeSmokeTest {
    private static readonly List<string> Results=new List<string>();private static readonly string[] Keys={"ln_prologueSeen","ln_tutorialDone","ln_beatsSeen","ln_endingSeen","ln_highScore","ln_bestRounds","ln_codexMask","ln_cardsMask"};private static Dictionary<string,int> _saved;
    public static IEnumerator Run(){Results.Clear();_saved=new Dictionary<string,int>();foreach(var key in Keys)_saved[key]=PlayerPrefs.HasKey(key)?PlayerPrefs.GetInt(key):-1;var stack=new Stack<IEnumerator>();stack.Push(Tests());bool failed=false;while(stack.Count>0&&!failed){object current=null;bool next=false;try{next=stack.Peek().MoveNext();if(next)current=stack.Peek().Current;}catch(Exception e){Results.Add("FAIL: "+e);failed=true;}if(failed)break;if(!next){stack.Pop();continue;}if(current is IEnumerator nested){stack.Push(nested);continue;}yield return current;}foreach(var kv in _saved){if(kv.Value<0)PlayerPrefs.DeleteKey(kv.Key);else PlayerPrefs.SetInt(kv.Key,kv.Value);}PlayerPrefs.Save();File.WriteAllText("Docs/RUNTIME_TESTS.md","# Runtime checks\n\n"+string.Join("\n\n",Results));Debug.Log(failed?"Runtime QA FAILED; see Docs/RUNTIME_TESTS.md":"Runtime QA PASSED");}
    private static void Check(bool condition,string message){if(!condition)throw new Exception(message);Results.Add("PASS: "+message);File.WriteAllText("Docs/runtime-progress.txt",string.Join("\n",Results));}
    private static IEnumerator Wait(Func<bool> condition,float seconds=8){float end=Time.realtimeSinceStartup+seconds;while(!condition()){if(Time.realtimeSinceStartup>end)throw new Exception("Timeout awaiting runtime condition");yield return null;}}
    private static void Click(GameManager g,int a,OpKind op,int b){g.Hud.Chips[a].Button.onClick.Invoke();g.Hud.Operators[(int)op].onClick.Invoke();g.Hud.Chips[b].Button.onClick.Invoke();}
    private static Level Level(int[] hand,int target)=>new Level{StartChips=hand,Target=target,Operators=Solver.All,TimeLimit=60,Tier=1};
    private static IEnumerator Tests(){
        var g=UnityEngine.Object.FindAnyObjectByType<GameManager>();g.Audio.Unlock();g.StartRun(false,false);yield return Wait(()=>g.State==GameState.RoundActive);g.Round.Begin(Level(new[]{5,6,2},13),false);
        Click(g,0,OpKind.Add,1);yield return Wait(()=>!g.Round.Busy);Check(g.Round.Puzzle.Hand.Count==2&&g.Round.Puzzle.Hand[1]==11,"Mouse-button operation merges 5+6 into a result chip");g.Hud.Undo.onClick.Invoke();Check(g.Round.Puzzle.Hand.Count==3,"Undo restores original hand");
        g.Round.Begin(Level(new[]{8,8,5},21),false);Click(g,0,OpKind.Sub,1);Check(g.Round.Puzzle.Hand.Count==3&&g.Round.Selected==0,"Invalid subtraction preserves hand and selection");
        g.Round.Begin(Level(new[]{5,6},20),false);Click(g,0,OpKind.Add,1);yield return Wait(()=>!g.Round.Busy);Check(g.Round.Puzzle.IsDeadEnd,"Dead end is detected");g.Hud.Undo.onClick.Invoke();Check(!g.Round.Puzzle.IsDeadEnd,"Undo escapes dead end");
        g.Luck.ResetForRun(false);g.Round.Begin(Level(new[]{7,5,2},37),false);Click(g,0,OpKind.Mul,1);yield return Wait(()=>!g.Round.Busy);float luck=g.Luck.Luck;g.Hud.Undo.onClick.Invoke();Click(g,0,OpKind.Mul,1);yield return Wait(()=>!g.Round.Busy);Check(Mathf.Approximately(luck,g.Luck.Luck)&&luck==1,"Undo does not allow repeated luck farming");
        g.Round.Begin(Level(new[]{4,5,2,6},16),false);g.Luck.SetLuck(0);g.Hints=1;g.Hud.Hint.onClick.Invoke();Check(g.Hints==0&&g.Luck.Luck==-2,"Hint consumes one use and costs 2 Luck");
        g.Luck.SetLuck(-14);g.Hints=1;g.Hud.Hint.onClick.Invoke();Check(g.Hints==1&&g.Luck.Luck==-14,"Hint blocked when it would kill the player");
        g.Luck.ResetForRun(false);g.Luck.SetLuck(-10);g.Luck.Add(10);Check(g.Luck.DefianceActive&&g.Luck.Luck==2,"Brink recovery triggers Defiance and +2 Luck");g.Luck.SetLuck(15);g.Luck.Add(-5);Check(g.Luck.DreamArmed,"Dream remains armed after Luck falls");g.Luck.FinishDream(2);
        g.TogglePause();float remaining=g.Round.Remaining;yield return new WaitForSecondsRealtime(.2f);Check(g.Paused&&Mathf.Approximately(g.Round.Remaining,remaining),"Pause freezes the round timer");g.TogglePause();Check(!g.Paused&&Time.timeScale==1,"Resume restores time");
        g.Round.Begin(Level(new[]{5,6},11),false);Click(g,0,OpKind.Add,1);yield return Wait(()=>g.State==GameState.RoundResolve);Check(g.Score>0&&g.RoundsWon==1,"Winning via chip buttons resolves score and round");yield return Wait(()=>g.State==GameState.RoundActive);
        g.Round.Begin(new Level{StartChips=new[]{5,6},Target=11,Operators=Solver.All,TimeLimit=.15f,Tier=1});float beforeLuck=g.Luck.Luck;yield return Wait(()=>g.State==GameState.RoundResolve);Check(Mathf.Abs(g.Luck.Luck-(beforeLuck-3))<.01f,"Timeout subtracts 3 Luck");yield return Wait(()=>g.State==GameState.RoundActive);
        SaveService.Set("ln_beatsSeen",1);g.Luck.SetLuck(-15);Check(g.State==GameState.Death,"Luck -15 immediately opens death screen");g.Continue();yield return Wait(()=>g.State==GameState.RoundActive);Check(SaveService.BeatsSeen==1,"Retry retains dream progress");
        g.StartTutorial();yield return null;Check(!g.Hud.Hint.gameObject.activeSelf&&!g.Hud.Timer.gameObject.activeSelf,"First hand hides advanced controls and timer");
        Click(g,0,OpKind.Add,1);yield return Wait(()=>g.Round.Puzzle.Target==32);yield return null;
        Click(g,1,OpKind.Mul,2);yield return Wait(()=>!g.Round.Busy);yield return null;Click(g,0,OpKind.Add,1);yield return Wait(()=>g.Round.Puzzle.Target==37);yield return null;
        Click(g,0,OpKind.Mul,1);yield return Wait(()=>!g.Round.Busy);yield return null;Click(g,0,OpKind.Add,2);yield return Wait(()=>g.State==GameState.Dream);
        Check(g.Luck.Luck==15&&g.Luck.DreamArmed,"Tutorial completion reaches +15 Luck and triggers the first vision");
        float rewardEnd=Time.realtimeSinceStartup+25;
        while(GameObject.Find("AcceptLuck")==null&&Time.realtimeSinceStartup<rewardEnd){g.Continue();yield return new WaitForSeconds(1);}
        Check(GameObject.Find("AcceptLuck")!=null,"Breakup vision returns to the creepy Luck rules popup");
        Check((SaveService.BeatsSeen&1)!=0,"First breakup memory is recorded once");
        float heldLuck=g.Luck.Luck;yield return new WaitForSeconds(.5f);Check(g.State==GameState.Dream&&g.Luck.Luck==heldLuck,"Warning waits for acknowledgement; no gameplay timer runs");
        GameObject.Find("AcceptLuck").GetComponent<UnityEngine.UI.Button>().onClick.Invoke();yield return Wait(()=>g.State==GameState.RoundActive);
        Check(SaveService.TutorialDone&&g.Luck.Luck==g.Config.postDreamLuck&&!g.Luck.DreamArmed,"Acknowledging first vision resumes with +2 Luck and clears its trigger");
        Check(g.Hud.Timer.gameObject.activeSelf&&g.Hud.Hint.gameObject.activeSelf,"Full controls appear after guided hands");
        SaveService.Set("ln_beatsSeen",0);g.Luck.SetLuck(-10);g.Luck.Add(10);g.DebugDream();yield return Wait(()=>g.State==GameState.Dream);float end=Time.realtimeSinceStartup+40;while(g.State==GameState.Dream&&Time.realtimeSinceStartup<end){g.Continue();yield return new WaitForSeconds(.8f);}Check(SaveService.BeatsSeen==3,"Double Vision plays and persists two dream beats");yield return Wait(()=>g.State==GameState.RoundActive);
        SaveService.Set("ln_beatsSeen",31);g.DebugDream();yield return Wait(()=>g.State==GameState.Dream);end=Time.realtimeSinceStartup+25;while(g.State==GameState.Dream&&Time.realtimeSinceStartup<end){g.Continue();yield return new WaitForSeconds(.8f);}Check(g.State==GameState.Ending&&SaveService.EndingSeen,"Beat 6 reaches Leave / Stay ending and unlocks modes");g.StartRun(false,true);yield return Wait(()=>g.State==GameState.RoundActive);g.DebugDream();yield return Wait(()=>g.State==GameState.RoundActive&&g.RoundIndex==2);Check(g.Endless&&g.Hints==2,"Endless Windfall restores Luck and grants a hint");g.MainMenu();Check(g.State==GameState.MainMenu,"Return to menu works after full flow");
        var acting=g.Director.Dealer.GetComponent<DealerPerformance>();acting.Quiet();yield return new WaitForSeconds(.4f);
        var hand=g.Director.Dealer.Find("GloveL");Vector3 rest=hand.localPosition;g.Director.Speak("Luck rewards you. Until it gets you.",1);
        yield return new WaitForSeconds(.3f);Check(acting.Speaking&&Vector3.Distance(rest,hand.localPosition)>.005f,"Dealer speaks with a visible hand gesture");
        yield return new WaitForSeconds(1.1f);Check(!acting.Speaking&&Vector3.Distance(rest,hand.localPosition)<.002f,"Dealer gesture settles to rest after speech");
        var clips=(AudioClip[])typeof(AudioManager).GetField("_generatedSfx",System.Reflection.BindingFlags.NonPublic|System.Reflection.BindingFlags.Instance).GetValue(g.Audio);
        foreach(var sound in new[]{SfxId.SFX_LuckPulse,SfxId.SFX_LuckCount}){var clip=clips[(int)sound];var data=new float[clip.samples];clip.GetData(data,0);float peak=0;foreach(float sample in data)peak=Mathf.Max(peak,Mathf.Abs(sample));Check(peak>.005f&&peak<.3f&&Mathf.Abs(data[0])<.001f&&Mathf.Abs(data[data.Length-1])<.001f,"Luck sound is non-silent, bounded and fades cleanly: "+sound);}

    }
    private static IEnumerator TutorialSay(GameManager g){yield return null;yield return null;g.Continue();yield return null;yield return null;}
} }
