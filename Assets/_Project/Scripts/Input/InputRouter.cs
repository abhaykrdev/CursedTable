using UnityEngine;
using UnityEngine.InputSystem;
namespace HollowSeven { public sealed class InputRouter:MonoBehaviour {
    public GameManager Game;private float _held;private readonly Key[] _digits={Key.Digit1,Key.Digit2,Key.Digit3,Key.Digit4,Key.Digit5,Key.Digit6};private readonly Key[] _ops={Key.Q,Key.W,Key.E,Key.R};
    private void Update(){var k=Keyboard.current;if(k==null)return;if(k.mKey.wasPressedThisFrame)Game.Audio.ToggleMute();if(k.pKey.wasPressedThisFrame)Game.TogglePause();if(k.escapeKey.wasPressedThisFrame){if(Game.Round.Selected>=0&&!Game.Paused)Game.Round.Clear();else Game.TogglePause();}
        if(Game.State==GameState.Prologue){if(k.spaceKey.wasPressedThisFrame)Game.Continue();_held=k.spaceKey.isPressed?_held+Time.unscaledDeltaTime:0;if(_held>=1)Game.SkipPrologue();}else if(k.spaceKey.wasPressedThisFrame)Game.Continue();
        if(Game.Paused)return;for(int i=0;i<6;i++)if(k[_digits[i]].wasPressedThisFrame)Game.Round.SelectChip(i);for(int i=0;i<4;i++)if(k[_ops[i]].wasPressedThisFrame)Game.Round.SelectOperator((OpKind)i);if(k.zKey.wasPressedThisFrame||k.backspaceKey.wasPressedThisFrame)Game.Round.Undo();if(k.hKey.wasPressedThisFrame)Game.Round.Hint();if(Mouse.current!=null&&Mouse.current.rightButton.wasPressedThisFrame)Game.Round.Clear();
#if UNITY_EDITOR || DEBUG
        if(Debug.isDebugBuild||Application.isEditor){if(k.f1Key.wasPressedThisFrame)Game.Round.DebugWin();if(k.f2Key.wasPressedThisFrame)Game.Luck.Add(5);if(k.f3Key.wasPressedThisFrame)Game.Luck.Add(-5);if(k.f4Key.wasPressedThisFrame)Game.DebugDream();if(k.f5Key.wasPressedThisFrame)Game.Luck.SetLuck(-15);if(k.f6Key.wasPressedThisFrame)Game.SkipTutorial();}
#endif
    }
} }
