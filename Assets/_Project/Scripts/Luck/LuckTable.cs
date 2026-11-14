using UnityEngine;
namespace HollowSeven {
[CreateAssetMenu(menuName="Cursed Table/Luck Table")]
public sealed class LuckTable : ScriptableObject {
    [System.Serializable] public struct Entry {public int value; public LuckTag tag; public float delta; public string codexBlurb; public Entry(int v,LuckTag t,float d,string text){value=v;tag=t;delta=d;codexBlurb=text;}}
    public Entry[] entries={
        new Entry(3,LuckTag.Lucky,1,"Third time lucky: a lucky number in many folk traditions."),new Entry(7,LuckTag.Lucky,1,"Lucky in Western traditions and the classic slot-machine symbol."),
        new Entry(8,LuckTag.Lucky,1,"In Chinese, its sound resembles the word for prosperity."),new Entry(9,LuckTag.Lucky,1,"In Chinese, its sound resembles the word for long-lasting."),
        new Entry(77,LuckTag.DoubleLucky,2,"Double sevens: fortune twice over."),new Entry(88,LuckTag.DoubleLucky,2,"Double prosperity in Chinese numerology."),
        new Entry(108,LuckTag.Jackpot,3,"Sacred in Hinduism and Buddhism; prayer strings commonly have 108 beads."),new Entry(777,LuckTag.Jackpot,3,"The classic slot-machine jackpot."),
        new Entry(786,LuckTag.Jackpot,3,"In South Asian Islamic numerology, associated with the opening words of the Bismillah."),new Entry(888,LuckTag.Jackpot,3,"Triple prosperity in Chinese numerology."),
        new Entry(4,LuckTag.Cursed,-1,"In parts of East Asia, its sound resembles the word for death."),new Entry(13,LuckTag.Cursed,-1,"Triskaidekaphobia: unlucky in many Western traditions."),
        new Entry(17,LuckTag.Cursed,-1,"In Italy, XVII rearranges to VIXI: I have lived."),new Entry(49,LuckTag.Cursed,-1,"In Japanese, 4 and 9 can be read as to suffer."),new Entry(666,LuckTag.Doom,-3,"In Christian tradition, linked with the Number of the Beast.")};
    public int Index(int v){for(int i=0;i<entries.Length;i++)if(entries[i].value==v)return i;return -1;}
    public LuckTag GetTag(int v){int i=Index(v);return i<0?LuckTag.None:entries[i].tag;}
    public float GetDelta(int v){int i=Index(v);return i<0?0:entries[i].delta;}
    public bool IsCursedOrDoom(int v)=>GetDelta(v)<0;
    public static bool IsCursed(int v)=>v==4||v==13||v==17||v==49||v==666;
}
}
