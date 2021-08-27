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
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using Proyecto26;
using RSG;

namespace SCILL.Client
{
    public class ApiClient
    {
        /// <summary>
        ///     Gets or sets the default API client for making HTTP calls.
        /// </summary>
        /// <value>The default API client.</value>
        [Obsolete("ApiClient.Default is deprecated, please use 'Configuration.Default.ApiClient' instead.")]
        public static ApiClient Default;

        private readonly JsonSerializerSettings serializerSettings = new JsonSerializerSettings
        {
            ConstructorHandling = ConstructorHandling.AllowNonPublicDefaultConstructor
        };

        /// <summary>
        ///     Initializes a new instance of the <see cref="ApiClient" /> class
        ///     with default configuration.
        /// </summary>
        public ApiClient() : this(Client.Configuration.Default)
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="ApiClient" /> class
        ///     with default base path (https://virtserver.swaggerhub.com/4Players-GmbH/scill-gaas/1.0.0).
        /// </summary>
        /// <param name="config">An instance of Configuration.</param>
        public ApiClient(Configuration config)
        {
            Configuration = config ?? Client.Configuration.Default;
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="ApiClient" /> class
        ///     with default configuration.
        /// </summary>
        /// <param name="basePath">The base path.</param>
        public ApiClient(string basePath)
        {
            if (string.IsNullOrEmpty(basePath))
                throw new ArgumentException("basePath cannot be empty");

            Configuration = Client.Configuration.Default;
        }

        /// <summary>
        ///     Gets or sets an instance of the IReadableConfiguration.
        /// </summary>
        /// <value>An instance of the IReadableConfiguration.</value>
        /// <remarks>
        ///     <see cref="IReadableConfiguration" /> helps us to avoid modifying possibly global
        ///     configuration values from within a given client. It does not guarantee thread-safety
        ///     of the <see cref="Configuration" /> instance in any way.
        /// </remarks>
        public IReadableConfiguration Configuration { get; set; }


        /// <summary>
        ///     Call the API with the given http request.
        /// </summary>
        /// <param name="apiRequest">Data required to send the api request</param>
        /// <param name="exceptionFactory">Api specific implementation of the <see cref="ExceptionFactory" /> delegate</param>
        /// <param name="methodName">
        ///     The API method name being called. Used for error tracing by being displayed in exception
        ///     messages.
        /// </param>
        /// <returns>
        ///     Promise of an <see cref="ApiResponse{T}" /> instance containing a deserialized object of type
        ///     <typeparamref name="T" /> on success or an <see cref="ApiException" /> on failure.
        /// </returns>
        public IPromise<ApiResponse<T>> CallApi<T>(ApiRequest apiRequest, ExceptionFactory exceptionFactory = null,
            string methodName = "")
        {
            var request = ToRequestHelper(apiRequest, Configuration.BasePath, Configuration.Timeout);


            var restClientPromise = RestClient.Request(request);
            var promise = new Promise<ApiResponse<T>>((resolve, reject) =>
            {
                restClientPromise.Then(responseHelper =>
                    {
                        var apiResponse = FromResponseHelper<T>(responseHelper);
                        if (null != exceptionFactory)
                        {
                            var exception = exceptionFactory(methodName, apiResponse);
                            if (null != exception)
                                throw exception;
                        }

                        apiResponse.Data = JsonConvert.DeserializeObject<T>(responseHelper.Text);
                        resolve(apiResponse);
                    })
                    .Catch(reject);
            });


            return promise;
        }

        /// <summary>
        ///     Converts an <see cref="ApiRequest" /> object into a RestClient <see cref="RequestHelper" /> object.
        /// </summary>
        /// <param name="scillRequest">The SCILL specific request data.</param>
        /// <param name="basePath">API base path</param>
        /// <param name="timeout">The request timeout duration.</param>
        /// <returns>RequestHelper object.</returns>
        private RequestHelper ToRequestHelper(ApiRequest scillRequest, string basePath, int timeout)
        {
            var request = new RequestHelper();
            request.Uri = MakeApiRequestUri(basePath, scillRequest.Path);
            request.Method = scillRequest.Method.ToString();
            request.Timeout = timeout;
            request.IgnoreHttpException = true;

            if (scillRequest.QueryParams.Count > 0)
                request.Params = scillRequest.QueryParams.ToDictionary(x => x.Key, x => x.Value);

            if (null != scillRequest.PostBody)
                request.BodyString =
                    JsonConvert.SerializeObject(scillRequest.PostBody, Formatting.Indented);

            request.Headers = scillRequest.HeaderParams;
            return request;
        }

        private ApiResponse<T> FromResponseHelper<T>(ResponseHelper responseHelper)
        {
            var response =
                new ApiResponse<T>(Convert.ToInt32(responseHelper.StatusCode), responseHelper.Headers,
                    responseHelper.Data, responseHelper.Text, responseHelper.Error);
            return response;
        }

        private string MakeApiRequestUri(string basePath, string path)
        {
            return basePath + path;
        }

        public ApiRequest CreateBaseApiRequest(object body, string path, HttpMethod method, string language = null,
            string httpContentType = "application/json")
        {
            var request = new ApiRequest(path, method);

            request.HeaderParams = new Dictionary<string, string>(Configuration.DefaultHeader);

            if (!string.IsNullOrEmpty(Configuration.AccessToken))
            {
                var accessTokenEscaped = EscapeString(Configuration.AccessToken);

                request.HeaderParams["Authorization"] = "Bearer " + accessTokenEscaped;
            }

            // to determine the Content-Type header
            string[] localVarHttpContentTypes =
            {
                httpContentType
            };
            var localVarHttpContentType =
                SelectHeaderContentType(localVarHttpContentTypes);
            if (localVarHttpContentType != null)
                request.HeaderParams.Add("Content-Type", localVarHttpContentType);

            // authentication (BearerAuth) required
            // bearer required
            if (language != null)
            {
                var languageQueryParams = ParameterToKeyValuePairs("", "language", language);
                foreach (var languageQueryParam in languageQueryParams)
                    if (!request.QueryParams.Contains(languageQueryParam))
                        request.QueryParams.Add(languageQueryParam);
            }

            request.PostBody = body;

            return request;
        }


        /// <summary>
        ///     Escape string (url-encoded).
        /// </summary>
        /// <param name="str">String to be escaped.</param>
        /// <returns>Escaped string.</returns>
        public string EscapeString(string str)
        {
            return UrlEncode(str);
        }


        /// <summary>
        ///     If parameter is DateTime, output in a formatted string (default ISO 8601), customizable with
        ///     Configuration.DateTime.
        ///     If parameter is a list, join the list with ",".
        ///     Otherwise just return the string.
        /// </summary>
        /// <param name="obj">The parameter (header, path, query, form).</param>
        /// <returns>Formatted string.</returns>
        public string ParameterToString(object obj)
        {
            if (obj is DateTime)
                // Return a formatted date string - Can be customized with Configuration.DateTimeFormat
                // Defaults to an ISO 8601, using the known as a Round-trip date/time pattern ("o")
                // https://msdn.microsoft.com/en-us/library/az4se3k1(v=vs.110).aspx#Anchor_8
                // For example: 2009-06-15T13:45:30.0000000
                return ((DateTime) obj).ToString(Configuration.DateTimeFormat);

            if (obj is DateTimeOffset)
                // Return a formatted date string - Can be customized with Configuration.DateTimeFormat
                // Defaults to an ISO 8601, using the known as a Round-trip date/time pattern ("o")
                // https://msdn.microsoft.com/en-us/library/az4se3k1(v=vs.110).aspx#Anchor_8
                // For example: 2009-06-15T13:45:30.0000000
                return ((DateTimeOffset) obj).ToString(Configuration.DateTimeFormat);

            if (obj is IList)
            {
                var flattenedString = new StringBuilder();
                foreach (var param in (IList) obj)
                {
                    if (flattenedString.Length > 0)
                        flattenedString.Append(",");
                    flattenedString.Append(param);
                }

                return flattenedString.ToString();
            }

            return Convert.ToString(obj);
        }

        /// <summary>
        ///     Deserialize the JSON string into a proper object.
        /// </summary>
        /// <param name="response">The HTTP response.</param>
        /// <param name="type">Object type.</param>
        /// <returns>Object representation of the JSON string.</returns>
        public object Deserialize(ResponseHelper response, Type type)
        {
            IDictionary<string, string> headers = response.Headers;
            if (type == typeof(byte[])) // return byte array
                return response.Data;

            // TODO: ? if (type.IsAssignableFrom(typeof(Stream)))
            if (type == typeof(Stream))
            {
                if (headers != null)
                {
                    var filePath = string.IsNullOrEmpty(Configuration.TempFolderPath)
                        ? Path.GetTempPath()
                        : Configuration.TempFolderPath;
                    var regex = new Regex(@"Content-Disposition=.*filename=['""]?([^'""\s]+)['""]?$");
                    foreach (var header in headers)
                    {
                        var match = regex.Match(header.ToString());
                        if (match.Success)
                        {
                            var fileName = filePath +
                                           SanitizeFilename(match.Groups[1].Value.Replace("\"", "")
                                               .Replace("'", ""));
                            File.WriteAllBytes(fileName, response.Data);
                            return new FileStream(fileName, FileMode.Open);
                        }
                    }
                }

                var stream = new MemoryStream(response.Data);
                return stream;
            }

            if (type.Name.StartsWith("System.Nullable`1[[System.DateTime")) // return a datetime object
                return DateTime.Parse(response.Text, null, DateTimeStyles.RoundtripKind);

            if (type == typeof(string) || type.Name.StartsWith("System.Nullable")) // return primitive type
                return ConvertType(response.Text, type);

            // at this point, it must be a model (json)
            try
            {
                return JsonConvert.DeserializeObject(response.Text, type, serializerSettings);
            }
            catch (Exception e)
            {
                throw new ApiException(500, e.Message);
            }
        }

        /// <summary>
        ///     Serialize an input (model) into JSON string
        /// </summary>
        /// <param name="obj">Object.</param>
        /// <returns>JSON string.</returns>
        public string Serialize(object obj)
        {
            try
            {
                return obj != null ? JsonConvert.SerializeObject(obj) : null;
            }
            catch (Exception e)
            {
                throw new ApiException(500, e.Message);
            }
        }

        /// <summary>
        ///     Check if the given MIME is a JSON MIME.
        ///     JSON MIME examples:
        ///     application/json
        ///     application/json; charset=UTF8
        ///     APPLICATION/JSON
        ///     application/vnd.company+json
        /// </summary>
        /// <param name="mime">MIME</param>
        /// <returns>Returns True if MIME type is json.</returns>
        public bool IsJsonMime(string mime)
        {
            var jsonRegex = new Regex("(?i)^(application/json|[^;/ \t]+/[^;/ \t]+[+]json)[ \t]*(;.*)?$");
            return mime != null && (jsonRegex.IsMatch(mime) || mime.Equals("application/json-patch+json"));
        }

        /// <summary>
        ///     Select the Content-Type header's value from the given content-type array:
        ///     if JSON type exists in the given array, use it;
        ///     otherwise use the first one defined in 'consumes'
        /// </summary>
        /// <param name="contentTypes">The Content-Type array to select from.</param>
        /// <returns>The Content-Type header to use.</returns>
        public string SelectHeaderContentType(string[] contentTypes)
        {
            if (contentTypes.Length == 0)
                return "application/json";

            foreach (var contentType in contentTypes)
                if (IsJsonMime(contentType.ToLower()))
                    return contentType;

            return contentTypes[0]; // use the first content type specified in 'consumes'
        }

        /// <summary>
        ///     Select the Accept header's value from the given accepts array:
        ///     if JSON exists in the given array, use it;
        ///     otherwise use all of them (joining into a string)
        /// </summary>
        /// <param name="accepts">The accepts array to select from.</param>
        /// <returns>The Accept header to use.</returns>
        public string SelectHeaderAccept(string[] accepts)
        {
            if (accepts.Length == 0)
                return null;

            if (accepts.Contains("application/json", StringComparer.OrdinalIgnoreCase))
                return "application/json";

            return string.Join(",", accepts);
        }

        /// <summary>
        ///     Encode string in base64 format.
        /// </summary>
        /// <param name="text">String to be encoded.</param>
        /// <returns>Encoded string.</returns>
        public static string Base64Encode(string text)
        {
            return Convert.ToBase64String(Encoding.UTF8.GetBytes(text));
        }

        /// <summary>
        ///     Dynamically cast the object into target type.
        /// </summary>
        /// <param name="fromObject">Object to be casted</param>
        /// <param name="toObject">Target type</param>
        /// <returns>Casted object</returns>
        public static dynamic ConvertType(dynamic fromObject, Type toObject)
        {
            return Convert.ChangeType(fromObject, toObject);
        }

        /// <summary>
        ///     Convert stream to byte array
        /// </summary>
        /// <param name="inputStream">Input stream to be converted</param>
        /// <returns>Byte array</returns>
        public static byte[] ReadAsBytes(Stream inputStream)
        {
            var buf = new byte[16 * 1024];
            using (var ms = new MemoryStream())
            {
                int count;
                while ((count = inputStream.Read(buf, 0, buf.Length)) > 0) ms.Write(buf, 0, count);

                return ms.ToArray();
            }
        }

        /// <summary>
        ///     URL encode a string
        ///     Credit/Ref: https://github.com/restsharp/RestSharp/blob/master/RestSharp/Extensions/StringExtensions.cs#L50
        /// </summary>
        /// <param name="input">String to be URL encoded</param>
        /// <returns>Byte array</returns>
        public static string UrlEncode(string input)
        {
            const int maxLength = 32766;

            if (input == null) throw new ArgumentNullException("input");

            if (input.Length <= maxLength) return Uri.EscapeDataString(input);

            var sb = new StringBuilder(input.Length * 2);
            var index = 0;

            while (index < input.Length)
            {
                var length = Math.Min(input.Length - index, maxLength);
                var subString = input.Substring(index, length);

                sb.Append(Uri.EscapeDataString(subString));
                index += subString.Length;
            }

            return sb.ToString();
        }

        /// <summary>
        ///     Sanitize filename by removing the path
        /// </summary>
        /// <param name="filename">Filename</param>
        /// <returns>Filename</returns>
        public static string SanitizeFilename(string filename)
        {
            var match = Regex.Match(filename, @".*[/\\](.*)$");

            if (match.Success)
                return match.Groups[1].Value;
            return filename;
        }

        /// <summary>
        ///     Convert params to key/value pairs.
        ///     Use collectionFormat to properly format lists and collections.
        /// </summary>
        /// <param name="name">Key name.</param>
        /// <param name="value">Value object.</param>
        /// <returns>A list of KeyValuePairs</returns>
        public IEnumerable<KeyValuePair<string, string>> ParameterToKeyValuePairs(string collectionFormat, string name,
            object value)
        {
            var parameters = new List<KeyValuePair<string, string>>();

            if (IsCollection(value) && collectionFormat == "multi")
            {
                var valueCollection = value as IEnumerable;
                parameters.AddRange(from object item in valueCollection
                    select new KeyValuePair<string, string>(name, ParameterToString(item)));
            }
            else
            {
                parameters.Add(new KeyValuePair<string, string>(name, ParameterToString(value)));
            }

            return parameters;
        }

        /// <summary>
        ///     Check if generic object is a collection.
        /// </summary>
        /// <param name="value"></param>
        /// <returns>True if object is a collection type</returns>
        private static bool IsCollection(object value)
        {
            return value is IList || value is ICollection;
        }
    }
}