using ShinyShoe;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace InkboundModEnabler {
    internal static class UnityExtensions {
        public static void SetAnchor(this RectTransform transform, double xMin, double xMax, double yMin, double yMax) {
            transform.anchorMin = new Vector2((float)xMin, (float)yMin);
            transform.anchorMax = new Vector2((float)xMax, (float)yMax);
        }

        public static void SetAnchor(this RectTransform transform, double x, double y) {
            transform.SetAnchor(x, x, y, y);
        }
        public static RectTransform ChildRect(this GameObject obj, string path) {
            return obj.ChildTransform(path) as RectTransform;
        }
        public static RectTransform Rect(this GameObject obj) {
            return obj.transform as RectTransform;
        }
        public static RectTransform Rect(this Transform obj) {
            return obj as RectTransform;
        }

        public static void SetRotate2D(this RectTransform rect, int degrees) {
            rect.eulerAngles = new Vector3(0, 0, degrees);
        }

        public static Transform ChildTransform(this GameObject obj, string path) {
            return obj.transform.Find(path);
        }
        public static GameObject ChildObject(this GameObject obj, string path) {
            return obj.ChildTransform(path)?.gameObject;
        }

        public static GameObject[] ChildObjects(this GameObject obj, params string[] paths) {
            return paths.Select(p => obj.transform.Find(p)?.gameObject).ToArray();
        }

        public static void DestroyChildren(this GameObject obj, params string[] paths) {
            obj.ChildObjects(paths).ForEach(GameObject.Destroy);
        }
        public static void DestroyChildrenImmediate(this GameObject obj, params string[] paths) {
            obj.ChildObjects(paths).ForEach(GameObject.DestroyImmediate);
        }
        public static void DestroyComponents<T>(this GameObject obj) where T : UnityEngine.Object {
            var componentList = obj.GetComponents<T>();
            foreach (var c in componentList)
                GameObject.DestroyImmediate(c);
        }
        public static T EditComponent<T>(this GameObject obj, Action<T> build) where T : Component {
            var component = obj.GetComponent<T>();
            build(component);
            return component;
        }

        public static string NullStr(this object obj) {
            return obj == null ? "<null>" : $"<good:{obj}>";
        }

        public static Image AddChildImage(this GameObject obj, Sprite sprite) {
            var (child, _) = Create("child-image", obj.transform);
            child.FillParent();
            var img = child.AddComponent<Image>();
            img.sprite = sprite;
            return img;
        }

        public static T MakeComponent<T>(this GameObject obj, Action<T> build) where T : Component {
            var component = obj.AddComponent<T>();
            build(component);
            return component;
        }
        public static void AddTo(this Transform obj, Transform parent) {
            obj.SetParent(parent);
            obj.localPosition = Vector3.zero;
            obj.localScale = Vector3.one;
            obj.localRotation = Quaternion.identity;
        }

        public static void FillParent(this RectTransform rect) {
            rect.SetAnchor(0, 1, 0, 1);
            rect.sizeDelta = Vector2.zero;
        }

        public static void FillParent(this GameObject obj) {
            obj.Rect().FillParent();
        }

        public static (GameObject, RectTransform) Create(string name, Transform parent = null) {
            var obj = new GameObject(name, typeof(RectTransform));
            if (parent != null)
                obj.AddTo(parent);
            return (obj, obj.Rect());
        }

        public static void AddTo(this Transform obj, GameObject parent) { obj.AddTo(parent.transform); }
        public static void AddTo(this GameObject obj, Transform parent) { obj.transform.AddTo(parent); }
        public static void AddTo(this GameObject obj, GameObject parent) { obj.transform.AddTo(parent.transform); }

    }
}