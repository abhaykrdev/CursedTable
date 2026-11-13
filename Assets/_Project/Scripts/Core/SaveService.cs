using UnityEngine;
namespace HollowSeven { public static class SaveService {
    public static bool PrologueSeen=>PlayerPrefs.GetInt("ln_prologueSeen",0)==1;public static bool TutorialDone=>PlayerPrefs.GetInt("ln_tutorialDone",0)==1;
    public static int BeatsSeen=>PlayerPrefs.GetInt("ln_beatsSeen",0);public static bool EndingSeen=>PlayerPrefs.GetInt("ln_endingSeen",0)==1;
    public static void Set(string key,int value){PlayerPrefs.SetInt(key,value);PlayerPrefs.Save();}
    public static void Beat(int beat)=>Set("ln_beatsSeen",BeatsSeen|(1<<(beat-1)));
    public static int NextBeat(){for(int i=0;i<6;i++)if((BeatsSeen&(1<<i))==0)return i+1;return 7;}
    public static int BeatCount(){int count=0;for(int i=0;i<6;i++)if((BeatsSeen&(1<<i))!=0)count++;return count;}
    public static void Record(int score,int rounds){Set("ln_highScore",Mathf.Max(score,PlayerPrefs.GetInt("ln_highScore")));Set("ln_bestRounds",Mathf.Max(rounds,PlayerPrefs.GetInt("ln_bestRounds")));}
    public static void ResetProgress(){foreach(var key in new[]{"ln_prologueSeen","ln_tutorialDone","ln_beatsSeen","ln_endingSeen","ln_highScore","ln_bestRounds","ln_codexMask","ln_cardsMask"})PlayerPrefs.DeleteKey(key);PlayerPrefs.Save();}
    public static string Rank(int rounds)=>rounds>=45?"UNBREAKABLE":rounds>=30?"HOUSE FAVOURITE":rounds>=20?"HIGH ROLLER":rounds>=10?"GAMBLER":rounds>=5?"REGULAR":"DRIFTER";
} }
