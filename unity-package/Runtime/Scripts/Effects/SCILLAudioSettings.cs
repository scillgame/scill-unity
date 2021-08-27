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
        [field: SerializeField] public AudioClip ChallengeUpdatedSound { get; private set; }


        [field: Header("Battle Pass Sounds")]
        [field: SerializeField]
        public AudioClip BattlePassUnlockedSound { get; private set; }

        [field: SerializeField] public AudioClip BattlePassLevelCompletedSound { get; private set; }
        [field: SerializeField] public AudioClip BattlePassLevelRewardClaimedSound { get; private set; }
        [field: SerializeField] public AudioClip BattlePassLevelChallengeUpdatedSound { get; private set; }


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