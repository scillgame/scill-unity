using SCILL.Model;
using UnityEngine;
using UnityEngine.UI;

namespace SCILL
{
    /// <summary>
    ///     <para>
    ///         This component will handle a reward available for a battle pass level. Every level can have a reward and it
    ///         typically is represented with an icon and some sort of state information. Often rewards can be clicked to show
    ///         a nice preview of the reward.
    ///     </para>
    ///     <para>
    ///         Create a prefab with this component attached to the root Game Object and build a nice UI below it to render an
    ///         icon with lock and claimed state icons. Connect that prefab to the <c>rewardIconPrefab</c> setting of
    ///         <see cref="SCILLBattlePassLevel" />. The level component will then instantiate this prefab automatically.
    ///     </para>
    ///     <para>
    ///         You need to create a <see cref="SCILLReward" /> asset and set the reward to the name of this reward asset in
    ///         the Admin Panel.
    ///         This class will load this resource (make sure its in a <c>Resources</c> folder in Unity) and will take the name
    ///         and
    ///         image from this reward asset to set the UI elements connected.
    ///     </para>
    /// </summary>
    public class SCILLBattlePassRewardIcon : MonoBehaviour
    {
        /// <summary>
        ///     An image field that will be used to set the image of the <see cref="SCILLReward" /> asset.
        /// </summary>
        [Tooltip("An image field that will be used to set the image of the reward that is set in the Admin Panel")]
        public Image rewardImage;

        /// <summary>
        ///     If the level of this reward has not been unlocked yet, then this GameObject will be shown. Otherwise it will be
        ///     hidden.
        /// </summary>
        [Tooltip("Set a game object that will be hidden by default and shown if the reward has been claimed")]
        public GameObject claimedIcon;

        /// <summary>
        ///     If the reward already has been claimed for this reward, then this GameObject will be shown, otherwise it will be
        ///     hidden.
        /// </summary>
        [Tooltip("Set a game object that will be hidden by default and shown if the level is locked")]
        public GameObject lockedIcon;

        private BattlePassLevel _battlePassLevel;

        private SCILLReward _reward;

        public BattlePassLevel battlePassLevel
        {
            get => _battlePassLevel;
            set
            {
                _battlePassLevel = value;
                UpdateUI();
            }
        }

        // Start is called before the first frame update
        private void Start()
        {
        }

        private void OnEnable()
        {
            SCILLBattlePassManager.OnBattlePassLevelRewardClaimed += OnBattlePassLevelRewardClaimed;
        }

        private void OnDisable()
        {
            SCILLBattlePassManager.OnBattlePassLevelRewardClaimed -= OnBattlePassLevelRewardClaimed;
        }

        private void OnBattlePassLevelRewardClaimed(BattlePassLevel level)
        {
            if (level.level_id == _battlePassLevel.level_id) _battlePassLevel.reward_claimed = level.reward_claimed;

            UpdateUI();
        }

        /// <summary>
        ///     This functions listens on the <see cref="SCILLBattlePassManager.OnBattlePassLevelRewardClaimed" /> event of
        ///     <see cref="SCILLBattlePassManager" /> and will show the
        ///     <see cref="claimedIcon" /> to indicate that this levels reward has been claimed. You can override this method if
        ///     you want to add
        ///     animations or your own business logic.
        /// </summary>
        /// <param name="claimed"></param>
        protected virtual void OnClaimed(bool claimed)
        {
            if (claimedIcon) claimedIcon.SetActive(claimed);
        }

        /// <summary>
        ///     Trigger a UI update. That is done automatically whenever anything changes but you can also trigger an update
        ///     manually by calling this function.
        /// </summary>
        private void UpdateUI()
        {
            _reward = Resources.Load<SCILLReward>(battlePassLevel.reward_amount);
            if (_reward) rewardImage.sprite = _reward.image;

            if (lockedIcon) lockedIcon.SetActive(battlePassLevel.level_completed == false);

            OnClaimed(battlePassLevel.reward_claimed == true);
        }
    }
}