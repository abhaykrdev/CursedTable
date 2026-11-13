using System;
namespace HollowSeven {
public enum RoundRule { Standard, NoAddition, DoubleCurses, LuckyPayout }
public static class RoundRules {
    public static RoundRule ForRound(int round) => round<5||round%5!=0?RoundRule.Standard:(RoundRule)(1+((round/5-1)%3));
    public static OpKind[] Operators(OpKind[] normal,RoundRule rule) => rule==RoundRule.NoAddition?Array.FindAll(normal,op=>op!=OpKind.Add):(OpKind[])normal.Clone();
    public static string Label(RoundRule rule) => rule==RoundRule.NoAddition?"HOUSE RULE: NO +\nAddition is locked this hand.":rule==RoundRule.DoubleCurses?"HOUSE RULE: DOUBLE CURSES\nCursed chips cost twice the Luck.":rule==RoundRule.LuckyPayout?"HOUSE RULE: LUCKY PAYOUT\nWinning Luck is doubled.":"";
    public static bool AdditionCanReach(int[] chips,int target){for(int mask=1;mask<(1<<chips.Length);mask++){int sum=0;for(int i=0;i<chips.Length;i++)if((mask&(1<<i))!=0)sum+=chips[i];if(sum==target)return true;}return false;}
    public static void NoAdditionFallback(int tier,out int[] chips,out int target){
        if(tier<=1){chips=new[]{2,3,5,8};target=30;}
        else if(tier==2){chips=new[]{2,6,10,8};target=68;}
        else if(tier==3){chips=new[]{25,10,3,8,6};target=169;}
        else{chips=new[]{75,10,8,6,5,12};target=468;}
    }
}
}
