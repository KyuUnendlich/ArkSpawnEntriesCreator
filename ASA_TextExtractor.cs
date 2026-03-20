using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection.PortableExecutable;
using System.Text;
using static System.Net.Mime.MediaTypeNames;

namespace ArkSpawnEntriesCreator
{
    class ASA_TextExtractor
    {
        public static void ExtractEngrams(string path)
        {
            using StreamReader reader = new(path);

            bool readingEngramEntries = false;

            //Whats the first thing to find, EngramEntries, DinoEntries, DinoAdditions
            while (!reader.EndOfStream)
            {
                string text = reader.ReadLine();

                if (text.Contains("AdditionalEngramBlueprintClasses"))
                {
                    readingEngramEntries = true;
                }
            }

            StringBuilder sb_engramEntries = new StringBuilder();
            bool foundEngramEntry = false;

            //Searching through engram entries
            while (readingEngramEntries && !reader.EndOfStream)
            {
                string text = reader.ReadLine();

                //Write Engram Entries
                if (text.Contains("BlueprintGeneratedClass"))
                {
                    int first_index = text.LastIndexOf(":") + 27;
                    int last_index = text.LastIndexOf(",") - 2;
                    string engramEntry = text.Substring(first_index, last_index - first_index);

                    sb_engramEntries.AppendLine("   " + engramEntry);
                    foundEngramEntry = true;
                }

                if (text.Contains("]"))
                {
                    readingEngramEntries = false;
                }
            }

            if (foundEngramEntry)
            {
                File.AppendAllText(path, "Engram Entries: ");
                File.AppendAllText(path, "\r\n");
                File.AppendAllText(path, sb_engramEntries.ToString());
                File.AppendAllText(path, "\r\n");
            }
        }

        public static void ExtractDino(string path)
        {
            using StreamReader reader = new(path);

            bool readingDinoEntries = false;

            //Whats the first thing to find, EngramEntries, DinoEntries, DinoAdditions
            while (!reader.EndOfStream)
            {
                string text = reader.ReadLine();

                if (text.Contains("AdditionalDinoEntries"))
                {
                    readingDinoEntries = true;
                }
            }

            StringBuilder sb_dinoEntries = new StringBuilder();
            bool foundDinoEntry = false;

            //Searching through dino entries
            while (readingDinoEntries && !reader.EndOfStream)
            {
                string text = reader.ReadLine();

                //Write Dino Entries
                if (text.Contains("BlueprintGeneratedClass"))
                {
                    int first_index = text.LastIndexOf(":") + 37;
                    int last_index = text.LastIndexOf(",") - 2;
                    string dinoEntry = text.Substring(first_index, last_index - first_index);

                    sb_dinoEntries.AppendLine("   " + dinoEntry);
                    foundDinoEntry = true;
                }

                if (text.Contains("]"))
                {
                    readingDinoEntries = false;
                }
            }

            if (foundDinoEntry)
            {
                File.AppendAllText(path, "Dino Entries: ");
                File.AppendAllText(path, "\r\n");
                File.AppendAllText(path, sb_dinoEntries.ToString());
                File.AppendAllText(path, "\r\n");
            }
        }

        public static void ExtractRemaps(string path)
        {

            using StreamReader reader = new(path);

            bool readingRemaps = false;

            while (!reader.EndOfStream)
            {
                string text = reader.ReadLine();

                if (text.Contains("Remap_NPC"))
                {
                    readingRemaps = true;
                }
            }

            StringBuilder sb_remaps = new StringBuilder();
            bool foundRemaps = false;

            bool fromPart = false;
            bool toPart = false;
            bool weights = false;
            string weightsTemp = "";

            while (!reader.EndOfStream && readingRemaps)
            {
                foundRemaps = true;

                string text = reader.ReadLine();

                if (text.Contains("FromClass"))
                {
                    sb_remaps.AppendLine("      ");
                    fromPart = true;
                    toPart = false;
                    sb_remaps.AppendLine("FromClass: ");
                }

                if (text.Contains("ToClass"))
                {
                    fromPart = false;
                    toPart = true;
                    sb_remaps.AppendLine("ToClass: ");
                }

                if (weights)
                {
                    weightsTemp += text.Replace(" ", "") + " ";
                }

                if (!weights && text.Contains("\"Weights\": ["))
                {
                    weights = true;
                }

                if (weights && text.Contains("],"))
                {
                    string weightsToPrint = weightsTemp.Substring(0, weightsTemp.Length - 4);
                    sb_remaps.AppendLine("      " + weightsToPrint);
                    weights = false;
                    weightsTemp = "";
                }

                if (fromPart || toPart)
                {
                    if (text.Contains("AssetPathName"))
                    {
                        int first_index = text.LastIndexOf(".") + 1;
                        int last_index = text.LastIndexOf(",") - 1;
                        string mainBP_ = text.Substring(first_index, last_index - first_index);

                        sb_remaps.AppendLine("      " + mainBP_);
                    }
                }

                //Searching for beginning of additions
                if (text.Contains("TheNPCSpawnEntriesContainerAdditions") || text.Contains("GlobalNPCRandomSpawnClassWeights"))
                {
                    readingRemaps = false;
                }
            }

            if (foundRemaps)
            {
                File.AppendAllText(path, "ALERT ALERT ALERT ALERT ALERT ALERT ALERT ALERT ALERT ALERT Remaps: ");
                File.AppendAllText(path, "\r\n");
                File.AppendAllText(path, sb_remaps.ToString());
                File.AppendAllText(path, "\r\n");
            }
        }

