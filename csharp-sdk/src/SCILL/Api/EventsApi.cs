/* 
 * SCILL API
 *
 * SCILL gives you the tools to activate, retain and grow your user base in your app or game by bringing you features well known in the gaming industry: Gamification. We take care of the services and technology involved so you can focus on your game and content.
 *
 * OpenAPI spec version: 1.0.0
 * Contact: support@scillgame.com
 * Generated by: https://github.com/swagger-api/swagger-codegen.git
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
    public partial interface IEventsApi : IApiAccessor
    {
        #region Asynchronous Operations

        /// <summary>
        /// Get all available events and required and optional properties
        /// </summary>
        /// <remarks>
        /// Get all available events and required and optional properties
        /// </remarks>
        /// <exception cref="SCILL.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="resolve">Called on valid API response.</param>
        /// <param name="reject">Called on error response.</param>
        void GetAvailableEventsAsync(Action<List<EventDescription>> resolve, Action<Exception> reject);

        /// <summary>
        /// Get all available events and required and optional properties
        /// </summary>
        /// <remarks>
        /// Get all available events and required and optional properties
        /// </remarks>
        /// <exception cref="SCILL.Client.ApiException">Thrown when fails to make API call</exception>
        /// <returns>Promise of List&lt;EventDescription&gt;</returns>
        IPromise<List<EventDescription>> GetAvailableEventsAsync();

        /// <summary>
        /// Get all available events and required and optional properties
        /// </summary>
        /// <remarks>
        /// Get all available events and required and optional properties
        /// </remarks>
        /// <exception cref="SCILL.Client.ApiException">Thrown when fails to make API call</exception>
        /// <returns>Promise of ApiResponse (List&lt;EventDescription&gt;)</returns>
        IPromise<ApiResponse<List<EventDescription>>> GetAvailableEventsAsyncWithHttpInfo();

        /// <summary>
        /// Post an event
        /// </summary>
        /// <remarks>
        /// Post an event to the SCILL backend
        /// </remarks>
        /// <exception cref="SCILL.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="resolve">Called on valid API response.</param>
        /// <param name="reject">Called on error response.</param>
        /// <param name="body">Event payload or team event payload</param>
        void SendEventAsync(Action<ActionResponse> resolve, Action<Exception> reject, EventPayload body);

        /// <summary>
        /// Post an event
        /// </summary>
        /// <remarks>
        /// Post an event to the SCILL backend
        /// </remarks>
        /// <exception cref="SCILL.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="body">Event payload or team event payload</param>
        /// <returns>Promise of ActionResponse</returns>
        IPromise<ActionResponse> SendEventAsync(EventPayload body);

        /// <summary>
        /// Post an event
        /// </summary>
        /// <remarks>
        /// Post an event to the SCILL backend
        /// </remarks>
        /// <exception cref="SCILL.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="body">Event payload or team event payload</param>
        /// <returns>Promise of ApiResponse (ActionResponse)</returns>
        IPromise<ApiResponse<ActionResponse>> SendEventAsyncWithHttpInfo(EventPayload body);

        #endregion Asynchronous Operations
    }

    /// <summary>
    /// Represents a collection of functions to interact with the API endpoints
    /// </summary>
    /// <inheritdoc/>
    public partial class EventsApi : IEventsApi
    {
        private SCILL.Client.ExceptionFactory _exceptionFactory = (name, response) => null;

        /// <summary>
        /// Initializes a new instance of the <see cref="EventsApi"/> class.
        /// </summary>
        /// <returns></returns>
        public EventsApi(String basePath)
        {
            this.Configuration = new SCILL.Client.Configuration {BasePath = basePath};

            ExceptionFactory = SCILL.Client.Configuration.DefaultExceptionFactory;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EventsApi"/> class
        /// </summary>
        /// <returns></returns>
        public EventsApi()
        {
            this.Configuration = SCILL.Client.Configuration.Default;

            ExceptionFactory = SCILL.Client.Configuration.DefaultExceptionFactory;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EventsApi"/> class
        /// using Configuration object
        /// </summary>
        /// <param name="configuration">An instance of Configuration</param>
        /// <returns></returns>
        public EventsApi(SCILL.Client.Configuration configuration = null)
        {
            if (configuration == null) // use the default one in Configuration
                this.Configuration = SCILL.Client.Configuration.Default;
            else
                this.Configuration = configuration;

            ExceptionFactory = SCILL.Client.Configuration.DefaultExceptionFactory;
        }

        /// <summary>
        /// Gets the base path of the API client.
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
            "SetBasePath is deprecated, please do 'Configuration.ApiClient = new ApiClient(\"http://new-path\")' instead.")]
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


        public void GetAvailableEventsAsync(Action<List<EventDescription>> resolve, Action<Exception> reject)
        {
            GetAvailableEventsAsync().Then(resolve).Catch(reject);
        }

        public IPromise<List<EventDescription>> GetAvailableEventsAsync()
        {
            return GetAvailableEventsAsyncWithHttpInfo().ExtractResponseData();
        }

        public IPromise<ApiResponse<List<EventDescription>>> GetAvailableEventsAsyncWithHttpInfo()
        {
            var localVarPath = "/api/v1/public/documentation";

            HttpMethod method = HttpMethod.Get;
            object body = null;

            ApiRequest request =
                Configuration.ApiClient.CreateBaseApiRequest(body, localVarPath, method);

            var responsePromise =
                Configuration.ApiClient.CallApi<List<EventDescription>>(request, ExceptionFactory,
                    "GetAvailableEvents");
            return responsePromise;
        }

        public void SendEventAsync(Action<ActionResponse> resolve, Action<Exception> reject, EventPayload body)
        {
            SendEventAsync(body).Then(resolve).Catch(reject);
        }

        public IPromise<ActionResponse> SendEventAsync(EventPayload body)
        {
            var promise = SendEventAsyncWithHttpInfo(body).ExtractResponseData();
            return promise;
        }

        public IPromise<ApiResponse<ActionResponse>> SendEventAsyncWithHttpInfo(EventPayload body)
        {
            // verify the required parameter 'body' is set
            if (body == null)
                throw new ApiException(400, "Missing required parameter 'body' when calling EventsApi->SendEvent");

            var localVarPath = "/api/v1/events";

            HttpMethod method = HttpMethod.Post;

            ApiRequest request =
                Configuration.ApiClient.CreateBaseApiRequest(body, localVarPath, method);


            // authentication (ApiKeyType) required
            string apiKey = this.Configuration.GetApiKeyWithPrefix("auth");
            if (!String.IsNullOrEmpty(apiKey))
            {
                request.QueryParams.AddRange(Configuration.ApiClient.ParameterToKeyValuePairs("", "auth",
                    this.Configuration.GetApiKeyWithPrefix("auth")));
            }

            var responsePromise =
                Configuration.ApiClient.CallApi<ActionResponse>(request, ExceptionFactory, "SendEvent");
            return responsePromise;
        }
    }
}