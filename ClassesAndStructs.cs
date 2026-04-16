using System;
using System.Collections.Generic;
using System.Text;

namespace ArkSpawnEntriesCreator
{
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

    public struct SpawnContainerMulti
    {
        public List<SpawnEntryMulti> spawnEntries;
        public string name;

        public SpawnContainerMulti(string name) : this()
        {
            this.name = name;
            this.spawnEntries = new List<SpawnEntryMulti>();
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

        public void AddSubBP(string subBP)
        {
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

    public class SpawnEntryMulti
    {
        public string entryName;
        public List<string> mainBPs;
        public List<string> subBPs;
        public List<string> subBPWeights;
        public string amountToSpawnChance;
        public string entryWeight;
        public List<string> maxPercentages;

        public SpawnEntryMulti(string entryName)
        {
            this.entryName = entryName;
            subBPs = new List<string>();
            subBPWeights = new List<string>();
            mainBPs = new List<string>();
            amountToSpawnChance = "";
            entryWeight = "";
            maxPercentages = new List<string> {"Default1", "Default2", "Default3", "Default4", "Default5",
                "Default6", "Default7", "Default8", "Default9", "Default10", "Default11",
                "Default12", "Default13", "Default14", "Default15"};
        }

        public SpawnEntryMulti()
        {
            entryName = "";
            subBPs = new List<string>();
            subBPWeights = new List<string>();
            mainBPs = new List<string>();
            amountToSpawnChance = "";
            entryWeight = "";
            maxPercentages = new List<string> {"Default1", "Default2", "Default3", "Default4", "Default5",
                "Default6", "Default7", "Default8", "Default9", "Default10", "Default11",
                "Default12", "Default13", "Default14", "Default15"};
        }

        public void AddSubBP(string subBP)
        {
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

        public void SetEntryWeight(string EW)
        {
            this.entryWeight = EW;
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

    public struct ASADinoEntry
    {
        public string entryName;
        public string BP;
        public string entryweight;
        public string spawnlimit;
        public int multiParamAmount;
        public string multiParamLine;

        public ASADinoEntry(string entryName, string BP, string entryweight, string spawnlimit, int multiParamAmount, string multiParamLine)
        {
            this.entryName = entryName;
            this.BP = BP;
            this.entryweight = entryweight;
            this.spawnlimit = spawnlimit;
            this.multiParamAmount = multiParamAmount;
            this.multiParamLine = multiParamLine;
        }
    }

    public struct LootDrop
    {
        public string lootDropName;
        public int minLevelBP;
        public int maxLevelBP;
        public string minNumItems;
        public string maxNumItems;
        public string setWeight;
        public string minQuantity;
        public string maxQuantity;
        public string minQuality;
        public string maxQuality;
        public string blueprint;
        public string blueprintChance;

        public string line;
        public List<string> addedLootDropBPs;

        public LootDrop(string lootDropName, int minLevelBP, int maxLevelBP, string minNumItems, string maxNumItems, string setWeight, string minQuantity,
                        string maxQuantity, string minQuality, string maxQuality, string blueprint, string blueprintChance) { 
            this.lootDropName = lootDropName;
            this.minLevelBP = minLevelBP;
            this.maxLevelBP = maxLevelBP;
            this.minNumItems = minNumItems;
            this.maxNumItems = maxNumItems;
            this.setWeight = setWeight;
            this.minQuantity = minQuantity;
            this.maxQuantity = maxQuantity;
            this.minQuality = minQuality;
            this.maxQuality = maxQuality;
            this.blueprint = blueprint;
            this.blueprintChance = blueprintChance;

            addedLootDropBPs = new List<string>();
            this.line = "";
        }
    }
    public struct SpawnKnowledge
    {
        public string dinoName;
        public string spawnContainer;
        public string entryWeight;
        public string spawnLimit;

        public SpawnKnowledge(string dinoName, string spawnContainer, string entryWeight, string spawnLimit)
        {
            this.dinoName = dinoName;
            this.spawnContainer = spawnContainer;
            this.entryWeight = entryWeight;
            this.spawnLimit = spawnLimit;
        }
    }
}
