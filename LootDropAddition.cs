using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.VisualBasic.FileIO;
using System.Globalization;
using System.Security.Cryptography.X509Certificates;

namespace ArkSpawnEntriesCreator
{
    class LootDropAddition
    {
        const string outputPath = "C:/Users/matth/Desktop/lootdrops.txt";

        const string outputPathCat = "C:/Users/matth/Desktop/lootdropscat.txt";

        const string csvPath = @"G:\ARK Saves\ArkSpawnEntriesCreator\LootDrops.csv";

        const string txtPath = @"G:\ARK Saves\ArkSpawnEntriesCreator\LootDropsBase.txt";

        const string txtPathFjo = @"G:\ARK Saves\ArkSpawnEntriesCreator\LootDropsFjordur.txt";

        const string middlepart_itemswights = "),ItemsWeights=(";

        public static void AddLootToLootDropLevel()
        {
            using (TextFieldParser csvParser = new TextFieldParser(csvPath))
            {
                // Added subcategory in beacon like this:
                // https://imgur.com/trJXySx
                // Subcategory is filled by adding blueprint names in " with , between multiple, then ),ItemsWeights=( then 10000,10000 for the same amount of blueprints
                // What comes before and after is stitched back at the end

                csvParser.CommentTokens = new string[] { "#" };
                csvParser.SetDelimiters(new string[] { "," });
                csvParser.HasFieldsEnclosedInQuotes = true;

                // Skip the rows with dino name / source
                csvParser.ReadLine();
                csvParser.ReadLine();
                // Read the row with the blueprint
                string[] loot_ids = csvParser.ReadFields();
                // Skip the row for special chars
                csvParser.ReadLine();
                string[] level_of_bp = csvParser.ReadFields();

                var lootTexts = new List<string>();

                using (var sr = new StreamReader(txtPathFjo))
                {
                    string line;
                    while ((line = sr.ReadLine()) != null)
                    {
                        lootTexts.Add(line);
                    }
                }

                //Divide csv blueprints into subcategories by level
                List<string> subcategories = GetSubcategoriesByLevel(loot_ids, level_of_bp);
                foreach (string category in subcategories) { 
                    
                }
               
                File.AppendAllText(outputPath, lootTexts[0] + subcategories[0] + lootTexts[1] + "\r\n");
                File.AppendAllText(outputPath, lootTexts[2] + subcategories[0] + lootTexts[3] + "\r\n");
                File.AppendAllText(outputPath, lootTexts[4] + subcategories[1] + lootTexts[5] + "\r\n");
                File.AppendAllText(outputPath, lootTexts[6] + subcategories[1] + lootTexts[7] + "\r\n");
                File.AppendAllText(outputPath, lootTexts[8] + subcategories[1] + lootTexts[9] + subcategories[2] + lootTexts[10] + "\r\n");
                File.AppendAllText(outputPath, lootTexts[11] + subcategories[1] + lootTexts[12] + subcategories[2] + lootTexts[13] + "\r\n");
                File.AppendAllText(outputPath, lootTexts[14] + subcategories[2] + lootTexts[15] + subcategories[3] + lootTexts[16] + "\r\n"); 
                File.AppendAllText(outputPath, lootTexts[17] + subcategories[2] + lootTexts[18] + subcategories[3] + lootTexts[19] + "\r\n");
                File.AppendAllText(outputPath, lootTexts[20] + subcategories[3] + lootTexts[21] + subcategories[4] + lootTexts[22] + "\r\n");
                File.AppendAllText(outputPath, lootTexts[23] + subcategories[3] + lootTexts[24] + subcategories[4] + lootTexts[25] + "\r\n");

            }
        }

