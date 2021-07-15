/* 
 * SCILL API
 *
 * SCILL gives you the tools to activate, retain and grow your user base in your app or game by bringing you features well known in the gaming industry: Gamification. We take care of the services and technology involved so you can focus on your game and content.
 *
 * OpenAPI spec version: 1.0.0
 * Contact: support@scillgame.com
 */

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using RSG;
using SCILL.Client;
using SCILL.Model;

namespace SCILL.Api
{
    /// <summary>
    /// Represents a collection of functions to interact with the API endpoints
    /// </summary>
    public partial interface IAuthApi : IApiAccessor
    {
        #region Asynchronous Operations

        /// <summary>
        /// Get an access token for any user identifier signed with the API-Key
        /// </summary>
        /// <exception cref="SCILL.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="resolve">Called on valid API response.</param>
        /// <param name="reject">Called on error response.</param>
        /// <param name="body">Foreign user identifier.</param>
        void GenerateAccessTokenAsync(Action<AccessToken> resolve, Action<Exception> reject,
            ForeignUserIdentifier body);

        /// <summary>
        /// Get an access token for any user identifier signed with the API-Key
        /// </summary>
        /// <remarks>
        /// 
        /// </remarks>
        /// <exception cref="SCILL.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="body">Foreign user identifier.</param>
        /// <returns>Promise of AccessToken</returns>
        IPromise<AccessToken> GenerateAccessTokenAsync(ForeignUserIdentifier body);

        /// <summary>
        /// Get an access token for any user identifier signed with the API-Key
        /// </summary>
        /// <remarks>
        /// 
        /// </remarks>
        /// <exception cref="SCILL.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="body">Foreign user identifier.</param>
        /// <returns>Promise of ApiResponse (AccessToken)</returns>
        IPromise<ApiResponse<AccessToken>> GenerateAccessTokenAsyncWithHttpInfo(
            ForeignUserIdentifier body);

        /// <summary>
        /// Get a topic to be used with an MQTT client to receive real time updates whenever a battle pass or challenges and levels within the battle pass change
        /// </summary>
        /// <remarks>
        /// Get a topic to be used with an MQTT client to receive real time updates whenever a battle pass changes.
        /// </remarks>
        /// <exception cref="SCILL.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="battlePassId">The battle pass you want to get notified</param>
        /// <param name="reject">Called on error response.</param>
        /// <param name="resolve">Called on response.</param>
        void GetUserBattlePassNotificationTopicAsync(Action<NotificationTopic> resolve, Action<Exception> reject,
            string battlePassId);


        /// <summary>
        /// Get a topic to be used with an MQTT client to receive real time updates whenever a battle pass or challenges and levels within the battle pass change
        /// </summary>
        /// <remarks>
        /// Get a topic to be used with an MQTT client to receive real time updates whenever a battle pass changes.
        /// </remarks>
        /// <exception cref="SCILL.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="battlePassId">The battle pass you want to get notified</param>
        /// <returns>Promise of NotificationTopic</returns>
        IPromise<NotificationTopic> GetUserBattlePassNotificationTopicAsync(string battlePassId);

        /// <summary>
        /// Get a topic to be used with an MQTT client to receive real time updates whenever a battle pass or challenges and levels within the battle pass change
        /// </summary>
        /// <remarks>
        /// Get a topic to be used with an MQTT client to receive real time updates whenever a battle pass changes.
        /// </remarks>
        /// <exception cref="SCILL.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="battlePassId">The battle pass you want to get notified</param>
        /// <returns>Promise of ApiResponse (NotificationTopic)</returns>
        IPromise<ApiResponse<NotificationTopic>> GetUserBattlePassNotificationTopicAsyncWithHttpInfo(
            string battlePassId);


