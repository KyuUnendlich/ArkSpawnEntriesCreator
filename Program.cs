using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.VisualBasic.FileIO;
using System.Globalization;


namespace ArkSpawnEntriesCreator
{
    class Program
    {
        const string Path = "C:/Users/matth/Desktop/ark.txt";
        
        const string startTextA = "ConfigAddNPCSpawnEntriesContainer=(NPCSpawnEntriesContainerClassString=\"";
        const string startTextB = "\",NPCSpawnEntries=(";
        const string spawnentryA = "(AnEntryName=\"";
        const string spawnentryB = "\",EntryWeight=";
        const string spawnentryC = ",NPCsToSpawnStrings=(\"";
        const string spawnentryD = "\"))";
        const string comma = ","; //use in front of non-first spawnentries and spawnlimits
        const string transition = "),NPCSpawnLimits=(";
        const string spawnlimitA = "(NPCClassString=\"";
        const string spawnlimitB = "\",MaxPercentageOfDesiredNumToAllow=";
        const string spawnlimitC = ")";
        const string ending = "))";
        
        const string startReduceA = "ConfigSubtractNPCSpawnEntriesContainer=(NPCSpawnEntriesContainerClassString=\"";
        const string startReduceB = "\",NPCSpawnEntries=((NPCsToSpawnStrings=(\"";
        const string reduceLoop1 = "\")),(NPCsToSpawnStrings=(\"";
        const string reduceTransition = "\"))),NPCSpawnLimits=((NPCClassString=\"";
        const string reduceLoop2 = "\"),(NPCClassString=\"";
        const string reduceEnding = "\")))";
        
        static void Main(string[] args)
        {
            //OldMethod();
            //OldReduceMethod();
            //CompareCSVFileEntries();
            //return;

            var path = @"G:\ARK Saves\ArkSpawnEntriesCreator\ArkSpawnEntries.csv";
            using (TextFieldParser csvParser = new TextFieldParser(path))
            {
                csvParser.CommentTokens = new string[] { "#" };
                csvParser.SetDelimiters(new string[] { "," });
                csvParser.HasFieldsEnclosedInQuotes = true;

                // Skip the row with the column names
                csvParser.ReadLine();
                // Read the row with the blueprint
                string[] dinoBPs = csvParser.ReadFields();
                // Skip the row with the descriptions
                csvParser.ReadFields();

                while (!csvParser.EndOfData)
                {
                    List<DinoEntry> dinoEntriesAdd = new List<DinoEntry>();
                    List<string> dinoEntriesRemove = new List<string>();
                    
                    // Read current line fields, pointer moves to the next line.
                    string[] entryweightArray = csvParser.ReadFields();
                    string[] spawnlimitArray = csvParser.ReadFields();

                    string spawnContainer = spawnlimitArray[0];
                    for (int i = 2; i < 174; i++)    // hardcoded to debug better, change later
                    {
                        if (!entryweightArray[i].Equals(""))
                        {
                            if (entryweightArray[i].Equals("r"))
                            {
                                dinoEntriesRemove.Add(dinoBPs[i]);
                            }
                            else
                            {
                                DinoEntry temp = new DinoEntry(dinoBPs[i], entryweightArray[i], spawnlimitArray[i]);
                                dinoEntriesAdd.Add(temp);
                                
                            }
                        }
                    }
                    
                    if (dinoEntriesAdd.Count != 0)
                    {
                        // First one is different
                        string outputText = startTextA + spawnContainer + startTextB;
                        
                        // Next ones can be iterated
                        for (int i = 0; i < dinoEntriesAdd.Count; i++)
                        {
                            if (i != 0)
                            {
                                outputText += comma;
                            }

                            outputText += spawnentryA + dinoEntriesAdd[i].BP + spawnentryB + dinoEntriesAdd[i].entryweight + spawnentryC + dinoEntriesAdd[i].BP + spawnentryD;
                            
                        }

                        //Transition to second block
                        outputText += transition + spawnlimitA + dinoEntriesAdd[0].BP + spawnlimitB + dinoEntriesAdd[0].spawnlimit + spawnlimitC;
                        
                        // Next ones can be iterated
                        for (int i = 1; i < dinoEntriesAdd.Count; i++)
                        {
                            outputText += comma + spawnlimitA + dinoEntriesAdd[i].BP + spawnlimitB + dinoEntriesAdd[i].spawnlimit + spawnlimitC;
                        }
                        
                        outputText += ending;

                        File.AppendAllText(Path, outputText);
                        File.AppendAllText(Path, "\r\n");
                    }

                    if (dinoEntriesRemove.Count != 0)
                    {
                        string outputText = startReduceA;
                        outputText += spawnContainer;
                        outputText += startReduceB;
                        outputText += dinoEntriesRemove[0];
                        
                        for (int i = 1; i < dinoEntriesRemove.Count; i++)
                        {
                            outputText += reduceLoop1;
                            outputText += dinoEntriesRemove[i];
                        }

                        outputText += reduceTransition;
                        outputText += dinoEntriesRemove[0];
                        for (int i = 1; i < dinoEntriesRemove.Count; i++)
                        {
                            outputText += reduceLoop2;
                            outputText += dinoEntriesRemove[i];
                        }

                        outputText += reduceEnding;
                        
                        File.AppendAllText(Path, outputText);
                        File.AppendAllText(Path, "\r\n");
                    }
                    // End of line, reset Lists
                }
            }

            
        }

