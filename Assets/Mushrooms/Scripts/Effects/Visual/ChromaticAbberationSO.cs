using UnityEngine;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal; // For URP
// or
using UnityEngine.Rendering.HighDefinition; // For HDRP

[CreateAssetMenu(fileName = "ChromaticAbberationSO", menuName = "Scriptable Objects/ChromaticAbberationSO")]
public class ChromaticAbberationSO : EffectSO
{

    [SerializeField] EffectSO effectSO;

    public override void Apply(PlayerContext context, VolumeProfile profile)
    {
        if (profile.TryGet<ChromaticAberration>(out var ChromaticAberration))
        {

            ChromaticAberration.active = true;

            Debug.Log("ChromaticAberration: " + (ChromaticAberration.active ? "Enabled" : "Disabled"));
        }
    }

    public override void Remove(PlayerContext context, VolumeProfile profile)
    {
        if (profile.TryGet<ChromaticAberration>(out var ChromaticAberration))
        {

            ChromaticAberration.active = true;

            Debug.Log("ChromaticAberration: " + (ChromaticAberration.active ? "Enabled" : "Disabled"));
        }
    }

}
