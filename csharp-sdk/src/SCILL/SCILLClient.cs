using System;
using System.Collections.Generic;
using RSG;
using SCILL.Api;
using SCILL.Client;
using SCILL.Model;

namespace SCILL
{
    public enum Environment
    {
        Production,
        Staging,
        Development
    }

    public delegate void BattlePassChangedNotificationHandler(BattlePassChallengeChangedPayload payload);

    public delegate void ChallengeChangedNotificationHandler(ChallengeWebhookPayload payload);

    public delegate void LeaderboardChangedNotificationHandler(LeaderboardUpdatePayload payload);

    /// <summary>
    ///     Use this class to initiate the client side API.
    /// </summary>
    public class SCILLClient
    {
        private static Configuration Config;

        /// <summary>
        ///     Initiate an instance of the <c>SCILLClient</c>  class with the access token previously generated in the backend or
        ///     in safe environments with the <see cref="SCILLBackend" /> class instance.
        ///     Use the getters to get a shared instance of the API classes. <c>SCILLClient</c>  sets them up correctly for
        ///     production use and with the correct authentication system.
        /// </summary>
        /// <remarks>
        ///     You can implement the backend in any programming language you like, for example with NodeJS you can implement the
        ///     backend side with a couple of lines of code.
        /// </remarks>
        /// <param name="accessToken">
        ///     You need to provide an access token that you previously generated with the
        ///     <see cref="AuthApi" />. Please check out the documentation about
        ///     <a href="https://developers.scillgame.com/api/authentication.html">access tokens</a> for more info.
        /// </param>
        /// <param name="appId">
        ///     This is the app id of your application. Use the
        ///     <a href="https://admin.scillgame.com/login">Admin Panel</a> to reveal the app id for your application.
        /// </param>
        /// <param name="language">The language setting.</param>
        /// <param name="environment">
        ///     The current environment. Leave in <c>Production</c> if you did not hear anything else from
        ///     our development team.
        /// </param>
        public SCILLClient(string accessToken, string appId, string language = null,
            Environment environment = Environment.Production)
        {
            AccessToken = accessToken;
            AppId = appId;
            Language = language;

            var hostSuffix = "";
            if (environment == Environment.Staging)
                hostSuffix = "-staging";
            else if (environment == Environment.Development) hostSuffix = "-dev";

            Config = Configuration.Default.Clone(string.Empty, Configuration.Default.BasePath);
            Config.ApiKey[ToString()] = AccessToken;

            // On client side, the event parser is set to use the access token to authenticate the request
            Config.AddApiKey("auth", "access_token");

            EventsApi = GetApi<EventsApi>(AccessToken, "https://ep" + hostSuffix + ".scillgame.com");
            ChallengesApi = GetApi<ChallengesApi>(AccessToken, "https://pcs" + hostSuffix + ".scillgame.com");
            BattlePassesApi = GetApi<BattlePassesApi>(AccessToken, "https://es" + hostSuffix + ".scillgame.com");
            AuthApi = GetApi<AuthApi>(AccessToken, "https://us" + hostSuffix + ".scillgame.com");
            LeaderboardsApi = GetApi<LeaderboardsApi>(AccessToken, "https://ls" + hostSuffix + ".scillgame.com");
        }

        /// <summary>
        /// The Access token used by this client. Required for authenticating all requests to the SCILL API.
        /// </summary>
        public string AccessToken { get; }
        /// <summary>
        /// The App Id of the app the client is sending requests for.
        /// </summary>
        public string AppId { get; }
        public string Language { get; }

        /// <summary>
        ///     Getter for the shared <see cref="EventsApi" /> instance. It’s used to send events required for challenges and
        ///     battle passes.
        /// </summary>
        public EventsApi EventsApi { get; }


        /// <summary>
        ///     Getter for the shared <see cref="ChallengesApi" /> instance. It’s used to handle challenges.
        /// </summary>
        public ChallengesApi ChallengesApi { get; }

        /// <summary>
        ///     Getter for the shared <see cref="BattlePassesApi" /> instance. It’s used to handle battle passes.
        /// </summary>
        public BattlePassesApi BattlePassesApi { get; }

        /// <summary>
        ///     Getter for the shared <see cref="AuthApi" /> instance. It’s used for authentication and for handling user data.
        /// </summary>
        public AuthApi AuthApi { get; }

        /// <summary>
        ///     Getter for the shared <see cref="LeaderboardsApi" /> instance. It’s used to handle leaderboards.
        /// </summary>
        public LeaderboardsApi LeaderboardsApi { get; }


        private T GetApi<T>(string token, string basePath) where T : IApiAccessor
        {
            return (T) Activator.CreateInstance(typeof(T), Config.Clone(token, basePath));
        }


