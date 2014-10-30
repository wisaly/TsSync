using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace TsSync
{
    class Program
    {
        static string processQuote(string text)
        {
            if (text.Contains('\'') && text.Contains('"'))
            {
                throw new Exception("Exist ' & \" at same time");
            }
            else if (text.Contains('\''))
            {
                return "\"" + text + "\"";
            }
            else// if (text.Contains('"'))
            {
                return "'" + text + "'";
            }
        }
        static void Main(string[] args)
        {
            if (args.Length != 2)
            {
                Console.WriteLine("usage: tssync unfinished.ts reference.ts");
                return;
            }

            try
            {
                XmlDocument xml = new XmlDocument();
                xml.Load(args[0]);
                XmlDocument xmlRef = new XmlDocument();
                xmlRef.Load(args[1]);

                foreach (XmlNode groupNode in xml.SelectNodes("/TS/context"))
                {
                    string groupName = processQuote(groupNode.SelectSingleNode("name").InnerText);
                    XmlNode refGroupNode = xmlRef.SelectSingleNode(string.Format("/TS/context[name={0}]", groupName));

                    // set unfinished to source
                    foreach (XmlNode msgNode in groupNode.SelectNodes("message"))
                    {
                        XmlNode transNode = msgNode.SelectSingleNode("translation");
                        if (transNode != null && (transNode.Attributes["type"] != null && transNode.Attributes["type"].Value == "unfinished"))
                        {
                            Console.WriteLine("[{0}]->[{1}]", transNode.InnerText, msgNode.SelectSingleNode("source").InnerText);
                            transNode.InnerText = msgNode.SelectSingleNode("source").InnerText;
                        }
                    }

                    if (refGroupNode == null)
                    {
                        continue;
                    }

                    foreach (XmlNode msgNode in groupNode.SelectNodes("message"))
                    {
                        string source = processQuote(msgNode.SelectSingleNode("source").InnerText);
                        XmlNode refMsgNode = refGroupNode.SelectSingleNode(string.Format("message[source={0}]", source));
                        if (refMsgNode == null)
                        {
                            continue;
                        }

                        XmlNode transNode = msgNode.SelectSingleNode("translation");
                        XmlNode refTransNode = refMsgNode.SelectSingleNode("translation");

                        if (transNode != null && refTransNode != null)
                        {
                            Console.WriteLine("[{0}]->[{1}]", transNode.InnerText, refTransNode.InnerText);
                            transNode.InnerText = refTransNode.InnerText;
                            transNode.Attributes.RemoveNamedItem("type");
                        }
                    }
                }

                xml.Save(args[0]);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
    }
}
