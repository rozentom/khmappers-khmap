using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

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
    }

  //  x achivedBy a
   // x achivedBy b
   // x achivedBy c
   // x achivedBy d

}


