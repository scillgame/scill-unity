using System;
using System.Collections.Generic;
using SCILL.Client;
using SCILL.Model;
using UnityEngine;
using UnityEngine.UI;

namespace SCILL
{
    public enum SCILLMemberType
    {
        User,
        Team
    }

    public class SCILLLeaderboard : MonoBehaviour
    {
        [Tooltip("The ID of the leaderboard. You find them in the Admin Panel.")]
        public string leaderboardId;

        [Tooltip("How many rankings are considered to be top ranks. Default is 3.")]
        public int numberOfTopEntries = 3;

        [Tooltip("Is this leaderboard grouped by users or teams")]
        public SCILLMemberType memberType = SCILLMemberType.User;

        [Tooltip("Set the number of decimals to shift the score")]
        public int numberOfDecimals = 0;

        [Tooltip("Defines how many items are loaded per page")]
        public int pageSize = 25;

        [Header("UI Connections")]
        [Tooltip("Link a UI.Text field that will be set with the leaderboards name adjustable in the Admin Panel.")]
        public Text leaderboardName;

        [Tooltip(
            "Set the container that ranking prefabs will be added to. Should have a VerticalLayoutGroup element for auto layouting the elements")]
        public Transform rankingsContainer;

        [Tooltip(
            "Connect an exposed user ranking item (for example in the header) which will be set with the users leaderboard entry.")]
        public SCILLLeaderboardRankingItem userRanking;

        [Header("Building Blocks")] [Tooltip("The prefab of a UI item that will be used for the users ranking entry")]
        public SCILLLeaderboardRankingItem userRankingPrefab;

        [Tooltip("The prefab of a UI item that will be used for the top ranking entries")]
        public SCILLLeaderboardRankingItem topRankingPrefab;

        [Tooltip("The prefab of a UI item that will be used for any other rankings")]
        public SCILLLeaderboardRankingItem defaultRankingPrefab;

        public delegate void UsersLeaderboardRankingChangedAction(LeaderboardUpdatePayload payload);

        public static event UsersLeaderboardRankingChangedAction OnUsersLeaderboardRankingChanged;

        public int CurrentPage { get; private set; }

        private float contentSize = 0;
        private bool allContentLoaded = false;
        private ScrollRect _scrollRect;

        private bool IsLoading { get; set; } = false;

        private void Awake()
        {
            _scrollRect = GetComponentInChildren<ScrollRect>();
        }

        private void Start()
        {
            if (null != SCILLManager.Instance.SCILLClient)
            {
                InitLeaderboardData();
            }
            else
            {
                SCILLManager.OnSCILLManagerReady += OnScillReady;
            }
        }

        private void OnScillReady()
        {
            InitLeaderboardData();
            UpdateLeaderboard();
        }

        private void InitLeaderboardData()
        {
            SCILLManager.OnSCILLManagerReady -= InitLeaderboardData;

            PollLeaderboard();
            SCILLManager.Instance.StartLeaderboardUpdateNotifications(leaderboardId, OnLeaderboardUpdated);
        }

        private void OnDestroy()
        {
            SCILLManager.OnSCILLManagerReady -= InitLeaderboardData;

            SCILLManager.Instance.StopLeaderboardUpdateNotifications(leaderboardId, OnLeaderboardUpdated);
        }

        private void OnLeaderboardUpdated(LeaderboardUpdatePayload payload)
        {
            // Make sure this this leaderboard has been updated
            if (payload.leaderboard_data.leaderboard_id != leaderboardId)
            {
                return;
            }

            PollLeaderboard();

            if (payload.member_data.member_type == "user" &&
                payload.member_data.member_id == SCILLManager.Instance.GetUserId())
            {
                OnUsersLeaderboardRankingChanged?.Invoke(payload);
            }
        }

        private void PollLeaderboard()
        {
            // Don't poll while we are loading new content
            if (IsLoading)
            {
                return;
            }

            // Reload all data currently visible
            var leaderboardPromise =
                SCILLManager.Instance.SCILLClient.GetLeaderboardAsync(leaderboardId, 1, pageSize * CurrentPage);

            leaderboardPromise.Then(leaderboard =>
            {
                List<LeaderboardRanking> rankings = (memberType == SCILLMemberType.User)
                    ? leaderboard.grouped_by_users
                    : leaderboard.grouped_by_teams;
                ClearRankings();
                AddLeaderItems(rankings);
            });
        }

