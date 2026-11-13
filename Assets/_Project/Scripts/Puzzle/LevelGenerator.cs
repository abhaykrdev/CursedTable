using System;
using System.Collections;
using System.Collections.Generic;
namespace HollowSeven {
public static class LevelGenerator {
    private static DifficultyTable _table;private static GameConfig _config;
    public static void Configure(DifficultyTable table,GameConfig config){_table=table;_config=config;Operation.ValueCap=config.valueCap;}
    public static Level Generate(int roundIndex,Random rng){Level result=null;var routine=GenerateRoutine(roundIndex,rng,x=>result=x);while(routine.MoveNext()){}return result;}
    public static IEnumerator GenerateRoutine(int roundIndex,Random rng,Action<Level> done){
        var tier=_table.ForRound(roundIndex);int tierIndex=_table.IndexForRound(roundIndex);var rule=RoundRules.ForRound(roundIndex);var operators=RoundRules.Operators(tier.operators,rule);bool jackpot=tierIndex>=3&&rng.NextDouble()<.2;
        for(int attempt=0;attempt<_config.generatorAttempts;attempt++){
            int count=rng.Next(tier.chipsMin,tier.chipsMax+1);var chips=new int[count];
            for(int i=0;i<count;i++){int v;int tries=0;do{v=rng.Next(1,tier.poolMax+1);if(tierIndex==0&&v==4)continue;int copies=0;for(int j=0;j<i;j++)if(chips[j]==v)copies++;if(copies<2)break;}while(++tries<100);if(tierIndex==0&&v==4)v=5;chips[i]=v;}
            if(tier.bigsMax>0){int bigs=rng.Next(1,tier.bigsMax+1);for(int i=0;i<bigs;i++)chips[i]=new[]{25,50,75,100}[rng.Next(4)];}
            var pool=new List<int>(chips);int steps=rng.Next(tier.stepsMin,Math.Min(tier.stepsMax,count-1)+1);int result=0;bool valid=true;
            for(int s=0;s<steps;s++){int i=rng.Next(pool.Count),j=rng.Next(pool.Count-1);if(j>=i)j++;int start=rng.Next(operators.Length);bool found=false;
                for(int o=0;o<operators.Length;o++){var op=operators[(o+start)%operators.Length];if(Operation.TryApply(pool[i],pool[j],op,out result,out _,out _)){found=true;break;}}
                if(!found){valid=false;break;}pool.RemoveAt(Math.Max(i,j));pool.RemoveAt(Math.Min(i,j));pool.Add(result);
            }
            bool isJackpot=result==108||(tierIndex>=4&&(result==777||result==786||result==888));
            if(valid&&result>=tier.targetMin&&result<=tier.targetMax&&Array.IndexOf(chips,result)<0&&!LuckTable.IsCursed(result)&&(tierIndex!=0||!RoundRules.AdditionCanReach(chips,result))&&(!jackpot||attempt>=300||isJackpot)){
                var solved=Solver.Solve(chips,result,_config.genNodeBudget,operators);
                if(solved.SafeShortest!=null&&solved.Shortest!=null&&solved.SafeShortest.Length>=tier.minSafeLength){done(Make(chips,result,tierIndex,roundIndex,solved,isJackpot,false,rule,operators));yield break;}
            }
            yield return null;
        }
        var fallbackChips=tier.fallbackChips;int fallbackTarget=tier.fallbackTarget;if(rule==RoundRule.NoAddition)RoundRules.NoAdditionFallback(tierIndex,out fallbackChips,out fallbackTarget);
        var fallback=Solver.Solve(fallbackChips,fallbackTarget,_config.genNodeBudget,operators);
        if(fallback.SafeShortest==null)throw new InvalidOperationException("Fallback has no safe solution, tier "+tierIndex);
        done(Make((int[])fallbackChips.Clone(),fallbackTarget,tierIndex,roundIndex,fallback,false,true,rule,operators));
    }
    private static Level Make(int[] chips,int target,int tier,int round,SolveResult solved,bool jackpot,bool fallback,RoundRule rule,OpKind[] operators){float ts=tier==5?Math.Max(6,_config.perStepTime-.25f*(round-30)):_config.perStepTime;return new Level{StartChips=chips,Target=target,Tier=tier+1,Operators=operators,Rule=rule,SafeSolution=solved.SafeShortest,ShortestSolution=solved.Shortest,TimeLimit=(_config.baseTime+ts*solved.SafeShortest.Length)*(tier>=1?.75f:1),IsJackpotRound=jackpot,UsedFallback=fallback};}
}
}
