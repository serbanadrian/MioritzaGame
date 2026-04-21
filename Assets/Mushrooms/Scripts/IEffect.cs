using UnityEngine;
using UnityEngine.UIElements;

interface IEffect
{
    void Apply(PlayerContext context);
    void Remove(PlayerContext context);
}

