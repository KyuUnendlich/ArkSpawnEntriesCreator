using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.VisualBasic.FileIO;
using System.Globalization;
using static System.Net.Mime.MediaTypeNames;
using System.Text;
using System.Xml;


namespace ArkSpawnEntriesCreator
{
    class Program
    {
        const string Path = "C:/Users/matth/Desktop/ark.txt";

        static void Main(string[] args)
        {

            //LootDropAddition.AddLootToLootDropLevelFjordurGenesis2Loots();
            //LootDropAddition.AddLootToLootDrop5Split();

            //CreatureLister.ListCreatures();

            //EngramCleanup.EngramsVanillaHiderXY();
            //EngramCleanup.EngramsDateRemover();
            //EngramCleanup.EngramsVanillaRemoverX();

            //MainASE
            //ASE_Things.CreateDinoEntries();

            //ASE_Things.OldMethod();
            //ASE_Things.OldReduceMethod();
            //ASE_Things.CompareCSVFileEntries();
            //ASE_Things.CompareCSVFileEntriesAllValues();

            ExtractAdditionalNPCSpawnValues();
        }

        private static void ExtractAdditionalNPCSpawnValues()
        {
            //Input text is extracted TheNPCSpawnEntriesContainerAdditions from ModDataAsset / PrimalGameData

            const string path = "C:/Users/matth/Desktop/strings.txt";
            // Open the text file using a stream reader.
            using StreamReader reader = new(path);

            bool searchingStartAdditions = false;

            File.AppendAllText(Path, "Added Engrams List:");
            File.AppendAllText(Path, "\r\n");

            //Searching for beginning of additions
            while (!searchingStartAdditions)
            {
                string text = reader.ReadLine();

                //Write Engram Entries
                if (text.Contains("BlueprintGeneratedClass'EngramEntry"))
                {
                    int first_index = text.LastIndexOf(":") + 39;
                    int last_index = text.LastIndexOf(",") - 2;
                    string engramEntry = text.Substring(first_index, last_index - first_index);

                    File.AppendAllText(Path, engramEntry);
                    File.AppendAllText(Path, "\r\n");
                }

                //Searching for beginning of additions
                if (text.Contains("TheNPCSpawnEntriesContainerAdditions"))
                {
                    searchingStartAdditions = true;
                }
            }

            bool foundLimitLine = false;
            bool mainBP = false;
            bool subBPs = false;
            bool subBPWeights = false;
            bool NPCsPercentage = false;
            bool globalSpawnWeights = false;

            List<SpawnContainer> spawnContainers = new List<SpawnContainer>();
            int spawnContainerIndex = -1;
            int currentSpawnEntryinContainer = -1;

            while (!reader.EndOfStream)
            {
                string text = reader.ReadLine();

                //Starting New Container
                if (text.Contains("\"SpawnEntriesContainerClass\":"))
                {
                    foundLimitLine = false;
                    spawnContainerIndex++;
                    currentSpawnEntryinContainer = -1;

                    int first_index = text.LastIndexOf(":") + 2;
                    int last_index = text.LastIndexOf(",");
                    string containerName = text.Substring(first_index, last_index - first_index);

                    spawnContainers.Add(new SpawnContainer(containerName));
                }

                if (!foundLimitLine)
                {

                    //Looking for used entry name in Container
                    if (text.Contains("\"AnEntryName\":"))
                    {
                        currentSpawnEntryinContainer++;

                        int first_index = text.LastIndexOf(":") + 1;
                        int last_index = text.LastIndexOf(",") - 3;
                        string entryName = text.Substring(first_index + 2, last_index - first_index);

                        spawnContainers[spawnContainerIndex].spawnEntries.Add(new SpawnEntry(entryName));
                    }

                    //Looking for Main BP
                    if (text.Contains("NPCsToSpawn"))
                    {
                        mainBP = true;
                    }

                    // MainBP Logic
                    if (mainBP)
                    {
                        if (text.Contains("AssetPathName"))
                        {
                            mainBP = false;

                            int first_index = text.LastIndexOf(".") + 1;
                            int last_index = text.LastIndexOf(",") - 1;
                            string mainBP_ = text.Substring(first_index, last_index - first_index);

                            spawnContainers[spawnContainerIndex].spawnEntries[currentSpawnEntryinContainer].SetMainBP(mainBP_);
                        }
                    }

                    //If there are subclasses for this entry
                    if (text.Contains("ToClass"))
                    {
                        subBPs = true;
                    }

                    //SubBP Logic
                    if (subBPs)
                    {
                        if (text.Contains("AssetPathName"))
                        {
                            int first_index = text.LastIndexOf(".") + 1;
                            int last_index = text.LastIndexOf(",") - 1;
                            string subBP = text.Substring(first_index, last_index - first_index);

                            spawnContainers[spawnContainerIndex].spawnEntries[currentSpawnEntryinContainer].AddSubBP(subBP);
                        }

                        if (text.Contains("\"Weights\":"))
                        {
                            subBPWeights = true;
                        }

                        else if (subBPWeights)
                        {
                            if (text.Contains("]"))
                            {
                                subBPs = false;
                                subBPWeights = false;
                            }
                            else
                            {
                                string text_nospaces = text.Replace(" ", "").Replace(",", "");
                                spawnContainers[spawnContainerIndex].spawnEntries[currentSpawnEntryinContainer].AddSubBPWeight(text_nospaces);
                            }
                        }
                    }

                    //Multiple NPC Logic
                    if (text.Contains("NPCsToSpawnPercentageChance"))
                    {
                        NPCsPercentage = true;
                    }

                    //Write the Chances for each
                    else if (NPCsPercentage)
                    {
                        if (text.Contains("]"))
                        {
                            NPCsPercentage = false;
                        }
                        else
                        {
                            string text_nospaces = text.Replace(" ", "");
                            spawnContainers[spawnContainerIndex].spawnEntries[currentSpawnEntryinContainer].SetNPCsAmountChance(text_nospaces);
                        }
                    }

                    //Looking for EntryWeight
                    if (text.Contains("EntryWeight"))
                    {

                        int first_index = text.LastIndexOf(":") + 1;
                        int last_index = text.LastIndexOf(",") - 1;
                        string entryWeight = text.Substring(first_index + 1, last_index - first_index);

                        spawnContainers[spawnContainerIndex].spawnEntries[currentSpawnEntryinContainer].SetEntryWeight(entryWeight);
                    }
                }


                //Starting Limit Part
                if (text.Contains("AdditionalNPCSpawnLimits"))
                {
                    foundLimitLine = true;
                }


                if (foundLimitLine)
                {
                    if (text.Contains("AssetPathName"))
                    {
                        int first_index = text.LastIndexOf(".") + 1;
                        int last_index = text.LastIndexOf(",") - 1;
                        string maxPercBP = text.Substring(first_index, last_index - first_index);

                        //Find correct BP
                        foreach (SpawnEntry spawnEntry in spawnContainers[spawnContainerIndex].spawnEntries)
                        {
                            if (spawnEntry.mainBP.Equals(maxPercBP))
                            {
                                bool maxPercFound = false;
                                while (!maxPercFound)
                                {
                                    string text2 = reader.ReadLine();
                                    if (text2.Contains("MaxPercentageOfDesiredNumToAllow"))
                                    {
                                        int first_index2 = text2.LastIndexOf(":") + 2;
                                        string maxPerc = text2.Substring(first_index2, text2.Length - first_index2);
                                        spawnEntry.SetmaxPercentage(maxPerc);
                                        maxPercFound = true;
                                        break;
                                    }
                                }
                            }

                        }

                    }

                }

                //Cancel out, end reached
                if (text.Contains("GlobalNPCRandomSpawnClassWeights"))
                {
                    globalSpawnWeights = true;
                    break;
                }
            }

            bool searchForMainBP = false;
            bool searchForSubBPs = false;
            bool searchForSubBPWeights = false;
            int currentGlobalSpawnEntry = -1;

            List<SpawnEntry> globalEntries = new List<SpawnEntry>();

            while (globalSpawnWeights)
            {
                string text = reader.ReadLine();

                if (text.Contains("FromClass"))
                {
                    searchForMainBP = true;
                    searchForSubBPs = false;
                    currentGlobalSpawnEntry++;
                }

                //SubBP Logic
                if (searchForSubBPs)
                {
                    if (text.Contains("AssetPathName"))
                    {
                        int first_index = text.LastIndexOf(".") + 1;
                        int last_index = text.LastIndexOf(",") - 1;
                        string subBP = text.Substring(first_index, last_index - first_index);

                        globalEntries[currentGlobalSpawnEntry].AddSubBP(subBP);
                    }

                    if (text.Contains("\"Weights\":"))
                    {
                        searchForSubBPWeights = true;
                    }

                    else if (searchForSubBPWeights)
                    {
                        if (text.Contains("]"))
                        {
                            searchForSubBPs = false;
                            searchForSubBPWeights = false;
                        }
                        else
                        {
                            string text_nospaces = text.Replace(" ", "").Replace(",", "");
                            globalEntries[currentGlobalSpawnEntry].AddSubBPWeight(text_nospaces);
                        }
                    }
                }

                //Main BP Search (lower than SubBP, cause otherwise it would find this line again (and this is an easy fix))
                if (searchForMainBP)
                {
                    if (text.Contains("AssetPathName"))
                    {
                        int first_index = text.LastIndexOf(".") + 1;
                        int last_index = text.LastIndexOf(",") - 1;
                        string globalMain = text.Substring(first_index, last_index - first_index);

                        SpawnEntry spawnEntry = new SpawnEntry();
                        spawnEntry.SetMainBP(globalMain);

                        searchForMainBP = false;
                        searchForSubBPs = true;

                        globalEntries.Add(spawnEntry);
                    }
                }

                //We Out
                if (text.Contains("ServerExtraWorldSingletonActorClasses"))
                {
                    break;
                }
            }

            //DinoAdditionsPrint
            if (spawnContainers.Count != 0)
            {
                File.AppendAllText(Path, "\r\n");
                File.AppendAllText(Path, "Dino Additions: ");
                File.AppendAllText(Path, "\r\n");
                StringBuilder sb = new StringBuilder();

                foreach (SpawnContainer cont in spawnContainers)
                {
                    sb.AppendLine("Container Name: " + cont.name);
                    foreach (SpawnEntry entry in cont.spawnEntries)
                    {
                        sb.AppendLine("   Entry Name: " + entry.entryName);
                        sb.AppendLine("      Main BP: " + entry.mainBP);
                        sb.AppendLine("      Entry Weight: " + entry.entryWeight);
                        sb.AppendLine("      Spawn Limit: " + entry.maxPercentage);
                        sb.AppendLine("      Multi Spawn Chance: " + entry.amountToSpawnChance);
                        int lengthOfSubBPs = entry.subBPs.Count;
                        for (int i = 0; i < lengthOfSubBPs; i++)
                        {
                            sb.AppendLine("         SubBP: " + entry.subBPs[i] + " " + entry.subBPWeights[i]);
                        }
                    }
                }
                File.AppendAllText(Path, sb.ToString());
            }

            //GlobalEntryWeights
            if (globalEntries.Count != 0)
            {
                File.AppendAllText(Path, "\r\n");
                File.AppendAllText(Path, "Global Entry Weights: ");
                File.AppendAllText(Path, "\r\n");
                StringBuilder sb = new StringBuilder();
                foreach (SpawnEntry entry in globalEntries)
                {
                    sb.AppendLine("   Main BP: " + entry.mainBP);
                    int lengthOfSubBPs = entry.subBPs.Count;
                    for (int i = 0; i < lengthOfSubBPs; i++)
                    {
                        sb.AppendLine("      SubBP: " + entry.subBPs[i] + " " + entry.subBPWeights[i]);
                    }
                }
                File.AppendAllText(Path, sb.ToString());
            }

        }

