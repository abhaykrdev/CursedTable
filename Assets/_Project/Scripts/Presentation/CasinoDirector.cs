using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
namespace HollowSeven { public sealed class CasinoDirector:MonoBehaviour {
    public GameManager Game;public Light SpotTable,PointDealer,PointNeon,ChandelierLight;public Renderer Felt,Mask;public Transform Rotor,Ball,CameraRig,Dealer;public GameObject Crack,ExitGlow,CalmMouth,Grin;public Volume Volume;
    private Vector3 _cameraPosition;private Quaternion _cameraRotation;private float _spin,_shake,_luck;private Vignette _vignette;private Bloom _bloom;private ColorAdjustments _grade;
    private DealerPerformance _performance;private float _conversation;
    public void Speak(string line,float seconds=0){if(_performance!=null)_performance.Speak(line,seconds);}
    public void Quiet(){if(_performance!=null)_performance.Quiet();}
    private void Awake(){if(Dealer!=null)_performance=Dealer.gameObject.AddComponent<DealerPerformance>();if(CameraRig!=null){_cameraPosition=CameraRig.localPosition;_cameraRotation=CameraRig.localRotation;}if(Volume!=null&&Volume.profile!=null){Volume.profile.TryGet(out _vignette);Volume.profile.TryGet(out _bloom);Volume.profile.TryGet(out _grade);}}
    public void Spin(){_spin=Game==null?1.1f:Game.Config.roundIntroSeconds;}public void Shake(){_shake=.25f;}public void RevealEnding(){if(Crack!=null)Crack.SetActive(true);if(ExitGlow!=null)ExitGlow.SetActive(true);}
    public void ResetPresentation(){if(Crack!=null)Crack.SetActive(false);if(ExitGlow!=null)ExitGlow.SetActive(false);}
    private void Update(){if(Game==null)return;_luck=Mathf.Lerp(_luck,Game.Luck.Luck,Time.deltaTime*2);float curse=Mathf.Clamp01(-_luck/15),bless=Mathf.Clamp01((_luck-10)/5);float t=Time.time;
        if(SpotTable!=null){SpotTable.intensity=3.2f*(1-.65f*curse+.35f*bless);SpotTable.color=Color.Lerp(UIFactory.Hex("FFC77A"),UIFactory.Hex("FF5A4A"),curse);}
        if(PointDealer!=null){PointDealer.color=Color.Lerp(UIFactory.Hex("FFF1DC"),UIFactory.Hex("FF1F3A"),curse);PointDealer.intensity=1.3f*(1+.6f*curse-.2f*bless);}
        if(Grin!=null){bool grinning=_luck<-.05f;if(Grin.activeSelf!=grinning)Grin.SetActive(grinning);if(CalmMouth!=null&&CalmMouth.activeSelf==grinning)CalmMouth.SetActive(!grinning);}
        RenderSettings.fogColor=Color.Lerp(UIFactory.Hex("120C10"),UIFactory.Hex("1A0508"),curse);RenderSettings.fogDensity=.03f+.03f*curse-.012f*bless;
        if(Felt!=null)Felt.material.color=Color.Lerp(Color.Lerp(UIFactory.Hex("0F2A22"),UIFactory.Hex("2A0F14"),curse),UIFactory.Hex("12332A"),bless);
        if(_vignette!=null)_vignette.intensity.value=.3f+.25f*curse-.1f*bless;if(_bloom!=null)_bloom.intensity.value=.6f-.2f*curse+.6f*bless;if(_grade!=null){_grade.saturation.value=-10-35*curse+25*bless;_grade.contrast.value=10+15*curse-5*bless;}
        _spin=Mathf.Max(0,_spin-Time.deltaTime);float speed=Mathf.Lerp(20+15*curse-8*bless,240,Mathf.Clamp01(_spin/1.1f));if(Game.State==GameState.Death||Game.State==GameState.Ending)speed=0;if(Rotor!=null)Rotor.Rotate(0,speed*Time.deltaTime,0);
        if(Ball!=null){float angle=_spin>0?-t*6:-(Rotor.eulerAngles.y+30)*Mathf.Deg2Rad;float radius=_spin>0?Mathf.Lerp(.3f,.42f,_spin/1.1f):.3f;Ball.localPosition=new Vector3(Mathf.Cos(angle)*radius,1.10f, .4f+Mathf.Sin(angle)*radius);}
        if(CameraRig!=null){_conversation=Mathf.Lerp(_conversation,_performance!=null&&_performance.Speaking&&Game.State==GameState.Prologue?1:0,Time.deltaTime*2);float amp=(1+.8f*curse-.6f*bless);_shake=Mathf.Max(0,_shake-Time.deltaTime);CameraRig.localPosition=_cameraPosition+new Vector3(0,.02f,.35f)*_conversation+new Vector3(Mathf.Sin(t*.35f)*.012f,Mathf.Sin(t*.5f)*.008f,0)*amp+Vector3.right*(Mathf.Sin(t*70)*_shake*.12f);CameraRig.localRotation=_cameraRotation*Quaternion.Euler(Mathf.Sin(t*.4f)*.3f,Mathf.Sin(t*.3f)*.4f,Mathf.Sin(t*.35f)*.55f*amp);}
        if(Dealer!=null)Dealer.localScale=new Vector3(1,1+Mathf.Sin(t*1.57f)*.006f,1);
    }
} }
