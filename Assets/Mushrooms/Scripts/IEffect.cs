using UnityEngine;
using UnityEngine.Rendering;

interface IEffect
{
    void Apply(PlayerContext context, VolumeProfile profile);
    void Remove(PlayerContext context, VolumeProfile profile);
}

