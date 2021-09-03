using System;
using System.Collections;
using System.Collections.Generic;
using SCILL;
using SCILL.Effects;
using SCILL.Model;
using UnityEngine;

public class SCILLLeaderboardAudio : SCILLAudioBase
{
    [SerializeField] private string leaderboardID;

    private void OnEnable()
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

    private void OnDisable()
    {
        SCILLManager.OnSCILLManagerReady -= RegisterToNotifications;
        UnRegisterFromNotifications();
    }

    private void RegisterToNotifications()
    {
        if (!string.IsNullOrEmpty(leaderboardID))
        {
            SCILLManager.Instance.StartLeaderboardUpdateNotifications(leaderboardID, OnLeaderboardUpdate);
        }
    }

    private void UnRegisterFromNotifications()
    {
        if (!string.IsNullOrEmpty(leaderboardID))
        {
            SCILLManager.Instance.StopLeaderboardUpdateNotifications(leaderboardID, OnLeaderboardUpdate);
        }
    }

    private void OnLeaderboardUpdate(LeaderboardUpdatePayload payload)
    {
        if (payload.leaderboard_data.leaderboard_id == leaderboardID)
        {
            bool wasCurrentUserUpdated = payload.member_data.member_type == "user" &&
                payload.member_data.member_id == SCILLManager.Instance.GetUserId();

            int? oldRank = payload.old_leaderboard_ranking?.rank;
            int? newRank = payload.new_leaderboard_ranking?.rank;
            bool didUserRankChange = oldRank != newRank;
            if (wasCurrentUserUpdated && didUserRankChange)
            {
                Play(audioSettings.LeaderboardUserRankingUpdatedSound);
            }
        }
    }
}