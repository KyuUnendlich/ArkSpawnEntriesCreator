using Microsoft.VisualBasic.FileIO;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;

namespace ArkSpawnEntriesCreator
{
    class ASA_LootDrops
    {
        const string startTextA = ",(SetName=\"";
        const string startTextB = "\",MinNumItems=";
        const string startTextC = ",MaxNumItems=";
        const string startTextD = ",NumItemsPower=1,SetWeight=";
        const string startTextE = ",bItemsRandomWithoutReplacement=True,ItemEntries=((EntryWeight=500,ItemClassStrings=(";

        const string middleText = "),ItemsWeights=(";

        const string settingsA = "),MinQuantity=";
        const string settingsB = ",MaxQuantity=";
        const string settingsC = ",MinQuality=";
        const string settingsD = ",MaxQuality=";
        const string settingsE = ",bForceBlueprint=";
        const string settingsF = ",ChanceToBeBlueprintOverride=";
        const string settingsG = ",ItemStatClampsMultiplier=";
        const string settingsH = ")))" + "))"; //extra two )) because its the final lootset


        public static void CreateModdedLootdrops(string baseLoot, string modAdditions, string Path)
        {

            List<LootDrop> lootDrops = new List<LootDrop>();

            using (var sr = new StreamReader(baseLoot))
            {
                string line;
                while ((line = sr.ReadLine()) != null)
                {

                    string[] lootdropName = line.Split("\"");

                    LootDrop lootdrop = CreateLootdropConstraints(lootdropName[1]);

                    int indexOfMinItemSets = line.IndexOf("MinItemSets=");
                    int indexOfMaxItemSets = line.IndexOf("MaxItemSets=");

                    string stringPart1 = line.Substring(0, indexOfMinItemSets+12);
                    string stringPart2 = line.Substring(indexOfMinItemSets+13, indexOfMaxItemSets - indexOfMinItemSets - 1);
                    string stringPart3 = line.Substring(indexOfMaxItemSets+13, line.Length - indexOfMaxItemSets  - 2 - 13); //Remove last 2 characters (two "))" )

                    //Add 1 to Min and MaxItemSets before saving the line
                    int minItemSets = Int32.Parse(line.Substring(indexOfMinItemSets + 12, 1)) + 1;
                    int maxItemSets = Int32.Parse(line.Substring(indexOfMaxItemSets + 12, 1)) + 1;

                    string reconstructedLine = stringPart1 + minItemSets + stringPart2 + maxItemSets + stringPart3;
                    lootdrop.line = reconstructedLine;

                    lootdrop.setWeight = GenerateSetWeight(lootdrop.line).ToString();

                    lootDrops.Add(lootdrop);
                }
            }

            using (TextFieldParser csvParser = new TextFieldParser(modAdditions))
            {
                csvParser.CommentTokens = new string[] { "#" };
                csvParser.SetDelimiters(new string[] { "," });
                csvParser.HasFieldsEnclosedInQuotes = true;

                while (!csvParser.EndOfData)
                {
                    string[] lootLine = csvParser.ReadFields();

                    if (!lootLine[2].Equals("x") && !lootLine[3].Equals(""))
                    {
                        string primal = lootLine[3];
                        int level = Int32.Parse(lootLine[1]);

                        foreach (LootDrop lootdrop in lootDrops)
                        {
                            if (lootdrop.minLevelBP <= level && lootdrop.maxLevelBP >= level)
                            {
                                lootdrop.addedLootDropBPs.Add(primal);
                            }
                        }
                    }
                }
            }


            foreach (LootDrop lootdrop in lootDrops) {

                string temp = lootdrop.line;
                temp += startTextA + "Modded Saddles" + startTextB + lootdrop.minNumItems + startTextC + lootdrop.maxNumItems + startTextD + lootdrop.setWeight + startTextE;

                string primals = "";
                string itemWeights = "";

                for (int i = 0; i < lootdrop.addedLootDropBPs.Count; i++) {
                    if (i != 0) {
                        primals += ",";
                        itemWeights += ",";
                    }
                    primals += "\"" + lootdrop.addedLootDropBPs[i] + "\"";
                    itemWeights += 5000;
                }

                temp += primals + middleText + itemWeights;
                temp += settingsA + lootdrop.minQuantity + settingsB + lootdrop.maxQuantity + settingsC + lootdrop.minQuality + settingsD + lootdrop.maxQuality;
                temp += settingsE + lootdrop.blueprint + settingsF + lootdrop.blueprintChance + settingsG + "0" + settingsH;

                File.AppendAllText(Path, temp);
                File.AppendAllText(Path, "\r\n");
            }

        }

