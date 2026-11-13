using System;
using System.Collections;
using UnityEngine;
namespace HollowSeven { public static class Tween {
    public enum Easing {Linear,EaseOutQuad,EaseInQuad,EaseInOutSine,EaseOutBack,EaseOutCubic,EaseInCubic}
    public static float Ease(Easing e,float t){switch(e){case Easing.EaseOutQuad:return 1-(1-t)*(1-t);case Easing.EaseInQuad:return t*t;case Easing.EaseInOutSine:return -(Mathf.Cos(Mathf.PI*t)-1)/2;case Easing.EaseOutBack:float u=t-1;return 1+2.70158f*u*u*u+1.70158f*u*u;case Easing.EaseOutCubic:return 1-Mathf.Pow(1-t,3);case Easing.EaseInCubic:return t*t*t;default:return t;}}
    public static Coroutine Run(MonoBehaviour owner,float duration,Action<float> onUpdate,Easing easing=Easing.Linear,Action onDone=null,bool unscaled=true)=>owner.StartCoroutine(Animate(duration,onUpdate,easing,onDone,unscaled));
    private static IEnumerator Animate(float seconds,Action<float> update,Easing easing,Action done,bool unscaled){for(float t=0;t<seconds;t+=unscaled?Time.unscaledDeltaTime:Time.deltaTime){update(Ease(easing,t/seconds));yield return null;}update(1);done?.Invoke();}
} }
