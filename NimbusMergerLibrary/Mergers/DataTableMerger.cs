using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UAssetAPI;

namespace NimbusMergerLibrary.Mergers
{
    public class DataTableMerger
    {
        protected UAsset _gameAsset = null;
        protected string _dataTablePath = string.Empty;

        public void Write(string path)
        {
            string exportFilePath = path + _dataTablePath;
            string directoryPath = Path.GetDirectoryName(exportFilePath);
            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }
            _gameAsset.Write(exportFilePath);
        }
    }
}
