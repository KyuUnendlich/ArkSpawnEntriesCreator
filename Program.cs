using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.VisualBasic.FileIO;
using System.Globalization;
using static System.Net.Mime.MediaTypeNames;
using System.Text;
using System.Xml;
using System.Linq.Expressions;
using System.Diagnostics.Tracing;


namespace ArkSpawnEntriesCreator
{
    class Program
    {

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

            if (replaceFile) {
                File.Delete(Path);
            }

            const string path = "C:/Users/matth/Desktop/strings.txt";
            // Open the text file using a stream reader.
            using StreamReader reader = new(path);

            bool readingEngramEntries = false;
            bool readingDinoEntries = false;
            bool readingRemaps = false;
            bool readingDinoAdditions = false;

            //Whats the first thing to find, EngramEntries, DinoEntries, DinoAdditions
            while (!reader.EndOfStream && !readingEngramEntries && !readingDinoEntries && !readingDinoAdditions)
            {
                string text = reader.ReadLine();

                if (text.Contains("AdditionalEngramBlueprintClasses"))
                {
                    readingEngramEntries = true;
                }

                if (text.Contains("AdditionalDinoEntries"))
                {
                    readingEngramEntries = false;
                    readingDinoEntries = true;
                }

                if (text.Contains("Remap_NPC"))
                {
                    readingEngramEntries = false;
                    readingDinoEntries = false;
                    readingRemaps = true;
                }

                if (text.Contains("TheNPCSpawnEntriesContainerAdditions"))
                {
                    readingEngramEntries = false;
                    readingDinoEntries = false;
                    readingRemaps = false;
                    readingDinoAdditions = true;
                }
            }

            StringBuilder sb_engramEntries = new StringBuilder();
            bool foundEngramEntry = false;

            //Searching through engram entries
            while (readingEngramEntries && !reader.EndOfStream)
            {
                string text = reader.ReadLine();

                //Write Engram Entries
                if (text.Contains("BlueprintGeneratedClass'EngramEntry"))
                {
                    int first_index = text.LastIndexOf(":") + 27;
                    int last_index = text.LastIndexOf(",") - 2;
                    string engramEntry = text.Substring(first_index, last_index - first_index);

                    sb_engramEntries.AppendLine("   "+ engramEntry);
                    foundEngramEntry = true;
                }

                if (text.Contains("AdditionalDinoEntries")) {
                    readingDinoEntries = true;
                    readingEngramEntries = false;
                }

                //Searching for remaps
                if (text.Contains("Remap_NPC"))
                {
                    readingRemaps = true;
                    readingEngramEntries = false;
                }

                //Searching for beginning of additions
                if (text.Contains("TheNPCSpawnEntriesContainerAdditions"))
                {
                    readingDinoAdditions = true;
                    readingEngramEntries = false;
                }
            }

            if (foundEngramEntry)
            {
                File.AppendAllText(Path, "Engram Entries: ");
                File.AppendAllText(Path, "\r\n");
                File.AppendAllText(Path, sb_engramEntries.ToString());
                File.AppendAllText(Path, "\r\n");
            }


            StringBuilder sb_dinoEntries = new StringBuilder();
            bool foundDinoEntry = false;

            //Searching through dino entries
            while (readingDinoEntries && !reader.EndOfStream)
            {
                string text = reader.ReadLine();

                //Write Dino Entries
                if (text.Contains("BlueprintGeneratedClass'DinoEntry"))
                {
                    int first_index = text.LastIndexOf(":") + 37;
                    int last_index = text.LastIndexOf(",") - 2;
                    string dinoEntry = text.Substring(first_index, last_index - first_index);

                    sb_dinoEntries.AppendLine("   " + dinoEntry);
                    foundDinoEntry = true;
                }

                //Searching for remaps
                if (text.Contains("Remap_NPC"))
                {
                    readingRemaps = true;
                    readingDinoEntries = false;
                }

                //Searching for beginning of additions
                if (text.Contains("TheNPCSpawnEntriesContainerAdditions"))
                {
                    readingDinoAdditions = true;
                    readingDinoEntries = false;
                }

            }

