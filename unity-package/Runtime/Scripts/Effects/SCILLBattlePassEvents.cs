using System;
using System.Collections.Generic;
using SCILL;
using SCILL.Model;
using UnityEngine;
using UnityEngine.Events;

namespace SCILL.Effects
{
    public class SCILLBattlePassEvents : MonoBehaviour
    {
        [SerializeField] public UnityEvent onUnlocked;
        [SerializeField] public UnityEvent onChallengeProgressUpdated;
        [SerializeField] public UnityEvent onLevelRewardClaimed;
        [SerializeField] public UnityEvent onLevelCompleted;
        
    
        protected Dictionary<string, List<BattlePassLevel>> StoredBattlePassLevels =
            new Dictionary<string, List<BattlePassLevel>>();

        private void OnEnable()
        {
            SCILLBattlePassManager.OnBattlePassLevelsUpdatedFromServer += OnBattlePassLevelsUpdated;
            SCILLBattlePassManager.OnBattlePassLevelRewardClaimed += OnBattlePassLevelRewardClaimed;
            SCILLBattlePassManager.OnBattlePassChallengeUpdate += OnBattlePassChallengeUpdate;
        }

        private void OnDisable()
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
