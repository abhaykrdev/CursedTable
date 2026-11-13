using UnityEngine;
namespace HollowSeven {
[CreateAssetMenu(menuName="Hollow Seven/Difficulty Table")]
public sealed class DifficultyTable : ScriptableObject {
    [System.Serializable] public sealed class Tier {
        public int firstRound,chipsMin,chipsMax,stepsMin,stepsMax,targetMin,targetMax,minSafeLength,bigsMax,poolMax;public float basePoints,penaltyPerSecond;public OpKind[] operators;public int[] fallbackChips;public int fallbackTarget;
        public Tier(int first,int cmin,int cmax,int smin,int smax,int tmin,int tmax,int safe,int big,int pool,float points,float penalty,OpKind[] ops,int[] fallback,int target){firstRound=first;chipsMin=cmin;chipsMax=cmax;stepsMin=smin;stepsMax=smax;targetMin=tmin;targetMax=tmax;minSafeLength=safe;bigsMax=big;poolMax=pool;basePoints=points;penaltyPerSecond=penalty;operators=ops;fallbackChips=fallback;fallbackTarget=target;}
    }
    public Tier[] tiers={
        new Tier(1,3,3,2,2,8,30,2,0,10,5,.25f,new[]{OpKind.Add,OpKind.Sub,OpKind.Mul},new[]{3,6,8},30),
        new Tier(5,4,4,2,3,15,150,2,0,10,7,.30f,new[]{OpKind.Add,OpKind.Sub,OpKind.Mul},new[]{4,5,2,6},16),
        new Tier(10,4,5,3,4,20,300,3,0,12,9,.35f,Solver.All,new[]{2,6,10,8},168),
        new Tier(16,5,5,3,4,50,500,3,1,12,11,.40f,Solver.All,new[]{100,10,3,10,4},170),
        new Tier(23,6,6,4,5,100,999,3,2,12,13,.45f,Solver.All,new[]{75,10,8,6,5,12},201),
        new Tier(31,6,6,4,5,100,999,3,2,12,15,.50f,Solver.All,new[]{75,10,8,6,5,12},201)};
    public int IndexForRound(int round){for(int i=tiers.Length-1;i>=0;i--)if(round>=tiers[i].firstRound)return i;return 0;}
    public Tier ForRound(int round)=>tiers[IndexForRound(round)];
}
}