        public static List<SpawnContainer> ExtractDinoAdditions(string path)
        {

            using StreamReader reader = new(path);
            bool readingDinoAdditions = false;

            bool foundLimitLine = false;
            bool mainBP = false;
            bool subBPs = false;
            bool subBPWeights = false;
            bool NPCsPercentage = false;

            while (!reader.EndOfStream)
            {
                string text = reader.ReadLine();

                //Searching for beginning of additions
                if (text.Contains("TheNPCSpawnEntriesContainerAdditions"))
                {
                    readingDinoAdditions = true;
                }
            }

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
                                    else
                                    {
                                        maxPercFound = true;
                                    }
                                }
                            }

                        }

                    }

                }
                /*
                //Cancel out, end reached
                if (text.Contains("GlobalNPCRandomSpawnClassWeights"))
                {
                    readingDinoAdditions = false;
                    break;
                }
                */
            }

            return spawnContainers;
        }

        public static List<SpawnEntry> ExtractGlobalReplacement(string path) {
            bool searchForMainBP = false;
            bool searchForSubBPs = false;
            bool searchForSubBPWeights = false;
            int currentGlobalSpawnEntry = -1;

            using StreamReader reader = new(path);
            bool globalSpawnWeights = false;

            while (!reader.EndOfStream)
            {
                string text = reader.ReadLine();
                if (text.Contains("GlobalNPCRandomSpawnClassWeights"))
                {
                    globalSpawnWeights = true;
                    break;
                }
            }

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

                //SpawnReplacementsFound
                if (text.Contains("SpawnReplacements"))
                {
                    File.AppendAllText(path, "ALERT ALERT ALERT ALERT ALERT ALERT ALERT ALERT ALERT ALERT SpawnReplacements");
                    File.AppendAllText(path, "\r\n");
                    File.AppendAllText(path, "\r\n");
                }
            }

            return globalEntries;
        }

        public static void CreateDinoAdditionsSB (List<SpawnContainer> spawnContainers, string path) {
            //DinoAdditionsPrint
            if (spawnContainers.Count != 0)
            {
                File.AppendAllText(path, "Dino Additions: ");
                File.AppendAllText(path, "\r\n");
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
                File.AppendAllText(path, sb.ToString());
            }
        }

        public static void CreateGlobalAdditionsSB(List<SpawnEntry> globalEntries, string path) {
            //GlobalEntryWeights
            if (globalEntries.Count != 0)
            {
                File.AppendAllText(path, "\r\n");
                File.AppendAllText(path, "Global Entry Weights: ");
                File.AppendAllText(path, "\r\n");
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
                File.AppendAllText(path, sb.ToString());
            }
        }

        public static void CreateMultiParamSummary(List<SpawnContainer> spawnContainers, string path) {
            //MultiParamsSummary
            if (spawnContainers.Count != 0)
            {
                File.AppendAllText(path, "\r\n");
                File.AppendAllText(path, "MultiParams Summary: ");
                File.AppendAllText(path, "\r\n");
                StringBuilder sb = new StringBuilder();

                Dictionary<string, List<string>> perBP_MultiParams = new Dictionary<string, List<string>>();
                Dictionary<string, List<string>> perBP_EntryWeights = new Dictionary<string, List<string>>();
                Dictionary<string, List<string>> perBP_SpawnLimits = new Dictionary<string, List<string>>();
                Dictionary<string, List<List<string>>> perBP_SubBPs = new Dictionary<string, List<List<string>>>();
                Dictionary<string, List<List<string>>> perBP_SubBPsWeights = new Dictionary<string, List<List<string>>>();

                foreach (SpawnContainer cont in spawnContainers)
                {
                    foreach (SpawnEntry entry in cont.spawnEntries)
                    {
                        perBP_EntryWeights.TryAdd(entry.mainBP, new List<string>());
                        perBP_SpawnLimits.TryAdd(entry.mainBP, new List<string>());
                        perBP_MultiParams.TryAdd(entry.mainBP, new List<string>());
                        perBP_SubBPs.TryAdd(entry.mainBP, new List<List<string>>());
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
                File.AppendAllText(path, sb.ToString());
            }
        }
    }
}
