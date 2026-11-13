using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
namespace HollowSeven { public sealed class ChipView:MonoBehaviour,IPointerEnterHandler,IPointerExitHandler {
    public int Index;public Button Button;public TextMeshProUGUI Number;public ShapeGraphic Ring,Glow;public RectTransform Rect;public HudController Hud;
    public void OnPointerEnter(PointerEventData e){Hud.Hover(Index);}
    public void OnPointerExit(PointerEventData e){Hud.Hover(-1);}
    public void Bind(int value,LuckTable table){Number.SetText("{0}",value);var tag=table.GetTag(value);var colour=tag==LuckTag.Cursed||tag==LuckTag.Doom?UIFactory.Red:tag==LuckTag.None?UIFactory.Brass:UIFactory.Green;Ring.color=colour;var marker=transform.Find("Tag").GetComponent<TextMeshProUGUI>();marker.text=tag==LuckTag.None?"":tag==LuckTag.Jackpot||tag==LuckTag.Doom?"•••":tag==LuckTag.DoubleLucky?"••":"•";marker.color=colour;}
    public void Select(bool selected){Glow.gameObject.SetActive(selected);Rect.localScale=Vector3.one*(selected?1.12f:1);}
} }