        /// <summary>
        /// Get a topic to be used with an MQTT client to receive real time updates whenever the specified challenge changes.
        /// </summary>
        /// <remarks>
        /// Get a topic to be used with an MQTT client to receive real time updates whenever the challenge changes.
        /// </remarks>
        /// <exception cref="SCILL.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="resolve">Called on valid API response.</param>
        /// <param name="reject">Called on error response.</param>
        /// <param name="challengeId">The challenge id you want to get notified</param>
        void GetUserChallengeNotificationTopicAsync(Action<NotificationTopic> resolve, Action<Exception> reject,
            string challengeId);

        /// <summary>
        /// Get a topic to be used with an MQTT client to receive real time updates whenever the specified challenge changes.
        /// </summary>
        /// <remarks>
        /// Get a topic to be used with an MQTT client to receive real time updates whenever the challenge changes.
        /// </remarks>
        /// <exception cref="SCILL.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="challengeId">The challenge id you want to get notified</param>
        /// <returns>Promise of NotificationTopic</returns>
        IPromise<NotificationTopic> GetUserChallengeNotificationTopicAsync(string challengeId);

        /// <summary>
        /// Get a topic to be used with an MQTT client to receive real time updates whenever the specified challenge changes.
        /// </summary>
        /// <remarks>
        /// Get a topic to be used with an MQTT client to receive real time updates whenever the challenge changes.
        /// </remarks>
        /// <exception cref="SCILL.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="challengeId">The challenge id you want to get notified</param>
        /// <returns>Promise of ApiResponse (NotificationTopic)</returns>
        IPromise<ApiResponse<NotificationTopic>> GetUserChallengeNotificationTopicAsyncWithHttpInfo(
            string challengeId);

        /// <summary>
        /// Get a topic to be used with an MQTT client to receive real time updates whenever challenges for the user provided by the access token changes.
        /// </summary>
        /// <remarks>
        /// Get a topic to be used with an MQTT client to receive real time updates whenever challenges for the user provided by the access token change.
        /// </remarks>
        /// <exception cref="SCILL.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="resolve">Called on valid API response.</param>
        /// <param name="reject">Called on error response.</param>
        void GetUserChallengesNotificationTopicAsync(Action<NotificationTopic> resolve, Action<Exception> reject);

        /// <summary>
        /// Get a topic to be used with an MQTT client to receive real time updates whenever challenges for the user provided by the access token changes.
        /// </summary>
        /// <remarks>
        /// Get a topic to be used with an MQTT client to receive real time updates whenever challenges for the user provided by the access token change.
        /// </remarks>
        /// <exception cref="SCILL.Client.ApiException">Thrown when fails to make API call</exception>
        /// <returns>Promise of NotificationTopic</returns>
        IPromise<NotificationTopic> GetUserChallengesNotificationTopicAsync();

        /// <summary>
        /// Get a topic to be used with an MQTT client to receive real time updates whenever challenges for the user provided by the access token changes.
        /// </summary>
        /// <remarks>
        /// Get a topic to be used with an MQTT client to receive real time updates whenever challenges for the user provided by the access token change.
        /// </remarks>
        /// <exception cref="SCILL.Client.ApiException">Thrown when fails to make API call</exception>
        /// <returns>Promise of ApiResponse (NotificationTopic)</returns>
        IPromise<ApiResponse<NotificationTopic>>
            GetUserChallengesNotificationTopicAsyncWithHttpInfo();

        /// <summary>
        /// Get MQTT topic for leaderboard
        /// </summary>
        /// <remarks>
        /// Get a topic to be used with an MQTT client to receive real time updates whenever the specified leaderboard changes.
        /// </remarks>
        /// <exception cref="SCILL.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="resolve">Called on valid API response.</param>
        /// <param name="reject">Called on error response.</param>
        /// <param name="leaderboardId">The id of the leaderboard you want to get notified</param>
        void GetLeaderboardNotificationTopicAsync(Action<NotificationTopic> resolve, Action<Exception> reject,
            string leaderboardId);