        #region Async

        /// <inheritdoc
        ///     cref="IEventsApi.SendEventAsync(System.Action{SCILL.Model.ActionResponse},System.Action{System.Exception},SCILL.Model.EventPayload)" />
        public void SendEventAsync(Action<ActionResponse> resolve, Action<Exception> reject, EventPayload payload)
        {
            EventsApi.SendEventAsync(resolve, reject, payload);
        }

        /// <inheritdoc cref="IEventsApi.SendEventAsync(SCILL.Model.EventPayload)" />
        public IPromise<ActionResponse> SendEventAsync(EventPayload payload)
        {
            return EventsApi.SendEventAsync(payload);
        }

        /// <summary>
        ///     Activate a personal challenge.
        /// </summary>
        /// <remarks>
        ///     Activate a personal challenge by product id and user challenge id.
        /// </remarks>
        /// <exception cref="SCILL.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="resolve">Called on valid API response.</param>
        /// <param name="reject">Called on error response.</param>
        /// <param name="challengeId">The challenge id (see challenge_id of Challenge object)</param>
        public void ActivatePersonalChallengeAsync(Action<ActionResponse> resolve, Action<Exception> reject,
            string challengeId)
        {
            ChallengesApi.ActivatePersonalChallengeAsync(resolve, reject, AppId, challengeId);
        }

        /// <summary>
        ///     Activate a personal challenge.
        /// </summary>
        /// <remarks>
        ///     Activate a personal challenge by product id and user challenge id.
        /// </remarks>
        /// <exception cref="SCILL.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="challengeId">The challenge id (see challenge_id of Challenge object)</param>
        /// <returns>Promise of ActionResponse</returns>
        public IPromise<ActionResponse> ActivatePersonalChallengeAsync(string challengeId)
        {
            return ChallengesApi.ActivatePersonalChallengeAsync(AppId, challengeId);
        }


        /// <summary>
        ///     Cancel an active personal challenge.
        /// </summary>
        /// <remarks>
        ///     Cancel an active personal challenge by product id and user challenge id
        /// </remarks>
        /// <exception cref="SCILL.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="resolve">Called on valid API response.</param>
        /// <param name="reject">Called on error response.</param>
        /// <param name="challengeId">The challenge id (see challenge_id of Challenge object)</param>
        public void CancelPersonalChallengeAsync(Action<ActionResponse> resolve, Action<Exception> reject,
            string challengeId)
        {
            ChallengesApi.CancelPersonalChallengeAsync(resolve, reject, AppId, challengeId);
        }

        /// <summary>
        ///     Cancel an active personal challenge.
        /// </summary>
        /// <remarks>
        ///     Cancel an active personal challenge by product id and user challenge id
        /// </remarks>
        /// <exception cref="SCILL.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="challengeId">The challenge id (see challenge_id of Challenge object)</param>
        /// <returns>Promise of ActionResponse</returns>
        public IPromise<ActionResponse> CancelPersonalChallengeAsync(string challengeId)
        {
            return ChallengesApi.CancelPersonalChallengeAsync(AppId, challengeId);
        }

        /// <summary>
        ///     Claim the reward of a finished personal challenge.
        /// </summary>
        /// <remarks>
        ///     Claim the reward of a finished personal challenge by product id and user challenge id
        /// </remarks>
        /// <exception cref="SCILL.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="resolve">Called on valid API response.</param>
        /// <param name="reject">Called on error response.</param>
        /// <param name="challengeId">The challenge id (see challenge_id of Challenge object)</param>
        public void ClaimPersonalChallengeRewardAsync(Action<ActionResponse> resolve, Action<Exception> reject,
            string challengeId)
        {
            ChallengesApi.ClaimPersonalChallengeRewardAsync(resolve, reject, AppId, challengeId);
        }

        /// <summary>
        ///     Claim the reward of a finished personal challenge.
        /// </summary>
        /// <remarks>
        ///     Claim the reward of a finished personal challenge by product id and user challenge id
        /// </remarks>
        /// <exception cref="SCILL.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="challengeId">The challenge id (see challenge_id of Challenge object)</param>
        /// <returns>Promise of ActionResponse</returns>
        public IPromise<ActionResponse> ClaimPersonalChallengeRewardAsync(string challengeId)
        {
            return ChallengesApi.ClaimPersonalChallengeRewardAsync(AppId, challengeId);
        }

