using ARSoft.Tools.Net.Dns;
using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Threading.Tasks;
using ElasticMail.Enums;

namespace ElasticMail.Utilities
{
    /// <summary>
    /// DNS related utilities.
    /// </summary>
    public class Dns
    {
        /// <summary>
        /// Prevents any damage to MxRecords due to multithreading issues.
        /// </summary>
        private static object _dnsSync = new object();

        /// <summary>
        /// Domains to be resolved.
        /// </summary>
        public static Dictionary<string, ResolvingState> Domains { get; set; } = new Dictionary<string, ResolvingState>();

        /// <summary>
        /// Result Mx Records
        /// </summary>
        public static List<MxRecord> MxRecords { get; set; } = new List<MxRecord>();

        /// <summary>
        /// A function that resolves the domains with a specific dns server.
        /// </summary>
        /// <param name="domain">domain to be resolved</param>
        /// <param name="server">required dns resolver</param>
        public async Task<List<MxRecord>> Resolve(string domain, string server)
        {
            try
            {
                Domains[domain] = ResolvingState.IN_PROGRESS;

                List<MxRecord> records;

                if (server != null)
                    records = new DnsStubResolver(new List<IPAddress>() { IPAddress.Parse(server) } ).Resolve<MxRecord>(domain, RecordType.Mx);
                else
                    records = new DnsStubResolver().Resolve<MxRecord>(domain, RecordType.Mx);

                records.ForEach(record => 
                    Console.WriteLine
                    (
                        "Domain: [" + record.Name + "]:. " +
                        "MX preference: [" + record.Preference.ToString() + "]:. " + 
                        "Mail Exchanger: [" + record.ExchangeDomainName?.ToString() + "]:. " + 
                        "DNS: [" + (server != null ? server : "unspecified") + "]:."
                    ));

                lock (_dnsSync)
                    Dns.MxRecords.AddRange(records);

                Domains[domain] = ResolvingState.FINISHED;

                return records;
            }
            catch (Exception ex)
            {
                Domains[domain] = ResolvingState.ERROR;

                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Resolving for " + domain + " domain failed as " + ex.Message);
                Console.ResetColor();

                return null; 
            }
        }
    }
}
