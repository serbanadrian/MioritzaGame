using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using Random = UnityEngine.Random;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal; // For URP
// or
using UnityEngine.Rendering.HighDefinition;
using System.Collections; // For HDRP



public class ActiveEffects : MonoBehaviour
{
    [SerializeField] PlayerContext player;
    private Dictionary<string, EffectSO> currentEffects;
    //params for effects
    private VolumeProfile profile;
    void Awake()
    {
        currentEffects = new Dictionary<string, EffectSO>();
        GameObject volumeObject = GameObject.FindGameObjectWithTag("Post-Process");
        if (volumeObject != null)
        {
            Volume ppVolume = volumeObject.GetComponent<Volume>();
            if (ppVolume != null)
            {
                profile = ppVolume.profile;
            }
            else
            {
                Debug.LogError("Volume component not found on the tagged GameObject!");
            }
        }
        else
        {
            Debug.LogError("No GameObject with the 'PostProcessingVolume' tag found!");
        }
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
            if (currentEffects == null)
            {
                effect.Apply(player, profile);
                //StartCoroutine(ApplyEffectDuration(effect));
                currentEffects.Add(effect.effectTag, effect);
            }
            else
            {
                //to see if they overlap or not
                if (currentEffects.ContainsKey(effect.effectTag))
                {
                    currentEffects[effect.effectTag].Remove(player, profile);
                }
                else
                {
                    effect.Apply(player, profile);
                    //  StartCoroutine(ApplyEffectDuration(effect));
                    currentEffects.Add(effect.effectTag, effect);
                }
            }
        }

        foreach (EffectSO effect in effectList)
        {
            if (currentEffects == null)
            {
                StartCoroutine(ApplyEffectDuration(effect));
            }
            else
            {
                //to see if they overlap or not
                if (!currentEffects.ContainsKey(effect.effectTag))
                {
                    currentEffects[effect.effectTag].Remove(player, profile);
                }
                else
                {
                    StartCoroutine(ApplyEffectDuration(effect));
                }
            }
        }
    }
    public IEnumerator ApplyEffectDuration(EffectSO effect)
    {
        if (effect.duration != 0)
        {
            yield return new WaitForSeconds(effect.duration);
            effect.Remove(player, profile);
            currentEffects.Remove(effect.effectTag);
        }
    }

}
