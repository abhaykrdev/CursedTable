using System.Collections.Generic;
namespace HollowSeven {
public sealed class PuzzleState {
    private readonly List<int> _hand; private readonly Stack<int[]> _undo=new Stack<int[]>(); private readonly OpKind[] _ops;
    public IReadOnlyList<int> Hand=>_hand; public int Target{get;} public bool IsDeadEnd=>_hand.Count==1&&!ContainsTarget;
    public bool ContainsTarget=>_hand.Contains(Target); public bool CanUndo=>_undo.Count>0;
    public PuzzleState(int[] chips,int target,OpKind[] ops=null){_hand=new List<int>(chips);Target=target;_ops=ops;}
    public bool TryCommit(int indexA,int indexB,OpKind op,out Step step){
        step=default;if(indexA<0||indexB<0||indexA>=_hand.Count||indexB>=_hand.Count||indexA==indexB)return false;
        if(_ops!=null&&System.Array.IndexOf(_ops,op)<0)return false;
        if(!Operation.TryApply(_hand[indexA],_hand[indexB],op,out int r,out int a,out int b))return false;
        _undo.Push(_hand.ToArray());_hand.RemoveAt(System.Math.Max(indexA,indexB));_hand.RemoveAt(System.Math.Min(indexA,indexB));_hand.Add(r);step=new Step(a,b,op,r);return true;
    }
    public bool Undo(){if(_undo.Count==0)return false;_hand.Clear();_hand.AddRange(_undo.Pop());return true;}
    public int[] Snapshot()=>_hand.ToArray();
}
}
