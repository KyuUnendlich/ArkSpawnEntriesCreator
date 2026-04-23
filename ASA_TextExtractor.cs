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
        public static void ExtractEntryNames(string path_strings, string Path)
        {
            using StreamReader reader = new(path_strings);

            List<string> entryNames = new List<string>();

            while (!reader.EndOfStream)
            {
                string text = reader.ReadLine();

                //Looking for entry names
                if (text.Contains("\"AnEntryName\":"))
                {
                    int first_index = text.LastIndexOf(":") + 1;
                    int last_index = text.LastIndexOf(",") - 3;
                    string entryName = text.Substring(first_index + 2, last_index - first_index);

                    entryNames.Add(entryName);
                }
            }

            string[] entryNameArray = entryNames.ToArray();
            HelperFunctions.selectionSort(entryNameArray);

            Dictionary<string, int> engramNameCount = new Dictionary<string, int>();

            List<string> entryNameListEveryoneOnce = new List<string>();

            foreach (string entryName in entryNameArray)
            {
                engramNameCount.TryAdd(entryName, 0);
                if (engramNameCount[entryName].Equals(0))
                {
                    entryNameListEveryoneOnce.Add(entryName);
                }
                engramNameCount[entryName]++;
            }

            StringBuilder sb_entryNames = new StringBuilder();

            foreach (string entryName in entryNameListEveryoneOnce) {
                string entryNameIt = entryName;
                if (entryName.Equals("")) {
                    entryNameIt = "NULL";
                }
                sb_entryNames.AppendLine("   " + entryNameIt + "  x " + engramNameCount[entryName]);
            }



            if (entryNames.Count > 0)
            {
                File.AppendAllText(Path, "All Entry Names: ");
                File.AppendAllText(Path, "\r\n");
                File.AppendAllText(Path, sb_entryNames.ToString());
                File.AppendAllText(Path, "\r\n");
            }
        }

        public static void ExtractEngrams(string path_strings, string Path)
        {
            using StreamReader reader = new(path_strings);

            bool readingEngramEntries = false;

            //Whats the first thing to find, EngramEntries, DinoEntries, DinoAdditions
            while (!reader.EndOfStream && !readingEngramEntries)
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
                File.AppendAllText(Path, "Engram Entries: ");
                File.AppendAllText(Path, "\r\n");
                File.AppendAllText(Path, sb_engramEntries.ToString());
                File.AppendAllText(Path, "\r\n");
            }
        }

        public static void ExtractDino(string path_strings, string Path)
        {
            using StreamReader reader = new(path_strings);

            bool readingDinoEntries = false;

            //Whats the first thing to find, EngramEntries, DinoEntries, DinoAdditions
            while (!reader.EndOfStream && !readingDinoEntries)
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
                File.AppendAllText(Path, "Dino Entries: ");
                File.AppendAllText(Path, "\r\n");
                File.AppendAllText(Path, sb_dinoEntries.ToString());
                File.AppendAllText(Path, "\r\n");
            }
        }

        public static void ExtractRemaps(string path_strings, string Path)
        {

            using StreamReader reader = new(path_strings);

            bool readingRemaps = false;

            while (!reader.EndOfStream && !readingRemaps)
            {
                string text = reader.ReadLine();

                if (text.Contains("Remap_NPC") || text.Contains("RemapAdditions\":") || text.Contains("GlobalNPCRandomPaleoSpawnClassWeights\": ["))
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

                if (!weights && text.Contains("\"Weights"))
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
                        int first_index = text.LastIndexOf(":") + 3;
                        int last_index = text.LastIndexOf(",") - 1;
                        string mainBP_ = text.Substring(first_index, last_index - first_index);

                        sb_remaps.AppendLine("      " + mainBP_);
                    }
                }

                //Searching for beginning of additions
                if (text.Contains("TheNPCSpawnEntriesContainerAdditions") || text.Contains("GlobalNPCRandomSpawnClassWeights") || 
                    text.Contains("ClassLoadedNPCRandomReplacements\": [") || text.Contains("\"AdditionalStructureEngrams\": [") || 
                    text.Contains("Remap_Engrams"))
                {
                    readingRemaps = false;
                }
            }

            if (foundRemaps)
            {
                File.AppendAllText(Path, "ALERT ALERT ALERT ALERT ALERT ALERT ALERT ALERT ALERT ALERT Remaps: ");
                File.AppendAllText(Path, "\r\n");
                File.AppendAllText(Path, sb_remaps.ToString());
                File.AppendAllText(Path, "\r\n");
            }
        }

        public static void ExtractRemappedEngrams(string path_strings, string Path)
        {

            using StreamReader reader = new(path_strings);

            bool readingRemappedEngrams = false;

            while (!reader.EndOfStream && !readingRemappedEngrams)
            {
                string text = reader.ReadLine();

                if (text.Contains("Remap_Engrams"))
                {
                    readingRemappedEngrams = true;
                }
            }

            StringBuilder sb_remappedEngrams = new StringBuilder();
            bool foundRemaps = false;

            bool fromPart = false;
            bool toPart = false;

            while (!reader.EndOfStream && readingRemappedEngrams)
            {
                foundRemaps = true;

                string text = reader.ReadLine();

                if (text.Contains("FromClass"))
                {
                    sb_remappedEngrams.AppendLine("      ");
                    fromPart = true;
                    toPart = false;
                    sb_remappedEngrams.AppendLine("FromClass: ");
                }

                if (text.Contains("ToClass"))
                {
                    fromPart = false;
                    toPart = true;
                    sb_remappedEngrams.AppendLine("ToClass: ");
                }

                if (fromPart || toPart)
                {
                    if (text.Contains("AssetPathName"))
                    {
                        int first_index = text.LastIndexOf(":") + 3;
                        int last_index = text.LastIndexOf(",") - 1;
                        string mainBP_ = text.Substring(first_index, last_index - first_index);

                        sb_remappedEngrams.AppendLine("      " + mainBP_);
                    }
                }

                //Searching for beginning of additions
                if (text.Contains("TheNPCSpawnEntriesContainerAdditions") || text.Contains("GlobalNPCRandomSpawnClassWeights") || text.Contains("],"))
                {
                    readingRemappedEngrams = false;
                }
            }

            if (foundRemaps)
            {
                File.AppendAllText(Path, "ALERT ALERT ALERT ALERT ALERT ALERT ALERT ALERT ALERT ALERT Remapped Engrams: ");
                File.AppendAllText(Path, "\r\n");
                File.AppendAllText(Path, sb_remappedEngrams.ToString());
                File.AppendAllText(Path, "\r\n");
            }
        }

        public static List<SpawnContainer> ExtractDinoAdditions(string path_strings)
        {

            using StreamReader reader = new(path_strings);
            bool readingDinoAdditions = false;

            bool foundLimitLine = false;
            bool mainBP = false;
            bool subBPFrom = false;
            bool subBPTo = false;
            bool subBPsameAsMainBP = false;
            bool subBPWeights = false;
            bool NPCsPercentage = false;

            while (!reader.EndOfStream && !readingDinoAdditions)
            {
                string text = reader.ReadLine();

                //Searching for beginning of additions
                if (text.Contains("TheNPCSpawnEntriesContainerAdditions") || text.Contains("AdditionalSpawns")) //Second one is just for Atlas Fish lol.
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
                    subBPsameAsMainBP = false;
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

                        int first_index = text.IndexOf(":") + 1;
                        int last_index = text.LastIndexOf(",") - 3;
                        string entryName = text.Substring(first_index + 2, last_index - first_index);

                        spawnContainers[spawnContainerIndex].spawnEntries.Add(new SpawnEntry(entryName));
                    }

                    //Looking for Main BP
                    if (text.Contains("NPCsToSpawn\":"))
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
                    if (text.Contains("FromClass"))
                    {
                        subBPFrom = true;
                        continue;
                    }

                    //If there are subclasses for this entry
                    if (subBPFrom)
                    {
                        if (text.Contains("AssetPathName"))
                        {
                            int first_index = text.LastIndexOf(".") + 1;
                            int last_index = text.LastIndexOf(",") - 1;
                            string subBP = text.Substring(first_index, last_index - first_index);

                            if (spawnContainers[spawnContainerIndex].spawnEntries[currentSpawnEntryinContainer].mainBP.Equals(subBP))
                            {
                                subBPsameAsMainBP = true;
                            }
                        }

                        if (text.Contains("ToClasses\":"))
                        {
                            subBPFrom = false;
                            subBPTo = true;
                            continue;
                        }
                        
                    }

                    //SubBP Logic
                    if (subBPTo)
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
                                subBPTo = false;
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
            }

            return spawnContainers;
        }

        public static List<SpawnEntry> ExtractGlobalReplacement(string path_strings, string Path) {
            bool searchForMainBP = false;
            bool searchForSubBPs = false;
            bool searchForSubBPWeights = false;
            int currentGlobalSpawnEntry = -1;

            using StreamReader reader = new(path_strings);
            bool globalSpawnWeights = false;

            while (!reader.EndOfStream && !globalSpawnWeights)
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

                /*
                //We Out
                if (text.Contains("ServerExtraWorldSingletonActorClasses"))
                {
                    break;
                }*/

                //SpawnReplacementsFound
            }

            return globalEntries;
        }

        public static void DetectSpawnReplacements(string path_strings, string Path) {

            using StreamReader reader = new(path_strings);
            while (!reader.EndOfStream)
            {
                string text = reader.ReadLine();
                //SpawnReplacementsFound
                if (text.Contains("SpawnReplacements"))
                {
                    File.AppendAllText(Path, "ALERT ALERT ALERT ALERT ALERT ALERT ALERT ALERT ALERT ALERT SpawnReplacements found, check original log");
                    File.AppendAllText(Path, "\r\n");
                    File.AppendAllText(Path, "\r\n");
                    break;
                }
            }
        }

        public static void CreateDinoAdditionsSB(List<SpawnContainer> spawnContainers, string Path) {
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
        }

        public static void CreateGlobalAdditionsSB(List<SpawnEntry> globalEntries, string Path) {
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

        public static void CreateMultiParamSummary(List<SpawnContainer> spawnContainers, string Path) {
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
                File.AppendAllText(Path, sb.ToString());
            }
        }

        public static List<SpawnContainerMulti> ExtractMultiMainDinoAdditions(string path_strings) {
            using StreamReader reader = new(path_strings);
            bool readingDinoAdditions = false;

            bool foundLimitLine = false;
            bool mainBP = false;
            bool subBPs = false;
            bool subBPWeights = false;
            bool NPCsPercentage = false;

            while (!reader.EndOfStream && !readingDinoAdditions)
            {
                string text = reader.ReadLine();

                //Searching for beginning of additions
                if (text.Contains("TheNPCSpawnEntriesContainerAdditions"))
                {
                    readingDinoAdditions = true;
                }
            }

            List<SpawnContainerMulti> spawnContainers = new List<SpawnContainerMulti>();
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

                    spawnContainers.Add(new SpawnContainerMulti(containerName));
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

                        spawnContainers[spawnContainerIndex].spawnEntries.Add(new SpawnEntryMulti(entryName));
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
                            //mainBP = false;

                            int first_index = text.LastIndexOf(".") + 1;
                            int last_index = text.LastIndexOf(",") - 1;
                            string mainBP_ = text.Substring(first_index, last_index - first_index);

                            spawnContainers[spawnContainerIndex].spawnEntries[currentSpawnEntryinContainer].mainBPs.Add(mainBP_);
                        }
                    }

                    //If there are subclasses for this entry
                    if (text.Contains("ToClass"))
                    {
                        subBPs = true;
                        mainBP = false;
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
                        mainBP = false;
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
                        foreach (SpawnEntryMulti spawnEntry in spawnContainers[spawnContainerIndex].spawnEntries)
                        {
                            List<string> mainBPList = spawnEntry.mainBPs;
                            List<string> maxPercentagesList = spawnEntry.maxPercentages;
                            for (int currentMainBP = 0; currentMainBP < mainBPList.Count; currentMainBP++)
                            {
                                if (mainBPList[currentMainBP].Equals(maxPercBP))
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
                                                maxPercentagesList[currentMainBP] = maxPerc;
                                                break;
                                            }

                                            //End of MaxPercentages
                                            if (text2.Contains("]"))
                                            {
                                                maxPercFound = true;
                                                goto searchloop;
                                            }
                                        }
                                        else
                                        {
                                            maxPercFound = true;
                                        }
                                    }
                                }
                                //End of MaxPercentages
                                if (text.Contains("]"))
                                {
                                    break;
                                }
                            }
                        }
                    }
                searchloop: string a;
                }
            }
            return spawnContainers;
        }

        public static void CreateDinoAdditionsSBMulti(List<SpawnContainerMulti> spawnContainers, string Path) {

            //DinoAdditionsPrint
            if (spawnContainers.Count != 0)
            {
                File.AppendAllText(Path, "Dino Additions: ");
                File.AppendAllText(Path, "\r\n");
                StringBuilder sb = new StringBuilder();

                foreach (SpawnContainerMulti cont in spawnContainers)
                {
                    sb.AppendLine("Container Name: " + cont.name);
                    foreach (SpawnEntryMulti entry in cont.spawnEntries)
                    {
                        List<string> mainBPList = entry.mainBPs;
                        List<string> maxPercentagesList = entry.maxPercentages;
                        for (int currentMainBP = 0; currentMainBP < mainBPList.Count; currentMainBP++)
                        {
                            sb.AppendLine("   Entry Name: " + entry.entryName);
                            sb.AppendLine("      Main BP: " + mainBPList[currentMainBP]);
                            sb.AppendLine("      Entry Weight: " + entry.entryWeight);
                            if (currentMainBP < maxPercentagesList.Count)
                            {
                                if (maxPercentagesList[currentMainBP].Contains("Default"))
                                {
                                    sb.AppendLine("      Spawn Limit: " + "Error, wrong BP or couldnt be found");
                                }
                                else
                                {
                                    sb.AppendLine("      Spawn Limit: " + maxPercentagesList[currentMainBP]);
                                }
                            }
                            sb.AppendLine("      Multi Spawn Chance: " + entry.amountToSpawnChance);
                            int lengthOfSubBPs = entry.subBPs.Count;
                            int lengthOfSubBPWeights = entry.subBPWeights.Count;
                            for (int currentSubBP = 0; currentSubBP < lengthOfSubBPs; currentSubBP++)
                            {
                                string subBPWeighttemp = "doesnt exist";
                                if (currentSubBP < lengthOfSubBPWeights)
                                {
                                    subBPWeighttemp = entry.subBPWeights[currentSubBP];
                                }
                                sb.AppendLine("         SubBP: " + entry.subBPs[currentSubBP] + " " + subBPWeighttemp);
                            }
                        }
                    }
                }
                File.AppendAllText(Path, sb.ToString());
            } 
        }

        public static void CreateMultiParamSummaryMulti (List<SpawnContainerMulti> spawnContainers, string Path)
        {

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
                Dictionary<string, List<List<string>>> perBP_SubBPs = new Dictionary<string, List<List<string>>>();
                Dictionary<string, List<List<string>>> perBP_SubBPsWeights = new Dictionary<string, List<List<string>>>();

                foreach (SpawnContainerMulti cont in spawnContainers)
                {
                    foreach (SpawnEntryMulti entry in cont.spawnEntries)
                    {
                        List<string> mainBPList = entry.mainBPs;
                        for (int i = 0; i < mainBPList.Count; i++)
                        {
                            perBP_EntryWeights.TryAdd(mainBPList[i], new List<string>());
                            perBP_SpawnLimits.TryAdd(mainBPList[i], new List<string>());
                            perBP_MultiParams.TryAdd(mainBPList[i], new List<string>());
                            perBP_SubBPs.TryAdd(mainBPList[i], new List<List<string>>());
                            perBP_SubBPsWeights.TryAdd(mainBPList[i], new List<List<string>>());
                        }
                    }
                }

                foreach (SpawnContainerMulti cont in spawnContainers)
                {
                    foreach (SpawnEntryMulti entry in cont.spawnEntries)
                    {
                        List<string> mainBPList = entry.mainBPs;
                        List<string> maxPercentagesList = entry.maxPercentages;
                        for (int i = 0; i < mainBPList.Count; i++)
                        {
                            perBP_EntryWeights[mainBPList[i]].Add(entry.entryWeight);
                            if (i < maxPercentagesList.Count)
                            {
                                perBP_SpawnLimits[mainBPList[i]].Add(maxPercentagesList[i]);
                            }
                            perBP_MultiParams[mainBPList[i]].Add(entry.amountToSpawnChance);
                            perBP_SubBPs[mainBPList[i]].Add(entry.subBPs);
                            perBP_SubBPsWeights[mainBPList[i]].Add(entry.subBPWeights);
                        }
                    }
                }

                var listofAllMainBPS = perBP_MultiParams.Keys;

                string[] mainBPArray = new string[listofAllMainBPS.Count];
                listofAllMainBPS.CopyTo(mainBPArray, 0);

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
                        if (limitList[i].Contains("Default"))
                        {
                            sb.AppendLine("      Spawn Limit: " + "             Error, wrong BP or couldnt be found");
                        }
                        else
                        {
                            sb.AppendLine("      Spawn Limit:                             " + limitList[i]);
                        }
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
                        int lengthOfSubBPWeights = currentSubBPWeightList.Count;

                        for (int j = 0; j < currentSubBPList.Count; j++)
                        {
                            if (j != 0)
                            {
                                subBPString += ", ";
                            }
                            subBPString += currentSubBPList[j] + ": ";

                            string subBPWeighttemp = "doesnt exist";
                            if (j < lengthOfSubBPWeights)
                            {
                                subBPWeighttemp = currentSubBPWeightList[j];
                            }
                            subBPString += subBPWeighttemp;
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
    }
}
