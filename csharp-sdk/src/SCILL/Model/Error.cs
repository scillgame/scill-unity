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
    /// Errors are reported using this structure. Error messages are human readable and will be translated (if available) in the language provided by the language query parameter.
    /// </summary>
    [DataContract]
        public partial class Error :  IEquatable<Error>, IValidatableObject
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Error" /> class.
        /// </summary>
        /// <param name="code">code.</param>
        /// <param name="message">message (required).</param>
        public Error(string code = default(string), string message = default(string))
        {
            // to ensure "message" is required (not null)
            if (message == null)
            {
                throw new InvalidDataException("message is a required property for Error and cannot be null");
            }
            else
            {
                this.message = message;
            }
            this.code = code;
        }
        
        /// <summary>
        /// Gets or Sets code
        /// </summary>
        [DataMember(Name="code", EmitDefaultValue=false)]
        public string code { get; set; }

        /// <summary>
        /// Gets or Sets message
        /// </summary>
        [DataMember(Name="message", EmitDefaultValue=false)]
        public string message { get; set; }

        /// <summary>
        /// Returns the string presentation of the object
        /// </summary>
        /// <returns>String presentation of the object</returns>
        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append("class Error {\n");
            sb.Append("  code: ").Append(code).Append("\n");
            sb.Append("  message: ").Append(message).Append("\n");
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
            return this.Equals(input as Error);
        }

        /// <summary>
        /// Returns true if Error instances are equal
        /// </summary>
        /// <param name="input">Instance of Error to be compared</param>
        /// <returns>Boolean</returns>
        public bool Equals(Error input)
        {
            if (input == null)
                return false;

            return 
                (
                    this.code == input.code ||
                    (this.code != null &&
                    this.code.Equals(input.code))
                ) && 
                (
                    this.message == input.message ||
                    (this.message != null &&
                    this.message.Equals(input.message))
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
                if (this.code != null)
                    hashCode = hashCode * 59 + this.code.GetHashCode();
                if (this.message != null)
                    hashCode = hashCode * 59 + this.message.GetHashCode();
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