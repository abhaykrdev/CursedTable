using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;
using UnityEngine.UI;

namespace HollowSeven.Editor {
public static class ProjectSetup {
    public const string Root = "Assets/_Project";
    [MenuItem("Cursed Table/Setup Foundation")]
    public static void Foundation() {
        foreach (var folder in new[]{"Scenes","Scripts/Core","Scripts/Puzzle","Scripts/Luck","Scripts/Story","Scripts/Tutorial","Scripts/UI","Scripts/Presentation","Scripts/Audio","Scripts/Input","Scripts/Tests/EditMode","Data/Tutorial","Data/Story","Prefabs/UI","Prefabs/Story","Resources/Story","Art/Placeholders","Art/Fonts","Art/Sprites","Audio/Music","Audio/Sfx","Audio/Ambience","Audio/Voice","Materials","Settings"}) Directory.CreateDirectory(Root+"/"+folder);
        Directory.CreateDirectory("Docs/screens");
        AssetDatabase.Refresh();
        var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
        new GameObject("_Managers"); new GameObject("Casino3D"); new GameObject("GlobalVolumes");
        var camera = new GameObject("PlayerCamera", typeof(Camera),typeof(AudioListener)).GetComponent<Camera>();
        camera.tag="MainCamera"; camera.clearFlags=CameraClearFlags.SolidColor; camera.backgroundColor=new Color(.027f,.02f,.039f);
        foreach (var spec in new[]{("Canvas_HUD",10),("Canvas_Story",20),("Canvas_Overlay",30)}) {
            var go=new GameObject(spec.Item1,typeof(RectTransform),typeof(Canvas),typeof(CanvasScaler),typeof(GraphicRaycaster));
            var canvas=go.GetComponent<Canvas>(); canvas.renderMode=RenderMode.ScreenSpaceOverlay; canvas.sortingOrder=spec.Item2;
            var scaler=go.GetComponent<CanvasScaler>(); scaler.uiScaleMode=CanvasScaler.ScaleMode.ScaleWithScreenSize; scaler.referenceResolution=new Vector2(1920,1080); scaler.matchWidthOrHeight=.5f;
        }
        new GameObject("EventSystem",typeof(EventSystem),typeof(InputSystemUIInputModule));
        EditorSceneManager.SaveScene(scene,Root+"/Scenes/Game.unity");
        AssetDatabase.SaveAssets();
        Debug.Log("Cursed Table M0 foundation created. Web settings left to user.");
    }
}
}