        /// <summary>
        /// Get MQTT topic for leaderboard
        /// </summary>
        /// <remarks>
        /// Get a topic to be used with an MQTT client to receive real time updates whenever the specified leaderboard changes.
        /// </remarks>
        /// <exception cref="SCILL.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="leaderboardId">The id of the leaderboard you want to get notified</param>
        /// <returns>Promise of NotificationTopic</returns>
        IPromise<NotificationTopic> GetLeaderboardNotificationTopicAsync(string leaderboardId);

        /// <summary>
        /// Get MQTT topic for leaderboard
        /// </summary>
        /// <remarks>
        /// Get a topic to be used with an MQTT client to receive real time updates whenever the specified leaderboard changes.
        /// </remarks>
        /// <exception cref="SCILL.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="leaderboardId">The id of the leaderboard you want to get notified</param>
        /// <returns>Promise of ApiResponse (NotificationTopic)</returns>
        IPromise<ApiResponse<NotificationTopic>> GetLeaderboardNotificationTopicAsyncWithHttpInfo(string leaderboardId);


        /// <summary>
        /// Get additional info stored per user
        /// </summary>
        /// <remarks>
        /// Returns additional info object with usernames and avatar image for a user which is used in the leaderboard system
        /// </remarks>
        /// <exception cref="SCILL.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="resolve">Called on valid API response.</param>
        /// <param name="reject">Called on error response.</param>
        void GetUserInfoAsync(Action<UserInfo> resolve, Action<Exception> reject);

        /// <summary>
        /// Get additional info stored per user
        /// </summary>
        /// <remarks>
        /// Returns additional info object with usernames and avatar image for a user which is used in the leaderboard system
        /// </remarks>
        /// <exception cref="SCILL.Client.ApiException">Thrown when fails to make API call</exception>
        /// <returns>Promise of UserInfo</returns>
        IPromise<UserInfo> GetUserInfoAsync();

        /// <summary>
        /// Get additional info stored per user
        /// </summary>
        /// <remarks>
        /// Returns additional info object with usernames and avatar image for a user which is used in the leaderboard system
        /// </remarks>
        /// <exception cref="SCILL.Client.ApiException">Thrown when fails to make API call</exception>
        /// <returns>Promise of ApiResponse (UserInfo)</returns>
        IPromise<ApiResponse<UserInfo>> GetUserInfoAsyncWithHttpInfo();

        /// <summary>
        /// Set additional info stored per user
        /// </summary>
        /// <remarks>
        /// Sets user info like username and avatar image which is returned as part of the user rankings in leaderboards.
        /// </remarks>
        /// <exception cref="SCILL.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="resolve">Called on valid API response.</param>
        /// <param name="reject">Called on error response.</param>
        /// <param name="body">UserInfo object stored in the SCILL database for the user</param>
        void SetUserInfoAsync(Action<UserInfo> resolve, Action<Exception> reject, UserInfo body);

        /// <summary>
        /// Set additional info stored per user
        /// </summary>
        /// <remarks>
        /// Sets user info like username and avatar image which is returned as part of the user rankings in leaderboards.
        /// </remarks>
        /// <exception cref="SCILL.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="body">UserInfo object stored in the SCILL database for the user</param>
        /// <returns>Promise of UserInfo</returns>
        IPromise<UserInfo> SetUserInfoAsync(UserInfo body);

        /// <summary>
        /// Set additional info stored per user
        /// </summary>
        /// <remarks>
        /// Sets user info like username and avatar image which is returned as part of the user rankings in leaderboards.
        /// </remarks>
        /// <exception cref="SCILL.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="body">UserInfo object stored in the SCILL database for the user</param>
        /// <returns>Promise of ApiResponse (UserInfo)</returns>
        IPromise<ApiResponse<UserInfo>> SetUserInfoAsyncWithHttpInfo(UserInfo body);

        #endregion Asynchronous Operations
    }

