using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal; // For URP
// or
using UnityEngine.Rendering.HighDefinition; // For HDRP

[CreateAssetMenu(fileName = "EffectSO", menuName = "Scriptable Objects/EffectSO")]
public abstract class EffectSO : ScriptableObject, IEffect
{
    [SerializeField] public string effectTag;
    [SerializeField] public float duration;//seconds
    [SerializeField] public bool enable;

    public abstract void Apply(PlayerContext context, VolumeProfile profile);

    public abstract void Remove(PlayerContext context, VolumeProfile profile);
}
