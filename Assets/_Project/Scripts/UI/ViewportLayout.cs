using UnityEngine;
using UnityEngine.UI;
namespace HollowSeven { public sealed class ViewportLayout:MonoBehaviour {
    public Camera Camera;public CanvasScaler[] Canvases;private int _width,_height;private GameObject _bars;private RectTransform[] _edges;
    private void Awake(){_bars=new GameObject("Canvas_Letterbox",typeof(RectTransform),typeof(Canvas));var canvas=_bars.GetComponent<Canvas>();canvas.renderMode=RenderMode.ScreenSpaceOverlay;canvas.sortingOrder=100;_edges=new RectTransform[4];for(int i=0;i<4;i++)_edges[i]=UIFactory.Panel(_bars.transform,"Bar_"+i,0,0,1,1,Color.black).rectTransform;}
    private void OnDestroy(){if(_bars!=null)Destroy(_bars);}
    private void LateUpdate(){if(Screen.width==_width&&Screen.height==_height)return;_width=Screen.width;_height=Screen.height;float scale=Mathf.Min(_width/1920f,_height/1080f);foreach(var canvas in Canvases){canvas.uiScaleMode=CanvasScaler.ScaleMode.ConstantPixelSize;canvas.scaleFactor=scale;}float w=1920*scale/_width,h=1080*scale/_height;Camera.rect=new Rect((1-w)/2,(1-h)/2,w,h);float vertical=(_height-1080*scale)/2,horizontal=(_width-1920*scale)/2;for(int i=0;i<4;i++){bool top=i<2;_edges[i].sizeDelta=top?new Vector2(_width,vertical):new Vector2(horizontal,_height);_edges[i].anchoredPosition=top?new Vector2(0,(i==0?1:-1)*(_height-vertical)/2):new Vector2((i==2?1:-1)*(_width-horizontal)/2,0);}}
} }
