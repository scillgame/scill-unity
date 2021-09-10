using System;
using System.Collections.Generic;
using SCILL.Model;
using UnityEngine;
using UnityEngine.Assertions;

namespace SCILL.Effects
{
    /// <summary>
    /// Sample script for playing feedback audio clips on Battle Pass realtime events. 
    /// Uses the data provided by the <see cref="audioSettings"/> scriptable object to play sound effects for events fired off by the <see cref="SCILLBattlePassEvents"/> script.
    /// </summary>
    public class SCILLBattlePassAudio : SCILLAudioBase
    {
        /// <summary>
        /// Sounds will be played based on the events provided by this <see cref="SCILLBattlePassEvents"/> script.
        /// </summary>
        [SerializeField] private SCILLBattlePassEvents _bpEvents;

        protected override void Awake()
        {
            base.Awake();
            if (null == _bpEvents)
                _bpEvents = GetComponent<SCILLBattlePassEvents>();
            Assert.IsNotNull(_bpEvents,
                "SCILLBattlePassAudio Script on object " + gameObject.name +
                " requires a SCILLBattlePassEvents script.");
        }

        protected void OnEnable()
        {
            _bpEvents.onChallengeProgressUpdated.AddListener(OnBattlePassChallengeUpdate);
            _bpEvents.onLevelRewardClaimed.AddListener(OnBattlePassLevelRewardClaimed);
            _bpEvents.onUnlocked.AddListener(OnBattlePassUnlocked);
            _bpEvents.onLevelCompleted.AddListener(OnLevelCompleted);
        }

        protected void OnDisable()
        {
            _bpEvents.onChallengeProgressUpdated.RemoveListener(OnBattlePassChallengeUpdate);
            _bpEvents.onLevelRewardClaimed.RemoveListener(OnBattlePassLevelRewardClaimed);
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

        private void OnBattlePassChallengeUpdate()
        {
            Play(audioSettings.BattlePassLevelChallengeUpdatedSound);
        }

        private void OnBattlePassLevelRewardClaimed()
        {
            Play(audioSettings.BattlePassLevelRewardClaimedSound);
        }
    }
}