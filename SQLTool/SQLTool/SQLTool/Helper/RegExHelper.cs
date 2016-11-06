using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System.Diagnostics;

namespace SQLTool.Helper
{
    public sealed class RegExHelper : Regex
    {
        public RegExHelper(string pattern)
            : base(pattern, RegexOptions.Singleline | RegexOptions.Compiled)
        {
        }

        public List<string> GetMatchList(string s)
        {
            MatchCollection mc = this.Matches(s);
            List<string> l = new List<string>();

            if (mc.Count > 0)
            {
                Match m = mc[0];
                l.Add(m.Groups[1].Value);
                l.Add(m.Groups[2].Value);
            }

            return l;
        }
    }
}
