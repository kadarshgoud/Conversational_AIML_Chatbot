using System;
using System.Xml;
using System.Text;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace AIMLBot.AIMLTagHandlers
{
    /// <summary>
    /// The learn element instructs the AIML interpreter to retrieve a resource specified by a URI, 
    /// and to process its AIML object contents.
    /// </summary>
    public class learn : Utils.AIMLTagHandler
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
        public learn(Bot bot,
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
            if (this.templateNode.Name.ToLower() == "learn")
            {
                // currently only AIML files in the local filesystem can be referenced
                // ToDo: Network HTTP and web service based learning
                if (this.templateNode.InnerText.Length > 0)
                    if (this.templateNode.Attributes.Count == 1)
                        if (this.templateNode.Attributes[0].Name.ToLower() == "name")
                            if (this.templateNode.Attributes[0].Value.ToLower() == "fromfile")
                                loadFromFile();
                            else if (this.templateNode.Attributes[0].Value.ToLower() == "fromtag")
                                loadFromTag();
                            else if (this.templateNode.Attributes[0].Value.ToLower() == "fromcreatedfile")
                            {
                                loadFromCreatedFile(request.rawInput);
                            }
                            else if (this.templateNode.Attributes[0].Value.ToLower() == "fileforgeography")
                            {
                                loadfileForGeography(request.rawInput);
                            }
                            else if (this.templateNode.Attributes[0].Value.ToLower() == "fileforpersonprofession")
                            {
                                loadfileforpersonprofession(request.rawInput);
                            }
                //else if (this.templateNode.Attributes[0].Value.ToLower() == "fileforpersonspecificregion")
                //  {
                //      Loadfileforpersonspecificregion(request.rawInput);
                //  }
            }

            else if (this.templateNode.Name.ToLower() == "forget")
            {
                if (this.templateNode.InnerText.Length > 0)
                    this.user.Predicates.removeSetting(this.templateNode.InnerText);
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

        private void loadFromCreatedFile(string statementnormal)
        {

            var learnData = this.templateNode.InnerText.Split(',');

            string withouttag_statement = statementnormal.Replace("learnf ", "");

            // string statement = withouttag_statement.First().ToString().ToUpper() + withouttag_statement.Substring(1);

            // string statement = new System.Globalization.CultureInfo("en-US", false).TextInfo.ToTitleCase(withouttag_statement.ToLower());

            var result = Regex.Replace(withouttag_statement, @"\b(\w)", m => m.Value.ToUpper());

            string statement = Regex.Replace(result, @"(\s(of|in|by|and|is|from|lives|was|on)|\'[st])\b", m => m.Value.ToLower(), RegexOptions.IgnoreCase);

            if (statement.Contains("is from"))
            {
                string aimlStatement = "<category><pattern>" + learnData[0].ToUpper() + " IS FROM " + "</pattern><template> " + statement + "</template></category>" + Environment.NewLine + "<category><pattern>" + learnData[0].ToUpper() + " IS FROM *" + "</pattern><template> " + statement + "</template></category>" + Environment.NewLine + "<category><pattern>WHO IS FROM " + learnData[1].ToUpper().Trim() + "</pattern><template> " + learnData[0].First().ToString().ToUpper() + learnData[0].Substring(1) + "</template></category>" + Environment.NewLine + "<category><pattern>WHO IS FROM * " + learnData[1].ToUpper().Trim() + "</pattern><template> " + learnData[0].First().ToString().ToUpper() + learnData[0].Substring(1) + "</template></category>" + Environment.NewLine + "<category><pattern>WHERE IS " + learnData[0].ToUpper() + "</pattern><template> " + statement + "</template></category>" + Environment.NewLine + "<category><pattern>WHERE IS " + learnData[0].ToUpper() + " * " + "</pattern><template> " + statement + "</template></category>";

                string path = @"G:\mkrAIMLbot\CustomAIMLStore\fileforpersonspecificregion.aiml";

                // This text is added only once to the file.
                if (!File.Exists(path))
                {
                    // Create a file to write to.
                    string createText = "<?xml version=\"1.0\" encoding=\"utf-8\"?>" + Environment.NewLine + "<aiml>" + Environment.NewLine + aimlStatement + Environment.NewLine + "</aiml>";
                    File.WriteAllText(path, createText, Encoding.UTF8);
                }
                else
                {
                    DeleteLastLine(path);
                    // This text is appended.
                    string appendText = aimlStatement + Environment.NewLine + "</aiml>";
                    File.AppendAllText(path, appendText, Encoding.UTF8);
                }
            }

            else if (statement.Contains("lives in"))
            {
                string aimlStatement = "<category><pattern>" + learnData[0].ToUpper() + " LIVES IN" + "</pattern><template> " + statement + "</template></category>" + Environment.NewLine + "<category><pattern>" + learnData[0].ToUpper() + " LIVES IN *" + "</pattern><template> " + statement + "</template></category>" + Environment.NewLine + "<category><pattern>WHO LIVES IN " + learnData[1].ToUpper().Trim() + "</pattern><template> " + learnData[0].First().ToString().ToUpper() + learnData[0].Substring(1) + "</template></category>" + Environment.NewLine + "<category><pattern>WHO LIVES IN * " + learnData[1].ToUpper().Trim() + "</pattern><template> " + learnData[0].First().ToString().ToUpper() + learnData[0].Substring(1) + "</template></category>" + Environment.NewLine + "<category><pattern>WHERE DOES " + learnData[0].ToUpper() + "</pattern><template> " + statement + "</template></category>" + Environment.NewLine + "<category><pattern>WHERE DOES " + learnData[0].ToUpper() + " * " + "</pattern><template> " + statement + "</template></category>";

                string path = @"G:\mkrAIMLbot\CustomAIMLStore\fileforpersonspecificregion.aiml";

                // This text is added only once to the file.
                if (!File.Exists(path))
                {
                    // Create a file to write to.
                    string createText = "<?xml version=\"1.0\" encoding=\"utf-8\"?>" + Environment.NewLine + "<aiml>" + Environment.NewLine + aimlStatement + Environment.NewLine + "</aiml>";
                    File.WriteAllText(path, createText, Encoding.UTF8);
                }
                else
                {
                    DeleteLastLine(path);
                    // This text is appended.
                    string appendText = aimlStatement + Environment.NewLine + "</aiml>";
                    File.AppendAllText(path, appendText, Encoding.UTF8);
                }
            }

            else if (statement.Contains("was Born in"))
            {
                string aimlStatement = "<category><pattern>" + learnData[0].ToUpper() + " BORN IN" + "</pattern><template> " + statement + "</template></category>" + Environment.NewLine + "<category><pattern>" + learnData[0].ToUpper() + " BORN IN *" + "</pattern><template> " + statement + "</template></category>" + Environment.NewLine + "<category><pattern>WHO BORN IN " + learnData[1].ToUpper().Trim() + "</pattern><template> " + learnData[0].First().ToString().ToUpper() + learnData[0].Substring(1) + "</template></category>" + Environment.NewLine + "<category><pattern>WHO BORN IN * " + learnData[1].ToUpper().Trim() + "</pattern><template> " + learnData[0].First().ToString().ToUpper() + learnData[0].Substring(1) + "</template></category>" + Environment.NewLine + "<category><pattern>WHERE DOES " + learnData[0].ToUpper() + " BORN" + "</pattern><template> " + statement + "</template></category>" + Environment.NewLine + "<category><pattern>WHERE DOES " + learnData[0].ToUpper() + " BIRTH * " + "</pattern><template> " + statement + "</template></category>" + Environment.NewLine + "<category><pattern>WHERE * " + learnData[0].ToUpper() + " BORN" + "</pattern><template> " + statement + "</template></category>" + Environment.NewLine + "<category><pattern>WHERE * " + learnData[0].ToUpper() + " BIRTH * " + "</pattern><template> " + statement + "</template></category>" + Environment.NewLine + "< category >< pattern > " + learnData[0].ToUpper() + " * BORN IN" + " </ pattern >< template > " + statement + " </ template ></ category > ";

                string path = @"G:\mkrAIMLbot\CustomAIMLStore\fileforpersondetails.aiml";

                // This text is added only once to the file.
                if (!File.Exists(path))
                {
                    // Create a file to write to.
                    string createText = "<?xml version=\"1.0\" encoding=\"utf-8\"?>" + Environment.NewLine + "<aiml>" + Environment.NewLine + aimlStatement + Environment.NewLine + "</aiml>";
                    File.WriteAllText(path, createText, Encoding.UTF8);
                }
            }

            else if (statement.Contains("was Born on"))
            {
                string aimlStatement = "<category><pattern>" + learnData[0].ToUpper() + " BORN ON" + "</pattern><template> " + statement + "</template></category>" + Environment.NewLine + "<category><pattern>" + learnData[0].ToUpper() + " * BORN ON *" + "</pattern><template> " + statement + "</template></category>" + Environment.NewLine + "<category><pattern>WHO BORN ON " + learnData[1].ToUpper().Trim() + "</pattern><template> " + learnData[0].First().ToString().ToUpper() + learnData[0].Substring(1) + "</template></category>" + Environment.NewLine + "<category><pattern>WHO BORN ON * " + learnData[1].ToUpper().Trim() + "</pattern><template> " + learnData[0].First().ToString().ToUpper() + learnData[0].Substring(1) + "</template></category>" + Environment.NewLine + "<category><pattern>WHEN DID " + learnData[0].ToUpper() + " BORN" + "</pattern><template> " + statement + "</template></category>" + Environment.NewLine + "<category><pattern>WHEN DID " + learnData[0].ToUpper() + " BIRTH * " + "</pattern><template> " + statement + "</template></category>" + Environment.NewLine + "<category><pattern>" + learnData[0].ToUpper() + " * BORN ON" + "</pattern><template> " + statement + "</template></category>";

                string path = @"G:\mkrAIMLbot\CustomAIMLStore\fileforpersondetails.aiml";

                // This text is added only once to the file.
                if (!File.Exists(path))
                {
                    // Create a file to write to.
                    string createText = "<?xml version=\"1.0\" encoding=\"utf-8\"?>" + Environment.NewLine + "<aiml>" + Environment.NewLine + aimlStatement + Environment.NewLine + "</aiml>";
                    File.WriteAllText(path, createText, Encoding.UTF8);
                }
                else
                {
                    DeleteLastLine(path);
                    // This text is appended.
                    string appendText = aimlStatement + Environment.NewLine + "</aiml>";
                    File.AppendAllText(path, appendText, Encoding.UTF8);
                }

            }

            else
            {

                var aimlStatement = "<category><pattern>WHAT IS " + learnData[0].ToUpper() + "</pattern><template> " + statement + "</template></category>" + Environment.NewLine + "<category><pattern>WHAT IS * " + learnData[0].ToUpper() + "</pattern><template> " + statement + "</template></category>";

                string path = @"G:\mkrAIMLbot\CustomAIMLStore\custom.aiml";

                // This text is added only once to the file.
                if (!File.Exists(path))
                {
                    // Create a file to write to.
                    string createText = "<?xml version=\"1.0\" encoding=\"utf-8\"?>" + Environment.NewLine + "<aiml>" + Environment.NewLine + aimlStatement + Environment.NewLine + "</aiml>";
                    File.WriteAllText(path, createText, Encoding.UTF8);
                }
                else
                {
                    DeleteLastLine(path);
                    // This text is appended.
                    string appendText = aimlStatement + Environment.NewLine + "</aiml>";
                    File.AppendAllText(path, appendText, Encoding.UTF8);
                }

                //this.bot.loadAIMLFromFiles();
            }
        }


        private void loadfileForGeography(string statementnormal)
        {
            var learnData = this.templateNode.InnerText.Split(',');

            string withouttag_statement = statementnormal.Replace("learng ", "");

            // string statement = withouttag_statement.First().ToString().ToUpper() + withouttag_statement.Substring(1);

            // string statement = new System.Globalization.CultureInfo("en-US", false).TextInfo.ToTitleCase(withouttag_statement.ToLower());

            var result = Regex.Replace(withouttag_statement, @"\b(\w)", m => m.Value.ToUpper());

            string statement = Regex.Replace(result, @"(\s(of|in|by|and|is)|\'[st])\b", m => m.Value.ToLower(), RegexOptions.IgnoreCase);

            var aimlStatement = "<category><pattern>WHERE IS " + learnData[0].ToUpper() + "</pattern><template> " + statement + "</template></category>" + Environment.NewLine + "<category><pattern>WHERE IS * " + learnData[0].ToUpper() + "</pattern><template> " + statement + "</template></category>" + Environment.NewLine + "<category><pattern>WHERE IS " + learnData[0].ToUpper() + " * " + "</pattern><template> " + statement + "</template></category>" + Environment.NewLine + "<category><pattern>WHERE IS * " + learnData[0].ToUpper() + " * " + "</pattern><template> " + statement + "</template></category>";

            string path = @"G:\mkrAIMLbot\CustomAIMLStore\fileforGeography.aiml";

            // This text is added only once to the file.
            if (!File.Exists(path))
            {
                // Create a file to write to.
                string createText = "<?xml version=\"1.0\" encoding=\"utf-8\"?>" + Environment.NewLine + "<aiml>" + Environment.NewLine + aimlStatement + Environment.NewLine + "</aiml>";
                File.WriteAllText(path, createText, Encoding.UTF8);
            }
            else
            {
                DeleteLastLine(path);
                // This text is appended.
                string appendText = aimlStatement + Environment.NewLine + "</aiml>";
                File.AppendAllText(path, appendText, Encoding.UTF8);
            }

            //this.bot.loadAIMLFromFiles();
        }

        private void loadfileforpersonprofession(string statement)
        {
            var learnData = this.templateNode.InnerText.Split(',');

            var aimlStatement = "<category><pattern>WHO IS " + learnData[0].ToUpper() + "</pattern><template> " + statement.Replace("learnp ", "") + "</template></category>" + Environment.NewLine + "<category><pattern>WHO IS * " + learnData[0].ToUpper() + "</pattern><template> " + statement.Replace("learnp ", "") + "</template></category>" + Environment.NewLine + "<category><pattern>WHO IS " + learnData[1].ToUpper() + "</pattern><template> " + learnData[0].First().ToString().ToUpper() + learnData[0].Substring(1) + "</template></category>" + Environment.NewLine + "<category><pattern>WHO IS * " + learnData[1].ToUpper() + "</pattern><template> " + learnData[0].First().ToString().ToUpper() + learnData[0].Substring(1) + "</template></category>";

            string path = @"G:\mkrAIMLbot\CustomAIMLStore\fileforpersonprofession.aiml";

            // This text is added only once to the file.
            if (!File.Exists(path))
            {
                // Create a file to write to.
                string createText = "<?xml version=\"1.0\" encoding=\"utf-8\"?>" + Environment.NewLine + "<aiml>" + Environment.NewLine + aimlStatement + Environment.NewLine + "</aiml>";
                File.WriteAllText(path, createText, Encoding.UTF8);
            }
            else
            {
                DeleteLastLine(path);
                // This text is appended.
                string appendText = aimlStatement + Environment.NewLine + "</aiml>";
                File.AppendAllText(path, appendText, Encoding.UTF8);
            }

            //this.bot.loadAIMLFromFiles();
        }

        //private void Loadfileforpersonspecificregion(string statementnormal)
        //{

        //    var learnData = this.templateNode.InnerText.Split(',');

        //    string withouttag_statement = statementnormal.Replace("learnf ", "");

        //    // string statement = withouttag_statement.First().ToString().ToUpper() + withouttag_statement.Substring(1);

        //    // string statement = new System.Globalization.CultureInfo("en-US", false).TextInfo.ToTitleCase(withouttag_statement.ToLower());

        //    var result = Regex.Replace(withouttag_statement, @"\b(\w)", m => m.Value.ToUpper());

        //    string statement = Regex.Replace(result, @"(\s(of|in|by|and|is|from|lives)|\'[st])\b", m => m.Value.ToLower(), RegexOptions.IgnoreCase);

        //    if (statement.Contains("is from"))
        //    {
        //        string aimlStatement = "<category><pattern>" + learnData[0].ToUpper() + " IS FROM " + "</pattern><template> " + statement + "</template></category>" + Environment.NewLine + "<category><pattern>" + learnData[0].ToUpper() + " IS FROM *" + "</pattern><template> " + statement + "</template></category>" + Environment.NewLine + "<category><pattern>WHO IS FROM " + learnData[1].ToUpper().Trim() + "</pattern><template> " + learnData[0].First().ToString().ToUpper() + learnData[0].Substring(1) + "</template></category>" + Environment.NewLine + "<category><pattern>WHO IS FROM * " + learnData[1].ToUpper().Trim() + "</pattern><template> " + learnData[0].First().ToString().ToUpper() + learnData[0].Substring(1) + "</template></category>" + Environment.NewLine + "<category><pattern>WHERE IS " + learnData[0].ToUpper() + "</pattern><template> " + statement + "</template></category>" + Environment.NewLine + "<category><pattern>WHERE IS " + learnData[0].ToUpper() + " * " + "</pattern><template> " + statement + "</template></category>";

        //        string path = @"G:\mkrAIMLbot\CustomAIMLStore\fileforpersonspecificregion.aiml";

        //        // This text is added only once to the file.
        //        if (!File.Exists(path))
        //        {
        //            // Create a file to write to.
        //            string createText = "<?xml version=\"1.0\" encoding=\"utf-8\"?>" + Environment.NewLine + "<aiml>" + Environment.NewLine + aimlStatement + Environment.NewLine + "</aiml>";
        //            File.WriteAllText(path, createText, Encoding.UTF8);
        //        }
        //        else
        //        {
        //            DeleteLastLine(path);
        //            // This text is appended.
        //            string appendText = aimlStatement + Environment.NewLine + "</aiml>";
        //            File.AppendAllText(path, appendText, Encoding.UTF8);
        //        }
        //    }

        //    else if(statement.Contains("lives in"))
        //    {
        //        string aimlStatement = "<category><pattern>" + learnData[0].ToUpper() + " LIVES IN" + "</pattern><template> " + statement + "</template></category>" + Environment.NewLine + "<category><pattern>" + learnData[0].ToUpper() + " LIVES IN *" + "</pattern><template> " + statement + "</template></category>" + Environment.NewLine + "<category><pattern>WHO LIVES IN " + learnData[1].ToUpper().Trim() + "</pattern><template> " + learnData[0].First().ToString().ToUpper() + learnData[0].Substring(1) + "</template></category>" + Environment.NewLine + "<category><pattern>WHO LIVES IN * " + learnData[1].ToUpper().Trim() + "</pattern><template> " + learnData[0].First().ToString().ToUpper() + learnData[0].Substring(1) + "</template></category>" + Environment.NewLine + "<category><pattern>WHERE DOES " + learnData[0].ToUpper() + "</pattern><template> " + statement + "</template></category>" + Environment.NewLine + "<category><pattern>WHERE DOES " + learnData[0].ToUpper() + " * " + "</pattern><template> " + statement + "</template></category>";

        //        string path = @"G:\mkrAIMLbot\CustomAIMLStore\fileforpersonspecificregion.aiml";

        //        // This text is added only once to the file.
        //        if (!File.Exists(path))
        //        {
        //            // Create a file to write to.
        //            string createText = "<?xml version=\"1.0\" encoding=\"utf-8\"?>" + Environment.NewLine + "<aiml>" + Environment.NewLine + aimlStatement + Environment.NewLine + "</aiml>";
        //            File.WriteAllText(path, createText, Encoding.UTF8);
        //        }
        //        else
        //        {
        //            DeleteLastLine(path);
        //            // This text is appended.
        //            string appendText = aimlStatement + Environment.NewLine + "</aiml>";
        //            File.AppendAllText(path, appendText, Encoding.UTF8);
        //        }
        //    }

        //    //string path = @"G:\mkrAIMLbot\CustomAIMLStore\fileforpersonspecificregion.aiml";

        //    //// This text is added only once to the file.
        //    //if (!File.Exists(path))
        //    //{
        //    //    // Create a file to write to.
        //    //    string createText = "<?xml version=\"1.0\" encoding=\"utf-8\"?>" + Environment.NewLine + "<aiml>" + Environment.NewLine + aimlStatement + Environment.NewLine + "</aiml>";
        //    //    File.WriteAllText(path, createText, Encoding.UTF8);
        //    //}
        //    //else
        //    //{
        //    //    DeleteLastLine(path);
        //    //    // This text is appended.
        //    //    string appendText = aimlStatement + Environment.NewLine + "</aiml>";
        //    //    File.AppendAllText(path, appendText, Encoding.UTF8);
        //    //}
        //}



        public static void DeleteLastLine(string filepath)
        {
            List<string> lines = File.ReadAllLines(filepath).ToList();

            File.WriteAllLines(filepath, lines.GetRange(0, lines.Count - 1).ToArray(), Encoding.UTF8);

        }
    }
}
