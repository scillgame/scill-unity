using System;
using System.Collections;
using System.Collections.Generic;
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
using Environment = SCILL.Environment;

public enum SupportedLanguages
{
    en,
    de
}


/// <summary>
///     Provides access to the SCILL SDK APIs and provides some convenience functions to make it easier to work with SCILL
///     from your own code.
///     Also provides access to Realtime Update Notifications from SCILL.
/// </summary>
/// <remarks>
///     In production, you should not use this class by providing your API key! Instead you should override this class and
///     override these functions:
///     <list type="bullet">
///         <item>
///             <c>GenerateAccessToken</c>
///         </item>
///         <item>
///             <c>GetUserId</c>
///         </item>
///     </list>
///     More info on this topic can be found here:
///     <a href="https://developers.scillgame.com/api/authentication.html">User IDs and Access Tokens.</a>
///     <para>
///         To summarize this topic: API keys are very powerful and therefore should be kept secret. Exposing things on
///         client
///         side aren’t secret, as everyone with a Debugger will be able to extract the API key very quickly from your
///         code.
///         The only way to keep the API key secret is to use it on server side - which can be a cloud function (AWS
///         Lambda,
///         Google Cloud Function, …) or your own HTTP server. Recommended procedure is to create that cloud function for
///         example in NodeJS and to use the SCILL JavaScript SDK to generate the access token for the user.
///     </para>
///     <para>
///         Your own Subclass of SCILLManager will then override the functions listed above to load the access token from
///         your
///         backend, instead of generating it on client side with the API key as we do in the default implementation of
///         this
///         class.
///     </para>
/// </remarks>
[HelpURL("https://developers.scillgame.com")]
public class SCILLManager : MonoBehaviour
{
    public delegate void SCILLManagerReadyAction();

    /// <summary>
    ///     Getter for the singleton instance of this class.
    /// </summary>
    public static SCILLManager Instance; // **<- reference link to SCILL

    /// <summary>
    ///     This is the App id which you can find in the Admin Panel for your app.
    /// </summary>
    [Tooltip("Set your App id here. You need to create an App in the SCILL Admin Panel")]
    public string AppId;

    /// <summary>
    ///     Leave in Production if you did not hear anything else from our development team.
    /// </summary>
    [Tooltip("You should leave this setting in Production. Sometimes, the SCILL team might ask you to change that.")]
    public Environment environment;

    [Tooltip("Set the language")] public SupportedLanguages language;

    /// <summary>
    ///     The API key used to authenticate requests. You can get your API-key in the SCILL Admin Panel.
    ///     This is for testing purposes. In production, you should implement a simple backend function to generate an access
    ///     token for your user id.
    /// </summary>
    [Header("Development/Testing options")]
    [Tooltip("Set your API key here. You can get your API-key in the SCILL Admin Panel.")]
    public string APIKey;

    /// <summary>
    ///     Default user id. This is just an example value. In your game you will need to set a persistent user id that will
    ///     reliably identify the user over time. If you are building a Steam game this could be the Steam-ID for example.
    ///     Override
    ///     <see cref="GetUserId" /> to implement your user id retrieval method.
    /// </summary>
    [Tooltip("Set a user id for testing purposes. ")]
    public string UserId = "12345";

    /// <summary>
    ///     Set a User ID which is used for testing. In your game you will need to set a persistent user id that will reliably
    ///     identify the user over time.
    ///     If you are building a Steam game this could be the Steam-ID for example
    /// </summary>
    [Tooltip("Set a default session id. Please check out developer documentation for more info.")]
    public string SessionId = "persistent";

    // <battlepassId, topic>
    private readonly Dictionary<string, string> _battlepassIdToTopicMap = new Dictionary<string, string>();

    // <leaderboardId, topic>
    private readonly Dictionary<string, string> _leaderboardIdToTopicMap = new Dictionary<string, string>();


    private ScillMqtt _mqtt;

    private string _personalChallengeNotificationTopic;

    // Local instances of SCILLClient. Please note, that SCILLBackend should not be used in game clients in production!
    private SCILLBackend _scillBackend;

    /// <summary>
    ///     Getter for the AccessToken.
    /// </summary>
    public string AccessToken { get; private set; }

    // Simple wrappers to get SCILL product APIs

