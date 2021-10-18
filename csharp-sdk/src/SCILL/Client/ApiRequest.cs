using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SCILL.Client
{
    public class ApiRequest
    {
        private string _path;
        private HttpMethod _method;
        private List<KeyValuePair<String, String>> _queryParams;
        private Object _postBody;
        private Dictionary<String, String> _headerParams;
        private Dictionary<String, String> _formParams;
        private String _contentType;

        public ApiRequest(string path, HttpMethod method) : this(path, method,
            new List<KeyValuePair<string, string>>(), null, new Dictionary<string, string>(),
            new Dictionary<string, string>(), "application/json")
        {
        }


        public ApiRequest(string path, HttpMethod method, List<KeyValuePair<string, string>> queryParams,
            object postBody, Dictionary<string, string> headerParams, Dictionary<string, string> formParams,
            string contentType)
        {
            _path = path;
            _method = method;
            _queryParams = queryParams;
            _postBody = postBody;
            _headerParams = headerParams;
            _formParams = formParams;
            _contentType = contentType;
        }

        /// <summary>
        /// Adds the parameter with name <see cref="name"/> and value <see cref="value"/> to the request.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <param name="configuration"></param>
        /// <param name="collectionFormat"></param>
        public void AddQueryParameter(string name, object value, Configuration configuration,
            string collectionFormat = "")
        {
            if (null != value)
            {
                // configuration --> ApiClient access is required for 
                QueryParams.AddRange(
                    ParameterToKeyValuePairs(collectionFormat, name,
                        value, configuration));
            }
        }

        /// <summary>
        ///     Convert params to key/value pairs.
        ///     Use collectionFormat to properly format lists and collections.
        /// If <see cref="collectionFormat"/> is set to "multi", will assume
        /// that <see cref="value"/> is an IEnumerable and add all values as parameter.
        /// </summary>
        /// <param name="collectionFormat">"multi" or empty</param>
        /// <param name="name">Key name.</param>
        /// <param name="value">Value object.</param>
        /// <param name="configuration">The Api Configuration</param>
        /// <returns>A list of KeyValuePairs</returns>
        private static IEnumerable<KeyValuePair<string, string>> ParameterToKeyValuePairs(string collectionFormat, string name,
            object value, Configuration configuration)
        {
            var parameters = new List<KeyValuePair<string, string>>();

            if (IsCollection(value) && collectionFormat == "multi")
            {
                var valueCollection = value as IEnumerable;
                parameters.AddRange(from object item in valueCollection
                    select new KeyValuePair<string, string>(name, ParameterToString(item, configuration)));
            }
            else
            {
                parameters.Add(new KeyValuePair<string, string>(name, ParameterToString(value, configuration)));
            }

            return parameters;
        }

        /// <summary>
        ///     If parameter is DateTime, output in a formatted string (default ISO 8601), customizable with
        ///     Configuration.DateTime.
        ///     If parameter is a list, join the list with ",".
        ///     Otherwise just return the string.
        /// </summary>
        /// <param name="obj">The parameter (header, path, query, form).</param>
        /// <param name="configuration">The Api Configuration</param>
        /// <returns>Formatted string.</returns>
        private static string ParameterToString(object obj, Configuration configuration)
        {
            if (obj is DateTime)
                // Return a formatted date string - Can be customized with Configuration.DateTimeFormat
                // Defaults to an ISO 8601, using the known as a Round-trip date/time pattern ("o")
                // https://msdn.microsoft.com/en-us/library/az4se3k1(v=vs.110).aspx#Anchor_8
                // For example: 2009-06-15T13:45:30.0000000
                return ((DateTime)obj).ToString(configuration.DateTimeFormat);

            if (obj is DateTimeOffset)
                // Return a formatted date string - Can be customized with Configuration.DateTimeFormat
                // Defaults to an ISO 8601, using the known as a Round-trip date/time pattern ("o")
                // https://msdn.microsoft.com/en-us/library/az4se3k1(v=vs.110).aspx#Anchor_8
                // For example: 2009-06-15T13:45:30.0000000
                return ((DateTimeOffset)obj).ToString(configuration.DateTimeFormat);

            if (obj is IList)
            {
                var flattenedString = new StringBuilder();
                foreach (var param in (IList)obj)
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
        ///     Check if generic object is a collection.
        /// </summary>
        /// <param name="value"></param>
        /// <returns>True if object is a collection type</returns>
        private static bool IsCollection(object value)
        {
            return value is IList || value is ICollection;
        }

        public HttpMethod Method
        {
            get => _method;
            set => _method = value;
        }

        public List<KeyValuePair<string, string>> QueryParams
        {
            get => _queryParams;
            set => _queryParams = value;
        }

        public object PostBody
        {
            get => _postBody;
            set => _postBody = value;
        }

        public Dictionary<string, string> HeaderParams
        {
            get => _headerParams;
            set => _headerParams = value;
        }

        public Dictionary<string, string> FormParams
        {
            get => _formParams;
            set => _formParams = value;
        }

        public string ContentType
        {
            get => _contentType;
            set => _contentType = value;
        }


        public string Path
        {
            get => this._path;
            set => this._path = value;
        }
    }
}