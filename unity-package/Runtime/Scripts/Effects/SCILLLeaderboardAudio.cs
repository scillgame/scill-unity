using SCILL;
using SCILL.Effects;
using SCILL.Model;
using UnityEngine;

namespace Scripts.Effects
{
    /// <summary>
    /// Sample script for playing feedback audio clips on Leaderboard realtime events. 
    /// Uses the data provided by the <see cref="AudioSettings"/> scriptable object to play sound effects for events fired off by the <see cref="SCILLBattlePassEvents"/> script.
    /// <remarks>
    /// This script will only play effects for the leaderboard identified by the <see cref="leaderboardID"/>.
    /// </remarks>
    /// </summary>
    public class SCILLLeaderboardAudio : SCILLAudioBase
    {
        /// <summary>
        /// Id of the leaderboard for which the sound effects should be played.
        /// </summary>
        [SerializeField] private string leaderboardID;

        protected virtual void OnEnable()
        {
            if (SCILLManager.Instance)
            {
                RegisterToNotifications();
            }
            else
            {
                SCILLManager.OnSCILLManagerReady += RegisterToNotifications;
            }
        }

        protected virtual void OnDisable()
        {
            SCILLManager.OnSCILLManagerReady -= RegisterToNotifications;
            UnRegisterFromNotifications();
        }

        /// <summary>
        /// Start listening to leaderboard update notifications for the leaderboard with the id <see cref="leaderboardID"/>.
        /// </summary>
        protected virtual void RegisterToNotifications()
        {
            if (!string.IsNullOrEmpty(leaderboardID))
            {
                SCILLManager.Instance.StartLeaderboardUpdateNotifications(leaderboardID, OnLeaderboardUpdate);
            }
        }

        /// <summary>
        /// Stop listening to leaderboard update notifications for the leaderboard with the id <see cref="leaderboardID"/>.
        /// </summary>
        protected virtual void UnRegisterFromNotifications()
        {
            if (!string.IsNullOrEmpty(leaderboardID))
            {
                SCILLManager.Instance.StopLeaderboardUpdateNotifications(leaderboardID, OnLeaderboardUpdate);
            }
        }

        /// <summary>
        /// Called on updates to the leaderboard. Will check if the current user's ranking has changed and if yes,
        /// it will play the audio clip referenced in the <see cref="AudioSettings"/> object.
        /// </summary>
        /// <param name="payload">Realtime update data.</param>
        protected virtual void OnLeaderboardUpdate(LeaderboardUpdatePayload payload)
        {
            if (payload.leaderboard_data.leaderboard_id == leaderboardID)
            {
                bool wasCurrentUserUpdated = payload.member_data.member_type == "user" &&
                                             payload.member_data.member_id == SCILLManager.Instance.GetUserId();

                int? oldRank = payload.old_leaderboard_ranking?.rank;
                int? newRank = payload.new_leaderboard_ranking?.rank;
                bool didUserRankUp = oldRank > newRank;
                if (wasCurrentUserUpdated && didUserRankUp)
                {
                    Play(audioSettings.LeaderboardUserRankingUpdatedSound);
                }
            }
        }
    }
}