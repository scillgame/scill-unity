using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using Proyecto26;
using RSG;
using SCILL;
using SCILL.Api;
using SCILL.Client;
using SCILL.Model;
using ScillHelpers;
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

    [Tooltip("Set the language")] public SupportedLanguages language;

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


    public delegate void SCILLManagerReadyAction();

    public static event SCILLManagerReadyAction OnSCILLManagerReady;


    public event ChallengeChangedNotificationHandler OnChallengeChangedNotification;
    public event BattlePassChangedNotificationHandler OnBattlePassChangedNotification;
    public event LeaderboardChangedNotificationHandler OnLeaderboardChangedNotification;


    private ScillMqtt _mqtt;

    private string _personalChallengeNotificationTopic = null;

    // <battlepassId, topic>
    private Dictionary<string, string> _battlepassIdToTopicMap = new Dictionary<string, string>();

    // <leaderboardId, topic>
    private Dictionary<string, string> _leaderboardIdToTopicMap = new Dictionary<string, string>();


    private void Awake()
    {
        // Create an instance of this class and make sure it stays (also survives scene changes)
        if (Instance == null)
        {
            Instance = this;

            _scillBackend = new SCILLBackend(this.APIKey, environment);
            var accessTokenPromise = GenerateAccessToken(GetUserId());
            accessTokenPromise.Then(accessToken =>
            {
                _accessToken = accessToken;
                Debug.Log(_accessToken);


                _scillClient = new SCILLClient(_accessToken, AppId, language.ToString(), environment);
                _mqtt = new ScillMqtt();
                _mqtt.OnMqttConnectionEstablished += OnMqttConnectionEstablished;

                OnSCILLManagerReady?.Invoke();

                StartCoroutine(PingRoutine());
            }).Catch(e =>
            {
                SCILLNotificationManager.Instance?.AddNotification(SCILLNotificationType.Error,
                    "Failed to generate access token");
                Debug.LogError(e.Message);
            });

            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        SceneManager.activeSceneChanged += SceneManagerOnactiveSceneChanged;
    }


    void Update()
    {
#if !UNITY_WEBGL || UNITY_EDITOR
        if (null != _mqtt)
            _mqtt.DispatchMessageQueue();
#endif
    }

    private void OnDestroy()
    {
        if (null != _mqtt)
        {
            _mqtt.OnMqttConnectionEstablished -= OnMqttConnectionEstablished;
            _mqtt.Close();
        }
    }

    IEnumerator PingRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(250);

            if (null != _mqtt)
            {
                _mqtt.Ping();
            }
        }
    }


    protected virtual void GenerateAccessToken(Action<string> callback, string userId)
    {
        GenerateAccessToken(userId).Then(callback);
    }

    protected virtual IPromise<string> GenerateAccessToken(string userId)
    {
        // Override this function and generate an access token in the backend!
        _scillBackend = new SCILLBackend(this.APIKey, environment);
        return _scillBackend.GetAccessTokenAsync(userId);
    }

    public virtual void SetUserInfo(string username, string avatar, [CanBeNull] Action<bool> callback)
    {
        var userInfo = new UserInfo(username, avatar);
        var userInfoPromise = _scillClient.AuthApi.SetUserInfoAsync(userInfo);
        if (null != callback)
        {
            userInfoPromise.Then(result =>
            {
                if (result.Equals(userInfo))
                {
                    callback(true);
                }

                callback(false);
            }).Catch(err => callback(false));
        }
    }

    public virtual void GetUserInfo(Action<UserInfo> callback)
    {
        var resultPromise = _scillClient.AuthApi.GetUserInfoAsync();
        if (null != callback)
        {
            resultPromise.Then(result => { callback(result); });
        }
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


    public void SendEventAsync(string eventName, string eventType = "single", string sessionId = null,
        EventMetaData metaData = null)
    {
        // Please note, in some cases you should change session ids. This is just a simple example where we don't need
        // to do that
        var payload = new EventPayload(GetUserId(), sessionId != null ? sessionId : SessionId, eventName, eventType,
            metaData);
        try
        {
            var eventResponsePromise = EventsApi.SendEventAsync(payload);
            // Debug.Log("SENT EVENT: " + payload.ToJson());
            SCILLNotificationManager.Instance?.AddNotification(SCILLNotificationType.Success,
                "Event sent: " + eventName);
        }
        catch (ApiException e)
        {
            Debug.LogError("EVENT FAILED: " + payload.ToJson());
            Debug.LogError(e);
            SCILLNotificationManager.Instance?.AddNotification(SCILLNotificationType.Error,
                "Event failed: " + eventName);
        }
    }

    public void SendEventForUserIdAsync(string userId, string eventName, string eventType = "single",
        EventMetaData metaData = null)
    {
        SendEventForUserIdAsync(userId, eventName, eventType, SessionId, metaData);
    }

    public void SendEventForUserIdAsync(string userId, string eventName, string eventType = "single",
        string sessionId = null, EventMetaData metaData = null)
    {
        // Please note, in some cases you should change session ids. This is just a simple example where we don't need
        // to do that
        var payload = new EventPayload(userId, sessionId != null ? sessionId : "persistent", eventName, eventType,
            metaData);
        try
        {
            var response = _scillBackend.EventsApi.SendEventAsync(payload);
            Debug.Log("SENT EVENT: " + payload.ToJson());
            SCILLNotificationManager.Instance?.AddNotification(SCILLNotificationType.Success,
                $"Event sent, User: {userId}, Event: {eventName}");
        }
        catch (ApiException e)
        {
            Debug.Log("EVENT FAILED: " + payload.ToJson());
            Debug.Log(e);
            SCILLNotificationManager.Instance?.AddNotification(SCILLNotificationType.Error,
                $"Event failed, User: {userId}, Event: {eventName}");
        }
    }

    // Basic wrapper for getting personal challenges
    public void GetPersonalChallengesAsync(Action<List<ChallengeCategory>> callback)
    {
        var challengesPromise = ChallengesApi.GetPersonalChallengesAsync(AppId);
        if (null != callback)
        {
            challengesPromise.Then(challengeCategories => { callback(challengeCategories); });
        }
    }

    public void GetPersonalRankingAsync(Action<LeaderboardMemberRanking> callback, string leaderboardId)
    {
        GetPersonalRankingAsync(leaderboardId).Then(callback);
    }

    public IPromise<LeaderboardMemberRanking> GetPersonalRankingAsync(string leaderboardId)
    {
        return LeaderboardsApi.GetLeaderboardRankingAsync("user", GetUserId(), leaderboardId);
    }

    #region Realtime Updates

    /// <summary>
    /// 
    /// </summary>
    /// <param name="mqttclient"></param>
    private void OnMqttConnectionEstablished(ScillMqtt mqttclient)
    {
        if (null != _personalChallengeNotificationTopic)
        {
            if (!mqttclient.IsSubscriptionActive(_personalChallengeNotificationTopic))
                mqttclient.SubscribeToTopicChallenge(_personalChallengeNotificationTopic,
                    OnChallengeChangedNotification);
        }

        foreach (var bpToTopic in _battlepassIdToTopicMap)
        {
            if (!mqttclient.IsSubscriptionActive(bpToTopic.Value))
                mqttclient.SubscribeToTopicBattlePass(bpToTopic.Value, OnBattlePassChangedNotification);
        }

        foreach (var leaderboardToTopic in _leaderboardIdToTopicMap)
        {
            if (!mqttclient.IsSubscriptionActive(leaderboardToTopic.Value))
                mqttclient.SubscribeToTopicLeaderboard(leaderboardToTopic.Value, OnLeaderboardChangedNotification);
        }
    }


    #region Personal Challenge Updates

    public void StartChallengeUpdateNotifications(ChallengeChangedNotificationHandler handler)
    {
        OnChallengeChangedNotification += handler;

        if (ShouldShartupChallengeMonitoring())
        {
            SCILLClient.AuthApi.GetUserChallengesNotificationTopicAsync().Then(topicRequestResult =>
            {
                _personalChallengeNotificationTopic = topicRequestResult.topic;
                if (IsMqttConnectionAvailable())
                    _mqtt.SubscribeToTopicChallenge(topicRequestResult.topic, OnChallengeChangedNotification);
            });
        }
    }

    private bool ShouldShartupChallengeMonitoring()
    {
        bool isChallengeTopicInvalid = String.IsNullOrEmpty(_personalChallengeNotificationTopic);
        bool isMqttSubInactive = null == _mqtt || !_mqtt.IsSubscriptionActive(_personalChallengeNotificationTopic);
        return isChallengeTopicInvalid && isMqttSubInactive;
    }

    private bool IsMqttConnectionAvailable()
    {
        return !(null == _mqtt || !_mqtt.IsConnected);
    }

    public void StopChallengeUpdateNotifications(ChallengeChangedNotificationHandler handler)
    {
        OnChallengeChangedNotification -= handler;

        if (OnChallengeChangedNotification == null ||
            OnChallengeChangedNotification?.GetInvocationList().Length <= 0)
        {
            StopMonitorUserChallenges();
        }
    }

    private void StopMonitorUserChallenges()
    {
        _mqtt.UnsubscribeFromTopic(_personalChallengeNotificationTopic);
        _personalChallengeNotificationTopic = null;
    }

    #endregion

    #region Battlepass Realtime Updates

    public void StartBattlePassUpdateNotifications(string battlePassId,
        BattlePassChangedNotificationHandler handler)
    {
        OnBattlePassChangedNotification += handler;

        if (ShouldStartMonitoring(_battlepassIdToTopicMap, battlePassId))
        {
            SCILLClient.AuthApi.GetUserBattlePassNotificationTopicAsync(battlePassId).Then(topicRequestResult =>
            {
                _battlepassIdToTopicMap[battlePassId] = topicRequestResult.topic;
                if (IsMqttConnectionAvailable())
                    _mqtt.SubscribeToTopicBattlePass(topicRequestResult.topic, OnBattlePassChangedNotification);
            });
        }
    }

    public void StopBattlePassUpdateNotifications(string battlePassId, BattlePassChangedNotificationHandler handler)
    {
        OnBattlePassChangedNotification -= handler;

        if (OnBattlePassChangedNotification == null ||
            OnBattlePassChangedNotification?.GetInvocationList().Length <= 0)
        {
            StopMonitoring(_battlepassIdToTopicMap, battlePassId);
        }
    }

    #endregion

    #region Leaderboard Realtime Updates

    public void StartLeaderboardUpdateNotifications(string leaderboardId,
        LeaderboardChangedNotificationHandler handler)
    {
        OnLeaderboardChangedNotification += handler;

        if (ShouldStartMonitoring(_leaderboardIdToTopicMap, leaderboardId))
        {
            SCILLClient.AuthApi.GetLeaderboardNotificationTopicAsync(leaderboardId).Then(topicRequestResult =>
            {
                _leaderboardIdToTopicMap[leaderboardId] = topicRequestResult.topic;
                if (IsMqttConnectionAvailable())
                    _mqtt.SubscribeToTopicLeaderboard(topicRequestResult.topic, OnLeaderboardChangedNotification);
            });
        }
    }

    public void StopLeaderboardUpdateNotifications(string leaderboardId, LeaderboardChangedNotificationHandler handler)
    {
        OnLeaderboardChangedNotification -= handler;

        if (OnLeaderboardChangedNotification == null ||
            OnLeaderboardChangedNotification?.GetInvocationList().Length <= 0)
        {
            StopMonitoring(_leaderboardIdToTopicMap, leaderboardId);
        }
    }

    #endregion


    private bool ShouldStartMonitoring(IDictionary<string, string> idToTopicMap, string key)
    {
        // Start Monitoring if:
        // we have not yet set a topic to this key
        bool isTopicInvalid = !idToTopicMap.ContainsKey(key);
        if (null != _mqtt && !isTopicInvalid)
        {
            // and we're not yet subscribed
            bool isMqttSubscriptionInactive = !_mqtt.IsSubscriptionActive(idToTopicMap[key]);
            return isMqttSubscriptionInactive;
        }

        return isTopicInvalid;
    }

    private void StopMonitoring(IDictionary<string, string> idToTopicMap, string key)
    {
        if (idToTopicMap.ContainsKey(key))
        {
            string topic = idToTopicMap[key];
            _mqtt.UnsubscribeFromTopic(topic);
            idToTopicMap.Remove(key);
        }
    }

    #endregion


#if UNITY_EDITOR
    [ContextMenu("Open SCILL Playground")]
    public void OpenPlayground()
    {
        var url = "https://playground.scillgame.com?appId=" + UnityWebRequest.EscapeURL(this.AppId) + "&apiKey=" +
                  UnityWebRequest.EscapeURL(this.APIKey) +
                  "&environment=" + UnityWebRequest.EscapeURL(this.environment.ToString().ToLower()) + "&userId=" +
                  UnityWebRequest.EscapeURL(UserId);
        Help.BrowseURL(url);
    }
#endif
}