        /// <summary>
        ///     Get personal challenges that are not yet completed.
        /// </summary>
        /// <remarks>
        ///     Get personal challenges organized in categories
        /// </remarks>
        /// <exception cref="SCILL.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="resolve">Called on valid API response.</param>
        /// <param name="reject">Called on error response.</param>
        /// <param name="includeCategories">
        ///     A list of categories that should be included in the response. Only the categories
        ///     provided will be returned (optional)
        /// </param>
        /// <param name="excludeCategories">
        ///     A list of categories that should be excluded from the response. All  categories except
        ///     those listed here will be returned (optional)
        /// </param>
        public void GetPersonalChallengesAsync(Action<List<ChallengeCategory>> resolve, Action<Exception> reject,
            List<string> includeCategories =
                null, List<string> excludeCategories = null)
        {
            ChallengesApi.GetPersonalChallengesAsync(resolve, reject, AppId, includeCategories, excludeCategories,
                Language);
        }

        /// <summary>
        ///     Get personal challenges that are not yet completed.
        /// </summary>
        /// <remarks>
        ///     Get personal challenges organized in categories
        /// </remarks>
        /// <exception cref="SCILL.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="includeCategories">
        ///     A list of categories that should be included in the response. Only the categories
        ///     provided will be returned (optional)
        /// </param>
        /// <param name="excludeCategories">
        ///     A list of categories that should be excluded from the response. All  categories except
        ///     those listed here will be returned (optional)
        /// </param>
        /// <returns>Promise of List&lt;ChallengeCategory&gt;</returns>
        public IPromise<List<ChallengeCategory>> GetPersonalChallengesAsync(List<string> includeCategories =
            null, List<string> excludeCategories = null)
        {
            return ChallengesApi.GetPersonalChallengesAsync(AppId, includeCategories, excludeCategories, Language);
        }

        /// <summary>
        ///     Get all personal challenges available for your app. Also includes completed challenges.
        /// </summary>
        /// <remarks>
        ///     Get personal challenges organized in categories that are not yet finished
        /// </remarks>
        /// <exception cref="SCILL.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="resolve">Called on valid API response.</param>
        /// <param name="reject">Called on error response.</param>
        /// <param name="includeCategories">
        ///     A list of categories that should be included in the response. Only the categories
        ///     provided will be returned (optional)
        /// </param>
        /// <param name="excludeCategories">
        ///     A list of categories that should be excluded from the response. All  categories except
        ///     those listed here will be returned (optional)
        /// </param>
        public void GetAllPersonalChallengesAsync(Action<List<ChallengeCategory>> resolve, Action<Exception> reject,
            List<string> includeCategories =
                null, List<string> excludeCategories = null)
        {
            ChallengesApi.GetAllPersonalChallengesAsync(resolve, reject, AppId, includeCategories, excludeCategories,
                Language);
        }

        /// <summary>
        ///     Get all personal challenges available for your app. Also includes completed challenges.
        /// </summary>
        /// <remarks>
        ///     Get personal challenges organized in categories that are not yet finished
        /// </remarks>
        /// <exception cref="SCILL.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="includeCategories">
        ///     A list of categories that should be included in the response. Only the categories
        ///     provided will be returned (optional)
        /// </param>
        /// <param name="excludeCategories">
        ///     A list of categories that should be excluded from the response. All  categories except
        ///     those listed here will be returned (optional)
        /// </param>
        /// <returns>Promise of List&lt;ChallengeCategory&gt;</returns>
        public IPromise<List<ChallengeCategory>> GetAllPersonalChallengesAsync(List<string> includeCategories =
            null, List<string> excludeCategories = null)
        {
            return ChallengesApi.GetAllPersonalChallengesAsync(AppId, includeCategories, excludeCategories, Language);
        }

        /// <summary>
        ///     Get personal challenges that are not yet completed.
        /// </summary>
        /// <remarks>
        ///     Get personal challenges organized in categories that are not yet finished
        /// </remarks>
        /// <exception cref="SCILL.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="resolve">Called on valid API response.</param>
        /// <param name="reject">Called on error response.</param>
        /// <param name="includeCategories">
        ///     A list of categories that should be included in the response. Only the categories
        ///     provided will be returned (optional)
        /// </param>
        /// <param name="excludeCategories">
        ///     A list of categories that should be excluded from the response. All  categories except
        ///     those listed here will be returned (optional)
        /// </param>
        public void GetUnresolvedPersonalChallengesAsync(Action<List<ChallengeCategory>> resolve,
            Action<Exception> reject,
            List<string> includeCategories =
                null, List<string> excludeCategories = null)
        {
            ChallengesApi.GetUnresolvedPersonalChallengesAsync(resolve, reject, AppId, includeCategories,
                excludeCategories,
                Language);
        }

