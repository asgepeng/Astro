using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Astro.Models
{
    /// <summary>
    /// Represents the current approval status of a content page.
    /// </summary>
    public enum PageStatus
    {
        /// <summary>Content is being drafted or under review.</summary>
        Staging,
        /// <summary>Content has been approved and is published.</summary>
        Approved,
        /// <summary>Content was reviewed and rejected.</summary>
        Reject,
        /// <summary>Previously rejected content has been updated and resubmitted.</summary>
        Resubmitted
    }

    public class Page
    {
        /// <summary>Unique identifier for the page.</summary>
        public Guid PageId { get; set; } = Guid.NewGuid();
        /// <summary>Page title or headline.</summary>
        public string Title { get; set; } = string.Empty;
        /// <summary>URL of the banner or preview image.</summary>
        public string ImageURL { get; set; } = string.Empty;
        /// <summary>Main HTML or Markdown content of the page.</summary>
        public string Content { get; set; } = string.Empty;
        /// <summary>Date and time when the page should be published.</summary>
        public DateTime PublishDate { get; set; } = DateTime.UtcNow;
        /// <summary>Date and time when the page should no longer be available.</summary>
        public DateTime? PublishUntil { get; set; } = DateTime.MaxValue;
        /// <summary>Current moderation status of the page.</summary>
        public PageStatus Status { get; set; } = PageStatus.Staging;
        /// <summary>Optional slug for SEO-friendly URL routing.</summary>
        public string Slug { get; set; } = string.Empty;
        /// <summary>Author or creator of the page.</summary>
        public string Author { get; set; } = string.Empty;
        /// <summary>List of tags or keywords related to the page.</summary>
        public List<string> Tags { get; set; } = new();
        /// <summary>Custom metadata key-value pairs (e.g., meta title, description).</summary>
        public Dictionary<string, string> MetaData { get; set; } = new();
    }

}
