using System;
using System.Collections;
using SCILL.Client;
using SCILL.Model;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

namespace SCILL.Effects
{
    /// <summary>
    /// Simple Utility class for accessing Leaderboard related realtime events in the editor via <c>UnityEvent</c>s. This script
    /// connects to <see cref="SCILLBattlePassManager"/> events, which are only called for the currently <see cref="SCILLBattlePassManager.SelectedBattlePass"/>.
    /// </summary>
    public class SCILLLeaderboardEvents : MonoBehaviour
    {
        /// <summary>
        /// Id of the leaderboard for which the sound effects should be played.
        /// </summary>
        [FormerlySerializedAs("leaderboardID")] [SerializeField]
        private string leaderboardId;

        /// <summary>
        /// Invoked, when the current users ranking is smaller than before.
        /// </summary>
        [SerializeField] public UnityEvent onUserRankingDecreased;

        /// <summary>
        /// Invoked, when the current users ranking is larger than before.
        /// </summary>
        [SerializeField] public UnityEvent onUserRankingIncreased;


        /// <summary>
        /// The current users leaderboard ranking in the leaderboard with id <see cref="leaderboardId"/>
        /// </summary>
        [Obsolete("Please use CurrentUserMemberRanking instead.")]
        protected LeaderboardRanking CurrentUserRanking;

        /// <summary>
        /// The current users leaderboard ranking in the leaderboard with id <see cref="leaderboardId"/>
        /// </summary>
        protected LeaderboardMember CurrentUserMember;

        protected virtual void OnEnable()
        {
            if (SCILLManager.Instance.IsConnected)
            {
                InitializeLeaderboardEvents();
            }
            else
            {
                SCILLManager.OnSCILLManagerReady += InitializeLeaderboardEvents;
            }
        }

        protected virtual void OnDisable()
        {
            SCILLManager.OnSCILLManagerReady -= InitializeLeaderboardEvents;
            UnRegisterFromNotifications();
        }

        /// <summary>
        /// Start listening to leaderboard update notifications for the leaderboard with the id <see cref="leaderboardId"/>.
        /// </summary>
        protected virtual void InitializeLeaderboardEvents()
        {
            StartCoroutine(DelayedInitializeLeaderboardEvents());
        }

        /// <summary>
        /// Coroutine: Waits a frame before start listening to leaderboard update notifications for the leaderboard with the id <see cref="leaderboardId"/>.
        /// </summary>
        protected IEnumerator DelayedInitializeLeaderboardEvents()
        {
            yield return null;
            if (!string.IsNullOrEmpty(leaderboardId))
            {
                SCILLManager.Instance.StartLeaderboardUpdateNotifications(leaderboardId, OnLeaderboardUpdate);
                RequestCurrentUserLeaderboardRank();
            }
        }


        /// <summary>
        /// Utility function for requesting the current users leaderboard ranking, using the
        /// <see cref="leaderboardId"/> and the current users UserId. The resolve callback
        /// function is set to <see cref="OnReceivedCurrentUserRanking"/>, which you can override.
        /// </summary>
        protected void RequestCurrentUserLeaderboardRank()
        {
            if (SCILLManager.Instance && null != SCILLManager.Instance.LeaderboardsApi &&
                !string.IsNullOrEmpty(leaderboardId))
            {
                SCILLManager.Instance.LeaderboardsApi.GetLeaderboardRankingAsync(
                    OnReceivedCurrentUserRanking,
                    OnGetLeaderboardRankingRejected,
                    "user",
                    SCILLManager.Instance.UserId,
                    leaderboardId);
            }
        }

        private void OnGetLeaderboardRankingRejected(Exception exception)
        {
            ApiException apiException = exception as ApiException;
            if (null != apiException && 404 == apiException.ErrorCode)
            {
                Debug.Log(
                    $"User with id {SCILLManager.Instance.UserId} not present in Leaderboard with id {leaderboardId}");
            }
            else
            {
                Debug.LogError(exception.Message);
            }
        }


        /// <summary>
        /// Callback function for the leaderboard ranking request for the current user.
        /// </summary>
        /// <param name="ranking">The current user's leaderboard ranking.</param>
        protected virtual void OnReceivedCurrentUserRanking(LeaderboardMemberRanking ranking)
        {
#pragma warning disable 618
            CurrentUserRanking = ranking.leaderboard_member.ToLeaderboardRanking();
#pragma warning restore 618
            CurrentUserMember = ranking.leaderboard_member;
        }


        /// <summary>
        /// Stop listening to leaderboard update notifications for the leaderboard with the id <see cref="leaderboardId"/>.
        /// </summary>
        protected virtual void UnRegisterFromNotifications()
        {
            if (!string.IsNullOrEmpty(leaderboardId))
            {
                SCILLManager.Instance.StopLeaderboardUpdateNotifications(leaderboardId, OnLeaderboardUpdate);
            }
        }

        /// <summary>
        /// Called when receiving realtime leaderboard updates from the SCILL backend.
        /// </summary>
        /// <param name="payload">The received realtime update payload.</param>
        protected virtual void OnLeaderboardUpdate(LeaderboardUpdatePayload payload)
        {
            if (payload.leaderboard_data.leaderboard_id == leaderboardId)
            {
                CheckUserRankingDecreased(payload);
                CheckUserRankingIncreased(payload);
            }
        }

        private void CheckUserRankingIncreased(LeaderboardUpdatePayload payload)
        {
            int? oldRank = payload.old_leaderboard_ranking?.rank;
            int? newRank = payload.new_leaderboard_ranking?.rank;
            if (null != CurrentUserMember && oldRank > CurrentUserMember.rank && newRank <= CurrentUserMember.rank)
            {
                onUserRankingIncreased.Invoke();
                RequestCurrentUserLeaderboardRank();
            }
        }

        private void CheckUserRankingDecreased(LeaderboardUpdatePayload payload)
        {
            bool wasCurrentUserUpdated = payload.member_data.member_type == "user" &&
                                         payload.member_data.member_id == SCILLManager.Instance.GetUserId();

            int? oldRank = payload.old_leaderboard_ranking?.rank;
            int? newRank = payload.new_leaderboard_ranking?.rank;
            if (wasCurrentUserUpdated && newRank < oldRank)
            {
                onUserRankingDecreased.Invoke();
            }
        }
    }
}