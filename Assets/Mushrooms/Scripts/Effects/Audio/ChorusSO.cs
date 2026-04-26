using UnityEngine;
using UnityEngine.Rendering;

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
