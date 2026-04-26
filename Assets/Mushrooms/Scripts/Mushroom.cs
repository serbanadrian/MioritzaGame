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

    // Fired when a good mushroom is eaten by water. Passes the world position where it happened.
    public static event Action<Vector3> OnGoodEatenByWater;

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
        Destroy(this.gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other == null) return;

        // Detect water by tag or by name containing "Apa" (Romanian for water)
        if (other.CompareTag("Apa") || other.CompareTag("Water") || other.name.Contains("Apa"))
        {
            if (data != null && data.type == MushroomType.Good)
            {
                if (effects != null) effects.ConsumeMushroom(data);
                // Destroy the mushroom
                Destroy(this.gameObject);
                // Destroy the water object as requested
                Destroy(other.gameObject);
                // Notify spawners to respawn mushrooms inside walking area
                OnGoodEatenByWater?.Invoke(transform.position);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other == null) return;
        var go = other.gameObject;
        if (go.CompareTag("Apa") || go.CompareTag("Water") || go.name.Contains("Apa"))
        {
            if (data != null && data.type == MushroomType.Good)
            {
                if (effects != null) effects.ConsumeMushroom(data);
                Destroy(this.gameObject);
                Destroy(go);
                OnGoodEatenByWater?.Invoke(transform.position);
            }
        }
    }
}
