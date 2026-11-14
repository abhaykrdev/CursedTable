using UnityEngine;
namespace HollowSeven {
[CreateAssetMenu(menuName="Cursed Table/Game Config")]
public sealed class GameConfig : ScriptableObject {
    public float startLuck=0, firstRunBonus=15, luckMin=-15, luckMax=15, blessedThreshold=10, brinkThreshold=-10, hauntThreshold=-6, postDreamLuck=2;
    public float timeoutLuck=-3, hintLuckCost=2, winLuckBase=1, winLuckSpeed=2, timerCursedPerLuck=.03f, timerBlessedMult=.85f;
    public float baseTime=15, perStepTime=10, grace=2, streakStep=.1f, defianceLuck=2, defianceScoreMult=2, secondSightIdleSeconds=6;
    public int streakMax=10, windfallScore=500, valueCap=9999, genNodeBudget=150000, liveNodeBudget=40000;
    public float windfallLuck=5, endlessCurseStep=.05f, endlessCurseCap=2, roundIntroSeconds=1.1f, resolveSeconds=1.1f, targetRollSeconds=.5f;
    public float mergeSeconds=.3f, dealSeconds=.25f, dealStagger=.06f, panelSeconds=4, hintSeconds=4, captionSeconds=3, cameraSway=.35f;
    public int generatorAttempts=400, seed=7102026;
}
}
