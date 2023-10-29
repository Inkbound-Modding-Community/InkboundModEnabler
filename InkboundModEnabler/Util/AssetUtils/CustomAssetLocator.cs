using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.ResourceManagement.ResourceLocations;

namespace InkboundModEnabler.Vestiges {
    public class CustomAssetLocator : UnityEngine.AddressableAssets.ResourceLocators.IResourceLocator {
        public static readonly CustomAssetLocator instance = new();
        public string LocatorId => "InkboundModEnablerCustomAssetLocator";
        private IEnumerable<object> _keys = new List<object>();
        public IEnumerable<object> Keys => _keys;

        public bool Locate(object key, Type type, out IList<IResourceLocation> locations) {
            locations = null;
            var s = key.ToString();
            if (type == typeof(Sprite)) {
                if (assetGUIDToPath.ContainsKey(s)) {
                    locations = new List<IResourceLocation> {
                                new ResourceLocationBase(s, assetGUIDToPath[s], "InkboundModEnablerCustomSpriteProvider", type)
                            };
                    return true;
                }
            }
            // Possible to add other assets
            return false;
        }

        internal Dictionary<string, string> assetGUIDToPath = new();
    }
}
