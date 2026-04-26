using System;
using UnityEngine;
using UnityEngine.Video;

namespace MioritzaGame.Game
{
    [Serializable]
    public struct CutsceneStep
    {
        public VideoClip _video;
        public Sprite _image;
        [TextArea(2, 5)] public string _caption;
        [Min(0f)] public float _duration;
    }
}
