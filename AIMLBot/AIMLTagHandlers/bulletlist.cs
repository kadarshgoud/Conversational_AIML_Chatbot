using System;
using System.Xml;
using System.Text;
using System.Collections.Generic;

namespace AIMLBot.AIMLTagHandlers
{
    /// <summary>
    /// The random element instructs the AIML interpreter to return exactly one of its contained li 
    /// elements randomly. The random element must contain one or more li elements of type 
    /// defaultListItem, and cannot contain any other elements.
    /// </summary>
    public class bulletlist : Utils.AIMLTagHandler
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
        public bulletlist(Bot bot,
                        AIMLBot.User user,
                        Utils.SubQuery query,
                        AIMLBot.Request request,
                        AIMLBot.Result result,
                        XmlNode templateNode)
            : base(bot, user, query, request, result, templateNode)
        {
            this.isRecursive = false;
        }

        protected override string ProcessChange()
        {
            if (this.templateNode.Name.ToLower() == "bulletlist")
            {
                if (this.templateNode.HasChildNodes)
                {
                    // only grab <li> nodes
                    List<XmlNode> listNodes = new List<XmlNode>();
                    foreach (XmlNode childNode in this.templateNode.ChildNodes)
                    {
                        if (childNode.Name == "item")
                        {
                            listNodes.Add(childNode);
                        }
                    }

                    foreach (XmlNode Singlenode in listNodes)
                    {
                        if (listNodes.Count > 0)
                        {
                            return "\u2022 '" + Singlenode.InnerXml;
                        }
                    }

                    //for (int i = 0; i <= listNodes.Count; i++)
                    //{

                    //    return "\u2022 '" + listNodes[i];
                    //}
                    //if (listNodes.Count > 0)
                    //{
                    //    //Random r = new Random();
                    //    //XmlNode chosenNode = (XmlNode)listNodes[r.Next(listNodes.Count)];
                    //    //return "\u2022 '" + chosenNode.InnerXml;

                    //    for (int i = 0; i <= listNodes.Count; i++)
                    //    { return "\u2022 '" + listNodes; }

                    //}

                }
            }
            return string.Empty;
        }
    }
}
