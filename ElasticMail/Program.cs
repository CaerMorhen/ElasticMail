using ARSoft.Tools.Net.Dns;
using ElasticMail.Utilities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using System.Threading;
using ElasticMail.Enums;

namespace ElasticMail
{
    /// <summary>
    /// A console application that resolves domains.
    /// </summary>
    public class Program
    {
        /// <summary>
        /// Csv utility to retrieve and save inputs or reports.
        /// </summary>
        private static Csv _csv = new Csv();

        /// <summary>
        /// Dns resolver.
        /// </summary>
        private static Dns _dns = new Dns();

        /// <summary>
        /// Available utilities' commands
        /// </summary>
        public static Dictionary<string, string> Commands { get; set; } = new Dictionary<string, string>()
        {
            { "InputDirectory", null },
            { "OutputDirectory", null },
            { "DnsServer", null },
        };

        /// <summary>
        /// Main method.
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {
            #region No args handler
            if (args.Length == 0)
            {
                Console.WriteLine("No parameters were provided...");

                return;
            }
            #endregion

            #region Parameters handling
            for (int i = 0; i < args.Length; i++)
            {
                switch (args[i].ToLower())
                {
                    case "-input":
                        i++;
                        Commands["InputDirectory"] = args[i];
                        break;

                    case "-dns":
                        i++;
                        Commands["DnsServer"] = args[i];
                        break;

                    case "-output":
                        i++;
                        Commands["OutputDirectory"] = args[i];
                        break;

                    default:
                        Dns.Domains.Add(args[i], ResolvingState.READY);
                        break;
                }
            }
            #endregion

            #region Input domains retrieval
            if (Commands["InputDirectory"] != null)
                foreach (var item in _csv.Retrieve(Commands["InputDirectory"]))
                    Dns.Domains.TryAdd(item, ResolvingState.READY);
            #endregion

            #region Resolving domains
            if (Dns.Domains.Count == 0)
            {
                Console.WriteLine("No domains to resolve...");

                return;
            }

            foreach (string domain in Dns.Domains.Keys)
                Task.Run(() => _dns.Resolve(domain, Commands["DnsServer"]).Result);
            #endregion

            #region Reporting
            /// Before reporting all the dns requests have to be already finished...
            while (Dns.Domains.ContainsValue(ResolvingState.IN_PROGRESS) || Dns.Domains.ContainsValue(ResolvingState.READY))
                Thread.Sleep(50);

            if (Commands["OutputDirectory"] != null)
            {
                List<OutputMapping> outputMappings = new List<OutputMapping>();

                Dns.MxRecords.ForEach
                (
                    mxRecord => outputMappings.Add
                        (
                            mxRecord != null ?
                            new OutputMapping
                            (
                                mxRecord.Name.ToString(), mxRecord.Preference.ToString(), mxRecord.ExchangeDomainName.ToString(), Commands["DnsServer"]
                            ) : null
                        )
                );

                _csv.Report(Commands["OutputDirectory"], outputMappings);
            }
            #endregion

            Console.ReadKey();
        }
    }
}
