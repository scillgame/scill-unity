using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SCILL
{
    [CreateAssetMenu(fileName = "Reward", menuName = "SCILL/Reward", order = 1)]
    public class SCILLReward : ScriptableObject
    {
        [Tooltip(
            "Set a UI Sprite which is used as a reward image in the Battle Pass. Make sure its located in a Resources folder so that it can be loaded at runtime")]
        public Sprite image;

        [Tooltip("The name of the reward")] public string name;

        [TextArea] [Tooltip("The description of the reward")]
        public string description;

        [Tooltip(
            "A prefab that must be located in a Resources folder. This item will be instantiated in the photo box to render a 3D image. Add some sort of animation to it - for example our Rotate script to give it some motion")]
        public GameObject prefab;
    }
}


