using System;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;
namespace HollowSeven.Editor {
public static class SolverSelfTest {
    private static int _tier,_seed,_fallbacks;private static double _total,_max;private static GameConfig _config;private static DifficultyTable _table;private static StringBuilder _report;
    [MenuItem("Cursed Table/Run Solver Self Test")]
    public static void Run(){
        EditorApplication.update-=Tick;_config=ScriptableObject.CreateInstance<GameConfig>();_table=ScriptableObject.CreateInstance<DifficultyTable>();LevelGenerator.Configure(_table,_config);
        _tier=0;_seed=1;_fallbacks=0;_total=0;_max=0;_report=new StringBuilder("# Solver verification\n\n500 seeds per tier; all routes replayed with allowed operators and no cursed operands.\n\n");
        File.WriteAllText("Docs/solver-progress.txt","RUNNING");EditorApplication.update+=Tick;
    }
    private static void Check(bool value,string msg){if(!value)throw new Exception(msg);}
    private static void Tick(){try{
        var tier=_table.tiers[_tier];var sw=System.Diagnostics.Stopwatch.StartNew();var level=LevelGenerator.Generate(tier.firstRound,new System.Random(_seed));sw.Stop();double ms=sw.Elapsed.TotalMilliseconds;_total+=ms;_max=Math.Max(_max,ms);if(level.UsedFallback)_fallbacks++;
        Check(level.SafeSolution!=null,"No safe solution");Check(level.Target>=tier.targetMin&&level.Target<=tier.targetMax,"Range");Check(!LuckTable.IsCursed(level.Target),"Cursed target");Check(Array.IndexOf(level.StartChips,level.Target)<0,"Already won");
        Check(level.SafeSolution.Length>=tier.minSafeLength,"Short safe route");Check(level.ShortestSolution.Length<=level.SafeSolution.Length,"Shortest ordering");
        var state=new PuzzleState(level.StartChips,level.Target,level.Operators);
        foreach(var step in level.SafeSolution){Check(!LuckTable.IsCursed(step.A)&&!LuckTable.IsCursed(step.B),"Cursed route");int a=-1,b=-1;for(int i=0;i<state.Hand.Count;i++){if(a<0&&state.Hand[i]==step.A)a=i;else if(b<0&&state.Hand[i]==step.B)b=i;}Check(state.TryCommit(a,b,step.Op,out var actual)&&actual.Result==step.Result,"Replay failed");}Check(state.ContainsTarget,"Target not reached");
        if(++_seed>500){_report.AppendLine($"Tier {_tier+1}: PASS correctness; fallback {_fallbacks}/500; average {_total/500:F2} ms; maximum {_max:F2} ms.");File.WriteAllText("Docs/SOLVER_TESTS.md",_report.ToString());Check(_fallbacks<5,"Fallback rate >=1%");Check(_total/500<40,"Average >40ms");Check(_max<400,"Maximum >400ms");_seed=1;_tier++;_fallbacks=0;_total=0;_max=0;}
        File.WriteAllText("Docs/solver-progress.txt",_tier==6?"PASS ALL 3000":"RUNNING tier "+(_tier+1)+" seed "+_seed);
        if(_tier==6){EditorApplication.update-=Tick;UnityEngine.Object.DestroyImmediate(_config);UnityEngine.Object.DestroyImmediate(_table);Debug.Log("Solver self-test PASS all 3000 seeds");}
    }catch(Exception e){EditorApplication.update-=Tick;File.WriteAllText("Docs/solver-progress.txt","FAIL tier "+(_tier+1)+" seed "+_seed+": "+e);Debug.LogError(e);}}
}
}
