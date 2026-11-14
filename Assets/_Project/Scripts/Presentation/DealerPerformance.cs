using UnityEngine;

namespace HollowSeven {
// Gestures follow the dialogue duration and return to the captured rest pose.
public sealed class DealerPerformance : MonoBehaviour {
    private Transform _head,_mouth,_leftEye,_rightEye,_leftHand,_rightHand,_leftArm,_rightArm;
    private Vector3 _mouthScale,_eyeLScale,_eyeRScale,_handLRest,_handRRest;
    private Quaternion _armLRest,_armRRest;
    private float _until,_weight,_start;
    private string _line;
    public bool Speaking => Time.time<_until;
    private void Awake(){
        var pivot=new GameObject("SpeakingHeadPivot");pivot.transform.SetParent(transform,false);pivot.transform.localPosition=new Vector3(0,1.67f,2.4f);_head=pivot.transform;
        foreach(var name in new[]{"Head","Mask","EyeLeft","EyeRight","Nose","Mouth","Mask_Crack","Mask_Grin"}){var part=transform.Find(name);if(part!=null)part.SetParent(_head,true);}
        _mouth=_head.Find("Mouth");_leftEye=_head.Find("EyeLeft");_rightEye=_head.Find("EyeRight");
        _leftHand=transform.Find("GloveL");_rightHand=transform.Find("GloveR");_leftArm=transform.Find("ArmL");_rightArm=transform.Find("ArmR");
        if(_mouth!=null)_mouthScale=_mouth.localScale;if(_leftEye!=null)_eyeLScale=_leftEye.localScale;if(_rightEye!=null)_eyeRScale=_rightEye.localScale;
        if(_leftHand!=null)_handLRest=_leftHand.localPosition;if(_rightHand!=null)_handRRest=_rightHand.localPosition;
        if(_leftArm!=null)_armLRest=_leftArm.localRotation;if(_rightArm!=null)_armRRest=_rightArm.localRotation;
    }
    public void Speak(string text,float duration=0){if(string.IsNullOrEmpty(text))return;_line=text;_start=Time.time;_until=_start+(duration>0?duration:Mathf.Clamp(text.Length/24f,1.2f,5));}
    public void Quiet(){_until=0;}
    private void LateUpdate(){
        _weight=Mathf.MoveTowards(_weight,Speaking?1:0,Time.deltaTime*4);float t=Time.time-_start;
        bool pause=false;if(!string.IsNullOrEmpty(_line)){int i=Mathf.Clamp((int)(t*24),0,_line.Length-1);pause=",.!?".IndexOf(_line[i])>=0;}
        float syllable=pause?0:Mathf.Pow(Mathf.Abs(Mathf.Sin(t*13)),2)*_weight;
        _head.localRotation=Quaternion.Euler(Mathf.Sin(t*2.1f)*2*_weight,Mathf.Sin(t*1.2f)*3*_weight,Mathf.Sin(t*.9f)*2.5f*_weight);
        if(_mouth!=null)_mouth.localScale=new Vector3(_mouthScale.x*(1-.08f*syllable),_mouthScale.y*(1+1.3f*syllable),_mouthScale.z);
        float blink=Mathf.Repeat(Time.time,4.7f)<.12f?.25f:1;
        if(_leftEye!=null)_leftEye.localScale=new Vector3(_eyeLScale.x,_eyeLScale.y*blink*(1-.15f*_weight),_eyeLScale.z);
        if(_rightEye!=null)_rightEye.localScale=new Vector3(_eyeRScale.x,_eyeRScale.y*blink*(1+.12f*_weight),_eyeRScale.z);
        float gesture=(.5f+.5f*Mathf.Sin(t*2.5f))*_weight;
        if(_leftHand!=null){_leftHand.localPosition=_handLRest+new Vector3(-.018f,.055f,-.035f)*gesture;_leftHand.localRotation=Quaternion.Euler(-8*gesture,0,-12*gesture);}
        if(_rightHand!=null){_rightHand.localPosition=_handRRest+new Vector3(.018f,.035f,-.025f)*_weight*(1-.5f*gesture);_rightHand.localRotation=Quaternion.Euler(-5*_weight,0,8*gesture);}
        if(_leftArm!=null)_leftArm.localRotation=_armLRest*Quaternion.Euler(-5*gesture,0,-4*gesture);
        if(_rightArm!=null)_rightArm.localRotation=_armRRest*Quaternion.Euler(-3*_weight,0,3*gesture);
    }
}
}
