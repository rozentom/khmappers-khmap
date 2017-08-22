using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using khmap;
using System.Linq;

namespace khmapTests.UnitTests
{
    [TestClass]
    public class GraphTextTests
    {

        private static readonly int STRING_SIZE_MAX = 10;


        ////////////////////////////////////////////////////simple2complex///////////////////////////////////////////////////////////////

        [TestMethod]
        public void emptyRule_remainsTheSame()
        {
            string text = "";
            List<string> simpleRules = functions.text2rules(text);
            List<string> complexRules = functions.simple2complex(simpleRules);
            string ans = functions.list2text(complexRules);
            Assert.AreEqual(ans, "");
        }

        [TestMethod]
        public void onlySpacesRule_remainsTheSame()
        {
            Random rnd = new Random();
            int size = rnd.Next(0, 10);
            string text = "";
            for (int i= 0; i < size; i++)
            {
                text = text + " ";
            }
            List<string> simpleRules = functions.text2rules(text);
            List<string> complexRules = functions.simple2complex(simpleRules);
            string ans = functions.list2text(complexRules);
            Assert.AreEqual(ans, "");
        }

        [TestMethod]
        public void only_TQ_Rules_remainsTheSame()
        {
            Random rnd = new Random();
            int ammount = rnd.Next(0, 10);
            string text = "";
            for (int i = 0; i < ammount; i++)
            {
                text = text + randomSimpleRule() + ";" + "\n";
            }
            List<string> simpleRules = functions.text2rules(text);
            List<string> complexRules = functions.simple2complex(simpleRules);
            string ans = functions.list2text(complexRules);
            Assert.AreEqual(ans, text);
        }

        [TestMethod]
        public void onlyLinkRules_remainsTheSame()
        {
            Random rnd = new Random();
            int ammount = rnd.Next(1, 10);
            string text = "";
            for (int i = 0; i < ammount; i++)
            {
                text = text + randomLinkRule() + ";" + "\n";
            }
            List<string> simpleRules = functions.text2rules(text);
            List<string> complexRules = functions.simple2complex(simpleRules);
            string ans = functions.list2text(complexRules);
            Assert.AreEqual(ans, text);
        }

        [TestMethod]
        public void LinkAnd_TQ_Rules_remainsTheSame()
        {
            Random rnd = new Random();
            int ammount = rnd.Next(1, 10);
            string text = "";
            for (int i = 0; i < ammount; i++)
            {
                text = text + randomLinkRule() + ";" + "\n";
            }
            for (int i = 0; i < ammount; i++)
            {
                text = text + randomSimpleRule() + ";" + "\n";
            }
            List<string> simpleRules = functions.text2rules(text);
            List<string> complexRules = functions.simple2complex(simpleRules);
            string ans = functions.list2text(complexRules);
            Assert.AreEqual(ans, text);
        }

        [TestMethod]
        public void convertingToComplexOneConverted()
        {
            string node1 = getRandomNodeType() + " " + randomString();
            string node2 = getRandomNodeType() + " " + randomString();
            string node3 = getRandomNodeType() + " " + randomString();
            string link = getRandomLinkType();

            string text = randomLinkRule(node1, node2, link) + ";" + "\n";
            text = text + randomLinkRule(node1, node3, link) + ";" + "\n";
            string andOr = "and";
            if (link.Contains(SharedCodedData.achivedBy) || link.Contains(SharedCodedData.extandedBy))
            {
                andOr = "or";
            }
            string shouldbe = "";
            if (node2.Equals(node3))
            {
                shouldbe = text;
            }
            else
            {
                shouldbe = node1 + " " + link + " " + node2 + " " + andOr + " " + node3 + ";" + "\n";

            }

            List<string> simpleRules = functions.text2rules(text);
            List<string> complexRules = functions.simple2complex(simpleRules);
            string res = functions.list2text(complexRules);


            Assert.AreEqual(shouldbe, res);



        }

        [TestMethod]
        public void convertingToComplexMultipleConverted()
        {
            string node1 = getRandomNodeType() + " " + randomString();
            string node2 = getRandomNodeType() + " " + randomString();
            string node3 = getRandomNodeType() + " " + randomString();
            string node4 = node3 + "1";

            string link = getRandomLinkType();

            string text = randomLinkRule(node1, node2, link) + ";" + "\n";
            text = text + randomLinkRule(node1, node3, link) + ";" + "\n";
            text = text + randomLinkRule(node1, node4, link) + ";" + "\n";
            string andOr = "and";
            if (link.Contains(SharedCodedData.achivedBy) || link.Contains(SharedCodedData.extandedBy))
            {
                andOr = "or";
            }
            string shouldbe = "";
            if (node2.Equals(node3))
            {
                shouldbe = node1 + " " + link + " " + node2 + " " + andOr + " " + node4 + ";" + "\n";
                shouldbe = shouldbe + node1 + " " + link + " " + node3 + " " + andOr + " " + node4 + ";" + "\n";
            }
            else
            {
                shouldbe = node1 + " " + link + " " + node2 + " ," + node3 + " " + andOr + " " + node4 + ";" + "\n";
            }

            List<string> simpleRules = functions.text2rules(text);
            List<string> complexRules = functions.simple2complex(simpleRules);
            string res = functions.list2text(complexRules);


            Assert.AreEqual(shouldbe, res);



        }

