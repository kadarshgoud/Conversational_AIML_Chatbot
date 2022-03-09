using System;
using System.Xml;
using System.Text;
using System.Net;
using System.Text.RegularExpressions;
using System.IO;
using Newtonsoft.Json;
using AIMLBot.Misc;
using System.Linq;

namespace AIMLBot.AIMLTagHandlers
{
    /// <summary>
    /// The websearch element search data from wiki    
    ///     
    /// for now its only search data from wiki 
    /// </summary>
    public class wikisearch : Utils.AIMLTagHandler
    {
        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="bot">The bot involved in this request</param>
        /// <param name="user">The user making the request</param>
        /// <param name="query">The query that originated this node</param>
        /// <param name="request">The request inputted into the system</param>
        /// <param name="result">The result to be passed to the user</param>
        /// <param name="templateNode">The node to be processed</param>
        public wikisearch(Bot bot,
                        AIMLBot.User user,
                        Utils.SubQuery query,
                        AIMLBot.Request request,
                        AIMLBot.Result result,
                        XmlNode templateNode)
            : base(bot, user, query, request, result, templateNode)
        {
        }

        protected override string ProcessChange()
        {
            if (this.templateNode.Name.ToLower() == "wikisearch")
            {
                return GetSearchResult(this.templateNode.InnerText);
            }
            return string.Empty;
        }


        // "https://en.wikipedia.org/w/api.php?action=opensearch&search=" + wikiquery + "&format=xml"
        // getting content:
        //  http://en.wikipedia.org/w/api.php?format=xml&action=query&prop=revisions&titles=Stack%20Overflow&rvprop=content&rvsection=0&rvparse


        private string GetSearchResult(string wikiquery)
        {
            // Converting first letter of the text to captials 

            StringBuilder formaloutput = new StringBuilder();
            if (wikiquery.Length > 0)
            {
                string[] words = wikiquery.ToLower().Split();
                foreach (string word in words)
                {
                    string newWord = word.Substring(0, 1);
                    newWord = newWord.ToUpper();
                    if (word.Length > 1)
                    {
                        newWord += word.Substring(1);
                    }
                    formaloutput.Append(newWord + " ");
                }
            }

            string inputtowiki = formaloutput.ToString();
            // wiki search logic 

            WebClient client = new WebClient();
            var response = client.DownloadString("https://en.wikipedia.org/w/api.php?action=query&prop=extracts&exsentences=2&exlimit=1&explaintext=1&format=json&exintro=1&titles=" + inputtowiki);

            var responseJson = JsonConvert.DeserializeObject<RootObject>(response);
            var firstKey = responseJson.query.Pages.First().Key;
            var extract = responseJson.query.Pages[firstKey].extract;
            string result = extract;
            return result;

        }
    }
}