        public static List<string> GetSubcategoriesByLevel(string[] loot_ids, string[] level_of_bp) {
            List<string> category1List = new List<string>();
            List<string> category2List = new List<string>();
            List<string> category3List = new List<string>();
            List<string> category4List = new List<string>();
            List<string> category5List = new List<string>();

            for (int i = 1; i < loot_ids.Length; i++)
            {
                int level = int.Parse(level_of_bp[i]);
                if (level > 79)
                {
                    category5List.Add(loot_ids[i]);
                }
                else if (level > 65)
                {
                    category4List.Add(loot_ids[i]);
                }
                else if (level > 45)
                {
                    category3List.Add(loot_ids[i]);
                }
                else if (level > 30)
                {
                    category2List.Add(loot_ids[i]);
                }
                else
                {
                    category1List.Add(loot_ids[i]);
                }
            }

            String category5 = "";
            String category4 = "";
            String category3 = "";
            String category2 = "";
            String category1 = "";
            String category5p2 = "";
            String category4p2 = "";
            String category3p2 = "";
            String category2p2 = "";
            String category1p2 = "";

            for (int i = 0; i < category5List.Count; i++)
            {
                if (i != 0)
                {
                    category5 += ",";
                    category5p2 += ",";
                }
                category5 += "\"" + category5List[i] + "\"";
                category5p2 += "10000";
            }

            for (int i = 0; i < category4List.Count; i++)
            {
                if (i != 0)
                {
                    category4 += ",";
                    category4p2 += ",";
                }
                category4 += "\"" + category4List[i] + "\"";
                category4p2 += "10000";
            }

            for (int i = 0; i < category3List.Count; i++)
            {
                if (i != 0)
                {
                    category3 += ",";
                    category3p2 += ",";
                }
                category3 += "\"" + category3List[i] + "\"";
                category3p2 += "10000";
            }

            for (int i = 0; i < category2List.Count; i++)
            {
                if (i != 0)
                {
                    category2 += ",";
                    category2p2 += ",";
                }
                category2 += "\"" + category2List[i] + "\"";
                category2p2 += "10000";
            }

            for (int i = 0; i < category1List.Count; i++)
            {
                if (i != 0)
                {
                    category1 += ",";
                    category1p2 += ",";

                }
                category1 += "\"" + category1List[i] + "\"";
                category1p2 += "10000";
            }

            category1 += middlepart_itemswights + category1p2;
            category2 += middlepart_itemswights + category2p2;
            category3 += middlepart_itemswights + category3p2;
            category4 += middlepart_itemswights + category4p2;
            category5 += middlepart_itemswights + category5p2;

            /*
            File.AppendAllText(outputPathCat, category1);
            File.AppendAllText(outputPathCat, "\r\n");
            File.AppendAllText(outputPathCat, category2);
            File.AppendAllText(outputPathCat, "\r\n");
            File.AppendAllText(outputPathCat, category3);
            File.AppendAllText(outputPathCat, "\r\n");
            File.AppendAllText(outputPathCat, category4);
            File.AppendAllText(outputPathCat, "\r\n");
            File.AppendAllText(outputPathCat, category5);
            File.AppendAllText(outputPathCat, "\r\n");
            File.AppendAllText(outputPathCat, ""+ category1List.Count+"  " + category2List.Count + "  " + category3List.Count + "  " + category4List.Count + "  " + category5List.Count);
            */

            List<string> subcat = new List<string>();
            subcat.Add(category1);
            subcat.Add(category2);
            subcat.Add(category3);
            subcat.Add(category4);
            subcat.Add(category5);

            return subcat;
        }


        public static void AddLootToLootDrop()
        {
            using (TextFieldParser csvParser = new TextFieldParser(csvPath))
            {
                csvParser.CommentTokens = new string[] { "#" };
                csvParser.SetDelimiters(new string[] { "," });
                csvParser.HasFieldsEnclosedInQuotes = true;

                // Skip the rows with dino name / source
                csvParser.ReadLine();
                csvParser.ReadLine();
                // Read the row with the blueprint
                string[] loot_ids = csvParser.ReadFields();
                // Skip the rows with the descriptions

                int counter = 0;
                // field 1: added loot for drop 1, field 2: weights for drop 1, field 3: added loot for drop 2 ...
                string[] addedLootArray = new string[16];

                var txtText = new List<string>();

                using (var sr = new StreamReader(txtPath))
                {
                    string line;
                    while ((line = sr.ReadLine()) != null)
                    {
                        txtText.Add(line);
                    }
                }

                while (!csvParser.EndOfData)
                {
                    string[] specialCaseArray = csvParser.ReadFields();

                    for (int i = 2; i < specialCaseArray.Length; i++)
                    {
                        if (specialCaseArray[i].Equals("x"))
                        {
                            // its a special case so skip it for now
                        }
                        else
                        {
                            counter++;
                            counter = counter % 8;

                            addedLootArray[counter * 2] += ",\"" + loot_ids[i] + "\"";
                            addedLootArray[counter * 2 + 1] += "10000,";
                        }
                    }
                }

                for (int i = 0; i < 8; i++) // adjust
                {
                    string outputText = txtText[i*3] + addedLootArray[i*2] + txtText[i*3+1] + addedLootArray[i*2 + 1] + txtText[i*3+2];

                    File.AppendAllText(outputPath, outputText);
                    File.AppendAllText(outputPath, "\r\n");
                }

            }
        }

    }
}

