using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Fisk.EnterpriseManageUtilities.DBUtility
{
    public class AdomdQueryImporter
    {
        private  string fileContents;

        public  AdomdQueryImporter(string filename)
        {
            this.fileContents = File.ReadAllText(AppDomain.CurrentDomain.BaseDirectory+ filename);
        }

        public string FromFile(string queryKey)
        {
            Regex regex = new Regex(@"[/|\-]{2}<" + queryKey + @">([^@]*)[/|\-]{2}</" + queryKey + @">");
            Match match = regex.Match(fileContents);

            return match.Success ? match.Groups[1].ToString() : "";
        }
    }
}
