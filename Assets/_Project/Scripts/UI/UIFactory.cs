using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
namespace HollowSeven { public static class UIFactory {
    public static readonly Color Ink=Hex("0B090F"),Bone=Hex("EDE6D6"),Brass=Hex("B08D3C"),Gold=Hex("E8C26A"),Red=Hex("E0243C"),Green=Hex("3DDC84");
    public static TMP_FontAsset Font;
    public static Action<SfxId> Sound;
    public static Color Hex(string s){ColorUtility.TryParseHtmlString("#"+s,out var c);return c;}
    public static RectTransform Rect(Transform parent,string name,float x,float y,float w,float h){var go=new GameObject(name,typeof(RectTransform));go.transform.SetParent(parent,false);var r=(RectTransform)go.transform;r.anchorMin=r.anchorMax=new Vector2(.5f,.5f);r.sizeDelta=new Vector2(w,h);r.anchoredPosition=new Vector2(x,y);return r;}
    public static Image Panel(Transform parent,string name,float x,float y,float w,float h,Color colour){var r=Rect(parent,name,x,y,w,h);var im=r.gameObject.AddComponent<Image>();im.color=colour;im.raycastTarget=false;return im;}
    public static ShapeGraphic Shape(Transform parent,string name,float x,float y,float w,float h,ShapeGraphic.Shape shape,Color colour,float thickness=3){var r=Rect(parent,name,x,y,w,h);var im=r.gameObject.AddComponent<ShapeGraphic>();im.Kind=shape;im.Thickness=thickness;im.color=colour;im.raycastTarget=false;return im;}
    public static TextMeshProUGUI Text(Transform parent,string name,string text,float x,float y,float w,float h,float size,Color colour,TextAlignmentOptions align=TextAlignmentOptions.Center){var r=Rect(parent,name,x,y,w,h);var t=r.gameObject.AddComponent<TextMeshProUGUI>();t.font=Font;t.text=text;t.fontSize=size;t.color=colour;t.alignment=align;t.raycastTarget=false;t.textWrappingMode=TextWrappingModes.Normal;return t;}
    public static Button Button(Transform parent,string name,string label,float x,float y,float w,float h,Action action){var im=Panel(parent,name,x,y,w,h,new Color(.07f,.045f,.06f,.94f));im.raycastTarget=true;var b=im.gameObject.AddComponent<Button>();b.targetGraphic=im;im.gameObject.AddComponent<ButtonFeel>();var colors=b.colors;colors.normalColor=Color.white;colors.highlightedColor=new Color(1.6f,1.45f,1.2f);colors.pressedColor=Gold;colors.selectedColor=Color.white;b.colors=colors;Shape(im.transform,"Border",0,0,w,h,ShapeGraphic.Shape.Frame,Brass,2);Text(im.transform,"Label",label,0,0,w-16,h-6,30,Bone);if(action!=null)b.onClick.AddListener(()=>action());return b;}
    public static void Clear(Transform parent){for(int i=parent.childCount-1;i>=0;i--)UnityEngine.Object.Destroy(parent.GetChild(i).gameObject);}
} }
