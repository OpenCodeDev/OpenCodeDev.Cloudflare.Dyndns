using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenCodeDev.Cloudflare.Dyndns.Core
{
    public class ConfigurationCloudflare
    {
        public string Key { get; set; }
        public string Record { get; set; }
        public string ZoneId { get; set; }
    }
}
