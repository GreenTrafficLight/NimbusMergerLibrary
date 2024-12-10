using Ace7LocalizationFormat.Formats;
using Org.BouncyCastle.Asn1.Ocsp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NimbusMergerLibrary.Mergers
{
    public static class LocalizationMerger
    {
        public static void MergeCmn(CmnFile exportCmn, CmnFile modCmn, Dictionary<char, DatFile> exportDats, Dictionary<char, DatFile> modDats, int gameMaxStringNumber, SortedDictionary<string, CmnString> parent)
        {
            // Iterate through all the strings of the Cmn
            foreach (string key in parent.Keys)
            {
                // If it's a new variable
                if (gameMaxStringNumber < parent[key].StringNumber)
                {
                    // Add it to the Cmn that is going to be exported
                    if (exportCmn.AddVariable(parent[key].Name, exportCmn.Root))
                    {
                        // Iterate through each localization contained in the game
                        foreach (char letter in exportDats.Keys)
                        {
                            // Check if the mod contains that localization
                            if (modDats.ContainsKey(letter))
                            {
                                // Check if the string exist in that localization
                                // If it doesn't, put a null string
                                string newString = modDats[letter].Strings.Count <= parent[key].StringNumber 
                                    ? "\0" 
                                    : modDats[letter].Strings[parent[key].StringNumber];
                                exportDats[letter].Strings.Add(newString);
                            }
                            else
                            {
                                // If it doesn't exist, add a null string
                                exportDats[letter].Strings.Add("\0");
                            }
                        }
                    }
                }
                MergeCmn(exportCmn, modCmn, exportDats, modDats, gameMaxStringNumber, parent[key].Childrens);
            }
        }
    }
}
