using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace ArkSpawnEntriesCreator
{
    class HelperFunctions
    {
        public static void selectionSort(string[] arr)
        {
            int n = arr.Length;

            // One by one move boundary of
            // unsorted subarray
            for (int i = 0; i < n - 1; i++)
            {

                // Find the minimum element
                // in unsorted array
                int min_idx = i;
                for (int j = i + 1; j < n; j++)
                    if (isAlphabeticallySmaller(
                      arr[j], arr[min_idx]))
                        min_idx = j;

                // Swap the found minimum
                // element with the first element
                String temp = arr[min_idx];
                arr[min_idx] = arr[i];
                arr[i] = temp;
            }
        }
        // Function to compare 2 words
        static bool isAlphabeticallySmaller(
          string str1, String str2)
        {
            str1 = str1.ToUpper();
            str2 = str2.ToUpper();
            if (str1.CompareTo(str2) < 0)
            {
                return true;
            }
            return false;
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
        const string Path = "C:/Users/matth/Desktop/Ascended/AtlasFish.txt";
    }




}
