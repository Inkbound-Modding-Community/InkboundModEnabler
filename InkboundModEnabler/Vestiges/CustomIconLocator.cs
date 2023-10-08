using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.ResourceManagement.ResourceLocations;

namespace InkboundModEnabler.Vestiges {
    internal class CustomIconLocator : UnityEngine.AddressableAssets.ResourceLocators.IResourceLocator {
        private static CustomIconLocator _instance;
        public static CustomIconLocator instance {
            get {
                _instance ??= new();
                return _instance;
            }
        }
        public string LocatorId => "InkboundModEnablerCustomSpriteLocator";
        private IEnumerable<object> _keys = new List<object>();
        public IEnumerable<object> Keys => _keys;

        public bool Locate(object key, Type type, out IList<IResourceLocation> locations) {
            locations = null;
            var s = key.ToString();
            if (assetGUIDToSmallIconPath.ContainsKey(s) && type == typeof(Sprite)) {
                // Here adding a second location might enable large sprites too?
                locations = new List<IResourceLocation> {
                    new ResourceLocationBase(s, assetGUIDToSmallIconPath[s], "InkboundModEnablerCustomSpriteProvider", type)
                };
                return true;
            }
            return false;
        }

        internal Dictionary<string, string> assetGUIDToSmallIconPath = new();
    }
}
