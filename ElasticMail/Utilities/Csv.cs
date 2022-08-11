using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using CsvHelper;
using CsvHelper.Configuration;
using CsvHelper.Configuration.Attributes;

namespace ElasticMail.Utilities
{
    /// <summary>
    /// Csv related utilities.
    /// </summary>
    public class Csv
    {
        /// <summary>
        /// A function that retrieves a list of strings out of the csv file.
        /// </summary>
        /// <param name="inputDirectory"></param>
        /// <returns></returns>
        public List<string> Retrieve(string inputDirectory)
        {
            try
            {
                using (var fileReader = new StreamReader(inputDirectory))
                    using (var csvReader = new CsvReader(fileReader, new CsvConfiguration(CultureInfo.InvariantCulture) { Delimiter = ";", BadDataFound = null }))
                        return csvReader.GetRecords<InputMapping>().Select(x => x.Domain).ToList();
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Retrieving from csv file failed due to " + ex.Message);
                Console.ResetColor();

                return null;
            }
        }

        /// <summary>
        /// A function used to report resolved domains into a csv file.
        /// </summary>
        /// <param name="domains"></param>
        /// <param name="outputDirectory"></param>
        public void Report(string outputDirectory, List<OutputMapping> outputMappings)
        {
            try 
            { 
                using (var fileWriter = new StreamWriter(outputDirectory))
                    using (var csvWriter = new CsvWriter(fileWriter, new CsvConfiguration(CultureInfo.InvariantCulture) { Delimiter = ";", BadDataFound = null }))
                        csvWriter.WriteRecords(new List<OutputMapping>(outputMappings));
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Reporting to a csv file failed due to " + ex.Message);
                Console.ResetColor();
            }
        }
    }

    /// <summary>
    /// Mapping of input file.
    /// </summary>
    public class InputMapping
    {
        [Name("domain")]
        public string Domain { get; set; }
    }

    /// <summary>
    /// Mapping of output file.
    /// </summary>
    public class OutputMapping
    {
        [Name("domain")]
        public string Domain { get; set; }
        [Name("mx preference")]
        public string MxPreference { get; set; }
        [Name("mail exchanger")]
        public string MailExchanger { get; set; }
        [Name("dns resolver")]
        public string DnsResolver { get; set; }

        /// <summary>
        /// Constructor mapping exact mx record fields to csv columns.
        /// </summary>
        /// <param name="domain"></param>
        /// <param name="mxPreference"></param>
        /// <param name="mailExchanger"></param>
        /// <param name="dnsResolver"></param>
        public OutputMapping(string domain, string mxPreference, string mailExchanger, string dnsResolver)
        {
            Domain = domain;
            MxPreference = mxPreference;
            MailExchanger = mailExchanger;
            DnsResolver = dnsResolver != null ? dnsResolver : "unspecified";
        }
    }
}
