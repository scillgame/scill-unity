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
    /// BattlePassUnlockInfo
    /// </summary>
    [DataContract]
        public partial class BattlePassUnlockInfo :  IEquatable<BattlePassUnlockInfo>, IValidatableObject
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BattlePassUnlockInfo" /> class.
        /// </summary>
        /// <param name="purchaseId">The id of this battle pass purchase.</param>
        /// <param name="battlePassId">The battle pass id.</param>
        /// <param name="userId">The user id of this battle pass purchase.</param>
        /// <param name="purchasePrice">The price paid for this battle pass.</param>
        /// <param name="purchaseCurrency">The currency used to purchase this battle pass.</param>
        /// <param name="purchasedAt">The date this battle pass has been purchased.</param>
        /// <param name="battlePassCompleted">Indicates if this battle pass has been completed.</param>
        public BattlePassUnlockInfo(string purchaseId = default(string), string battlePassId = default(string), string userId = default(string), decimal? purchasePrice = default(decimal?), string purchaseCurrency = default(string), string purchasedAt = default(string), bool? battlePassCompleted = default(bool?))
        {
            this.purchase_id = purchaseId;
            this.battle_pass_id = battlePassId;
            this.user_id = userId;
            this.purchase_price = purchasePrice;
            this.purchase_currency = purchaseCurrency;
            this.purchased_at = purchasedAt;
            this.battle_pass_completed = battlePassCompleted;
        }
        
        /// <summary>
        /// The id of this battle pass purchase
        /// </summary>
        /// <value>The id of this battle pass purchase</value>
        [DataMember(Name="purchase_id", EmitDefaultValue=false)]
        public string purchase_id { get; set; }

        /// <summary>
        /// The battle pass id
        /// </summary>
        /// <value>The battle pass id</value>
        [DataMember(Name="battle_pass_id", EmitDefaultValue=false)]
        public string battle_pass_id { get; set; }

        /// <summary>
        /// The user id of this battle pass purchase
        /// </summary>
        /// <value>The user id of this battle pass purchase</value>
        [DataMember(Name="user_id", EmitDefaultValue=false)]
        public string user_id { get; set; }

        /// <summary>
        /// The price paid for this battle pass
        /// </summary>
        /// <value>The price paid for this battle pass</value>
        [DataMember(Name="purchase_price", EmitDefaultValue=false)]
        public decimal? purchase_price { get; set; }

        /// <summary>
        /// The currency used to purchase this battle pass
        /// </summary>
        /// <value>The currency used to purchase this battle pass</value>
        [DataMember(Name="purchase_currency", EmitDefaultValue=false)]
        public string purchase_currency { get; set; }

        /// <summary>
        /// The date this battle pass has been purchased
        /// </summary>
        /// <value>The date this battle pass has been purchased</value>
        [DataMember(Name="purchased_at", EmitDefaultValue=false)]
        public string purchased_at { get; set; }

        /// <summary>
        /// Indicates if this battle pass has been completed
        /// </summary>
        /// <value>Indicates if this battle pass has been completed</value>
        [DataMember(Name="battle_pass_completed", EmitDefaultValue=false)]
        public bool? battle_pass_completed { get; set; }

        /// <summary>
        /// Returns the string presentation of the object
        /// </summary>
        /// <returns>String presentation of the object</returns>
        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append("class BattlePassUnlockInfo {\n");
            sb.Append("  purchase_id: ").Append(purchase_id).Append("\n");
            sb.Append("  battle_pass_id: ").Append(battle_pass_id).Append("\n");
            sb.Append("  user_id: ").Append(user_id).Append("\n");
            sb.Append("  purchase_price: ").Append(purchase_price).Append("\n");
            sb.Append("  purchase_currency: ").Append(purchase_currency).Append("\n");
            sb.Append("  purchased_at: ").Append(purchased_at).Append("\n");
            sb.Append("  battle_pass_completed: ").Append(battle_pass_completed).Append("\n");
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
            return this.Equals(input as BattlePassUnlockInfo);
        }

        /// <summary>
        /// Returns true if BattlePassUnlockInfo instances are equal
        /// </summary>
        /// <param name="input">Instance of BattlePassUnlockInfo to be compared</param>
        /// <returns>Boolean</returns>
        public bool Equals(BattlePassUnlockInfo input)
        {
            if (input == null)
                return false;

            return 
                (
                    this.purchase_id == input.purchase_id ||
                    (this.purchase_id != null &&
                    this.purchase_id.Equals(input.purchase_id))
                ) && 
                (
                    this.battle_pass_id == input.battle_pass_id ||
                    (this.battle_pass_id != null &&
                    this.battle_pass_id.Equals(input.battle_pass_id))
                ) && 
                (
                    this.user_id == input.user_id ||
                    (this.user_id != null &&
                    this.user_id.Equals(input.user_id))
                ) && 
                (
                    this.purchase_price == input.purchase_price ||
                    (this.purchase_price != null &&
                    this.purchase_price.Equals(input.purchase_price))
                ) && 
                (
                    this.purchase_currency == input.purchase_currency ||
                    (this.purchase_currency != null &&
                    this.purchase_currency.Equals(input.purchase_currency))
                ) && 
                (
                    this.purchased_at == input.purchased_at ||
                    (this.purchased_at != null &&
                    this.purchased_at.Equals(input.purchased_at))
                ) && 
                (
                    this.battle_pass_completed == input.battle_pass_completed ||
                    (this.battle_pass_completed != null &&
                    this.battle_pass_completed.Equals(input.battle_pass_completed))
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
                if (this.purchase_id != null)
                    hashCode = hashCode * 59 + this.purchase_id.GetHashCode();
                if (this.battle_pass_id != null)
                    hashCode = hashCode * 59 + this.battle_pass_id.GetHashCode();
                if (this.user_id != null)
                    hashCode = hashCode * 59 + this.user_id.GetHashCode();
                if (this.purchase_price != null)
                    hashCode = hashCode * 59 + this.purchase_price.GetHashCode();
                if (this.purchase_currency != null)
                    hashCode = hashCode * 59 + this.purchase_currency.GetHashCode();
                if (this.purchased_at != null)
                    hashCode = hashCode * 59 + this.purchased_at.GetHashCode();
                if (this.battle_pass_completed != null)
                    hashCode = hashCode * 59 + this.battle_pass_completed.GetHashCode();
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
