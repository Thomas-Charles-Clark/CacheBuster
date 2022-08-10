using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CacheBuster.Models
{
    public class PassBust
    {
        // 0 = Localhost
        // 1 = Dev
        // 2 = Preview
        // 3 = Production
        public byte target { get; set; }
        public List<BustItem> items { get; set; }
    }

    public class BustItem
    {
        public byte? templateId { get; set; }
        public DateTimeOffset? explicitExpiry { get; set; }
        public DateTime contentDate { get; set; }
        public bool? tradeSwitch { get; set; }
        public string contentType { get; set; }
        public long contentId { get; set; }
        public short countryId { get; set; }
        public byte languageId { get; set; }
    }
}