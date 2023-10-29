using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.ResourceManagement.ResourceProviders;

namespace InkboundModEnabler.Vestiges {
    internal class CustomSpriteProvider : ResourceProviderBase {
        public static string baseSpritePath => InkboundModEnabler.settings.customVestigeRoot.Value;
        public CustomSpriteProvider() {
            m_ProviderId = "InkboundModEnablerCustomSpriteProvider";
        }
        public override void Provide(ProvideHandle provideHandle) {
            string SpritePath = Path.Combine(baseSpritePath, provideHandle.Location.InternalId);
            if (File.Exists(SpritePath)) {
                byte[] array = File.ReadAllBytes(SpritePath);
                Texture2D texture2D = new Texture2D(2, 2, TextureFormat.BGRA32, false);
                texture2D.LoadImage(array);
                var sprite = Sprite.Create(texture2D, new Rect(0f, 0f, texture2D.width, texture2D.height), new Vector2(0f, 0f), 200f);
                provideHandle.Complete<Sprite>(sprite, sprite != null, (sprite != null) ? null : new Exception($"Error loading sprite {provideHandle.Location.PrimaryKey} at {SpritePath}"));
            } else {
                InkboundModEnabler.log.LogError($"Tried to load sprite {provideHandle.Location.PrimaryKey} with path {SpritePath} but file does not exist.");
                provideHandle.Complete<Sprite>(null, false, new Exception($"Tried to load sprite {provideHandle.Location.PrimaryKey} with path {SpritePath} but file does not exist."));
            }
        }
    }
}
