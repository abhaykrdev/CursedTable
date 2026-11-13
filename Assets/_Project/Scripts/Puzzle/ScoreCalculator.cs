using UnityEngine;
namespace HollowSeven { public static class ScoreCalculator {
    public static RoundResult Calculate(GameConfig config,DifficultyTable.Tier tier,float elapsed,float limit,int streak,bool defiance){float speed=Mathf.Clamp01(1-Mathf.Max(0,elapsed-config.grace)/Mathf.Max(.01f,limit-config.grace));float score=Mathf.Max(0,tier.basePoints-tier.penaltyPerSecond*Mathf.Max(0,elapsed-config.grace))*(1+config.streakStep*Mathf.Min(streak,config.streakMax))*(defiance?config.defianceScoreMult:1);return new RoundResult(elapsed,Mathf.Round(score*100),config.winLuckBase+config.winLuckSpeed*speed,streak);}
} }
