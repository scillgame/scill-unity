using System;
using SCILL.Client;
using SCILL.Model;
using UnityEngine;
using UnityEngine.Events;

namespace SCILL
{
    /// <summary>
    ///     <para>
    ///         This is a utility script  for subscribing to realtime updates when a specific leaderboard was first
    ///         loaded
    ///         from the SCILL
    ///         backend and for subscribing to realtime changes to the leaderboard.
    ///     </para>
    ///     <para>
    ///         When being enabled, this script will request a full leaderboard reload and call the
    ///         <see cref="OnLeaderboardRankingLoaded" /> unity event on response. Changes to the leaderboard will be
    ///         broadcasted to the <see cref="OnLeaderboardRankingChanged" /> unity event. Use this script if you'd like to
    ///         have access to those events over the inspector or via code.
    ///     </para>
    ///     <para>
    ///         Please note that <c>SCILLLeaderboardManager</c> is designed to work with leaderboards of type <c>user</c> only,
    ///         not for type <c>teams</c>.
    ///     </para>
    /// </summary>
    public class SCILLLeaderboardManager : MonoBehaviour
    {
        /// <summary>
        ///     The leaderboard id for which you'd like to receive realtime updates.
        /// </summary>
        public string leaderboardId;

        /// <summary>
        ///     Called when receiving a full reload of the <see cref="Leaderboard" /> with the supplied
        ///     <see cref="leaderboardId" />.
        /// </summary>
        [SerializeField] public LeaderboardRankingLoaded OnLeaderboardRankingLoaded;

        /// <summary>
        ///     Called when receiving a realtime update to the <see cref="Leaderboard" /> with the supplied
        ///     <see cref="leaderboardId" />.
        /// </summary>
        [SerializeField] public LeaderboardRankingChanged OnLeaderboardRankingChanged;

        private void OnEnable()
        {
            if (leaderboardId != null)
            {
                if (!string.IsNullOrEmpty(SCILLManager.Instance.AccessToken))
                    InitLeaderboardData();
                else
                    SCILLManager.OnSCILLManagerReady += InitLeaderboardData;
            }
        }

        private void OnDisable()
        {
            SCILLManager.OnSCILLManagerReady -= InitLeaderboardData;

            SCILLManager.Instance.StopLeaderboardUpdateNotifications(leaderboardId, OnLeaderboardChangedPayload);
        }

        private void InitLeaderboardData()
        {
            SCILLManager.OnSCILLManagerReady -= InitLeaderboardData;

            LoadPersonalRanking();
            SCILLManager.Instance.StartLeaderboardUpdateNotifications(leaderboardId, OnLeaderboardChangedPayload);
        }


        private void OnLeaderboardChangedPayload(LeaderboardUpdatePayload payload)
        {
            // Debug.Log(payload.ToJson());
            var userId = SCILLManager.Instance.GetUserId();
            var memberId = payload.member_data.member_id;
            // Debug.Log("MEMBER TYPE: " + payload.member_data.member_type);
            // Debug.Log("User-ID: " + userId + ", Member-ID: " + memberId);
            if (payload.member_data.member_type == "user" &&
                payload.member_data.member_id == SCILLManager.Instance.GetUserId())
                // Debug.Log("LEADERBOARD UPDATED FOR USER");
                OnLeaderboardRankingChanged?.Invoke(payload);
        }

        private void LoadPersonalRanking()
        {
            try
            {
                SCILLManager.Instance.GetPersonalRankingAsync(
                    memberRanking =>
                    {
                        if (memberRanking != null && memberRanking.member != null && memberRanking.member.rank >= 1)
                            OnLeaderboardRankingLoaded?.Invoke(memberRanking.member);
                    },
                    e =>
                    {
                        if (e is ApiException apiException)
                        {
                            HandleApiException(apiException);
                        }
                        else
                        {
                            Debug.LogError(e);
                            throw e;
                        }
                    },
                    leaderboardId);
            }
            catch (ApiException e)
            {
                HandleApiException(e);
            }
        }

        private void HandleApiException(ApiException e)
        {
            Debug.Log("Failed to load leaderboard ranking " + leaderboardId);
            Debug.Log(e.Message);
        }

        [Serializable]
        public class LeaderboardRankingLoaded : UnityEvent<LeaderboardRanking>
        {
        }

        [Serializable]
        public class LeaderboardRankingChanged : UnityEvent<LeaderboardUpdatePayload>
        {
        }
    }
}