    /// <summary>
    /// Represents a collection of functions to interact with the API endpoints
    /// </summary>
    /// <inheritdoc/>
    public partial class AuthApi : IAuthApi
    {
        private SCILL.Client.ExceptionFactory _exceptionFactory = (name, response) => null;


        /// <summary>
        /// Initializes a new instance of the <see cref="AuthApi"/> class
        /// </summary>
        /// <returns></returns>
        public AuthApi() : this(SCILL.Client.Configuration.Default)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AuthApi"/> class.
        /// </summary>
        /// <returns></returns>
        public AuthApi(String basePath) : this(new SCILL.Client.Configuration {BasePath = basePath})
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AuthApi"/> class
        /// using Configuration object
        /// </summary>
        /// <param name="configuration">An instance of Configuration</param>
        /// <returns></returns>
        public AuthApi(SCILL.Client.Configuration configuration = null)
        {
            if (configuration == null) // use the default one in Configuration
                this.Configuration = SCILL.Client.Configuration.Default;
            else
                this.Configuration = configuration;

            ExceptionFactory = SCILL.Client.Configuration.DefaultExceptionFactory;
        }

        /// <summary>
        /// Gets the base path of the Configuration.
        /// </summary>
        /// <value>The base path</value>
        public String GetBasePath()
        {
            return this.Configuration.BasePath;
        }

        /// <summary>
        /// Sets the base path of the API client.
        /// </summary>
        /// <value>The base path</value>
        [Obsolete(
            "SetBasePath is deprecated, please do 'Configuration.BasePath = \"http://new-path\" instead.")]
        public void SetBasePath(String basePath)
        {
            // do nothing
        }

        /// <summary>
        /// Gets or sets the configuration object
        /// </summary>
        /// <value>An instance of the Configuration</value>
        public SCILL.Client.Configuration Configuration { get; set; }

        /// <summary>
        /// Provides a factory method hook for the creation of exceptions.
        /// </summary>
        public SCILL.Client.ExceptionFactory ExceptionFactory
        {
            get
            {
                if (_exceptionFactory != null && _exceptionFactory.GetInvocationList().Length > 1)
                {
                    throw new InvalidOperationException("Multicast delegate for ExceptionFactory is unsupported.");
                }

                return _exceptionFactory;
            }
            set { _exceptionFactory = value; }
        }

        /// <summary>
        /// Gets the default header.
        /// </summary>
        /// <returns>Dictionary of HTTP header</returns>
        [Obsolete("DefaultHeader is deprecated, please use Configuration.DefaultHeader instead.")]
        public IDictionary<String, String> DefaultHeader()
        {
            return new ReadOnlyDictionary<string, string>(this.Configuration.DefaultHeader);
        }

        /// <summary>
        /// Add default header.
        /// </summary>
        /// <param name="key">Header field name.</param>
        /// <param name="value">Header field value.</param>
        /// <returns></returns>
        [Obsolete("AddDefaultHeader is deprecated, please use Configuration.AddDefaultHeader instead.")]
        public void AddDefaultHeader(string key, string value)
        {
            this.Configuration.AddDefaultHeader(key, value);
        }

        public void GenerateAccessTokenAsync(Action<AccessToken> resolve, Action<Exception> reject,
            ForeignUserIdentifier body)
        {
            GenerateAccessTokenAsync(body).Then(resolve).Catch(reject);
        }

        public IPromise<AccessToken> GenerateAccessTokenAsync(ForeignUserIdentifier body)
        {
            return GenerateAccessTokenAsyncWithHttpInfo(body).ExtractResponseData();
        }

