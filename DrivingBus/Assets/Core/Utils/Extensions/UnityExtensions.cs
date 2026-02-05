using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;
using UnityEngine.Assertions;
using Rand = UnityEngine.Random;

namespace Core.Utils.Extensions
{
    /// <summary>
    /// Unity Extensions class.
    /// </summary>
    public static class UnityExtensions
    {
        #region Runtime extensions

        public static void ReleaseIfZero(this System.Threading.SemaphoreSlim sem)
        {
            if (sem.CurrentCount == 0)
                sem.Release();
        }

        /// <summary>
        /// debug string
        /// </summary>
        public static string ToDebugString<TKey, TValue> (this IDictionary<TKey, TValue> dictionary)
        {
            return "{" + string.Join(",", dictionary.Select(kv => kv.Key + "=" + kv.Value).ToArray()) + "}";
        }

        /// <summary>
        /// debug string
        /// </summary>
        public static string ToDebugString<TKey> (this IEnumerable<TKey> collection)
        {
            return "{" + string.Join(",", collection.Select(v => v.ToString()).ToArray()) + "}";
        }

        /// <summary>
        /// Create a sprite from this texture
        /// </summary>
        public static Sprite ToSprite(this Texture2D texture)
        {
            var rect = new Rect(0, 0, texture.width, texture.height);
            var pivot = new Vector2(0.5f, 0.5f);
            return Sprite.Create(texture, rect, pivot, 100.0f, 0, meshType : SpriteMeshType.FullRect);
        }

        public static Texture2D ToNewTexture(this RenderTexture rt, bool mipMaps = false)
        {
            Texture2D image = new Texture2D(rt.width, rt.height);
            rt.ReadToTexture(image, mipMaps);
            return image;
        }

        public static void ReadToTexture(this RenderTexture rt, Texture2D texture, bool mipMaps)
        {
            // Copy to rexture
            var old = RenderTexture.active;
            RenderTexture.active = rt;
            texture.ReadPixels(new Rect(0, 0, rt.width, rt.height), 0, 0, true);
            texture.Apply();
            RenderTexture.active = old;
        }

        // Produces new Texture2D using RenderTexture.GetTemporary();
        // Need wait to ensure the canvas already resized after camera's aspect has changed.
        public async static Task<Texture2D> CaptureUIFrame(this Camera camera, int w, int h)
        {
            var rt = RenderTexture.GetTemporary(w, h);
            var oldRt = camera.targetTexture;
            camera.targetTexture = rt;
            await Task.Delay(150);
            camera.Render();
            camera.targetTexture = oldRt;
            var tex = rt.ToNewTexture();
            RenderTexture.ReleaseTemporary(rt);
            return tex;
        }

        public static Texture2D CaptureFrame(this Camera camera, int w, int h)
        {
            var rt = RenderTexture.GetTemporary(w, h);
            var oldRt = camera.targetTexture;
            camera.targetTexture = rt;
            camera.Render();
            camera.targetTexture = oldRt;
            var tex = rt.ToNewTexture();
            RenderTexture.ReleaseTemporary(rt);
            return tex;
        }
        
        public static void DestroyAllChildren(this Transform t)
        {
            foreach(Transform obj in t)
                GameObject.Destroy(obj.gameObject);

            t.DetachChildren();
        }

        public static Transform GetActiveChild(this Transform parent, int targetIdx) {
            int idx = 0;
            foreach(Transform c in parent)
                if (c.gameObject.activeSelf)
                    if (targetIdx == idx)
                        return c;
                    else
                        idx++;
            return null;
        }

        public static void ResetTransform(this Transform t, bool stretch = false)
        {
            t.localRotation = Quaternion.identity;
            t.localPosition = Vector3.zero;
            t.localScale = Vector3.one;
            var rt = t.GetComponent<RectTransform>();
             if (rt != null)
             {
                 rt.anchoredPosition = Vector2.zero;
                 if (stretch)
                    rt.StrectTransform();
             }
        }

