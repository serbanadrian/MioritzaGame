using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal; // For URP
// or
using UnityEngine.Rendering.HighDefinition; // For HDRP

interface IEffect
{
    void Apply(PlayerContext context, VolumeProfile profile);
    void Remove(PlayerContext context, VolumeProfile profile);
}

