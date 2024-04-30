using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.VisualBasic.FileIO;
using System.Globalization;

namespace ArkSpawnEntriesCreator
{
    class LootDropAddition
    {
        const string outputPath = "C:/Users/matth/Desktop/lootdrops.txt";

        const string csvPath = @"G:\ARK Saves\ArkSpawnEntriesCreator\LootDrops.csv";

        const string txtPath = @"G:\ARK Saves\ArkSpawnEntriesCreator\LootDropsBase.txt";

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

