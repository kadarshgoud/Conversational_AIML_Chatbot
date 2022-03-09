using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace ChatBotDemo.Helpers
{
    public class WikiSearchTextOutput
    {
        public static string SendingRecieveingRequestsfromWiki(string keyword)
        {
            WebClient client = new WebClient();
            var response = client.DownloadString("https://en.wikipedia.org/w/api.php?action=query&prop=extracts&exsentences=2&exlimit=1&explaintext=1&format=json&exintro=1&titles=" + keyword + "&redirects=true");

            var responseJson = JsonConvert.DeserializeObject<RootObject>(response);
            var firstKey = responseJson.query.pages.First().Key;
            var extract = responseJson.query.pages[firstKey].extract;
            string result = extract;
            return result;
        }


    }
}
