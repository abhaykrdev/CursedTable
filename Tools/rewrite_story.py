from pathlib import Path
p=Path('Assets/_Project/Scripts/Story/StoryPanel.cs')
p.write_text(r'''using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
namespace HollowSeven {
public sealed class StoryPanel:MonoBehaviour {
    public static readonly string[] DreamCaptions={"Gone. Just like that.","Nothing left to hold on to.","She is sorry. Now.","No... I never wanted...","Who sits with her?","This does not hurt less."};
    public GameManager Game;
    private RectTransform _stage,_far,_near,_moving;private TextMeshProUGUI _caption,_deviceText;private Image _flash;private float _start,_duration;private string _line,_id;private int _visible=-1;
    private readonly List<RectTransform> _rain=new List<RectTransform>();private readonly List<Vector2> _rainOrigins=new List<Vector2>();
    private static Color C(string hex)=>UIFactory.Hex(hex);
    private Image Box(Transform parent,string name,float x,float y,float w,float h,string colour,float alpha=1){var c=C(colour);c.a=alpha;return UIFactory.Panel(parent,name,x,y,w,h,c);}
    private ShapeGraphic Ellipse(Transform parent,string name,float x,float y,float w,float h,string colour,float alpha=1){var c=C(colour);c.a=alpha;return UIFactory.Shape(parent,name,x,y,w,h,ShapeGraphic.Shape.Disc,c);}
    private ShapeGraphic Poly(Transform parent,string name,float x,float y,float w,float h,string colour,params Vector2[] points){var graphic=UIFactory.Shape(parent,name,x,y,w,h,ShapeGraphic.Shape.Polygon,C(colour));graphic.Points=points;graphic.SetAllDirty();return graphic;}
    private Image Line(Transform parent,string name,float x,float y,float w,float h,float angle,string colour,float alpha=1){var b=Box(parent,name,x,y,w,h,colour,alpha);b.rectTransform.localRotation=Quaternion.Euler(0,0,angle);return b;}
    private RectTransform Person(Transform parent,string name,float x,float y,float scale,string coat,bool female=false,bool back=false){
        var r=UIFactory.Rect(parent,name,x,y,200,430);r.localScale=Vector3.one*scale;
        Ellipse(r,"HairSilhouette",0,156,80,female?112:85,"10121C");Ellipse(r,"Face",back?0:12,149,53,68,back?"171922":"917B73");
        if(female){Ellipse(r,"HairLeft",-26,123,37,98,"10121C");Ellipse(r,"HairRight",30,128,26,90,"10121C");}
        Poly(r,"Coat",0,-2,180,290,coat,new Vector2(-.25f,.5f),new Vector2(.25f,.5f),new Vector2(.48f,-.5f),new Vector2(-.48f,-.5f));
        Line(r,"SleeveL",-67,-13,30,208,-7,coat);Line(r,"SleeveR",67,-13,30,208,7,coat);Ellipse(r,"HandL",-79,-122,25,38,"917B73");Ellipse(r,"HandR",79,-122,25,38,"917B73");
        Line(r,"LegL",-34,-207,38,153,-3,"11141C");Line(r,"LegR",32,-207,38,153,3,"11141C");Ellipse(r,"ShoeL",-42,-282,64,25,"090D15");Ellipse(r,"ShoeR",40,-282,64,25,"090D15");
        if(!back){Poly(r,"LapelL",-22,88,48,86,female?"DFB660":"383843",new Vector2(-.5f,.5f),new Vector2(.5f,-.45f),new Vector2(.4f,.5f));Poly(r,"LapelR",22,88,48,86,female?"DFB660":"383843",new Vector2(.5f,.5f),new Vector2(-.5f,-.45f),new Vector2(-.4f,.5f));}
        return r;
    }
    private void Bouquet(Transform parent,float x,float y,float scale){var root=UIFactory.Rect(parent,"Bouquet",x,y,150,180);root.localScale=Vector3.one*scale;Poly(root,"Paper",0,-20,100,170,"B39477",new Vector2(-.5f,.5f),new Vector2(.5f,.5f),new Vector2(0,-.5f));for(int i=0;i<7;i++){float a=i*2.4f;Ellipse(root,"Flower_"+i,Mathf.Cos(a)*35,60+Mathf.Sin(a)*25,35,32,i%2==0?"A84D54":"D29B91");}}
    private void Street(bool dawn=false){Box(_far,"Sky",0,0,1920,850,dawn?"705D69":"111B2C");Ellipse(_far,"Haze",400,160,1000,600,dawn?"C49674":"34475C",.35f);for(int i=0;i<11;i++){float x=-1000+i*200;float h=330+(i*97%250);Box(_far,"Building_"+i,x,270-h/2,170,h,i%2==0?"192333":"202A3B");for(int j=0;j<4;j++)Box(_far,"Window_"+i+"_"+j,x-42+(j%2)*70,150-j/2*90,24,43,"B39878",.25f);}
        Box(_stage,"Road",0,-260,1920,300,"101722");for(int i=0;i<16;i++){float x=-900+i*120;Line(_stage,"WetReflection_"+i,x,-190-(i%4)*32,25+(i%3)*15,2,0,i%3==0?"E6AC6B":"69849C",.4f);}
        Box(_stage,"ShopFront",340,105,680,570,"242535");Box(_stage,"ShopWindow",380,80,530,430,dawn?"E0AD76":"9B704E",.65f);Box(_stage,"WindowMullion",380,80,8,430,"292435");Box(_stage,"WindowBase",380,-145,560,35,"0E1420");
        Box(_near,"LampPost",-700,70,18,650,"10151E");Line(_near,"LampArm",-630,358,150,12,-8,"10151E");Ellipse(_near,"LampHalo",-560,335,230,170,"EDBF7A",.06f);Ellipse(_near,"LampBulb",-560,335,28,18,"F3CCA0");
    }
    private void Rain(){for(int i=0;i<54;i++){float x=-980+(i*173%1960),y=-400+(i*97%800);var line=Line(_near,"Rain_"+i,x,y,1.2f,35+i%4*15,-12,"90B6D0",.17f);_rain.Add(line.rectTransform);_rainOrigins.Add(new Vector2(x,y));}}
    private void RingCloseup(){Box(_far,"Dark",0,0,1920,850,"131522");for(int i=0;i<15;i++)Ellipse(_far,"Bokeh_"+i,-900+i*145,180-Mathf.Sin(i*1.3f)*170,35+i%4*23,35+i%4*23,i%3==0?"C48C5D":"3F5969",.2f);
        var hand=UIFactory.Rect(_stage,"HisLeftHand",-120,-40,850,500);hand.localRotation=Quaternion.Euler(0,0,-12);Poly(hand,"Sleeve",-300,-70,460,360,"202332",new Vector2(-.5f,.5f),new Vector2(.5f,.3f),new Vector2(.5f,-.3f),new Vector2(-.5f,-.5f));Box(hand,"ShirtCuff",-80,-50,65,215,"6E6E7C");Ellipse(hand,"Palm",70,-40,290,210,"9D7C6D");for(int i=0;i<4;i++)Ellipse(hand,"Finger_"+i,210+i*14,28-i*44,220-i*17,43,"A98776");var ring=UIFactory.Shape(hand,"WeddingRing",167,-59,30,48,ShapeGraphic.Shape.Ring,C("E4BB77"),7);Line(hand,"RingGlint",168,-54,3,35,-18,"FFE4B2");
        var bottle=UIFactory.Rect(_near,"Bottle",550,-140,300,480);bottle.localRotation=Quaternion.Euler(0,0,-25);Box(bottle,"GlassBody",0,-65,140,270,"1F3A3D");Ellipse(bottle,"Shoulder",0,70,140,100,"1F3A3D");Box(bottle,"Neck",0,155,49,120,"244648");Box(bottle,"Label",0,-55,110,115,"8D805E");Box(bottle,"Cap",0,220,52,25,"5C675B");Line(bottle,"Reflection",-43,-35,5,220,0,"7D9991",.35f);}
    private void Door(){Box(_far,"StreetWall",0,0,1920,850,"131823");for(int i=-5;i<=5;i++)Box(_far,"StoneSeam_"+i,i*200,0,2,850,"30313A");Box(_stage,"DoorArchShadow",170,25,620,820,"080D16");Ellipse(_stage,"DoorArch",170,270,610,420,"7E6040");Ellipse(_stage,"ArchInset",170,270,570,380,"101722");Box(_stage,"DoorFrame",170,-50,610,640,"7E6040");Box(_stage,"Door",170,-55,560,650,"12121D");Box(_stage,"DoorSlit",310,-55,10,650,"F3CB8B");Box(_stage,"DoorHandle",250,-35,12,100,"D5AD69");Poly(_stage,"LightSpill",390,-330,650,140,"634934",new Vector2(-.5f,.5f),new Vector2(-.46f,.5f),new Vector2(.5f,-.5f),new Vector2(-.3f,-.5f));
        UIFactory.Text(_stage,"NeonTitle","THE HOLLOW",170,250,560,85,52,C("E7BE79"));UIFactory.Text(_stage,"NeonSeven","7",170,122,300,180,156,C("F1C176"));UIFactory.Shape(_stage,"SignRing",170,130,200,200,ShapeGraphic.Shape.Ring,C("B47B43"),2);
        _moving=Person(_near,"HeroAtDoor",-325,-70,1.3f,"20202E",false,true);Rain();}
    private void Ward(string id){Box(_far,"HospitalWall",0,0,1920,850,"26333D");Box(_far,"Window",-300,100,650,500,"101C2A");for(int i=0;i<4;i++)Line(_far,"WindowReflection_"+i,-530+i*160,100,5,500,0,"526773",.5f);Box(_stage,"WallRail",0,-60,1920,18,"56646B");Box(_stage,"BedFrame",270,-220,730,24,"7F8E90");Box(_stage,"BedLegL",-30,-290,16,130,"7F8E90");Box(_stage,"BedLegR",570,-290,16,130,"7F8E90");Ellipse(_stage,"Sheet",270,-163,720,150,"9BABB0");Ellipse(_stage,"Pillow",520,-110,160,75,"BBC4C1");Ellipse(_stage,"HerHead",500,-90,94,55,"A78B82");Ellipse(_stage,"HerHair",515,-82,90,65,"1A2029");Line(_stage,"Arm",300,-114,230,25,-4,"A68A80");
        Box(_stage,"Monitor",760,18,190,155,"131C23");UIFactory.Text(_stage,"MonitorLine","—  /\\  —",760,20,180,100,35,C("78B8A3"));Box(_stage,"MonitorStand",760,-130,12,150,"68787B");
        Box(_near,"ChairBack",-340,-160,150,130,"674F48");Box(_near,"ChairSeat",-340,-239,160,20,"674F48");Line(_near,"ChairLegL",-402,-300,12,120,-5,"574E4C");Line(_near,"ChairLegR",-278,-300,12,120,5,"574E4C");
        if(id.StartsWith("D6")){_moving=Person(_stage,"HisReflection",-300,-12,.75f,"41464F",false,true);var cg=_moving.gameObject.AddComponent<CanvasGroup>();cg.alpha=.65f;}
        if(id.EndsWith("C")){Box(_near,"FlowerTable",-600,-205,160,15,"69605A");Box(_near,"FlowerCup",-600,-150,35,65,"B0B4AA");Bouquet(_near,-600,-90,.35f);}
    }
    public void Hide(){foreach(Transform t in transform){t.gameObject.SetActive(false);Destroy(t.gameObject);}_stage=null;_moving=null;_deviceText=null;_flash=null;_rain.Clear();_rainOrigins.Clear();}
    public void Show(string id,string caption,string heading,float duration){Hide();_id=id;_line=caption;_start=Time.time;_duration=duration;_visible=-1;UIFactory.Font=Game.Font;
        if(id!="P8"){Box(transform,"Backdrop",0,0,1920,1080,"080D16");_stage=UIFactory.Rect(transform,"Stage_"+id,0,0,1920,810);_far=UIFactory.Rect(_stage,"Background",0,0,1920,810);_near=UIFactory.Rect(_stage,"Foreground",0,0,1920,810);
            if(id=="P2"||id=="P1"||id=="C2"){Street();Person(_stage,"She",240,-35,.72f,"BD934E",true);Person(_stage,"OtherMan",430,-28,.79f,"292430",false,true);Bouquet(_stage,325,-27,.5f);Person(_near,"HeroForeground",-650,-130,1.9f,"090E18",false,true);Rain();}
            else if(id=="P3"){RingCloseup();}
            else if(id=="P4"){Door();}
            else if(id=="E2"){Street(true);_moving=Person(_stage,"WalkingAway",-150,-70,1.1f,"292C38",false,true);}
            else if(id.StartsWith("D5")||id.StartsWith("D6")){Ward(id);}
            else if(id.StartsWith("D4")){Street();Box(_stage,"SignalPost",-500,60,15,620,"101822");Box(_stage,"SignalBox",-500,220,80,140,"0A1019");Ellipse(_stage,"RedLight",-500,250,46,46,"DE434F");_moving=Person(_stage,"SheRunning",-150,-65,.85f,"BD934E",true);Rain();if(id=="D4B"){Ellipse(_near,"HeadlightL",-270,-30,400,320,"FFF2D6");Ellipse(_near,"HeadlightR",270,-30,400,320,"FFF2D6");}_flash=Box(_near,"LightFlash",0,0,1920,850,"FFF9EA",0);}
            else if(id=="D2B"||id=="D3B"){Box(_far,"Room",0,0,1920,850,"202333");var phone=Box(_stage,"Device",0,-5,id=="D3B"?370:800,600,"0A1019");UIFactory.Shape(phone.transform,"Bezel",0,0,id=="D3B"?370:800,600,ShapeGraphic.Shape.Frame,C("71635C"),6);_deviceText=UIFactory.Text(phone.transform,"Screen",id=="D3B"?"I am so sorry":"0",0,35,700,280,id=="D3B"?34:180,C("C8C8BE"));UIFactory.Text(phone.transform,"ScreenLabel",id=="D3B"?"UNSENT":"AVAILABLE BALANCE",0,180,650,80,21,C("787F88"));}
            else if(id.StartsWith("D1")){Door();_moving.gameObject.SetActive(false);Person(_stage,"She",300,-60,.78f,"BD934E",true);if(id=="D1A"){_moving=Person(_near,"LeavingMan",-240,-70,.9f,"292430",false,true);Box(_moving,"Suitcase",115,-200,100,130,"563D35");}if(id=="D1C"){Bouquet(_stage,470,-290,.5f);Box(_stage,"Bin",480,-330,120,120,"27323A");}}
            else if(id=="D3A"){Box(_far,"Wall",0,0,1920,850,"242536");var frame=Box(_stage,"Photograph",0,0,610,550,"AF9675");Box(frame.transform,"PhotoPaper",0,0,565,505,"77716D");Person(frame.transform,"HerInPhoto",100,-5,.55f,"AE9158",true);Person(frame.transform,"HimInPhoto",-100,-5,.6f,"282B38");}
            else if(id=="D3C"){Street();Person(_stage,"SheAtWindow",130,-70,1.1f,"BD934E",true,true);Box(_near,"WindowMullion",0,0,12,850,"0A111C");Rain();}
            else {Box(_far,"RoomWall",0,0,1920,850,"2C2730");Person(_stage,"She",80,-25,.85f,"B58A49",true);Box(_near,"Table",0,-180,1100,30,"675042");Box(_near,"TableLegL",-460,-310,30,260,"3D3030");Box(_near,"TableLegR",460,-310,30,260,"3D3030");for(int i=0;i<5;i++)Line(_near,"UnpaidBill_"+i,-200+i*100,-160+i%2*12,160,60,i*7-12,"B8AA93");if(id=="D2C")UIFactory.Shape(_near,"Necklace",0,-135,190,70,ShapeGraphic.Shape.Ring,C("D0AD68"),5);}
            _near.SetAsLastSibling();
        }
        Box(transform,"LetterboxTop",0,484,1920,112,"05080D");Box(transform,"LetterboxBottom",0,-467,1920,146,"05080D");
        if(heading!="")UIFactory.Text(transform,"Heading",heading,0,484,1200,50,20,C("B79565"));
        _caption=UIFactory.Text(transform,"Caption",caption,0,-432,1600,75,38,C("F0E6D4"));_caption.fontStyle=FontStyles.Italic;
        UIFactory.Text(transform,"ContinuePrompt",id.StartsWith("P")?"CLICK TO CONTINUE  ·  HOLD SPACE TO SKIP":"CLICK OR SPACE TO CONTINUE",0,-503,1300,28,16,C("81786E"));
        var hit=UIFactory.Panel(transform,"ContinueInput",0,0,1920,1080,new Color(0,0,0,0));hit.raycastTarget=true;var button=hit.gameObject.AddComponent<Button>();button.targetGraphic=hit;button.transition=UnityEngine.UI.Selectable.Transition.None;button.onClick.AddListener(Game.Continue);
        Game.Audio.SetStory(id);
    }
    private void Update(){if(_caption==null)return;float t=Time.time-_start,u=Mathf.Clamp01(t/_duration);if(_stage!=null){_stage.localScale=Vector3.one*Mathf.Lerp(1,1.035f,u);_far.anchoredPosition=new Vector2(-8*u,0);_near.anchoredPosition=new Vector2(10*u,0);}
        for(int i=0;i<_rain.Count;i++){var p=_rainOrigins[i];_rain[i].anchoredPosition=new Vector2(p.x-25*(t%2),Mathf.Repeat(p.y+430-t*(290+i%7*15),860)-430);}
        if(_moving!=null){if(_id=="D1A")_moving.anchoredPosition=new Vector2(-240-u*230,-70);else if(_id=="D4A")_moving.anchoredPosition=new Vector2(-150+u*430,-65+Mathf.Sin(t*12)*6);else if(_id=="P4")_moving.anchoredPosition=new Vector2(-325+u*55,-70+Mathf.Sin(t*5)*2);else if(_id=="D6B")_moving.anchoredPosition=new Vector2(-300,-12-u*70);else if(_id=="E2")_moving.localScale=Vector3.one*Mathf.Lerp(1.1f,.7f,u);}
        if(_flash!=null){var c=_flash.color;c.a=_id=="D4C"?1-u:_id=="D4B"?u*u:0;_flash.color=c;if(_id=="D4C")_flash.color=Color.Lerp(Color.white,Color.black,u);}
        if(_deviceText!=null&&_id=="D3B")_deviceText.maxVisibleCharacters=t<2?Mathf.FloorToInt(t*9):Mathf.Max(0,13-Mathf.FloorToInt((t-2)*8));
        int count=Mathf.Min(_line.Length,Mathf.Max(0,(int)((t-.15f)*42)));if(count!=_visible){_visible=count;_caption.maxVisibleCharacters=count;}
    }
}
}
''',encoding='utf-8')