        public IPromise<ApiResponse<AccessToken>> GenerateAccessTokenAsyncWithHttpInfo(
            ForeignUserIdentifier body)
        {
            // verify the required parameter 'body' is set
            if (body == null)
                throw new ApiException(400,
                    "Missing required parameter 'body' when calling AuthApi->GenerateAccessToken");

            string path = "/api/v1/auth/access-token";

            ApiRequest request =
                Configuration.ApiClient.CreateBaseApiRequest(body, path, HttpMethod.Post);

            var responsePromise =
                Configuration.ApiClient.CallApi<AccessToken>(request, ExceptionFactory, "GenerateAccessToken");
            return responsePromise;
        }

        public void GetUserBattlePassNotificationTopicAsync(Action<NotificationTopic> resolve, Action<Exception> reject,
            string battlePassId)
        {
            GetUserBattlePassNotificationTopicAsync(battlePassId).Then(resolve).Catch(reject);
        }

        public IPromise<NotificationTopic> GetUserBattlePassNotificationTopicAsync(
            string battlePassId)
        {
            return GetUserBattlePassNotificationTopicAsyncWithHttpInfo(battlePassId).ExtractResponseData();
        }

        public IPromise<ApiResponse<NotificationTopic>>
            GetUserBattlePassNotificationTopicAsyncWithHttpInfo(string battlePassId)
        {
            // verify the required parameter 'battlePassId' is set
            if (battlePassId == null)
                throw new ApiException(400,
                    "Missing required parameter 'battlePassId' when calling AuthApi->GetUserBattlePassNotificationTopic");

            var localVarPath = "/api/v1/auth/user-battle-pass-topic-link";
            HttpMethod method = HttpMethod.Get;
            object body = null;

            ApiRequest request =
                Configuration.ApiClient.CreateBaseApiRequest(body, localVarPath, method);

            request.QueryParams.AddRange(
                Configuration.ApiClient.ParameterToKeyValuePairs("", "battle_pass_id", battlePassId));

            var responsePromise = Configuration.ApiClient.CallApi<NotificationTopic>(request, ExceptionFactory,
                "GetUserBattlePassNotificationTopic");
            return responsePromise;
        }

        public void GetUserChallengeNotificationTopicAsync(Action<NotificationTopic> resolve, Action<Exception> reject,
            string challengeId)
        {
            GetUserChallengeNotificationTopicAsync(challengeId).Then(resolve).Catch(reject);
        }

        public IPromise<NotificationTopic> GetUserChallengeNotificationTopicAsync(
            string challengeId)
        {
            return GetUserChallengeNotificationTopicAsyncWithHttpInfo(challengeId).ExtractResponseData();
        }

        public IPromise<ApiResponse<NotificationTopic>>
            GetUserChallengeNotificationTopicAsyncWithHttpInfo(string challengeId)
        {
            // verify the required parameter 'challengeId' is set
            if (challengeId == null)
                throw new ApiException(400,
                    "Missing required parameter 'challengeId' when calling AuthApi->GetUserChallengeNotificationTopic");

            var localVarPath = "/api/v1/auth/user-challenge-topic-link";

            HttpMethod method = HttpMethod.Get;
            object body = null;

            ApiRequest request =
                Configuration.ApiClient.CreateBaseApiRequest(body, localVarPath, method);


            request.QueryParams.AddRange(
                Configuration.ApiClient.ParameterToKeyValuePairs("", "challenge_id", challengeId));

            var responsePromise = Configuration.ApiClient.CallApi<NotificationTopic>(request, ExceptionFactory,
                "GetUserChallengeNotificationTopic");
            return responsePromise;
        }

        public void GetUserChallengesNotificationTopicAsync(Action<NotificationTopic> resolve, Action<Exception> reject)
        {
            GetUserChallengesNotificationTopicAsync().Then(resolve).Catch(reject);
        }

        public IPromise<NotificationTopic> GetUserChallengesNotificationTopicAsync()
        {
            return GetUserChallengesNotificationTopicAsyncWithHttpInfo().ExtractResponseData();
        }


