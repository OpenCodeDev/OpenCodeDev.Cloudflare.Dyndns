using CloudFlare.Client;
using CloudFlare.Client.Api.Zones;
using CloudFlare.Client.Api.Zones.DnsRecord;
using CloudFlare.Client.Client.Zones;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace OpenCodeDev.Cloudflare.Dyndns.Core
{
    internal class DnsUpdater
    {
        /// <summary>
        /// --apikey Cloudflare's Token Access
        /// </summary>
        internal string Key { get; set; } = string.Empty;

        /// <summary>
        /// --zoneid Cloudflare Zone ID.
        /// </summary>
        internal string ZoneId { get; set; } = string.Empty;

        /// <summary>
        /// --arecord Name of the record to update with new IP.
        /// </summary>
        internal string ARecordName { get; set; } = string.Empty;

        internal IPAddress PublicAddress { get; set; }
        /// <summary>
        /// --waitexit is flag to set this to true... when true script will wait console read to exit (used to test)
        /// </summary>
        internal bool WaitToExit { get; set; } = false;
        private Dictionary<string, string> FlagArguments { get; set; } = new();
        public DnsUpdater() { }
        public DnsUpdater(params string[] args)
        {
            
            string lastFlag = "";
            //eg: --manual --api SDASDFDFSDfsdfs
            foreach (var item in args)
            {
                // if last flag not empty and current item is a flag
                if (!string.IsNullOrEmpty(lastFlag) && item.StartsWith("--"))
                {
                    AddFlag(lastFlag); // add empty flag
                    lastFlag = ""; // Reset Last flag
                }
                else if (!string.IsNullOrEmpty(lastFlag) && !item.StartsWith("--"))
                {
                    AddFlag(lastFlag, item); // add value flag
                    lastFlag = ""; // Reset Last flag
                }

                // if start with -- it's a flag key and save for the following value if any
                if (item.StartsWith("--")) lastFlag = item;
            }
            try
            {
                GetPublicIp().GetAwaiter().GetResult();
                bool error = ConstructParams(out string[] messages);
                foreach (var item in messages)
                {
                    Console.WriteLine(item);
                }
                if (error) 
                    throw new ArgumentException("Cannot run DNS script.");
            }
            catch (Exception)
            {
                throw;
            }
            
        }
       
        private bool ConstructParams(out string[] messages)
        {
            List<string> events = new List<string>
            {
                "Checking Params..."
            };
            bool error = false;
            ProcessBoolFlag("--waitexit", v => WaitToExit = v, ref events);
            ProcessValueFlag("--apikey", true, v => Key = v, ref error, ref events);
            ProcessValueFlag("--arecord", true, v => ARecordName = v, ref error, ref events);
            ProcessValueFlag("--zoneid", true, v => ZoneId = v, ref error, ref events);
            messages = events.ToArray();
            return error;
        }

        private void ProcessValueFlag(string flag, bool required, Action<string> onSet,  ref bool error, ref List<string> messages)
        {

            if (FlagArguments.ContainsKey(flag))
            {
                onSet.Invoke(FlagArguments[flag]);
                messages.Add($"flag {flag} is defined");
            }
            else if(required)
            {
                error = true;
                messages.Add($"flag {flag} is not define and it is required.");
            }
        }
        private void ProcessBoolFlag(string flag, Action<bool> onValueTest, ref List<string> messages)
        {
            onValueTest.Invoke(FlagArguments.ContainsKey(flag));
            // Set message if true
            if (FlagArguments.ContainsKey(flag))
            {
                messages.Add($"optional flag {flag} is defined");
            }
        }
        private void AddFlag(string flag) => FlagArguments[flag] = string.Empty;
        private void AddFlag(string flag, string val) => FlagArguments[flag] = val;

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

        internal async Task UpdateNow()
        {
            Console.WriteLine("Starting Update...");

            using var client = new CloudFlareClient(Key);
                var zones = await client.Zones.GetAsync();

            Zone zoneFound = default;
    
            foreach (var zone in zones.Result)
            {
                
                if(zone.Id == ZoneId)
                {
                    zoneFound = zone;
                    break; // found zone move on
                }
            }
            DnsRecord record = default;
            if (zoneFound == default)
            {
                Console.WriteLine($"Zone hasn't found any zone with '{ZoneId}'. Make sure TOKEN is allowed access to the given zone.");
                return;
            }
            else
            {
                // Get Records
                var dnsRecords = await client.Zones.DnsRecords.GetAsync(zoneFound.Id);
                // For each type A record... find A Record Name
                foreach (var dnsRecord in dnsRecords.Result.Where(p => p.Type == CloudFlare.Client.Enumerators.DnsRecordType.A))
                {
                    string name = dnsRecord.Name;
                    if (name.Equals(ARecordName, StringComparison.OrdinalIgnoreCase))
                    {
                        Console.WriteLine($"{dnsRecord.Name} is found.");
                        record = dnsRecord;
                    }
                }     
            }

            if (record == default)
            {
                Console.WriteLine($"Zone '{zoneFound.Name}' was found . But it looks like '{ARecordName}' was not found.");
                return;
            }
            else
            {
                if(PublicAddress.ToString() != record.Content)
                {
                    ModifiedDnsRecord recordA = new ModifiedDnsRecord()
                    {
                        Content = PublicAddress.ToString(),
                        Name = record.Name,
                        Proxied = record.Proxied,
                        Ttl = record.Ttl,
                        Type = record.Type
                    };
                    await client.Zones.DnsRecords.UpdateAsync(ZoneId, record.Id, recordA);
                    Console.WriteLine($"{record.Name} has been updated from {record.Content} to {recordA.Content}.");
                }
                else
                {
                    Console.WriteLine($"{record.Name} is already up to date with {record.Content}.");

                }

            }


        }
    }
}