        private static void FormatPrehistoricStringsStrings()
        {
            //strings from this table https://docs.google.com/spreadsheets/d/13w7AA2Ufcw4Zud9FuOAkNZ70coiuHzc9hvEBzEKgmGo/edit?gid=1123079697#gid=1123079697

            const string path = "C:/Users/matth/Desktop/strings.txt";
            try
            {
                // Open the text file using a stream reader.
                using StreamReader reader = new(path);


                while (!reader.EndOfStream)
                {
                    // Read the stream as a string.
                    string text = reader.ReadLine();

                    int length = text.Length;
                    int indexStart = text.IndexOf('/');
                    text = text.Substring(indexStart, length - 27);

                    int indexEnd = text.IndexOf('\'');
                    text = text.Substring(0, indexEnd);

                    text += "_C";

                    File.AppendAllText(Path, text);
                    File.AppendAllText(Path, "\r\n");
                }
            }
            catch (IOException e)
            {
                Console.WriteLine("The file could not be read:");
                Console.WriteLine(e.Message);
            }
        }
    }

    public struct SpawnContainer
    {
        public List<SpawnEntry> spawnEntries;
        public string name;

        public SpawnContainer(string name) : this()
        {
            this.name = name;
            this.spawnEntries = new List<SpawnEntry>();
        }
    }

