using UnityEngine;
using UnityEngine.Rendering;

[CreateAssetMenu(fileName = "EchoSO", menuName = "Scriptable Objects/EchoSO")]
public class EchoSO : EffectSO
{
    public override void Apply(PlayerContext context, VolumeProfile profile)
    {
        if (context.TryGetComponent<AudioEchoFilter>(out var filter))
        {
            filter.enabled = true;
            Debug.Log("Echo: " + (filter.enabled ? "Enabled" : "Disabled"));
        }
    }

    public override void Remove(PlayerContext context, VolumeProfile profile)
    {
        if (context.TryGetComponent<AudioEchoFilter>(out var filter))
        {
            filter.enabled = false;
            Debug.Log("Echo: " + (filter.enabled ? "Enabled" : "Disabled"));
        }
    }
}
