using System;
using System.Collections.Generic;
using SCILL;
using SCILL.Model;
using UnityEngine;
using UnityEngine.Events;

namespace SCILL.Effects
{
    /// <summary>
    /// Simple Utility class for accessing Battle Pass related realtime events in the editor via <c>UnityEvent</c>. This script
    /// connects to <see cref="SCILLBattlePassManager"/> events, which are only called for the currently <see cref="SCILLBattlePassManager.SelectedBattlePass"/>.
    /// </summary>
    public class SCILLBattlePassEvents : MonoBehaviour
    {
        /// <summary>
        /// Called when the currently selected Battle Pass was unlocked.
        /// </summary>
        [SerializeField] public UnityEvent onUnlocked;
        /// <summary>
        /// Called when a level challenge on the currently selected Battle Pass was updated.
        /// </summary>
        [SerializeField] public UnityEvent onChallengeProgressUpdated;
        /// <summary>
        /// Called when a level rewards for the currently selected Battle Pass was claimed.
        /// </summary>
        [SerializeField] public UnityEvent onLevelRewardClaimed;
        /// <summary>
        /// Called when a level for the currently selected Battle Pass was completed.
        /// </summary>
        [SerializeField] public UnityEvent onLevelCompleted;
        
    
        protected Dictionary<string, List<BattlePassLevel>> StoredBattlePassLevels =
            new Dictionary<string, List<BattlePassLevel>>();

        protected void OnEnable()
        {
            SCILLBattlePassManager.OnBattlePassLevelsUpdatedFromServer += OnBattlePassLevelsUpdated;
            SCILLBattlePassManager.OnBattlePassLevelRewardClaimed += OnBattlePassLevelRewardClaimed;
            SCILLBattlePassManager.OnBattlePassChallengeUpdate += OnBattlePassChallengeUpdate;
        }

        protected void OnDisable()
        {
            SCILLBattlePassManager.OnBattlePassLevelsUpdatedFromServer -= OnBattlePassLevelsUpdated;
            SCILLBattlePassManager.OnBattlePassLevelRewardClaimed -= OnBattlePassLevelRewardClaimed;
            SCILLBattlePassManager.OnBattlePassChallengeUpdate -= OnBattlePassChallengeUpdate;
        }

        private void OnBattlePassChallengeUpdate(BattlePassChallengeChangedPayload challengechangedpayload)
        {
            onChallengeProgressUpdated.Invoke();
        }

        private void OnBattlePassLevelRewardClaimed(BattlePassLevel level)
        {
            onLevelRewardClaimed.Invoke();
        }

        private void OnBattlePassLevelsUpdated(List<BattlePassLevel> currentBpLevels)
        {
            if (currentBpLevels.Count > 0)
            {
                string battlePassID = currentBpLevels[0].battle_pass_id;
                if (StoredBattlePassLevels.ContainsKey(battlePassID) && null != StoredBattlePassLevels[battlePassID])
                {
                    // initialize feedback clip with update sound
                    List<BattlePassLevel> previousBattlePassLevels = StoredBattlePassLevels[battlePassID];
                    for (int bpLevelID = 0; bpLevelID < currentBpLevels.Count; bpLevelID++)
                    {
                        BattlePassLevel currentBpLevel = currentBpLevels[bpLevelID];
                        BattlePassLevel previousBpLevel = previousBattlePassLevels[bpLevelID];


                        // Check if was unlocked
                        bool wasBpUnlocked =
                            null == previousBpLevel.activated_at && null != currentBpLevel.activated_at;
                        if (wasBpUnlocked)
                        {
                            onUnlocked.Invoke();
                        }

                        // check if level was completed
                        bool wasLevelCompleted = currentBpLevel.level_completed !=
                                                 previousBpLevel.level_completed;
                        if (wasLevelCompleted)
                        {
                            onLevelCompleted.Invoke();
                        }
                    }
                }

                StoredBattlePassLevels[battlePassID] = currentBpLevels;
            }
        }
    }
}
