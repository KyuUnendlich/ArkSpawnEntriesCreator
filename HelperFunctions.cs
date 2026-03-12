using System;
using System.Collections.Generic;
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

    }
}
