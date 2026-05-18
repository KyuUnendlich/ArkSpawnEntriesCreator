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
using System.Xml.Linq;


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

            //OldASAMethods.ExtractAdditionalNPCSpawnValues(replaceFile, false, Path);

            ASASpawnContainer();
        }


        enum Mod
        {
            Prehistoric1, /*Beasts*/                XyphMischoptera,                    ACEndemics, /*Endemics*/            MarniiGriffins, /*Marnii MULTI*/
            Prehistoric2,                           XyphVetuli,                         ACAquaria, /*Aquaria*/              IslaNycta,
            Prehistoric3,                           KamiFeral,                          AC2WildArk, /*Addititio   2 Files*/ PaddletailSpino, /*PSpino*/
            Prehistoric4,                           TheSunkenWorld,                     FayeMenagerie, /*Menagerie*/        PolyOrycterocetus,
            Prehistoric5,                           NeoEuropa, /*Europa*/               AussieDiamantina, /*NickDiamant*/   DeimosShantungo,
            CyrusDrakonis,                          FerasDinocroc,                      AussieMuttaburra, /*NickMutta*/     AsharavelBestiary,
            WakSpino,                               FerasSmilo, /*GreaterSaber*/        AussieMinmi, /*AussieAdditions*/    MegaBitsAndBobs,
            Hatze,                                  FerasFoxes, /*Arcticfox*/           AbiArgentino, /*Argentino*/         KalugaAppalachian,
            TACElementalRaptors, /*TAC_FireAnd*/    CuriousCryptids, /*TutorialDino*/   FeralMajunga, /*Majunga*/
            MoroHydrovanta, /*Hydrovanta*/          AstraeosCreatures,                  FeralWalliserops, /*Walliserops*/   ShadCritterReworks,
            ShadAtlas,                              MyrmDracoteuthis, /*Dracoteuthis*/  Barsboldia,                         NoUntameables,
            PortsOfAtlas,                           MyrmDraconisGlaucus, /*Glaucus*/    Sivatherium,                        ShinsPortedCreatures,
            AtlasReborn,       /*MULTIMAIN*/        IsleSkyshroud, /*skysh  MULTIMAIN*/ Cockatrice,                         ArborealAdditions,
            AtlasFish,         /*2 Files*/          IsleOxalaia, /*IoMSpino*/           AA_Brachiosaurus, /*AABrachio*/     CrystalWyvernPort,
            MoroLivy,                               IsleSpearcrest, /*BSSpearcrest*/    AA_Acrocantho, /*AAAcro*/           CastleCraftDecor,
            MoroGigantophis,                        IsleSuchomimus, /*IsleOfMythsSucho*/Meraxes,                            PaleoAscension,
            SulfurTitan, /*TitanSulfur*/            HorizonSuchomimus, /*Horizons*/     RunicWyverns,                       BetterCreatures,
            AA_Edmontonia, /* Edmontonia */         MoroTylo,                           NeoAurochs, /*Aurochs*/             ImprovedPhoenix,
            AA_Anomalocaris,                        MoroNotho,                          NeoStygi, /*Stygi*/                 
            Cricosaurus, /*AA_Crico*/               Rubidgea, /*OCRubidgea*/            NeoStyraco, /*Styra*/               
            ArketyDraconyx, /*ATDraconyx*/          Birdwatcher,                        Skjaldastordr, /*MSVV_Skjaldastor*/ ClayAnkylo,
            ArketyScotoharpes, /*ATScoto*/          TACVectispinus,                     PygmyHippo,                         ClayLeed,
            ArketyBombardier, /*ATBomb  MULTIMAIN*/ TACAntrodemus,                      ExtremeGargantSpino, /*t5ege*/      ClayPachyrhino,
            PiggleLycosuchus, /* Lycosuchus */      TACEocarcharia, /*Eocarcharia*/     CyrusRedPanda,                      ClayRex,
            PiggleAdasaurus, /*PPR-Ada*/            TACDaemonis, /*Daemonis*/           CyrusThoraxSpider, /*CyrusJumping*/ ClayTrike,
            PaleoApexPredators, /*PA_EVO_01*/       TACDzungatherium,                   CyrusMagnaGecko, /*CyrusGecko*/     CyberStructures,
            PaleoDangerousDepths, /*PA_EVO_02*/     TACBastionBeetle,                   CliffansCritters,                   MaewingPort,
            PaleoHardHittingHerbivores,/*PA_EVO_03*/Tyrannodominator,                   HuskyWolf,                          FerasWildSeas, /*FCR_Wild*/
            PaleoNativeAquatics, /*PA_PLUS_01*/     ARKOSanguivern,                     Stegotetrabelodon, /*Steg*/         FerasEnhy, /*Ferasvanil*/
            PaleoRulersWastelands, /*PA_PLUS_02*/   ARKOHapipalus,                      Gingerpithecus,                     GiantIsopod,
            XyphCharnia,                            ARKODesolatitan,                    StarExoticAnimals, /*RR_Exo MULTI*/ GuishanCollection,
            XyphDick,                               MoreWyverns,                        StarAnimals, /*RR_StarAnim  MULTI*/ ScorchedReborn, /*SEReb*/
            XyphEnantiophoenix,                     Monolopho, /*Forogotten*/           StarFarmAnimals, /*RR_Farm  MULTI*/ TweeWyvern, /*Tristan*/
            XyphMegistotherium,                     Noxcalva, /*BlazingNoxcalva*/       OceaniaContent, /*AEM_Content*/     ACGrandHunt, /*Additional*/
            XyphMeiolania,                          RoyalArchaeopteryx,                 AnomalyGalvarex,                    TACIguana,
        }

        const string Path = "E:/ARK Saves/ArkSpawnEntriesCreator/AscendedModsAdditions/LifeFinds.txt";
        const bool replaceFile = true;
        const bool multimain = false;

        public static void ASASpawnContainer() 
        {
            const string Path_test = "E:/ARK Saves/ArkSpawnEntriesCreator/testing.txt";
            const string Path_test2 = "E:/ARK Saves/ArkSpawnEntriesCreator/testing2.txt";

            if (replaceFile)
            {
                File.Delete(Path_test);
                File.Delete(Path_test2);
            }

            const string Path_base = "E:/ARK Saves/ArkSpawnEntriesCreator/AscendedModsAdditions";

            const string path_spawn = "E:/ARK Saves/ArkSpawnEntriesCreator/ASASpawnEntries.csv";
            const string path_spawn2 = "E:/ARK Saves/ArkSpawnEntriesCreator/ASASpawnEntriesBase.csv";

            const string path_engram = "E:/ARK Saves/ArkSpawnEntriesCreator/Engrams.csv";
            const string path_engram2 = "E:/ARK Saves/ArkSpawnEntriesCreator/EngramVanALL.csv";

            const string path_loot = "E:/ARK Saves/ArkSpawnEntriesCreator/Loot.csv";
            const string path_lootbase = "E:/ARK Saves/ArkSpawnEntriesCreator/ASA_LootDropsDefault.txt";

            const string path_toolkit = "E:/ARK Saves/ArkSpawnEntriesCreator/ToolkitReplacements.csv";

            //ASA_SpawnContainers.FindPhrase(Path_base, Path_test, "Wyvern_Character_BP_Fire_C");
            //ASA_Engrams.CreateFullEngramLine(path_engram, Path_test);

            //ASA_SpawnContainers.ReadContainerKnowledge(path_spawn2, Path_test, path_spawn, true);

            //ASA_SpawnContainers.CreateToolkitReplacements(path_toolkit, Path_test);
            //ASA_SpawnContainers.CreateToolkitSpawners(path_spawn, Path_test);

            //ASA_LootDrops.CreateModdedLootdrops(path_lootbase, path_loot, Path_test);

            //ASA_Engrams.AddModifiersToEngrams(path_engram, Path_test);

            //ASA_SpawnContainers.CreateSpawnContainerAdditions(path_spawn, Path_test);

            ASAExtractor();
        }

        public static void ASAExtractor()
        {
            if (replaceFile)
            {
                File.Delete(Path);
            }

            const string path_strings = "E:/ARK Saves/ArkSpawnEntriesCreator/strings.txt";

            ASA_TextExtractor.ExtractEntryNames(path_strings, Path);
            ASA_TextExtractor.ExtractEngrams(path_strings, Path);
            ASA_TextExtractor.ExtractDino(path_strings, Path);
            ASA_TextExtractor.ExtractRemaps(path_strings, Path);
            ASA_TextExtractor.ExtractRemappedEngrams(path_strings, Path);
            ASA_TextExtractor.DetectSpawnReplacements(path_strings, Path);
            if (multimain)
            {
                List<SpawnContainerMulti> spawnContainers = ASA_TextExtractor.ExtractMultiMainDinoAdditions(path_strings);
                List<SpawnEntry> globalEntries = ASA_TextExtractor.ExtractGlobalReplacement(path_strings, Path);
                ASA_TextExtractor.CreateDinoAdditionsSBMulti(spawnContainers, Path);
                ASA_TextExtractor.CreateGlobalAdditionsSB(globalEntries, Path);
                ASA_TextExtractor.CreateMultiParamSummaryMulti(spawnContainers, Path);
            }
            else
            {
                List<SpawnContainer> spawnContainers = ASA_TextExtractor.ExtractDinoAdditions(path_strings);
                List<SpawnEntry> globalEntries = ASA_TextExtractor.ExtractGlobalReplacement(path_strings, Path);
                ASA_TextExtractor.CreateDinoAdditionsSB(spawnContainers, Path);
                ASA_TextExtractor.CreateGlobalAdditionsSB(globalEntries, Path);
                ASA_TextExtractor.CreateMultiParamSummary(spawnContainers, Path);
            }
        }
    }
}
