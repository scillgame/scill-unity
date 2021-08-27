using System;
using System.Collections;
using System.Collections.Generic;
using RSG;
using SCILL.Api;
using SCILL.Client;
using SCILL.Model;
using ScillHelpers;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

namespace SCILL
{
    public enum SupportedLanguages
    {
        en,
        de
    }


    /// <summary>
    ///     This class is designed as a “Singleton” and uses <c>DontDestroyOnLoad</c>  to make sure, that the class instance
    ///     persists
    ///     even after scene changes. It provides access to the SCILL SDK APIs and provides some convenience functions to make
    ///     it easier to work with SCILL from your own code.
    ///     Create an empty <c>GameObject</c> in your Scene and add this script. Then set your API key, AppId and the language.
    ///     If
    ///     your app supports multiple languages, you can use the <see cref="SetLanguage" /> method to set the language via
    ///     Script.
    ///     You can also choose an environment: You should leave that in <c>Production</c>. Sometimes, when working closely
    ///     with our
    ///     development team we might ask you to choose a different value but usually <c>Production</c> is the correct setting.
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
        [Tooltip(
            "You should leave this setting in Production. Sometimes, the SCILL team might ask you to change that.")]
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

        // Local instance of SCILLBackend. Please note, that SCILLBackend should not be used in game clients in production!
        private SCILLBackend _scillBackend;

        /// <summary>
        ///     Getter for the AccessToken.
        /// </summary>
        public string AccessToken { get; private set; }

        // Simple wrappers to get SCILL product APIs

        /// <summary>
        ///     Getter for the shared <see cref="EventsApi" /> instance. It’s used to send events required for challenges and
        ///     battle passes.
        /// </summary>
        public EventsApi EventsApi => SCILLClient.EventsApi;

        /// <summary>
        ///     Getter for the shared <see cref="ChallengesApi" /> instance. It’s used to handle challenges.
        /// </summary>
        public ChallengesApi ChallengesApi => SCILLClient.ChallengesApi;

        /// <summary>
        ///     Getter for the shared <see cref="BattlePassesApi" /> instance. It’s used to handle battle passes.
        /// </summary>
        public BattlePassesApi BattlePassesApi => SCILLClient.BattlePassesApi;

        /// <summary>
        ///     Getter for the shared <see cref="LeaderboardsApi" /> instance. It’s used to handle leaderboards.
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
                GenerateAccessToken(
                    accessToken =>
                    {
                        AccessToken = accessToken;

                        SCILLClient = new SCILLClient(AccessToken, AppId, language.ToString(), environment);
                        _mqtt = new ScillMqtt();
                        ScillMqtt.OnMqttConnectionEstablished += OnMqttConnectionEstablished;

                        OnSCILLManagerReady?.Invoke();

                        StartCoroutine(PingRoutine());
                    },
                    e =>
                    {
                        SCILLNotificationManager.Instance?.AddNotification(SCILLNotificationType.Error,
                            "Failed to generate access token");
                        Debug.LogError(e.Message);
                    },
                    GetUserId());


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
                ScillMqtt.OnMqttConnectionEstablished -= OnMqttConnectionEstablished;
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

        /// <summary>
        ///     Pings the API regularly to keep the websocket connection online.
        /// </summary>
        /// <returns></returns>
        private IEnumerator PingRoutine()
        {
            while (true)
            {
                yield return new WaitForSeconds(250);

                if (null != _mqtt) _mqtt.Ping();
            }
        }

        /// <summary>
        ///     This uses the <c>SCILLBackend</c>  class to call the <c>GenerateAccessToken</c>  function to directly call
        ///     SCILL backend with
        ///     the API key provided in the inspector. This is not recommend in production and you should override this function to
        ///     call a backend function you control to hide the API key from the user.
        /// </summary>
        /// <param name="resolve">Called on successful API response with generated AccessToken.</param>
        /// <param name="reject">Called on API response exception.</param>
        /// <param name="userId">The userId for which the access token should be generated.</param>
        public virtual void GenerateAccessToken(Action<string> resolve, Action<Exception> reject, string userId)
        {
            // Override this function and generate an access token in the backend!
            GenerateAccessToken(userId).Then(resolve).Catch(reject);
        }