        /// <summary>
        ///     Get personal challenges that are not yet completed.
        /// </summary>
        /// <remarks>
        ///     Get personal challenges organized in categories that are not yet finished
        /// </remarks>
        /// <exception cref="SCILL.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="includeCategories">
        ///     A list of categories that should be included in the response. Only the categories
        ///     provided will be returned (optional)
        /// </param>
        /// <param name="excludeCategories">
        ///     A list of categories that should be excluded from the response. All  categories except
        ///     those listed here will be returned (optional)
        /// </param>
        /// <returns>Promise of List&lt;ChallengeCategory&gt;</returns>
        public IPromise<List<ChallengeCategory>> GetUnresolvedPersonalChallengesAsync(List<string> includeCategories =
            null, List<string> excludeCategories = null)
        {
            return ChallengesApi.GetUnresolvedPersonalChallengesAsync(AppId, includeCategories, excludeCategories,
                Language);
        }

        /// <summary>
        ///     Get personal challenge by id
        /// </summary>
        /// <remarks>
        ///     Get personal challenges organized in categories
        /// </remarks>
        /// <exception cref="SCILL.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="resolve">Called on valid API response.</param>
        /// <param name="reject">Called on error response.</param>
        /// <param name="challengeId">The challenge id (see challenge_id of Challenge object)</param>
        public void GetPersonalChallengeByIdAsync(Action<Challenge> resolve, Action<Exception> reject,
            string challengeId)
        {
            ChallengesApi.GetPersonalChallengeByIdAsync(resolve, reject, AppId, challengeId, Language);
        }

        /// <summary>
        ///     Get personal challenge by id
        /// </summary>
        /// <remarks>
        ///     Get personal challenges organized in categories
        /// </remarks>
        /// <exception cref="SCILL.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="challengeId">The challenge id (see challenge_id of Challenge object)</param>
        /// <returns>Promise of Challenge</returns>
        public IPromise<Challenge> GetPersonalChallengeByIdAsync(string challengeId)
        {
            return ChallengesApi.GetPersonalChallengeByIdAsync(AppId, challengeId, Language);
        }

        /// <summary>
        ///     Get active personal challenges
        /// </summary>
        /// <remarks>
        ///     Get active personal challenges organized in categories
        /// </remarks>
        /// <exception cref="SCILL.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="resolve">Called on valid API response.</param>
        /// <param name="reject">Called on error response.</param>
        public void GetActivePersonalChallengesAsync(Action<List<ChallengeCategory>> resolve, Action<Exception> reject)
        {
            ChallengesApi.GetActivePersonalChallengesAsync(resolve, reject, AppId, Language);
        }

        /// <summary>
        ///     Get active personal challenges
        /// </summary>
        /// <remarks>
        ///     Get active personal challenges organized in categories
        /// </remarks>
        /// <exception cref="SCILL.Client.ApiException">Thrown when fails to make API call</exception>
        /// <returns>Promise of List&lt;ChallengeCategory&gt;</returns>
        public IPromise<List<ChallengeCategory>> GetActivePersonalChallengesAsync()
        {
            return ChallengesApi.GetActivePersonalChallengesAsync(AppId, Language);
        }

        /// <summary>
        ///     Unlock a personal challenges
        /// </summary>
        /// <remarks>
        ///     Unlock a personal challenge by product id and challenge id
        /// </remarks>
        /// <exception cref="SCILL.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="resolve">Called on valid API response.</param>
        /// <param name="reject">Called on error response.</param>
        /// <param name="challengeId">The challenge id (see challenge_id of Challenge object)</param>
        public void UnlockPersonalChallengeAsync(Action<ActionResponse> resolve, Action<Exception> reject,
            string challengeId)
        {
            ChallengesApi.UnlockPersonalChallengeAsync(resolve, reject, AppId, challengeId, Language);
        }

        /// <summary>
        ///     Unlock a personal challenges
        /// </summary>
        /// <remarks>
        ///     Unlock a personal challenge by product id and challenge id
        /// </remarks>
        /// <exception cref="SCILL.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="challengeId">The challenge id (see challenge_id of Challenge object)</param>
        /// <returns>Promise of ActionResponse</returns>
        public IPromise<ActionResponse> UnlockPersonalChallengeAsync(string challengeId)
        {
            return ChallengesApi.UnlockPersonalChallengeAsync(AppId, challengeId, Language);
        }

        /// <summary>
        ///     Activate a given battle pass level by id
        /// </summary>
        /// <remarks>
        ///     Activate a given battle pass level by id
        /// </remarks>
        /// <exception cref="SCILL.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="resolve">Called on valid API response.</param>
        /// <param name="reject">Called on error response.</param>
        /// <param name="levelId">The id of the battle pass level.</param>
        public void ActivateBattlePassLevelAsync(Action<ActionResponse> resolve, Action<Exception> reject,
            string levelId)
        {
            BattlePassesApi.ActivateBattlePassLevelAsync(resolve, reject, AppId, levelId, Language);
        }

