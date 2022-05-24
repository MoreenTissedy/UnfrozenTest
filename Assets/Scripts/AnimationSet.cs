using System;
using System.Collections.Generic;
using System.Linq;
using Spine.Unity;
using UnityEngine;

/// <summary>
/// This is one of my common scripts.
/// This is a dictionary with animations for a specific character.
/// It enables us to reuse component settings between characters and switch animation sets (eg, when switching weapons). 
/// </summary>
namespace UnfrozenTest
{
    [CreateAssetMenu(fileName = "New Animation Set", menuName = "Animation Set", order = 40)]
    public class AnimationSet : ScriptableObject
    {
        public SkeletonDataAsset skeleton;
        public string filter;
        public string[] animations;

        public string Get(AnimationSetItems item)
        {
            int index = (int) item;
            return animations[index];
        }
    }
}
