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
    /// Battle Passes are grouped into levels that contain challenges that must be achieved to unlock the next level. Only challenges for the current level are tracking progress.
    /// </summary>
    [DataContract]
        public partial class BattlePassLevel :  IEquatable<BattlePassLevel>, IValidatableObject
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BattlePassLevel" /> class.
        /// </summary>
        /// <param name="levelId">Unique id of this BattlePassLevel object..</param>
        /// <param name="appId">The app id.</param>
        /// <param name="battlePassId">The id of the battle pass this level belongs to.</param>
        /// <param name="rewardAmount">In the Admin Panel you can set different types of rewards. You can also set an identifier of an in-game-item or anything you like. Use this to include the reward into your own business logic..</param>
        /// <param name="rewardTypeName">There are different types of rewards available. Possible values are Coins, Voucher, Money and Experience. This is deprecated in favor of level_reward_type which uses a slug instead of a human readable expression.</param>
        /// <param name="levelRewardType">The reward type in a machine readable slug. Available values are nothing, coin, experience, item.</param>
        /// <param name="levelCompleted">Indicates if this level is completed, i.e. all challenges have been completed..</param>
        /// <param name="levelPriority">Indicates the position of the level..</param>
        /// <param name="rewardClaimed">Indicates if this level has already be claimed..</param>
        /// <param name="activatedAt">The date when this level has been activated or null if it&#x27;s not activated..</param>
        /// <param name="challenges">An array of BattlePassLevelChallenge objects. Please note, not all values are available from the challenge object, as battle passes handle the lifecycle of challenges..</param>
        public BattlePassLevel(string levelId = default(string), string appId = default(string), string battlePassId = default(string), string rewardAmount = default(string), string rewardTypeName = default(string), string levelRewardType = default(string), bool? levelCompleted = default(bool?), decimal? levelPriority = default(decimal?), bool? rewardClaimed = default(bool?), string activatedAt = default(string), List<BattlePassLevelChallenge> challenges = default(List<BattlePassLevelChallenge>))
        {
            this.level_id = levelId;
            this.app_id = appId;
            this.battle_pass_id = battlePassId;
            this.reward_amount = rewardAmount;
            this.reward_type_name = rewardTypeName;
            this.level_reward_type = levelRewardType;
            this.level_completed = levelCompleted;
            this.level_priority = levelPriority;
            this.reward_claimed = rewardClaimed;
            this.activated_at = activatedAt;
            this.challenges = challenges;
        }
        
        /// <summary>
        /// Unique id of this BattlePassLevel object.
        /// </summary>
        /// <value>Unique id of this BattlePassLevel object.</value>
        [DataMember(Name="level_id", EmitDefaultValue=false)]
        public string level_id { get; set; }

        /// <summary>
        /// The app id
        /// </summary>
        /// <value>The app id</value>
        [DataMember(Name="app_id", EmitDefaultValue=false)]
        public string app_id { get; set; }

        /// <summary>
        /// The id of the battle pass this level belongs to
        /// </summary>
        /// <value>The id of the battle pass this level belongs to</value>
        [DataMember(Name="battle_pass_id", EmitDefaultValue=false)]
        public string battle_pass_id { get; set; }

        /// <summary>
        /// In the Admin Panel you can set different types of rewards. You can also set an identifier of an in-game-item or anything you like. Use this to include the reward into your own business logic.
        /// </summary>
        /// <value>In the Admin Panel you can set different types of rewards. You can also set an identifier of an in-game-item or anything you like. Use this to include the reward into your own business logic.</value>
        [DataMember(Name="reward_amount", EmitDefaultValue=false)]
        public string reward_amount { get; set; }

        /// <summary>
        /// There are different types of rewards available. Possible values are Coins, Voucher, Money and Experience. This is deprecated in favor of level_reward_type which uses a slug instead of a human readable expression
        /// </summary>
        /// <value>There are different types of rewards available. Possible values are Coins, Voucher, Money and Experience. This is deprecated in favor of level_reward_type which uses a slug instead of a human readable expression</value>
        [DataMember(Name="reward_type_name", EmitDefaultValue=false)]
        public string reward_type_name { get; set; }

        /// <summary>
        /// The reward type in a machine readable slug. Available values are nothing, coin, experience, item
        /// </summary>
        /// <value>The reward type in a machine readable slug. Available values are nothing, coin, experience, item</value>
        [DataMember(Name="level_reward_type", EmitDefaultValue=false)]
        public string level_reward_type { get; set; }

        /// <summary>
        /// Indicates if this level is completed, i.e. all challenges have been completed.
        /// </summary>
        /// <value>Indicates if this level is completed, i.e. all challenges have been completed.</value>
        [DataMember(Name="level_completed", EmitDefaultValue=false)]
        public bool? level_completed { get; set; }

        /// <summary>
        /// Indicates the position of the level.
        /// </summary>
        /// <value>Indicates the position of the level.</value>
        [DataMember(Name="level_priority", EmitDefaultValue=false)]
        public decimal? level_priority { get; set; }

        /// <summary>
        /// Indicates if this level has already be claimed.
        /// </summary>
        /// <value>Indicates if this level has already be claimed.</value>
        [DataMember(Name="reward_claimed", EmitDefaultValue=false)]
        public bool? reward_claimed { get; set; }

        /// <summary>
        /// The date when this level has been activated or null if it&#x27;s not activated.
        /// </summary>
        /// <value>The date when this level has been activated or null if it&#x27;s not activated.</value>
        [DataMember(Name="activated_at", EmitDefaultValue=false)]
        public string activated_at { get; set; }

        /// <summary>
        /// An array of BattlePassLevelChallenge objects. Please note, not all values are available from the challenge object, as battle passes handle the lifecycle of challenges.
        /// </summary>
        /// <value>An array of BattlePassLevelChallenge objects. Please note, not all values are available from the challenge object, as battle passes handle the lifecycle of challenges.</value>
        [DataMember(Name="challenges", EmitDefaultValue=false)]
        public List<BattlePassLevelChallenge> challenges { get; set; }

        /// <summary>
        /// Returns the string presentation of the object
        /// </summary>
        /// <returns>String presentation of the object</returns>
        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append("class BattlePassLevel {\n");
            sb.Append("  level_id: ").Append(level_id).Append("\n");
            sb.Append("  app_id: ").Append(app_id).Append("\n");
            sb.Append("  battle_pass_id: ").Append(battle_pass_id).Append("\n");
            sb.Append("  reward_amount: ").Append(reward_amount).Append("\n");
            sb.Append("  reward_type_name: ").Append(reward_type_name).Append("\n");
            sb.Append("  level_reward_type: ").Append(level_reward_type).Append("\n");
            sb.Append("  level_completed: ").Append(level_completed).Append("\n");
            sb.Append("  level_priority: ").Append(level_priority).Append("\n");
            sb.Append("  reward_claimed: ").Append(reward_claimed).Append("\n");
            sb.Append("  activated_at: ").Append(activated_at).Append("\n");
            sb.Append("  challenges: ").Append(challenges).Append("\n");
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
            return this.Equals(input as BattlePassLevel);
        }

        /// <summary>
        /// Returns true if BattlePassLevel instances are equal
        /// </summary>
        /// <param name="input">Instance of BattlePassLevel to be compared</param>
        /// <returns>Boolean</returns>
        public bool Equals(BattlePassLevel input)
        {
            if (input == null)
                return false;

            return 
                (
                    this.level_id == input.level_id ||
                    (this.level_id != null &&
                    this.level_id.Equals(input.level_id))
                ) && 
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
                    this.reward_amount == input.reward_amount ||
                    (this.reward_amount != null &&
                    this.reward_amount.Equals(input.reward_amount))
                ) && 
                (
                    this.reward_type_name == input.reward_type_name ||
                    (this.reward_type_name != null &&
                    this.reward_type_name.Equals(input.reward_type_name))
                ) && 
                (
                    this.level_reward_type == input.level_reward_type ||
                    (this.level_reward_type != null &&
                    this.level_reward_type.Equals(input.level_reward_type))
                ) && 
                (
                    this.level_completed == input.level_completed ||
                    (this.level_completed != null &&
                    this.level_completed.Equals(input.level_completed))
                ) && 
                (
                    this.level_priority == input.level_priority ||
                    (this.level_priority != null &&
                    this.level_priority.Equals(input.level_priority))
                ) && 
                (
                    this.reward_claimed == input.reward_claimed ||
                    (this.reward_claimed != null &&
                    this.reward_claimed.Equals(input.reward_claimed))
                ) && 
                (
                    this.activated_at == input.activated_at ||
                    (this.activated_at != null &&
                    this.activated_at.Equals(input.activated_at))
                ) && 
                (
                    this.challenges == input.challenges ||
                    this.challenges != null &&
                    input.challenges != null &&
                    this.challenges.SequenceEqual(input.challenges)
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
                if (this.level_id != null)
                    hashCode = hashCode * 59 + this.level_id.GetHashCode();
                if (this.app_id != null)
                    hashCode = hashCode * 59 + this.app_id.GetHashCode();
                if (this.battle_pass_id != null)
                    hashCode = hashCode * 59 + this.battle_pass_id.GetHashCode();
                if (this.reward_amount != null)
                    hashCode = hashCode * 59 + this.reward_amount.GetHashCode();
                if (this.reward_type_name != null)
                    hashCode = hashCode * 59 + this.reward_type_name.GetHashCode();
                if (this.level_reward_type != null)
                    hashCode = hashCode * 59 + this.level_reward_type.GetHashCode();
                if (this.level_completed != null)
                    hashCode = hashCode * 59 + this.level_completed.GetHashCode();
                if (this.level_priority != null)
                    hashCode = hashCode * 59 + this.level_priority.GetHashCode();
                if (this.reward_claimed != null)
                    hashCode = hashCode * 59 + this.reward_claimed.GetHashCode();
                if (this.activated_at != null)
                    hashCode = hashCode * 59 + this.activated_at.GetHashCode();
                if (this.challenges != null)
                    hashCode = hashCode * 59 + this.challenges.GetHashCode();
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