        [TestMethod]
        public void convertingToComplexMultipleConvertedAndNotConverted()
        {
            string node1 = getRandomNodeType() + " 1";// + randomString();
            string node2 = getRandomNodeType() + " 2";// + randomString();
            string node3 = getRandomNodeType() + " 3";// + randomString();
            string node4 = node3 + "1";

            string link = getRandomLinkType();

            string text = randomLinkRule(node1, node2, link) + ";" + "\n";
            text = text + randomLinkRule(node1, node3, link) + ";" + "\n";
            text = text + randomLinkRule(node1, node4, link) + ";" + "\n";
            text = text + randomLinkRule(node3, node4, link) + ";" + "\n";
            text = text + randomLinkRule(node4, node2, link) + ";" + "\n";

            string andOr = "and";
            if (link.Contains(SharedCodedData.achivedBy) || link.Contains(SharedCodedData.extandedBy))
            {
                andOr = "or";
            }
            string shouldbe = "";
            if (node2.Equals(node3))
            {
                shouldbe = node1 + " " + link + " " + node2 + " " + andOr + " " + node4 + ";" + "\n";
                shouldbe = shouldbe + node1 + " " + link + " " + node3 + " " + andOr + " " + node4 + ";" + "\n";
            }
            else
            {
                shouldbe = node1 + " " + link + " " + node2 + " ," + node3 + " " + andOr + " " + node4 + ";" + "\n";
            }

            shouldbe = shouldbe + randomLinkRule(node3, node4, link) + ";" + "\n";
            shouldbe = shouldbe + randomLinkRule(node4, node2, link) + ";" + "\n";
            List<string> simpleRules = functions.text2rules(text);
            List<string> complexRules = functions.simple2complex(simpleRules);
            string res = functions.list2text(complexRules);


            Assert.AreEqual(shouldbe, res);



        }



        ///////////////////////////////////////////////////helping functions///////////////////////////////////////////////////////////////

        private static string getRandomNodeType()
        {
            Random rnd = new Random();
            int flag = rnd.Next(0, 1);
            string type = "Quality";
            if (flag == 1)
            {
                type = "Task";
            }
            return type;
        }
        private static string getRandomLinkType()
        {
            Random rnd = new Random();
            int flag = rnd.Next(0, 7);
            
            string type = "";
            if (flag == 0)
            {
                type = SharedCodedData.achivedBy;
            }
            else if (flag == 1)
            {
                type = SharedCodedData.associatedWIth;
            }
            else if (flag == 2)
            {
                type = SharedCodedData.consistsOF;
            }
            else if (flag == 3)
            {
                type = SharedCodedData.extandedBy;
            }
            else if (flag == 4)
            {
                type = SharedCodedData.minus;
            }
            else if (flag == 5)
            {
                type = SharedCodedData.minusminus;
            }
            else if (flag == 6)
            {
                type = SharedCodedData.plus;
            }
            else if (flag == 7)
            {
                type = SharedCodedData.plusplus;
            }
            return type;
        }
        private static string randomSimpleRule()
        {
            string type = getRandomNodeType();
            string rule = type + " " + randomString();
            return rule;
        }

        private static string randomLinkRule()
        {
            string node1 = getRandomNodeType() + " "+ randomString();
            string node2 = getRandomNodeType() + " " + randomString();
            string link = getRandomLinkType();
            string rule = node1 + " " + link + " " + node2;
            return rule;
        }

        private static string randomLinkRule(string node1, string node2, string link)
        {
            string rule = node1 + " " + link + " " + node2;
            return rule;
        }

   /*     private static string randomString()
        {
            Random rnd = new Random();
            int stringSize = rnd.Next(1, STRING_SIZE_MAX + 1);
            string chars = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            string finalString = "";
            for (int i = 0; i < stringSize; i++)
            {
                int index = rnd.Next(0, chars.Length);
                finalString = finalString + chars[index];
            }
            return finalString;
        }
        */
        private static string randomString(int minSize, int maxSize)
        {
            Random rnd = new Random();
            int stringSize = rnd.Next(minSize, maxSize + 1);
            string chars = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            string finalString = "";
            for (int i = 0; i < stringSize; i++)
            {
                int index = rnd.Next(0, chars.Length);
                finalString = finalString + chars[index];
            }
            return finalString;
        }

        public static string randomString()
        {
            Random rnd = new Random();
            int length = rnd.Next(1, STRING_SIZE_MAX + 1);
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[rnd.Next(s.Length)]).ToArray());
        }

    }
}
