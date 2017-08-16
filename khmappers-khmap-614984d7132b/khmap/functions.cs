using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Helpers;

namespace khmap
{
    public class functions
    {

        public static string removeDupSpace(string s)
        {
            string ans = "";
            int spaceCnt = 0;
            foreach(char c in s)
            {
                if(c==' ' && spaceCnt > 0)
                {
                    continue;
                }
                else if(c==' ')
                {
                    spaceCnt++;
                }
                else
                {
                    spaceCnt = 0;
                }
                ans = ans + c;
            }
            return ans;
        }
        public static List<string> text2rules(string text)
        {
            List<string> rules = new List<string>();
            string[] array = text.Split(';');
            foreach (string s in array)
            {
                string rule = s;
                if (rule.ElementAt(rule.Length - 1) == ' ')
                {
                    rule = rule.Substring(0, rule.Length - 1);
                }
                rules.Add(rule);
            }
            return rules;
        }
        public static List<string> conmplex2simple(List<string> complexRules)
        {
            List<string> simpleRules = new List<string>();
            foreach(string r in complexRules)
            {
                string rule = r;
                while(rule.Contains("and") || rule.Contains("or"))
                {
                    int andIndex = rule.IndexOf("and");
                    int orIndex = rule.IndexOf("or");
                    int splitIndex = -1;
                    int skipSize = -1;
                    if (andIndex>-1 && andIndex < orIndex)
                    {
                        splitIndex = andIndex;
                        skipSize = 3;
                    }
                    else
                    {
                        splitIndex = orIndex;
                        skipSize = 2;
                    }
                    string prevAndOr = rule.Substring(0, splitIndex-1);
                    simpleRules.Add(prevAndOr);
                    int lastIndexOfSpace = prevAndOr.LastIndexOf(" ");
                    string firstPartOfNewRule = rule.Substring(0, lastIndexOfSpace);
                    string secondPartOfNewRule = rule.Substring(splitIndex + skipSize);
                    rule = firstPartOfNewRule + secondPartOfNewRule;
                }
                simpleRules.Add(rule);
            }
            return simpleRules;
        }

        public static List<string> simple2complex(List<string> simpleRules)
        {
            List<string> complexRules = new List<string>();

            foreach(string rule1 in simpleRules)
            {
                int lastIndexOfSpaceRule1 = rule1.LastIndexOf(" ");
                string partToSearch = rule1.Substring(0, lastIndexOfSpaceRule1);
                foreach (string rule2 in simpleRules)
                {
                    if (rule2.Contains(partToSearch))
                    {

                    }
                }
            }


            return complexRules;
        }

        //  x achivedBy a
        // x achivedBy b
        // x achivedBy c
        // x achivedBy d

        public static List<string> model2List(string currentModel)
        {
            var modelAsBsonDocument = BsonDocument.Parse(currentModel);
            List<string> rules = new List<string>();
            Dictionary<string, string> key2text = new Dictionary<string, string>();
            List<string> nodesThatWerentLinked = new List<string>();
            Dictionary<string, string> linksToText = new Dictionary<string, string>();
            linksToText.Add("achieved by", "is achived by");
            linksToText.Add("consists of", "is consisted of");
            linksToText.Add("extended by", "is extended by");
            linksToText.Add(" ", "is assosiated with");
            linksToText.Add("", "is assosiated with");
            linksToText.Add("?", "?");
            linksToText.Add("++", "conttributes[positively]");
            linksToText.Add("+", "conttributes[wildly]");
            linksToText.Add("--", "conttributes[negatively]");
            linksToText.Add("-", "conttributes[natrually]");

            foreach (var ruleInModelArray in modelAsBsonDocument["nodeDataArray"].AsBsonArray)
            {
                string key = ruleInModelArray["key"].ToString();
                string type = ruleInModelArray["category"].ToString();
                string text = ruleInModelArray["text"].ToString();
                string value = type + " " + text;
                key2text.Add(key, value);
                nodesThatWerentLinked.Add(value);
            }
            foreach (var link in modelAsBsonDocument["linkDataArray"].AsBsonArray)
            {
                string fromAsKey = null;
                string toAsKey = null;

                try
                {
                    fromAsKey = link["from"].ToString();
                    toAsKey = link["to"].ToString();
                }
                catch
                {

                }      
                string linkKey = link["text"].ToString();

                string ruleAns = "";

                if (fromAsKey != null)
                {
                    string fromAsText = key2text[fromAsKey];
                    ruleAns = ruleAns + fromAsText + " ";
                    ruleAns = ruleAns + linksToText[linkKey];
                    if (toAsKey != null && !linkKey.Equals("?"))
                    {
                        string toAsText = key2text[toAsKey];
                        ruleAns = ruleAns + " " + toAsText;
                        rules.Add(ruleAns);
                        nodesThatWerentLinked.Remove(fromAsText);
                        nodesThatWerentLinked.Remove(toAsText);
                    }
                    else
                    {
                        string fromRule = fromAsText;
                        if (!rules.Contains(fromRule) && nodesThatWerentLinked.Contains(fromAsText))
                        {
                            rules.Add(fromRule);
                            nodesThatWerentLinked.Remove(fromAsText);
                        }
                    }
                }
                else
                {
                    if (toAsKey != null)
                    {
                        string toAsText = key2text[toAsKey];
                        string toRule = toAsText;
                        if (!rules.Contains(toRule) && nodesThatWerentLinked.Contains(toAsText))
                        {
                            rules.Add(toRule);
                            nodesThatWerentLinked.Remove(toAsText);
                        }
                    }
                }
            }
            foreach(string s in nodesThatWerentLinked)
            {
                rules.Add(s);
            }
            return rules;
        }

        public static string list2text(List<string> rules)
        {
            string text = "";
            foreach(var rule in rules)
            {
                text = text + rule + ";" + "\n";
            }
            return text;
        }
    }





}


