
using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

namespace SCILL
{
    /// <summary>
    /// Configuration class, defining the parameters available in the
    /// SCILLConfig.json file, which can be used to adjust SDK settings, e.g.
    /// the Api Version that should be used.
    /// </summary>
    public class SCILLSettings
    {
        public string ApiVersion { get; set; } = "v2";
        public string Domain { get; set; } = "scill.4players.io";
        public string DomainPrefixAuthentication { get; set; } = "us";
        public string DomainPrefixEvents { get; set; } = "ep";
        public string DomainPrefixChallenges { get; set; } = "pcs";
        public string DomainPrefixBattlePasses { get; set; } = "es";
        public string DomainPrefixLeaderboards { get; set; } = "ls";

        private Dictionary<ApiEndpointType, string> endpointTypeToPrefix = new Dictionary<ApiEndpointType, string>();

        public SCILLSettings()
        {
            Init();
        }
        
        public string GetApiEndpointURL(ApiEndpointType endpointType, string hostSuffix)
        {
            if(endpointTypeToPrefix.Keys.Count < 1)
                Init();

            string endpointUrl = null;
            if (endpointTypeToPrefix.ContainsKey(endpointType))
            {
                string prefix = endpointTypeToPrefix[endpointType];
                endpointUrl = "https://"
                                     + prefix
                                     + hostSuffix
                                     + "."
                                     + Domain;
            }
            return endpointUrl;
        }

        /// <summary>
        /// Retrieves the configuration from the given file path. Expects a json
        /// compatible text file.
        /// </summary>
        /// <param name="filePath">Path to the configuration file relative to a Resources folder</param>
        /// <returns>The found configuration or null, if file was not found.</returns>
        public static SCILLSettings Load(string filePath = "SCILLConfig")
        {
            SCILLSettings result = null;
            TextAsset configAsset = Resources.Load<TextAsset>(filePath);
            if (configAsset)
            {
                result = JsonConvert.DeserializeObject<SCILLSettings>(configAsset.text);
                result.Init();
            }
            else
            {
                Debug.LogWarning($"Loading {filePath} from Resources failed, fallback to default configuration.");
            }
            return result;
        }

        private void Init()
        {
            endpointTypeToPrefix.Clear();
            endpointTypeToPrefix[ApiEndpointType.Authentication] = DomainPrefixAuthentication;
            endpointTypeToPrefix[ApiEndpointType.Events] = DomainPrefixEvents;
            endpointTypeToPrefix[ApiEndpointType.Challenges] = DomainPrefixChallenges;
            endpointTypeToPrefix[ApiEndpointType.BattlePasses] = DomainPrefixBattlePasses;
            endpointTypeToPrefix[ApiEndpointType.Leaderboards] = DomainPrefixLeaderboards;
            
        }
        
        public enum ApiEndpointType
        {
            Authentication,
            Events,
            Challenges,
            BattlePasses,
            Leaderboards
        }
    }
}   