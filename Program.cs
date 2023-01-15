using Newtonsoft.Json.Linq;
using OpenCodeDev.Cloudflare.Dyndns;
using OpenCodeDev.Cloudflare.Dyndns.Core;
using System;
using System.Net;
using System.Reflection;
using System.Runtime.CompilerServices;

internal class Program
{

    public static string ToMD5(string str)
    {
        using (System.Security.Cryptography.MD5 md5 = System.Security.Cryptography.MD5.Create())
        {
            byte[] inputBytes = System.Text.Encoding.ASCII.GetBytes(str);
            byte[] hashBytes = md5.ComputeHash(inputBytes);
            return Convert.ToHexString(hashBytes);
        }
    }

    private static void Main(string[] args)
    {
        DnsUpdater dns = new DnsUpdater();
        try
        {

            var v = typeof(Program).Assembly.GetName().Version;
            if (v == null) throw new Exception("Cannot get version ??");
            JObject versionFile = new JObject();
            versionFile["Version"] = v.ToString(3);
            versionFile["Checksum"] = ToMD5(v.ToString(3));

            File.WriteAllText("./version.json", versionFile.ToString());

            dns = new DnsUpdater(args);
            do
            {
                var prov = dns.Tasks.Dequeue();
                prov.Start();
                // Wait 1s for each domain
                Task.Delay(1000).GetAwaiter().GetResult();
            } while (dns.Tasks.Count > 0);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
        finally {
            if (typeof(DnsUpdater).GetEnvironmentBuild().Equals("Debug", StringComparison.OrdinalIgnoreCase))
            {
                Console.WriteLine("Press any key to continue;");
                Console.ReadKey();
            }
        }

    }
}