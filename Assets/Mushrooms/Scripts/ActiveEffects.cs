using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using Random = UnityEngine.Random;

public class ActiveEffects : MonoBehaviour
{
    [SerializeField] PlayerContext player;
    [SerializeField] private AudioSource consumeAudioSource;
    private Dictionary<string, EffectSO> currentEffects;
    private VolumeProfile profile;

    void Awake()
    {
        currentEffects = new Dictionary<string, EffectSO>();
        profile = ResolveOrCreateProfile();
    }

    private static VolumeProfile ResolveOrCreateProfile()
    {
        Volume target = null;
        var existing = GameObject.FindGameObjectWithTag("Post-Process");
        if (existing != null) target = existing.GetComponent<Volume>();
        if (target == null)
        {
            foreach (var v in Object.FindObjectsByType<Volume>(FindObjectsSortMode.None))
            {
                if (v.profile != null) { target = v; break; }
            }
        }

        if (target != null && target.profile != null)
        {
            // Clone so we don't mutate the shared asset, but re-assign the clone
            // to the Volume so the renderer actually sees our overrides.
            var cloned = Object.Instantiate(target.profile);
            target.profile = cloned;
            return EnsureOverrides(cloned);
        }

        var go = new GameObject("AutoPostProcessVolume");
        var volume = go.AddComponent<Volume>();
        volume.isGlobal = true;
        volume.priority = 1000;
        var newProfile = ScriptableObject.CreateInstance<VolumeProfile>();
        volume.profile = newProfile;
        return EnsureOverrides(newProfile);
    }

    private static VolumeProfile EnsureOverrides(VolumeProfile p)
    {
        EnsureOverride<ChromaticAberration>(p, ca => { ca.intensity.overrideState = true; ca.intensity.value = 1f; ca.active = false; });
        EnsureOverride<Vignette>(p, v => { v.intensity.overrideState = true; v.intensity.value = 0.6f; v.smoothness.overrideState = true; v.smoothness.value = 1f; v.color.overrideState = true; v.color.value = Color.black; v.active = false; });
        EnsureOverride<FilmGrain>(p, fg => { fg.type.overrideState = true; fg.type.value = FilmGrainLookup.Large01; fg.intensity.overrideState = true; fg.intensity.value = 1f; fg.active = false; });
        if (p.TryGet<DepthOfField>(out var existingDof) == true) existingDof.active = false;
        EnsureOverride<LensDistortion>(p, ld => { ld.intensity.overrideState = true; ld.intensity.value = 0.6f; ld.active = false; });
        EnsureOverride<ColorAdjustments>(p, ca => { ca.hueShift.overrideState = true; ca.hueShift.value = 60f; ca.saturation.overrideState = true; ca.saturation.value = 30f; ca.active = false; });
        return p;
    }

    private static void EnsureOverride<T>(VolumeProfile p, System.Action<T> configure) where T : VolumeComponent
    {
        if (p.TryGet<T>(out var existing) == false)
        {
            existing = p.Add<T>(false);
        }
        configure(existing);
    }

    public void ConsumeMushroom(MushroomSO data)
    {
        if (data == null)
        {
            Debug.LogError($"{nameof(ActiveEffects)}.{nameof(ConsumeMushroom)} called with null {nameof(MushroomSO)}.");
            return;
        }

        if (gameObject.activeSelf == false)
        {
            Debug.LogWarning($"{nameof(ActiveEffects)} GameObject was inactive — activating before applying effects.");
            gameObject.SetActive(true);
        }

        if (player == null) player = UnityEngine.Object.FindAnyObjectByType<PlayerContext>();
        if (player != null) player.InsanityChange(data.InsanityPoints);

        ApplyMushroomEffects(data);
    }

    public void ApplyMushroomEffects(MushroomSO data)
    {
        if (data == null)
        {
            Debug.LogError($"{nameof(ActiveEffects)}.{nameof(ApplyMushroomEffects)} called with null {nameof(MushroomSO)}.");
            return;
        }

        if (gameObject.activeSelf == false)
        {
            Debug.LogWarning($"{nameof(ActiveEffects)} GameObject was inactive — activating before applying effects.");
            gameObject.SetActive(true);
        }

        if (consumeAudioSource != null) consumeAudioSource.Play();
        if (player == null) player = UnityEngine.Object.FindAnyObjectByType<PlayerContext>();

        var roll = Random.Range(0f, 1f);
        var effectList = roll < data.chance ? data.goodEffects : data.badEffects;
        if (effectList == null) return;

        if (profile == null) profile = ResolveOrCreateProfile();
        if (currentEffects == null) currentEffects = new Dictionary<string, EffectSO>();

        foreach (var effect in effectList)
        {
            if (effect == null) continue;
            if (currentEffects.ContainsKey(effect.effectTag) == true)
            {
                currentEffects[effect.effectTag].Remove(player, profile);
                currentEffects.Remove(effect.effectTag);
            }

            effect.Apply(player, profile);
            currentEffects.Add(effect.effectTag, effect);

            if (effect.duration > 0)
            {
                if (isActiveAndEnabled == true && gameObject.activeInHierarchy == true)
                {
                    StartCoroutine(ApplyEffectDuration(effect));
                }
                else
                {
                    Debug.LogWarning($"{nameof(ActiveEffects)} cannot start duration coroutine for '{effect.effectTag}' — GameObject inactive in hierarchy. Effect will persist until next consume.");
                }
            }
        }
    }

    public IEnumerator ApplyEffectDuration(EffectSO effect)
    {
        yield return new WaitForSeconds(effect.duration);

        if (currentEffects.ContainsKey(effect.effectTag) && currentEffects[effect.effectTag] == effect)
        {
            effect.Remove(player, profile);
            currentEffects.Remove(effect.effectTag);
        }
    }
}