        /// <summary>
        ///     Activate a given battle pass level by id
        /// </summary>
        /// <remarks>
        ///     Activate a given battle pass level by id
        /// </remarks>
        /// <exception cref="SCILL.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="levelId">The id of the battle pass level.</param>
        /// <returns>Promise of ActionResponse</returns>
        public IPromise<ActionResponse> ActivateBattlePassLevelAsync(string levelId)
        {
            return BattlePassesApi.ActivateBattlePassLevelAsync(AppId, levelId, Language);
        }

        /// <summary>
        ///     Claim the battle pass level reward. This will trigger a Webhook that you can use to unlock the reward on server
        ///     side. If you don&#x27;t have a server you can also unlock in the client application after receiving a positive
        ///     response.
        /// </summary>
        /// <remarks>
        ///     Claim the battle pass level id
        /// </remarks>
        /// <exception cref="SCILL.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="resolve">Called on valid API response.</param>
        /// <param name="reject">Called on error response.</param>
        /// <param name="levelId">The id of the battle pass level.</param>
        public void ClaimBattlePassLevelRewardAsync(Action<ActionResponse> resolve, Action<Exception> reject,
            string levelId)
        {
            BattlePassesApi.ClaimBattlePassLevelRewardAsync(resolve, reject, AppId, levelId, Language);
        }

        /// <summary>
        ///     Claim the battle pass level reward. This will trigger a Webhook that you can use to unlock the reward on server
        ///     side. If you don&#x27;t have a server you can also unlock in the client application after receiving a positive
        ///     response.
        /// </summary>
        /// <remarks>
        ///     Claim the battle pass level id
        /// </remarks>
        /// <exception cref="SCILL.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="levelId">The id of the battle pass level.</param>
        /// <returns>Promise of ActionResponse</returns>
        public IPromise<ActionResponse> ClaimBattlePassLevelRewardAsync(string levelId)
        {
            return BattlePassesApi.ClaimBattlePassLevelRewardAsync(AppId, levelId, Language);
        }

        /// <summary>
        ///     Get battle pass levels for an app (from all battle passes)
        /// </summary>
        /// <remarks>
        ///     Get all battle pass levels for an app
        /// </remarks>
        /// <exception cref="SCILL.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="resolve">Called on valid API response.</param>
        /// <param name="reject">Called on error response.</param>
        public void GetAllBattlePassLevelsAsync(Action<List<BattlePassLevel>> resolve, Action<Exception> reject)
        {
            BattlePassesApi.GetAllBattlePassLevelsAsync(resolve, reject, AppId, Language);
        }

        /// <summary>
        ///     Get battle pass levels for an app (from all battle passes)
        /// </summary>
        /// <remarks>
        ///     Get all battle pass levels for an app
        /// </remarks>
        /// <exception cref="SCILL.Client.ApiException">Thrown when fails to make API call</exception>
        /// <returns>Promise of List&lt;BattlePassLevel&gt;</returns>
        public IPromise<List<BattlePassLevel>> GetAllBattlePassLevelsAsync()
        {
            return BattlePassesApi.GetAllBattlePassLevelsAsync(AppId, Language);
        }


        /// <summary>
        ///     Get battle passes
        /// </summary>
        /// <remarks>
        ///     Get active battle passes for the app
        /// </remarks>
        /// <exception cref="SCILL.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="resolve">Called on valid API response.</param>
        /// <param name="reject">Called on error response.</param>
        public void GetActiveBattlePassesAsync(Action<List<BattlePass>> resolve, Action<Exception> reject)
        {
            BattlePassesApi.GetActiveBattlePassesAsync(resolve, reject, AppId, Language);
        }

        /// <summary>
        ///     Get battle passes
        /// </summary>
        /// <remarks>
        ///     Get active battle passes for the app
        /// </remarks>
        /// <exception cref="SCILL.Client.ApiException">Thrown when fails to make API call</exception>
        /// <returns>Promise of List&lt;BattlePass&gt;</returns>
        public IPromise<List<BattlePass>> GetActiveBattlePassesAsync()
        {
            return BattlePassesApi.GetActiveBattlePassesAsync(AppId, Language);
        }

        /// <summary>
        ///     Get battle pass by id
        /// </summary>
        /// <remarks>
        ///     Get battle pass for the product with id
        /// </remarks>
        /// <exception cref="SCILL.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="resolve">Called on valid API response.</param>
        /// <param name="reject">Called on error response.</param>
        /// <param name="battlePassId">
        ///     The id of the battle pass. It’s the same as in battle_pass_id you received in earlier
        ///     requests (i.e. getting all active battle passes for a product).
        /// </param>
        public void GetBattlePassAsync(Action<BattlePass> resolve, Action<Exception> reject, string battlePassId)
        {
            BattlePassesApi.GetBattlePassAsync(resolve, reject, AppId, battlePassId, Language);
        }

