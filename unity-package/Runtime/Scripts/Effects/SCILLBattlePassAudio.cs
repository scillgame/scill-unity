using System.Collections.Generic;
using SCILL.Model;
using UnityEngine;

namespace SCILL.Effects
{
    public class SCILLBattlePassAudio : SCILLAudioBase
    {
        protected Dictionary<string, List<BattlePassLevel>> StoredBattlePassLevels =
            new Dictionary<string, List<BattlePassLevel>>();

        protected void OnEnable()
        {
            SCILLBattlePassManager.OnBattlePassLevelsUpdatedFromServer += BattlePassLevelsUpdated;
            SCILLBattlePassManager.OnBattlePassLevelRewardClaimed += OnBattlePassLevelRewardClaimed;
            SCILLBattlePassManager.OnBattlePassChallengeUpdate += OnBattlePassChallengeUpdate;
        }

        private void OnBattlePassChallengeUpdate(BattlePassChallengeChangedPayload challengechangedpayload)
        {
            Play(audioSettings.BattlePassLevelChallengeUpdatedSound);
        }

        protected void OnDisable()
        {
            SCILLBattlePassManager.OnBattlePassLevelsUpdatedFromServer -= BattlePassLevelsUpdated;
            SCILLBattlePassManager.OnBattlePassLevelRewardClaimed -= OnBattlePassLevelRewardClaimed;
        }

        private void OnBattlePassLevelRewardClaimed(BattlePassLevel level)
        {
            Play(audioSettings.BattlePassLevelRewardClaimedSound);
        }


        private void BattlePassLevelsUpdated(List<BattlePassLevel> currentBpLevels)
        {
            AudioClip feedbackAudioClip = null;

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
                            feedbackAudioClip = audioSettings.BattlePassUnlockedSound;
                        }

                        // check if level was completed
                        bool wasLevelCompleted = currentBpLevel.level_completed !=
                                                 previousBpLevel.level_completed;
                        if (wasLevelCompleted)
                        {
                            feedbackAudioClip = audioSettings.BattlePassLevelCompletedSound;
                        }

                    }
                }

                StoredBattlePassLevels[battlePassID] = currentBpLevels;
            }
            
            Play(feedbackAudioClip);
        }

        private void Play(AudioClip feedbackAudioClip)
        {
            if (feedbackAudioClip && audioSource)
            {
                audioSource.PlayOneShot(feedbackAudioClip);
            }
        }
    }
}