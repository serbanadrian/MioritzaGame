using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal; // For URP
// or
using UnityEngine.Rendering.HighDefinition; // For HDRP

[CreateAssetMenu(fileName = "FilmGrainSO", menuName = "Scriptable Objects/FilmGrainSO")]
public class FilmGrainSO : EffectSO
{

    [SerializeField] EffectSO effectSO;
    private FilmGrain v;

    public override void Apply(PlayerContext context, VolumeProfile profile)
    {
        if (profile.TryGet<FilmGrain>(out var FilmGrain))
        {

            FilmGrain.active = true;

            Debug.Log("FilmGrain: " + (FilmGrain.active ? "Enabled" : "Disabled"));
        }
    }

    public override void Remove(PlayerContext context, VolumeProfile profile)
    {
        if (profile.TryGet<FilmGrain>(out var FilmGrain))
        {

            FilmGrain.active = true;

            Debug.Log("FilmGrain: " + (FilmGrain.active ? "Enabled" : "Disabled"));
        }
    }

}