        /// <summary>
        ///     Get battle pass by id
        /// </summary>
        /// <remarks>
        ///     Get battle pass for the product with id
        /// </remarks>
        /// <exception cref="SCILL.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="battlePassId">
        ///     The id of the battle pass. It’s the same as in battle_pass_id you received in earlier
        ///     requests (i.e. getting all active battle passes for a product).
        /// </param>
        /// <returns>Promise of BattlePass</returns>
        public IPromise<BattlePass> GetBattlePassAsync(string battlePassId)
        {
            return BattlePassesApi.GetBattlePassAsync(AppId, battlePassId, Language);
        }

        /// <summary>
        ///     Get battle pass levels for a battle pass
        /// </summary>
        /// <remarks>
        ///     Get battle pass levels for a battle pass
        /// </remarks>
        /// <exception cref="SCILL.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="resolve">Called on valid API response.</param>
        /// <param name="reject">Called on error response.</param>
        /// <param name="battlePassId">
        ///     The id of the battle pass. It’s the same as in battle_pass_id you received in earlier
        ///     requests (i.e. getting all active battle passes for a product).
        /// </param>
        public void GetBattlePassLevelsAsync(Action<List<BattlePassLevel>> resolve, Action<Exception> reject,
            string battlePassId)
        {
            BattlePassesApi.GetBattlePassLevelsAsync(resolve, reject, AppId, battlePassId, Language);
        }

        /// <summary>
        ///     Get battle pass levels for a battle pass
        /// </summary>
        /// <remarks>
        ///     Get battle pass levels for a battle pass
        /// </remarks>
        /// <exception cref="SCILL.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="battlePassId">
        ///     The id of the battle pass. It’s the same as in battle_pass_id you received in earlier
        ///     requests (i.e. getting all active battle passes for a product).
        /// </param>
        /// <returns>Promise of List&lt;BattlePassLevel&gt;</returns>
        public IPromise<List<BattlePassLevel>> GetBattlePassLevelsAsync(string battlePassId)
        {
            return BattlePassesApi.GetBattlePassLevelsAsync(AppId, battlePassId, Language);
        }

        /// <summary>
        ///     Get battle passes
        /// </summary>
        /// <remarks>
        ///     Get battle passes for the product
        /// </remarks>
        /// <exception cref="SCILL.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="resolve">Called on valid API response.</param>
        /// <param name="reject">Called on error response.</param>
        public void GetBattlePassesAsync(Action<List<BattlePass>> resolve, Action<Exception> reject)
        {
            BattlePassesApi.GetBattlePassesAsync(resolve, reject, AppId, Language);
        }

        /// <summary>
        ///     Get battle passes
        /// </summary>
        /// <remarks>
        ///     Get battle passes for the product
        /// </remarks>
        /// <exception cref="SCILL.Client.ApiException">Thrown when fails to make API call</exception>
        /// <returns>Promise of List&lt;BattlePass&gt;</returns>
        public IPromise<List<BattlePass>> GetBattlePassesAsync()
        {
            return BattlePassesApi.GetBattlePassesAsync(AppId, Language);
        }

        /// <summary>
        ///     Get battle passes unlocked by the user
        /// </summary>
        /// <remarks>
        ///     Get unlocked battle passes for the user encoded in the access token
        /// </remarks>
        /// <exception cref="SCILL.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="resolve">Called on valid API response.</param>
        /// <param name="reject">Called on error response.</param>
        public void GetUnlockedBattlePassesAsync(Action<List<BattlePass>> resolve, Action<Exception> reject)
        {
            BattlePassesApi.GetUnlockedBattlePassesAsync(resolve, reject, AppId, Language);
        }

        /// <summary>
        ///     Get battle passes unlocked by the user
        /// </summary>
        /// <remarks>
        ///     Get unlocked battle passes for the user encoded in the access token
        /// </remarks>
        /// <exception cref="SCILL.Client.ApiException">Thrown when fails to make API call</exception>
        /// <returns>Promise of List&lt;BattlePass&gt;</returns>
        public IPromise<List<BattlePass>> GetUnlockedBattlePassesAsync()
        {
            return BattlePassesApi.GetUnlockedBattlePassesAsync(AppId, Language);
        }

