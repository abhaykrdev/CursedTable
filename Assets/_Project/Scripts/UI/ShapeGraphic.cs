using UnityEngine;
using UnityEngine.UI;
namespace HollowSeven {
// Native vector geometry: every ring and chip remains an editable UI object.
[RequireComponent(typeof(CanvasRenderer))]
public sealed class ShapeGraphic:MaskableGraphic {
    public enum Shape {Disc,Ring,Frame,Diamond,Polygon}
    public Vector2[] Points;
    [SerializeField] private Shape _shape;[SerializeField] private float _thickness=3;[SerializeField,Range(0,1)] private float _fill=1;
    public Shape Kind{get=>_shape;set{_shape=value;SetVerticesDirty();}}public float Thickness{get=>_thickness;set{_thickness=value;SetVerticesDirty();}}public float Fill{get=>_fill;set{_fill=value;SetVerticesDirty();}}
    protected override void OnPopulateMesh(VertexHelper vh){vh.Clear();var r=GetPixelAdjustedRect();var c=r.center;
        if(_shape==Shape.Polygon&&Points!=null&&Points.Length>=3){vh.AddVert(c,color,Vector2.zero);for(int i=0;i<Points.Length;i++)vh.AddVert(c+Vector2.Scale(Points[i],r.size),color,Vector2.zero);for(int i=0;i<Points.Length;i++)vh.AddTriangle(0,i+1,(i+1)%Points.Length+1);return;}
        if(_shape==Shape.Frame){Quad(vh,new Vector2(r.xMin,r.yMin),new Vector2(r.xMax,r.yMin+_thickness));Quad(vh,new Vector2(r.xMin,r.yMax-_thickness),new Vector2(r.xMax,r.yMax));Quad(vh,new Vector2(r.xMin,r.yMin),new Vector2(r.xMin+_thickness,r.yMax));Quad(vh,new Vector2(r.xMax-_thickness,r.yMin),new Vector2(r.xMax,r.yMax));return;}
        int segments=_shape==Shape.Diamond?4:96;float rx=r.width/2,ry=r.height/2;
        for(int i=0;i<segments;i++){float a=(90-i*360f/segments*_fill)*Mathf.Deg2Rad,b=(90-(i+1)*360f/segments*_fill)*Mathf.Deg2Rad;int start=vh.currentVertCount;
            var p=new Vector2(Mathf.Cos(a),Mathf.Sin(a));var q=new Vector2(Mathf.Cos(b),Mathf.Sin(b));
            vh.AddVert(c+new Vector2(p.x*rx,p.y*ry),color,Vector2.zero);vh.AddVert(c+new Vector2(q.x*rx,q.y*ry),color,Vector2.zero);
            if(_shape==Shape.Ring){vh.AddVert(c+new Vector2(q.x*(rx-_thickness),q.y*(ry-_thickness)),color,Vector2.zero);vh.AddVert(c+new Vector2(p.x*(rx-_thickness),p.y*(ry-_thickness)),color,Vector2.zero);vh.AddTriangle(start,start+1,start+2);vh.AddTriangle(start,start+2,start+3);}else{vh.AddVert(c,color,Vector2.zero);vh.AddTriangle(start,start+1,start+2);}
        }
    }
    private void Quad(VertexHelper vh,Vector2 a,Vector2 b){int n=vh.currentVertCount;vh.AddVert(a,color,Vector2.zero);vh.AddVert(new Vector2(a.x,b.y),color,Vector2.zero);vh.AddVert(b,color,Vector2.zero);vh.AddVert(new Vector2(b.x,a.y),color,Vector2.zero);vh.AddTriangle(n,n+1,n+2);vh.AddTriangle(n,n+2,n+3);}
}
}
