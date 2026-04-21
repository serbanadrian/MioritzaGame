using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using Random = UnityEngine.Random;

public class ActiveEffects : MonoBehaviour
{
    [SerializeField] PlayerContext player;
    private Dictionary<string, EffectSO> currentEffects;
    void Start()
    {
    }
    public void ConsumeMushroom(MushroomSO data)
    {
        List<EffectSO> effectList;
        player.InsanityChange(data.InsanityPoints);
        float roll = Random.Range(0, 1);
        if (roll < data.chance)
        {
            effectList = data.goodEffects;
        }
        else
        {
            effectList = data.badEffects;
        }
        foreach (EffectSO effect in effectList)
        {
            if (!currentEffects.ContainsKey(effect.effectTag))
            {
                currentEffects[effect.effectTag].Remove(player);
            }
            else
            {
                effect.Apply(player);
                currentEffects[effect.effectTag] = effect;
            }
        }
    }
}
