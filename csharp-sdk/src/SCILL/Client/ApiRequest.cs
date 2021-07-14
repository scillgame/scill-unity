using System;
using System.Collections.Generic;

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