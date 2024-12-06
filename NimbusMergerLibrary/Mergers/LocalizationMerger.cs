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
            foreach (string key in parent.Keys)
            {
                if (gameMaxStringNumber < parent[key].StringNumber)
                {
                    Console.WriteLine(parent[key].Name);
                    if (exportCmn.AddVariable(parent[key].Name, exportCmn.Root))
                    {
                        foreach (char letter in exportDats.Keys)
                        {
                            if (modDats.ContainsKey(letter))
                            {
                                string newString = modDats[letter].Strings[parent[key].StringNumber];
                                exportDats[letter].Strings.Add(newString);
                            }
                            else
                            {
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
