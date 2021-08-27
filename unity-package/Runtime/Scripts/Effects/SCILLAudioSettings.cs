using System.Collections.Generic;
using UnityEngine;

namespace SCILL.Effects
{
    [CreateAssetMenu(fileName = "SCILLAudioSettings", menuName = "SCILL/AudioSettings", order = 0)]
    public class SCILLAudioSettings : ScriptableObject
    {
        [Header("Personal Challenge Sounds")] [SerializeField]
        private AudioClip challengeUnlockedSound;
        [SerializeField] private AudioClip challengeActivatedSound;
        [SerializeField] private AudioClip challengeOvertimeSound;
        [SerializeField] private AudioClip challengeUnclaimedSound;
        [SerializeField] private AudioClip challengeFinishedSound;

        [Header("Battle Pass Sounds")] [SerializeField]
        private AudioClip battlePassUnlockedSound;
        [SerializeField] private AudioClip battlePassLevelCompletedSound;
        [SerializeField] private AudioClip battlePassLevelRewardClaimedSound;
        
        public AudioClip BattlePassLevelCompletedSound => battlePassLevelCompletedSound;
        public AudioClip BattlePassLevelRewardClaimedSound => battlePassLevelRewardClaimedSound;
        public AudioClip BattlePassUnlockedSound => battlePassUnlockedSound;

        private Dictionary<string, AudioClip> _challengeClips = new Dictionary<string, AudioClip>();

        public void OnEnable()
        {
            _challengeClips["unlocked"] = challengeUnlockedSound;
            _challengeClips["in-progress"] = challengeActivatedSound;
            _challengeClips["overtime"] = challengeOvertimeSound;
            _challengeClips["unclaimed"] = challengeUnclaimedSound;
            _challengeClips["finished"] = challengeFinishedSound;
        }

        public AudioClip GetChallengeAudioFromType(string type)
        {
            AudioClip result = null;
            if (_challengeClips.ContainsKey(type))
            {
                result = _challengeClips[type];
            }

            return result;
        }
    }
}