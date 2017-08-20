﻿using MongoDB.Bson;
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
            catch(Exception e)
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
                    if (rule.ElementAt(rule.Length - 1) == ' ')
                    {
                        rule = rule.Substring(0, rule.Length - 1);
                    }
                    if (!rule.Equals("\n"))
                    {
                        rules.Add(rule);
                    }
                }
                return rules;
            }
            catch(Exception e)
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
                    while (rule.Contains("and") || rule.Contains("or") || rule.Contains(","))
                    {
                        int andIndex = rule.IndexOf("and");
                        int orIndex = rule.IndexOf("or");
                        int commaIndex = rule.IndexOf(",");
                        int splitIndex = -1;
                        int skipSize = -1;
                        if (andIndex > -1 && (andIndex < orIndex || orIndex < 0) && (andIndex < commaIndex || commaIndex < 0))
                        {
                            splitIndex = andIndex;
                            skipSize = 3;
                        }
                        else if (orIndex > -1 && (orIndex < andIndex || andIndex < 0) && (orIndex < commaIndex || commaIndex < 0))
                        {
                            splitIndex = orIndex;
                            skipSize = 2;
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
                        rule = firstPartOfNewRule + secondPartOfNewRule;
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

        public static List<string> simple2complex(List<string> simpleRules)
        {
            try
            {
                List<string> complexRules = new List<string>();
                List<string> rulesToRemove = new List<string>();

                foreach (string rule1 in simpleRules)
                {
                    if (rulesToRemove.Contains(rule1))
                    {
                        continue;
                    }
                    string newRule = rule1;
                    int lengthOfPart1 = sizeOfPart1(rule1);
                    if (lengthOfPart1 < 0)
                    {
                        complexRules.Add(rule1);
                        continue;
                    }
                    string part1 = rule1.Substring(0, lengthOfPart1);
                    foreach (string rule2 in simpleRules)
                    {
                        if (rule1.Equals(rule2))
                        {
                            continue;
                        }
                        if (rule2.Contains(part1))
                        {
                            newRule = newRule + " ," + rule2.Substring(lengthOfPart1 + 1);
                            rulesToRemove.Add(rule2);
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

        //  x achivedBy a
        // x achivedBy b
        // x achivedBy c
        // x achivedBy d

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
                    text = text + rule + ";" + "\n";
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
    }





}


