using System;
using System.Buffers;
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
    void OnMouseDown()
    {
        effects.ConsumeMushroom(data);
    }
}
