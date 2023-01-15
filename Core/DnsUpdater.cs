using CloudFlare.Client;
using CloudFlare.Client.Api.Zones;
using CloudFlare.Client.Api.Zones.DnsRecord;
using CloudFlare.Client.Client.Zones;
using Newtonsoft.Json.Linq;
using OpenCodeDev.Cloudflare.Dyndns.Core.Providers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection.Metadata;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace OpenCodeDev.Cloudflare.Dyndns.Core
{
    internal class DnsUpdater
    {

        public Queue<IProvider> Tasks { get; set; } = new Queue<IProvider>();
        internal IPAddress PublicAddress { get; set; }
        public Configuration? ConfigFile { get; set; }
        public DnsUpdater() { }
        public DnsUpdater(params string[] args)
        {
            //No Args? Config


            try
            {
                Console.WriteLine("Reading Config...");
                if (File.Exists("./config.json"))
                {
                    string json = File.ReadAllText("./config.json");
                    ConfigFile = JObject.Parse(json).ToObject<Configuration>();
                }
                if (ConfigFile == null) throw new Exception("Config cannot be null maybe json is invalid?");
                Console.WriteLine("Fetching Public Ip...");

                GetPublicIp().GetAwaiter().GetResult();
                if (PublicAddress == null) throw new Exception("Couldn't fetch ip address.");
                Console.WriteLine("Queueing Tasks...");

                TakeCloudflare();
            }
            catch (Exception)
            {
                throw;
            }
            
        }
       
        internal void TakeCloudflare()
        {
            foreach (var item in ConfigFile.Cloudflare)
            {
                Tasks.Enqueue(new Providers.Cloudflare(item, PublicAddress));
            }
        }
        
        internal async Task GetPublicIp()
        {
            string url = "https://api.ipify.org/";
            using (var httpClient = new HttpClient())
            {
                var request = new HttpRequestMessage(HttpMethod.Get, url);
                var response = await httpClient.SendAsync(request).ConfigureAwait(false);
                string ip = await response.Content.ReadAsStringAsync();
                PublicAddress = IPAddress.Parse(ip);
            }
            
        }

  
    }
}
