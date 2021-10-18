/* 
 * SCILL API
 *
 * SCILL gives you the tools to activate, retain and grow your user base in your app or game by bringing you features well known in the gaming industry: Gamification. We take care of the services and technology involved so you can focus on your game and content.
 *
 * OpenAPI spec version: 1.1.0
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
    /// The ranking for the user or team in the leaderboard
    /// </summary>
    [DataContract]
        public partial class LeaderboardMember :  IEquatable<LeaderboardMember>, IValidatableObject
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LeaderboardMember" /> class.
        /// </summary>
        /// <param name="memberId">The id of the user - its the same user id you used to create the access token and the same user id you used to send the events.</param>
        /// <param name="memberType">Indicates what type this entry is, it&#x27;s either user or team.</param>
        /// <param name="score">The score achieved as an integer value. If you want to store floats, for example laptimes you need to convert them into an int before (i.e. multiply by 100 to get hundreds of seconds and format back to float in UI).</param>
        /// <param name="rank">The position within the leaderboard.</param>
        /// <param name="metadataResults">Returns an array of the LeaderboardMemberMetadata objects containing user ranking metadata information.</param>
        /// <param name="additionalInfo">additionalInfo.</param>
        public LeaderboardMember(string memberId = default(string), string memberType = default(string), int? score = default(int?), int? rank = default(int?), List<LeaderboardMemberMetadata> metadataResults = default(List<LeaderboardMemberMetadata>), UserInfo additionalInfo = default(UserInfo))
        {
            this.member_id = memberId;
            this.member_type = memberType;
            this.score = score;
            this.rank = rank;
            this.metadata_results = metadataResults;
            this.additional_info = additionalInfo;
        }
        
        /// <summary>
        /// The id of the user - its the same user id you used to create the access token and the same user id you used to send the events
        /// </summary>
        /// <value>The id of the user - its the same user id you used to create the access token and the same user id you used to send the events</value>
        [DataMember(Name="member_id", EmitDefaultValue=false)]
        public string member_id { get; set; }

        /// <summary>
        /// Indicates what type this entry is, it&#x27;s either user or team
        /// </summary>
        /// <value>Indicates what type this entry is, it&#x27;s either user or team</value>
        [DataMember(Name="member_type", EmitDefaultValue=false)]
        public string member_type { get; set; }

        /// <summary>
        /// The score achieved as an integer value. If you want to store floats, for example laptimes you need to convert them into an int before (i.e. multiply by 100 to get hundreds of seconds and format back to float in UI)
        /// </summary>
        /// <value>The score achieved as an integer value. If you want to store floats, for example laptimes you need to convert them into an int before (i.e. multiply by 100 to get hundreds of seconds and format back to float in UI)</value>
        [DataMember(Name="score", EmitDefaultValue=false)]
        public int? score { get; set; }

        /// <summary>
        /// The position within the leaderboard
        /// </summary>
        /// <value>The position within the leaderboard</value>
        [DataMember(Name="rank", EmitDefaultValue=false)]
        public int? rank { get; set; }

        /// <summary>
        /// Returns an array of the LeaderboardMemberMetadata objects containing user ranking metadata information
        /// </summary>
        /// <value>Returns an array of the LeaderboardMemberMetadata objects containing user ranking metadata information</value>
        [DataMember(Name="metadata_results", EmitDefaultValue=false)]
        public List<LeaderboardMemberMetadata> metadata_results { get; set; }

        /// <summary>
        /// Gets or Sets additional_info
        /// </summary>
        [DataMember(Name="additional_info", EmitDefaultValue=false)]
        public UserInfo additional_info { get; set; }

        /// <summary>
        /// Returns the string presentation of the object
        /// </summary>
        /// <returns>String presentation of the object</returns>
        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append("class LeaderboardMember {\n");
            sb.Append("  member_id: ").Append(member_id).Append("\n");
            sb.Append("  member_type: ").Append(member_type).Append("\n");
            sb.Append("  score: ").Append(score).Append("\n");
            sb.Append("  rank: ").Append(rank).Append("\n");
            sb.Append("  metadata_results: ").Append(metadata_results).Append("\n");
            sb.Append("  additional_info: ").Append(additional_info).Append("\n");
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
            return this.Equals(input as LeaderboardMember);
        }

        /// <summary>
        /// Returns true if LeaderboardMember instances are equal
        /// </summary>
        /// <param name="input">Instance of LeaderboardMember to be compared</param>
        /// <returns>Boolean</returns>
        public bool Equals(LeaderboardMember input)
        {
            if (input == null)
                return false;

            return 
                (
                    this.member_id == input.member_id ||
                    (this.member_id != null &&
                    this.member_id.Equals(input.member_id))
                ) && 
                (
                    this.member_type == input.member_type ||
                    (this.member_type != null &&
                    this.member_type.Equals(input.member_type))
                ) && 
                (
                    this.score == input.score ||
                    (this.score != null &&
                    this.score.Equals(input.score))
                ) && 
                (
                    this.rank == input.rank ||
                    (this.rank != null &&
                    this.rank.Equals(input.rank))
                ) && 
                (
                    this.metadata_results == input.metadata_results ||
                    this.metadata_results != null &&
                    input.metadata_results != null &&
                    this.metadata_results.SequenceEqual(input.metadata_results)
                ) && 
                (
                    this.additional_info == input.additional_info ||
                    (this.additional_info != null &&
                    this.additional_info.Equals(input.additional_info))
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
                if (this.member_id != null)
                    hashCode = hashCode * 59 + this.member_id.GetHashCode();
                if (this.member_type != null)
                    hashCode = hashCode * 59 + this.member_type.GetHashCode();
                if (this.score != null)
                    hashCode = hashCode * 59 + this.score.GetHashCode();
                if (this.rank != null)
                    hashCode = hashCode * 59 + this.rank.GetHashCode();
                if (this.metadata_results != null)
                    hashCode = hashCode * 59 + this.metadata_results.GetHashCode();
                if (this.additional_info != null)
                    hashCode = hashCode * 59 + this.additional_info.GetHashCode();
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