        private static void CompareCSVFileEntries()
        {
            var path = @"G:\ARK Saves\ArkSpawnEntriesCreator\ArkSpawnEntries.csv";
            var pathComp = @"G:\ARK Saves\ArkSpawnEntriesCreator\ArkSpawnEntriesComp.csv";
            using (TextFieldParser csvParser = new TextFieldParser(path))
            using (TextFieldParser csvParserComp = new TextFieldParser(pathComp))
            {
                csvParser.CommentTokens = new string[] { "#" };
                csvParser.SetDelimiters(new string[] { "," });
                csvParser.HasFieldsEnclosedInQuotes = true;

                csvParserComp.CommentTokens = new string[] { "#" };
                csvParserComp.SetDelimiters(new string[] { "," });
                csvParserComp.HasFieldsEnclosedInQuotes = true;

                // Skip the row with the column names
                csvParser.ReadLine();
                // Read the row with the blueprint
                string[] dinoBPs = csvParser.ReadFields();
                // Skip the row with the descriptions
                csvParser.ReadFields();

                // Skip the row with the column names
                csvParserComp.ReadLine();
                // Read the row with the blueprint
                csvParserComp.ReadFields();
                // Skip the row with the descriptions
                csvParserComp.ReadFields();

                while (!csvParser.EndOfData)
                {
                    List<DinoEntry> dinoEntriesAdd = new List<DinoEntry>();
                    List<string> dinoEntriesRemove = new List<string>();

                    // Read current line fields, pointer moves to the next line.
                    string[] entryweightArray = csvParser.ReadFields();
                    string[] spawnlimitArray = csvParser.ReadFields();

                    string[] entryweightArrayComp = csvParserComp.ReadFields();
                    string[] spawnlimitArrayComp = csvParserComp.ReadFields();

                    string spawnContainer = spawnlimitArray[0];
                    for (int i = 2; i < 196; i++)    // hardcoded to debug better, change later
                    {
                        if (!entryweightArray[i].Equals("") && !entryweightArray[i].Equals("r") && !entryweightArrayComp[i].Equals(""))
                        {
                             File.AppendAllText(Path, dinoBPs[i]+spawnContainer+spawnlimitArray[i]);
                             File.AppendAllText(Path, "\r\n");
                        }
                    }
                }
            }
        }

        private static void OldReduceMethod()
        {
            const string spawnContainer = "DinoSpawnEntriesSnow_C";
            const string dino1 = "Yutyrannus_Character_BP_C";
            const string dino2 = "Ptero_Character_BP_C";
            
            string outputText = startReduceA;
            outputText += spawnContainer;
            outputText += startReduceB;
            outputText += dino1;
            //outputText += reduceLoop1;
            //outputText += dino2;
            outputText += reduceTransition;
            outputText += dino1;
            //outputText += reduceLoop2;
            //outputText += dino2;
            outputText += reduceEnding;
            
            File.AppendAllText(Path, outputText);
            File.AppendAllText(Path, "\r\n");
        }

        private static void OldMethod()
        {

            //Change these values as you want new containers
            const int amount = 3;
            const string spawnContainer = "DinoSpawnEntriesBeach_C";
            const string dino1 = "Orolo_Character_BP_C";
            const string entryweight1 = "0.05";
            const string maxAllowed1 = "0.015";
            const string dino2 = "AMonkey_Character_BP_C";
            const string entryweight2 = "0.03";
            const string maxAllowed2 = "0.01";
            const string dino3 = "Zuniceratops_Character_BP_C";
            const string entryweight3 = "0.1";
            const string maxAllowed3 = "0.025";
            const string dino4 = "Brachiosaurus_Character_BP_C";
            const string entryweight4 = "0.05";
            const string maxAllowed4 = "0.07";

            string outputText = startTextA;
            outputText += spawnContainer + startTextB + spawnentryA + dino1 + spawnentryB + entryweight1 + spawnentryC + dino1 + spawnentryD;
            if (amount >= 2){
                outputText += comma + spawnentryA + dino2 + spawnentryB + entryweight2 + spawnentryC + dino2 + spawnentryD;
            }
            if (amount >= 3)
            {
                outputText += comma + spawnentryA + dino3 + spawnentryB + entryweight3 + spawnentryC + dino3 + spawnentryD;
            }
            if (amount >= 4)
            {
                outputText += comma + spawnentryA + dino4 + spawnentryB + entryweight4 + spawnentryC + dino4 + spawnentryD;
            }
            outputText += transition + spawnlimitA + dino1 + spawnlimitB + maxAllowed1 + spawnlimitC;
            if (amount >= 2)
            {
                outputText += comma + spawnlimitA + dino2 + spawnlimitB + maxAllowed2 + spawnlimitC;
            }
            if (amount >= 3)
            {
                outputText += comma + spawnlimitA + dino3 + spawnlimitB + maxAllowed3 + spawnlimitC;
            }
            if (amount >= 4)
            {
                outputText += comma + spawnlimitA + dino4 + spawnlimitB + maxAllowed4 + spawnlimitC;
            }
            outputText += ending;


            File.AppendAllText(Path, outputText);
            File.AppendAllText(Path, "\r\n");
        }
        
