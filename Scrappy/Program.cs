using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using HtmlAgilityPack;
using FileHelpers;

namespace Scrappy
{
    [DelimitedRecord(",")]
    [IgnoreEmptyLines()]
    [IgnoreFirst()]
    class Country
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Latitude { get; set; }
        public string Longitude { get; set; }
    }

    class Program
    {
        static void Main(string[] args)
        {
            WebClient client = new WebClient();
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            string baseHtml = "";

            byte[] pageContent = client.DownloadData("https://developers.google.com/public-data/docs/canonical/countries_csv");

            UTF8Encoding utf = new UTF8Encoding();
            baseHtml = utf.GetString(pageContent);

            HtmlDocument htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(baseHtml);

            var allCountryList = new List<Country>();

            var trs = htmlDoc.DocumentNode.SelectNodes("//table//tr").Skip(1);

            foreach (var tr in trs)
            {
                var tds = tr.SelectNodes("td");
                allCountryList.Add(new Country() { Id = tds[0].InnerText, Name = tds[3].InnerText, Latitude = tds[1].InnerText, Longitude = tds[2].InnerText });
            }

            var selectedCountryIds = new List<String>() { "RO", "AT", "BE", "BG", "CH", "CY", "DK", "EE", "FR", "FI", "DE", "GR", "IE", "IT", "LU", "LV", "LT", "MT", "NL", "PL", "PT", "GB", "CZ", "SK", "SI", "ES", "SE", "HU", "HR", "RU", "BY", "RS", "MK", "ME", "MD", "NO", "TR", "UA", "AFR", "NAM", "SAM", "ASI", "OCE" };
            var selectedCountries = allCountryList.Where(c => selectedCountryIds.Contains(c.Id)).ToList();
            selectedCountries.Sort((c1, c2) => c1.Id.CompareTo(c2.Id));
            selectedCountries.AddRange(new List<Country>() {
                new Country() { Id = "AFR", Name = "Africa", Latitude = "15.454166", Longitude = "18.732207" },
                new Country() { Id = "NAM", Name = "America", Latitude = "37.09024", Longitude = "-95.712891" },
                new Country() { Id = "SAM", Name = "South America", Latitude = "-14.235004", Longitude = "-51.92528" },
                new Country() { Id = "ASI", Name = "Asia", Latitude = "35.86166", Longitude = "104.195397" },
                new Country() { Id = "OCE", Name = "Oceania", Latitude = "-0.789275", Longitude = "113.921327" }
            });
            //Write to CSV
            var engine = new FileHelperEngine<Country>
            {
                HeaderText = "Id,Label,Lat,Lon"
            };
            engine.WriteFile("..//..//..//db//Nodes.csv", selectedCountries);
        }
    }
}
