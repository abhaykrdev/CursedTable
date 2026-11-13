using UnityEngine;
namespace HollowSeven {
public sealed class VoiceLibrary : ScriptableObject {
    public AudioClip[] clips;
    public AudioClip Find(string key) {
        if(string.IsNullOrEmpty(key)||clips==null)return null;
        foreach(var clip in clips)if(clip!=null&&(clip.name==key||clip.name==key+".mp3"))return clip;
        return null;
    }
    public static string ForLine(string text) {
        switch(text){
            case "Make 11.":return "vo_dealer_tut_01_make11";
            case "That's it. Two chips become one.":return "vo_dealer_tut_02_thatsit";
            case "Make 32. Start with 5 × 6.":return "vo_dealer_tut_03_make32";
            case "Now use the 30. Add the 2.":return "vo_dealer_tut_04_use30";
            case "Results are chips too.":return "vo_dealer_tut_05_results";
            case "The green 7 gives Luck. Multiply it by 5.":return "vo_dealer_tut_06_green7";
            case "Add 2 to reach 37. Leave the cursed 4 alone.":return "vo_dealer_tut_07_cursed4";
            case "You don't need every chip. Just the right ones.":return "vo_dealer_tut_08_rightones";
            case "Place your chips.":return "vo_dealer_place_chips";
            case "Choose a chip first.":return "vo_dealer_choose_chip_first";
            case "Too risky.":return "vo_dealer_too_risky";
        }
        return null;
    }
}
}
