using UnityEngine;
namespace HollowSeven { public sealed class AudioManager:MonoBehaviour {
    [System.Serializable] public struct SoundSlot {public SfxId id;public AudioClip clip;}
    public SoundSlot[] sounds;public AudioClip MUS_Base,MUS_Tension,MUS_Shimmer,MUS_Menu,MUS_Dream,MUS_Ending,AMB_Rain,AMB_RoomTone,AMB_Heartbeat;
    private AudioSource[] _pool,_stems;private bool _unlocked;public bool Muted{get;private set;}public float Master{get;private set;}=1;public float Music{get;private set;}=.7f;public float Sfx{get;private set;}=.9f;public GameManager Game;
    [SerializeField] private bool _proceduralFallback=true;private AudioClip[] _generatedSfx;private AudioSource _rainSource;private bool _raining;private float _duck;private AudioSource _heroVoice;
    private VoiceLibrary _voices;private Coroutine _voiceRoutine;private bool _voiceActive,_voicePaused;private string _voiceKey;private float _voiceEnded;private float _voiceDuck=1;
    public bool VoiceBusy=>_voiceActive;
    public int VoiceStarts{get;private set;}
    public AudioClip VoiceClip=>_heroVoice==null?null:_heroVoice.clip;
    public float VoiceTime=>_heroVoice==null?0:_heroVoice.time;
    public float VoiceLength(string key){if(_voices==null)_voices=Resources.Load<VoiceLibrary>("VoiceLibrary");var c=_voices==null?null:_voices.Find(key);return c==null?0:c.length;}
    public bool PlayVoice(string key,bool incidental=false){
        Unlock();if(string.IsNullOrEmpty(key))return false;
        if(_voiceActive)return false;
        if(incidental&&key==_voiceKey&&Time.unscaledTime-_voiceEnded<4)return false;
        if(_voices==null)_voices=Resources.Load<VoiceLibrary>("VoiceLibrary");
        var clip=_voices==null?null:_voices.Find(key);if(clip==null){Debug.LogWarning("Missing dialogue recording: "+key);return false;}
        if(_heroVoice==null){_heroVoice=gameObject.AddComponent<AudioSource>();_heroVoice.playOnAwake=false;_heroVoice.loop=false;_heroVoice.spatialBlend=0;_heroVoice.priority=0;}
        _voiceKey=key;_voiceActive=true;VoiceStarts++;_voiceRoutine=StartCoroutine(VoicePlayback(clip));return true;
    }
    private System.Collections.IEnumerator VoicePlayback(AudioClip clip){
        if(clip.loadState==AudioDataLoadState.Unloaded)clip.LoadAudioData();
        float deadline=Time.realtimeSinceStartup+10;
        while(clip.loadState==AudioDataLoadState.Loading&&Time.realtimeSinceStartup<deadline)yield return null;
        if(clip.loadState!=AudioDataLoadState.Loaded){Debug.LogWarning("Unable to load dialogue: "+clip.name);_voiceActive=false;_voiceRoutine=null;yield break;}
        _heroVoice.clip=clip;_heroVoice.volume=Master*Sfx;_heroVoice.mute=Muted;_heroVoice.Play();
        yield return null;
        while(_heroVoice.isPlaying||_voicePaused||(Game!=null&&Game.Paused))yield return null;
        _voiceActive=false;_voiceEnded=Time.unscaledTime;_voiceRoutine=null;
    }
    public void StopVoice(){if(_voiceRoutine!=null)StopCoroutine(_voiceRoutine);_voiceRoutine=null;if(_heroVoice!=null)_heroVoice.Stop();_voiceActive=false;_voicePaused=false;_voiceKey=null;if(Game!=null&&Game.Director!=null)Game.Director.Quiet();}
    private void OnDisable(){StopVoice();}

