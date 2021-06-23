using System;
using System.CodeDom.Compiler;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Newtonsoft.Json;
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


    public event ChallengeChangedNotificationHandler OnChallengeChangedNotification;
    public event BattlePassChangedNotificationHandler OnBattlePassChangedNotification;

    private ScillMqtt _mqtt;

    private string _personalChallengeNotificationTopic = null;

    // <battlepassID, topic>
    private Dictionary<string, string> _battlepassIdToTopicMap = new Dictionary<string, string>();

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
                StartCoroutine(PingRoutine());
            }).Catch(e => { Debug.LogError(e.Message); });

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
            _mqtt.Close();
        }
    }

    IEnumerator PingRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(250);

            if (null != _mqtt && _mqtt.IsConnected)
            {
                _mqtt.Ping();
            }
        }
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
            eventResponsePromise.Catch(e =>
            {
                var error = e as RequestException;
                Debug.LogError("EVENT FAILED: " + e.Message);
                if (null != error)
                {
                    Debug.LogError("Error Response" + error.Response);
                }
            });
        }
        catch (ApiException e)
        {
            Debug.LogError("EVENT FAILED: " + payload.ToJson());
            Debug.LogError(e);
            throw;
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


    #region Realtime Updates

    public void StartChallengeUpdateNotifications(ChallengeChangedNotificationHandler handler)
    {
        OnChallengeChangedNotification += handler;

        if (String.IsNullOrEmpty(_personalChallengeNotificationTopic) &&
            !_mqtt.IsSubscriptionActive(_personalChallengeNotificationTopic))
        {
            StartCoroutine(WaitForMqttConnectionBeforeChallengeMonitoring());
        }
    }

    private IEnumerator WaitForMqttConnectionBeforeChallengeMonitoring()
    {
        while (null == _mqtt || !_mqtt.IsConnected)
        {
            yield return null;
        }

        StartMonitorUserChallenges();
    }

    private void StartMonitorUserChallenges()
    {
        SCILLClient.AuthApi.GetUserChallengesNotificationTopicAsync().Then(topicRequestResult =>
        {
            _personalChallengeNotificationTopic = topicRequestResult.topic;
            _mqtt.SubscribeToTopicChallenge(topicRequestResult.topic, OnChallengeChangedNotification);
        });
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


    public void StartBattlePassUpdateNotifications(string battlePassId,
        BattlePassChangedNotificationHandler handler)
    {
        OnBattlePassChangedNotification += handler;

        if ((!_battlepassIdToTopicMap.ContainsKey(battlePassId)) ||
            !_mqtt.IsSubscriptionActive(_battlepassIdToTopicMap[battlePassId]))
        {
            StartCoroutine(WaitForMqttConnectionBeforeBattlePassMonitoring(battlePassId));
        }
    }

    private IEnumerator WaitForMqttConnectionBeforeBattlePassMonitoring(string battlePassId)
    {
        while (null == _mqtt || !_mqtt.IsConnected)
        {
            yield return null;
        }

        StartMonitorBattlePass(battlePassId);
    }

    private void StartMonitorBattlePass(string battlePassId)
    {
        SCILLClient.AuthApi.GetUserBattlePassNotificationTopicAsync(battlePassId).Then(topicRequestResult =>
        {
            _battlepassIdToTopicMap[battlePassId] = topicRequestResult.topic;
            _mqtt.SubscribeToTopicBattlePass(topicRequestResult.topic, OnBattlePassChangedNotification);
        });
    }

    public void StopBattlePassUpdateNotifications(string battlePassId, BattlePassChangedNotificationHandler handler)
    {
        OnBattlePassChangedNotification -= handler;

        if (OnBattlePassChangedNotification == null ||
            OnBattlePassChangedNotification?.GetInvocationList().Length <= 0)
        {
            StopMonitorBattlePass(battlePassId);
        }
    }

    private void StopMonitorBattlePass(string battlePassId)
    {
        if (_battlepassIdToTopicMap.ContainsKey(battlePassId))
        {
            string topic = _battlepassIdToTopicMap[battlePassId];
            _mqtt.UnsubscribeFromTopic(topic);
            _battlepassIdToTopicMap.Remove(battlePassId);
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