using System;
namespace HollowSeven { public static class GameEvents {
    public static event Action<GameState,GameState> StateChanged;
    public static event Action<float,float> LuckChanged;public static event Action<LuckState> LuckStateChanged;
    public static event Action<int,LuckTag,float> LuckValueSpent;public static event Action<Level> RoundStarted;public static event Action<Step> OperationCommitted;
    public static event Action UndoPerformed,DreamArmed,DreamEnded,DefianceFired,RunEnded;public static event Action<int> DreamStarted;
    public static event Action<RoundResult> RoundWon;public static event Action<RoundFailReason> RoundFailed;
    public static void RaiseStateChanged(GameState a,GameState b)=>StateChanged?.Invoke(a,b);
    public static void RaiseLuckChanged(float a,float b)=>LuckChanged?.Invoke(a,b);public static void RaiseLuckStateChanged(LuckState a)=>LuckStateChanged?.Invoke(a);
    public static void RaiseLuckValueSpent(int v,LuckTag tag,float d)=>LuckValueSpent?.Invoke(v,tag,d);public static void RaiseRoundStarted(Level l)=>RoundStarted?.Invoke(l);public static void RaiseOperationCommitted(Step s)=>OperationCommitted?.Invoke(s);
    public static void RaiseUndoPerformed()=>UndoPerformed?.Invoke();public static void RaiseDreamArmed()=>DreamArmed?.Invoke();public static void RaiseDreamEnded()=>DreamEnded?.Invoke();public static void RaiseDreamStarted(int n)=>DreamStarted?.Invoke(n);public static void RaiseDefianceFired()=>DefianceFired?.Invoke();public static void RaiseRunEnded()=>RunEnded?.Invoke();
    public static void RaiseRoundWon(RoundResult r)=>RoundWon?.Invoke(r);public static void RaiseRoundFailed(RoundFailReason r)=>RoundFailed?.Invoke(r);
    [UnityEngine.RuntimeInitializeOnLoadMethod(UnityEngine.RuntimeInitializeLoadType.SubsystemRegistration)] private static void Reset(){StateChanged=null;LuckChanged=null;LuckStateChanged=null;LuckValueSpent=null;RoundStarted=null;OperationCommitted=null;UndoPerformed=null;DreamArmed=null;DreamEnded=null;DreamStarted=null;DefianceFired=null;RunEnded=null;RoundWon=null;RoundFailed=null;}
} }
