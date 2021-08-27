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
    /// This object stores information about a battle pass challenge state. It is designed to update challenges loaded previously with the getBattlePassLevels API. Indices allow you to quickly update locally stored Challenge objects without iterating or reloading data.
    /// </summary>
    [DataContract]
        public partial class BattlePassChallengeState :  IEquatable<BattlePassChallengeState>, IValidatableObject
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BattlePassChallengeState" /> class.
        /// </summary>
        /// <param name="appId">The unique id of the app.</param>
        /// <param name="battlePassId">The unique id of this battle pass..</param>
        /// <param name="levelId">Unique id of this BattlePassLevel object..</param>
        /// <param name="userId">This is your user id. You can set this to whatever you like, either your real user id or an obfuscated user id. However you need to be consistent here. Events linked to this user id only track if challenges or battle passes are unlocked with the same user id..</param>
        /// <param name="levelPositionIndex">Typical usage pattern is to load battle pass levels with getBattlePassLevels operation and store them for rendering. Using this value you can quickly identify the index of the level that changed..</param>
        /// <param name="challengeId">The unique id of this challenge. Every challenge is linked to a product..</param>
        /// <param name="challengePositionIndex">Same as level_position_index. Use this index to identify the challenge that changed within the levels challenges array. Typical usage pattern is to update the previously stored score and type..</param>
        /// <param name="challengeGoal">Indicates how many “tasks” must be completed or done to complete this challenge..</param>
        /// <param name="userChallengeCurrentScore">Indicates how many tasks the user already has completed. Use this in combination with challenge_goal to render a nice progress bar..</param>
        /// <param name="type">Indicates the status of the challenge. This can be one of the following unlock: Challenge does not track anything. in-progress: Challenge is active and tracking. overtime: User did not manage to finish the challenge in time. unclaimed: The challenge has been completed but the reward has not yet been claimed. finished: The challenge has been successfully be completed and the reward has been claimed.</param>
        public BattlePassChallengeState(string appId = default(string), string battlePassId = default(string), string levelId = default(string), string userId = default(string), int? levelPositionIndex = default(int?), string challengeId = default(string), int? challengePositionIndex = default(int?), int? challengeGoal = default(int?), int? userChallengeCurrentScore = default(int?), string type = default(string))
        {
            this.app_id = appId;
            this.battle_pass_id = battlePassId;
            this.level_id = levelId;
            this.user_id = userId;
            this.level_position_index = levelPositionIndex;
            this.challenge_id = challengeId;
            this.challenge_position_index = challengePositionIndex;
            this.challenge_goal = challengeGoal;
            this.user_challenge_current_score = userChallengeCurrentScore;
            this.type = type;
        }
        
        /// <summary>
        /// The unique id of the app
        /// </summary>
        /// <value>The unique id of the app</value>
        [DataMember(Name="app_id", EmitDefaultValue=false)]
        public string app_id { get; set; }

        /// <summary>
        /// The unique id of this battle pass.
        /// </summary>
        /// <value>The unique id of this battle pass.</value>
        [DataMember(Name="battle_pass_id", EmitDefaultValue=false)]
        public string battle_pass_id { get; set; }

        /// <summary>
        /// Unique id of this BattlePassLevel object.
        /// </summary>
        /// <value>Unique id of this BattlePassLevel object.</value>
        [DataMember(Name="level_id", EmitDefaultValue=false)]
        public string level_id { get; set; }

        /// <summary>
        /// This is your user id. You can set this to whatever you like, either your real user id or an obfuscated user id. However you need to be consistent here. Events linked to this user id only track if challenges or battle passes are unlocked with the same user id.
        /// </summary>
        /// <value>This is your user id. You can set this to whatever you like, either your real user id or an obfuscated user id. However you need to be consistent here. Events linked to this user id only track if challenges or battle passes are unlocked with the same user id.</value>
        [DataMember(Name="user_id", EmitDefaultValue=false)]
        public string user_id { get; set; }

        /// <summary>
        /// Typical usage pattern is to load battle pass levels with getBattlePassLevels operation and store them for rendering. Using this value you can quickly identify the index of the level that changed.
        /// </summary>
        /// <value>Typical usage pattern is to load battle pass levels with getBattlePassLevels operation and store them for rendering. Using this value you can quickly identify the index of the level that changed.</value>
        [DataMember(Name="level_position_index", EmitDefaultValue=false)]
        public int? level_position_index { get; set; }

        /// <summary>
        /// The unique id of this challenge. Every challenge is linked to a product.
        /// </summary>
        /// <value>The unique id of this challenge. Every challenge is linked to a product.</value>
        [DataMember(Name="challenge_id", EmitDefaultValue=false)]
        public string challenge_id { get; set; }

        /// <summary>
        /// Same as level_position_index. Use this index to identify the challenge that changed within the levels challenges array. Typical usage pattern is to update the previously stored score and type.
        /// </summary>
        /// <value>Same as level_position_index. Use this index to identify the challenge that changed within the levels challenges array. Typical usage pattern is to update the previously stored score and type.</value>
        [DataMember(Name="challenge_position_index", EmitDefaultValue=false)]
        public int? challenge_position_index { get; set; }

        /// <summary>
        /// Indicates how many “tasks” must be completed or done to complete this challenge.
        /// </summary>
        /// <value>Indicates how many “tasks” must be completed or done to complete this challenge.</value>
        [DataMember(Name="challenge_goal", EmitDefaultValue=false)]
        public int? challenge_goal { get; set; }

        /// <summary>
        /// Indicates how many tasks the user already has completed. Use this in combination with challenge_goal to render a nice progress bar.
        /// </summary>
        /// <value>Indicates how many tasks the user already has completed. Use this in combination with challenge_goal to render a nice progress bar.</value>
        [DataMember(Name="user_challenge_current_score", EmitDefaultValue=false)]
        public int? user_challenge_current_score { get; set; }

        /// <summary>
        /// Indicates the status of the challenge. This can be one of the following unlock: Challenge does not track anything. in-progress: Challenge is active and tracking. overtime: User did not manage to finish the challenge in time. unclaimed: The challenge has been completed but the reward has not yet been claimed. finished: The challenge has been successfully be completed and the reward has been claimed
        /// </summary>
        /// <value>Indicates the status of the challenge. This can be one of the following unlock: Challenge does not track anything. in-progress: Challenge is active and tracking. overtime: User did not manage to finish the challenge in time. unclaimed: The challenge has been completed but the reward has not yet been claimed. finished: The challenge has been successfully be completed and the reward has been claimed</value>
        [DataMember(Name="type", EmitDefaultValue=false)]
        public string type { get; set; }

        /// <summary>
        /// Returns the string presentation of the object
        /// </summary>
        /// <returns>String presentation of the object</returns>
        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append("class BattlePassChallengeState {\n");
            sb.Append("  app_id: ").Append(app_id).Append("\n");
            sb.Append("  battle_pass_id: ").Append(battle_pass_id).Append("\n");
            sb.Append("  level_id: ").Append(level_id).Append("\n");
            sb.Append("  user_id: ").Append(user_id).Append("\n");
            sb.Append("  level_position_index: ").Append(level_position_index).Append("\n");
            sb.Append("  challenge_id: ").Append(challenge_id).Append("\n");
            sb.Append("  challenge_position_index: ").Append(challenge_position_index).Append("\n");
            sb.Append("  challenge_goal: ").Append(challenge_goal).Append("\n");
            sb.Append("  user_challenge_current_score: ").Append(user_challenge_current_score).Append("\n");
            sb.Append("  type: ").Append(type).Append("\n");
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
            return this.Equals(input as BattlePassChallengeState);
        }

        /// <summary>
        /// Returns true if BattlePassChallengeState instances are equal
        /// </summary>
        /// <param name="input">Instance of BattlePassChallengeState to be compared</param>
        /// <returns>Boolean</returns>
        public bool Equals(BattlePassChallengeState input)
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
                    this.battle_pass_id == input.battle_pass_id ||
                    (this.battle_pass_id != null &&
                    this.battle_pass_id.Equals(input.battle_pass_id))
                ) && 
                (
                    this.level_id == input.level_id ||
                    (this.level_id != null &&
                    this.level_id.Equals(input.level_id))
                ) && 
                (
                    this.user_id == input.user_id ||
                    (this.user_id != null &&
                    this.user_id.Equals(input.user_id))
                ) && 
                (
                    this.level_position_index == input.level_position_index ||
                    (this.level_position_index != null &&
                    this.level_position_index.Equals(input.level_position_index))
                ) && 
                (
                    this.challenge_id == input.challenge_id ||
                    (this.challenge_id != null &&
                    this.challenge_id.Equals(input.challenge_id))
                ) && 
                (
                    this.challenge_position_index == input.challenge_position_index ||
                    (this.challenge_position_index != null &&
                    this.challenge_position_index.Equals(input.challenge_position_index))
                ) && 
                (
                    this.challenge_goal == input.challenge_goal ||
                    (this.challenge_goal != null &&
                    this.challenge_goal.Equals(input.challenge_goal))
                ) && 
                (
                    this.user_challenge_current_score == input.user_challenge_current_score ||
                    (this.user_challenge_current_score != null &&
                    this.user_challenge_current_score.Equals(input.user_challenge_current_score))
                ) && 
                (
                    this.type == input.type ||
                    (this.type != null &&
                    this.type.Equals(input.type))
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
                if (this.battle_pass_id != null)
                    hashCode = hashCode * 59 + this.battle_pass_id.GetHashCode();
                if (this.level_id != null)
                    hashCode = hashCode * 59 + this.level_id.GetHashCode();
                if (this.user_id != null)
                    hashCode = hashCode * 59 + this.user_id.GetHashCode();
                if (this.level_position_index != null)
                    hashCode = hashCode * 59 + this.level_position_index.GetHashCode();
                if (this.challenge_id != null)
                    hashCode = hashCode * 59 + this.challenge_id.GetHashCode();
                if (this.challenge_position_index != null)
                    hashCode = hashCode * 59 + this.challenge_position_index.GetHashCode();
                if (this.challenge_goal != null)
                    hashCode = hashCode * 59 + this.challenge_goal.GetHashCode();
                if (this.user_challenge_current_score != null)
                    hashCode = hashCode * 59 + this.user_challenge_current_score.GetHashCode();
                if (this.type != null)
                    hashCode = hashCode * 59 + this.type.GetHashCode();
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