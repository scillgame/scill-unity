/* 
 * SCILL API
 *
 * SCILL gives you the tools to activate, retain and grow your user base in your app or game by bringing you features well known in the gaming industry: Gamification. We take care of the services and technology involved so you can focus on your game and content.
 *
 * OpenAPI spec version: 1.2.0
 * Contact: support@scillgame.com
 * Generated by: https://github.com/swagger-api/swagger-codegen.git
 */
using System;
using System.Linq;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.ComponentModel.DataAnnotations;
using SwaggerDateConverter = SCILL.Client.SwaggerDateConverter;

namespace SCILL.Model
{
    /// <summary>
    /// The Leaderboard object contains information about the leaderboard itself like the name and the id
    /// </summary>
    [DataContract]
        public partial class LeaderboardInfo :  IEquatable<LeaderboardInfo>, IValidatableObject
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LeaderboardInfo" /> class.
        /// </summary>
        /// <param name="appId">The id of the app.</param>
        /// <param name="leaderboardId">The id of the leaderboard.</param>
        /// <param name="leaderboardName">The name of the leaderboard.</param>
        /// <param name="eventType">The event type that triggers this leaderboard.</param>
        /// <param name="sortOrderAscending">True if this leaderboard sorts the score ascending or false if the ranking is defined by a descending score..</param>
        public LeaderboardInfo(string appId = default(string), string leaderboardId = default(string), string leaderboardName = default(string), string eventType = default(string), bool? sortOrderAscending = default(bool?))
        {
            this.app_id = appId;
            this.leaderboard_id = leaderboardId;
            this.leaderboard_name = leaderboardName;
            this.event_type = eventType;
            this.sort_order_ascending = sortOrderAscending;
        }
        
        /// <summary>
        /// The id of the app
        /// </summary>
        /// <value>The id of the app</value>
        [DataMember(Name="app_id", EmitDefaultValue=false)]
        public string app_id { get; set; }

        /// <summary>
        /// The id of the leaderboard
        /// </summary>
        /// <value>The id of the leaderboard</value>
        [DataMember(Name="leaderboard_id", EmitDefaultValue=false)]
        public string leaderboard_id { get; set; }

        /// <summary>
        /// The name of the leaderboard
        /// </summary>
        /// <value>The name of the leaderboard</value>
        [DataMember(Name="leaderboard_name", EmitDefaultValue=false)]
        public string leaderboard_name { get; set; }

        /// <summary>
        /// The event type that triggers this leaderboard
        /// </summary>
        /// <value>The event type that triggers this leaderboard</value>
        [DataMember(Name="event_type", EmitDefaultValue=false)]
        public string event_type { get; set; }

        /// <summary>
        /// True if this leaderboard sorts the score ascending or false if the ranking is defined by a descending score.
        /// </summary>
        /// <value>True if this leaderboard sorts the score ascending or false if the ranking is defined by a descending score.</value>
        [DataMember(Name="sort_order_ascending", EmitDefaultValue=false)]
        public bool? sort_order_ascending { get; set; }

        /// <summary>
        /// Returns the string presentation of the object
        /// </summary>
        /// <returns>String presentation of the object</returns>
        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append("class LeaderboardInfo {\n");
            sb.Append("  app_id: ").Append(app_id).Append("\n");
            sb.Append("  leaderboard_id: ").Append(leaderboard_id).Append("\n");
            sb.Append("  leaderboard_name: ").Append(leaderboard_name).Append("\n");
            sb.Append("  event_type: ").Append(event_type).Append("\n");
            sb.Append("  sort_order_ascending: ").Append(sort_order_ascending).Append("\n");
            sb.Append("}\n");
            return sb.ToString();
        }
  
        /// <summary>
        /// Returns the JSON string presentation of the object
        /// </summary>
        /// <returns>JSON string presentation of the object</returns>
        public virtual string ToJson()
        {
            return JsonConvert.SerializeObject(this, Formatting.Indented);
        }

        /// <summary>
        /// Returns true if objects are equal
        /// </summary>
        /// <param name="input">Object to be compared</param>
        /// <returns>Boolean</returns>
        public override bool Equals(object input)
        {
            return this.Equals(input as LeaderboardInfo);
        }

        /// <summary>
        /// Returns true if LeaderboardInfo instances are equal
        /// </summary>
        /// <param name="input">Instance of LeaderboardInfo to be compared</param>
        /// <returns>Boolean</returns>
        public bool Equals(LeaderboardInfo input)
        {
            if (input == null)
                return false;

            return 
                (
                    this.app_id == input.app_id ||
                    (this.app_id != null &&
                    this.app_id.Equals(input.app_id))
                ) && 
                (
                    this.leaderboard_id == input.leaderboard_id ||
                    (this.leaderboard_id != null &&
                    this.leaderboard_id.Equals(input.leaderboard_id))
                ) && 
                (
                    this.leaderboard_name == input.leaderboard_name ||
                    (this.leaderboard_name != null &&
                    this.leaderboard_name.Equals(input.leaderboard_name))
                ) && 
                (
                    this.event_type == input.event_type ||
                    (this.event_type != null &&
                    this.event_type.Equals(input.event_type))
                ) && 
                (
                    this.sort_order_ascending == input.sort_order_ascending ||
                    (this.sort_order_ascending != null &&
                    this.sort_order_ascending.Equals(input.sort_order_ascending))
                );
        }

        /// <summary>
        /// Gets the hash code
        /// </summary>
        /// <returns>Hash code</returns>
        public override int GetHashCode()
        {
            unchecked // Overflow is fine, just wrap
            {
                int hashCode = 41;
                if (this.app_id != null)
                    hashCode = hashCode * 59 + this.app_id.GetHashCode();
                if (this.leaderboard_id != null)
                    hashCode = hashCode * 59 + this.leaderboard_id.GetHashCode();
                if (this.leaderboard_name != null)
                    hashCode = hashCode * 59 + this.leaderboard_name.GetHashCode();
                if (this.event_type != null)
                    hashCode = hashCode * 59 + this.event_type.GetHashCode();
                if (this.sort_order_ascending != null)
                    hashCode = hashCode * 59 + this.sort_order_ascending.GetHashCode();
                return hashCode;
            }
        }

        /// <summary>
        /// To validate all properties of the instance
        /// </summary>
        /// <param name="validationContext">Validation context</param>
        /// <returns>Validation Result</returns>
        IEnumerable<System.ComponentModel.DataAnnotations.ValidationResult> IValidatableObject.Validate(ValidationContext validationContext)
        {
            yield break;
        }
    }
}
