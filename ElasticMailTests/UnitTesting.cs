using ElasticMail.Enums;
using ElasticMail.Utilities;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;

namespace ElasticMailTests
{
    /// <summary>
    /// Unit tests.
    /// </summary>
    public class Tests
    {
        /// <summary>
        /// Csv utility to retrieve and save inputs or reports.
        /// </summary>
        private Csv _csv = new Csv();

        /// <summary>
        /// Dns resolver.
        /// </summary>
        private Dns _dns = new Dns();

        /// <summary>
        /// <<<TEST>>>
        /// </summary>
        public string InputsDirectory { get; } = @"C:\ElasticMail\inputs.csv";

        /// <summary>
        /// <<<TEST>>>
        /// </summary>
        public string DefaultDnsServer { get; } = "8.8.8.8";

        /// <summary>
        /// <<<TEST>>>
        /// </summary>
        [Test]
        public void TestCsvRetrieval()
        {
            List<string> patternList = new List<string>() { "gmail.com", "hotmail.com", "yahoo.com", "aol.com" };

            List<string> testList = _csv.Retrieve(InputsDirectory);

            Assert.IsTrue(Enumerable.SequenceEqual(patternList, testList), "Csv Retrieval Test");
        }

        /// <summary>
        /// <<<TEST>>>
        /// </summary>
        [Test]
        public void TestResolve()
        {
            Assert.IsTrue
            (
                _dns.Resolve("gmail.com", DefaultDnsServer).Result
                    .Where
                    (
                        x => x.ExchangeDomainName.ToString().Equals("alt3.gmail-smtp-in.l.google.com.")
                    ).FirstOrDefault() != null, "Domain resolving Test"
            );
        }
    }
}