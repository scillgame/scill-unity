using System;
using System.Collections.Generic;
using SCILL.Model;
using UnityEngine;
using UnityEngine.Assertions;

namespace SCILL.Effects
{
    [RequireComponent(typeof(SCILLBattlePassEvents))]
    public class SCILLBattlePassAudio : SCILLAudioBase
    {
        private SCILLBattlePassEvents _bpEvents;

        private void Awake()
        {
            _bpEvents = GetComponent<SCILLBattlePassEvents>();
            Assert.IsNotNull(_bpEvents, "SCILLBattlePassAudio Script on object " + gameObject.name + " requires a SCILLBattlePassEvents script.");
        }

        protected void OnEnable()
        {
            SCILLBattlePassManager.OnBattlePassChallengeUpdate += OnBattlePassChallengeUpdate;
            SCILLBattlePassManager.OnBattlePassLevelRewardClaimed += OnBattlePassLevelRewardClaimed;
            _bpEvents.onUnlocked.AddListener(OnBattlePassUnlocked);
            _bpEvents.onLevelCompleted.AddListener(OnLevelCompleted);
        }
        
        protected void OnDisable()
        {
            SCILLBattlePassManager.OnBattlePassChallengeUpdate -= OnBattlePassChallengeUpdate;
            SCILLBattlePassManager.OnBattlePassLevelRewardClaimed -= OnBattlePassLevelRewardClaimed;
            _bpEvents.onUnlocked.RemoveListener(OnBattlePassUnlocked);
            _bpEvents.onLevelCompleted.RemoveListener(OnLevelCompleted);
        }

        private void OnLevelCompleted()
        {
            Play(audioSettings.BattlePassLevelCompletedSound);
        }

        private void OnBattlePassUnlocked()
        {
            Play(audioSettings.BattlePassUnlockedSound);
        }

        private void OnBattlePassChallengeUpdate(BattlePassChallengeChangedPayload challengechangedpayload)
        {
            Play(audioSettings.BattlePassLevelChallengeUpdatedSound);
        }

        

        private void OnBattlePassLevelRewardClaimed(BattlePassLevel level)
        {
            Play(audioSettings.BattlePassLevelRewardClaimedSound);
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