        /// <summary>
        ///     This uses the <c>SCILLBackend</c>  class to call the <c>GenerateAccessToken</c>  function to directly call
        ///     SCILL backend with
        ///     the API key provided in the inspector. This is not recommend in production. You should override the
        ///     <see cref="GenerateAccessToken(System.Action{string},System.Action{System.Exception},string)" /> function to call a
        ///     backend function you control to hide the API key from the user.
        /// </summary>
        /// <param name="userId">The userId for which the access token should be generated.</param>
        /// <returns>Promise of access token, given as string</returns>
        public virtual IPromise<string> GenerateAccessToken(string userId)
        {
            // Override this function and generate an access token in the backend!
            return _scillBackend.GetAccessTokenAsync(userId);
        }

        /// <summary>
        ///     Uses the <c>AuthApi</c> Sets user info like username and avatar image which is returned as part of the user
        ///     rankings in leaderboards.
        /// </summary>
        /// <param name="callback">
        ///     Called on API response with true, if user info was set successfully and false, if setting user
        ///     info failed.
        /// </param>
        /// <param name="username">UserInfo object stored in the SCILL database for the user</param>
        /// <param name="avatar">Avatar image for the user.</param>
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


        /// <summary>
        ///     Returns additional info object with usernames and avatar image for a user which is used in the leaderboard system.
        /// </summary>
        /// <param name="resolve">Called on successful API response with an instance of UserInfo.</param>
        /// <param name="reject">Called on API response failure.</param>
        public virtual void GetUserInfoAsync(Action<UserInfo> resolve, Action<Exception> reject)
        {
            try
            {
                SCILLClient.AuthApi.GetUserInfoAsync().Then(resolve).Catch(reject);
            }
            catch (ApiException e)
            {
                reject(e);
            }
        }

        /// <summary>
        ///     Used to retrieve the UserId.
        /// </summary>
        /// <remarks>
        ///     This class just returns the user id provided in the inspector. If no user is provided in the inspector, it will
        ///     attempt to retrieve the device unique identifier. Some plattforms, i.e. WebGL,
        ///     do not support device unique identifiers. For those, a new guid will be generated and stored in the player prefs.
        ///     This is good to quickly test with one persistent
        ///     user that you control. In production you need to override this function to return your own User ID depending on
        ///     your game.
        /// </remarks>
        /// <returns>Returns the User ID which is then used in a call to <c>GenerateAccessToken</c>  to generate the access token.</returns>
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

        /// <summary>
        ///     Change the Language in runtime. Will update the SCILLClient instance, references to the old instance will be
        ///     invalid.
        /// </summary>
        /// <param name="newSupportedLanguage">The new language.</param>
        public virtual void SetLanguage(SupportedLanguages newSupportedLanguage)
        {
            language = newSupportedLanguage;

            // Update the SCILLClient instance with the new language setting
            SCILLClient = new SCILLClient(AccessToken, AppId, language.ToString(), environment);
        }