        /// <summary>
        ///     Unlock the battle pass for the user specified in the access token
        /// </summary>
        /// <remarks>
        ///     Unlock the battle pass for a user
        /// </remarks>
        /// <exception cref="SCILL.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="resolve">Called on valid API response.</param>
        /// <param name="reject">Called on error response.</param>
        /// <param name="battlePassId">
        ///     The id of the battle pass. It’s the same as in battle_pass_id you received in earlier
        ///     requests (i.e. getting all active battle passes for a product).
        /// </param>
        /// <param name="body">Provide purchase info for the battle pass (optional)</param>
        public void UnlockBattlePassAsync(Action<BattlePassUnlockInfo> resolve, Action<Exception> reject,
            string battlePassId,
            BattlePassUnlockPayload body = null)
        {
            BattlePassesApi.UnlockBattlePassAsync(resolve, reject, AppId, battlePassId, body, Language);
        }

        /// <summary>
        ///     Unlock the battle pass for the user specified in the access token
        /// </summary>
        /// <remarks>
        ///     Unlock the battle pass for a user
        /// </remarks>
        /// <exception cref="SCILL.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="battlePassId">
        ///     The id of the battle pass. It’s the same as in battle_pass_id you received in earlier
        ///     requests (i.e. getting all active battle passes for a product).
        /// </param>
        /// <param name="body">Provide purchase info for the battle pass (optional)</param>
        /// <returns>Promise of BattlePassUnlockInfo</returns>
        public IPromise<BattlePassUnlockInfo> UnlockBattlePassAsync(string battlePassId,
            BattlePassUnlockPayload body = null)
        {
            return BattlePassesApi.UnlockBattlePassAsync(AppId, battlePassId, body, Language);
        }

        /// <summary>
        ///     Retrieve Leaderboard
        /// </summary>
        /// <remarks>
        ///     Provides the current leaderboard rankings for a specific leaderboard.
        /// </remarks>
        /// <exception cref="SCILL.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="resolve">Called on valid API response.</param>
        /// <param name="reject">Called on error response.</param>
        /// <param name="leaderboardId">The id of the leaderboard</param>
        /// <param name="currentPage">
        ///     The page index starting at 1. The number of pageSize elements are returned for each page.
        ///     Default value is 1 (optional)
        /// </param>
        /// <param name="pageSize">The number of elements per page. Default is 25. (optional)</param>
        public void GetLeaderboardAsync(Action<Leaderboard> resolve, Action<Exception> reject, string leaderboardId,
            int? currentPage = null,
            int? pageSize = null)
        {
            LeaderboardsApi.GetLeaderboardAsync(resolve, reject, leaderboardId, currentPage, pageSize, Language);
        }

        /// <summary>
        ///     Retrieve Leaderboard
        /// </summary>
        /// <remarks>
        ///     Provides the current leaderboard rankings for a specific leaderboard.
        /// </remarks>
        /// <exception cref="SCILL.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="leaderboardId">The id of the leaderboard</param>
        /// <param name="currentPage">
        ///     The page index starting at 1. The number of pageSize elements are returned for each page.
        ///     Default value is 1 (optional)
        /// </param>
        /// <param name="pageSize">The number of elements per page. Default is 25. (optional)</param>
        /// <returns>Promise of Leaderboard</returns>
        public IPromise<Leaderboard> GetLeaderboardAsync(string leaderboardId, int? currentPage = null,
            int? pageSize = null)
        {
            return LeaderboardsApi.GetLeaderboardAsync(leaderboardId, currentPage, pageSize, Language);
        }

        /// <summary>
        ///     Retrieve User Ranking
        /// </summary>
        /// <remarks>
        ///     Returns a LeaderboardMemberRanking item for the specified leaderboard. Use this route to get the position of a user
        ///     of team in a specified leaderboard.
        /// </remarks>
        /// <exception cref="SCILL.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="resolve">Called on valid API response.</param>
        /// <param name="reject">Called on error response.</param>
        /// <param name="memberType">
        ///     The member type, can be user or team (right now) and sets which leaderboards should be
        ///     selected.
        /// </param>
        /// <param name="memberId">
        ///     Either the user_id or team_id you used when sending the events. The memberType flag identifies
        ///     which one is used.
        /// </param>
        /// <param name="leaderboardId">The id of the leaderboard</param>
        public void GetLeaderboardRankingAsync(Action<LeaderboardMemberRanking> resolve, Action<Exception> reject,
            string memberType,
            string memberId, string leaderboardId)
        {
            LeaderboardsApi.GetLeaderboardRankingAsync(resolve, reject, memberType, memberId, leaderboardId, Language);
        }

