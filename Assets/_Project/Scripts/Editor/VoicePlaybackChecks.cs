using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEngine;
namespace HollowSeven.Editor {
public static class VoicePlaybackChecks {
    static readonly List<string> Results=new List<string>();
    static GameManager g;
    static void Check(bool condition,string label){if(!condition)throw new Exception(label);Results.Add("PASS: "+label);File.WriteAllText("Docs/voice-progress.txt",string.Join("\n",Results));}
    public static IEnumerator Run(bool transitionsOnly=false){
        Results.Clear();g=UnityEngine.Object.FindFirstObjectByType<GameManager>();
        var saved=new Dictionary<string,int>();foreach(var key in new[]{"ln_prologueSeen","ln_tutorialDone","ln_beatsSeen","ln_endingSeen","ln_highScore","ln_bestRounds","ln_codexMask","ln_cardsMask","ln_muted"})saved[key]=PlayerPrefs.HasKey(key)?PlayerPrefs.GetInt(key):-1;
        var stack=new Stack<IEnumerator>();stack.Push(transitionsOnly?Transitions():Tests());bool failed=false;
        while(stack.Count>0&&!failed){object current=null;bool next=false;try{next=stack.Peek().MoveNext();if(next)current=stack.Peek().Current;}catch(Exception e){Results.Add("FAIL: "+e);failed=true;}if(failed)break;if(!next){stack.Pop();continue;}if(current is IEnumerator nested){stack.Push(nested);continue;}yield return current;}
        g.MainMenu();foreach(var kv in saved){if(kv.Value<0)PlayerPrefs.DeleteKey(kv.Key);else PlayerPrefs.SetInt(kv.Key,kv.Value);}PlayerPrefs.Save();
        File.WriteAllText(transitionsOnly?"Docs/VOICE_TRANSITIONS.md":"Docs/VOICE_VALIDATION.md","# Voice playback validation\n\n"+string.Join("\n\n",Results));Debug.Log(failed?"Voice QA FAILED":"Voice QA PASSED");
    }
    static IEnumerator Wait(Func<bool> predicate,float limit=20){float end=Time.realtimeSinceStartup+limit;while(!predicate()){if(Time.realtimeSinceStartup>end)throw new Exception("Voice test timeout");yield return null;}}
    static IEnumerator Shot(string key,string id,string line,string speaker="HERO"){
        g.ChangeState(GameState.Dream);
        var beat=(IEnumerator)typeof(GameManager).GetMethod("StoryBeat",BindingFlags.Instance|BindingFlags.NonPublic).Invoke(g,new object[]{id,line,"VOICE QA",8f,false,speaker,key});
        int starts=g.Audio.VoiceStarts;Check(beat.MoveNext()&&g.Audio.VoiceBusy,"Starts "+key);
        float begin=Time.realtimeSinceStartup;int token=g.ContinueCount;
        while(g.Audio.VoiceBusy){g.Continue();CheckStep(beat.MoveNext(),"Scene advanced during "+key);CheckStep(g.Audio.VoiceStarts==starts+1,"Repeated voice from click: "+key);if(Time.realtimeSinceStartup-begin>25)throw new Exception("Clip never finished: "+key);yield return null;}
        Check(Time.realtimeSinceStartup-begin>=g.Audio.VoiceLength(key)-.15f,"Full recording finishes before scene exit: "+key);
        g.Continue();yield return null;Check(!beat.MoveNext(),"Fresh click advances completed line: "+key);g.Story.Hide();
    }
    static void CheckStep(bool condition,string label){if(!condition)throw new Exception(label);}
    static void Commit(int a,OpKind op,int b){g.Round.SelectChip(a);g.Round.SelectOperator(op);g.Round.SelectChip(b);}
    static IEnumerator Transitions(){
        g.MainMenu();g.Audio.Unlock();g.StartTutorial();yield return null;g.SkipTutorial();yield return null;Check(g.Audio.VoiceClip==null||g.Audio.VoiceClip.name!="vo_dealer_tut_01_make11"||!g.Audio.VoiceBusy,"Explicit tutorial skip cancels instruction");g.MainMenu();
        g.ChangeState(GameState.Dream);g.Audio.PlayVoice("vo_open_01_hero");yield return new WaitForSeconds(.25f);bool muted=g.Audio.Muted;g.Audio.ToggleMute();float position=g.Audio.VoiceTime;yield return new WaitForSeconds(.25f);Check(g.Audio.VoiceBusy&&g.Audio.VoiceTime>position,"Mute does not finish or restart dialogue");if(g.Audio.Muted!=muted)g.Audio.ToggleMute();g.MainMenu();
        g.TutorialFinished();yield return Wait(()=>g.State==GameState.Dream);float end=Time.realtimeSinceStartup+110;
        while(GameObject.Find("AcceptLuck")==null){g.Continue();if(Time.realtimeSinceStartup>end)throw new Exception("First reward flow timeout");yield return null;}
        Check(g.Audio.VoiceBusy&&g.Audio.VoiceClip.name=="vo_dealer_luck_rewards","First reward reaches voiced warning after every memory line");
        while(g.Audio.VoiceBusy){g.Continue();CheckStep(g.State==GameState.Dream,"Warning closed during speech");yield return null;}
        yield return null;yield return null;Check(GameObject.Find("AcceptLuck")!=null,"Clicks during warning do not queue a dismissal");g.Continue();yield return Wait(()=>g.State==GameState.RoundActive);Check(g.Luck.Luck==g.Config.postDreamLuck,"Fresh acknowledgement starts gameplay after warning");g.MainMenu();
        foreach(int round in new[]{5,10,15}){g.StartRun(false,false);g.RoundIndex=round;g.NextRound();string key=round==5?"vo_dealer_rule_noadd":round==10?"vo_dealer_rule_doublecurse":"vo_dealer_rule_payout";yield return Wait(()=>g.Audio.VoiceBusy&&g.Audio.VoiceClip!=null&&g.Audio.VoiceClip.name.StartsWith(key));Check(!g.Round.Active&&g.State==GameState.RoundIntro,"Rule "+round+" plays before timer starts");yield return Wait(()=>g.State==GameState.RoundActive);Check(!g.Audio.VoiceBusy,"Rule "+round+" finishes before gameplay");g.MainMenu();}
        g.ShowEnding();end=Time.realtimeSinceStartup+35;while(GameObject.Find("Leave")==null){g.Continue();if(Time.realtimeSinceStartup>end)throw new Exception("Ending timeout");yield return null;}Check(!g.Audio.VoiceBusy,"Ending choices wait for both dealer recordings");
        g.StartRun(false,false);yield return Wait(()=>g.State==GameState.RoundActive);g.Luck.SetLuck(-15);yield return null;Check(g.Audio.VoiceBusy&&g.Audio.VoiceClip.name=="vo_end_death_dealer","Death starts its supplied voice");g.Continue();Check(g.State==GameState.Death,"Death retry cannot truncate its recording");yield return Wait(()=>!g.Audio.VoiceBusy);g.Continue();yield return Wait(()=>g.State==GameState.RoundActive);Check(true,"Death retry resumes after recording completes");g.MainMenu();
    }
    static IEnumerator Tests(){
        g.MainMenu();g.Audio.Unlock();var lib=Resources.Load<VoiceLibrary>("VoiceLibrary");Check(lib!=null&&lib.clips.Length==60,"All 60 supplied recordings are included");
        foreach(var clip in lib.clips){clip.LoadAudioData();yield return Wait(()=>clip.loadState!=AudioDataLoadState.Loading);CheckStep(clip.loadState==AudioDataLoadState.Loaded&&clip.length>0,"Unreadable clip: "+clip.name);var samples=new float[clip.samples*clip.channels];CheckStep(clip.GetData(samples,0),"Unable to decode "+clip.name);float peak=0;foreach(float value in samples){CheckStep(!float.IsNaN(value)&&!float.IsInfinity(value),"Invalid sample: "+clip.name);peak=Mathf.Max(peak,Mathf.Abs(value));}CheckStep(peak>.001f,"Silent clip: "+clip.name);}
        Check(true,"All MP3s load, decode and contain finite non-silent samples");
        g.ChangeState(GameState.Dream);g.Audio.PlayVoice("vo_open_01_hero");yield return new WaitForSeconds(.4f);g.TogglePause();yield return new WaitForSecondsRealtime(.15f);float held=g.Audio.VoiceTime;yield return new WaitForSecondsRealtime(.4f);Check(g.Audio.VoiceBusy&&Mathf.Abs(g.Audio.VoiceTime-held)<.03f,"Pause holds voice and prevents false completion");g.TogglePause();yield return new WaitForSeconds(.3f);Check(g.Audio.VoiceTime>held,"Resume continues from the held sample");
        int starts=g.Audio.VoiceStarts;g.Audio.PlayVoice("vo_open_01_hero");g.Audio.PlayVoice("vo_dealer_too_risky");Check(g.Audio.VoiceStarts==starts,"Busy voice rejects duplicate and competing playback");g.MainMenu();Check(!g.Audio.VoiceBusy,"Menu cancellation stops outstanding dialogue");
        for(int i=0;i<StoryScript.Opening.Length;i++){var shot=StoryScript.Opening[i];string key="vo_open_"+(i+1).ToString("00")+(i==1?"_wife":i==7?"_dealer_door":i==9||i==10?"_dealer":"_hero");yield return Shot(key,shot.Id,shot.Line,shot.Speaker);}
        for(int i=0;i<StoryScript.FirstMemory.Length;i++){var shot=StoryScript.FirstMemory[i];string key="vo_first_"+(i+1).ToString("00")+(i==1?"_wife":i==2?"_boyfriend":i==6?"_hero_laugh":i==7?"_dealer":"_hero");yield return Shot(key,shot.Id,shot.Line,shot.Speaker);}
        for(int n=1;n<=6;n++)for(int p=0;p<3;p++)yield return Shot("vo_mem"+n+"_"+(char)('a'+p)+"_hero","D"+n+(char)('A'+p),StoryPanel.DreamCaption(n,p));
        yield return Shot("vo_end_01_dealer","P8","You are not what I feed on any more.","THE DEALER");
        yield return Shot("vo_end_02_dealer","P8","Some things cannot be won back. You can still walk away.","THE DEALER");
        yield return Shot("vo_end_03_hero","E2","This does not hurt less. But the rain has stopped.");
        g.StartTutorial();yield return null;starts=g.Audio.VoiceStarts;g.Round.SelectChip(0);yield return null;g.Round.SelectOperator(OpKind.Add);yield return null;Check(g.Audio.VoiceStarts==starts,"Tutorial chip/operator selections do not replay instruction");g.Round.SelectChip(1);yield return Wait(()=>!g.Round.Busy);Check(g.Round.Level.Target==11,"Fast tutorial win waits for its voice before changing hand");
        yield return Wait(()=>g.Round.Level.Target==32);Commit(1,OpKind.Mul,2);yield return Wait(()=>!g.Round.Busy);yield return Wait(()=>g.Audio.VoiceClip!=null&&g.Audio.VoiceClip.name=="vo_dealer_tut_04_use30");Commit(0,OpKind.Add,1);
        yield return Wait(()=>g.Round.Level.Target==37);Commit(0,OpKind.Mul,1);yield return Wait(()=>!g.Round.Busy);yield return Wait(()=>g.Audio.VoiceClip!=null&&g.Audio.VoiceClip.name=="vo_dealer_tut_07_cursed4");Commit(0,OpKind.Add,2);
        yield return Wait(()=>g.State==GameState.RoundResolve);Check(g.Luck.Luck==15,"Voiced tutorial completes with +15 Luck");g.MainMenu();
        g.StartRun(false,false);yield return null;Check(g.State==GameState.RoundIntro&&!g.Round.Active,"Round timer waits for introduction voice");yield return Wait(()=>g.State==GameState.RoundActive);g.MainMenu();
        g.Audio.PlayVoice("vo_dealer_too_risky",true);yield return Wait(()=>!g.Audio.VoiceBusy);starts=g.Audio.VoiceStarts;Check(!g.Audio.PlayVoice("vo_dealer_too_risky",true)&&g.Audio.VoiceStarts==starts,"Repeated warning has a cooldown after completion");
    }
}
}
