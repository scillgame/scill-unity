using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using JetBrains.Annotations;
using NativeWebSocket;
using Newtonsoft.Json;
using Proyecto26;
using RSG;
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

    EventsApi eventsApi;
    ChallengesApi challengesApi;
    BattlePassesApi battlePassesApi;
    AuthApi authApi;
    LeaderboardsApi leaderboardsApi;

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

                eventsApi = new EventsApi(new Configuration());
                challengesApi = new ChallengesApi(new Configuration());
                battlePassesApi = new BattlePassesApi(new Configuration());
                authApi = new AuthApi(new Configuration());
                leaderboardsApi = new LeaderboardsApi(new Configuration());

                _scillClient = new SCILLClient(_accessToken, AppId, language.ToString(), environment);
            }).Catch(e => { Debug.LogError(e.Message); });
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        SceneManager.activeSceneChanged += SceneManagerOnactiveSceneChanged;
    }

    protected virtual IPromise<string> GenerateAccessToken(string userId)
    {
        // Configuration defaultConfig = Configuration.Default;
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


    #region Realtime Updates

    public delegate void ChallengeChangedNotificationHandler(ChallengeWebhookPayload payload);

    public event ChallengeChangedNotificationHandler OnChallengeChangedNotification;

    public delegate void BattlePassChangedNotificationHandler(BattlePassChallengeChangedPayload payload);

    public event BattlePassChangedNotificationHandler OnBattlePassChangedNotification;


    private List<WebSocket> _webhookClients = new List<WebSocket>();
    private WebSocket _challengesWebhookClient;


    void Update()
    {
#if !UNITY_WEBGL || UNITY_EDITOR
        foreach (var client in _webhookClients)
        {
            client.DispatchMessageQueue();
        }
#endif
    }

    public async Task StartChallengeUpdateNotifications(ChallengeChangedNotificationHandler handler)
    {
        OnChallengeChangedNotification += handler;

        if (_challengesWebhookClient == null)
        {
            await StartMonitorUserChallenges();
        }
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

    public void StartBattlePassUpdateNotifications(string battlePassId,
        BattlePassChangedNotificationHandler handler)
    {
        OnBattlePassChangedNotification += handler;

        // if (!_battlePassMqttClients.ContainsKey(battlePassId))
        // {
        //     StartMonitorBattlePass(battlePassId);
        // }
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

    private async Task StartMonitorUserChallenges()
    {
        var headers = new Dictionary<string, string>();
        string websocketURL = "wss://mqtt.scillgame.com:8083/mqtt";
        _challengesWebhookClient = new WebSocket(websocketURL);

        _challengesWebhookClient.OnMessage += data =>
        {
            string jsonStr = Encoding.UTF8.GetString(data);
            var payload = JsonConvert.DeserializeObject<ChallengeWebhookPayload>(jsonStr);
            if (payload != null)
            {
                OnChallengeChangedNotification?.Invoke(payload);
            }
        };
        _challengesWebhookClient.OnOpen += () => { Debug.Log("Websocket Connection Opened"); };

        _challengesWebhookClient.OnError += msg => { Debug.LogError("Websocket Connection Error:" + msg); };

        _challengesWebhookClient.OnClose += code => { Debug.Log("Closed with code: " + code.ToString()); };

        _webhookClients.Add(_challengesWebhookClient);

        await _challengesWebhookClient.Connect();
        
        
    }

    private void StopMonitorUserChallenges()
    {
        if (_challengesWebhookClient == null)
        {
            return;
        }

        Debug.Log("Calling Close from StopMonitorUserChallenges");
        _challengesWebhookClient.Close();
        _webhookClients.Remove(_challengesWebhookClient);
        _challengesWebhookClient = null;
    }


    private void StartMonitorBattlePass(string battlePassId)
    {
        // var client = CreateMQTTClient();
        // _battlePassMqttClients.Add(battlePassId, client);
        //
        // // Subscribe to that topic once the MQTT connection is established
        // client.UseConnectedHandler(async e =>
        // {
        //     // Get the MQTT topic for listening on changes for the challenges
        //     var notificationTopic =  AuthApi.GetUserBattlePassNotificationTopicAsync(battlePassId);
        //
        //     // Subscribe to the returned topic
        //      client.SubscribeAsync(new MqttTopicFilterBuilder()
        //         .WithTopic(notificationTopic.topic).Build());
        // });
        //
        // // Handle incoming messages and send payloads to callback handler
        // client.UseApplicationMessageReceivedHandler(e =>
        // {
        //     string jsonStr = Encoding.UTF8.GetString(e.ApplicationMessage.Payload);
        //     var payload = JsonConvert.DeserializeObject<BattlePassChallengeChangedPayload>(jsonStr);
        //     if (payload != null)
        //     {
        //         OnBattlePassChangedNotification?.Invoke(payload);
        //     }
        // });
        //
        // // Connect to SCILLs MQTT server to receive real time notifications
        // var options = new MqttClientOptionsBuilder()
        //     .WithTcpServer("mqtt.scillgame.com", 1883)
        //     .Build();
        //
        //  client.ConnectAsync(options, CancellationToken.None);
    }

    private void StopMonitorBattlePass(string battlePassId)
    {
        // if (_battlePassMqttClients.ContainsKey(battlePassId))
        // {
        //      _battlePassMqttClients[battlePassId].DisconnectAsync();
        //     _battlePassMqttClients.Remove(battlePassId);
        // }
    }

    private async void OnApplicationQuit()
    {
        await StopMonitoring();
    }

    private async Task StopMonitoring()
    {
        Debug.Log("Calling Close from StopMonitoring");

        foreach (var client in _webhookClients)
        {
            await client.Close();
        }

        _webhookClients.Clear();
    }

    #endregion
}