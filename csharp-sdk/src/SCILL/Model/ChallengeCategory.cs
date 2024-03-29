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
    /// Challenges are grouped into categories. You can exclude or include categories in queries. Per default all categories will be returned.
    /// </summary>
    [DataContract]
        public partial class ChallengeCategory :  IEquatable<ChallengeCategory>, IValidatableObject
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ChallengeCategory" /> class.
        /// </summary>
        /// <param name="isDailyCategory">Indicates if this is the daily category, bringing up new challenges every day for the user to tackle..</param>
        /// <param name="categoryPosition">In the admin panel you set the order of the categories. This is the position index and indicates the position within the categories array..</param>
        /// <param name="categorySlug">A short name without special chars to make it easier to refer to a specific category (in code) that is language and id agnostic..</param>
        /// <param name="categoryName">The name of the category in the local language set as the query parameter..</param>
        /// <param name="categoryId">Indicates how many “tasks” must be completed or done to complete this challenge..</param>
        /// <param name="challenges">An array of Challenge objects..</param>
        public ChallengeCategory(bool? isDailyCategory = default(bool?), int? categoryPosition = default(int?), string categorySlug = default(string), string categoryName = default(string), string categoryId = default(string), List<Challenge> challenges = default(List<Challenge>))
        {
            this.is_daily_category = isDailyCategory;
            this.category_position = categoryPosition;
            this.category_slug = categorySlug;
            this.category_name = categoryName;
            this.category_id = categoryId;
            this.challenges = challenges;
        }
        
        /// <summary>
        /// Indicates if this is the daily category, bringing up new challenges every day for the user to tackle.
        /// </summary>
        /// <value>Indicates if this is the daily category, bringing up new challenges every day for the user to tackle.</value>
        [DataMember(Name="is_daily_category", EmitDefaultValue=false)]
        public bool? is_daily_category { get; set; }

        /// <summary>
        /// In the admin panel you set the order of the categories. This is the position index and indicates the position within the categories array.
        /// </summary>
        /// <value>In the admin panel you set the order of the categories. This is the position index and indicates the position within the categories array.</value>
        [DataMember(Name="category_position", EmitDefaultValue=false)]
        public int? category_position { get; set; }

        /// <summary>
        /// A short name without special chars to make it easier to refer to a specific category (in code) that is language and id agnostic.
        /// </summary>
        /// <value>A short name without special chars to make it easier to refer to a specific category (in code) that is language and id agnostic.</value>
        [DataMember(Name="category_slug", EmitDefaultValue=false)]
        public string category_slug { get; set; }

        /// <summary>
        /// The name of the category in the local language set as the query parameter.
        /// </summary>
        /// <value>The name of the category in the local language set as the query parameter.</value>
        [DataMember(Name="category_name", EmitDefaultValue=false)]
        public string category_name { get; set; }

        /// <summary>
        /// Indicates how many “tasks” must be completed or done to complete this challenge.
        /// </summary>
        /// <value>Indicates how many “tasks” must be completed or done to complete this challenge.</value>
        [DataMember(Name="category_id", EmitDefaultValue=false)]
        public string category_id { get; set; }

        /// <summary>
        /// An array of Challenge objects.
        /// </summary>
        /// <value>An array of Challenge objects.</value>
        [DataMember(Name="challenges", EmitDefaultValue=false)]
        public List<Challenge> challenges { get; set; }

        /// <summary>
        /// Returns the string presentation of the object
        /// </summary>
        /// <returns>String presentation of the object</returns>
        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append("class ChallengeCategory {\n");
            sb.Append("  is_daily_category: ").Append(is_daily_category).Append("\n");
            sb.Append("  category_position: ").Append(category_position).Append("\n");
            sb.Append("  category_slug: ").Append(category_slug).Append("\n");
            sb.Append("  category_name: ").Append(category_name).Append("\n");
            sb.Append("  category_id: ").Append(category_id).Append("\n");
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
            return this.Equals(input as ChallengeCategory);
        }

        /// <summary>
        /// Returns true if ChallengeCategory instances are equal
        /// </summary>
        /// <param name="input">Instance of ChallengeCategory to be compared</param>
        /// <returns>Boolean</returns>
        public bool Equals(ChallengeCategory input)
        {
            if (input == null)
                return false;

            return 
                (
                    this.is_daily_category == input.is_daily_category ||
                    (this.is_daily_category != null &&
                    this.is_daily_category.Equals(input.is_daily_category))
                ) && 
                (
                    this.category_position == input.category_position ||
                    (this.category_position != null &&
                    this.category_position.Equals(input.category_position))
                ) && 
                (
                    this.category_slug == input.category_slug ||
                    (this.category_slug != null &&
                    this.category_slug.Equals(input.category_slug))
                ) && 
                (
                    this.category_name == input.category_name ||
                    (this.category_name != null &&
                    this.category_name.Equals(input.category_name))
                ) && 
                (
                    this.category_id == input.category_id ||
                    (this.category_id != null &&
                    this.category_id.Equals(input.category_id))
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
                if (this.is_daily_category != null)
                    hashCode = hashCode * 59 + this.is_daily_category.GetHashCode();
                if (this.category_position != null)
                    hashCode = hashCode * 59 + this.category_position.GetHashCode();
                if (this.category_slug != null)
                    hashCode = hashCode * 59 + this.category_slug.GetHashCode();
                if (this.category_name != null)
                    hashCode = hashCode * 59 + this.category_name.GetHashCode();
                if (this.category_id != null)
                    hashCode = hashCode * 59 + this.category_id.GetHashCode();
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
