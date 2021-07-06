﻿using System;
using System.Collections;
using System.Collections.Generic;
using SCILL.Client;
using SCILL.Model;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace SCILL
{
    public class SCILLLeaderboardManager : MonoBehaviour
    {
        public string leaderboardId;

        [Serializable]
        public class LeaderboardRankingLoaded : UnityEvent<LeaderboardRanking>
        {
        }

        [SerializeField] public LeaderboardRankingLoaded OnLeaderboardRankingLoaded;

        [Serializable]
        public class LeaderboardRankingChanged : UnityEvent<LeaderboardUpdatePayload>
        {
        }

        [SerializeField] public LeaderboardRankingChanged OnLeaderboardRankingChanged;

        private void OnEnable()
        {
            if (leaderboardId != null)
            {
                if (!string.IsNullOrEmpty(SCILLManager.Instance.AccessToken))
                {
                    InitLeaderboardData();
                }
                else
                {
                    SCILLManager.OnSCILLManagerReady += InitLeaderboardData;
                }
            }
        }

        private void InitLeaderboardData()
        {
            SCILLManager.OnSCILLManagerReady -= InitLeaderboardData;

            LoadPersonalRanking();
            SCILLManager.Instance.StartLeaderboardUpdateNotifications(leaderboardId, OnLeaderboardChangedPayload);
        }

        private void OnDisable()
        {
            SCILLManager.OnSCILLManagerReady -= InitLeaderboardData;

            SCILLManager.Instance.StopLeaderboardUpdateNotifications(leaderboardId, OnLeaderboardChangedPayload);
        }


        private void OnLeaderboardChangedPayload(LeaderboardUpdatePayload payload)
        {
            Debug.Log(payload.ToJson());
            var userId = SCILLManager.Instance.GetUserId();
            var memberId = payload.member_data.member_id;
            Debug.Log("MEMBER TYPE: " + payload.member_data.member_type);
            Debug.Log("User-ID: " + userId + ", Member-ID: " + memberId);
            if (payload.member_data.member_type == "user" &&
                payload.member_data.member_id == SCILLManager.Instance.GetUserId())
            {
                Debug.Log("LEADERBOARD UPDATED FOR USER");
                OnLeaderboardRankingChanged?.Invoke(payload);
            }
        }

        private void LoadPersonalRanking()
        {
            try
            {
                SCILLManager.Instance.GetPersonalRankingAsync(memberRanking =>
                    {
                        if (memberRanking != null && memberRanking.member != null && memberRanking.member.rank >= 1)
                        {
                            OnLeaderboardRankingLoaded?.Invoke(memberRanking.member);
                        }
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
    }
}
