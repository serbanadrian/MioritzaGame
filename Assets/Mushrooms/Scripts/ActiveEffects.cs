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
        float roll = Random.Range(0f, 1f);
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
            // If an effect with this tag is already active, remove the old one first
            if (currentEffects.ContainsKey(effect.effectTag))
            {
                currentEffects[effect.effectTag].Remove(player, profile);
                currentEffects.Remove(effect.effectTag);
            }

            // Apply the new effect and track it
            effect.Apply(player, profile);
            currentEffects.Add(effect.effectTag, effect);

            // Start the duration timer if it's not permanent
            if (effect.duration > 0)
            {
                StartCoroutine(ApplyEffectDuration(effect));
            }
        }
    }

    public IEnumerator ApplyEffectDuration(EffectSO effect)
    {
        yield return new WaitForSeconds(effect.duration);

        // Only remove it if it hasn't been overwritten by a new effect of the same type
        if (currentEffects.ContainsKey(effect.effectTag) && currentEffects[effect.effectTag] == effect)
        {
            effect.Remove(player, profile);
            currentEffects.Remove(effect.effectTag);
        }
    }

}
