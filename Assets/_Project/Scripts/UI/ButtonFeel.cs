using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
namespace HollowSeven { public sealed class ButtonFeel:MonoBehaviour,IPointerEnterHandler,IPointerExitHandler,IPointerDownHandler,IPointerUpHandler {
    private Button _button;private Coroutine _animation;
    private void Awake(){_button=GetComponent<Button>();_button.onClick.AddListener(ClickSound);}
    private void ClickSound(){if(GetComponentInParent<HudController>()==null)UIFactory.Sound?.Invoke(SfxId.SFX_UIClick);}
    private void Scale(float target){if(_button==null||!_button.interactable)return;if(_animation!=null)StopCoroutine(_animation);Vector3 start=transform.localScale;_animation=Tween.Run(this,.12f,t=>transform.localScale=Vector3.LerpUnclamped(start,Vector3.one*target,t),Tween.Easing.EaseOutQuad);}
    public void OnPointerEnter(PointerEventData e){if(_button==null||!_button.IsActive()||!_button.interactable)return;Scale(1.04f);UIFactory.Sound?.Invoke(SfxId.SFX_UIHover);}public void OnPointerExit(PointerEventData e)=>Scale(1);public void OnPointerDown(PointerEventData e)=>Scale(.96f);public void OnPointerUp(PointerEventData e)=>Scale(1.04f);
} }
