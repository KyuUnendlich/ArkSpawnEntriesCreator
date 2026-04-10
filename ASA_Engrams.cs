using Microsoft.VisualBasic.FileIO;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace ArkSpawnEntriesCreator
{
    class ASA_Engrams
    {
        public static void AddModifiersToEngrams(string path_engram, string Path)
        {
            List<string> allEngramList = new List<string>();

            StringBuilder sb = new StringBuilder();
            List<string> specialEngrams = new List<string>();

            using (TextFieldParser csvParser = new TextFieldParser(path_engram))
            {
                csvParser.CommentTokens = new string[] { "#" };
                csvParser.SetDelimiters(new string[] { "," });
                csvParser.HasFieldsEnclosedInQuotes = true;

                while (!csvParser.EndOfData)
                {
                    string[] engram = csvParser.ReadFields();

                    string tempEngram = engram[0];

                    if (engram[0] != "") {

                        if (engram[1] != "X" && engram[1] != "x")
                        {
                            string[] stringparts = tempEngram.Split(',');

                            if (engram[2] != "")
                            {

                                int firstEqualsPos = stringparts[1].IndexOf("=") + 1;
                                stringparts[1] = stringparts[1].Substring(0, firstEqualsPos) + engram[2];

                                if (engram[3] != "")
                                {
                                    int secondEqualsPos = stringparts[2].IndexOf("=") + 1;
                                    stringparts[2] = stringparts[2].Substring(0, secondEqualsPos) + engram[3];
                                }

                            }

                            sb.AppendLine(string.Join(",", stringparts));
                        }
                    }
                }
            }

            File.AppendAllText(Path, sb.ToString());
            File.AppendAllText(Path, "\r\n");
            File.AppendAllText(Path, "\r\n");

            string engramReturn = string.Join(",", specialEngrams);
            File.AppendAllText(Path, "EngramWorkbench=" + engramReturn);
            File.AppendAllText(Path, "\r\n");
            File.AppendAllText(Path, "OverrideUnlock=" + engramReturn);


        }

        public static void CreateFullEngramLine(string path_engram, string Path)
        {
            List<string> allEngramList = new List<string>();

            StringBuilder sb = new StringBuilder();

            using (TextFieldParser csvParser = new TextFieldParser(path_engram))
            {
                csvParser.CommentTokens = new string[] { "#" };
                csvParser.SetDelimiters(new string[] { "," });
                csvParser.HasFieldsEnclosedInQuotes = true;

                while (!csvParser.EndOfData)
                {
                    string[] engram = csvParser.ReadFields();

                    sb.AppendLine("OverrideNamedEngramEntries=(EngramClassName=\""+ engram[0] + "\",EngramLevelRequirement=0,EngramPointsCost=0,EngramHidden=False,RemoveEngramPreReq=True)");
                }
            }

            File.AppendAllText(Path, sb.ToString());
        }

        public static void EngramRemoveRemoved(string path_engramAll, string path_engramRemove, string Path)
        {
            List<string> allEngramList = new List<string>();

            using (TextFieldParser csvParserAll = new TextFieldParser(path_engramAll))
            {
                csvParserAll.CommentTokens = new string[] { "#" };
                csvParserAll.SetDelimiters(new string[] { "," });
                csvParserAll.HasFieldsEnclosedInQuotes = true;

                while (!csvParserAll.EndOfData)
                {
                    string[] engram = csvParserAll.ReadFields();
                    allEngramList.Add(engram[0]);
                }
            }

            using (TextFieldParser csvParserRemove = new TextFieldParser(path_engramRemove))
            {
                csvParserRemove.CommentTokens = new string[] { "#" };
                csvParserRemove.SetDelimiters(new string[] { "," });
                csvParserRemove.HasFieldsEnclosedInQuotes = true;

                while (!csvParserRemove.EndOfData)
                {
                    string[] engram = csvParserRemove.ReadFields();

                    if ((engram[0].Contains("EngramHidden=True")) || (engram[0].Contains("EngramHidden=true"))) {

                        string[] engramName = engram[0].Split(',');

                        for (int i = 0; i < allEngramList.Count; i++) {
                            if (allEngramList[i].Contains(engramName[0])) {
                                allEngramList.RemoveAt(i);
                                break;
                            }
                        }

                    }
                }
            }

            foreach (string engram in allEngramList)
            {
                File.AppendAllText(Path, engram);
                File.AppendAllText(Path, "\r\n");
            }
        }

        public static void EngramSplitRemReq(string path_engram, string Path)
        {
            using (TextFieldParser csvParser = new TextFieldParser(path_engram))
            {
                csvParser.CommentTokens = new string[] { "#" };
                csvParser.SetDelimiters(new string[] { "," });
                csvParser.HasFieldsEnclosedInQuotes = true;

                StringBuilder sbRemove = new StringBuilder();
                StringBuilder sbRequire = new StringBuilder();
                StringBuilder sbNotes = new StringBuilder();

                while (!csvParser.EndOfData)
                {
                    string[] engram = csvParser.ReadFields();

                    string engramRemove = "NO ";
                    string engramRequire = "NO ";

                    if ((engram[0].Contains("EngramHidden=True")) || (engram[0].Contains("EngramHidden=true")))
                    {
                        engramRemove = "YES";
                        sbRemove.AppendLine(engram[0]);
                    }

                    if ((engram[0].Contains("RemoveEngramPreReq=True")) || (engram[0].Contains("RemoveEngramPreReq=true")))
                    {
                        engramRequire = "YES";
                        sbRequire.AppendLine(engram[0]);
                    }

                    sbNotes.AppendLine("Remove: "+ engramRemove + " Require: "+ engramRequire+"  "+engram[0]);
                }

                File.AppendAllText(Path, "RemoveList");
                File.AppendAllText(Path, "\r\n");
                File.AppendAllText(Path, sbRemove.ToString());
                File.AppendAllText(Path, "\r\n");
                File.AppendAllText(Path, "RequireList");
                File.AppendAllText(Path, "\r\n");
                File.AppendAllText(Path, sbRequire.ToString());
                File.AppendAllText(Path, "\r\n");
                File.AppendAllText(Path, "NotesList");
                File.AppendAllText(Path, "\r\n");
                File.AppendAllText(Path, sbNotes.ToString());
            }

        }
    } 
}

    

