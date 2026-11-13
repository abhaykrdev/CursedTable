using System;
using System.Collections.Generic;
namespace HollowSeven {
public static class Solver {
    public static readonly OpKind[] All={OpKind.Add,OpKind.Sub,OpKind.Mul,OpKind.Div};
    public static SolveResult Solve(int[] chips,int target,int nodeBudget)=>Solve(chips,target,nodeBudget,All);
    public static SolveResult Solve(int[] chips,int target,int nodeBudget,OpKind[] operators){
        // Independent budgets prevent the unrestricted search starving the safety proof.
        var safe=new Search(chips,target,nodeBudget,operators,true);var safePath=safe.Run();
        var any=new Search(chips,target,nodeBudget,operators,false);var anyPath=any.Run();
        return new SolveResult{Shortest=anyPath,SafeShortest=safePath,BudgetExceeded=safe.Exceeded||any.Exceeded,Nodes=safe.Nodes+any.Nodes};
    }
    private sealed class Search {
        private readonly int[] _chips; private readonly int _target,_budget; private readonly OpKind[] _ops;private readonly bool _safe;
        private readonly HashSet<string> _visited=new HashSet<string>();private readonly Step[] _path;private Step[] _found;
        public int Nodes; public bool Exceeded=>Nodes>_budget;
        public Search(int[] c,int target,int budget,OpKind[] ops,bool safe){_chips=(int[])c.Clone();Array.Sort(_chips);_target=target;_budget=budget;_ops=ops;_safe=safe;_path=new Step[Math.Max(1,c.Length)];}
        public Step[] Run(){if(Array.IndexOf(_chips,_target)>=0)return Array.Empty<Step>();for(int d=1;d<_chips.Length;d++){_visited.Clear();if(Dfs(_chips,d,0))return _found;if(Exceeded)break;}return null;}
        private bool Dfs(int[] state,int depth,int at){
            if(++Nodes>_budget||depth==0)return false;
            // Full state keys make correctness independent of hash collisions.
            string key=depth+":"+string.Join(",",state);if(!_visited.Add(key))return false;
            for(int i=0;i<state.Length-1;i++) {if(i>0&&state[i]==state[i-1])continue;
                for(int j=i+1;j<state.Length;j++) {if(j>i+1&&state[j]==state[j-1])continue;
                    if(_safe&&(LuckTable.IsCursed(state[i])||LuckTable.IsCursed(state[j])))continue;
                    foreach(var op in _ops){
                        if(!Operation.TryApply(state[i],state[j],op,out int r,out int left,out int right))continue;
                        _path[at]=new Step(left,right,op,r);
                        if(r==_target){_found=new Step[at+1];Array.Copy(_path,_found,at+1);return true;}
                        if(depth<=1)continue;
                        var next=new int[state.Length-1];int n=0;for(int k=0;k<state.Length;k++)if(k!=i&&k!=j)next[n++]=state[k];next[n]=r;Array.Sort(next);
                        if(Dfs(next,depth-1,at+1))return true;if(Exceeded)return false;
                    }
                }
            }return false;
        }
    }
}
}