        private static void GroupVariant()
        {
            //OldMethod();
            //OldReduceMethod();
            //return;

            var path = @"D:\ARK Saves\ArkSpawnEntriesCreator\ArkSpawnEntries.csv";
            using (TextFieldParser csvParser = new TextFieldParser(path))
            {
                csvParser.CommentTokens = new string[] { "#" };
                csvParser.SetDelimiters(new string[] { "," });
                csvParser.HasFieldsEnclosedInQuotes = true;

                // Skip the row with the column names
                csvParser.ReadLine();
                // Read the row with the blueprint
                string[] dinoBPs = csvParser.ReadFields();
                // Skip the row with the descriptions
                csvParser.ReadFields();

                while (!csvParser.EndOfData)
                {
                    List<DinoEntry> dinoEntriesAdd = new List<DinoEntry>();
                    List<string> dinoEntriesRemove = new List<string>();
                    
                    // Read current line fields, pointer moves to the next line.
                    string[] entryweightArray = csvParser.ReadFields();
                    string[] spawnlimitArray = csvParser.ReadFields();
                    string[] amountChancesArray = csvParser.ReadFields();

                    string spawnContainer = spawnlimitArray[0];
                    for (int i = 2; i < 174; i++)    // hardcoded to debug better, change later
                    {
                        if (!entryweightArray[i].Equals(""))
                        {
                            if (entryweightArray[i].Equals("r"))
                            {
                                dinoEntriesRemove.Add(dinoBPs[i]);
                            }
                            else
                            {
                                if (!amountChancesArray[i].Equals(""))
                                {
                                    string[] amountsArray = amountChancesArray[i].Split(",");

                                    int size = amountsArray.Length;

                                    float entryWeight = float.Parse(entryweightArray[i]);
                                    
                                    float twoChance = 0.0f;
                                    float threeChance = 0.0f;
                                    float fourChance = 0.0f;

                                    float oneChance = float.Parse(amountsArray[0]);

                                    if (size > 1)
                                    {
                                        twoChance = float.Parse(amountsArray[1]);
                                    }

                                    if (size > 2)
                                    {
                                        threeChance = float.Parse(amountsArray[2]);
                                    }

                                    if (size > 3)
                                    {
                                        fourChance = float.Parse(amountsArray[3]);
                                    }

                                    // variable to reduce entries to actual used values, chances are just proportional;
                                    // the idea is to have a percentage of creatures normally as entrywight, now with groups of dinos spawning you have a lot more that spawn
                                    // so you reduce the values so they add up to original dino count
                                    float amountofDinos = oneChance + 2.0f * twoChance + 3.0f * threeChance +
                                                          4.0f * fourChance;
                                    float sumOfChances = oneChance + twoChance + threeChance + fourChance;
                                    float amountMultipleReducer = sumOfChances / amountofDinos; // this is always <= 1
                                    oneChance = oneChance / sumOfChances * amountMultipleReducer * entryWeight;
                                    if (twoChance != 0.0f)
                                    {
                                        twoChance = twoChance / sumOfChances * amountMultipleReducer * entryWeight;
                                    }

                                    if (threeChance != 0.0f)
                                    {
                                        threeChance = threeChance / sumOfChances * amountMultipleReducer * entryWeight;
                                    }

                                    if (fourChance != 0.0f)
                                    {
                                        fourChance = fourChance / sumOfChances * amountMultipleReducer * entryWeight;
                                    }

                                    DinoEntry temp = new DinoEntry(dinoBPs[i], entryweightArray[i], spawnlimitArray[i],
                                        oneChance.ToString("0.000"), twoChance.ToString("0.000"),
                                        threeChance.ToString("0.000"), fourChance.ToString("0.000"));
                                    dinoEntriesAdd.Add(temp);
                                }
                                else
                                {
                                    DinoEntry temp = new DinoEntry(dinoBPs[i], entryweightArray[i], spawnlimitArray[i]);
                                    dinoEntriesAdd.Add(temp);
                                }


                            }
                        }
                    }
                    
                    if (dinoEntriesAdd.Count != 0)
                    {
                        // First one is different
                        string outputText = startTextA + spawnContainer + startTextB;
                        
                        // Next ones can be iterated
                        for (int i = 0; i < dinoEntriesAdd.Count; i++)
                        {
                            if (i != 0)
                            {
                                outputText += comma;
                            }

                            outputText += spawnentryA + dinoEntriesAdd[i].BP + spawnentryB + dinoEntriesAdd[i].chanceForOne + spawnentryC + dinoEntriesAdd[i].BP + spawnentryD;

                            if (!dinoEntriesAdd[i].chanceForTwo.Equals("0.000"))
                            {
                                outputText += comma + spawnentryA + dinoEntriesAdd[i].BP + "2" + spawnentryB + dinoEntriesAdd[i].chanceForTwo + spawnentryC + dinoEntriesAdd[i].BP + "\",\"" + dinoEntriesAdd[i].BP + spawnentryD;
                            }
                            if (!dinoEntriesAdd[i].chanceForThree.Equals("0.000"))
                            {
                                outputText += comma + spawnentryA + dinoEntriesAdd[i].BP + "3" + spawnentryB + dinoEntriesAdd[i].chanceForThree + spawnentryC + dinoEntriesAdd[i].BP + "\",\"" + dinoEntriesAdd[i].BP + "\",\"" + dinoEntriesAdd[i].BP + spawnentryD;
                            }
                            if (!dinoEntriesAdd[i].chanceForFour.Equals("0.000"))
                            {
                                outputText += comma + spawnentryA + dinoEntriesAdd[i].BP + "4" + spawnentryB + dinoEntriesAdd[i].chanceForFour + spawnentryC + dinoEntriesAdd[i].BP + "\",\"" + dinoEntriesAdd[i].BP + "\",\"" + dinoEntriesAdd[i].BP + "\",\"" + dinoEntriesAdd[i].BP + spawnentryD;
                            }
                        }

                        //Transition to second block
                        outputText += transition + spawnlimitA + dinoEntriesAdd[0].BP + spawnlimitB + dinoEntriesAdd[0].spawnlimit + spawnlimitC;
                        
                        // Next ones can be iterated
                        for (int i = 1; i < dinoEntriesAdd.Count; i++)
                        {
                            outputText += comma + spawnlimitA + dinoEntriesAdd[i].BP + spawnlimitB + dinoEntriesAdd[i].spawnlimit + spawnlimitC;
                        }
                        
                        outputText += ending;

                        File.AppendAllText(Path, outputText);
                        File.AppendAllText(Path, "\r\n");
                    }

                    if (dinoEntriesRemove.Count != 0)
                    {
                        string outputText = startReduceA;
                        outputText += spawnContainer;
                        outputText += startReduceB;
                        outputText += dinoEntriesRemove[0];
                        
                        for (int i = 1; i < dinoEntriesRemove.Count; i++)
                        {
                            outputText += reduceLoop1;
                            outputText += dinoEntriesRemove[i];
                        }

                        outputText += reduceTransition;
                        outputText += dinoEntriesRemove[0];
                        for (int i = 1; i < dinoEntriesRemove.Count; i++)
                        {
                            outputText += reduceLoop2;
                            outputText += dinoEntriesRemove[i];
                        }

                        outputText += reduceEnding;
                        
                        File.AppendAllText(Path, outputText);
                        File.AppendAllText(Path, "\r\n");
                    }
                    // End of line, reset Lists
                }
            }

            
        }
    }

    public struct DinoEntry
    {
        public string BP;
        public string entryweight;
        public string spawnlimit;
        public string chanceForOne;
        public string chanceForTwo;
        public string chanceForThree;
        public string chanceForFour;
        
        public DinoEntry(string BP, string entryweight, string spawnlimit, string chanceForOne, string chanceForTwo, string chanceForThree, string chanceForFour)
        {
            this.BP = BP;
            this.entryweight = entryweight;
            this.spawnlimit = spawnlimit;
            this.chanceForOne = chanceForOne;
            this.chanceForTwo = chanceForTwo;
            this.chanceForThree = chanceForThree;
            this.chanceForFour = chanceForFour;
        }
        
        public DinoEntry(string BP, string entryweight, string spawnlimit)
        {
            this.BP = BP;
            this.entryweight = entryweight;
            this.spawnlimit = spawnlimit;
            this.chanceForOne = entryweight;
            this.chanceForTwo = "0.000";
            this.chanceForThree = "0.000";
            this.chanceForFour = "0.000";
        }
    }
}
