using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Threading.Tasks;
using SCILL;
using SCILL.Api;
using SCILL.Client;
using SCILL.Model;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

public enum SupportedLanguages
{
    en,
    de
}

[HelpURL("https://developers.scillgame.com")]
public class SCILLManager : MonoBehaviour
{
    // Properties to be set in the Unity inspector
    [Tooltip("Set your App id here. You need to create an App in the SCILL Admin Panel")]
    public string AppId;
    [Tooltip("You should leave this setting in Production. Sometimes, the SCILL team might ask you to change that.")]
    public SCILL.Environment environment;
    [Tooltip("Set the language")] 
    public SupportedLanguages language;

    [Header("Development/Testing options")]
    [Tooltip("Set your API key here. You can get your API-key in the SCILL Admin Panel")]
    public string APIKey;
    [Tooltip("Set a user id for testing purposes. ")]
    public string UserId = "12345";
    // Default session id. This is just an example value.
    [Tooltip("Set a default session id. Please check out developer documentation for more info.")]
    public string SessionId = "persistent";

    // Getter for the access token
    public string AccessToken => _accessToken;
    
    // In this case, we use a unique devide identifier. Multi device support requires a user account system like
    // Steam, Playfab, etc.
    //public string UserId => SystemInfo.deviceUniqueIdentifier;

    // Getter for the singleton instance of this class
    public static SCILLManager Instance; // **<- reference link to SCILL
    
    // Simple wrappers to get SCILL product APIs
    public EventsApi EventsApi => _scillClient.EventsApi;
    public ChallengesApi ChallengesApi => _scillClient.ChallengesApi;
    public BattlePassesApi BattlePassesApi => _scillClient.BattlePassesApi;
    public LeaderboardsApi LeaderboardsApi => _scillClient.LeaderboardsApi;
    public SCILLClient SCILLClient => _scillClient;

    // Local instances of SCILLClient. Please note, that SCILLBackend should not be used in game clients in production!
    private SCILLBackend _scillBackend;
    private SCILLClient _scillClient;
    private string _accessToken;
    
    private void Awake()
    {
        // Create an instance of this class and make sure it stays (also survives scene changes)
        if (Instance == null) {
            Instance = this;
            
            _accessToken = GenerateAccessToken(GetUserId());
            Debug.Log(_accessToken);

            _scillClient = new SCILLClient(_accessToken, AppId, language.ToString(), environment);
            
            DontDestroyOnLoad(gameObject);
        }
        else {
            Destroy(gameObject);
        }
        
        SceneManager.activeSceneChanged += SceneManagerOnactiveSceneChanged;
    }
    
    protected virtual string GenerateAccessToken(string userId)
    {
        // Override this function and generate an access token in the backend!
        _scillBackend = new SCILLBackend(this.APIKey, environment);
        return _scillBackend.GetAccessToken(userId);
    }

    public virtual bool SetUserInfo(string username, string avatar)
    {
        var userInfo = new UserInfo(username, avatar);
        var result =_scillClient.AuthApi.SetUserInfo(userInfo);
        if (result.Equals(userInfo))
        {
            return true;
        }

        return false;
    }
    
    public virtual UserInfo GetUserInfo()
    {
        var result =_scillClient.AuthApi.GetUserInfo();
        return result;
    }

    public virtual string GetUserId()
    {
        // Override this function to return a User Id
        if (!string.IsNullOrEmpty(UserId))
        {
            return UserId;
        }
        else
        {
            return SystemInfo.deviceUniqueIdentifier;    
        }
        
    }

    public virtual void SetLanguage(SupportedLanguages newSupportedLanguage)
    {
        language = newSupportedLanguage;
        
        // Update the SCILLClient instance with the new language setting
        _scillClient = new SCILLClient(_accessToken, AppId, language.ToString(), environment);
    }

    private void SceneManagerOnactiveSceneChanged(Scene oldScene, Scene newScene)
    {
        
    }

    /// <summary>
    /// Unity method called every frame
    /// </summary>
    private void Update()
    {
    }

    // Basic convenience function to send an event. Users global UserId and sessionId
    public async void SendEventAsync(string eventName, string eventType = "single", EventMetaData metaData = null)
    {
        // Please note, in some cases you should change session ids. This is just a simple example where we don't need
        // to do that
        var payload = new EventPayload(GetUserId(), SessionId, eventName, eventType, metaData);
        try
        {
            var response = await EventsApi.SendEventAsync(payload);
        }
        catch (ApiException e)
        {
            Debug.Error("EVENT FAILED: " + payload.ToJson());
            Debug.Error(e);
            throw;
        }
    }
    
    public async void SendEventAsync(string eventName, string eventType = "single", string sessionId = null, EventMetaData metaData = null)
    {
        // Please note, in some cases you should change session ids. This is just a simple example where we don't need
        // to do that
        var payload = new EventPayload(GetUserId(), sessionId != null ? sessionId : SessionId, eventName, eventType, metaData);
        try
        {
            var response = await EventsApi.SendEventAsync(payload);
        }
        catch (ApiException e)
        {
            Debug.Error("EVENT FAILED: " + payload.ToJson());
            Debug.Error(e);
            throw;
        }
    }

    // Basic wrapper for getting personal challenges
    public async Task<List<ChallengeCategory>> GetPersonalChallengesAsync()
    {
        return await ChallengesApi.GetPersonalChallengesAsync(AppId);
    }

#if UNITY_EDITOR    
    [ContextMenu("Open SCILL Playground")]
    public void OpenPlayground()
    {
        var url = "https://playground.scillgame.com?appId=" + UnityWebRequest.EscapeURL(this.AppId) + "&apiKey=" + UnityWebRequest.EscapeURL(this.APIKey) +
            "&environment=" + UnityWebRequest.EscapeURL(this.environment.ToString().ToLower()) + "&userId=" + UnityWebRequest.EscapeURL(UserId);
        Help.BrowseURL(url);
    }
#endif
}
