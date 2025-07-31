using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Astro.Models
{
    /// <summary>
    /// Represents the type of input or format expected for a question.
    /// </summary>
    public enum QuestionType
    {
        /// <summary>Single-line text input</summary>
        TextBox,
        /// <summary>Multi-line text input</summary>
        TextArea,
        /// <summary>Single option from a set (radio buttons)</summary>
        SingleChoice,
        /// <summary>Multiple options from a set (checkboxes)</summary>
        MultipleChoice,
        /// <summary>Numeric input</summary>
        Number,
        /// <summary>Boolean choice (e.g., Yes/No)</summary>
        Boolean,
        /// <summary>Date picker input</summary>
        Date,
        /// <summary>Range of two dates (e.g., start and end date)</summary>
        DateRange,
        /// <summary>Time picker input</summary>
        Time,
        /// <summary>Rating input (e.g., 1 to 5 stars)</summary>
        Rating,
        /// <summary>Dropdown selection input</summary>
        Dropdown,
        /// <summary>File upload input</summary>
        FileUpload,
        /// <summary>Section title or instructional text (not a question)</summary>
        Content,
        /// <summary>Email input (with validation)</summary>
        Email,
        /// <summary>Phone number input (with validation)</summary>
        PhoneNumber,
        /// <summary>URL input (with validation)</summary>
        Url
    }
    public class Question
    {
        [JsonPropertyName("questionId")]
        public Guid QuestionId { get; set; } = Guid.NewGuid();
        [JsonPropertyName("category")]
        public string Category { get; set; } = string.Empty;
        [JsonPropertyName("type")]
        public string Type { get; set; } = "multiple_choice";
        [JsonPropertyName("questionText")]
        public string QuestionText { get; set; } = string.Empty;
        [JsonPropertyName("description")]
        public string Description { get; set; } = string.Empty; 
        [JsonPropertyName("options")]
        public List<string> Options { get; set; } = new();
        [JsonPropertyName("isRequired")]
        public bool IsRequired { get; set; } = true;
        [JsonPropertyName("score")]
        public double? Score { get; set; } = null;
        [JsonPropertyName("correctAnswers")]
        public List<string> CorrectAnswers { get; set; } = new();
    }
}
