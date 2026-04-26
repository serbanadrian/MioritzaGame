using UnityEngine;
using UnityEngine.Rendering;

[CreateAssetMenu(fileName = "BlurSO", menuName = "Scriptable Objects/BlurSO")]
public class BlurSO : EffectSO
{
    public override void Apply(PlayerContext context, VolumeProfile profile) { }
    public override void Remove(PlayerContext context, VolumeProfile profile) { }
}
