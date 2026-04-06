using Microsoft.VisualBasic.FileIO;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace ArkSpawnEntriesCreator
{
    class ASA_SpawnContainers
    {
        const string startTextA = "ConfigAddNPCSpawnEntriesContainer=(NPCSpawnEntriesContainerClassString=\"";
        const string startTextB = "\",NPCSpawnEntries=(";
        const string spawnentryA = "(AnEntryName=\"";
        const string spawnentryB = "\",EntryWeight=";
        const string spawnentryC_a = ",NPCsToSpawnStrings=(\"";
        const string spawnentryC_b = "\",\"";
        const string spawnOffsetA = "\"),NPCsSpawnOffsets=((";
        const string spawnOffsetB = "),(";
        const string spawnPercentage = ")),NPCsToSpawnPercentageChance=(";
        const string spawnRest = "),ManualSpawnPointSpreadRadius=1650.0,RandGroupSpawnOffsetZMin=200.0,RandGroupSpawnOffsetZMax=500.0)";
        const string comma = ","; //use in front of non-first spawnentries and spawnlimits
        const string transition = "),NPCSpawnLimits=(";
        const string spawnlimitA = "(NPCClassString=\"";
        const string spawnlimitB = "\",MaxPercentageOfDesiredNumToAllow=";
        const string spawnlimitC = ")";
        const string ending = "))";


        public static void CreateSpawnContainerAdditions(string path_spawn, string Path) {
            using (TextFieldParser csvParser = new TextFieldParser(path_spawn))
            {
                csvParser.CommentTokens = new string[] { "#" };
                csvParser.SetDelimiters(new string[] { "," });
                csvParser.HasFieldsEnclosedInQuotes = true;

                // Skip the row with the column names
                string[] entryNames = csvParser.ReadFields();
                // Read the row with the blueprint
                string[] dinoBPs = csvParser.ReadFields();

                // Skip the row with info
                csvParser.ReadLine();
                // Read the row with multiparams
                string[] multiParams = csvParser.ReadFields();

                // Skip additional rows until first container
                for (int i = 0; i < 7; i++)
                {
                    csvParser.ReadLine();
                }

                StringBuilder sb = new StringBuilder();

                while (!csvParser.EndOfData)
                {
                    List<ASADinoEntry> dinoEntriesAdd = new List<ASADinoEntry>();
                    List<string> dinoEntriesRemove = new List<string>();

                    // Read current line fields, pointer moves to the next line.
                    string[] entryweightArray = csvParser.ReadFields();
                    string[] spawnlimitArray = csvParser.ReadFields();

                    string spawnEntryContainerName = spawnlimitArray[0];

                    for (int currentDinoIndex = 3; currentDinoIndex < entryweightArray.Length; currentDinoIndex++)
                    {
                        if (!entryweightArray[currentDinoIndex].Equals(""))
                        {
                            if (entryweightArray[currentDinoIndex].Equals("x"))
                            {
                                dinoEntriesRemove.Add(dinoBPs[currentDinoIndex]);
                            }
                            else
                            {
                                //Create Multiparams values
                                string[] parts = multiParams[currentDinoIndex].Split(':');
                                int multiParamAmount = Int32.Parse(parts[0]);
                                string multiParamLine = parts[1].Replace("-", ",");

                                ASADinoEntry temp = new ASADinoEntry(entryNames[currentDinoIndex], dinoBPs[currentDinoIndex], entryweightArray[currentDinoIndex], spawnlimitArray[currentDinoIndex], multiParamAmount, multiParamLine);
                                dinoEntriesAdd.Add(temp);

                            }
                        }
                    }

                    if (dinoEntriesAdd.Count != 0)
                    {
                        // First one is different
                        string outputText = startTextA + spawnEntryContainerName + startTextB;

                        // Next ones can be iterated
                        for (int currentDinoEntryIndex = 0; currentDinoEntryIndex < dinoEntriesAdd.Count; currentDinoEntryIndex++)
                        {
                            if (currentDinoEntryIndex != 0)
                            {
                                outputText += comma;
                            }

                            outputText += spawnentryA + dinoEntriesAdd[currentDinoEntryIndex].entryName + spawnentryB + dinoEntriesAdd[currentDinoEntryIndex].entryweight + spawnentryC_a;

                            for (int groupAmount = 0; groupAmount < dinoEntriesAdd[currentDinoEntryIndex].multiParamAmount; groupAmount++)
                            {
                                if (groupAmount != 0)
                                {
                                    outputText += spawnentryC_b;
                                }
                                outputText += dinoEntriesAdd[currentDinoEntryIndex].BP;

                            }
                            outputText += spawnOffsetA;

                            for (int groupAmount = 0; groupAmount < dinoEntriesAdd[currentDinoEntryIndex].multiParamAmount; groupAmount++)
                            {
                                if (groupAmount != 0)
                                {
                                    outputText += spawnOffsetB;
                                }
                                outputText += GetSpawnOffsetPerIndex(groupAmount);

                            }

                            outputText += spawnPercentage + dinoEntriesAdd[currentDinoEntryIndex].multiParamLine + spawnRest;

                        }

                        //Transition to second block
                        outputText += transition + spawnlimitA + dinoEntriesAdd[0].BP + spawnlimitB + dinoEntriesAdd[0].spawnlimit + spawnlimitC;

                        // Next ones can be iterated
                        for (int currentDinoEntryIndex_Again = 1; currentDinoEntryIndex_Again < dinoEntriesAdd.Count; currentDinoEntryIndex_Again++)
                        {
                            outputText += comma + spawnlimitA + dinoEntriesAdd[currentDinoEntryIndex_Again].BP + spawnlimitB + dinoEntriesAdd[currentDinoEntryIndex_Again].spawnlimit + spawnlimitC;
                        }

                        outputText += ending;

                        File.AppendAllText(Path, outputText);
                        File.AppendAllText(Path, "\r\n");

                    }
                }
            }
        }

        public static void CountModdedEntriesPerContainer(string path_spawn, string Path, string path_spawn2, bool secondFile)
        {
            using (TextFieldParser csvParser = new TextFieldParser(path_spawn))
            using (TextFieldParser csvParser2 = new TextFieldParser(path_spawn2))
            {
                csvParser.CommentTokens = new string[] { "#" };
                csvParser.SetDelimiters(new string[] { "," });
                csvParser.HasFieldsEnclosedInQuotes = true;
                csvParser2.CommentTokens = new string[] { "#" };
                csvParser2.SetDelimiters(new string[] { "," });
                csvParser2.HasFieldsEnclosedInQuotes = true;

                // Skip all rows until first container
                for (int i = 0; i < 11; i++)
                {
                    csvParser.ReadLine();
                    csvParser2.ReadLine();
                }

                while (!csvParser.EndOfData || !csvParser2.EndOfData)
                {
                    int dinoCount = 0;

                    // Read current line fields, pointer moves to the next line.
                    string[] entryweightArray = csvParser.ReadFields();
                    string[] entryweightArray2 = csvParser2.ReadFields();
                    csvParser2.ReadFields();
                    string[] spawnlimitArray = csvParser.ReadFields();

                    string spawnEntryContainerName = spawnlimitArray[0];

                    for (int currentDinoIndex = 3; currentDinoIndex < entryweightArray.Length; currentDinoIndex++)
                    {
                        if (!entryweightArray[currentDinoIndex].Equals(""))
                        {
                            if (entryweightArray[currentDinoIndex].Equals("x"))
                            {
                                dinoCount--;
                            }
                            else {
                                dinoCount++;
                            }
                        }
                    }

                    if (secondFile) {
                        for (int currentDinoIndex = 3; currentDinoIndex < entryweightArray2.Length; currentDinoIndex++)
                        {
                            if (!entryweightArray2[currentDinoIndex].Equals(""))
                            {
                                if (entryweightArray[currentDinoIndex].Equals("x"))
                                {
                                    dinoCount--;
                                }
                                else
                                {
                                    dinoCount++;
                                }
                            }
                        }
                    }

                    string txt = spawnEntryContainerName + " Entry Count: ";
                    int txtLength = txt.Length;
                    for (int i = 0; i < 65 - txtLength; i++) { 
                        txt += " ";
                    }

                    File.AppendAllText(Path, txt + dinoCount);
                    File.AppendAllText(Path, "\r\n");
                }
            }
        }

        public static void CreateToolkitReplacements(string path_tool, string Path)
        {
            using (TextFieldParser csvParser = new TextFieldParser(path_tool))
            {
                csvParser.CommentTokens = new string[] { "#" };
                csvParser.SetDelimiters(new string[] { "," });
                csvParser.HasFieldsEnclosedInQuotes = true;

                // Skip the row with the column names
                csvParser.ReadLine();

                StringBuilder sb = new StringBuilder();
                int count = 0;

                while (!csvParser.EndOfData)
                {
                    string[] toolkitLine = csvParser.ReadFields();
                    if (toolkitLine[2] != "")
                    {
                        count++;

                        sb.AppendLine("ReplacementsFromClass" + count + "=" + toolkitLine[2]);

                        string replacementTo = "ReplacementsToClasses" + count + "=" + toolkitLine[4];
                        string replacementPercent = "ReplacementsChances" + count + "=" + toolkitLine[5];

                        int iter = 3;
                        while (toolkitLine[4 + iter] != "")
                        {
                            replacementTo += "," + toolkitLine[4 + iter];
                            replacementPercent += "," + toolkitLine[5 + iter];
                            iter += 3;
                        }

                        sb.AppendLine(replacementTo);
                        sb.AppendLine(replacementPercent);
                    }
                }

                File.AppendAllText(Path, "[CTReplacements]");
                File.AppendAllText(Path, "\r\n");
                File.AppendAllText(Path, sb.ToString());
                File.AppendAllText(Path, "\r\n");
            }
        }

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

        public static void CheckFor_NOT_BP_C(string path_tool, string Path)
        {
            using (TextFieldParser csvParser = new TextFieldParser(path_tool))
            {
                csvParser.CommentTokens = new string[] { "#" };
                csvParser.SetDelimiters(new string[] { "," });
                csvParser.HasFieldsEnclosedInQuotes = true;

                StringBuilder sb = new StringBuilder();
                bool found_C = false;

                while (!csvParser.EndOfData) {
                    string[] potentialBPs = csvParser.ReadFields();
                    foreach (string potBP in potentialBPs)
                    {
                        if (potBP.Length > 2)
                        {
                            string potBP_EndChars = potBP.Substring(potBP.Length - 2);
                            if (potBP_EndChars.Equals("_C"))
                            {
                                sb.AppendLine(potBP);
                                found_C = true;
                            }
                        }
                    }
                }

                if (found_C)
                {
                    File.AppendAllText(Path, "_Cs at the end: ");
                    File.AppendAllText(Path, "\r\n");
                    File.AppendAllText(Path, sb.ToString());
                    File.AppendAllText(Path, "\r\n");
                }
            }
        }

        public static string GetSpawnOffsetPerIndex(int index) {
            switch (index) {
                case 0:
                    return "X=0.0,Y=0.0,Z=0.0";
                case 1:
                    return "X=250.0,Y=0.0,Z=250.0";
                case 2:
                    return "X=-250.0,Y=0.0,Z=250.0";
                case 3:
                    return "X=250.0,Y=0.0,Z=-250.0";
                case 4:
                    return "X=-250.0,Y=0.0,Z=-250.0";
                case 5:
                    return "X=250.0,Y=0.0,Z=0.0";
                case 6:
                    return "X=-250.0,Y=0.0,Z=0.0";
                case 7:
                    return "X=0.0,Y=0.0,Z=250.0";
                case 8:
                    return "X=0.0,Y=0.0,Z=-250.0";
                case 9:
                    return "X=100.0,Y=0.0,Z=0.0";
                case 10:
                    return "X=-100.0,Y=0.0,Z=0.0";
                case 11:
                    return "X=0.0,Y=0.0,Z=100.0";
                case 12:
                    return "X=0.0,Y=0.0,Z=-100.0";
                case 13:
                    return "X=100.0,Y=0.0,Z=100.0";
                case 14:
                    return "X=-100.0,Y=0.0,Z=100.0";
                case 15:
                    return "X=100.0,Y=0.0,Z=-100.0";
                case 16:
                    return "X=-100.0,Y=0.0,Z=-100.0";
                default:
                    return "X=0.0,Y=0.0,Z=0.0";

            } 
        }
    }
}
