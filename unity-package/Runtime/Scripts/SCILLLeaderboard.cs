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

    /// <summary>
    ///     <para>
    ///         This component handles the communication with the SCILL backend to load leaderboards in real time.
    ///         It also implements user interfaces.
    ///     </para>
    ///     <para>
    ///         This class does two things:
    ///     </para>
    ///     <list type="bullet">
    ///         <item>
    ///             Load the <see cref="Leaderboard" /> defined by the <see cref="leaderboardId" /> and update it using the
    ///             SCILL realtime notifications.
    ///         </item>
    ///         <item>
    ///             Instantiate one of the ranking item prefabs (i.e. <see cref="defaultRankingPrefab" /> or
    ///             <see cref="topRankingPrefab" />) property for each <see cref="LeaderboardRanking" /> object contained in
    ///             the <a href="https://developers.scillgame.com/api/leaderboards.html#retrieve-leaderboard">response</a>  and
    ///             add it as a child to the <see cref="rankingsContainer" /> transform.
    ///         </item>
    ///     </list>
    ///     <para>
    ///         The prefabs like <see cref="defaultRankingPrefab" /> must have a <see cref="SCILLLeaderboardRankingItem" />
    ///         component attached that handles UI for each ranking item.
    ///     </para>
    ///     <para>
    ///         Sometimes, it’s not easy for players to find themselves in the leaderboard. To solve that, we added support
    ///         for the users position to be shown in the header of the leaderboard UI (or somewhere else that makes sense for
    ///         your own game). Use the <see cref="userRanking" /> inspector property to connect an
    ///         <see cref="SCILLLeaderboardRankingItem" /> component. The <c>SCILLLeaderboard</c> will update its values for
    ///         the
    ///         individual users and the whole leaderboard.
    ///     </para>
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         This leaderboard implements infinite scrolling via a <c>ScrollRect</c> which should be below the hierarchy of
    ///         this component. For an example of a valid hierarchy check out the Leaderboard Prefab supplied with the plugin
    ///         or the explanation given in
    ///         <a href="https://developers.scillgame.com/sdks/unity/classes/scillleaderboard.html#usage"> the Usage example.</a>
    ///     </para>
    ///     <para>
    ///         This script provides the <see cref="OnUsersLeaderboardRankingChanged" /> event for realtime notifications on
    ///         changes of the current users leaderboard ranking. For realtime notifications for all users, use the events
    ///         provided by the <see cref="SCILLLeaderboardManager" />.
    ///     </para>
    /// </remarks>
    public class SCILLLeaderboard : MonoBehaviour
    {
        public delegate void UsersLeaderboardRankingChangedAction(LeaderboardUpdatePayload payload);


        /// <summary>
        ///     You need to set a Leaderboard ID. You find the leaderboard ID in the Admin Panel in the Leaderboard list. You can
        ///     also change this value via script and implement tabs or a leaderboard selection that allows users to display
        ///     different leaderboards.
        /// </summary>
        /// <remarks>
        ///     You can change the leaderboard id via script everytime, but make sure you call the <see cref="UpdateLeaderboard" />
        ///     method so the
        ///     data is updated.
        /// </remarks>
        [Tooltip("The ID of the leaderboard. You find them in the Admin Panel.")]
        public string leaderboardId;

        /// <summary>
        ///     The leaderboard instantiates the <see cref="topRankingPrefab" /> for the first <c>numberOfTopEntries</c> items to
        ///     highlight them. Use this setting to decide how many items are shown highlighted.
        /// </summary>
        [Tooltip("How many rankings are considered to be top ranks. Default is 3.")]
        public int numberOfTopEntries = 3;


        /// <summary>
        ///     <para>
        ///         SCILL Leaderboards are generated automatically for users and teams. To update leaderboards for teams you need
        ///         to
        ///         set a <c>team_id</c> when sending events. See <a href="https://developers.scillgame.com/events.html">Events</a>
        ///         for more info. Use this setting to set if team rankings or
        ///         user rankings should be displayed.
        ///     </para>
        ///     <para>
        ///         You need to update the leaderboard with the <see cref="UpdateLeaderboard" /> method if you change this value
        ///         via script, which will also reset the pagination state, as team rankings contain much less items than user
        ///         rankings.
        ///     </para>
        /// </summary>
        [Tooltip("Is this leaderboard grouped by users or teams")]
        public SCILLMemberType memberType = SCILLMemberType.User;


        /// <summary>
        ///     <para>
        ///         SCILL Events do not support float values (see
        ///         <a href="https://developers.scillgame.com/events.html#why-no-floats">Why no floats explanation</a>).
        ///     </para>
        ///     <para>
        ///         But some
        ///         leaderboard scores like lap times are typically stored in a float value to capture also fractions of a second.
        ///         To
        ///         support this use case you can convert the float in your game to an integer value capturing the precision you
        ///         want
        ///         by multiplying with for example 100 or 1000. Then, set <c>numberOfDecimals</c> to <c>2</c> to divide the
        ///         leaderboard score by 100 or <c>3</c> to divide by 1000 before displaying it.
        ///     </para>
        /// </summary>
        [Tooltip("Set the number of decimals to shift the score")]
        public int numberOfDecimals;

        /// <summary>
        ///     Defines how many items are loaded and rendered at once. If you have a scrolling UI you should set this value
        ///     higher, if you use pagination you can set this value to exactly the number of items visible at once. Allowed values
        ///     are between <c>1</c> and <c>100</c>.
        /// </summary>
        [Tooltip("Defines how many items are loaded per page")]
        public int pageSize = 25;

        /// <summary>
        ///     Connect a <c>UnityEngine.UI.Text</c> component. The text will be set to the name of the leaderboard. This is useful
        ///     if you want to have multiple leaderboards or if you want to make use of SCILLs multi-language support.
        /// </summary>
        [Header("UI Connections")]
        [Tooltip("Link a UI.Text field that will be set with the leaderboards name adjustable in the Admin Panel.")]
        public Text leaderboardName;


        /// <summary>
        ///     Connect a <c>Transform</c> component. The prefabs for each ranking item will be instantiated into this transform.
        ///     Make sure you add an auto layout component like <c>VerticalLayoutGroup</c> to that <c>rankingsContainer</c> object
        ///     so that
        ///     items get displayed correctly.
        /// </summary>
        [Tooltip(
            "Set the container that ranking prefabs will be added to. Should have a VerticalLayoutGroup element for auto layouting the elements")]
        public Transform rankingsContainer;

        /// <summary>
        ///     <para>
        ///         Connect a SCILLLeaderboardRankingItem instance from your scene hierarchy. It will be updated with the
        ///         users value in the leaderboard or will be hidden if the user does not have an entry in the leaderboard.
        ///     </para>
        ///     <para>
        ///         This can be used to display the users current ranking in a header - this way the user doesn't have to scroll to
        ///         his entry
        ///         to check the score and ranking.
        ///     </para>
        /// </summary>
        [Tooltip(
            "Connect an exposed user ranking item (for example in the header) which will be set with the users leaderboard entry.")]
        public SCILLLeaderboardRankingItem userRanking;

        /// <summary>
        ///     Connect a prefab that will be used for the users entry in the leaderboard . We recommend that the prefabs
        ///     highlights the current user so that she/he
        ///     finds her/himself quickly in the leaderboard.
        /// </summary>
        [Header("Building Blocks")] [Tooltip("The prefab of a UI item that will be used for the users ranking entry")]
        public SCILLLeaderboardRankingItem userRankingPrefab;

        /// <summary>
        ///     Connect a prefab that will be used for the top entries in the leaderboard. We recommend applying a different style
        ///     than the <see cref="defaultRankingPrefab" />. This prefab will be used for the first
        ///     <see cref="numberOfTopEntries" /> entries.
        /// </summary>
        [Tooltip("The prefab of a UI item that will be used for the top ranking entries")]
        public SCILLLeaderboardRankingItem topRankingPrefab;

        /// <summary>
        ///     Connect a prefab that will be used for all entries in the leaderboard except the users and top rankings.
        /// </summary>
        [Tooltip("The prefab of a UI item that will be used for any other rankings")]
        public SCILLLeaderboardRankingItem defaultRankingPrefab;

        private bool _allContentLoaded;


        private ScrollRect _scrollRect;

        /// <summary>
        ///     Returns the current page starting at <c>1</c>.
        /// </summary>
        public int CurrentPage { get; private set; }

        private bool IsLoading { get; set; } = false;

        private bool IsInitialized { get; set; } = false;

        private void Awake()
        {
            _scrollRect = GetComponentInChildren<ScrollRect>();
        }

        private void Start()
        {
            if (null != SCILLManager.Instance.SCILLClient)
                InitLeaderboardData();
            else
                SCILLManager.OnSCILLManagerReady += OnScillReady;
        }

        private void Update()
        {
            if (IsInitialized && _scrollRect)
                if (_scrollRect.normalizedPosition.y < 0.25)
                    if (IsLoading == false && _allContentLoaded == false)
                        //Debug.Log("Bottom of page reached, adding next page, Loading: " + loading + ", All Content Loaded: " + allContentLoaded);
                        AddNextPage();
        }

        private void OnEnable()
        {
            UpdateLeaderboard();
        }

        private void OnDestroy()
        {
            SCILLManager.OnSCILLManagerReady -= InitLeaderboardData;

            SCILLManager.Instance.StopLeaderboardUpdateNotifications(leaderboardId, OnLeaderboardUpdated);
        }


        /// <summary>
        ///     <para>
        ///         Use this event to subscribe to changes to the current users leaderboard ranking. This is called when receiving
        ///         a realtime leaderboard notification from the SCILL backend concerning the current user.
        ///     </para>
        ///     <para>
        ///         To access leaderboard update notifications for all users, use the <see cref="SCILLLeaderboardManager" />
        ///         events.
        ///     </para>
        /// </summary>
        public static event UsersLeaderboardRankingChangedAction OnUsersLeaderboardRankingChanged;

        protected virtual void OnScillReady()
        {
            InitLeaderboardData();
            UpdateLeaderboard();
        }

        private void InitLeaderboardData()
        {
            SCILLManager.OnSCILLManagerReady -= InitLeaderboardData;

            RequestFullLeaderboardReload();
            SCILLManager.Instance.StartLeaderboardUpdateNotifications(leaderboardId, OnLeaderboardUpdated);
            IsInitialized = true;
        }


        protected virtual void OnLeaderboardUpdated(LeaderboardUpdatePayload payload)
        {
            // Make sure this  leaderboard has been updated
            if (payload.leaderboard_data.leaderboard_id != leaderboardId) return;

            RequestFullLeaderboardReload();

            if (payload.member_data.member_type == "user" &&
                payload.member_data.member_id == SCILLManager.Instance.GetUserId())
                OnUsersLeaderboardRankingChanged?.Invoke(payload);
        }

        protected virtual void AddLeaderItems(List<LeaderboardRanking> rankings)
        {
            foreach (var ranking in rankings)
            {
                //Debug.Log("Adding ranking " + ranking.rank);
                var prefab = defaultRankingPrefab;
                if (ranking.member_id == SCILLManager.Instance.GetUserId())
                {
                    prefab = userRankingPrefab;
                    UpdateUsersHeaderRankingDisplay(ranking);
                }
                else if (ranking.rank <= numberOfTopEntries) prefab = topRankingPrefab;

                var rankingGo = Instantiate(prefab.gameObject, rankingsContainer.transform, false);
                var rankingItem = rankingGo.GetComponent<SCILLLeaderboardRankingItem>();
                if (rankingItem)
                {
                    rankingItem.numberOfDecimals = numberOfDecimals;
                    rankingItem.ranking = ranking;
                }
            }
        }

        /// <summary>
        ///     Reset the current leaderboard and load from scratch. This function is used internally in <c>OnEnable</c> to
        ///     initiate
        ///     loading the leaderboard. You can also use this function to reset and load the leaderboard if you have changed the
        ///     <c>leaderboardId</c> via script.
        /// </summary>
        public virtual void UpdateLeaderboard()
        {
            CurrentPage = 1;
            _allContentLoaded = false;

            if (SCILLManager.Instance == null || null == SCILLManager.Instance.SCILLClient) return;

            LoadLeaderboardRankings(CurrentPage, true);
            RequestUsersHeaderRankingDisplayUpdate();
        }

        protected virtual void RequestUsersHeaderRankingDisplayUpdate()
        {
            if (!userRanking) return;

            // Load users ranking in this leaderboard
            try
            {
                var leaderboardRankingPromise =
                    SCILLManager.Instance.SCILLClient.GetLeaderboardRankingAsync("user",
                        SCILLManager.Instance.GetUserId(),
                        leaderboardId);

                leaderboardRankingPromise.Then(leaderboardMemberRanking =>
                {
                    LeaderboardRanking ranking = leaderboardMemberRanking.member;
                    UpdateUsersHeaderRankingDisplay(ranking);
                });
            }
            catch (ApiException e)
            {
                Debug.Log("Failed to load users leaderboard: " + e.Message);
                throw;
            }
        }

        protected virtual void UpdateUsersHeaderRankingDisplay(LeaderboardRanking ranking)
        {
            // If user is not in leaderboard, hide it
            if (ranking.rank < 0)
            {
                userRanking.gameObject.SetActive(false);
            }
            else
            {
                // Update the header element with the latest ranking values
                userRanking.numberOfDecimals = numberOfDecimals;
                userRanking.ranking = ranking;
                userRanking.gameObject.SetActive(true);
            }
        }

        protected virtual void RequestFullLeaderboardReload()
        {
            LoadLeaderboardRankings(1, pageSize * CurrentPage, true);
        }

        protected virtual void LoadLeaderboardRankings(int page, bool clear = false)
        {
            LoadLeaderboardRankings(page, pageSize, clear);
        }

        protected virtual void LoadLeaderboardRankings(int page, int customPageSize, bool clear = false)
        {
            if (null != SCILLManager.Instance.SCILLClient && !IsLoading)
            {
                //Debug.Log("LOAD LEADERBOARD RANKINGS " + page);
                IsLoading = true;

                var loadPromise =
                    SCILLManager.Instance.SCILLClient.GetLeaderboardAsync(leaderboardId, page, customPageSize);

                loadPromise.Then(leaderboard =>
                {
                    //Debug.Log(leaderboard.ToJson());

                    if (leaderboardName) leaderboardName.text = leaderboard.name;

                    var rankings = memberType == SCILLMemberType.User
                        ? leaderboard.grouped_by_users
                        : leaderboard.grouped_by_teams;

                    //Debug.Log("Loaded leaderboard rankings, number of items: " + rankings.Count + ", Page-Size: " + pageSize);

                    // Make sure we stop loading new stuff if we are at the end of the list
                    if (rankings.Count < pageSize) _allContentLoaded = true;

                    if (clear) ClearRankings();

                    AddLeaderItems(rankings);

                    IsLoading = false;
                }).Catch(
                    exception => { Debug.LogError(exception.Message); }
                );
            }
        }

        protected virtual void AddNextPage()
        {
            // Dont load new data if we are at the end of the list
            if (_allContentLoaded || IsLoading)
                //Debug.Log("Skipping adding next page, Loading: " + loading + ", All content loaded: " + allContentLoaded);
                return;

            CurrentPage++;
            // Debug.Log("Adding next page");
            LoadLeaderboardRankings(CurrentPage);
        }

        protected virtual void ClearRankings()
        {
            //Debug.Log("Clearing rankings list");

            // Make sure we delete all items from the battle pass levels container
            // This way we can leave some dummy level items in Unity Editor which makes it easier to design UI
            foreach (var child in rankingsContainer
                .GetComponentsInChildren<SCILLLeaderboardRankingItem>())
                Destroy(child.gameObject);
        }
    }
}