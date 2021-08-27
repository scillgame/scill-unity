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
    /// Each level in battle passes contains one or more challenges that need to be fullfilled to unlock the next level. This structure holds challenge information and is based on the Challenge structure. However, as Battle Passes manage the lifecycle of challenges, this data structure is a bit simpler. The same principles apply mostly as for the personal challenges, i.e. you can share the exact same UI to render personal challenges and battle pass challenges.
    /// </summary>
    [DataContract]
        public partial class BattlePassLevelChallenge :  IEquatable<BattlePassLevelChallenge>, IValidatableObject
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BattlePassLevelChallenge" /> class.
        /// </summary>
        /// <param name="challengeId">The unique id of this challenge. Every challenge is linked to a product..</param>
        /// <param name="challengeName">The name of the challenge in the language set by the language parameter..</param>
        /// <param name="challengeGoal">Indicates how many “tasks” must be completed or done to complete this challenge..</param>
        /// <param name="challengeGoalCondition">With this you can set the way how the SCILL system approaches the challenges state. 0 means, that the counter of the challenge must be brought above the goal. If this is 1, then the counter must be kept below the goal. This is often useful for challenges that include times, like: Manage the level in under 50 seconds..</param>
        /// <param name="userChallengeCurrentScore">Indicates how many tasks the user already has completed. Use this in combination with challenge_goal to render a nice progress bar..</param>
        /// <param name="challengeXp">If you have experience, player rankings whatever, you can use this field to set the gain in that when this challenge is rewarded..</param>
        /// <param name="challengeIcon">In the admin panel you can set a string representing an image. This can be a URL, but it can also be an image or texture that you have in your games asset database..</param>
        /// <param name="challengeIconHd">This is the HD variant of the challenge icon image. If you have a game, that runs on multiple platforms that could come in handy. Otherwise just leave blank..</param>
        /// <param name="type">Indicates the status of the challenge. This can be one of the following unlock: Challenge does not track anything. in-progress: Challenge is active and tracking. overtime: User did not manage to finish the challenge in time. unclaimed: The challenge has been completed but the reward has not yet been claimed. finished: The challenge has been successfully be completed and the reward has been claimed.</param>
        public BattlePassLevelChallenge(string challengeId = default(string), string challengeName = default(string), int? challengeGoal = default(int?), int? challengeGoalCondition = default(int?), int? userChallengeCurrentScore = default(int?), int? challengeXp = default(int?), string challengeIcon = default(string), string challengeIconHd = default(string), string type = default(string))
        {
            this.challenge_id = challengeId;
            this.challenge_name = challengeName;
            this.challenge_goal = challengeGoal;
            this.challenge_goal_condition = challengeGoalCondition;
            this.user_challenge_current_score = userChallengeCurrentScore;
            this.challenge_xp = challengeXp;
            this.challenge_icon = challengeIcon;
            this.challenge_icon_hd = challengeIconHd;
            this.type = type;
        }
        
        /// <summary>
        /// The unique id of this challenge. Every challenge is linked to a product.
        /// </summary>
        /// <value>The unique id of this challenge. Every challenge is linked to a product.</value>
        [DataMember(Name="challenge_id", EmitDefaultValue=false)]
        public string challenge_id { get; set; }

        /// <summary>
        /// The name of the challenge in the language set by the language parameter.
        /// </summary>
        /// <value>The name of the challenge in the language set by the language parameter.</value>
        [DataMember(Name="challenge_name", EmitDefaultValue=false)]
        public string challenge_name { get; set; }

        /// <summary>
        /// Indicates how many “tasks” must be completed or done to complete this challenge.
        /// </summary>
        /// <value>Indicates how many “tasks” must be completed or done to complete this challenge.</value>
        [DataMember(Name="challenge_goal", EmitDefaultValue=false)]
        public int? challenge_goal { get; set; }

        /// <summary>
        /// With this you can set the way how the SCILL system approaches the challenges state. 0 means, that the counter of the challenge must be brought above the goal. If this is 1, then the counter must be kept below the goal. This is often useful for challenges that include times, like: Manage the level in under 50 seconds.
        /// </summary>
        /// <value>With this you can set the way how the SCILL system approaches the challenges state. 0 means, that the counter of the challenge must be brought above the goal. If this is 1, then the counter must be kept below the goal. This is often useful for challenges that include times, like: Manage the level in under 50 seconds.</value>
        [DataMember(Name="challenge_goal_condition", EmitDefaultValue=false)]
        public int? challenge_goal_condition { get; set; }

        /// <summary>
        /// Indicates how many tasks the user already has completed. Use this in combination with challenge_goal to render a nice progress bar.
        /// </summary>
        /// <value>Indicates how many tasks the user already has completed. Use this in combination with challenge_goal to render a nice progress bar.</value>
        [DataMember(Name="user_challenge_current_score", EmitDefaultValue=false)]
        public int? user_challenge_current_score { get; set; }

        /// <summary>
        /// If you have experience, player rankings whatever, you can use this field to set the gain in that when this challenge is rewarded.
        /// </summary>
        /// <value>If you have experience, player rankings whatever, you can use this field to set the gain in that when this challenge is rewarded.</value>
        [DataMember(Name="challenge_xp", EmitDefaultValue=false)]
        public int? challenge_xp { get; set; }

        /// <summary>
        /// In the admin panel you can set a string representing an image. This can be a URL, but it can also be an image or texture that you have in your games asset database.
        /// </summary>
        /// <value>In the admin panel you can set a string representing an image. This can be a URL, but it can also be an image or texture that you have in your games asset database.</value>
        [DataMember(Name="challenge_icon", EmitDefaultValue=false)]
        public string challenge_icon { get; set; }

        /// <summary>
        /// This is the HD variant of the challenge icon image. If you have a game, that runs on multiple platforms that could come in handy. Otherwise just leave blank.
        /// </summary>
        /// <value>This is the HD variant of the challenge icon image. If you have a game, that runs on multiple platforms that could come in handy. Otherwise just leave blank.</value>
        [DataMember(Name="challenge_icon_hd", EmitDefaultValue=false)]
        public string challenge_icon_hd { get; set; }

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
            sb.Append("class BattlePassLevelChallenge {\n");
            sb.Append("  challenge_id: ").Append(challenge_id).Append("\n");
            sb.Append("  challenge_name: ").Append(challenge_name).Append("\n");
            sb.Append("  challenge_goal: ").Append(challenge_goal).Append("\n");
            sb.Append("  challenge_goal_condition: ").Append(challenge_goal_condition).Append("\n");
            sb.Append("  user_challenge_current_score: ").Append(user_challenge_current_score).Append("\n");
            sb.Append("  challenge_xp: ").Append(challenge_xp).Append("\n");
            sb.Append("  challenge_icon: ").Append(challenge_icon).Append("\n");
            sb.Append("  challenge_icon_hd: ").Append(challenge_icon_hd).Append("\n");
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
            return this.Equals(input as BattlePassLevelChallenge);
        }

        /// <summary>
        /// Returns true if BattlePassLevelChallenge instances are equal
        /// </summary>
        /// <param name="input">Instance of BattlePassLevelChallenge to be compared</param>
        /// <returns>Boolean</returns>
        public bool Equals(BattlePassLevelChallenge input)
        {
            if (input == null)
                return false;

            return 
                (
                    this.challenge_id == input.challenge_id ||
                    (this.challenge_id != null &&
                    this.challenge_id.Equals(input.challenge_id))
                ) && 
                (
                    this.challenge_name == input.challenge_name ||
                    (this.challenge_name != null &&
                    this.challenge_name.Equals(input.challenge_name))
                ) && 
                (
                    this.challenge_goal == input.challenge_goal ||
                    (this.challenge_goal != null &&
                    this.challenge_goal.Equals(input.challenge_goal))
                ) && 
                (
                    this.challenge_goal_condition == input.challenge_goal_condition ||
                    (this.challenge_goal_condition != null &&
                    this.challenge_goal_condition.Equals(input.challenge_goal_condition))
                ) && 
                (
                    this.user_challenge_current_score == input.user_challenge_current_score ||
                    (this.user_challenge_current_score != null &&
                    this.user_challenge_current_score.Equals(input.user_challenge_current_score))
                ) && 
                (
                    this.challenge_xp == input.challenge_xp ||
                    (this.challenge_xp != null &&
                    this.challenge_xp.Equals(input.challenge_xp))
                ) && 
                (
                    this.challenge_icon == input.challenge_icon ||
                    (this.challenge_icon != null &&
                    this.challenge_icon.Equals(input.challenge_icon))
                ) && 
                (
                    this.challenge_icon_hd == input.challenge_icon_hd ||
                    (this.challenge_icon_hd != null &&
                    this.challenge_icon_hd.Equals(input.challenge_icon_hd))
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
                if (this.challenge_id != null)
                    hashCode = hashCode * 59 + this.challenge_id.GetHashCode();
                if (this.challenge_name != null)
                    hashCode = hashCode * 59 + this.challenge_name.GetHashCode();
                if (this.challenge_goal != null)
                    hashCode = hashCode * 59 + this.challenge_goal.GetHashCode();
                if (this.challenge_goal_condition != null)
                    hashCode = hashCode * 59 + this.challenge_goal_condition.GetHashCode();
                if (this.user_challenge_current_score != null)
                    hashCode = hashCode * 59 + this.user_challenge_current_score.GetHashCode();
                if (this.challenge_xp != null)
                    hashCode = hashCode * 59 + this.challenge_xp.GetHashCode();
                if (this.challenge_icon != null)
                    hashCode = hashCode * 59 + this.challenge_icon.GetHashCode();
                if (this.challenge_icon_hd != null)
                    hashCode = hashCode * 59 + this.challenge_icon_hd.GetHashCode();
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