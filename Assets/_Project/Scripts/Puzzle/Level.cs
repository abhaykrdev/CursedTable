using System;
namespace HollowSeven {
[Serializable] public sealed class Level {
    public int[] StartChips; public int Target,Tier; [NonSerialized] public Step[] SafeSolution,ShortestSolution; public float TimeLimit; public bool IsJackpotRound,UsedFallback;
    public OpKind[] Operators;public RoundRule Rule;
    public float CurseMultiplier=>Rule==RoundRule.DoubleCurses?2:1;
    public float WinLuckMultiplier=>Rule==RoundRule.LuckyPayout?2:1;
}
}
