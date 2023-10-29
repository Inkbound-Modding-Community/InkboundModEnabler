using ShinyShoe.SharedDataLoader;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InkboundModEnabler {
    public static class AssetLibUtils {
        public static List<AssetLibrary> assetLibraryList = new();
        public static void RegisterManifestEntry(this AssetLibraryManifest.Entry entry) {
            RegisterNewManifestEntry(entry);
        }
        public static void RegisterNewManifestEntry(AssetLibraryManifest.Entry newEntry) {
            foreach (var assetLib in assetLibraryList) {
                assetLib._manifest.entries.Add(newEntry);
                assetLib._nameToEntry[newEntry.name] = newEntry;
                assetLib._assetIDToEntry[newEntry.assetID] = newEntry;
                assetLib._dataIdToEntry[newEntry.dataId] = newEntry;
                List<AssetLibraryManifest.Entry> list;
                if (!assetLib._baseDataTypeToEntries.TryGetValue(newEntry.classType, out list)) {
                    list = new List<AssetLibraryManifest.Entry>();
                    assetLib._baseDataTypeToEntries.Add(newEntry.classType, list);
                }
                list.Add(newEntry);
            }
        }
    }
}