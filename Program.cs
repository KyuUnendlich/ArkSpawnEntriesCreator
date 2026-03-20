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

            //OldASAMethods.ExtractAdditionalNPCSpawnValues(replaceFile, secondString2, Path);

            ASAExtractor();
        }


        enum Mod
        {
            Prehistoric1, /*Beasts*/                CuriousCryptids, /*TutorialDino*/   RunicWyverns,
            Prehistoric2,                           AstraeosCreatures,                  NeoAurochs, /*Aurochs*/
            Prehistoric3,                           MyrmDracoteuthis, /*Dracoteuthis*/  NeoStygi, //Stygi*/
            Prehistoric4,                           MyrmDraconisGlaucus, /*Glaucus*/    NeoStyraco, /*Styra*/
            Prehistoric5,                           IsleSkyshroud, /*skyshroud*/        Skjaldastordr, /*MSVV_Skjaldastordr*/
            CyrusDrakonis,                          IsleOxalaia, /*IoMSpino*/           PygmyHippo,
            WakSpino,                               IsleSpearcrest, /*BSSpearcrest*/    ExtremeGargantSpino, /*t5ege*/
            Hatze,                                  IsleSuchomimus, /*IsleOfMythsSucho*/CyrusRedPanda,
            TACElementalRaptors, /*TAC_FireAndIce*/ HorizonSuchomimus, /*Horizons*/     CyrusThoraxSpider, /*CyrusJumping*/
            MoroHydrovanta, /*Hydrovanta*/          MoroTylo,                           CyrusMagnaGecko, /*CyrusGecko*/
            ShadAtlas,                              MoroNotho,                          CliffansCritters,
            PortsOfAtlas,                           Rubidgea, /*OCRubidgea*/            HuskyWolf,
            AtlasReborn,                            Birdwatcher,                        Stegotetrabelodon, /*Steg*/
            AtlasFish,                              TACVectispinus,                     StarSeahorse, /*RR_Mod_StarSeahorse*/
            MoroLivy,                               TACAntrodemus,                      StarExoticAnimals, /*RR_Mod_ExoticAnimals*/
            MoroGigantophis,                        TACEocarcharia, /*Eocarcharia*/     StarAnimals, /*RR_Mod_StarAnimals*/
            SulfurTitan, /*TitanSulfur*/            TACDaemonis, /*Daemonis*/           StarFarmAnimals, /*RR_Mod_StarFarmAnimals*/
            Edmontonia,                             TACDzungatherium,                   OceaniaContent, /*AEM_Content*/
            Anomalocaris, /*AA_Anomalo*/            Tyrannodominator,                   AnomalyGalvarex,
            Cricosaurus, /*AA_Crico*/               ARKOSanguivern,                     MarniiGriffins, /*MarniiModsOwl*/
            Draconyx, /*ATDraconyx*/                ARKOHapipalus,                      IslaNycta,
            Scotoharpes, /*ATScoto*/                ARKODesolatitan,                    PaddletailSpino, /*PSpino*/
            BombardierBeetle, /*ATBombardier*/      MoreWyverns,                        AsharavelBestiary,
            Lycosuchus,                             Monolopho, /*Forogotten*/           DeimosShantungo,
            Adasaurus, /*PPR-Ada*/                  Noxcalva, /*BlazingNoxcalva*/       MegaBitsAndBobs,
            PaleoApexPredators, /*PA_EVO_01*/       RoyalArchaeopteryx,                 KalugaAppalachian,
            PaleoDangerousDepths, /*PA_EVO_02*/     ACEndemics, /*Endemics*/            ArborealAdditions,
            PaleoHardHittingHerbivores,/*PA_EVO_03*/ACAquaria, /*Aquaria*/
            PaleoNativeAquatics, /*PA_PLUS_01*/     AC2WildArk, /*AdditionalCreatures*/ ShadCritterReworks,
            PaleoRulersWastelands,  /*PA_PLUS_02*/  TACBastionBeetle,                   NoUntameables,
            XyphCharnia,                            FayeMenagerie, /*Menagerie*/
            XyphDick,                               AussieDiamantina, /*NickDiamant*/
            XyphEnantiophoenix,                     AussieMuttaburra, /*NickMutta*/
            XyphMegistotherium,                     AussieMinmi, /*AussieAdditions*/
            XyphMeiolania,                          AbiArgentino, /*Argentino*/
            XyphMischoptera,                        FeralMajunga, /*Majunga*/
            XyphVetuli,                             FeralWalliserops, /*Walliserops*/
            KamiFeral,                              Barsboldia,
            TheSunkenWorld,                         Sivatherium,
            NeoEuropa, /*Europa*/                   Cockatrice,
            FerasDinocroc,                          Brachiosaurus, /*AABrachio*/
            FerasSmilo, /*GreaterSaber*/            Acrocantho, /*AAAcro*/
            FerasFoxes, /*Arcticfox*/               Meraxes,
        }

        //There is now a branch (multi-main-bp) for Mods that have one addition per container with multiple different dinos
        //const string Path = "C:/Users/matth/Desktop/Ascended/AtlasFish.txt";
        const bool replaceFile = true;
        const bool secondString2 = false;
        const bool multimain = false;
        const string Path = "E:/ARK Saves/ArkSpawnEntriesCreator/AscendedModsAdditions/Prehistoric3.txt";

        public static void ASAExtractor()
        {
            if (replaceFile)
            {
                File.Delete(Path);
            }
            const string path_strings = "E:/ARK Saves/ArkSpawnEntriesCreator/strings.txt";

            ASA_TextExtractor.ExtractEngrams(path_strings, Path);
            ASA_TextExtractor.ExtractDino(path_strings, Path);
            ASA_TextExtractor.ExtractRemaps(path_strings, Path);
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
