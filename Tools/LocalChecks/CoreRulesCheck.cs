using System;
using System.Collections.Generic;
using HollowSeven;
public static class CoreRulesCheck {
    private static void Check(bool value,string message){if(!value)throw new Exception(message);}
    private static void Replay(Level level){
        var state=new PuzzleState(level.StartChips,level.Target,level.Operators);
        Check(level.SafeSolution!=null,"Missing safe solution");
        foreach(var step in level.SafeSolution){
            Check(!LuckTable.IsCursed(step.A)&&!LuckTable.IsCursed(step.B),"Cursed safe operand");
            Check(Array.IndexOf(level.Operators,step.Op)>=0,"Forbidden operation in safe solution");
            int a=-1,b=-1;for(int i=0;i<state.Hand.Count;i++){if(a<0&&state.Hand[i]==step.A)a=i;else if(b<0&&state.Hand[i]==step.B)b=i;}
            Check(a>=0&&b>=0&&state.TryCommit(a,b,step.Op,out var committed),"Cannot replay safe solution");
        }
        Check(state.ContainsTarget,"Replayed solution did not win");
    }
    public static void Main(){
        var config=new GameConfig();var table=new DifficultyTable();LevelGenerator.Configure(table,config);
        var times=new Dictionary<int,(float min,float max)>();int total=0,fallbacks=0;
        for(int round=1;round<=40;round++)for(int seed=0;seed<25;seed++){
            var level=LevelGenerator.Generate(round,new Random(round*10000+seed));Replay(level);total++;if(level.UsedFallback)fallbacks++;
            Check(level.Rule==RoundRules.ForRound(round),"Wrong house rule");
            if(level.Rule==RoundRule.NoAddition)Check(Array.IndexOf(level.Operators,OpKind.Add)<0,"Addition not banned");
            if(round<5)Check(!RoundRules.AdditionCanReach(level.StartChips,level.Target),"Early addition-only puzzle");
            Check(level.SafeSolution.Length>=table.ForRound(round).minSafeLength,"Too short safe route");
            float stepTime=level.Tier==6?Math.Max(6,config.perStepTime-.25f*(round-30)):config.perStepTime;
            float expected=(config.baseTime+stepTime*level.SafeSolution.Length)*(level.Tier>=2?.75f:1);
            Check(Math.Abs(expected-level.TimeLimit)<.001f,"Wrong time budget");
            if(!times.TryGetValue(level.Tier,out var range))range=(float.MaxValue,0);times[level.Tier]=(Math.Min(range.min,level.TimeLimit),Math.Max(range.max,level.TimeLimit));
        }
        for(int tier=1;tier<6;tier++){RoundRules.NoAdditionFallback(tier,out var chips,out var target);var ops=RoundRules.Operators(table.tiers[tier].operators,RoundRule.NoAddition);var solved=Solver.Solve(chips,target,config.genNodeBudget,ops);Check(solved.SafeShortest!=null&&solved.SafeShortest.Length>=table.tiers[tier].minSafeLength,"Unsafe modifier fallback");}
        var subset=new PuzzleState(new[]{2,5,3},10,Solver.All);Check(subset.TryCommit(1,0,OpKind.Mul,out var result)&&subset.ContainsTarget&&subset.Hand.Count==2,"Unused chip win regressed");
        Console.WriteLine("PASS: "+total+" seeded levels across rounds 1–40; safe replay, house rules, timer budgets, early variety and unused-chip wins. Fallback levels: "+fallbacks);
        foreach(var pair in times)Console.WriteLine("Tier "+pair.Key+": "+pair.Value.min+"–"+pair.Value.max+" seconds");
        Console.WriteLine("PASS: all No Addition fallback seeds independently solved.");
    }
}
