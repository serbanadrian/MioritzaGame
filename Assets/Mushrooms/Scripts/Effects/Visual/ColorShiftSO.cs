using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

[CreateAssetMenu(fileName = "ColorShiftSO", menuName = "Scriptable Objects/ColorShiftSO")]
public class ColorShiftSO : EffectSO
{
    public override void Apply(PlayerContext context, VolumeProfile profile)
    {
        if (profile == null) return;
        if (profile.TryGet<ColorAdjustments>(out var ca) == false) ca = profile.Add<ColorAdjustments>(false);
        ca.hueShift.overrideState = true;
        ca.hueShift.value = 60f;
        ca.saturation.overrideState = true;
        ca.saturation.value = 30f;
        ca.active = true;
    }

    public override void Remove(PlayerContext context, VolumeProfile profile)
    {
        if (profile == null) return;
        if (profile.TryGet<ColorAdjustments>(out var ca)) ca.active = false;
    }
}
