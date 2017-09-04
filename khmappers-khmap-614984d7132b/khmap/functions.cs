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

        public static Dictionary<string, string> linksToText = new Dictionary<string, string>()
        {
            {"achieved by", SharedCodedData.achivedBy},
            {"consists of", SharedCodedData.consistsOF},
            {"extended by", SharedCodedData.extandedBy},
            {" ", SharedCodedData.associatedWIth},
            {"", SharedCodedData.associatedWIth},
            {"?", "?"},
            {"++", SharedCodedData.plusplus},
            {"+", SharedCodedData.plus},
            {"--", SharedCodedData.minusminus},
            {"-", SharedCodedData.minus}
        };


        public static string removeDupSpace(string s)
        {
            try
            {
                string ans = "";
                int spaceCnt = 0;
                foreach (char c in s)
                {
                    if (c == ' ' && spaceCnt > 0)
                    {
                        continue;
                    }
                    else if (c == ' ')
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
            catch (Exception e)
            {
                throw new Exception();
            }
        }
        public static List<string> text2rules(string text)
        {
            try
            {
                List<string> rules = new List<string>();
                string[] array = text.Split(';');
                foreach (string s in array)
                {
                    string rule = s;
                    if (rule.Length == 0)
                    {
                        continue;
                    }
                    else if (rule.ElementAt(rule.Length - 1) == ' ')
                    {
                        rule = rule.Substring(0, rule.Length - 1);
                    }
                    if (rule.ElementAt(0) == '\n')
                    {
                        rule = rule.Substring(1);
                    }
                    if (!rule.Equals(""))
                    {
                        rules.Add(rule);
                    }
                }
                return rules;
            }
            catch (Exception e)
            {
                throw new Exception();
            }
        }
        public static List<string> complex2simple(List<string> complexRules)
        {
            try
            {
                List<string> simpleRules = new List<string>();
                foreach (string r in complexRules)
                {
                    string rule = r;
                    while (rule.Contains(" and ") || rule.Contains(" or ") || rule.Contains(","))
                    {
                        int andIndex = rule.IndexOf("and");
                        int orIndex = rule.IndexOf("or");
                        int commaIndex = rule.IndexOf(",");
                        int splitIndex = -1;
                        int skipSize = -1;
                        if (andIndex > -1 && (andIndex < orIndex || orIndex < 0) && (andIndex < commaIndex || commaIndex < 0))
                        {
                            splitIndex = andIndex;
                            skipSize = 4;
                        }
                        else if (orIndex > -1 && (orIndex < andIndex || andIndex < 0) && (orIndex < commaIndex || commaIndex < 0))
                        {
                            splitIndex = orIndex;
                            skipSize = 3;
                        }
                        else
                        {
                            splitIndex = commaIndex;
                            skipSize = 1;
                        }
                        string prevAndOr = rule.Substring(0, splitIndex - 1);
                        simpleRules.Add(prevAndOr);
                        int lengthOfPart1 = sizeOfPart1(rule);
                        string firstPartOfNewRule = rule.Substring(0, lengthOfPart1);
                        string secondPartOfNewRule = rule.Substring(splitIndex + skipSize);
                        rule = firstPartOfNewRule + " " + secondPartOfNewRule;
                    }
                    simpleRules.Add(rule);
                }
                return simpleRules;
            }
            catch (Exception e)
            {
                throw new Exception();
            }
        }

        private static int sizeOfPart1(string rule1)
        {
            try
            {


                int lengthOfPart1 = -1;
                if (rule1.Contains(SharedCodedData.achivedBy))
                {
                    lengthOfPart1 = rule1.IndexOf(SharedCodedData.achivedBy) + SharedCodedData.achivedBy.Length;
                }
                else if (rule1.Contains(SharedCodedData.associatedWIth))
                {
                    lengthOfPart1 = rule1.IndexOf(SharedCodedData.associatedWIth) + SharedCodedData.associatedWIth.Length;
                }
                else if (rule1.Contains(SharedCodedData.consistsOF))
                {
                    lengthOfPart1 = rule1.IndexOf(SharedCodedData.consistsOF) + SharedCodedData.consistsOF.Length;
                }
                else if (rule1.Contains(SharedCodedData.extandedBy))
                {
                    lengthOfPart1 = rule1.IndexOf(SharedCodedData.extandedBy) + SharedCodedData.extandedBy.Length;
                }
                else if (rule1.Contains(SharedCodedData.minus))
                {
                    lengthOfPart1 = rule1.IndexOf(SharedCodedData.minus) + SharedCodedData.minus.Length;
                }
                else if (rule1.Contains(SharedCodedData.minusminus))
                {
                    lengthOfPart1 = rule1.IndexOf(SharedCodedData.minusminus) + SharedCodedData.minusminus.Length;
                }
                else if (rule1.Contains(SharedCodedData.plus))
                {
                    lengthOfPart1 = rule1.IndexOf(SharedCodedData.plus) + SharedCodedData.plus.Length;
                }
                else if (rule1.Contains(SharedCodedData.plusplus))
                {
                    lengthOfPart1 = rule1.IndexOf(SharedCodedData.plusplus) + SharedCodedData.plusplus.Length;
                }
                return lengthOfPart1;
            }
            catch (Exception e)
            {
                throw new Exception();
            }
        }
        private static int sizeOfFirstNode(string rule1)
        {
            try
            {
                int lengthOfPart1 = -1;
                if (rule1.Contains(SharedCodedData.achivedBy))
                {
                    lengthOfPart1 = rule1.IndexOf(SharedCodedData.achivedBy) - 1;
                }
                else if (rule1.Contains(SharedCodedData.associatedWIth))
                {
                    lengthOfPart1 = rule1.IndexOf(SharedCodedData.associatedWIth) - 1;
                }
                else if (rule1.Contains(SharedCodedData.consistsOF))
                {
                    lengthOfPart1 = rule1.IndexOf(SharedCodedData.consistsOF) - 1;
                }
                else if (rule1.Contains(SharedCodedData.extandedBy))
                {
                    lengthOfPart1 = rule1.IndexOf(SharedCodedData.extandedBy) - 1;
                }
                else if (rule1.Contains(SharedCodedData.minus))
                {
                    lengthOfPart1 = rule1.IndexOf(SharedCodedData.minus) - 1;
                }
                else if (rule1.Contains(SharedCodedData.minusminus))
                {
                    lengthOfPart1 = rule1.IndexOf(SharedCodedData.minusminus) - 1;
                }
                else if (rule1.Contains(SharedCodedData.plus))
                {
                    lengthOfPart1 = rule1.IndexOf(SharedCodedData.plus) - 1;
                }
                else if (rule1.Contains(SharedCodedData.plusplus))
                {
                    lengthOfPart1 = rule1.IndexOf(SharedCodedData.plusplus) - 1;
                }
                return lengthOfPart1;
            }
            catch (Exception e)
            {
                throw new Exception();
            }
        }

        private static string getLinkText(string rule1)
        {
            try
            {
                string linkText = "";
                if (rule1.Contains(SharedCodedData.achivedBy))
                {
                    linkText = SharedCodedData.achivedBy;
                }
                else if (rule1.Contains(SharedCodedData.associatedWIth))
                {
                    linkText = SharedCodedData.associatedWIth;
                }
                else if (rule1.Contains(SharedCodedData.consistsOF))
                {
                    linkText = SharedCodedData.consistsOF;
                }
                else if (rule1.Contains(SharedCodedData.extandedBy))
                {
                    linkText = SharedCodedData.extandedBy;
                }
                else if (rule1.Contains(SharedCodedData.minus))
                {
                    linkText = SharedCodedData.minus;
                }
                else if (rule1.Contains(SharedCodedData.minusminus))
                {
                    linkText = SharedCodedData.minusminus;
                }
                else if (rule1.Contains(SharedCodedData.plus))
                {
                    linkText = SharedCodedData.plus;
                }
                else if (rule1.Contains(SharedCodedData.plusplus))
                {
                    linkText = SharedCodedData.plusplus;
                }
                return linkText;
            }
            catch (Exception e)
            {
                throw new Exception();
            }
        }

        public static List<string> simple2complex(List<string> simpleRules)
        {
            try
            {
                List<string> complexRules = new List<string>();
                List<string> nodes1ToRemove = new List<string>();
                List<string> rulesToRemoveAsNode2 = new List<string>();


                foreach (string rule1 in simpleRules)
                {

                    string newRule = rule1;
                    int lengthOfPart1 = sizeOfPart1(rule1);
                    if (lengthOfPart1 < 0)
                    {
                        complexRules.Add(rule1);
                        continue;
                    }
                    string part1 = rule1.Substring(0, lengthOfPart1);
                    if (nodes1ToRemove.Contains(part1))
                    {
                        continue;
                    }
                    foreach (string rule2 in simpleRules)
                    {
                        if (rule1.Equals(rule2))
                        {
                            continue;
                        }
                        if (rule2.Contains(part1) && !rulesToRemoveAsNode2.Contains(rule2))
                        {
                            newRule = newRule + " ," + rule2.Substring(lengthOfPart1 + 1);
                            rulesToRemoveAsNode2.Add(rule2);
                            nodes1ToRemove.Add(part1);
                        }
                    }

                    if (newRule.Contains(","))
                    {
                        string wordToAdd = "and";
                        if (rule1.Contains(SharedCodedData.achivedBy) || rule1.Contains(SharedCodedData.extandedBy))
                        {
                            wordToAdd = "or";
                        }
                        int index = newRule.LastIndexOf(",");
                        newRule = newRule.Substring(0, index) + wordToAdd + " " + newRule.Substring(index + 1);
                    }
                    complexRules.Add(newRule);

                }


                return complexRules;
            }
            catch (Exception e)
            {
                throw new Exception();
            }
        }

        private static int addNode(Dictionary<string, int> nodes, BsonArray nodeDataArray, string currNode, int AvailableKey)
        {

            string currNodeText = "";
            bool isTask = currNode.Substring(0, 4).ToUpper().Equals("TASK");
            bool isQuality = currNode.Length>=7 && currNode.Substring(0, 7).ToUpper().Equals("QUALITY");
            if (isTask)
            {
                currNodeText = currNode.Substring(5);
            }
            else if (isQuality) 
            {
                currNodeText = currNode.Substring(8);
            }
            else
            {
                throw new Exception();
            }
            if (!nodes.Keys.Contains(currNode))
            {
                nodes.Add(currNode, AvailableKey);
                string category = "Quality";
                if (isTask)
                {
                    category = "Task";
                }
                BsonDocument node = new BsonDocument
                {
                    { "category", category},
                    { "text", currNodeText},
                    {"fill", "#ffffff" },
                    {"stroke", "#000000" },
                    {"strokeWidth", "1" },
                    {"description", "Add a Description"},
                    { "key", AvailableKey.ToString()},
                    {"refs", new BsonDocument() },
                    {"ctxs", new BsonDocument() },

                };
                nodeDataArray.Add(node);
                AvailableKey--;
            }
            return AvailableKey;
        }

        public static BsonDocument simple2graph(List<string> simpleRules)
        {
            int AvailableKey = -1;
            Dictionary<string, int> nodes = new Dictionary<string, int>();
            BsonArray nodeDataArray = new BsonArray();
            BsonArray linkDataArray = new BsonArray();

            //adding the nodes
            foreach (string rule in simpleRules)
            {
                int indexOfSecondNode = sizeOfPart1(rule);
                if (indexOfSecondNode >= 0)
                {
                    ///add first node
                    int lengthOfFirstNode = sizeOfFirstNode(rule);
                    string firtNode = rule.Substring(0, lengthOfFirstNode);
                    AvailableKey = addNode(nodes, nodeDataArray, firtNode, AvailableKey);

                    ////add second node
                    indexOfSecondNode++;
                    string secondNode = rule.Substring(indexOfSecondNode);
                    AvailableKey = addNode(nodes, nodeDataArray, secondNode, AvailableKey);
                }
                else
                {
                    AvailableKey = addNode(nodes, nodeDataArray, rule, AvailableKey);
                }
            }

            ///adding the links
            foreach (string rule in simpleRules)
            {
                string linkTextKey = getLinkText(rule);
                if (!linkTextKey.Equals(""))
                {
                    int lengthOfFirstNode = sizeOfFirstNode(rule);
                    string firtNode = rule.Substring(0, lengthOfFirstNode);
                    int indexOfSecondNode = sizeOfPart1(rule) + 1;
                    string secondNode = rule.Substring(indexOfSecondNode);

                    if (firtNode.Contains("Task") && secondNode.Contains("Task"))
                    {
                        if (!(linkTextKey.Equals(SharedCodedData.achivedBy) ||
                            linkTextKey.Equals(SharedCodedData.extandedBy) ||
                            linkTextKey.Equals(SharedCodedData.consistsOF)))
                        {
                            throw new Exception();
                        }
                    }
                    else if (firtNode.Contains("Task") && secondNode.Contains("Quality"))
                    {
                        if (linkTextKey.Equals(SharedCodedData.achivedBy) ||
                            linkTextKey.Equals(SharedCodedData.extandedBy) ||
                            linkTextKey.Equals(SharedCodedData.consistsOF))
                        {
                            throw new Exception();
                        }
                    }
                    else if (firtNode.Contains("Task") && secondNode.Contains("Quality"))
                    {
                        if (!linkTextKey.Equals(SharedCodedData.associatedWIth))
                        {
                            throw new Exception();
                        }
                    }
                    else if (firtNode.Contains("Quality") && secondNode.Contains("Quality"))
                    {
                        if (linkTextKey.Equals(SharedCodedData.achivedBy) ||
                            linkTextKey.Equals(SharedCodedData.extandedBy) ||
                            linkTextKey.Equals(SharedCodedData.consistsOF) ||
                            linkTextKey.Equals(SharedCodedData.associatedWIth))
                        {
                            throw new Exception();
                        }
                    }


                    BsonDocument linkDoc = new BsonDocument()
                    {
                        {"category", SharedCodedData.toCategory[linkTextKey] },
                        {"text", SharedCodedData.toText[linkTextKey] },
                        {"routing", new BsonDocument()
                            {
                                {"class", "go.EnumValue"},
                                {"classType", "Link" },
                                {"name", "Normal"}
                            }
                        },
                        {"description", "Add a Description"},
                        {"from", nodes[firtNode] },
                        {"to", nodes[secondNode] },
                        {"refs", new BsonDocument() },
                        {"ctxs", new BsonDocument() },
                    };
                    linkDataArray.Add(linkDoc);
                }
            }

            BsonDocument model = new BsonDocument()
            {
                {"class", "go.GraphLinksModel" },
                {"nodeDataArray", nodeDataArray },
                {"linkDataArray", linkDataArray}
            };
            return model;
        }
        public static List<string> model2List(string currentModel)
        {
            try
            {
                var modelAsBsonDocument = BsonDocument.Parse(currentModel);
                List<string> rules = new List<string>();
                Dictionary<string, string> key2text = new Dictionary<string, string>();
                List<string> nodesThatWerentLinked = new List<string>();


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
                foreach (string s in nodesThatWerentLinked)
                {
                    rules.Add(s);
                }
                return rules;
            }
            catch (Exception e)
            {
                throw new Exception();
            }
        }

        public static string list2text(List<string> rules)
        {
            try
            {
                string text = "";
                foreach (var rule in rules)
                {
                    if (!isOnlySpaces(rule))
                    {
                        text = text + rule + ";" + "\n";
                    }
                }
                return text;
            }
            catch (Exception e)
            {
                throw new Exception();
            }
        }

        public static List<string> fixBackSleshN(List<string> rules)
        {
            try
            {
                List<string> ansRules = new List<string>();
                foreach (string r in rules)
                {
                    string rule = r;
                    if (rule.IndexOf("\n") == 0)
                    {
                        rule = rule.Substring(1);
                    }
                    ansRules.Add(rule);
                }
                return ansRules;
            }
            catch (Exception e)
            {
                throw new Exception();
            }
        }

        public static bool isOnlySpaces(string s)
        {
            foreach (char c in s)
            {
                if (c != ' ')
                {
                    return false;
                }
            }
            return true;
        }

        internal static BsonDocument setGraphLocationsByBson(BsonDocument prevGraph, BsonDocument newGraph)
        {
            var prevNodeDataArray = prevGraph["nodeDataArray"].AsBsonArray;
            var newNodeDataArray = newGraph["nodeDataArray"].AsBsonArray;
            foreach (var oldNode in prevNodeDataArray)
            {
                string oldNodeName = oldNode["text"].ToString();
                foreach (var newNode in newNodeDataArray)
                {
                    string newNodeName = newNode["text"].ToString();
                    if (newNodeName.Equals(oldNodeName))
                    {
                        newNode["loc"] = oldNode["loc"].ToString();
                    }
                }
            }
            return newGraph;
        }

        internal static string removeDupBackSleshN(string s)
        {
            try
            {
                string ans = "";
                int spaceCnt = 0;
                foreach (char c in s)
                {
                    if (c == '\n' && spaceCnt > 0)
                    {
                        continue;
                    }
                    else if (c == '\n')
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
            catch (Exception e)
            {
                throw new Exception();
            }
        }

        public static string removeDupLines(string text)
        {
            var lines = text.Split('\n');
            string ans = "";
            bool flag = false;
            foreach (string line in lines)
            {
                if (!ans.Contains(line))
                {
                    ans = ans + line + "\n";
                    flag = true;
                }
            }
            if (flag)
            {
                ans = ans.Substring(0, ans.Length - 1);
            }
            return ans;
        }

        public static string fixBackSleshInName(string text)
        {
            string ans = "";
            foreach (char c in text)
            {
                if (c != '\n')
                {
                    ans = ans + c;
                }
                if (c == ';')
                {
                    ans = ans + "\n";
                }
            }
            return ans;
        }

        public static BsonDocument simple2LinkNames(List<string> simpleRules)
        {
            BsonDocument res = new BsonDocument()
            {
                {SharedCodedData.achivedBy, new BsonArray() },
                {SharedCodedData.associatedWIth, new BsonArray() },
                {SharedCodedData.consistsOF, new BsonArray() },
                {SharedCodedData.extandedBy, new BsonArray() },
                {"contribution", new BsonArray() },
            };
            
            foreach(string rule in simpleRules)
            {
                int indexOfSecondNode = sizeOfPart1(rule);
                if (indexOfSecondNode >= 0)
                {
                    string linkText = getLinkText(rule);
                    ///get first node
                    int lengthOfFirstNode = sizeOfFirstNode(rule);
                    string firstNode = rule.Substring(0, lengthOfFirstNode);

                    ////get second node
                    indexOfSecondNode++;
                    string secondNode = rule.Substring(indexOfSecondNode);
                    //BsonArray lst = new BsonArray();
                    //lst.Add(firstNode);
                    //lst.Add(secondNode);
                    string labelName = "";
                    if (linkText.Equals(SharedCodedData.minus))
                    {
                        linkText = "contribution";
                        labelName = firstNode + ", " + secondNode + ", -";
                    }
                    else if (linkText.Equals(SharedCodedData.minusminus))
                    {
                        linkText = "contribution";
                        labelName = firstNode + ", " + secondNode + ", --";
                    }
                    else if (linkText.Equals(SharedCodedData.plus))
                    {
                        linkText = "contribution";
                        labelName = firstNode + ", " + secondNode + ", +";
                    }
                    else if (linkText.Equals(SharedCodedData.plusplus))
                    {
                        linkText = "contribution";
                        labelName = firstNode + ", " + secondNode + ", ++";
                    }
                    else
                    {
                        labelName = firstNode + ", " + secondNode;
                    }
                    string firstNodeName = "";
                    if (firstNode.Substring(0, 4).ToLower().Equals("task"))
                    {
                        firstNodeName = firstNode.Substring(5);
                    }
                    else
                    {
                        firstNodeName = firstNode.Substring(8);
                    }
                    string secNodeName = "";
                    if (secondNode.Substring(0, 4).ToLower().Equals("task"))
                    {
                        secNodeName = secondNode.Substring(5);
                    }
                    else
                    {
                        secNodeName = secondNode.Substring(8);
                    }
                    BsonDocument node = new BsonDocument()
                    {
                        {"labelName",labelName },
                        {"firstNode",firstNodeName },
                        {"secondNode",secNodeName }
                    };
                    res[linkText].AsBsonArray.Add(node);
                }
            }
            return res;
        }


        public static BsonArray simple2TQNames(List<string> simpleRules,string taskOrQuality, int taskOrQualityLength)//task = 4, quality = 7
        {
            try
            {
                BsonArray res = new BsonArray();
                foreach (string rule in simpleRules)
                {
                    int indexOfSecondNode = sizeOfPart1(rule);
                    if (indexOfSecondNode >= 0)
                    {
                        ///get first node
                        int lengthOfFirstNode = sizeOfFirstNode(rule);
                        string firstNode = rule.Substring(0, lengthOfFirstNode);

                        ////get second node
                        indexOfSecondNode++;
                        string secondNode = rule.Substring(indexOfSecondNode);

                        if (!res.Contains(firstNode) && firstNode.Length >= taskOrQualityLength && firstNode.ToLower().Substring(0, taskOrQualityLength).Equals(taskOrQuality))
                        {
                            res.Add(firstNode);
                        }
                        if (!res.Contains(secondNode) && secondNode.Length >= taskOrQualityLength && secondNode.ToLower().Substring(0, taskOrQualityLength).Equals(taskOrQuality))
                        {
                            res.Add(secondNode);
                        }
                    }
                    else
                    {
                        if (!res.Contains(rule) && rule.Length >= taskOrQualityLength && rule.ToLower().Substring(0, taskOrQualityLength).Equals(taskOrQuality))
                        {
                            res.Add(rule);
                        }
                    }
                }
                return res;
            }
            catch(Exception e)
            {
                throw new Exception();
            }
        }

        public static BsonDocument simple2Names(List<string> simpleRules)
        {
            BsonDocument res = simple2LinkNames(simpleRules);
            BsonArray taskNames = simple2TQNames(simpleRules, "task", 4);
            BsonArray qualityNames = simple2TQNames(simpleRules, "quality", 7);
            BsonElement taskElement = new BsonElement("task", taskNames);
            BsonElement qualityElement = new BsonElement("quality", qualityNames);
            res.Add(taskElement);
            res.Add(qualityElement);
            return res;
        }
    }






}