    public class SpawnEntry
    {
        public string entryName;
        public string mainBP;
        public List<string> subBPs;
        public List<string> subBPWeights;
        public string amountToSpawnChance;
        public string entryWeight;
        public string maxPercentage;

        public SpawnEntry(string entryName)
        {
            this.entryName = entryName;
            this.subBPs = new List<string>();
            this.subBPWeights = new List<string>();
            this.mainBP = "";
            this.amountToSpawnChance = "";
            this.entryWeight = "";
            this.maxPercentage = "";
        }

        public SpawnEntry()
        {
            this.entryName = "";
            this.subBPs = new List<string>();
            this.subBPWeights = new List<string>();
            this.mainBP = "";
            this.amountToSpawnChance = "";
            this.entryWeight = "";
            this.maxPercentage = "";
        }

        public void AddSubBP(string subBP) {
            this.subBPs.Add(subBP);
        }

        public void AddSubBPWeight(string subBPweight)
        {
            this.subBPWeights.Add(subBPweight);
        }

        public void SetNPCsAmountChance(string amount)
        {
            this.amountToSpawnChance += amount;
        }
        public void SetMainBP(string bp)
        {
            this.mainBP = bp;
        }
        public void SetEntryWeight(string EW)
        {
            this.entryWeight = EW;
        }

        public void SetmaxPercentage(string MP)
        {
            this.maxPercentage = MP;
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
