using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
namespace HollowSeven {
public sealed class StoryPanel:MonoBehaviour {
    public static readonly string[] DreamCaptions={"Gone. Just like that.","Nothing left to hold on to.","She is sorry. Now.","No... I never wanted...","Who sits with her?","This does not hurt less."};
    public static string DreamCaption(int beat,int panel){
        string[,] lines={
            {"She thought he would stay. He already had his bag by the door.","He left without looking back. Their plans went with him.","The flowers went into the bin. Her phone stayed in her hand."},
            {"The rent was due. He had promised to pay half.","She checked the account again. Zero. The number did not change.","She set her necklace beside the bills. One more thing to sell."},
            {"She found an old photograph of us. Before the silence. Before him.","'I am so sorry.' She typed it, then deleted it. Again.","I had wanted her to regret leaving. I had never imagined her this alone."},
            {"She stepped into the rain, holding her phone. My name was on the screen.","Headlights. A horn. The memory would not let me look away.","The table went silent. This was never what I wanted."},
            {"A hospital room. Her coat folded neatly at the foot of the bed.","The chair beside her was empty. No boyfriend. No visitors.","I could win another hand. I could not reach through the glass."},
            {"There was someone in the window now. My reflection, beside her bed.","The house had kept me watching. I had mistaken watching for caring.","I could stay here chasing answers. Or leave, and decide what to do with them."}
        };return lines[Mathf.Clamp(beat-1,0,5),Mathf.Clamp(panel,0,2)];
    }
    public GameManager Game;
    private RectTransform _stage,_far,_near,_moving,_wifePose,_doorPanel,_heroPortrait,_heroMouth,_heroBrowL,_heroBrowR;private TextMeshProUGUI _caption,_deviceText;private Image _flash;private float _start,_duration;private string _line,_id;private int _visible=-1;private bool _reveal;private TextMeshProUGUI _prompt;
    public bool TextComplete=>_reveal||_visible>=(_line==null?0:_line.Length);
    public void RevealText(){_reveal=true;if(_caption!=null)_caption.maxVisibleCharacters=int.MaxValue;}
    private readonly List<RectTransform> _rain=new List<RectTransform>();private readonly List<Vector2> _rainOrigins=new List<Vector2>();
    private static Color C(string hex)=>UIFactory.Hex(hex);
    private Image Box(Transform parent,string name,float x,float y,float w,float h,string colour,float alpha=1){var c=C(colour);c.a=alpha;return UIFactory.Panel(parent,name,x,y,w,h,c);}
    private ShapeGraphic Ellipse(Transform parent,string name,float x,float y,float w,float h,string colour,float alpha=1){var c=C(colour);c.a=alpha;return UIFactory.Shape(parent,name,x,y,w,h,ShapeGraphic.Shape.Disc,c);}
    private ShapeGraphic Poly(Transform parent,string name,float x,float y,float w,float h,string colour,params Vector2[] points){var graphic=UIFactory.Shape(parent,name,x,y,w,h,ShapeGraphic.Shape.Polygon,C(colour));graphic.Points=points;graphic.SetAllDirty();return graphic;}
    private Image Line(Transform parent,string name,float x,float y,float w,float h,float angle,string colour,float alpha=1){var b=Box(parent,name,x,y,w,h,colour,alpha);b.rectTransform.localRotation=Quaternion.Euler(0,0,angle);return b;}
    private RectTransform Person(Transform parent,string name,float x,float y,float scale,string coat,bool female=false,bool back=false){
        var r=UIFactory.Rect(parent,name,x,y,200,500);r.localScale=Vector3.one*scale;
        bool boyfriend=name.Contains("Boyfriend")||name.Contains("OtherMan")||name.Contains("LeavingMan");
        string skin=female?"BA9380":boyfriend?"B58E77":"A88876",shade=female?"927263":"78635B",hair=boyfriend?"3C302E":"171D28";
        Ellipse(r,"GroundShadow",0,-284,160,18,"0A111C",.35f);

        Poly(r,"LegL",-25,-204,45,164,"1B2330",new Vector2(-.46f,.5f),new Vector2(.5f,.5f),new Vector2(.32f,-.5f),new Vector2(-.28f,-.5f));
        Poly(r,"LegR",26,-204,45,164,"111A26",new Vector2(-.5f,.5f),new Vector2(.46f,.5f),new Vector2(.28f,-.5f),new Vector2(-.32f,-.5f));
        Line(r,"TrouserCreaseL",-25,-205,1.5f,129,0,"445061",.4f);Line(r,"TrouserCreaseR",26,-205,1.5f,129,0,"445061",.25f);
        Ellipse(r,"ShoeL",-32,-282,59,20,"101722");Ellipse(r,"ShoeR",34,-282,59,20,"101722");
        Box(r,"Neck",0,123,28,45,skin);
        Poly(r,"Coat",0,-9,158,245,coat,new Vector2(-.5f,.32f),new Vector2(-.3f,.5f),new Vector2(.3f,.5f),new Vector2(.5f,.32f),new Vector2(.35f,-.5f),new Vector2(-.35f,-.5f));
        Poly(r,"CoatShadow",-35,-22,62,220,shade,new Vector2(-.45f,.35f),new Vector2(.45f,.5f),new Vector2(.22f,-.5f),new Vector2(-.38f,-.5f)).color=new Color(.05f,.07f,.11f,.22f);
        Line(r,"SleeveL",-69,40,29,120,-9,coat);Line(r,"SleeveR",69,40,29,120,9,coat);
        Line(r,"ForearmL",-78,-58,25,104,-2,coat);Line(r,"ForearmR",78,-58,25,104,2,coat);
        Box(r,"CuffL",-80,-109,24,10,"A09A8F");Box(r,"CuffR",80,-109,24,10,"A09A8F");
        Ellipse(r,"HandL",-80,-128,20,31,skin);Ellipse(r,"HandR",80,-128,20,31,skin);
        Line(r,"FingerL",-83,-133,1,13,0,shade);Line(r,"FingerR",83,-133,1,13,0,shade);
        if(!back){
            Poly(r,"Shirt",0,82,54,86,"C7BCA9",new Vector2(-.5f,.5f),new Vector2(.5f,.5f),new Vector2(.15f,-.5f),new Vector2(-.15f,-.5f));
            Poly(r,"LapelL",-28,71,45,99,female?"D3AF65":"465061",new Vector2(-.5f,.5f),new Vector2(.3f,.5f),new Vector2(.5f,-.5f),new Vector2(-.1f,-.1f));
            Poly(r,"LapelR",28,71,45,99,female?"AD8449":"303B4E",new Vector2(.5f,.5f),new Vector2(-.3f,.5f),new Vector2(-.5f,-.5f),new Vector2(.1f,-.1f));
            if(boyfriend)Line(r,"Tie",0,69,10,63,0,"633B3B");
            for(int i=0;i<3;i++)Ellipse(r,"Button_"+i,6,5-i*32,4,4,"BBA382");
            Line(r,"Pocket",-37,-48,32,2,-3,"7D746B",.6f);
        }else{Line(r,"BackSeam",0,-12,1,194,0,"78818B",.2f);Line(r,"ShoulderSeam",0,97,87,2,0,"78818B",.2f);}
        Ellipse(r,"HairSilhouette",0,166,78,female?106:87,hair);
        if(!back){
            Ellipse(r,"EarL",-31,159,12,20,shade);Ellipse(r,"EarR",31,159,12,20,skin);
            Poly(r,"Face",0,164,63,81,skin,new Vector2(-.38f,.5f),new Vector2(.38f,.5f),new Vector2(.5f,.15f),new Vector2(.34f,-.35f),new Vector2(.12f,-.5f),new Vector2(-.12f,-.5f),new Vector2(-.34f,-.35f),new Vector2(-.5f,.15f));
            Ellipse(r,"FaceShade",-14,163,23,59,shade,.42f);
            Ellipse(r,"EyeL",-13,172,13,5,"DACDB7");Ellipse(r,"EyeR",13,172,13,5,"DACDB7");Ellipse(r,"PupilL",-10,172,4,5,"24252B");Ellipse(r,"PupilR",16,172,4,5,"24252B");
            Line(r,"BrowL",-13,180,17,2.8f,female?-7:5,hair);Line(r,"BrowR",13,180,17,2.8f,female?7:-5,hair);
            Line(r,"Nose",2,160,3,15,-5,shade);Line(r,"Nostril",5,153,7,2,0,shade);
            Line(r,"Mouth",1,144,18,2.5f,female?2:0,"6E5050");Line(r,"LipHighlight",1,142,12,1,0,"CDA58E",.5f);
            Ellipse(r,"HairCrown",-4,199,71,28,hair);Poly(r,"HairPart",-18,188,44,34,hair,new Vector2(-.5f,.5f),new Vector2(.5f,.5f),new Vector2(-.4f,-.5f));
            if(female){Ellipse(r,"HairLeft",-32,158,19,83,hair);Ellipse(r,"HairRight",32,162,14,76,hair);Line(r,"HairHighlight",-34,170,2,48,-2,"51505B",.45f);}
        }else{Ellipse(r,"Face",0,166,62,70,hair);Line(r,"HairRim",27,174,2,37,8,"526075",.5f);if(female){Ellipse(r,"HairLeft",-28,132,28,105,hair);Ellipse(r,"HairRight",28,132,28,105,hair);}}
        return r;
    }
    private void HomeDeparture(){
        Box(_far,"HomeWall",0,0,1920,850,"39313A");Box(_stage,"HomeFloor",0,-295,1920,250,"201D29");Box(_stage,"Skirting",0,-175,1920,17,"8D7963");
        Box(_stage,"DoorFrame",615,40,350,650,"9C866E");Box(_stage,"DoorOpen",615,40,318,620,"171B28");
        Box(_stage,"WindowFrame",-590,135,360,400,"8C7B68");Box(_stage,"Window",-590,135,330,370,"293C50");Box(_stage,"WindowBar",-590,135,8,370,"897762");
        Box(_stage,"Sofa",-570,-112,480,150,"5C4857");Box(_stage,"SofaSeat",-570,-175,470,38,"725967");
        var frame=Box(_stage,"WeddingPhotoFrame",0,255,150,150,"AA8D68");Box(frame.transform,"PhotoPaper",0,0,132,132,"D0BCA0");Person(frame.transform,"WeddingHero",-28,-5,.16f,"293343");Person(frame.transform,"WeddingWife",28,-5,.16f,"D8CFBC",true);
        Person(_stage,"Husband",-175,-30,1.1f,"263447");_wifePose=Person(_near,"WifeLeaving",350,-30,1.05f,"BD934E",true);
        Box(_near,"HerSuitcase",510,-246,115,160,"6B4D42");UIFactory.Shape(_near,"SuitcaseHandle",510,-160,53,27,ShapeGraphic.Shape.Frame,C("BD9E76"),5);
        for(int i=0;i<7;i++)Line(_stage,"Floorboard_"+i,-900+i*300,-300,2,240,-12+i*4,"55424B");
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
    private void RearAlley(){
        Box(_far,"NightSky",0,0,1920,850,"101C2B");
        for(int i=0;i<7;i++){float x=-960+i*260;Box(_far,"DistantRoof_"+i,x,220,230,250+(i%3)*60,"172232");for(int n=0;n<3;n++)Box(_far,"DistantWindow_"+i+"_"+n,x-65+n*60,280,20,38,"C29159",.18f);}
        Poly(_stage,"CasinoRearWall",80,0,1740,760,"26303B",new Vector2(-.5f,.42f),new Vector2(.5f,.5f),new Vector2(.5f,-.5f),new Vector2(-.5f,-.5f));
        for(int row=0;row<11;row++){float y=-340+row*65;Line(_stage,"Mortar_"+row,70,y,1720,1,0,"47505B",.35f);for(int brick=0;brick<13;brick++)Line(_stage,"Joint_"+row+"_"+brick,-780+brick*140+row%2*70,y+32,1,64,0,"47505B",.25f);}
        Box(_stage,"ServiceDoorFrame",370,-10,350,580,"8F714B");Box(_stage,"ServiceDoor",370,-14,326,562,"111824");Box(_stage,"LightUnderDoor",370,-292,326,4,"F4BD79");Line(_stage,"DoorLightLeak",530,-15,3,556,0,"E8B57B",.7f);Box(_stage,"Handle",485,-55,9,66,"C6A578");
        UIFactory.Text(_stage,"CasinoBackSign","CURSED TABLE",325,342,920,100,57,C("DBB27C"));UIFactory.Text(_stage,"RearEntranceSign","R E A R   E N T R A N C E",370,235,420,52,17,C("9B927E"));
        Line(_stage,"ServicePipe",-635,15,18,720,0,"171F2B");Line(_stage,"PipeClamp",-635,220,34,10,0,"5C6468");Line(_stage,"PipeClampLower",-635,-130,34,10,0,"5C6468");
        Box(_near,"Ground",0,-360,1920,110,"101B28");
        Poly(_stage,"DoorGlowOnPaving",460,-345,650,100,"70513E",new Vector2(-.38f,.5f),new Vector2(.1f,.5f),new Vector2(.5f,-.5f),new Vector2(-.5f,-.5f));
        for(int i=0;i<14;i++)Line(_near,"PuddleReflection_"+i,-860+i*130,-335-i%3*20,35+i%4*17,2,-2,i%3==0?"C49466":"577283",.45f);
        Box(_stage,"Bin",-760,-210,220,235,"182431");Line(_stage,"BinLid",-760,-88,245,15,-4,"40505C");
        Box(_stage,"LampBracket",625,286,100,14,"151D29");Ellipse(_stage,"AmberHalo",625,213,340,340,"E7AF72",.045f);Ellipse(_stage,"InnerHalo",625,213,160,160,"E7AF72",.07f);Box(_stage,"LampGlass",625,248,44,59,"D7AA76");Box(_stage,"LampCap",625,283,66,12,"111B28");
        _moving=Person(_near,"HeroBehindCasino",-240,-72,1.02f,"202B3B",false,true);
        Poly(_moving,"CoatLightEdge",78,-5,15,290,"566173",new Vector2(-.5f,.5f),new Vector2(.5f,-.5f),new Vector2(.1f,-.5f));
        Line(_moving,"WeddingRingGlint",-80,-116,3,9,-10,"E7C995");Rain();
    }
    private void Breakup(string id){
        Box(_far,"ApartmentWall",0,0,1920,850,"292B3B");
        for(int i=-8;i<=8;i++)Line(_far,"WallpaperSeam_"+i,i*120,20,1,800,0,"55525B",.18f);
        Box(_stage,"Skirting",0,-230,1920,18,"88735F");Box(_stage,"Floor",0,-330,1920,180,"191D2A");
        for(int i=-8;i<=8;i++)Line(_stage,"Floorboard_"+i,i*180,-330,2,180,-i*6,"3A3640");
        Box(_stage,"WindowFrame",-585,120,380,410,"817362");Box(_stage,"NightWindow",-585,120,352,382,"132033");
        for(int i=0;i<5;i++){Box(_stage,"OutsideBuilding_"+i,-730+i*72,100,60,210+i%3*35,"1C2D42");Box(_stage,"OutsideLight_"+i,-720+i*72,130,9,16,"B88D60",.6f);}
        Box(_stage,"WindowBar",-585,120,8,382,"3D3C45");Box(_stage,"Windowsill",-585,-88,410,14,"8B7968");
        Box(_stage,"CurtainL",-797,125,70,450,"514251");Box(_stage,"CurtainR",-375,125,70,450,"514251");
        Box(_stage,"DoorFrame",610,45,345,590,"88715B");Box(_stage,"Doorway",610,45,316,567,id=="F2"?"B59470":"171C28");
        _doorPanel=Box(_stage,"ApartmentDoor",610,45,310,560,"34303B").rectTransform;
        Box(_doorPanel,"DoorInset",0,35,250,340,"3D3843");Box(_doorPanel,"DoorHandle",-120,-28,10,52,"C6A271");
        Box(_stage,"SofaBase",-580,-180,490,100,"493C4B");Ellipse(_stage,"SofaBack",-580,-88,485,170,"5C4859");Box(_stage,"SofaCushion",-580,-169,420,40,"705567");
        Box(_stage,"SideTable",-20,-220,260,18,"79614F");Box(_stage,"TableLeg",-20,-305,18,170,"453A37");
        Box(_stage,"LampStem",-80,-136,6,145,"9F8565");Poly(_stage,"LampShade",-80,-53,130,110,"C5A783",new Vector2(-.3f,.5f),new Vector2(.3f,.5f),new Vector2(.5f,-.5f),new Vector2(-.5f,-.5f));Ellipse(_far,"WarmPool",-80,-50,660,560,"C19769",.07f);
        _wifePose=Person(_stage,"Wife",id=="F3"?85:-150,-20,1.08f,"BD934E",true);
        if(id!="F3"){
            _moving=Person(_near,"Boyfriend",345,-20,1.1f,"333544",false,id=="F2");Box(_moving,"PackedSuitcase",120,-214,102,132,"735847");UIFactory.Shape(_moving,"CaseHandle",120,-142,48,25,ShapeGraphic.Shape.Frame,C("B39576"),4);
            if(id=="F1"){var arm=_wifePose.Find("SleeveR");arm.localRotation=Quaternion.Euler(0,0,22);var forearm=_wifePose.Find("ForearmR");forearm.localRotation=Quaternion.Euler(0,0,55);forearm.localPosition=new Vector3(112,-42,0);_wifePose.Find("CuffR").localPosition=new Vector3(153,-70,0);_wifePose.Find("HandR").localPosition=new Vector3(164,-77,0);}
        }else{
            var phone=Box(_wifePose,"PhoneInHand",78,-100,32,57,"101722");Box(phone.transform,"PhoneScreen",0,0,25,46,"84A1B3");Ellipse(_wifePose,"PhoneGlow",78,-92,100,120,"B5D0E0",.08f);
            _wifePose.Find("Face").localRotation=Quaternion.Euler(0,0,-8);Bouquet(_near,395,-318,.42f);
            UIFactory.Text(_near,"UnsentContact","YOUR NAME.\nNO MESSAGE.",440,-100,390,120,25,C("A4ABB6"));
        }
        for(int i=0;i<12;i++)Line(_stage,"WindowRain_"+i,-748+i*29,90+(i%4)*57,1,38,-8,"8197AB",.3f);
    }
    private void HeroLaugh(){
        Box(_far,"CasinoDark",0,0,1920,850,"0A121B");
        for(int i=-4;i<=4;i++){Box(_far,"CasinoPanel_"+i,i*260,0,210,810,"18212C");Line(_far,"GoldTrim_"+i,i*260-104,0,2,810,0,"75624A",.35f);}
        for(int i=0;i<8;i++)Ellipse(_far,"AmberBokeh_"+i,-900+i*260,250-i%3*100,25+i%4*20,25+i%4*20,"C29561",.16f);
        _heroPortrait=UIFactory.Rect(_stage,"HeroCloseup",60,-20,850,850);
        Poly(_heroPortrait,"Shoulders",0,-310,1100,330,"202B3B",new Vector2(-.5f,-.5f),new Vector2(-.3f,.25f),new Vector2(-.12f,.5f),new Vector2(.12f,.5f),new Vector2(.3f,.25f),new Vector2(.5f,-.5f));
        Box(_heroPortrait,"Neck",0,-133,140,190,"967765");
        Poly(_heroPortrait,"Shirt",0,-260,270,250,"B3ABA0",new Vector2(-.5f,.5f),new Vector2(0,.18f),new Vector2(.5f,.5f),new Vector2(.3f,-.5f),new Vector2(-.3f,-.5f));
        Poly(_heroPortrait,"LapelL",-185,-265,210,250,"334052",new Vector2(-.5f,.3f),new Vector2(.25f,.5f),new Vector2(.5f,-.5f));
        Poly(_heroPortrait,"LapelR",185,-265,210,250,"283447",new Vector2(.5f,.3f),new Vector2(-.25f,.5f),new Vector2(-.5f,-.5f));
        Ellipse(_heroPortrait,"EarL",-171,85,55,109,"967765");Ellipse(_heroPortrait,"EarR",171,85,55,109,"B39377");
        Poly(_heroPortrait,"Face",0,85,350,440,"B39377",new Vector2(-.43f,.5f),new Vector2(.43f,.5f),new Vector2(.5f,.15f),new Vector2(.4f,-.3f),new Vector2(.13f,-.5f),new Vector2(-.13f,-.5f),new Vector2(-.4f,-.3f),new Vector2(-.5f,.15f));
        Poly(_heroPortrait,"FaceShadow",-78,83,180,440,"7C665F",new Vector2(-.4f,.5f),new Vector2(.5f,.5f),new Vector2(.1f,.1f),new Vector2(.3f,-.28f),new Vector2(.5f,-.5f),new Vector2(-.25f,-.28f),new Vector2(-.5f,.15f));
        Ellipse(_heroPortrait,"CheekWarmth",95,53,68,95,"B99A82",.18f);Ellipse(_heroPortrait,"UnderEyeL",-79,96,92,17,"6F6260",.3f);Ellipse(_heroPortrait,"UnderEyeR",79,96,92,17,"8E7262",.25f);
        Poly(_heroPortrait,"Hair",0,270,395,180,"111824",new Vector2(-.5f,-.4f),new Vector2(-.47f,.25f),new Vector2(-.1f,.5f),new Vector2(.4f,.3f),new Vector2(.5f,-.5f),new Vector2(.15f,-.08f),new Vector2(-.32f,-.3f));
        Ellipse(_heroPortrait,"EyeSocketL",-80,120,106,42,"514B4E");Ellipse(_heroPortrait,"EyeSocketR",80,120,106,42,"66534D");
        Ellipse(_heroPortrait,"EyeL",-80,118,78,17,"D0C4A4");Ellipse(_heroPortrait,"EyeR",80,118,78,17,"E1D1AB");Ellipse(_heroPortrait,"PupilL",-65,118,15,20,"111824");Ellipse(_heroPortrait,"PupilR",65,118,15,20,"111824");
        _heroBrowL=Line(_heroPortrait,"BrowL",-82,150,109,14,-12,"171C27").rectTransform;_heroBrowR=Line(_heroPortrait,"BrowR",82,150,109,14,12,"171C27").rectTransform;
        Poly(_heroPortrait,"NoseShadow",0,42,54,118,"735C54",new Vector2(.05f,.5f),new Vector2(.5f,-.38f),new Vector2(-.5f,-.5f));
        Line(_heroPortrait,"CheekL",-117,16,3,34,-27,"765F58");Line(_heroPortrait,"CheekR",117,16,2,34,27,"95705D");
        _heroMouth=UIFactory.Rect(_heroPortrait,"LaughingMouth",0,-57,174,125);Ellipse(_heroMouth,"MouthOpening",0,0,174,125,"201B28");
        Poly(_heroMouth,"UpperTeeth",0,36,150,32,"E2D8C0",new Vector2(-.5f,.3f),new Vector2(.5f,.3f),new Vector2(.35f,-.5f),new Vector2(-.35f,-.5f));
        Ellipse(_heroMouth,"LowerTeeth",0,-40,93,13,"BAAD9A");
        _heroMouth.localScale=new Vector3(1,.1f,1);
        Line(_heroPortrait,"SmileCreaseL",-108,-44,6,40,-30,"67514D");Line(_heroPortrait,"SmileCreaseR",108,-44,6,40,30,"87664F");
        Box(_near,"FeltEdge",0,-405,1920,80,"17372F");
    }
    private void Ward(string id){Box(_far,"HospitalWall",0,0,1920,850,"26333D");Box(_far,"Window",-300,100,650,500,"101C2A");for(int i=0;i<4;i++)Line(_far,"WindowReflection_"+i,-530+i*160,100,5,500,0,"526773",.5f);Box(_stage,"WallRail",0,-60,1920,18,"56646B");Box(_stage,"BedFrame",270,-220,730,24,"7F8E90");Box(_stage,"BedLegL",-30,-290,16,130,"7F8E90");Box(_stage,"BedLegR",570,-290,16,130,"7F8E90");Ellipse(_stage,"Sheet",270,-163,720,150,"9BABB0");Ellipse(_stage,"Pillow",520,-110,160,75,"BBC4C1");Ellipse(_stage,"HerHead",500,-90,94,55,"A78B82");Ellipse(_stage,"HerHair",515,-82,90,65,"1A2029");Line(_stage,"Arm",300,-114,230,25,-4,"A68A80");
        Box(_stage,"Monitor",760,18,190,155,"131C23");UIFactory.Text(_stage,"MonitorLine","—  /\\  —",760,20,180,100,35,C("78B8A3"));Box(_stage,"MonitorStand",760,-130,12,150,"68787B");
        Box(_near,"ChairBack",-340,-160,150,130,"674F48");Box(_near,"ChairSeat",-340,-239,160,20,"674F48");Line(_near,"ChairLegL",-402,-300,12,120,-5,"574E4C");Line(_near,"ChairLegR",-278,-300,12,120,5,"574E4C");
        if(id.StartsWith("D6")){_moving=Person(_stage,"HisReflection",-300,-12,.75f,"41464F",false,true);var cg=_moving.gameObject.AddComponent<CanvasGroup>();cg.alpha=.65f;}
        if(id.EndsWith("C")){Box(_near,"FlowerTable",-600,-205,160,15,"69605A");Box(_near,"FlowerCup",-600,-150,35,65,"B0B4AA");Bouquet(_near,-600,-90,.35f);}
    }
    public void Hide(){if(Game!=null&&Game.Director!=null)Game.Director.Quiet();if(Game!=null&&Game.Audio!=null){Game.Audio.StopVoice();Game.Audio.SetStory("");}foreach(Transform t in transform){t.gameObject.SetActive(false);Destroy(t.gameObject);}_stage=null;_moving=null;_wifePose=null;_doorPanel=null;_heroPortrait=null;_heroMouth=null;_heroBrowL=null;_heroBrowR=null;_deviceText=null;_flash=null;_rain.Clear();_rainOrigins.Clear();}
    public void Show(string id,string caption,string heading,float duration,string speaker="HERO",string voice=null){Hide();_id=id;_line=caption;_start=Time.time;_duration=duration;_visible=-1;_reveal=false;UIFactory.Font=Game.Font;
        if(id!="P8"&&id!="FB"){Box(transform,"Backdrop",0,0,1920,1080,"080D16");_stage=UIFactory.Rect(transform,"Stage_"+id,0,0,1920,810);_far=UIFactory.Rect(_stage,"Background",0,0,1920,810);_near=UIFactory.Rect(_stage,"Foreground",0,0,1920,810);
            if(id=="P0"){RearAlley();}
            else if(id=="HL"||id=="HS"){HeroLaugh();}
            else if(id=="PH"){HomeDeparture();}
            else if(id=="F1"||id=="F2"||id=="F3"){Breakup(id);}
            else if(id=="P2"||id=="P1"||id=="C2"){Street();Person(_stage,"She",240,-35,.72f,"BD934E",true);Person(_stage,"OtherMan",430,-28,.79f,"292430",false,true);Bouquet(_stage,325,-27,.5f);Person(_near,"HeroForeground",-650,-130,1.9f,"090E18",false,true);Rain();}
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
        if(id=="FB"){_flash=Box(transform,"MemoryCollapse",0,0,1920,1080,"05080D",.85f);}
        Box(transform,"LetterboxTop",0,484,1920,112,"05080D");Box(transform,"LetterboxBottom",0,-438,1920,204,"05080D");
        if(heading!="")UIFactory.Text(transform,"Heading",heading,0,484,1740,52,27,C("B79565"));
        UIFactory.Text(transform,"Speaker",speaker,0,-361,1700,36,25,C("D6B578"));
        _caption=UIFactory.Text(transform,"Caption",caption,0,-424,1710,108,34,C("F0E6D4"));_caption.fontStyle=FontStyles.Normal;
        _prompt=UIFactory.Text(transform,"ContinuePrompt","CLICK / SPACE TO REVEAL TEXT",0,-506,1700,30,23,C("B5AAA0"));
        var hit=UIFactory.Panel(transform,"ContinueInput",0,0,1920,1080,new Color(0,0,0,0));hit.raycastTarget=true;var button=hit.gameObject.AddComponent<Button>();button.targetGraphic=hit;button.transition=UnityEngine.UI.Selectable.Transition.None;button.onClick.AddListener(Game.Continue);
        Game.Audio.SetStory(id);Game.Audio.PlayVoice(voice);if((id=="P8"||id=="FB")&&speaker.Contains("DEALER")&&Game.Director!=null)Game.Director.Speak(caption,Game.Audio.VoiceLength(voice)>0?Game.Audio.VoiceLength(voice):duration);
    }
    private void Update(){if(_caption==null)return;float t=Time.time-_start,u=Mathf.Clamp01(t/_duration);if(_stage!=null){_stage.localScale=Vector3.one*Mathf.Lerp(1,1.035f,u);_far.anchoredPosition=new Vector2(-8*u,0);_near.anchoredPosition=new Vector2(10*u,0);}
        for(int i=0;i<_rain.Count;i++){var p=_rainOrigins[i];_rain[i].anchoredPosition=new Vector2(p.x-25*(t%2),Mathf.Repeat(p.y+430-t*(290+i%7*15),860)-430);}
        if(_moving!=null){if(_id=="P0")_moving.localRotation=Quaternion.Euler(0,0,Mathf.Sin(t*.65f)*.4f);else if(_id=="F2")_moving.anchoredPosition=new Vector2(345+u*360,-20+Mathf.Sin(t*7)*2);else if(_id=="D1A")_moving.anchoredPosition=new Vector2(-240-u*230,-70);else if(_id=="D4A")_moving.anchoredPosition=new Vector2(-150+u*430,-65+Mathf.Sin(t*12)*6);else if(_id=="P4")_moving.anchoredPosition=new Vector2(-325+u*55,-70+Mathf.Sin(t*5)*2);else if(_id=="D6B")_moving.anchoredPosition=new Vector2(-300,-12-u*70);else if(_id=="E2")_moving.localScale=Vector3.one*Mathf.Lerp(1.1f,.7f,u);}
        if(_doorPanel!=null&&_id=="F2")_doorPanel.localScale=new Vector3(Mathf.Lerp(1,.12f,Mathf.Clamp01(t/2)),1,1);
        if(_wifePose!=null)_wifePose.localRotation=Quaternion.Euler(0,0,_id=="F3"?-2-Mathf.Sin(t*.8f)*.3f:Mathf.Sin(t*.8f)*.4f);
        if(_flash!=null&&_id=="FB"){var fade=_flash.color;fade.a=1-Mathf.Clamp01(t/1.4f);_flash.color=fade;}
        if(_flash!=null&&_id!="FB"){var c=_flash.color;c.a=_id=="D4C"?1-u:_id=="D4B"?u*u:0;_flash.color=c;if(_id=="D4C")_flash.color=Color.Lerp(Color.white,Color.black,u);}
        if(_deviceText!=null&&_id=="D3B")_deviceText.maxVisibleCharacters=t<2?Mathf.FloorToInt(t*9):Mathf.Max(0,13-Mathf.FloorToInt((t-2)*8));
        if(_heroPortrait!=null){
            float laugh=_id=="HS"?0:Mathf.Clamp01((t-3.8f)/2.4f), syllable=Mathf.Abs(Mathf.Sin(t*(laugh>.5f?11:7)));
            _heroPortrait.localScale=Vector3.one*Mathf.Lerp(.87f,1.15f,Mathf.Clamp01(t/9));
            _heroPortrait.localRotation=Quaternion.Euler(0,0,Mathf.Lerp(3,-6,laugh)+Mathf.Sin(t*9)*laugh*1.3f);
            _heroPortrait.anchoredPosition=new Vector2(60,-20+syllable*laugh*9);
            _heroMouth.localScale=new Vector3(1+syllable*laugh*.08f,Mathf.Lerp(.12f,.42f,Mathf.Clamp01(t/2))+(syllable*.75f)*laugh,1);
            _heroBrowL.localRotation=Quaternion.Euler(0,0,-12-laugh*8);_heroBrowR.localRotation=Quaternion.Euler(0,0,12+laugh*8);
        }
        int count=Mathf.Min(_line.Length,Mathf.Max(0,(int)((t-.15f)*42)));if(_id=="HL"){count=t<1?Mathf.FloorToInt(t*7):t<2.2f?7:t<3.3f?14:t<4.8f?24:t<5.5f?31:Mathf.Min(_line.Length,31+Mathf.FloorToInt((t-5.5f)*11));}
        if(_reveal)count=_line.Length;
        if(count!=_visible){_visible=count;_caption.maxVisibleCharacters=count;}
        if(_prompt!=null)_prompt.text=Game.Audio.VoiceBusy&&TextComplete?"LISTENING…  ·  CLICK REVEALS TEXT":TextComplete?(_id.StartsWith("P")?"CLICK / SPACE TO CONTINUE  ·  HOLD SPACE TO SKIP OPENING":"CLICK / SPACE TO CONTINUE"):"CLICK / SPACE TO REVEAL TEXT";
    }
}
}
