using System.Collections.Generic;
using SCILL.Model;
using UnityEngine;

namespace SCILL.Effects
{
    public class SCILLBattlePassAudio : SCILLAudioBase
    {
        protected string RegisteredBattlePassID;

        protected Dictionary<string, List<BattlePassLevel>> StoredBattlePassLevels =
            new Dictionary<string, List<BattlePassLevel>>();

        protected Dictionary<string, BattlePass> StoredBattlePasses =
            new Dictionary<string, BattlePass>();

        protected void OnEnable()
        {
            SCILLBattlePassManager.OnBattlePassLevelsUpdatedFromServer += BattlePassLevelsUpdated;
            SCILLBattlePassManager.OnBattlePassLevelRewardClaimed += OnBattlePassLevelRewardClaimed;
            SCILLBattlePassManager.OnBattlePassUpdatedFromServer += OnBattlePassUpdated;
        }

        private void OnBattlePassUpdated(BattlePass battlepass)
        {
            string battlePassID = battlepass.battle_pass_id;
            if (StoredBattlePasses.ContainsKey(battlePassID) && null != StoredBattlePasses[battlePassID])
            {
                BattlePass previousBp = StoredBattlePasses[battlePassID];
                if (null == previousBp.unlocked_at && null != battlepass.unlocked_at)
                {
                    Play(audioSettings.BattlePassUnlockedSound);
                }
            }

            StoredBattlePasses[battlePassID] = battlepass;
        }

        private void OnBattlePassLevelRewardClaimed(BattlePassLevel level)
        {
            Play(audioSettings.BattlePassLevelRewardClaimedSound);
        }

        protected void OnDisable()
        {
            SCILLBattlePassManager.OnBattlePassLevelsUpdatedFromServer -= BattlePassLevelsUpdated;
            SCILLBattlePassManager.OnBattlePassLevelRewardClaimed -= OnBattlePassLevelRewardClaimed;
            SCILLBattlePassManager.OnBattlePassUpdatedFromServer -= OnBattlePassUpdated;
        }

        private void BattlePassLevelsUpdated(List<BattlePassLevel> currentBpLevels)
        {
            if (currentBpLevels.Count > 0)
            {
                string battlePassID = currentBpLevels[0].battle_pass_id;
                if (StoredBattlePassLevels.ContainsKey(battlePassID) && null != StoredBattlePassLevels[battlePassID])
                {
                    List<BattlePassLevel> previousBattlePassLevels = StoredBattlePassLevels[battlePassID];
                    for (int bpLevelID = 0; bpLevelID < currentBpLevels.Count; bpLevelID++)
                    {
                        BattlePassLevel currentBpLevel = currentBpLevels[bpLevelID];
                        BattlePassLevel previousBpLevel = previousBattlePassLevels[bpLevelID];

                        AudioClip feedbackAudioClip = null;

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

                        Play(feedbackAudioClip);
                    }
                }

                StoredBattlePassLevels[battlePassID] = currentBpLevels;
            }
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