        /// <summary>
        ///     Retrieve User Ranking
        /// </summary>
        /// <remarks>
        ///     Returns a LeaderboardMemberRanking item for the specified leaderboard. Use this route to get the position of a user
        ///     of team in a specified leaderboard.
        /// </remarks>
        /// <exception cref="SCILL.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="memberType">
        ///     The member type, can be user or team (right now) and sets which leaderboards should be
        ///     selected.
        /// </param>
        /// <param name="memberId">
        ///     Either the user_id or team_id you used when sending the events. The memberType flag identifies
        ///     which one is used.
        /// </param>
        /// <param name="leaderboardId">The id of the leaderboard</param>
        /// <returns>Promise of LeaderboardMemberRanking</returns>
        public IPromise<LeaderboardMemberRanking> GetLeaderboardRankingAsync(string memberType,
            string memberId, string leaderboardId)
        {
            return LeaderboardsApi.GetLeaderboardRankingAsync(memberType, memberId, leaderboardId, Language);
        }


        /// <summary>
        ///     Retrieve User Rankings
        /// </summary>
        /// <remarks>
        ///     Returns an array of LeaderboardRanking items defined for all leaderboards in the application specified for the
        ///     user.
        /// </remarks>
        /// <exception cref="SCILL.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="resolve">Called on valid API response.</param>
        /// <param name="reject">Called on error response.</param>
        /// <param name="memberType">
        ///     The member type, can be user or team (right now) and sets which leaderboards should be
        ///     selected.
        /// </param>
        /// <param name="memberId">
        ///     Either the user_id or team_id you used when sending the events. The memberType flag identifies
        ///     which one is used.
        /// </param>
        public void GetLeaderboardRankingsAsync(Action<List<LeaderboardMemberRanking>> resolve,
            Action<Exception> reject,
            string memberType, string memberId)
        {
            LeaderboardsApi.GetLeaderboardRankingsAsync(resolve, reject, memberType, memberId, Language);
        }

        /// <summary>
        ///     Retrieve User Rankings
        /// </summary>
        /// <remarks>
        ///     Returns an array of LeaderboardRanking items defined for all leaderboards in the application specified for the
        ///     user.
        /// </remarks>
        /// <exception cref="SCILL.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="memberType">
        ///     The member type, can be user or team (right now) and sets which leaderboards should be
        ///     selected.
        /// </param>
        /// <param name="memberId">
        ///     Either the user_id or team_id you used when sending the events. The memberType flag identifies
        ///     which one is used.
        /// </param>
        /// <returns>Promise of List&lt;LeaderboardMemberRanking&gt;</returns>
        public IPromise<List<LeaderboardMemberRanking>> GetLeaderboardRankingsAsync(
            string memberType, string memberId)
        {
            return LeaderboardsApi.GetLeaderboardRankingsAsync(memberType, memberId, Language);
        }

        /// <summary>
        ///     Retrieve Leaderboards
        /// </summary>
        /// <remarks>
        ///     Returns an array of Leaderboard items defined for the application.
        /// </remarks>
        /// <exception cref="SCILL.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="resolve">Called on valid API response.</param>
        /// <param name="reject">Called on error response.</param>
        /// <param name="currentPage">
        ///     The page index starting at 1. The number of pageSize elements are returned for each page.
        ///     Default value is 1 (optional)
        /// </param>
        /// <param name="pageSize">The number of elements per page. Default is 25. (optional)</param>
        public void GetLeaderboardsAsync(Action<List<Leaderboard>> resolve, Action<Exception> reject,
            int? currentPage = null,
            int? pageSize = null)
        {
            LeaderboardsApi.GetLeaderboardsAsync(resolve, reject, currentPage, pageSize, Language);
        }

        /// <summary>
        ///     Retrieve Leaderboards
        /// </summary>
        /// <remarks>
        ///     Returns an array of Leaderboard items defined for the application.
        /// </remarks>
        /// <exception cref="SCILL.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="currentPage">
        ///     The page index starting at 1. The number of pageSize elements are returned for each page.
        ///     Default value is 1 (optional)
        /// </param>
        /// <param name="pageSize">The number of elements per page. Default is 25. (optional)</param>
        /// <returns>Promise of List&lt;Leaderboard&gt;</returns>
        public IPromise<List<Leaderboard>> GetLeaderboardsAsync(int? currentPage = null,
            int? pageSize = null)
        {
            return LeaderboardsApi.GetLeaderboardsAsync(currentPage, pageSize, Language);
        }

        #endregion
    }

    internal static class ConfigurationExtension
    {
        public static Configuration Clone(this Configuration config, string token, string newBasePath)
        {
            return new Configuration(
                    config.DefaultHeader,
                    config.ApiKey,
                    config.ApiKeyPrefix,
                    newBasePath)
                {AccessToken = token};
        }
    }
}