using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

[CreateAssetMenu(fileName = "DistortionSO", menuName = "Scriptable Objects/DistortionSO")]
public class DistortionSO : EffectSO
{
    public override void Apply(PlayerContext context, VolumeProfile profile)
    {
        if (profile == null) return;
        if (profile.TryGet<LensDistortion>(out var ld) == false) ld = profile.Add<LensDistortion>(false);
        ld.intensity.overrideState = true;
        ld.intensity.value = 0.6f;
        ld.active = true;
    }

    public override void Remove(PlayerContext context, VolumeProfile profile)
    {
        if (profile == null) return;
        if (profile.TryGet<LensDistortion>(out var ld)) ld.active = false;
    }
}
