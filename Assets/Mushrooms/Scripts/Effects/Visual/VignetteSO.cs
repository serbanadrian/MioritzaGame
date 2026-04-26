using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

[CreateAssetMenu(fileName = "VignetteSO", menuName = "Scriptable Objects/VignetteSO")]
public class VignetteSO : EffectSO
{
    public override void Apply(PlayerContext context, VolumeProfile profile)
    {
        // Disabled — vignette doesn't suit the game's fixed top-down camera.
        if (profile == null) return;
        if (profile.TryGet<Vignette>(out var v)) v.active = false;
    }

    public override void Remove(PlayerContext context, VolumeProfile profile)
    {
        if (profile == null) return;
        if (profile.TryGet<Vignette>(out var v)) v.active = false;
    }
}