    public void Unlock(){if(_unlocked)return;_unlocked=true;Muted=PlayerPrefs.GetInt("ln_muted")==1;Master=PlayerPrefs.GetFloat("ln_volMaster",1);Music=PlayerPrefs.GetFloat("ln_volMusic",.7f);Sfx=PlayerPrefs.GetFloat("ln_volSfx",.9f);_pool=new AudioSource[12];for(int i=0;i<12;i++)_pool[i]=gameObject.AddComponent<AudioSource>();_stems=new AudioSource[3];if(_proceduralFallback){_generatedSfx=new AudioClip[System.Enum.GetValues(typeof(SfxId)).Length];for(int i=0;i<_generatedSfx.Length;i++)_generatedSfx[i]=ProceduralSound.Click((SfxId)i);if(MUS_Base==null)MUS_Base=ProceduralSound.Ambient(0);if(MUS_Tension==null)MUS_Tension=ProceduralSound.Ambient(1);if(MUS_Shimmer==null)MUS_Shimmer=ProceduralSound.Ambient(2);if(AMB_Rain==null)AMB_Rain=ProceduralSound.Rain();}var clips=new[]{MUS_Base,MUS_Tension,MUS_Shimmer};double start=AudioSettings.dspTime+.15;for(int i=0;i<3;i++){_stems[i]=gameObject.AddComponent<AudioSource>();_stems[i].loop=true;_stems[i].volume=0;_stems[i].clip=clips[i];if(clips[i]!=null)_stems[i].PlayScheduled(start);}_rainSource=gameObject.AddComponent<AudioSource>();_rainSource.clip=AMB_Rain;_rainSource.loop=true;_rainSource.volume=0;if(AMB_Rain!=null)_rainSource.Play();if(AMB_RoomTone!=null){_roomSource=gameObject.AddComponent<AudioSource>();_roomSource.playOnAwake=false;_roomSource.spatialBlend=0;_roomSource.clip=AMB_RoomTone;_roomSource.loop=true;_roomSource.volume=0;_roomSource.Play();}}
    private AudioSource _roomSource;private string _storyId="";
    private float _nextHoverSound;
    public void PlaySfx(SfxId id){if(id==SfxId.SFX_UIClick&&!_unlocked)Unlock();if(id==SfxId.SFX_UIHover){if(Time.unscaledTime<_nextHoverSound)return;_nextHoverSound=Time.unscaledTime+.08f;}if(id==SfxId.SFX_LuckPulse||id==SfxId.SFX_VisionBreak)_duck=.6f;if(!_unlocked)return;AudioClip clip=null;if(sounds!=null)foreach(var slot in sounds)if(slot.id==id){clip=slot.clip;break;}if(clip==null&&_generatedSfx!=null)clip=_generatedSfx[(int)id];if(clip==null)return;int copies=0;foreach(var source in _pool)if(source.isPlaying&&source.clip==clip)copies++;if(copies>=3)return;foreach(var source in _pool)if(!source.isPlaying){source.mute=Muted;source.clip=clip;source.volume=Muted?0:Master*Sfx;source.pitch=Random.Range(.97f,1.03f);source.Play();return;}}
    public void SetStory(string id){_storyId=id;
        _raining=id=="P0"||id=="P1"||id=="P2"||id=="P4"||id=="D3C"||id=="D4A";if(id=="D4B")PlaySfx(SfxId.SFX_Screech);if(id=="P4")PlaySfx(SfxId.SFX_DoorCreak);if(id=="FB")PlaySfx(SfxId.SFX_VisionBreak);}
    public void ToggleMute(){Muted=!Muted;RefreshLevels();if(_pool!=null)foreach(var source in _pool)source.mute=Muted;SaveService.Set("ln_muted",Muted?1:0);}
    public void SetMaster(float value){value=Mathf.Clamp01(value);Master=value;RefreshLevels();PlayerPrefs.SetFloat("ln_volMaster",value);PlayerPrefs.Save();}
    public void SetMusic(float value){value=Mathf.Clamp01(value);Music=value;RefreshLevels();PlayerPrefs.SetFloat("ln_volMusic",value);PlayerPrefs.Save();}
    public void SetSfx(float value){value=Mathf.Clamp01(value);Sfx=value;RefreshLevels();PlayerPrefs.SetFloat("ln_volSfx",value);PlayerPrefs.Save();}
    private bool _refreshMix;
    private void RefreshLevels(){
        _refreshMix=true;
        if(_pool!=null)foreach(var source in _pool){source.volume=Master*Sfx;source.mute=Muted;}
        if(_heroVoice!=null){_heroVoice.volume=Master*Sfx;_heroVoice.mute=Muted;}
        if(_stems!=null)foreach(var source in _stems)if(source!=null){source.mute=Muted;if(Master==0||Music==0)source.volume=0;}
        if(_roomSource!=null){_roomSource.mute=Muted;if(Master==0||Music==0)_roomSource.volume=0;}
        if(_rainSource!=null){_rainSource.mute=Muted;if(Master==0||Music==0)_rainSource.volume=0;}
    }
    private void Update(){if(!_unlocked||Game==null)return;
        if(_roomSource!=null){
            bool casino=_storyId==""||_storyId=="P8"||_storyId=="FB"||_storyId=="HS"||_storyId=="HL";
            float target=casino?Master*Music*.8f*_voiceDuck*(Game.Paused?.5f:1):0;
            _roomSource.mute=Muted;
            _roomSource.volume=_refreshMix?target:Mathf.MoveTowards(_roomSource.volume,target,Time.unscaledDeltaTime*.45f);
        }
if(_heroVoice!=null){_heroVoice.volume=Master*Sfx;_heroVoice.mute=Muted;if(Game.Paused&&!_voicePaused){_heroVoice.Pause();_voicePaused=true;}else if(!Game.Paused&&_voicePaused){_heroVoice.UnPause();_voicePaused=false;}}_voiceDuck=Mathf.MoveTowards(_voiceDuck,VoiceBusy?.28f:1,Time.unscaledDeltaTime*(VoiceBusy?8:2));float curse=Mathf.Clamp01(-Game.Luck.Luck/15),bless=Mathf.Clamp01((Game.Luck.Luck-10)/5);_duck=Mathf.Max(0,_duck-Time.unscaledDeltaTime);float volume=Muted?0:Master*Music*(_roomSource!=null?.25f:1)*_voiceDuck*(Game.Paused?.4f:1)*(1-_duck*.6f);if(Game.State==GameState.Dream||Game.State==GameState.Prologue)volume*=.4f;for(int i=0;i<3;i++){float target=(i==0?1-.4f*curse:i==1?curse:bless)*volume;_stems[i].volume=_refreshMix?target:Mathf.Lerp(_stems[i].volume,target,Time.unscaledDeltaTime*2);}if(_rainSource!=null)_rainSource.volume=Mathf.Lerp(_rainSource.volume,_raining&&!Muted?Master*Music*.4f*_voiceDuck:0,_refreshMix?1:Time.unscaledDeltaTime*3);_refreshMix=false;}
} }
