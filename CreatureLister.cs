using Microsoft.VisualBasic.FileIO;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.IO;
using System.Text;

namespace ArkSpawnEntriesCreator
{
    class CreatureLister
    {
        const string Path = "C:/Users/matth/Desktop/arklist.txt";

        public static void ListCreatures()
        {
            var path = @"E:\ARK Saves\ArkSpawnEntriesCreator\ArkSpawnEntriesCreatureList.csv";
            using (TextFieldParser csvParser = new TextFieldParser(path))
            {
                csvParser.CommentTokens = new string[] { "#" };
                csvParser.SetDelimiters(new string[] { "," });
                csvParser.HasFieldsEnclosedInQuotes = true;

                // Read the first row with the dino names
                string[] dinoNameArray = csvParser.ReadFields();

                //List<String> illegalWords = new List<string> {"Tek","Aberrant", "X-", "R-"};
                List<String> illegalWords = new List<string> { "Tek"};
                List<String> acceptedEntries = new List<string>();
                List<String> deniedEntries = new List<string>();

                foreach (String dinoName in dinoNameArray) {
                    bool illegal = false;
                    for (int i = 0; i < illegalWords.Count; i++) {
                        if (dinoName.Contains(illegalWords[i])) { 
                            illegal = true;
                        }
                    }
                    if (illegal)
                    {
                        deniedEntries.Add(dinoName);
                    }
                    else {
                        acceptedEntries.Add(dinoName);
                    }
                }

                File.AppendAllText(Path, "acceptedEntries:");
                File.AppendAllText(Path, "\r\n");

                foreach (String accEntry in acceptedEntries) {
                    File.AppendAllText(Path, accEntry);
                    File.AppendAllText(Path, "\r\n");
                }

                File.AppendAllText(Path, "\r\n"); 

                File.AppendAllText(Path, "deniedEntries:");
                File.AppendAllText(Path, "\r\n");

                foreach (String debniedEntry in deniedEntries)
                {
                    File.AppendAllText(Path, debniedEntry);
                    File.AppendAllText(Path, "\r\n");
                }
            }
        }
    }
}
