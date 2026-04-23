using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal; // For URP
// or
using UnityEngine.Rendering.HighDefinition; // For HDRP

[CreateAssetMenu(fileName = "BlurSO", menuName = "Scriptable Objects/BlurSO")]
public class BlurSO : EffectSO
{
    [SerializeField] EffectSO effectSO;
    [SerializeField] public string namrEffect;
    [SerializeField] public bool enabled;
    private DepthOfField dof;

    public override void Apply(PlayerContext context, VolumeProfile profile)
    {
        if (profile.TryGet<DepthOfField>(out var depthOfField))
        {
            dof = depthOfField;
            dof.active = false;

            Debug.Log("Depth of Field: " + (dof.active ? "Enabled" : "Disabled"));
        }
    }

    public override void Remove(PlayerContext context, VolumeProfile profile)
    {
        if (profile.TryGet<DepthOfField>(out var depthOfField))
        {
            dof = depthOfField;
            dof.active = true;
            Debug.Log("Depth of Field: " + (dof.active ? "Enabled" : "Disabled"));
        }
    }


}
