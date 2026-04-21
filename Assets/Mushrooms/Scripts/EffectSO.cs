using UnityEngine;

[CreateAssetMenu(fileName = "EffectSO", menuName = "Scriptable Objects/EffectSO")]
public abstract class EffectSO : ScriptableObject, IEffect
{
    [SerializeField] public string effectTag;
    [SerializeField] public float duration;//seconds

    public abstract void Apply(PlayerContext context);

    public abstract void Remove(PlayerContext context);
}
