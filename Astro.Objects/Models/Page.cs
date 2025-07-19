using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Astro.Models
{
    public enum PageStatus
    {
        Staging, Approved, Reject, Resubmitted
    }
    public class Page
    {
        public string Title { get; set; } = string.Empty;
        public string ImageURL { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public DateTime PublishDate { get; set; }
        public DateTime? PublishUntil { get; set;} = DateTime.MaxValue;
        public PageStatus Status { get; set; } = PageStatus.Staging;
        public Dictionary<string, string> MetaData { get; set; }
    }
}
