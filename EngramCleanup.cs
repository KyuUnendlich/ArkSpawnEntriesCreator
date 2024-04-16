using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.VisualBasic.FileIO;
using System.Globalization;

namespace ArkSpawnEntriesCreator
{
    class EngramCleanup
    {
        const string Path = "C:/Users/matth/Desktop/engram.txt";


        public static void EngramsDateRemover()
        {

            var path = @"G:\ARK Saves\ArkSpawnEntriesCreator\Engrams.csv";
            using (TextFieldParser csvParser = new TextFieldParser(path))
            {
                csvParser.CommentTokens = new string[] { "#" };
                csvParser.SetDelimiters(new string[] { "," });
                csvParser.HasFieldsEnclosedInQuotes = true;

                while (!csvParser.EndOfData)
                {
                    string[] line = csvParser.ReadFields();
                    String temp = line[0].Substring(51);
                    File.AppendAllText(Path, temp);
                    File.AppendAllText(Path, "\r\n");

                }
            }
        }


        public static void EngramsVanillaRemoverX()
        {
            //Remove Engram if "x" in second column
            var path = @"G:\ARK Saves\ArkSpawnEntriesCreator\Engrams.csv";
            using (TextFieldParser csvParser = new TextFieldParser(path))
            {
                csvParser.CommentTokens = new string[] { "#" };
                csvParser.SetDelimiters(new string[] { "," });
                csvParser.HasFieldsEnclosedInQuotes = true;

                string hiddenText = ", EngramHidden=true";

                while (!csvParser.EndOfData)
                {
                    string[] line = csvParser.ReadFields();
                    if (line[1].Equals("x"))
                    {
                    }
                    else
                    {
                        File.AppendAllText(Path, line[0]);
                        File.AppendAllText(Path, "\r\n");
                    }
                }
            }
        }

            public static void EngramsVanillaHiderXY()
        {
            // Hides Engram, if second column has "x" as value, if "y" doesnt hide
            var path = @"G:\ARK Saves\ArkSpawnEntriesCreator\Engrams.csv";
            using (TextFieldParser csvParser = new TextFieldParser(path))
            {
                csvParser.CommentTokens = new string[] { "#" };
                csvParser.SetDelimiters(new string[] { "," });
                csvParser.HasFieldsEnclosedInQuotes = true;

                string hiddenText = ", EngramHidden=true";

                while (!csvParser.EndOfData)
                {
                    string[] line = csvParser.ReadFields();
                    if (line[2].Equals("y"))
                    {
                        File.AppendAllText(Path, line[0]);
                        File.AppendAllText(Path, "\r\n");
                    }
                        else if (line[1].Equals("x"))
                    {
                        int splitPosition = line[0].IndexOf(",");
                        string firstPartString = line[0].Substring(0, splitPosition);
                        string secondPartString = line[0].Substring(splitPosition);

                        File.AppendAllText(Path, firstPartString + hiddenText + secondPartString);
                        File.AppendAllText(Path, "\r\n");
                    }
                    else
                    {
                        File.AppendAllText(Path, line[0]);
                        File.AppendAllText(Path, "\r\n");
                    }
                }
            }
        }
    }
}
