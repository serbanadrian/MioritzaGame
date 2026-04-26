using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

[CreateAssetMenu(fileName = "VignetteSO", menuName = "Scriptable Objects/VignetteSO")]
public class VignetteSO : EffectSO
{
    public override void Apply(PlayerContext context, VolumeProfile profile)
    {
        if (profile == null) return;
        if (profile.TryGet<Vignette>(out var v) == false) v = profile.Add<Vignette>(false);
        v.intensity.overrideState = true;
        v.intensity.value = 0.6f;
        v.smoothness.overrideState = true;
        v.smoothness.value = 1f;
        v.color.overrideState = true;
        v.color.value = Color.black;
        v.active = true;
    }

    public override void Remove(PlayerContext context, VolumeProfile profile)
    {
        if (profile == null) return;
        if (profile.TryGet<Vignette>(out var v)) v.active = false;
    }
}
