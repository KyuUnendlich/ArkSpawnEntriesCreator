using Microsoft.VisualBasic.FileIO;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace ArkSpawnEntriesCreator
{
    class ASA_SpawnContainers
    {
        public static void CheckForBP_C(string path_spawn, string Path)
        {
            using (TextFieldParser csvParser = new TextFieldParser(path_spawn))
            {
                csvParser.CommentTokens = new string[] { "#" };
                csvParser.SetDelimiters(new string[] { "," });
                csvParser.HasFieldsEnclosedInQuotes = true;

                // Skip the row with the column names
                csvParser.ReadLine();
                // Read the row with the blueprint
                string[] dinoBPs = csvParser.ReadFields();

                StringBuilder sb = new StringBuilder();
                bool foundMissing_C = false;

                foreach (string dinoBP in dinoBPs)
                {
                    if (dinoBP.Length > 2)
                    {
                        string dinoBP_EndChars = dinoBP.Substring(dinoBP.Length - 2);
                        if (!dinoBP_EndChars.Equals("_C"))
                        {
                            sb.AppendLine(dinoBP);
                            foundMissing_C = true;
                        }
                    }
                }
                if (foundMissing_C) { 
                    File.AppendAllText(Path, "Missing _Cs at the end: ");
                    File.AppendAllText(Path, "\r\n");
                    File.AppendAllText(Path, sb.ToString());
                    File.AppendAllText(Path, "\r\n");
                }
            }
        }
    }
}
