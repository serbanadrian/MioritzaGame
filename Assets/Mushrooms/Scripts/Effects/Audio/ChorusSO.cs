using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal; // For URP
// or
using UnityEngine.Rendering.HighDefinition; // For HDRP

[CreateAssetMenu(fileName = "ChorusSO", menuName = "Scriptable Objects/ChorusSO")]
public class ChorusSO : EffectSO
{

    [SerializeField] EffectSO effectSO;
    private AudioChorusFilter v;

    public override void Apply(PlayerContext context, VolumeProfile profile)
    {
        if (context.TryGetComponent<AudioChorusFilter>(out var chorus))
        {

            chorus.enabled = true;

            Debug.Log("Chorus: " + (chorus.enabled ? "Enabled" : "Disabled"));
        }
    }

    public override void Remove(PlayerContext context, VolumeProfile profile)
    {
        if (context.TryGetComponent<AudioChorusFilter>(out var chorus))
        {

            chorus.enabled = false;

            Debug.Log("Chorus: " + (chorus.enabled ? "Enabled" : "Disabled"));
        }
    }

}
