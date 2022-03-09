using System;
using System.Xml;
using System.Text;
using System.Net;
using System.Text.RegularExpressions;
using System.IO;

namespace AIMLBot.AIMLTagHandlers
{
    /// <summary>
    /// The websearch element search data from wiki    
    ///     
    /// for now its only search data from wiki 
    /// </summary>
    public class searchimage : Utils.AIMLTagHandler
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
        public searchimage(Bot bot,
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
            if (this.templateNode.Name.ToLower() == "searchimage")
            {
                if (this.templateNode.InnerText.Length > 0)
                    if (this.templateNode.Attributes.Count == 1)
                        if (this.templateNode.Attributes[0].Name.ToLower() == "name")
                            if (this.templateNode.Attributes[0].Value.ToLower() == "fromfile")
                                loadFromFile();
                            else if (this.templateNode.Attributes[0].Value.ToLower() == "fromtag")
                                loadFromTag();
                            else if (this.templateNode.Attributes[0].Value.ToLower() == "search image")
                            {
                                var totalstring = searchwikiimage(request.rawInput);
                                return totalstring;
                            }
                //else if (this.templateNode.Attributes[0].Value.ToLower() == "fileforgeography")
                //{
                //    loadfileForGeography(request.rawInput);
                //}
                //else if (this.templateNode.Attributes[0].Value.ToLower() == "fileforpersonprofession")
                //{
                //    loadfileforpersonprofession(request.rawInput);
                //}
                //else if (this.templateNode.Attributes[0].Value.ToLower() == "fileforpersonspecificregion")
                //  {
                //      Loadfileforpersonspecificregion(request.rawInput);
                //  }
            }
            return string.Empty;
        }

        void loadFromFile()
        {
            string path = this.templateNode.InnerText;
            FileInfo fi = new FileInfo(path);
            if (!fi.Exists)
            {
                path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, Path.Combine(this.templateNode.InnerText, "log.aiml"));
                fi = new FileInfo(path);
            }
            if (fi.Exists)
            {
                XmlDocument doc = new XmlDocument();
                try
                {
                    doc.Load(path);
                    this.bot.loadAIMLFromXML(doc, path);
                }
                catch
                {
                    this.bot.writeToLog("ERROR! Attempted (but failed) to <learn> some new AIML from the following URI: " + path);
                }
            }
        }
        void loadFromTag()
        {
            var learnData = this.templateNode.InnerText.Split(',');
            this.user.Predicates.addSetting(learnData[0].Trim(), learnData[1].Trim());
            this.user.Predicates.grabSetting(this.templateNode.Attributes[0].Value);

        }

        private string searchwikiimage(string inputimage)
        {
            var learnData = this.templateNode.InnerText.Split(',');

            string withouttag_statement = null;

            if (inputimage.Contains("search image"))
            {
                withouttag_statement = inputimage.Replace("search image", "");
            }
            else if (inputimage.Contains("searchimage"))
            {
                withouttag_statement = inputimage.Replace("searchimage", "");
            }
            else if (inputimage.Contains("search picture"))
            {
                withouttag_statement = inputimage.Replace("search picture", "");
            }
            else if (inputimage.Contains("searchpicture"))
            {
                withouttag_statement = inputimage.Replace("searchpicture", "");
            }
            // string statement = withouttag_statement.First().ToString().ToUpper() + withouttag_statement.Substring(1);

            // string statement = new System.Globalization.CultureInfo("en-US", false).TextInfo.ToTitleCase(withouttag_statement.ToLower());

            var result = Regex.Replace(withouttag_statement, @"\b(\w)", m => m.Value.ToUpper());

            string statement = Regex.Replace(result, @"(\s(of|in|by|and|is|from|lives|was|on)|\'[st])\b", m => m.Value.ToLower(), RegexOptions.IgnoreCase);

            return "_wikiimagesearch" + statement;
        }
    }
}