        private static int GenerateSetWeight(string line)
        {
            string[] setWeightsArray = line.Split("SetWeight=");
            float alreadyExistingWeights = 0;

            for (int i = 1; i < setWeightsArray.Length; i++) { 
                int findComma = setWeightsArray[i].IndexOf(',');
                string setWeightC = setWeightsArray[i].Substring(0, findComma);

                float setWeightI = float.Parse(setWeightC, CultureInfo.InvariantCulture);
                alreadyExistingWeights += setWeightI;
            }
            return (int) alreadyExistingWeights / 2;
        }

        public static LootDrop CreateLootdropConstraints(string lootDropName)
        {
            switch (lootDropName)
            {
                case "SupplyCrate_Cave_QualityTier1_C":
                    return new LootDrop(lootDropName, minLevelBP: 1, maxLevelBP: 35, minNumItems: "1", maxNumItems: "3", setWeight: "500", minQuantity: "2",
                        maxQuantity: "3", minQuality: "1.2", maxQuality: "2.3", blueprint: "False", blueprintChance: "0.3");
                case "SupplyCrate_Cave_QualityTier2_C":
                    return new LootDrop(lootDropName, minLevelBP: 36, maxLevelBP: 66, minNumItems: "1", maxNumItems: "3", setWeight: "500", minQuantity: "2",
                        maxQuantity: "3", minQuality: "1.2", maxQuality: "2.3", blueprint: "False", blueprintChance: "0.3");
                case "SupplyCrate_Cave_QualityTier3_C":
                    return new LootDrop(lootDropName, minLevelBP: 67, maxLevelBP: 89, minNumItems: "1", maxNumItems: "3", setWeight: "500", minQuantity: "2",
                        maxQuantity: "3", minQuality: "1.2", maxQuality: "2.3", blueprint: "False", blueprintChance: "0.3");
                case "SupplyCrate_Cave_QualityTier3_Ragnarok_C":
                    return new LootDrop(lootDropName, minLevelBP: 75, maxLevelBP: 89, minNumItems: "1", maxNumItems: "3", setWeight: "500", minQuantity: "2",
                        maxQuantity: "3", minQuality: "1.2", maxQuality: "2.3", blueprint: "False", blueprintChance: "0.3");
                case "SupplyCrate_Cave_QualityTier3_ScorchedEarth_C":
                    return new LootDrop(lootDropName, minLevelBP: 75, maxLevelBP: 89, minNumItems: "2", maxNumItems: "3", setWeight: "500", minQuantity: "3",
                        maxQuantity: "4", minQuality: "1.2", maxQuality: "2.3", blueprint: "False", blueprintChance: "0.3");
                case "SupplyCrate_Cave_QualityTier4_C":
                    return new LootDrop(lootDropName, minLevelBP: 91, maxLevelBP: 105, minNumItems: "1", maxNumItems: "3", setWeight: "500", minQuantity: "2",
                        maxQuantity: "3", minQuality: "1.2", maxQuality: "2.3", blueprint: "False", blueprintChance: "0.3");
                case "SupplyCrate_Cave_QualityTier4_Ragnarok_C":
                    return new LootDrop(lootDropName, minLevelBP: 91, maxLevelBP: 105, minNumItems: "2", maxNumItems: "3", setWeight: "500", minQuantity: "3",
                        maxQuantity: "4", minQuality: "1.2", maxQuality: "2.3", blueprint: "False", blueprintChance: "0.3");
                case "SupplyCrate_SwampCaveTier3_C":
                    return new LootDrop(lootDropName, minLevelBP: 75, maxLevelBP: 105, minNumItems: "2", maxNumItems: "3", setWeight: "500", minQuantity: "4",
                        maxQuantity: "5", minQuality: "1.2", maxQuality: "2.3", blueprint: "False", blueprintChance: "0.3");

                case "SupplyCrate_IceCaveTier1_C":
                    return new LootDrop(lootDropName, minLevelBP: 1, maxLevelBP: 35, minNumItems: "1", maxNumItems: "3", setWeight: "500", minQuantity: "3",
                        maxQuantity: "4", minQuality: "1.2", maxQuality: "2.3", blueprint: "False", blueprintChance: "0.3");
                case "SupplyCrate_IceCaveTier2_C":
                    return new LootDrop(lootDropName, minLevelBP: 36, maxLevelBP: 66, minNumItems: "1", maxNumItems: "3", setWeight: "500", minQuantity: "3",
                        maxQuantity: "4", minQuality: "1.2", maxQuality: "2.3", blueprint: "False", blueprintChance: "0.3");
                case "SupplyCrate_IceCaveTier3_C":
                    return new LootDrop(lootDropName, minLevelBP: 67, maxLevelBP: 90, minNumItems: "1", maxNumItems: "3", setWeight: "500", minQuantity: "4",
                        maxQuantity: "5", minQuality: "1.2", maxQuality: "2.3", blueprint: "False", blueprintChance: "0.3");

                case "SupplyCrate_OceanInstant_C":
                    return new LootDrop(lootDropName, minLevelBP: 95, maxLevelBP: 200, minNumItems: "1", maxNumItems: "2", setWeight: "500", minQuantity: "1",
                        maxQuantity: "2", minQuality: "1.2", maxQuality: "2.3", blueprint: "False", blueprintChance: "0.3");

                case "SupplyCrate_Level03_C":
                    return new LootDrop(lootDropName, minLevelBP: 1, maxLevelBP: 21, minNumItems: "1", maxNumItems: "3", setWeight: "500", minQuantity: "2",
                        maxQuantity: "3", minQuality: "1.2", maxQuality: "2.3", blueprint: "False", blueprintChance: "0.3");
                case "SupplyCrate_Level03_Double_C":
                    return new LootDrop(lootDropName, minLevelBP: 1, maxLevelBP: 21, minNumItems: "2", maxNumItems: "4", setWeight: "500", minQuantity: "3",
                        maxQuantity: "4", minQuality: "1.2", maxQuality: "2.3", blueprint: "False", blueprintChance: "0.3");
                case "SupplyCrate_Level03_ScorchedEarth_C":
                    return new LootDrop(lootDropName, minLevelBP: 1, maxLevelBP: 25, minNumItems: "1", maxNumItems: "3", setWeight: "500", minQuantity: "2",
                        maxQuantity: "3", minQuality: "1.2", maxQuality: "2.3", blueprint: "False", blueprintChance: "0.3");
                case "SupplyCrate_Level03_Double_ScorchedEarth_C":
                    return new LootDrop(lootDropName, minLevelBP: 1, maxLevelBP: 25, minNumItems: "2", maxNumItems: "4", setWeight: "500", minQuantity: "3",
                        maxQuantity: "4", minQuality: "1.2", maxQuality: "2.3", blueprint: "False", blueprintChance: "0.3");

                case "SupplyCrate_Level15_C":
                    return new LootDrop(lootDropName, minLevelBP: 22, maxLevelBP: 37, minNumItems: "1", maxNumItems: "3", setWeight: "500", minQuantity: "2",
                        maxQuantity: "3", minQuality: "1.2", maxQuality: "2.3", blueprint: "False", blueprintChance: "0.3");
                case "SupplyCrate_Level15_Double_C":
                    return new LootDrop(lootDropName, minLevelBP: 22, maxLevelBP: 37, minNumItems: "2", maxNumItems: "4", setWeight: "500", minQuantity: "3",
                        maxQuantity: "4", minQuality: "1.2", maxQuality: "2.3", blueprint: "False", blueprintChance: "0.3");
                case "SupplyCrate_Level15_ScorchedEarth_C":
                    return new LootDrop(lootDropName, minLevelBP: 26, maxLevelBP: 40, minNumItems: "1", maxNumItems: "3", setWeight: "500", minQuantity: "2",
                        maxQuantity: "3", minQuality: "1.2", maxQuality: "2.3", blueprint: "False", blueprintChance: "0.3");
                case "SupplyCrate_Level15_Double_ScorchedEarth_C":
                    return new LootDrop(lootDropName, minLevelBP: 26, maxLevelBP: 40, minNumItems: "2", maxNumItems: "4", setWeight: "500", minQuantity: "3",
                        maxQuantity: "4", minQuality: "1.2", maxQuality: "2.3", blueprint: "False", blueprintChance: "0.3");

                case "SupplyCrate_Level25_C":
                    return new LootDrop(lootDropName, minLevelBP: 38, maxLevelBP: 50, minNumItems: "1", maxNumItems: "3", setWeight: "500", minQuantity: "2",
                        maxQuantity: "3", minQuality: "1.2", maxQuality: "2.3", blueprint: "False", blueprintChance: "0.3");
                case "SupplyCrate_Level25_Double_C":
                    return new LootDrop(lootDropName, minLevelBP: 38, maxLevelBP: 50, minNumItems: "2", maxNumItems: "4", setWeight: "500", minQuantity: "3",
                        maxQuantity: "4", minQuality: "1.2", maxQuality: "2.3", blueprint: "False", blueprintChance: "0.3");

                case "SupplyCrate_Level30_ScorchedEarth_C":
                    return new LootDrop(lootDropName, minLevelBP: 41, maxLevelBP: 53, minNumItems: "1", maxNumItems: "3", setWeight: "500", minQuantity: "2",
                        maxQuantity: "3", minQuality: "1.2", maxQuality: "2.3", blueprint: "False", blueprintChance: "0.3");
                case "SupplyCrate_Level30_Double_ScorchedEarth_C":
                    return new LootDrop(lootDropName, minLevelBP: 41, maxLevelBP: 53, minNumItems: "2", maxNumItems: "4", setWeight: "500", minQuantity: "3",
                        maxQuantity: "4", minQuality: "1.2", maxQuality: "2.3", blueprint: "False", blueprintChance: "0.3");

                case "SupplyCrate_Level35_C":
                    return new LootDrop(lootDropName, minLevelBP: 51, maxLevelBP: 64, minNumItems: "1", maxNumItems: "3", setWeight: "500", minQuantity: "2",
                        maxQuantity: "3", minQuality: "1.2", maxQuality: "2.3", blueprint: "False", blueprintChance: "0.3");
                case "SupplyCrate_Level35_Double_C":
                    return new LootDrop(lootDropName, minLevelBP: 51, maxLevelBP: 64, minNumItems: "2", maxNumItems: "4", setWeight: "500", minQuantity: "3",
                        maxQuantity: "4", minQuality: "1.2", maxQuality: "2.3", blueprint: "False", blueprintChance: "0.3");

                case "SupplyCrate_Level45_C":
                    return new LootDrop(lootDropName, minLevelBP: 65, maxLevelBP: 79, minNumItems: "1", maxNumItems: "3", setWeight: "500", minQuantity: "2",
                        maxQuantity: "3", minQuality: "1.2", maxQuality: "2.3", blueprint: "False", blueprintChance: "0.3");
                case "SupplyCrate_Level45_Double_C":
                    return new LootDrop(lootDropName, minLevelBP: 65, maxLevelBP: 79, minNumItems: "2", maxNumItems: "4", setWeight: "500", minQuantity: "3",
                        maxQuantity: "4", minQuality: "1.2", maxQuality: "2.3", blueprint: "False", blueprintChance: "0.3");
                case "SupplyCrate_Level45_ScorchedEarth_C":
                    return new LootDrop(lootDropName, minLevelBP: 54, maxLevelBP: 67, minNumItems: "1", maxNumItems: "3", setWeight: "500", minQuantity: "2",
                        maxQuantity: "3", minQuality: "1.2", maxQuality: "2.3", blueprint: "False", blueprintChance: "0.3");
                case "SupplyCrate_Level45_Double_ScorchedEarth_C":
                    return new LootDrop(lootDropName, minLevelBP: 54, maxLevelBP: 67, minNumItems: "2", maxNumItems: "4", setWeight: "500", minQuantity: "3",
                        maxQuantity: "4", minQuality: "1.2", maxQuality: "2.3", blueprint: "False", blueprintChance: "0.3");

                case "SupplyCrate_Level55_ScorchedEarth_C":
                    return new LootDrop(lootDropName, minLevelBP: 68, maxLevelBP: 82, minNumItems: "1", maxNumItems: "3", setWeight: "500", minQuantity: "2",
                        maxQuantity: "3", minQuality: "1.2", maxQuality: "2.3", blueprint: "False", blueprintChance: "0.3");
                case "SupplyCrate_Level55_Double_ScorchedEarth_C":
                    return new LootDrop(lootDropName, minLevelBP: 68, maxLevelBP: 82, minNumItems: "2", maxNumItems: "4", setWeight: "500", minQuantity: "3",
                        maxQuantity: "4", minQuality: "1.2", maxQuality: "2.3", blueprint: "False", blueprintChance: "0.3");

                case "SupplyCrate_Level60_C":
                    return new LootDrop(lootDropName, minLevelBP: 80, maxLevelBP: 97, minNumItems: "1", maxNumItems: "3", setWeight: "500", minQuantity: "2",
                        maxQuantity: "3", minQuality: "1.2", maxQuality: "2.3", blueprint: "False", blueprintChance: "0.3");
                case "SupplyCrate_Level60_Double_C":
                    return new LootDrop(lootDropName, minLevelBP: 80, maxLevelBP: 97, minNumItems: "2", maxNumItems: "4", setWeight: "500", minQuantity: "3",
                        maxQuantity: "4", minQuality: "1.2", maxQuality: "2.3", blueprint: "False", blueprintChance: "0.3");

                case "SupplyCrate_Level70_ScorchedEarth_C":
                    return new LootDrop(lootDropName, minLevelBP: 83, maxLevelBP: 97, minNumItems: "1", maxNumItems: "3", setWeight: "500", minQuantity: "2",
                        maxQuantity: "3", minQuality: "1.2", maxQuality: "2.3", blueprint: "False", blueprintChance: "0.3");
                case "SupplyCrate_Level70_Double_ScorchedEarth_C":
                    return new LootDrop(lootDropName, minLevelBP: 83, maxLevelBP: 97, minNumItems: "2", maxNumItems: "4", setWeight: "500", minQuantity: "3",
                        maxQuantity: "4", minQuality: "1.2", maxQuality: "2.3", blueprint: "False", blueprintChance: "0.3");


                default:
                    return new LootDrop(lootDropName, minLevelBP: 1, maxLevelBP: 1, minNumItems: "2", maxNumItems: "4", setWeight: "500", minQuantity: "1",
                        maxQuantity: "1", minQuality: "1.2", maxQuality: "2.3", blueprint: "False", blueprintChance: "0.3");
            }

        }
    }
}
