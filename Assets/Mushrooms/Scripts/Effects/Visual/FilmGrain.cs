using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

[CreateAssetMenu(fileName = "FilmGrainSO", menuName = "Scriptable Objects/FilmGrainSO")]
public class FilmGrainSO : EffectSO
{
    public override void Apply(PlayerContext context, VolumeProfile profile)
    {
        if (profile == null) return;
        if (profile.TryGet<FilmGrain>(out var fg) == false) fg = profile.Add<FilmGrain>(false);
        fg.type.overrideState = true;
        fg.type.value = FilmGrainLookup.Large01;
        fg.intensity.overrideState = true;
        fg.intensity.value = 1f;
        fg.active = true;
    }

    public override void Remove(PlayerContext context, VolumeProfile profile)
    {
        if (profile == null) return;
        if (profile.TryGet<FilmGrain>(out var fg)) fg.active = false;
    }
}
