using System.Collections.Generic;
using UnityEngine;
namespace HollowSeven { public sealed class LuckManager:MonoBehaviour {
    [SerializeField] private GameConfig _config;[SerializeField] private LuckTable _table;
    private readonly HashSet<int> _fired=new HashSet<int>();private bool _dead;
    public float Luck{get;private set;}public LuckState State=>Luck<0?LuckState.Cursed:Luck>_config.blessedThreshold?LuckState.Blessed:LuckState.Neutral;
    public bool BrinkFlag{get;private set;}public bool DreamArmed{get;private set;}public bool DefianceActive{get;private set;}
    public bool Tutorial{get;set;}public float CurseMultiplier{get;set;}=1;
    public void Configure(GameConfig c,LuckTable t){_config=c;_table=t;}
    public void ResetForRun(bool firstRunAfterTutorial){_dead=false;BrinkFlag=false;DreamArmed=false;DefianceActive=false;_fired.Clear();SetLuck(firstRunAfterTutorial?_config.firstRunBonus:_config.startLuck);}
    public void Add(float delta)=>SetLuck(Luck+delta);
    public void SetLuck(float value){var oldState=State;float old=Luck;Luck=Mathf.Clamp(value,_config.luckMin,_config.luckMax);GameEvents.RaiseLuckChanged(old,Luck);if(oldState!=State)GameEvents.RaiseLuckStateChanged(State);if(Tutorial)return;
        if(Luck<=_config.luckMin){if(!_dead){_dead=true;BrinkFlag=false;GameEvents.RaiseRunEnded();}return;}
        if(Luck<=_config.brinkThreshold)BrinkFlag=true;
        if(BrinkFlag&&Luck>=0&&!DefianceActive){DefianceActive=true;GameEvents.RaiseDefianceFired();Add(_config.defianceLuck);}
        if(Luck>=_config.luckMax&&!DreamArmed){DreamArmed=true;GameEvents.RaiseDreamArmed();}
    }
    private float SpendDelta(int value){if(!_fired.Add(value))return 0;float delta=_table.GetDelta(value);if(delta==0)return 0;delta*=delta<0?CurseMultiplier:1;GameEvents.RaiseLuckValueSpent(value,_table.GetTag(value),delta);return delta;}
    public void Spend(int value)=>Add(SpendDelta(value));
    public void SpendOperation(Step step,bool winning){float delta=SpendDelta(step.A)+SpendDelta(step.B);if(winning)delta+=SpendDelta(step.Result);Add(delta);}
    public void BeginRound()=>_fired.Clear();
    public void FinishDream(float value){BrinkFlag=false;DreamArmed=false;DefianceActive=false;SetLuck(value);}
} }
