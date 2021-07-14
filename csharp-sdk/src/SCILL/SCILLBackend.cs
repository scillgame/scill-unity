using System;
using RSG;
using SCILL.Api;
using SCILL.Client;
using SCILL.Model;


namespace SCILL
{
    /// <summary>
    ///     Use this class to initiate the backend side API. This API requires an API key to setup which should not be exposed
    ///     in unsafe client applications.
    /// </summary>
    public class SCILLBackend
    {
        private static Configuration _config;

        /// <summary>
        ///     Initiate an instance of the SCILLBackend class with your API Key generated in the
        ///     <a href="https://admin.scillplay.com/">Admin Panel.</a> Use the
        ///     getters to get a shared instance of the API classes. SCILLBackend sets them up correctly for production use and
        ///     with the correct authentication system.
        /// </summary>
        /// <param name="apiKey">
        ///     The API key for your application. You can generate an API key in the
        ///     <a href="https://admin.scillplay.com/">Admin Panel</a> for your application. Please note: Don’t expose the API key
        ///     in unsecure environments like Web Apps.
        /// </param>
        /// <param name="environment">
        ///     The current environment. Leave in <c>Production</c> if you did not hear anything else from
        ///     our development team.
        /// </param>
        public SCILLBackend(string apiKey, Environment environment = Environment.Production)
        {
            var hostSuffix = "";
            if (environment == Environment.Staging)
                hostSuffix = "-staging";
            else if (environment == Environment.Development) hostSuffix = "-dev";

            _config = Configuration.Default.Clone(string.Empty, Configuration.Default.BasePath);
            _config.AddApiKey("auth", "api_key");


            // On backend side, the event parser is set to use the api key to authenticate the request
            EventsApi = GetApi<EventsApi>(apiKey, "https://ep" + hostSuffix + ".scillgame.com");
            AuthApi = GetApi<AuthApi>(apiKey, "https://us" + hostSuffix + ".scillgame.com");

        }

        /// <summary>
        ///     The authentication type used in the backend - usually "api_key".
        /// </summary>
        public string ApiKey => _config.ApiKey[ToString()];

        /// <summary>
        ///     Getter for the shared <see cref="AuthApi" /> instance. It’s used for authentication and for handling user data.
        /// </summary>
        public AuthApi AuthApi { get; }

        /// <summary>
        ///     Getter for the shared <see cref="EventsApi" /> instance. It’s used to send events required for challenges and
        ///     battle passes.
        /// </summary>
        public EventsApi EventsApi { get; }


        private T GetApi<T>(string token, string basePath) where T : IApiAccessor
        {
            return (T) Activator.CreateInstance(typeof(T), _config.Clone(token, basePath));
        }

        /// <summary>
        ///     Returns an access token for the provided user id. Please consult documentation on
        ///     <a href="https://developers.scillgame.com/api/authentication.html#user-ids">user ids</a>.
        /// </summary>
        /// <param name="resolve">Called on valid API response.</param>
        /// <param name="reject">Called on error response.</param>
        /// <param name="userId">
        ///     The unique user id identifying the “current user”. This can be any string and its up to you which
        ///     type of data you send here. User Ids may not change as challenge progress is sent via events with the same user id.
        /// </param>
        public void GetAccessTokenAsync(Action<string> resolve, Action<Exception> reject, string userId)
        {
            GetAccessTokenAsync(userId).Then(resolve).Catch(reject);
        }

        /// <summary>
        ///     Returns an access token for the provided user id. Please consult documentation on
        ///     <a href="https://developers.scillgame.com/api/authentication.html#user-ids">user ids</a>.
        /// </summary>
        /// <param name="userId">
        ///     The unique user id identifying the “current user”. This can be any string and its up to you which
        ///     type of data you send here. User Ids may not change as challenge progress is sent via events with the same user id.
        /// </param>
        /// <returns>Promise of access token, given as string</returns>
        public IPromise<string> GetAccessTokenAsync(string userId)
        {
            return GetAccessTokenAsync(new ForeignUserIdentifier(userId));
        }

        private IPromise<string> GetAccessTokenAsync(ForeignUserIdentifier foreignUser)
        {
            if (string.IsNullOrEmpty(_config.AccessToken) == false)
                return Promise<string>.Resolved(_config.AccessToken);

            var tokenPromise = AuthApi.GenerateAccessTokenAsync(foreignUser);

            var stringTokenPromise = new Promise<string>((resolve, reject) =>
            {
                tokenPromise.Then(accesToken => resolve(accesToken.token))
                    .Catch(reject);
            });


            return stringTokenPromise;
        }
    }
}