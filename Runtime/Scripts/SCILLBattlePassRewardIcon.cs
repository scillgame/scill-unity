using System.Collections;
using System.Collections.Generic;
using SCILL.Model;
using UnityEngine;
using UnityEngine.UI;


namespace SCILL
{
    public class SCILLBattlePassRewardIcon : MonoBehaviour
    {
        [Tooltip("An image field that will be used to set the image of the reward that is set in the Admin Panel")]
        public Image rewardImage;

        [Tooltip("Set a game object that will be hidden by default and shown if the reward has been claimed")]
        public GameObject claimedIcon;

        [Tooltip("Set a game object that will be hidden by default and shown if the level is locked")]
        public GameObject lockedIcon;

        private SCILLReward _reward;
        private BattlePassLevel _battlePassLevel;

        public BattlePassLevel battlePassLevel
        {
            get => _battlePassLevel;
            set
            {
                _battlePassLevel = value;
                UpdateUI();
            }
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
            if (level.level_id == _battlePassLevel.level_id)
            {
                _battlePassLevel.reward_claimed = level.reward_claimed;
            }

            UpdateUI();
        }

        // Start is called before the first frame update
        void Start()
        {
        }

        protected virtual void OnClaimed(bool claimed)
        {
            if (claimedIcon)
            {
                claimedIcon.SetActive(claimed);
            }
        }

        // Update is called once per frame
        void UpdateUI()
        {
            _reward = Resources.Load<SCILLReward>(battlePassLevel.reward_amount);
            if (_reward)
            {
                rewardImage.sprite = _reward.image;
            }

            if (lockedIcon)
            {
                lockedIcon.SetActive(battlePassLevel.level_completed == false);
            }

            OnClaimed(battlePassLevel.reward_claimed == true);
        }
    }
}