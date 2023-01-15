using CloudFlare.Client.Api.Zones.DnsRecord;
using CloudFlare.Client.Api.Zones;
using CloudFlare.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;

namespace OpenCodeDev.Cloudflare.Dyndns.Core.Providers
{
    public class Cloudflare : IProvider

    { 
        ConfigurationCloudflare Config { get; set; }
        IPAddress PublicIp { get; set; }
        public Cloudflare(ConfigurationCloudflare conf, IPAddress pubIp)
        {
            Config = conf;
            PublicIp = pubIp;
        }
        public void Start()
        {
            UpdateNow().GetAwaiter().GetResult();
        }

        internal async Task UpdateNow()
        {
            Console.WriteLine("Starting Update...");

            using var client = new CloudFlareClient(Config.Key);
            var zones = await client.Zones.GetAsync();

            Zone zoneFound = default;

            foreach (var zone in zones.Result)
            {

                if (zone.Id == Config.ZoneId)
                {
                    zoneFound = zone;
                    break; // found zone move on
                }
            }
            DnsRecord record = default;
            if (zoneFound == default)
            {
                Console.WriteLine($"Zone hasn't found any zone with '{Config.ZoneId}'. Make sure TOKEN is allowed access to the given zone.");
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
                    if (name.Equals(Config.Record, StringComparison.OrdinalIgnoreCase))
                    {
                        Console.WriteLine($"{dnsRecord.Name} is found.");
                        record = dnsRecord;
                    }
                }
            }

            if (record == default)
            {
                Console.WriteLine($"Zone '{zoneFound.Name}' was found . But it looks like '{Config.Record}' was not found.");
                return;
            }
            else
            {
                if (PublicIp.ToString() != record.Content)
                {
                    ModifiedDnsRecord recordA = new ModifiedDnsRecord()
                    {
                        Content = PublicIp.ToString(),
                        Name = record.Name,
                        Proxied = record.Proxied,
                        Ttl = record.Ttl,
                        Type = record.Type
                    };
                    await client.Zones.DnsRecords.UpdateAsync(Config.ZoneId, record.Id, recordA);
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