            if (foundDinoEntry)
            {
                File.AppendAllText(Path, "Dino Entries: ");
                File.AppendAllText(Path, "\r\n");
                File.AppendAllText(Path, sb_dinoEntries.ToString());
                File.AppendAllText(Path, "\r\n");
            }

            StringBuilder sb_remaps = new StringBuilder();
            bool foundRemaps = false;

            while (!reader.EndOfStream && readingRemaps)
            {
                foundRemaps = true;
                string text = reader.ReadLine();

                //Write Remaps
                if (text.Contains("AssetPathName"))
                {
                    int first_index = 30;
                    int last_index = text.LastIndexOf(",") - 1;
                    string remapBP = text.Substring(first_index, last_index - first_index);

                    sb_remaps.AppendLine("   " + remapBP);
                }

                //Searching for beginning of additions
                if (text.Contains("TheNPCSpawnEntriesContainerAdditions"))
                {
                    readingRemaps = false;
                    readingDinoAdditions = true;
                }
            }

            if (foundRemaps)
            {
                File.AppendAllText(Path, "ALERT ALERT ALERT ALERT ALERT ALERT ALERT ALERT ALERT ALERT Remaps: ");
                File.AppendAllText(Path, "\r\n");
                File.AppendAllText(Path, sb_remaps.ToString());
                File.AppendAllText(Path, "\r\n");
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


            while (!reader.EndOfStream && readingDinoAdditions)
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
                            string text_nospaces = text.Replace(" ", "").Replace(",", "-");
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
                                    if (!reader.EndOfStream)
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
                                    else {
                                        maxPercFound = true;
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

            while (globalSpawnWeights && !reader.EndOfStream)
            {
                string text = reader.ReadLine();

                if (text.Contains("FromClass"))
                {
                    searchForMainBP = true;
                    searchForSubBPs = false;
                    currentGlobalSpawnEntry++;
                }

                //Global SubBP Logic 
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

                //Global Main BP Search (lower than SubBP, cause otherwise it would find this line again (and this is an easy fix))
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

            //MultiParamsSummary
            if (spawnContainers.Count != 0)
            {
                File.AppendAllText(Path, "\r\n");
                File.AppendAllText(Path, "MultiParams Summary: ");
                File.AppendAllText(Path, "\r\n");
                StringBuilder sb = new StringBuilder();

                Dictionary<string, List<string>> perBP_MultiParams = new Dictionary<string, List<string>>();
                Dictionary<string, List<string>> perBP_EntryWeights = new Dictionary<string, List<string>>();
                Dictionary<string, List<string>> perBP_SpawnLimits = new Dictionary<string, List<string>>();
                Dictionary<string, List<List<string>>> perBP_SubBPs = new Dictionary<string, List<List<string>>> ();
                Dictionary<string, List<List<string>>> perBP_SubBPsWeights = new Dictionary<string, List<List<string>>>();

                foreach (SpawnContainer cont in spawnContainers)
                {
                    foreach (SpawnEntry entry in cont.spawnEntries)
                    {
                        perBP_EntryWeights.TryAdd(entry.mainBP, new List<string>());
                        perBP_SpawnLimits.TryAdd(entry.mainBP, new List<string>());
                        perBP_MultiParams.TryAdd(entry.mainBP, new List<string>());
                        perBP_SubBPs.TryAdd(entry.mainBP, new List<List<string>> ());
                        perBP_SubBPsWeights.TryAdd(entry.mainBP, new List<List<string>>());

                    }
                }

                foreach (SpawnContainer cont in spawnContainers)
                {
                    foreach (SpawnEntry entry in cont.spawnEntries)
                    {
                        perBP_EntryWeights[entry.mainBP].Add(entry.entryWeight);
                        perBP_SpawnLimits[entry.mainBP].Add(entry.maxPercentage);
                        perBP_MultiParams[entry.mainBP].Add(entry.amountToSpawnChance);
                        perBP_SubBPs[entry.mainBP].Add(entry.subBPs);
                        perBP_SubBPsWeights[entry.mainBP].Add(entry.subBPWeights);
                    }
                }

                var mainBPList = perBP_MultiParams.Keys;

                string[] mainBPArray = new string[mainBPList.Count];
                mainBPList.CopyTo(mainBPArray, 0);

                HelperFunctions.selectionSort(mainBPArray);

                foreach (string key in mainBPArray)
                {
                    sb.AppendLine("   Main BP: " + key);

                    List<string> entryList = perBP_EntryWeights[key];
                    List<string> limitList = perBP_SpawnLimits[key];
                    List<string> multiList = perBP_MultiParams[key];
                    List<List<string>> subBPList = perBP_SubBPs[key];
                    List<List<string>> subBPWeightList = perBP_SubBPsWeights[key];


                    int amountEntries = entryList.Count;
                    int amountLimits = limitList.Count;
                    int amountMultis = multiList.Count;

                    //This shouldnt be possible
                    if (!amountEntries.Equals(amountLimits) || !amountEntries.Equals(amountMultis))
                    {
                        sb.AppendLine("   Error: Different Amount of Values for each List" + amountEntries + " " + amountLimits + " " + amountMultis);
                        amountEntries = Math.Min(Math.Min(amountEntries, amountMultis), amountLimits);
                    }

                    for (int i = 0; i < amountEntries; i++)
                    {
                        sb.AppendLine("      Entry Weight:                   " + entryList[i]);
                        sb.AppendLine("      Spawn Limit:                             " + limitList[i]);
                        sb.AppendLine("      Multi Spawn Chance:                               " + multiList[i]);
                    }

                    int amountSubBPs = subBPList.Count;
                    int amountSubBPWeights = subBPWeightList.Count;

                    //This shouldnt be possible
                    if (!amountSubBPs.Equals(amountSubBPWeights))
                    {
                        sb.AppendLine("   Error: Different Amount of Values for each Sub BP List" + amountSubBPs + " " + amountSubBPWeights);
                        amountSubBPs = Math.Min(amountSubBPs, amountSubBPWeights);
                    }

                    for (int i = 0; i < amountSubBPs; i++)
                    {

                        string subBPString = "";

                        List<string> currentSubBPList = subBPList[i];
                        List<string> currentSubBPWeightList = subBPWeightList[i];

                        for (int j = 0; j < currentSubBPList.Count; j++)
                        {
                            if (j != 0)
                            {
                                subBPString += ", ";
                            }
                            subBPString += currentSubBPList[j] + ": ";
                            subBPString += currentSubBPWeightList[j];
                        }

                        if (!subBPString.Equals(""))
                        {
                            sb.AppendLine("SubBPList: " + subBPString);
                        }
                    }
                    sb.AppendLine("");
                }
                File.AppendAllText(Path, sb.ToString());
            }
        }

        enum Mod
        {
            Prehistoric1,
            Prehistoric2,
            Prehistoric3,
            Prehistoric4,
            Prehistoric5,
            CyrusDrakonis,
            WakSpino,
            Hatze,
            ElementalRaptors,
            MoroHydrovanta,
            ShadAtlas,
            PortsOfAtlas,
            AtlasReborn,
            AtlasFish,
            MoroLivy,
            MoroGigantophis,
            SulfurTitan,
            Edmontonia,
            Anomalocaris,
            Cricosaurus,


        }

        //There is now a branch (multi-main-bp) for Mods that have one addition per container with multiple different dinos

        //const string Path = "C:/Users/matth/Desktop/Ascended/AtlasFish.txt";
        const bool replaceFile = true;
        const string Path = "E:/ARK Saves/ArkSpawnEntriesCreator/AscendedModsAdditions/MoroHydrovanta.txt";
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