        private void SceneManagerOnactiveSceneChanged(Scene oldScene, Scene newScene)
        {
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

        #region APIWrappers

        /// <summary>
        ///     Sends an event using <c>EventsApi.SendEvent</c>  method. See
        ///     <see cref="SendEventAsync(string,string,string,SCILL.Model.EventMetaData)" /> for further information.
        /// </summary>
        /// <param name="eventName">The event name as a string.</param>
        /// <param name="eventType">Either <c>"single"</c> or <c>"group"</c>.</param>
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
        ///     This is the event name as a string. These have predefined event names for many games and
        ///     applications. It’s wise to use those as this allows us to analyse data and help you balancing your application or
        ///     game.
        /// </param>
        /// <param name="eventType">
        ///     Either <c>"single"</c> or <c>"group"</c>. You can send multiple events in one request (group) or send
        ///     events in sequence. Please note, that depending on your tier you might run into rate limits.
        /// </param>
        /// <param name="sessionId">
        ///     This is required if event_type is single and identifies a session. This can be anything used to
        ///     group events together. For example this can be a level or a match id.
        /// </param>
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

        /// <summary>
        ///     Sends an event using <c>EventsApi.SendEvent</c> method for a specific <paramref name="userId" />. See
        ///     <see cref="SendEventAsync(string,string,string,SCILL.Model.EventMetaData)" /> for further information.
        /// </summary>
        /// <param name="userId">The userId for which the event should be sent.</param>
        /// <param name="eventName">This is the event name as a string.</param>
        /// <param name="eventType">Either <c>"single"</c> or <c>"group"</c>.</param>
        /// <param name="metaData">A EventMetaData object that you can/must use to set property values for the respective event.</param>
        public void SendEventForUserIdAsync(string userId, string eventName, string eventType = "single",
            EventMetaData metaData = null)
        {
            SendEventForUserIdAsync(userId, eventName, eventType, SessionId, metaData);
        }

        /// <summary>
        ///     Sends an event using <c>EventsApi.SendEvent</c> method for a specific <paramref name="userId" />. See
        ///     <see cref="SendEventAsync(string,string,string,SCILL.Model.EventMetaData)" /> for further information.
        /// </summary>
        /// <param name="userId">The userId for which the event should be sent.</param>
        /// <param name="eventName">This is the event name as a string.</param>
        /// <param name="eventType">Either <c>"single"</c> or <c>"group"</c>.</param>
        /// <param name="sessionId">
        ///     This is required if event_type is single and identifies a session. This can be anything used to
        ///     group events together. For example this can be a level or a match id.
        /// </param>
        /// <param name="metaData">A EventMetaData object that you can/must use to set property values for the respective event.</param>
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

        /// <summary>
        ///     Get personal challenges organized in categories. Basic wrapper for getting personal challenges from
        ///     <see cref="ChallengesApi" />.
        /// </summary>
        /// <param name="resolve">Called on a successful API response with a List of <see cref="ChallengeCategory" />.</param>
        /// <param name="reject">Called on API response failure.</param>
        public void GetPersonalChallengesAsync(Action<List<ChallengeCategory>> resolve, Action<Exception> reject)
        {
            ChallengesApi.GetPersonalChallengesAsync(AppId).Then(resolve).Catch(reject);
        }

        /// <summary>
        ///     Use this to get the position of a user in a specified leaderboard. Wrapper for
        ///     <c>LeaderboardsApi.GetLeaderboardRankingAsync</c>
        /// </summary>
        /// <param name="resolve">
        ///     Called on a successful API response with a <see cref="LeaderboardMemberRanking" /> item for the
        ///     specified leaderboard.
        /// </param>
        /// <param name="reject">Called on API response failure.</param>
        /// <param name="leaderboardId">The id of the leaderboard.</param>
        public void GetPersonalRankingAsync(Action<LeaderboardMemberRanking> resolve, Action<Exception> reject,
            string leaderboardId)
        {
            GetPersonalRankingAsync(leaderboardId).Then(resolve).Catch(reject);
        }

        /// <summary>
        ///     Use this to get the position of a user in a specified leaderboard. Wrapper for
        ///     <c>LeaderboardsApi.GetLeaderboardRankingAsync</c>
        /// </summary>
        /// <param name="leaderboardId">The id of the leaderboard.</param>
        /// <returns>
        ///     Promise of a <see cref="LeaderboardMemberRanking" /> item for the
        ///     specified leaderboard.
        /// </returns>
        public IPromise<LeaderboardMemberRanking> GetPersonalRankingAsync(string leaderboardId)
        {
            return LeaderboardsApi.GetLeaderboardRankingAsync("user", GetUserId(), leaderboardId);
        }

        #endregion

        #region Realtime Updates

        /// <summary>
        ///     Called when the connection to SCILLs MQTT server was successfully established. If there are already pending
        ///     subscriptions for any notification topic, this will handle starting those subscriptions.
        /// </summary>
        /// <param name="mqttclient">The mqtt client which established the connection.</param>
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

        /// <summary>
        ///     Connects to SCILLs MQTT server and forwards incoming payloads to the provided handler function.
        /// </summary>
        /// <param name="handler">Provide a handler function that is called whenever new payloads are sent from the SCILL backend. </param>
        public void StartChallengeUpdateNotifications(ChallengeChangedNotificationHandler handler)
        {
            OnChallengeChangedNotification += handler;

            if (ShouldShartupChallengeMonitoring() && null != SCILLClient)
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
            var isChallengeTopicInvalid = string.IsNullOrEmpty(_personalChallengeNotificationTopic);
            var isMqttSubInactive = null == _mqtt || !_mqtt.IsSubscriptionActive(_personalChallengeNotificationTopic);
            return isChallengeTopicInvalid && isMqttSubInactive;
        }

        private bool IsMqttConnectionAvailable()
        {
            return !(null == _mqtt || !_mqtt.IsConnected);
        }

        /// <summary>
        ///     Disconnect from the MQTT server and stop receiving notifications.
        /// </summary>
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

        /// <summary>
        ///     Connects to SCILLs MQTT server and forwards incoming payloads to the provided handler function.
        /// </summary>
        /// <param name="battlePassId">
        ///     Provide the battle pass id. This is the same as the <c>battle_pass_id</c>  property in the
        ///     <see cref="BattlePass" /> object.
        /// </param>
        /// <param name="handler">Provide a handler function that is called whenever new payloads are sent from the SCILL backend. </param>
        public void StartBattlePassUpdateNotifications(string battlePassId,
            BattlePassChangedNotificationHandler handler)
        {
            OnBattlePassChangedNotification += handler;

            if (ShouldStartMonitoring(_battlepassIdToTopicMap, battlePassId) && null != SCILLClient)
                SCILLClient.AuthApi.GetUserBattlePassNotificationTopicAsync(battlePassId).Then(topicRequestResult =>
                {
                    _battlepassIdToTopicMap[battlePassId] = topicRequestResult.topic;
                    if (IsMqttConnectionAvailable())
                        _mqtt.SubscribeToTopicBattlePass(topicRequestResult.topic, OnBattlePassChangedNotification);
                });
        }

        /// <summary>
        ///     Disconnects from the MQTT server and stops receiving notifications.
        /// </summary>
        public void StopBattlePassUpdateNotifications(string battlePassId, BattlePassChangedNotificationHandler handler)
        {
            OnBattlePassChangedNotification -= handler;

            if (OnBattlePassChangedNotification == null ||
                OnBattlePassChangedNotification?.GetInvocationList().Length <= 0)
                StopMonitoring(_battlepassIdToTopicMap, battlePassId);
        }

        #endregion

        #region Leaderboard Realtime Updates

        /// <summary>
        ///     Connects to SCILLs MQTT server and forwards incoming payloads to the provided handler function.
        /// </summary>
        /// <param name="leaderboardId">
        ///     Provide the leaderboard id. This is the same as the <c>leaderboard_id</c>  property in the
        ///     <see cref="Leaderboard" /> object.
        /// </param>
        /// <param name="handler">Provide a handler function that is called whenever new payloads are sent from the SCILL backend. </param>
        public void StartLeaderboardUpdateNotifications(string leaderboardId,
            LeaderboardChangedNotificationHandler handler)
        {
            OnLeaderboardChangedNotification += handler;

            if (ShouldStartMonitoring(_leaderboardIdToTopicMap, leaderboardId) && null != SCILLClient)
                SCILLClient.AuthApi.GetLeaderboardNotificationTopicAsync(leaderboardId).Then(topicRequestResult =>
                {
                    _leaderboardIdToTopicMap[leaderboardId] = topicRequestResult.topic;
                    if (IsMqttConnectionAvailable())
                        _mqtt.SubscribeToTopicLeaderboard(topicRequestResult.topic, OnLeaderboardChangedNotification);
                });
        }

        /// <summary>
        ///     Disconnects from the MQTT server and stops receiving notifications.
        /// </summary>
        public void StopLeaderboardUpdateNotifications(string leaderboardId,
            LeaderboardChangedNotificationHandler handler)
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
}