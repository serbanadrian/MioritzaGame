using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal; // For URP
// or
using UnityEngine.Rendering.HighDefinition; // For HDRP

[CreateAssetMenu(fileName = "VignetteSO", menuName = "Scriptable Objects/VignetteSO")]
public class VignetteSO : EffectSO
{

    [SerializeField] EffectSO effectSO;
    private Vignette v;

    public override void Apply(PlayerContext context, VolumeProfile profile)
    {
        if (profile.TryGet<Vignette>(out var Vignette))
        {

            Vignette.active = true;

            Debug.Log("Vignette: " + (Vignette.active ? "Enabled" : "Disabled"));
        }
    }

    public override void Remove(PlayerContext context, VolumeProfile profile)
    {
        if (profile.TryGet<Vignette>(out var Vignette))
        {

            Vignette.active = true;

            Debug.Log("Vignette: " + (Vignette.active ? "Enabled" : "Disabled"));
        }
    }

}
