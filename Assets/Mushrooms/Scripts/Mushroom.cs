using System;
using System.Buffers;
using Unity.VisualScripting;
using Unity.VisualScripting.InputSystem;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class Mushroom : MonoBehaviour
{
    [SerializeField] MushroomSO data;
    [SerializeField] ActiveEffects effects;
    private SpriteRenderer sprite;

    void Awake()
    {
        sprite = this.GetComponent<SpriteRenderer>();
        Debug.Log(data);
        if (data != null)
        {
            sprite.sprite = data.sprite;
        }
    }
    public void Initialize(MushroomSO newData, ActiveEffects sceneEffects = null)
    {
        data = newData;
        if (sprite == null) sprite = this.GetComponent<SpriteRenderer>();
        if (data != null && sprite != null)
        {
            sprite.sprite = data.sprite;
        }

        if (sceneEffects != null)
        {
            effects = sceneEffects;
        }
        else if (effects == null)
        {
            effects = GetComponentInChildren<ActiveEffects>();
            if (effects == null)
            {
                var go = GameObject.FindGameObjectWithTag("ActiveEffects");
                if (go != null && go.TryGetComponent<ActiveEffects>(out var ae))
                    effects = ae;
            }
        }
    }
    void OnMouseDown()
    {
        if (effects == null)
        {
            Debug.LogError("Mushroom.effects is null. Ensure ActiveEffects is assigned in the spawner or scene.");
            return;
        }
        effects.ConsumeMushroom(data);
        Destroy(this.GameObject());
    }
}
