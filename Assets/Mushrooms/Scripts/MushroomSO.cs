using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
[CreateAssetMenu(fileName = "Mushroom", menuName = "Mushrooms/Create new mushroom")]
public class MushroomSO : ScriptableObject
{
    [SerializeField] public string mushroomName;
    [SerializeField] public Sprite sprite;
    [SerializeField, TextArea(2, 3)] public string pickupHint;
    [SerializeField] int insanityPoints;
    public int InsanityPoints
    {
        get
        {
            return insanityPoints;
        }
    }
    [SerializeField] public MushroomType type;
    [SerializeField] public bool cleansToxicWater;
    [SerializeField] public float chance;
    //include normal effects(audio and visual) + hints
    [SerializeField] public List<EffectSO> goodEffects;
    //more aggressive effects(audio and visual) + misleading hints
    [SerializeField] public List<EffectSO> badEffects;
    // Start is called once before the first execution of Update after the MonoBehaviour is created

}
public enum MushroomType
{
    Good,
    Bad,
    Neutral
}
