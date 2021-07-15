using UnityEngine;

namespace SCILL
{
    /// <summary>
    ///     This class is part of the SCILL reward system. Use it to create reward assets that describe the reward. You can
    ///     also attach a prefab (3D model) that will be used to render a 3D model in the reward preview model.
    /// </summary>
    /// <remarks>
    ///     Make sure you place your reward assets in a <c>Resources</c> folder as they will be loaded at runtime! Also make
    ///     sure that
    ///     you place all reward assets like the 3D model preview prefab and the sprite in a <c>Resources</c> folder.
    /// </remarks>
    [CreateAssetMenu(fileName = "Reward", menuName = "SCILL/Reward", order = 1)]
    public class SCILLReward : ScriptableObject
    {
        /// <summary>
        ///     A sprite image that will be used in the <see cref="SCILLBattlePassRewardIcon" /> component to show an icon of the
        ///     reward.
        /// </summary>
        [Tooltip(
            "Set a UI Sprite which is used as a reward image in the Battle Pass. Make sure its located in a Resources folder so that it can be loaded at runtime")]
        public Sprite image;

        /// <summary>
        ///     A name of that reward.
        /// </summary>
        [Tooltip("The name of the reward")] public new string name;

        /// <summary>
        ///     Describe the reward a bit more.
        /// </summary>
        [TextArea] [Tooltip("The description of the reward")]
        public string description;

        /// <summary>
        ///     The reward preview will instantiate this prefab in a “photo box” to render a 3D preview of this model. You can
        ///     attach the <c>Rotate</c> script (which is part of the SCILL package) to add some rotation to the script. You will also
        ///     need to set the layer of this prefab to a layer that you exclude from your other game cameras to make sure the
        ///     reward preview will not be visible somewhere in the game.
        /// </summary>
        [Tooltip(
            "A prefab that must be located in a Resources folder. This item will be instantiated in the photo box to render a 3D image. Add some sort of animation to it - for example our Rotate script to give it some motion")]
        public GameObject prefab;
    }
}