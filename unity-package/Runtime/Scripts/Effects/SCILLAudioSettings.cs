using System.Collections.Generic;
using SCILL.Model;
using UnityEngine;

namespace SCILL.Effects
{
    /// <summary>
    /// <para>
    /// Scriptable Object referencing the audioclips that should be played as feedback for SCILL related realtime events.
    /// </para>
    /// <para>
    /// Simply right click into your asset folder and select <c>Create/SCILL/Audiosettings</c> to create a new <c>SCILLAudioSettings</c> 
    /// file.
    /// </para>
    /// </summary>
    [CreateAssetMenu(fileName = "SCILLAudioSettings", menuName = "SCILL/AudioSettings", order = 0)]
    public class SCILLAudioSettings : ScriptableObject
    {
        
        /// <summary>
        /// Clip that is played when a challenge was unlocked.
        /// </summary>
        [Header("Personal Challenge Sounds")]
        [SerializeField]
        private AudioClip challengeUnlockedSound;

        /// <summary>
        /// Clip that is played when a challenge was activated.
        /// </summary>
        [SerializeField] private AudioClip challengeActivatedSound;
        /// <summary>
        /// Clip that is played when a challenge was canceled because of overtime.
        /// </summary>
        [SerializeField] private AudioClip challengeOvertimeSound;
        /// <summary>
        /// Clip that is played when the challenge goal was reached, but the challenge has yet to be claimed.
        /// Leave this empty, if challenges are automatically claimed in your game, as otherwise the <see cref="challengeFinishedSound"/> will
        /// potentially be played at the same time as this sound.
        /// </summary>
        [SerializeField] private AudioClip challengeUnclaimedSound;
        /// <summary>
        /// Clip that is played when a challenge was finished and has been played.
        /// </summary>
        [SerializeField] private AudioClip challengeFinishedSound;
        /// <summary>
        /// Clip that is played when a challenge's progress was updated.
        /// </summary>
        [field: SerializeField] public AudioClip ChallengeUpdatedSound { get; private set; }

        
        /// <summary>
        /// Clip that is played when a Battle Pass was unlocked.
        /// </summary>

        [field: Header("Battle Pass Sounds")]
        [field: SerializeField]
        public AudioClip BattlePassUnlockedSound { get; private set; }

        /// <summary>
        /// Clip that is played when a Battle Pass Level was completed.
        /// </summary>
        [field: SerializeField] public AudioClip BattlePassLevelCompletedSound { get; private set; }
        /// <summary>
        /// Clip that is played when a Battle Pass Level Reward was claimed. 
        /// </summary>
        [field: SerializeField] public AudioClip BattlePassLevelRewardClaimedSound { get; private set; }
        /// <summary>
        /// Clip that is played when the progress for any Battle Pass Level was updated.
        /// </summary>
        [field: SerializeField] public AudioClip BattlePassLevelChallengeUpdatedSound { get; private set; }

        /// <summary>
        /// Clip that is played when the current users leaderboard ranking changed.
        /// </summary>
        [field: Header("LeaderboardSounds Sounds")]
        [field: SerializeField]
        public AudioClip LeaderboardUserRankingUpdatedSound { get; private set; }


        private Dictionary<string, AudioClip> _challengeClips = new Dictionary<string, AudioClip>();

        protected void OnEnable()
        {
            _challengeClips["unlocked"] = challengeUnlockedSound;
            _challengeClips["in-progress"] = challengeActivatedSound;
            _challengeClips["overtime"] = challengeOvertimeSound;
            _challengeClips["unclaimed"] = challengeUnclaimedSound;
            _challengeClips["finished"] = challengeFinishedSound;
        }

        /// <summary>
        /// Retrieves the correct audio clip based on the <see cref="Challenge"/>'s <see cref="Challenge.type"/>. Currently valid types are:
        /// <c>unlocked</c>, <c>in-progress</c>, <c>overtime</c>, <c>unclaimed</c> and <c>finished</c>.
        /// </summary>
        /// <param name="type">The <see cref="Challenge"/>'s updated <see cref="Challenge.type"/>.</param>
        /// <returns>The <c>AudioClip</c> that should be played for the given <see cref="type"/> or null if no clip was found for the given <see cref="type"/>.</returns>
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