        public static void StrectTransform(this RectTransform rt)
        {
            rt.anchorMin = Vector2.zero;
            rt.anchorMax = Vector2.one;
            rt.sizeDelta = Vector2.zero;
        }

        public static void AnchorsToCenter(this RectTransform rt)
        {
            var pt = rt.parent as RectTransform;

            if (pt == null)
                return;

            var ptr = pt.rect;
            var pos = rt.localPosition;
            var size = rt.rect;

            rt.anchorMin = rt.anchorMax = new Vector2(0.5f, 0.5f);
            rt.localPosition = pos;
            rt.sizeDelta = new Vector2(size.width, size.height);
        }

        public static void AnchorsToCorners(this RectTransform rt)
        {
            var pt = rt.parent as RectTransform;

            if (pt == null)
                return;

            var ptr = pt.rect;

            var newAnchorsMin = new Vector2(
                rt.anchorMin.x + rt.offsetMin.x / ptr.width,
                rt.anchorMin.y + rt.offsetMin.y / ptr.height
            );

            var newAnchorsMax = new Vector2(
                rt.anchorMax.x + rt.offsetMax.x / ptr.width,
                rt.anchorMax.y + rt.offsetMax.y / ptr.height
            );

            rt.anchorMin = newAnchorsMin;
            rt.anchorMax = newAnchorsMax;
            rt.offsetMin = rt.offsetMax = Vector2.zero;
        }

        #region Set Position

        /// <summary>
        /// Sets position.x
        /// </summary>
        /// <param name="newX">x</param>
        public static void SetPositionX(this Transform t, float newX) {
            t.position = new Vector3(newX, t.position.y, t.position.z);
        }

        /// <summary>
        /// Sets position.y
        /// </summary>
        /// <param name="newY">x</param>
        public static void SetPositionY(this Transform t, float newY) {
            t.position = new Vector3(t.position.x, newY, t.position.z);
        }

        /// <summary>
        /// Sets position.z
        /// </summary>
        /// <param name="newZ">x</param>
        public static void SetPositionZ(this Transform t, float newZ) {
            t.position = new Vector3(t.position.x, t.position.y, newZ);
        }

        #endregion

        #region Look at

        /// <summary>
        /// Uses Z axe rotation to look at
        /// </summary>
        /// <param name="target">transform to look at</param>
        public static void LookAt2D(this Transform tr, Transform target)
        {
            tr.LookAt2D(target.position);
        }

        /// <summary>
        /// Uses Z axe rotation to look at
        /// </summary>
        /// <param name="target">point to look at</param>
        public static void LookAt2D(this Transform tr, Vector3 pos)
        {
            var relative = tr.InverseTransformPoint(pos);
            float angle = Mathf.Atan2(relative.x, relative.y) * Mathf.Rad2Deg;
            tr.Rotate(0, 0, -angle, Space.Self);
        }

        #endregion

        /// <summary>
        /// Gets all children's and the root's transforms in one array
        /// </summary>
        /// <returns>An array of all nested children transforms, incluing root</returns>
        public static Transform[] GetMeAndMyChildren(this GameObject g)
        {
            return g.GetComponentsInChildren<Transform>();
        }

