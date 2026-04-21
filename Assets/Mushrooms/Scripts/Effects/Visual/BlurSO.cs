using UnityEngine;

[CreateAssetMenu(fileName = "BlurSO", menuName = "Scriptable Objects/BlurSO")]
public class BlurSO : EffectSO
{
    [SerializeField] EffectSO effectSO;
    //params for effects
    public override void Apply(PlayerContext context)
    {
        Debug.Log("Applying blur effects");
    }

    public override void Remove(PlayerContext context)
    {
        Debug.Log("Removing blur effects");
    }


}
