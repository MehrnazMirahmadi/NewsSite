using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Framework.Implementations
{
    public static class ValidateFileName
    {
        // This method checks if the file name is valid.
        public static bool CheckFileName(this string fileName)
        {
            if (fileName == null)
            {
                return false;
            }

            fileName = fileName.Trim().ToLower();

            if (ContainsInvalidExtensions(fileName))
            {
                return false;
            }
           

            return true;
        }


        // This method generates a unique file name by appending a GUID.
        public static string ToUniqueFileName(this string fileName)
        {
            return Guid.NewGuid().ToString().Replace("-", "_") + fileName;
        }

        // This method checks if the file name contains invalid extensions.
        private static bool ContainsInvalidExtensions(string fileName)
        {
            string[] invalidExtensions =
            {
            ".php", ".asp", ".ascx", ".aspx", ".jsp", ".jspx", ".exe", ".bat", ".sh", ".cmd",
            ".com", ".dll", ".vbs", ".js", ".jse", ".wsf", ".wsh", ".msc", ".msi", ".msp",
            ".jar", ".vb", ".vbe", ".py", ".pl", ".cgi", ".sql", ".rb", ".war", ".ear",
            ".adp", ".bas", ".chm", ".crt", ".csh", ".fxp", ".hlp", ".hta", ".inf", ".ins",
            ".isp", ".jse", ".ksh", ".lnk", ".mdb", ".mde", ".mdt", ".mdw", ".msc", ".ocx",
            ".ops", ".pcd", ".pif", ".prf", ".ps1", ".scf", ".scr", ".sct", ".shb", ".sys",
            ".url", ".vb", ".vbe", ".vbs", ".vsmacros", ".vss", ".vst", ".vsw", ".ws", ".wsc",
            ".wsf", ".wsh", ".xnk", ".psm1", ".psd1", ".cpl", ".msh", ".msh1", ".msh2",
            ".mshxml", ".msh1xml", ".msh2xml", ".gadget", ".ad", ".adn", ".mad", ".maf", ".mag",
            ".mam", ".maq", ".mar", ".mas", ".mat", ".mau", ".mav", ".maw", ".cnt", ".pdx",
            ".reg", ".scf", ".ps1xml", ".ps2", ".ps2xml", ".cer", ".der", ".crt", ".p12", ".pfx",
            ".stm", ".hlp", ".mspx"
        };

            foreach (var extension in invalidExtensions)
            {
                if (fileName.Contains(extension))
                {
                    return true;
                }
            }

            return false;
        }

        // Additional validation method for file names.
        public static bool AdditionalValidation(this string fileName)
        {
            fileName = fileName.Trim().ToLower();

            if (ContainsInvalidExtensions(fileName))
            {
                return false;
            }

            // Add any other validation logic here if needed.
            return true;
        }


    }


}
