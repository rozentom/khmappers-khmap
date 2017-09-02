using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace khmap
{
    public class SharedCodedData
    {
        public static string OWNED_SUPIRIOR = "ownedSup";
        public static string SHARED_SUPIRIOR = "sharedSup";
        public static string NOT_SUPIRIOR_BUT_SHARED = "shared";
        public static string NOT_SUPIRIOR_BUT_OWNED = "owned";

        public static string achivedBy = "is achieved by";
        public static string consistsOF = "consists of";
        public static string extandedBy = "is extended by";
        public static string associatedWIth = "is assosiated with";
        public static string plusplus = "contributes[positively]";
        public static string plus = "contributes[wildly]";
        public static string minusminus = "contributes[negatively]";
        public static string minus = "contributes[natrually]";

        public static Dictionary<string, string> toCategory = new Dictionary<string, string>()
        {
            {achivedBy, "AchievedBy" },
            {consistsOF, "ConsistsOf" },
            {extandedBy, "ExtendedBy" },
            {associatedWIth, "Association" },
            {plus, "Contribution" },
            {plusplus, "Contribution" },
            {minus, "Contribution" },
            {minusminus, "Contribution" }
        };

        public static Dictionary<string, string> toText = new Dictionary<string, string>()
        {
            {achivedBy, "achieved by" },
            {consistsOF, "consists of" },
            {extandedBy, "extended by" },
            {associatedWIth, "" },
            {plus, "+" },
            {plusplus, "++" },
            {minus, "-" },
            {minusminus, "--" }
        };
    }
}