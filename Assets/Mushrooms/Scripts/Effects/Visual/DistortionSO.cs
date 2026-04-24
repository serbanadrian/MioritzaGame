using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal; // For URP
// or
using UnityEngine.Rendering.HighDefinition; // For HDRP


[CreateAssetMenu(fileName = "DistortionSO", menuName = "Scriptable Objects/DistortionSO")]
public class DistortionSO : EffectSO
{
    [SerializeField] EffectSO effectSO;
    [SerializeField] public string namrEffect;
    [SerializeField] public bool enabled;
    private LensDistortion dof;

    public override void Apply(PlayerContext context, VolumeProfile profile)
    {
        if (profile.TryGet<LensDistortion>(out var distortion))
        {
            dof = distortion;
            dof.active = false;

            Debug.Log("Distortion: " + (dof.active ? "Enabled" : "Disabled"));
        }
    }

    public override void Remove(PlayerContext context, VolumeProfile profile)
    {
        if (profile.TryGet<LensDistortion>(out var distortion))
        {
            dof = distortion;
            dof.active = true;
            Debug.Log("Distortion: " + (dof.active ? "Enabled" : "Disabled"));
        }
    }


}