    /// <summary>
    ///     Getter for the shared <see cref="EventsApi" /> instance.
    /// </summary>
    public EventsApi EventsApi => SCILLClient.EventsApi;

    /// <summary>
    ///     Getter for the shared <see cref="ChallengesApi" /> instance.
    /// </summary>
    public ChallengesApi ChallengesApi => SCILLClient.ChallengesApi;

    /// <summary>
    ///     Getter for the shared <see cref="BattlePassesApi" /> instance.
    /// </summary>
    public BattlePassesApi BattlePassesApi => SCILLClient.BattlePassesApi;

    /// <summary>
    ///     Getter for the shared <see cref="LeaderboardsApi" /> instance.
    /// </summary>
    public LeaderboardsApi LeaderboardsApi => SCILLClient.LeaderboardsApi;

    /// <summary>
    ///     Getter for the shared <see cref="SCILLClient" /> instance. Will update when changing language.
    /// </summary>
    public SCILLClient SCILLClient { get; private set; }


    private void Awake()
    {
        // Create an instance of this class and make sure it stays (also survives scene changes)
        if (Instance == null)
        {
            Instance = this;

            _scillBackend = new SCILLBackend(APIKey, environment);
            var accessTokenPromise = GenerateAccessToken(GetUserId());
            accessTokenPromise.Then(accessToken =>
            {
                AccessToken = accessToken;
                Debug.Log(AccessToken);


                SCILLClient = new SCILLClient(AccessToken, AppId, language.ToString(), environment);
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


    private void Update()
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


    /// <summary>
    ///     Invoked when the <see cref="AccessToken" /> was successfully generated and the <see cref="SCILLClient" /> instance
    ///     initiated.
    /// </summary>
    public static event SCILLManagerReadyAction OnSCILLManagerReady;

    private event ChallengeChangedNotificationHandler OnChallengeChangedNotification;
    private event BattlePassChangedNotificationHandler OnBattlePassChangedNotification;
    private event LeaderboardChangedNotificationHandler OnLeaderboardChangedNotification;

    private IEnumerator PingRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(250);

            if (null != _mqtt) _mqtt.Ping();
        }
    }

    /// <summary>
    ///     This class uses the <c>SCILLBackend</c>  class to call the <c>GenerateAccessToken</c>  function to directly call
    ///     SCILL backend with
    ///     the API key provided in the inspector. This is not recommend in production and you should override this function to
    ///     call a backend function you control to hide the API key from the user.
    /// </summary>
    /// <param name="resolve">Called on successful API response with generated AccessToken.</param>
    /// <param name="reject">Called on API response exception.</param>
    /// <param name="userId">The userId for which the access token should be generated.</param>
    public virtual void GenerateAccessToken(Action<string> resolve, Action<Exception> reject, string userId)
    {
        GenerateAccessToken(userId).Then(resolve).Catch(reject);
    }

    /// <summary>
    ///     This class uses the <c>SCILLBackend</c>  class to call the <c>GenerateAccessToken</c>  function to directly call
    ///     SCILL backend with
    ///     the API key provided in the inspector. This is not recommend in production and you should override this function to
    ///     call a backend function you control to hide the API key from the user.
    /// </summary>
    /// <param name="userId">The userId for which the access token should be generated.</param>
    /// <returns>Promise of Access Token.</returns>
    public virtual IPromise<string> GenerateAccessToken(string userId)
    {
        // Override this function and generate an access token in the backend!
        _scillBackend = new SCILLBackend(APIKey, environment);
        return _scillBackend.GetAccessTokenAsync(userId);
    }

    public virtual void SetUserInfoAsync(Action<bool> callback, string username, string avatar)
    {
        var userInfo = new UserInfo(username, avatar);
        var userInfoPromise = SCILLClient.AuthApi.SetUserInfoAsync(userInfo);
        if (null != callback)
            userInfoPromise.Then(result =>
            {
                if (result.Equals(userInfo)) callback(true);

                callback(false);
            }).Catch(err => callback(false));
    }

    public virtual void GetUserInfoAsync(Action<UserInfo> resolve, Action<Exception> reject)
    {
        SCILLClient.AuthApi.GetUserInfoAsync().Then(resolve).Catch(reject);
    }

    public virtual string GetUserId()
    {
        // Override this function to return a User Id
        if (!string.IsNullOrEmpty(UserId)) return UserId;

        var userIdPlayerPrefKey = "SCILL-Default-UserId";
        var id = PlayerPrefs.GetString(userIdPlayerPrefKey, null);
        if (string.IsNullOrEmpty(id)) id = SystemInfo.deviceUniqueIdentifier;

        if (SystemInfo.unsupportedIdentifier == id)
        {
            id = Guid.NewGuid().ToString();
            Debug.Log("SystemInfo.deviceUniqueIdentifier unsupported, falling back to generating a Guid.");
        }

        PlayerPrefs.SetString(userIdPlayerPrefKey, id);
        PlayerPrefs.Save();

        return id;
    }

    public virtual void SetLanguage(SupportedLanguages newSupportedLanguage)
    {
        language = newSupportedLanguage;

        // Update the SCILLClient instance with the new language setting
        SCILLClient = new SCILLClient(AccessToken, AppId, language.ToString(), environment);
    }

    private void SceneManagerOnactiveSceneChanged(Scene oldScene, Scene newScene)
    {
    }


    /// <summary>
    ///     <para>
    ///         Sends an event using EventsApi.SendEvent method. Select a proper eventName for your event. A list of
    ///         supported events can be found in our Event Reference Guide. Depending on the event type, different additional
    ///         properties can be send in the meta-data object.
    ///     </para>
    ///     <para>
    ///         Event type defines how the event is processed in the SCILL Backend. Two possible values are possible today:
    ///         <list type="table">
    ///             <item>
    ///                 <term>single</term>
    ///                 <description>
    ///                     The amount value (can be a property like amount, score, distance, …) of the event is
    ///                     incremented to the last event with the same structure. Use that for events that will collect skill
    ///                     over
    ///                     time (i.e. the number of kills in a shooter for example)
    ///                 </description>
    ///             </item>
    ///             <item>
    ///                 <term>group</term>
    ///                 <description>
    ///                     The amount value overrides the last events value. Use this type for events that don’t collect skill
    ///                     over time, but where the current value defines the skill. A laptime in a racing game for example is
    ///                     typically not summed up over time.
    ///                 </description>
    ///             </item>
    ///         </list>
    ///     </para>
    ///     <para>
    ///         The last parameter to be set is the SessionId. The SessionId defines how events of the single type are
    ///         incremented together. Only events of the same SessionId will be incremented. If the SessionId changes, the
    ///         counter starts at 0.
    ///     </para>
    /// </summary>
    /// <param name="eventName">
    ///     This is the event type as a string. These have predefined event names for many games and
    ///     applications. It’s wise to use those as this allows us to analyse data and help you balancing your application or
    ///     game.
    /// </param>
    /// <param name="eventType">
    ///     This is either single or group. You can send multiple events in one request (group) or send
    ///     events in sequence. Please note, that depending on your tier you might run into rate limits.
    /// </param>
    /// <param name="metaData">A EventMetaData object that you can/must use to set property values for the respective event.</param>
    public void SendEventAsync(string eventName, string eventType = "single", EventMetaData metaData = null)
    {
        SendEventAsync(eventName, eventType, SessionId, metaData);
    }

    /// <summary>
    ///     <para>
    ///         Sends an event using EventsApi.SendEvent method. Select a proper eventName for your event. A list of
    ///         supported events can be found in our Event Reference Guide. Depending on the event type, different additional
    ///         properties can be send in the meta-data object.
    ///     </para>
    ///     <para>
    ///         Event type defines how the event is processed in the SCILL Backend. Two possible values are possible today:
    ///         <list type="table">
    ///             <item>
    ///                 <term>single</term>
    ///                 <description>
    ///                     The amount value (can be a property like amount, score, distance, …) of the event is
    ///                     incremented to the last event with the same structure. Use that for events that will collect skill
    ///                     over
    ///                     time (i.e. the number of kills in a shooter for example)
    ///                 </description>
    ///             </item>
    ///             <item>
    ///                 <term>group</term>
    ///                 <description>
    ///                     The amount value overrides the last events value. Use this type for events that don’t collect skill
    ///                     over time, but where the current value defines the skill. A laptime in a racing game for example is
    ///                     typically not summed up over time.
    ///                 </description>
    ///             </item>
    ///         </list>
    ///     </para>
    ///     <para>
    ///         The last parameter to be set is the SessionId. The SessionId defines how events of the single type are
    ///         incremented together. Only events of the same SessionId will be incremented. If the SessionId changes, the
    ///         counter starts at 0.
    ///     </para>
    /// </summary>
    /// <param name="eventName">
    ///     This is the event type as a string. These have predefined event names for many games and
    ///     applications. It’s wise to use those as this allows us to analyse data and help you balancing your application or
    ///     game.
    /// </param>
    /// <param name="eventType">
    ///     This is either single or group. You can send multiple events in one request (group) or send
    ///     events in sequence. Please note, that depending on your tier you might run into rate limits.
    /// </param>
    /// <param name="sessionId">This is required if event_type is single and identifies a session. This can be anything used to group events together. For example this can be a level or a match id.</param>
    /// <param name="metaData">A EventMetaData object that you can/must use to set property values for the respective event.</param>
    public void SendEventAsync(string eventName, string eventType = "single", string sessionId = null,
        EventMetaData metaData = null)
    {
        // Please note, in some cases you should change session ids. This is just a simple example where we don't need
        // to do that
        var payload = new EventPayload(GetUserId(), sessionId != null ? sessionId : SessionId, eventName, eventType,
            metaData);

        try
        {
            SendEventAsync(eventName, payload);
        }
        catch (ApiException e)
        {
            HandleEventApiException(eventName, payload, e);
        }
    }

    private void SendEventAsync(string eventName, EventPayload payload)
    {
        EventsApi.SendEventAsync(
            response =>
            {
                SCILLNotificationManager.Instance?.AddNotification(SCILLNotificationType.Success,
                    "Event sent: " + eventName);
            },
            e =>
            {
                if (e is ApiException apiException)
                {
                    HandleEventApiException(eventName, payload, apiException);
                }
                else
                {
                    Debug.LogError(e);
                    throw e;
                }
            },
            payload);
    }

    private static void HandleEventApiException(string eventName, EventPayload payload, ApiException e)
    {
        Debug.LogError("EVENT FAILED: " + payload.ToJson());
        Debug.LogError(e);
        SCILLNotificationManager.Instance?.AddNotification(SCILLNotificationType.Error,
            "Event failed: " + eventName);
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
            SendEventAsync(eventName, payload);
        }
        catch (ApiException e)
        {
            HandleEventApiException(eventName, payload, e);
        }
    }

    // Basic wrapper for getting personal challenges
    public void GetPersonalChallengesAsync(Action<List<ChallengeCategory>> resolve, Action<Exception> reject)
    {
        ChallengesApi.GetPersonalChallengesAsync(AppId).Then(resolve).Catch(reject);
    }

    public void GetPersonalRankingAsync(Action<LeaderboardMemberRanking> resolve, Action<Exception> reject,
        string leaderboardId)
    {
        GetPersonalRankingAsync(leaderboardId).Then(resolve).Catch(reject);
    }

    public IPromise<LeaderboardMemberRanking> GetPersonalRankingAsync(string leaderboardId)
    {
        return LeaderboardsApi.GetLeaderboardRankingAsync("user", GetUserId(), leaderboardId);
    }


#if UNITY_EDITOR
    [ContextMenu("Open SCILL Playground")]
    public void OpenPlayground()
    {
        var url = "https://playground.scillgame.com?appId=" + UnityWebRequest.EscapeURL(AppId) + "&apiKey=" +
                  UnityWebRequest.EscapeURL(APIKey) +
                  "&environment=" + UnityWebRequest.EscapeURL(environment.ToString().ToLower()) + "&userId=" +
                  UnityWebRequest.EscapeURL(UserId);
        Help.BrowseURL(url);
    }
#endif

    #region Realtime Updates

    /// <summary>
    /// </summary>
    /// <param name="mqttclient"></param>
    private void OnMqttConnectionEstablished(ScillMqtt mqttclient)
    {
        if (null != _personalChallengeNotificationTopic)
            if (!mqttclient.IsSubscriptionActive(_personalChallengeNotificationTopic))
                mqttclient.SubscribeToTopicChallenge(_personalChallengeNotificationTopic,
                    OnChallengeChangedNotification);

        foreach (var bpToTopic in _battlepassIdToTopicMap)
            if (!mqttclient.IsSubscriptionActive(bpToTopic.Value))
                mqttclient.SubscribeToTopicBattlePass(bpToTopic.Value, OnBattlePassChangedNotification);

        foreach (var leaderboardToTopic in _leaderboardIdToTopicMap)
            if (!mqttclient.IsSubscriptionActive(leaderboardToTopic.Value))
                mqttclient.SubscribeToTopicLeaderboard(leaderboardToTopic.Value, OnLeaderboardChangedNotification);
    }


    #region Personal Challenge Updates

    public void StartChallengeUpdateNotifications(ChallengeChangedNotificationHandler handler)
    {
        OnChallengeChangedNotification += handler;

        if (ShouldShartupChallengeMonitoring())
            SCILLClient.AuthApi.GetUserChallengesNotificationTopicAsync().Then(topicRequestResult =>
            {
                _personalChallengeNotificationTopic = topicRequestResult.topic;
                if (IsMqttConnectionAvailable())
                    _mqtt.SubscribeToTopicChallenge(topicRequestResult.topic, OnChallengeChangedNotification);
            });
    }

    private bool ShouldShartupChallengeMonitoring()
    {
        var isChallengeTopicInvalid = string.IsNullOrEmpty(_personalChallengeNotificationTopic);
        var isMqttSubInactive = null == _mqtt || !_mqtt.IsSubscriptionActive(_personalChallengeNotificationTopic);
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
            StopMonitorUserChallenges();
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
            SCILLClient.AuthApi.GetUserBattlePassNotificationTopicAsync(battlePassId).Then(topicRequestResult =>
            {
                _battlepassIdToTopicMap[battlePassId] = topicRequestResult.topic;
                if (IsMqttConnectionAvailable())
                    _mqtt.SubscribeToTopicBattlePass(topicRequestResult.topic, OnBattlePassChangedNotification);
            });
    }

    public void StopBattlePassUpdateNotifications(string battlePassId, BattlePassChangedNotificationHandler handler)
    {
        OnBattlePassChangedNotification -= handler;

        if (OnBattlePassChangedNotification == null ||
            OnBattlePassChangedNotification?.GetInvocationList().Length <= 0)
            StopMonitoring(_battlepassIdToTopicMap, battlePassId);
    }

    #endregion

    #region Leaderboard Realtime Updates

    public void StartLeaderboardUpdateNotifications(string leaderboardId,
        LeaderboardChangedNotificationHandler handler)
    {
        OnLeaderboardChangedNotification += handler;

        if (ShouldStartMonitoring(_leaderboardIdToTopicMap, leaderboardId))
            SCILLClient.AuthApi.GetLeaderboardNotificationTopicAsync(leaderboardId).Then(topicRequestResult =>
            {
                _leaderboardIdToTopicMap[leaderboardId] = topicRequestResult.topic;
                if (IsMqttConnectionAvailable())
                    _mqtt.SubscribeToTopicLeaderboard(topicRequestResult.topic, OnLeaderboardChangedNotification);
            });
    }

    public void StopLeaderboardUpdateNotifications(string leaderboardId, LeaderboardChangedNotificationHandler handler)
    {
        OnLeaderboardChangedNotification -= handler;

        if (OnLeaderboardChangedNotification == null ||
            OnLeaderboardChangedNotification?.GetInvocationList().Length <= 0)
            StopMonitoring(_leaderboardIdToTopicMap, leaderboardId);
    }

    #endregion


    private bool ShouldStartMonitoring(IDictionary<string, string> idToTopicMap, string key)
    {
        // Start Monitoring if:
        // we have not yet set a topic to this key
        var isTopicInvalid = !idToTopicMap.ContainsKey(key);
        if (null != _mqtt && !isTopicInvalid)
        {
            // and we're not yet subscribed
            var isMqttSubscriptionInactive = !_mqtt.IsSubscriptionActive(idToTopicMap[key]);
            return isMqttSubscriptionInactive;
        }

        return isTopicInvalid;
    }

    private void StopMonitoring(IDictionary<string, string> idToTopicMap, string key)
    {
        if (idToTopicMap.ContainsKey(key))
        {
            var topic = idToTopicMap[key];
            _mqtt.UnsubscribeFromTopic(topic);
            idToTopicMap.Remove(key);
        }
    }

    #endregion
}