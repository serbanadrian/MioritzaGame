using UnityEngine;
using UnityEngine.Rendering;

[CreateAssetMenu(fileName = "ReverbNoiseSO", menuName = "Scriptable Objects/ReverbNoiseSO")]
public class ReverbNoiseSO : EffectSO
{
    public override void Apply(PlayerContext context, VolumeProfile profile)
    {
        if (context.TryGetComponent<AudioReverbFilter>(out var filter))
        {
            filter.enabled = true;
            Debug.Log("Reverb: " + (filter.enabled ? "Enabled" : "Disabled"));
        }

        if (context.TryGetComponent<AudioDistortionFilter>(out var noise))
        {
            noise.enabled = true;
            Debug.Log("Noise: " + (noise.enabled ? "Enabled" : "Disabled"));
        }
    }

    public override void Remove(PlayerContext context, VolumeProfile profile)
    {
        if (context.TryGetComponent<AudioReverbFilter>(out var filter))
        {
            filter.enabled = false;
            Debug.Log("Reverb: " + (filter.enabled ? "Enabled" : "Disabled"));
        }

        if (context.TryGetComponent<AudioDistortionFilter>(out var noise))
        {
            noise.enabled = false;
            Debug.Log("Noise: " + (noise.enabled ? "Enabled" : "Disabled"));
        }
    }
}
