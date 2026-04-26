using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

[CreateAssetMenu(fileName = "ChromaticAbberationSO", menuName = "Scriptable Objects/ChromaticAbberationSO")]
public class ChromaticAbberationSO : EffectSO
{
    public override void Apply(PlayerContext context, VolumeProfile profile)
    {
        if (profile == null) return;
        if (profile.TryGet<ChromaticAberration>(out var ca) == false) ca = profile.Add<ChromaticAberration>(false);
        ca.intensity.overrideState = true;
        ca.intensity.value = 1f;
        ca.active = true;
    }

    public override void Remove(PlayerContext context, VolumeProfile profile)
    {
        if (profile == null) return;
        if (profile.TryGet<ChromaticAberration>(out var ca)) ca.active = false;
    }
}
