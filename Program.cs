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

            ASAExtractor();
        }


        enum Mod
        {
            Prehistoric1, /*Beasts*/                XyphMischoptera,                    ACAquaria, /*Aquaria*/              MarniiGriffins, /*Marnii   MULTIMAIN*/
            Prehistoric2,                           XyphVetuli,                         AC2WildArk, /*Addititio   2 Files*/ IslaNycta,
            Prehistoric3,                           KamiFeral,                          TACBastionBeetle,                   PaddletailSpino, /*PSpino*/
            Prehistoric4,                           TheSunkenWorld,                     FayeMenagerie, /*Menagerie*/        AsharavelBestiary,
            Prehistoric5,                           NeoEuropa, /*Europa*/               AussieDiamantina, /*NickDiamant*/   DeimosShantungo,
            CyrusDrakonis,                          FerasDinocroc,                      AussieMuttaburra, /*NickMutta*/     MegaBitsAndBobs,
            WakSpino,                               FerasSmilo, /*GreaterSaber*/        AussieMinmi, /*AussieAdditions*/    KalugaAppalachian,
            Hatze,                                  FerasFoxes, /*Arcticfox*/           AbiArgentino, /*Argentino*/         
            TACElementalRaptors, /*TAC_FireAnd*/    CuriousCryptids, /*TutorialDino*/   FeralMajunga, /*Majunga*/
            MoroHydrovanta, /*Hydrovanta*/          AstraeosCreatures,                  FeralWalliserops, /*Walliserops*/   ShadCritterReworks,
            ShadAtlas,                              MyrmDracoteuthis, /*Dracoteuthis*/  Barsboldia,                         NoUntameables,
            PortsOfAtlas,                           MyrmDraconisGlaucus, /*Glaucus*/    Sivatherium,                        ShinsPortedCreatures,
            AtlasReborn,       /*MULTIMAIN*/        IsleSkyshroud, /*skysh  MULTIMAIN*/ Cockatrice,                         ArborealAdditions,
            AtlasFish,         /*2 Files*/          IsleOxalaia, /*IoMSpino*/           Brachiosaurus, /*AABrachio*/
            MoroLivy,                               IsleSpearcrest, /*BSSpearcrest*/    Acrocantho, /*AAAcro*/
            MoroGigantophis,                        IsleSuchomimus, /*IsleOfMythsSucho*/Meraxes,
            SulfurTitan, /*TitanSulfur*/            HorizonSuchomimus, /*Horizons*/     RunicWyverns,
            Edmontonia,                             MoroTylo,                           NeoAurochs, /*Aurochs*/
            Anomalocaris, /*AA_Anomalo*/            MoroNotho,                          NeoStygi, /*Stygi*/
            Cricosaurus, /*AA_Crico*/               Rubidgea, /*OCRubidgea*/            NeoStyraco, /*Styra*/
            Draconyx, /*ATDraconyx*/                Birdwatcher,                        Skjaldastordr, /*MSVV_Skjaldastor*/
            Scotoharpes, /*ATScoto*/                TACVectispinus,                     PygmyHippo,
            BombardierBeetle, /*ATBomb  MULTIMAIN*/ TACAntrodemus,                      ExtremeGargantSpino, /*t5ege*/
            Lycosuchus,                             TACEocarcharia, /*Eocarcharia*/     CyrusRedPanda,
            Adasaurus, /*PPR-Ada*/                  TACDaemonis, /*Daemonis*/           CyrusThoraxSpider, /*CyrusJumping*/
            PaleoApexPredators, /*PA_EVO_01*/       TACDzungatherium,                   CyrusMagnaGecko, /*CyrusGecko*/
            PaleoDangerousDepths, /*PA_EVO_02*/     Tyrannodominator,                   CliffansCritters,
            PaleoHardHittingHerbivores,/*PA_EVO_03*/ARKOSanguivern,                     HuskyWolf,
            PaleoNativeAquatics, /*PA_PLUS_01*/     ARKOHapipalus,                      Stegotetrabelodon, /*Steg*/
            PaleoRulersWastelands, /*PA_PLUS_02*/   ARKODesolatitan,                    StarSeahorse, /*RR_Seah   MULTIMAIN*/
            XyphCharnia,                            MoreWyverns,                        StarExoticAnimals, /*RR_Exo   MULTIMAIN*/
            XyphDick,                               Monolopho, /*Forogotten*/           StarAnimals, /*RR_StarAnim   MULTIMAIN*/
            XyphEnantiophoenix,                     Noxcalva, /*BlazingNoxcalva*/       StarFarmAnimals, /*RR_Farm   MULTIMAIN*/
            XyphMegistotherium,                     RoyalArchaeopteryx,                 OceaniaContent, /*AEM_Content*/
            XyphMeiolania,                          ACEndemics, /*Endemics*/            AnomalyGalvarex,
        }

        const string Path = "E:/ARK Saves/ArkSpawnEntriesCreator/AscendedModsAdditions/Prehistoric4.txt";
        const bool replaceFile = true;
        const bool multimain = false;

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
