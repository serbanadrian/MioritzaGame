using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal; // For URP
// or
using UnityEngine.Rendering.HighDefinition; // For HDRP


[CreateAssetMenu(fileName = "ColorShiftSO", menuName = "Scriptable Objects/ColorShiftSO")]
public class ColorShiftSO : EffectSO
{
    [SerializeField] EffectSO effectSO;
    private ColorAdjustments v;

    public override void Apply(PlayerContext context, VolumeProfile profile)
    {
        if (profile.TryGet<ColorAdjustments>(out var ColorAdjustments))
        {

            ColorAdjustments.active = true;

            Debug.Log("ColorAdjustments: " + (ColorAdjustments.active ? "Enabled" : "Disabled"));
        }
    }

    public override void Remove(PlayerContext context, VolumeProfile profile)
    {
        if (profile.TryGet<ColorAdjustments>(out var ColorAdjustments))
        {

            ColorAdjustments.active = true;

            Debug.Log("ColorAdjustments: " + (ColorAdjustments.active ? "Enabled" : "Disabled"));
        }
    }
}