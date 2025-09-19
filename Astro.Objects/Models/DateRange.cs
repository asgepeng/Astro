using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Astro.Models
{
    public class DateRange
    {
        public DateRange()
        {
            StartDate = DateTime.Today.AddDays(-1 * DateTime.Today.Day);
            EndDate = DateTime.Today.AddDays(1).AddSeconds(-1);
        }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
}
