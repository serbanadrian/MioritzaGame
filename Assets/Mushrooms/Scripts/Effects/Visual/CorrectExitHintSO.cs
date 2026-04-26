using UnityEngine;
using UnityEngine.Rendering;

[CreateAssetMenu(fileName = "CorrectExitHintSO", menuName = "Scriptable Objects/CorrectExitHintSO")]
public class CorrectExitHintSO : EffectSO
{
    public override void Apply(PlayerContext context, VolumeProfile profile)
    {
        // Sticky glow — stays on until the player crosses the exit (scene reload clears it).
        MioritzaGame.Game.ExitGlow.GlowAll(float.MaxValue);
    }

    public override void Remove(PlayerContext context, VolumeProfile profile)
    {
        // No-op — the hint is meant to last until the level transitions.
    }
}
