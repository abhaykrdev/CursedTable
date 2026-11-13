using NUnit.Framework;
using UnityEngine;
namespace HollowSeven.Tests {
public class CoreTests {
    [TestCase(8,8,OpKind.Sub,false,0)] [TestCase(6,4,OpKind.Div,false,0)] [TestCase(12,3,OpKind.Div,true,4)] [TestCase(1,5,OpKind.Mul,false,0)] [TestCase(9999,1,OpKind.Add,false,0)] [TestCase(4,9,OpKind.Sub,true,5)]
    public void Operations(int a,int b,OpKind op,bool valid,int expected){Assert.AreEqual(valid,Operation.TryApply(a,b,op,out int r,out int left,out int right));if(valid)Assert.AreEqual(expected,r);if(op==OpKind.Sub||op==OpKind.Div)Assert.GreaterOrEqual(left,right);}
    [Test] public void SolverExamples(){Assert.AreEqual(1,Solver.Solve(new[]{5,6},11,40000).SafeShortest.Length);Assert.AreEqual(2,Solver.Solve(new[]{11,5,2},27,40000).SafeShortest.Length);var safe=Solver.Solve(new[]{4,5,2,6},16,40000).SafeShortest;Assert.AreEqual(2,safe.Length);foreach(var s in safe){Assert.AreNotEqual(4,s.A);Assert.AreNotEqual(4,s.B);}}
    [Test] public void SolverHonoursOperators(){Assert.IsNull(Solver.Solve(new[]{5,6},30,40000,new[]{OpKind.Add,OpKind.Sub}).SafeShortest);}
    [Test] public void UndoRestoresDuplicateHand(){var p=new PuzzleState(new[]{5,5,2},12);Assert.IsTrue(p.TryCommit(0,1,OpKind.Add,out _));CollectionAssert.AreEqual(new[]{2,10},p.Hand);Assert.IsTrue(p.Undo());CollectionAssert.AreEqual(new[]{5,5,2},p.Hand);}
    [Test] public void ScoreExample(){var c=ScriptableObject.CreateInstance<GameConfig>();var d=ScriptableObject.CreateInstance<DifficultyTable>();var r=ScoreCalculator.Calculate(c,d.tiers[1],12,45,3,false);Assert.AreEqual(520,r.Score);Assert.AreEqual(2.53,r.WinLuck,.01);Assert.AreEqual(3,ScoreCalculator.Calculate(c,d.tiers[1],2,45,0,false).WinLuck);Assert.AreEqual(1,ScoreCalculator.Calculate(c,d.tiers[1],45,45,0,false).WinLuck);Object.DestroyImmediate(c);Object.DestroyImmediate(d);}
    [Test] public void LuckOncePerRoundAndArming(){var c=ScriptableObject.CreateInstance<GameConfig>();var t=ScriptableObject.CreateInstance<LuckTable>();var go=new GameObject("Luck test");var l=go.AddComponent<LuckManager>();l.Configure(c,t);l.ResetForRun(false);l.Spend(7);l.Spend(7);Assert.AreEqual(1,l.Luck);l.BeginRound();l.Spend(7);Assert.AreEqual(2,l.Luck);l.SetLuck(15);l.Add(-3);Assert.IsTrue(l.DreamArmed);bool died=false;System.Action handler=()=>died=true;GameEvents.RunEnded+=handler;l.SetLuck(-15);Assert.IsTrue(died);GameEvents.RunEnded-=handler;Object.DestroyImmediate(go);Object.DestroyImmediate(c);Object.DestroyImmediate(t);}
}
}
