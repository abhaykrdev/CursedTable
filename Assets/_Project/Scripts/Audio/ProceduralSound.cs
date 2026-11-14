using UnityEngine;
namespace HollowSeven {
// Imported clips override procedural fallback sounds.
public static class ProceduralSound {
    private const int Rate=22050;
    private static AudioClip Clip(string name,float[] data){var clip=AudioClip.Create(name,data.Length,1,Rate,false);clip.SetData(data,0);return clip;}
    public static AudioClip Click(SfxId id){if(id==SfxId.SFX_UIClick||id==SfxId.SFX_UIHover)return MenuThump(id);if(id==SfxId.SFX_LuckPulse||id==SfxId.SFX_LuckCount||id==SfxId.SFX_VisionBreak)return LuckSound(id);bool win=id==SfxId.SFX_Win||id==SfxId.SFX_Jackpot||id==SfxId.SFX_LuckUp;bool dark=id==SfxId.SFX_Curse||id==SfxId.SFX_Doom||id==SfxId.SFX_Death||id==SfxId.SFX_Timeout;float duration=win?.75f:dark?.6f:id==SfxId.SFX_Merge?.22f:.1f;var data=new float[(int)(Rate*duration)];var rng=new System.Random(710+(int)id);for(int i=0;i<data.Length;i++){float t=i/(float)Rate;float noise=(float)(rng.NextDouble()*2-1);float value;if(win){float notes=0;for(int n=0;n<3;n++){float nt=t-n*.09f;if(nt>=0){float f=new[]{440f,554.37f,659.25f}[n];notes+=Mathf.Sin(2*Mathf.PI*f*nt)*Mathf.Exp(-nt*8)*.07f;}}value=notes;}else if(dark){value=(Mathf.Sin(2*Mathf.PI*65*t)+Mathf.Sin(2*Mathf.PI*69*t))*.09f*Mathf.Exp(-t*7)+noise*.03f*Mathf.Exp(-t*30);}else{float freq=id==SfxId.SFX_ChipSelect?1200:id==SfxId.SFX_Merge?640:1700;value=(noise*.24f+Mathf.Sin(2*Mathf.PI*freq*t)*.12f)*Mathf.Exp(-t*(id==SfxId.SFX_Merge?30:80));}data[i]=value*Mathf.Clamp01(t*1500)*Mathf.Clamp01((duration-t)*100);}return Clip("Original_"+id,data);}
    private static AudioClip MenuThump(SfxId id){
        bool hover=id==SfxId.SFX_UIHover;
        float duration=hover?.14f:.24f,phase=0;
        var data=new float[(int)(Rate*duration)];
        for(int i=0;i<data.Length;i++){
            float t=i/(float)Rate;
            float frequency=(hover?105:70)+(hover?65:100)*Mathf.Exp(-t*35);
            phase+=2*Mathf.PI*frequency/Rate;
            float envelope=Mathf.Clamp01(t/.006f)*Mathf.Exp(-t*(hover?27:18))*Mathf.Clamp01((duration-t)/.025f);
            data[i]=(Mathf.Sin(phase)+.22f*Mathf.Sin(phase*2)+.08f*Mathf.Sin(phase*3))*envelope*(hover?.10f:.23f);
        }
        return Clip("MenuThump_"+id,data);
    }
    private static AudioClip LuckSound(SfxId id){
        bool tick=id==SfxId.SFX_LuckCount,cut=id==SfxId.SFX_VisionBreak;
        float duration=tick?.065f:cut?.5f:.65f;var data=new float[(int)(Rate*duration)];
        var random=new System.Random(701);float phase=0,filtered=0;
        for(int i=0;i<data.Length;i++){
            float t=i/(float)Rate;float env=Mathf.Clamp01(t/(tick?.006f:.014f))*Mathf.Exp(-t*(tick?65:8));
            float f=tick?720:cut?125:72+125*Mathf.Exp(-t*22);phase+=2*Mathf.PI*f/Rate;
            filtered=Mathf.Lerp(filtered,(float)(random.NextDouble()*2-1),.12f);
            float value=tick?Mathf.Sin(phase)*.032f:(Mathf.Sin(phase)*.23f+Mathf.Sin(phase*2)*.045f);
            if(cut)value=filtered*.15f+Mathf.Sin(phase)*.08f;
            data[i]=value*env*Mathf.Clamp01((duration-t)*120);
        }
        return Clip("Original_"+id,data);
    }
    public static AudioClip Ambient(int stem){const int seconds=8;var data=new float[Rate*seconds];for(int i=0;i<data.Length;i++){float t=i/(float)Rate;float tone;if(stem==0)tone=(Mathf.Sin(2*Mathf.PI*55*t)*.025f+Mathf.Sin(2*Mathf.PI*82.5f*t)*.016f+Mathf.Sin(2*Mathf.PI*130.75f*t)*.009f)*( .8f+.2f*Mathf.Sin(2*Mathf.PI*t/8));else if(stem==1)tone=(Mathf.Sin(2*Mathf.PI*58.25f*t)+Mathf.Sin(2*Mathf.PI*61*t))*.027f*(.6f+.4f*Mathf.Sin(2*Mathf.PI*t/4));else tone=(Mathf.Sin(2*Mathf.PI*220*t)+Mathf.Sin(2*Mathf.PI*330*t))*.016f*(.6f+.4f*Mathf.Sin(2*Mathf.PI*t/8));data[i]=tone;}return Clip("Original_Ambience_"+stem,data);}
    public static AudioClip Rain(){var data=new float[Rate*4];var rng=new System.Random(77);float smooth=0;for(int i=0;i<data.Length;i++){smooth=Mathf.Lerp(smooth,(float)(rng.NextDouble()*2-1),.2f);data[i]=smooth*.2f;}return Clip("Original_Rain",data);}
}
}