        /// <summary>
        /// Standard Fisher-Yates shuffle for elements of the list
        /// </summary>
        public static void Shuffle<T>(this IList<T> list)
        {
            int n = list.Count;
            while (n-- > 1)
            {
                int k = Rand.Range(0, n + 1);
                var value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }

        /// <summary>
        /// Saves mesh to OBJ format string
        /// </summary>
        /// <returns>string with OBJ data</returns>
        public static string ToObjString(this Mesh mesh)
        {
            var sb = new StringBuilder();

            sb.Append("g ").Append(mesh.name).Append("\n");
            foreach (var v in mesh.vertices)
            {
                sb.Append(string.Format("v {0} {1} {2}\n", v.x, v.y, v.z));
            }

            sb.Append("\n");
            foreach (var v in mesh.normals)
            {
                sb.Append(string.Format("vn {0} {1} {2}\n", v.x, v.y, v.z));
            }
            sb.Append("\n");

            foreach (var v in mesh.uv)
            {
                sb.Append(string.Format("vt {0} {1}\n", v.x, v.y));
            }

            for (int material = 0; material < mesh.subMeshCount; material++)
            {
                sb.Append("\n");
                //sb.Append("usemtl ").Append(mats[material].name).Append("\n");
                //sb.Append("usemap ").Append(mats[material].name).Append("\n");

                var triangles = mesh.GetTriangles(material);
                for (int i = 0; i < triangles.Length; i += 3)
                {
                    sb.Append(string.Format("f {0}/{0}/{0} {1}/{1}/{1} {2}/{2}/{2}\n", triangles[i] + 1, triangles[i + 1] + 1, triangles[i + 2] + 1));
                }
            }

            return sb.ToString();
        }

        /// <summary>
        /// Saves mesh to an OBJ file
        /// </summary>
        /// <param name="filename">name of the file, where saved data will go</param>
        /// <param name="mesh">mesh to save</param>
        public static void ToObjFile(this Mesh mesh, string filename) {
            using (var sw = new StreamWriter(File.OpenWrite(filename))) {
                sw.Write(mesh.ToObjString());
            }
        }


        #endregion


#region Editor extensions
#if UNITY_EDITOR

        /// <summary>
        /// Editor helper that toggles scene wireframe mode
        /// </summary>
        public static void ToggleWireframe(this SceneView v)
        {
#if UNITY_2018_1_OR_NEWER
            var md = v.cameraMode;
            md.drawMode = md.drawMode == DrawCameraMode.Textured ?
                md.drawMode = DrawCameraMode.Wireframe :
                md.drawMode = DrawCameraMode.Textured;
            v.cameraMode = md;
#else
            v.renderMode = v.renderMode == DrawCameraMode.Textured ?
                v.renderMode = DrawCameraMode.Wireframe :
                v.renderMode = DrawCameraMode.Textured;
            v.Repaint();
#endif
        }

#endif
#endregion
        static Vector3[] _transCorners = new Vector3[4];
        public static Rect TransformAsRectInLocal(this RectTransform o, RectTransform tr)
        {
            tr.GetWorldCorners(_transCorners);
            _transCorners[0] = o.InverseTransformPoint(_transCorners[0]);
            _transCorners[1] = o.InverseTransformPoint(_transCorners[1]);
            _transCorners[2] = o.InverseTransformPoint(_transCorners[2]);
            _transCorners[3] = o.InverseTransformPoint(_transCorners[3]);
            Rect r = new Rect(_transCorners[0], _transCorners[2] - _transCorners[0]);
            return r;
        }


        /// <summary>
        /// Takes root SO, find 'key' field, ensures that value is matching
        /// value. If changed - returns true, if already correct - false
        /// </summary>
#if UNITY_EDITOR
        public static bool EnsurePropertyValue(this SerializedObject so, string key, UnityEngine.Object val)
        {
            var field = so.FindProperty(key);
            Assert.IsNotNull(field);

            if ( field.objectReferenceValue != val)
            {
                field.objectReferenceValue = val;
                so.ApplyModifiedProperties();
                return true;
            }
            return false;
        }


        /// <summary>
        /// Quick helper to get my inspector ;)
        /// </summary>
        [MenuItem ("CONTEXT/Object/Find my inspector")]
        static void FindInspector(MenuCommand command)
        {
            var m = command.context;
            var ed = Editor.CreateEditor(m);
            var ms = MonoScript.FromScriptableObject(ed);
            EditorGUIUtility.PingObject(ms);
            Debug.Log($" MonoScript: {ms.name}");
        }

        /// <summary>
        /// Quick helper to ping selected object, sometimes helps to expand project path to it
        /// </summary>
        [MenuItem ("CONTEXT/Object/Ping me")]
        static void PingInspected(MenuCommand command)
        {
            var m = command.context;
            EditorGUIUtility.PingObject(m);
        }

#endif
    }
}
