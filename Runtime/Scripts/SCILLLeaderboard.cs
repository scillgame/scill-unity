using System.Collections.Generic;
using System.Threading.Tasks;
using SCILL.Client;
using SCILL.Model;
using UnityEngine;
using UnityEngine.UI;

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
    [Tooltip("Set the container that ranking prefabs will be added to. Should have a VerticalLayoutGroup element for auto layouting the elements")]
    public Transform rankingsContainer;
    [Tooltip("Connect an exposed user ranking item (for example in the header) which will be set with the users leaderboard entry.")]
    public SCILLLeaderboardRankingItem userRanking;

    [Header("Building Blocks")]
    [Tooltip("The prefab of a UI item that will be used for the users ranking entry")]
    public SCILLLeaderboardRankingItem userRankingPrefab; 
    [Tooltip("The prefab of a UI item that will be used for the top ranking entries")]
    public SCILLLeaderboardRankingItem topRankingPrefab;
    [Tooltip("The prefab of a UI item that will be used for any other rankings")]
    public SCILLLeaderboardRankingItem defaultRankingPrefab;
    
    public int CurrentPage { get; private set; }
    
    private float contentSize = 0;
    private bool allContentLoaded = false;
    private ScrollRect _scrollRect;
    private Task<Leaderboard> loadTask;

    private bool loading => loadTask != null && !loadTask.IsCompleted;

    private void Awake()
    {
        _scrollRect = GetComponentInChildren<ScrollRect>();
    }

    private void Start()
    {
        InvokeRepeating(nameof(PollLeaderboard), 10.0f, 10.0f);
    }

    private async void PollLeaderboard()
    {
        // Don't poll while we are loading new content
        if (loadTask != null && !loadTask.IsCompleted)
        {
            return;
        }
        
        // Reload all data currently visible
        var leaderboard = await SCILLManager.Instance.SCILLClient.GetLeaderboardAsync(leaderboardId, 1, pageSize * CurrentPage);
        List<LeaderboardRanking> rankings = (memberType == SCILLMemberType.User)
            ? leaderboard.grouped_by_users
            : leaderboard.grouped_by_teams;
        ClearRankings();
        AddLeaderItems(rankings);
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

        if (SCILLManager.Instance == null)
        {
            return;
        }
        
        LoadLeaderboardRankings(CurrentPage, true);
        UpdateUsersLeaderboardRanking();
    }

    private async void UpdateUsersLeaderboardRanking()
    {
        if (!userRanking)
        {
            return;
        }
        
        // Load users ranking in this leaderboard
        try
        {
            var leaderboardRanking = await
                SCILLManager.Instance.SCILLClient.GetLeaderboardRankingAsync("user", SCILLManager.Instance.GetUserId(),
                    leaderboardId);

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
        }
        catch (ApiException e)
        {
            Debug.Log("Failed to load users leaderboard: " + e.Message);
            throw;
        }
    }

    private async void LoadLeaderboardRankings(int page, bool clear = false)
    {
        //Debug.Log("LOAD LEADERBOARD RANKINGS " + page);
        loadTask = SCILLManager.Instance.SCILLClient.GetLeaderboardAsync(leaderboardId, page, pageSize);

        var leaderboard = await loadTask;
        
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
    }

    protected virtual void AddNextPage()
    {
        // Dont load new data if we are at the end of the list
        if (allContentLoaded || loading)
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
        foreach (SCILLLeaderboardRankingItem child in rankingsContainer.GetComponentsInChildren<SCILLLeaderboardRankingItem>()) {
            Destroy(child.gameObject);
        }        
    }

    public void Update()
    {
        if (_scrollRect)
        {
            if (_scrollRect.normalizedPosition.y < 0.25)
            {
                if (loading == false && allContentLoaded == false)
                {
                    //Debug.Log("Bottom of page reached, adding next page, Loading: " + loading + ", All Content Loaded: " + allContentLoaded);
                    AddNextPage();    
                }
            }
        }
    }
}
