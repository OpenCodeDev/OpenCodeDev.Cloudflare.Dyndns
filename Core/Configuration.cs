using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenCodeDev.Cloudflare.Dyndns.Core
{
    public class Configuration
    {

        public List<ConfigurationCloudflare> Cloudflare { get; set; } = new List<ConfigurationCloudflare>();
    }
}