        public IPromise<ApiResponse<NotificationTopic>>
            GetUserChallengesNotificationTopicAsyncWithHttpInfo()
        {
            var localVarPath = "/api/v1/auth/user-challenges-topic-link";

            HttpMethod method = HttpMethod.Get;
            object body = null;

            ApiRequest request =
                Configuration.ApiClient.CreateBaseApiRequest(body, localVarPath, method);


            var responsePromise = Configuration.ApiClient.CallApi<NotificationTopic>(request, ExceptionFactory,
                "GetUserChallengesNotificationTopic");
            return responsePromise;
        }

        public void GetLeaderboardNotificationTopicAsync(Action<NotificationTopic> resolve, Action<Exception> reject,
            string leaderboardId)
        {
            GetLeaderboardNotificationTopicAsync(leaderboardId).Then(resolve).Catch(reject);
        }

        public IPromise<NotificationTopic> GetLeaderboardNotificationTopicAsync(string leaderboardId)
        {
            return GetLeaderboardNotificationTopicAsyncWithHttpInfo(leaderboardId).ExtractResponseData();
        }

        public IPromise<ApiResponse<NotificationTopic>> GetLeaderboardNotificationTopicAsyncWithHttpInfo(
            string leaderboardId)
        {
            if (leaderboardId == null)
                throw new ApiException(400,
                    "Missing required parameter 'leaderboardId' when calling AuthApi->GetLeaderboardNotificationTopic");
            string localVarPath = "/api/v1/auth/leaderboard-topic-link";

            HttpMethod method = HttpMethod.Get;
            object body = null;

            ApiRequest request =
                Configuration.ApiClient.CreateBaseApiRequest(body, localVarPath, method);

            if (leaderboardId != null)
                request.QueryParams.AddRange(
                    this.Configuration.ApiClient.ParameterToKeyValuePairs("", "leaderboard_id",
                        leaderboardId));


            var responsePromise =
                Configuration.ApiClient.CallApi<NotificationTopic>(request, ExceptionFactory,
                    "GetLeaderboardNotificationTopic");

            return responsePromise;
        }

        public void GetUserInfoAsync(Action<UserInfo> resolve, Action<Exception> reject)
        {
            GetUserInfoAsync().Then(resolve).Catch(reject);
        }


        public IPromise<UserInfo> GetUserInfoAsync()
        {
            return GetUserInfoAsyncWithHttpInfo().ExtractResponseData();
        }

        public IPromise<ApiResponse<UserInfo>> GetUserInfoAsyncWithHttpInfo()
        {
            var localVarPath = "/api/v1/user-additional-info";

            HttpMethod method = HttpMethod.Get;
            object body = null;

            ApiRequest request =
                Configuration.ApiClient.CreateBaseApiRequest(body, localVarPath, method);

            var responsePromise = Configuration.ApiClient.CallApi<UserInfo>(request, ExceptionFactory, "GetUserInfo");
            return responsePromise;
        }

        public void SetUserInfoAsync(Action<UserInfo> resolve, Action<Exception> reject, UserInfo body)
        {
            SetUserInfoAsync(body).Then(resolve).Catch(reject);
        }

        public IPromise<UserInfo> SetUserInfoAsync(UserInfo body)
        {
            return SetUserInfoAsyncWithHttpInfo(body).ExtractResponseData();
        }

        public IPromise<ApiResponse<UserInfo>> SetUserInfoAsyncWithHttpInfo(UserInfo body)
        {
            // verify the required parameter 'body' is set
            if (body == null)
                throw new ApiException(400, "Missing required parameter 'body' when calling AuthApi->SetUserInfo");

            var localVarPath = "/api/v1/user-additional-info";
            HttpMethod method = HttpMethod.Put;

            ApiRequest request =
                Configuration.ApiClient.CreateBaseApiRequest(body, localVarPath, method);

            var responsePromise = Configuration.ApiClient.CallApi<UserInfo>(request, ExceptionFactory, "SetUserInfo");
            return responsePromise;
        }
    }
}