        private void AddLeaderItems(List<LeaderboardRanking> rankings)
        {
            foreach (var ranking in rankings)
            {
                //Debug.Log("Adding ranking " + ranking.rank);
                SCILLLeaderboardRankingItem prefab = defaultRankingPrefab;
                if (ranking.member_id == SCILLManager.Instance.GetUserId())
                {
                    prefab = userRankingPrefab;
                }
                else if (ranking.rank <= numberOfTopEntries)
                {
                    prefab = topRankingPrefab;
                }

                GameObject rankingGo = Instantiate(prefab.gameObject, rankingsContainer.transform, false);
                var rankingItem = rankingGo.GetComponent<SCILLLeaderboardRankingItem>();
                if (rankingItem)
                {
                    rankingItem.numberOfDecimals = numberOfDecimals;
                    rankingItem.ranking = ranking;
                }
            }
        }

        public void UpdateLeaderboard()
        {
            CurrentPage = 1;
            allContentLoaded = false;
            contentSize = 0;

            if (SCILLManager.Instance == null || null == SCILLManager.Instance.SCILLClient)
            {
                return;
            }

            LoadLeaderboardRankings(CurrentPage, true);
            UpdateUsersLeaderboardRanking();
        }

        private void UpdateUsersLeaderboardRanking()
        {
            if (!userRanking)
            {
                return;
            }

            // Load users ranking in this leaderboard
            try
            {
                var leaderboardRankingPromise =
                    SCILLManager.Instance.SCILLClient.GetLeaderboardRankingAsync("user",
                        SCILLManager.Instance.GetUserId(),
                        leaderboardId);

                leaderboardRankingPromise.Then(leaderboardRanking =>
                {
                    // If user is not in leaderboard, hide it
                    if (leaderboardRanking.member.rank < 0)
                    {
                        userRanking.gameObject.SetActive(false);
                    }
                    else
                    {
                        // Update the header element with the latest ranking values
                        userRanking.numberOfDecimals = numberOfDecimals;
                        userRanking.ranking = leaderboardRanking.member;
                        userRanking.gameObject.SetActive(true);
                    }
                });
            }
            catch (ApiException e)
            {
                Debug.Log("Failed to load users leaderboard: " + e.Message);
                throw;
            }
        }

        private void LoadLeaderboardRankings(int page, bool clear = false)
        {
            if (null != SCILLManager.Instance.SCILLClient)
            {
                //Debug.Log("LOAD LEADERBOARD RANKINGS " + page);
                var loadPromise = SCILLManager.Instance.SCILLClient.GetLeaderboardAsync(leaderboardId, page, pageSize);
                IsLoading = true;

                loadPromise.Then(leaderboard =>
                {
                    //Debug.Log(leaderboard.ToJson());

                    if (leaderboardName)
                    {
                        leaderboardName.text = leaderboard.name;
                    }

                    List<LeaderboardRanking> rankings = (memberType == SCILLMemberType.User)
                        ? leaderboard.grouped_by_users
                        : leaderboard.grouped_by_teams;

                    //Debug.Log("Loaded leaderboard rankings, number of items: " + rankings.Count + ", Page-Size: " + pageSize);

                    // Make sure we stop loading new stuff if we are at the end of the list
                    if (rankings.Count < pageSize)
                    {
                        allContentLoaded = true;
                    }

                    if (clear)
                    {
                        ClearRankings();
                    }

                    AddLeaderItems(rankings);

                    IsLoading = false;
                });
            }
        }

        protected virtual void AddNextPage()
        {
            // Dont load new data if we are at the end of the list
            if (allContentLoaded || IsLoading)
            {
                //Debug.Log("Skipping adding next page, Loading: " + loading + ", All content loaded: " + allContentLoaded);
                return;
            }

            CurrentPage++;

            LoadLeaderboardRankings(CurrentPage);
        }

        private void OnEnable()
        {
            UpdateLeaderboard();
        }

        protected virtual void ClearRankings()
        {
            //Debug.Log("Clearing rankings list");

            // Make sure we delete all items from the battle pass levels container
            // This way we can leave some dummy level items in Unity Editor which makes it easier to design UI
            foreach (SCILLLeaderboardRankingItem child in rankingsContainer
                .GetComponentsInChildren<SCILLLeaderboardRankingItem>())
            {
                Destroy(child.gameObject);
            }
        }

        public void Update()
        {
            if (_scrollRect)
            {
                if (_scrollRect.normalizedPosition.y < 0.25)
                {
                    if (IsLoading == false && allContentLoaded == false)
                    {
                        //Debug.Log("Bottom of page reached, adding next page, Loading: " + loading + ", All Content Loaded: " + allContentLoaded);
                        AddNextPage();
                    }
                }
            }
        }
    }
}