using OpenCodeDev.Cloudflare.Dyndns;
using OpenCodeDev.Cloudflare.Dyndns.Core;
using System;
using System.Net;
using System.Reflection;

internal class Program
{




    private static void Main(string[] args)
    {
        DnsUpdater dns = new DnsUpdater();
        try
        {
            dns = new DnsUpdater(args);
            dns.UpdateNow().GetAwaiter().GetResult();
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
        finally {
            if (dns.WaitToExit || typeof(DnsUpdater).GetEnvironmentBuild().Equals("Debug", StringComparison.OrdinalIgnoreCase))
            {
                Console.WriteLine("Press any key to continue;");
                Console.ReadKey();
            }
        }

    }
}