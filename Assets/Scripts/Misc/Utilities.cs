using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Quaternion = UnityEngine.Quaternion;
using random = UnityEngine.Random;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;

namespace Misc
{
    public static class Utilities
    {
        private static readonly System.Random RandomSys = new System.Random();

        // Find the index of the character occuring n-th time.
        public static int IndexOfNth(this string str, string value, int nth = 0)
        {
            if (nth < 0)
                throw new ArgumentException("Can not find a negative index of substring in string. Must start with 0");

            int offset = str.IndexOf(value);

            for (int i = 0; i < nth; i++)
            {
                if (offset == -1) return -1;
                offset = str.IndexOf(value, offset + 1);
            }

            return offset;
        }

        // Delayed invocation of Action (waits for seconds equal to "delay" and then invokes Action)
        public static Coroutine Invoke(this MonoBehaviour mb, Action f, float delay)
        {
            return mb.StartCoroutine(InvokeRoutine(f, delay));
        }

        // Delayed invocation of Action (continuously invokes the predicate and waits until it return true value)
        public static void Invoke(this MonoBehaviour mb, Action f, Func<bool> predicate)
        {
            mb.StartCoroutine(InvokeRoutine(f, predicate));
        }

        private static IEnumerator InvokeRoutine(Action f, float delay)
        {
            yield return new WaitForSeconds(delay);
            f();
        }

        private static IEnumerator InvokeRoutine(Action f, Func<bool> predicate)
        {
            while (predicate() == false)
            {
                yield return null;
            }

            f();
        }

        public static float RandomSign()
        {
            return (random.value < 0.5f) ? -1f : 1f;
        }

        public static bool EqualLayer(int layer, LayerMask layerMask)
        {
            return (layerMask.value & (1 << layer)) > 0;
        }

        public static void CallBackOnInvoke(this GameObject go, Action onIvoke, Action<GameObject> callback)
        {
            onIvoke += () => callback(go);
        }

        public static int RandomExcept(int min, int max, int exception)
        {
            int randomNumber = random.Range(min, max);
            int itereations = 0;

            while (randomNumber == exception)
            {
                itereations += 1;
                randomNumber = random.Range(min, max);
                if (itereations > 1000)
                    throw new Exception(
                        "The number of iterations for finding non-matching random integer for list exceeded 1000");
            }

            return randomNumber;
        }

        public static int RandomExceptList(int min, int max, List<int> exceptions)
        {
            int itereations = 0;
            int randomNumber = random.Range(min, max);

            while (exceptions.Contains(randomNumber))
            {
                itereations += 1;
                randomNumber = random.Range(min, max);
                if (itereations > 1000)
                    throw new Exception(
                        "The number of iterations for finding non-matching random integer for list exceeded 1000");
            }

            return randomNumber;
        }

        public static T ChooseRandomObject<T>(params T[] objects)
        {
            int i = random.Range(0, objects.Length);
            return objects[i];
        }

        [Obsolete("", false)]
        public static T ChooseRandomObject<T>(IList<T> objects, int randSeed)
        {
            int initialSeed = random.seed;
            random.InitState(randSeed);
            int i = random.Range(0, objects.Count);
            random.InitState(initialSeed);

            return objects[i];
        }

        public static T ChooseRandomObject<T>(this IList<T> objects)
        {
            if (objects == null) throw new ArgumentException("Argument list to choose random element from can't be null.");
            if (objects.Count == 0) throw new ArgumentException("Argument list to choose random element from can't be empty (Count == 0).");
        
            var i = random.Range(0, objects.Count);
            return objects[i];
        }

        public static List<T> ChooseRandomElements<T>(this List<T> myList, int choiceAmount)
        {
            return myList.OrderBy(x => random.value).Take(choiceAmount).ToList();
        }

        public static List<T> PoolRandomElements<T>(this List<T> myList, int amount)
        {
            List<T> pooledList = new();
            if (myList == null) throw new ArgumentNullException(nameof(myList));
            for (int i = 0; i < amount; i++)
            {
                pooledList.Add(myList.GetRandom());
            }

            return myList;
        }
        public static T GetRandom<T>(this IList<T> list)
        {
            return list.Count == 0 ? default : list[RandomSys.Next(0, list.Count)];
        }

        public static bool IsBetween(this float value, float start, float end)
        {
            var max = Math.Max(start, end);
            var min = Math.Min(start, end);

            return value >= min && value <= max;
        }

        public static T PopRandom<T>(this List<T> myList)
        {
            if (myList == null || myList.Count < 0) throw new ArgumentException("List is either null or has length of 0."); 
            int index = random.Range(0, myList.Count);
            T element = Pop(myList, index);
        
            return element;
        }
    
    
        public static T Pop<T>(this List<T> myList, int index)
        {
            if (myList == null || myList.Count < 0) throw new ArgumentException("List is either null or has length of 0."); 

            T element = myList[index];
            myList.RemoveAt(index);

            return element;
        }
    


        public static bool RemoveIfExists<T>(this List<T> myList, T element)  where T: Collider
        {
            for (int i = 0; i < myList.Count; i++)
            {
                if (element == myList[i])
                {
                    myList.RemoveAt(i);
                    return true;
                }

            }
            return false;
        }
        public static float Remap(this float value, float from1, float to1, float from2, float to2)
        {
            return (value - from1) / (to1 - from1) * (to2 - from2) + from2;
        }

        public static int GetFirstLayer(LayerMask layerMask)
        {
            int layerNumber = -1;
            int layer = layerMask.value;

            while (layer > 0)
            {
                layer = layer >> 1;
                layerNumber++;
            }

            return layerNumber;
        }

        public static Vector3 GetPositive(this Vector3 vector3)
        {
            return new Vector3(Mathf.Abs(vector3.x), Mathf.Abs(vector3.y), Mathf.Abs(vector3.z));
        }

        public static Texture2D ResizeImage(Texture2D texture2D, int targetX, int targetY)
        {
            RenderTexture rt = new RenderTexture(targetX, targetY, 24);
            RenderTexture.active = rt;

            Graphics.Blit(texture2D, rt);

            Texture2D result = new Texture2D(targetX, targetY);
            result.ReadPixels(new Rect(0, 0, targetX, targetY), 0, 0);
            result.Apply();

            return result;
        }

        public static ParticleSystem DoParticle(ParticleSystem particle, Vector3 position, Quaternion rotation, float scaleMutipilier, float killDelay = -1f, Transform parent = null, bool worldPositionsStays = true)
        {
            ParticleSystem particleSystem = UnityEngine.Object.Instantiate(particle, position, rotation);
            particleSystem.transform.SetParent(parent, worldPositionsStays);
            particleSystem.transform.localScale *= scaleMutipilier;
            particleSystem.Play();

            if (killDelay != -1f)
            {
                UnityEngine.Object.Destroy(particleSystem.gameObject, killDelay);
            }

            particleSystem.tag = "destroyOnInit";

            return particleSystem;
        }

        [Serializable]
        public class SerializableList<T>
        {
            public List<T> list = new List<T>();
        }

        public static T ImportJson<T>(string path)
        {
            TextAsset textAsset = Resources.Load<TextAsset>(path);
            return JsonUtility.FromJson<T>(textAsset.text);
        }

        public static Bounds GetMeshRenderersBounds(List<MeshRenderer> meshRenderers)
        {
            Bounds combinedBounds = new Bounds();

            foreach (MeshRenderer renderer in meshRenderers)
            {
                combinedBounds.Encapsulate(renderer.bounds);
            }

            return combinedBounds;
        }

        public static Bounds GetMeshRenderersBounds(MeshRenderer[] meshRenderers)
        {
            Bounds combinedBounds = meshRenderers[0].bounds;

            foreach (MeshRenderer renderer in meshRenderers)
            {
                combinedBounds.Encapsulate(renderer.bounds);
            }

            return combinedBounds;
        }

        public static Bounds GetMeshRenderersLocalBounds(MeshRenderer[] meshRenderers)
        {
            Bounds combinedBounds = new Bounds();

            foreach (MeshRenderer renderer in meshRenderers)
            {
                combinedBounds.Encapsulate(renderer.localBounds);
            }

            return combinedBounds;
        }

        public static GameObject[] GetChildGameObjects(Transform parent)
        {
            GameObject[] childGOs = new GameObject[parent.childCount + 1];

            childGOs[0] = parent.gameObject;

            for(int i = 0; i < parent.childCount; i++)
            {
                childGOs[i] = parent.GetChild(i).gameObject;
            }

            return childGOs;
        }

        public static Material[] CreateMeshRendererMaterialInstances(ref MeshRenderer[] meshRenderers)
        {
            List<Material> materials = new List<Material>();

            foreach (MeshRenderer renderer in meshRenderers)
            {
                for(int i = 0; i < renderer.materials.Length; i++)
                {
                    Material mat = new Material(renderer.materials[i]);
                    renderer.materials[i] = mat;
                    materials.Add(mat);
                }
            }

            return materials.ToArray();
        }

        public static Material[] CreateMeshRendererMaterialInstances(ref List<MeshRenderer> meshRenderers)
        {
            List<Material> materials = new List<Material>();

            foreach (MeshRenderer renderer in meshRenderers)
            {
                Material mat = new Material(renderer.material);
                renderer.material = mat;
                materials.Add(mat);
            }

            return materials.ToArray();
        }

        public static Vector3 WorldToCanvasPosition(this Canvas canvas, Vector3 worldPosition, Camera camera = null)
        {
            if (camera == null)
            {
                camera = Camera.main;
            }
            var viewportPosition = camera.WorldToViewportPoint(worldPosition);
            return canvas.ViewportToCanvasPosition(viewportPosition);
        }

        public static Vector3 ScreenToCanvasPosition(this Canvas canvas, Vector3 screenPosition)
        {
            var viewportPosition = new Vector3(screenPosition.x / Screen.width,
                screenPosition.y / Screen.height,
                0);
            return canvas.ViewportToCanvasPosition(viewportPosition);
        }

        public static Vector3 ViewportToCanvasPosition(this Canvas canvas, Vector3 viewportPosition)
        {
            var centerBasedViewPortPosition = viewportPosition - new Vector3(0.5f, 0.5f, 0);
            var canvasRect = canvas.GetComponent<RectTransform>();
            var scale = canvasRect.sizeDelta;
            return Vector3.Scale(centerBasedViewPortPosition, scale);
        }

        public static Vector2 RotateBy(Vector2 v, float a, bool bUseRadians = false)
        {
            if (!bUseRadians) a *= Mathf.Deg2Rad;
            var ca = System.Math.Cos(a);
            var sa = System.Math.Sin(a);
            var rx = v.x * ca - v.y * sa;

            return new Vector2((float)rx, (float)(v.x * sa + v.y * ca));
        }

        public static void RotateBy(ref Vector2 v, float a, bool bUseRadians = false)
        {
            if (!bUseRadians) a *= Mathf.Deg2Rad;
            var ca = System.Math.Cos(a);
            var sa = System.Math.Sin(a);
            var rx = v.x * ca - v.y * sa;

            v.x = (float)rx;
            v.y = (float)(v.x * sa + v.y * ca);
        }

        public static class EasingFunctions
        {
            public static float Linear(float t) => t;

            public static float InQuad(float t) => t * t;
            public static float OutQuad(float t) => 1 - InQuad(1 - t);
            public static float InOutQuad(float t)
            {
                if (t < 0.5) return InQuad(t * 2) / 2;
                return 1 - InQuad((1 - t) * 2) / 2;
            }

            public static float InCubic(float t) => t * t * t;
            public static float OutCubic(float t) => 1 - InCubic(1 - t);
            public static float InOutCubic(float t)
            {
                if (t < 0.5) return InCubic(t * 2) / 2;
                return 1 - InCubic((1 - t) * 2) / 2;
            }

            public static float InQuart(float t) => t * t * t * t;
            public static float OutQuart(float t) => 1 - InQuart(1 - t);
            public static float InOutQuart(float t)
            {
                if (t < 0.5) return InQuart(t * 2) / 2;
                return 1 - InQuart((1 - t) * 2) / 2;
            }

            public static float InQuint(float t) => t * t * t * t * t;
            public static float OutQuint(float t) => 1 - InQuint(1 - t);
            public static float InOutQuint(float t)
            {
                if (t < 0.5) return InQuint(t * 2) / 2;
                return 1 - InQuint((1 - t) * 2) / 2;
            }

            public static float InSine(float t) => (float)-Math.Cos(t * Math.PI / 2);
            public static float OutSine(float t) => (float)Math.Sin(t * Math.PI / 2);
            public static float InOutSine(float t) => (float)(Math.Cos(t * Math.PI) - 1) / -2;

            public static float InExpo(float t) => (float)Math.Pow(2, 10 * (t - 1));
            public static float OutExpo(float t) => 1 - InExpo(1 - t);
            public static float InOutExpo(float t)
            {
                if (t < 0.5) return InExpo(t * 2) / 2;
                return 1 - InExpo((1 - t) * 2) / 2;
            }

            public static float InCirc(float t) => -((float)Math.Sqrt(1 - t * t) - 1);
            public static float OutCirc(float t) => 1 - InCirc(1 - t);
            public static float InOutCirc(float t)
            {
                if (t < 0.5) return InCirc(t * 2) / 2;
                return 1 - InCirc((1 - t) * 2) / 2;
            }

            public static float InElastic(float t) => 1 - OutElastic(1 - t);
            public static float OutElastic(float t)
            {
                float p = 0.3f;
                return (float)Math.Pow(2, -10 * t) * (float)Math.Sin((t - p / 4) * (2 * Math.PI) / p) + 1;
            }
            public static float InOutElastic(float t)
            {
                if (t < 0.5) return InElastic(t * 2) / 2;
                return 1 - InElastic((1 - t) * 2) / 2;
            }

            public static float InBack(float t)
            {
                float s = 1.70158f;
                return t * t * ((s + 1) * t - s);
            }
            public static float OutBack(float t) => 1 - InBack(1 - t);
            public static float InOutBack(float t)
            {
                if (t < 0.5) return InBack(t * 2) / 2;
                return 1 - InBack((1 - t) * 2) / 2;
            }

            public static float InBounce(float t) => 1 - OutBounce(1 - t);
            public static float OutBounce(float t)
            {
                float div = 2.75f;
                float mult = 7.5625f;

                if (t < 1 / div)
                {
                    return mult * t * t;
                }
                else if (t < 2 / div)
                {
                    t -= 1.5f / div;
                    return mult * t * t + 0.75f;
                }
                else if (t < 2.5 / div)
                {
                    t -= 2.25f / div;
                    return mult * t * t + 0.9375f;
                }
                else
                {
                    t -= 2.625f / div;
                    return mult * t * t + 0.984375f;
                }
            }
            public static float InOutBounce(float t)
            {
                if (t < 0.5) return InBounce(t * 2) / 2;
                return 1 - InBounce((1 - t) * 2) / 2;
            }
        }
    
    
        public static Vector2 CalculateFocusedScrollPosition(this ScrollRect scrollView, Vector2 focusPoint)
        {
            Vector2 contentSize = scrollView.content.rect.size;
            Vector2 viewportSize = ((RectTransform)scrollView.content.parent).rect.size;
            Vector2 contentScale = scrollView.content.localScale;

            contentSize.Scale(contentScale);
            focusPoint.Scale(contentScale);

            Vector2 scrollPosition = scrollView.normalizedPosition;
            if (scrollView.horizontal && contentSize.x > viewportSize.x)
                scrollPosition.x = Mathf.Clamp01((focusPoint.x - viewportSize.x * 0.5f) / (contentSize.x - viewportSize.x));
            if (scrollView.vertical && contentSize.y > viewportSize.y)
                scrollPosition.y = Mathf.Clamp01((focusPoint.y - viewportSize.y * 0.5f) / (contentSize.y - viewportSize.y));

            return scrollPosition;
        }

        public static Vector2 CalculateFocusedScrollPosition(this ScrollRect scrollView, RectTransform item)
        {
            Vector2 itemCenterPoint = scrollView.content.InverseTransformPoint(item.transform.TransformPoint(item.rect.center));

            Vector2 contentSizeOffset = scrollView.content.rect.size;
            contentSizeOffset.Scale(scrollView.content.pivot);

            return scrollView.CalculateFocusedScrollPosition(itemCenterPoint + contentSizeOffset);
        }

        public static void FocusAtPoint(this ScrollRect scrollView, Vector2 focusPoint)
        {
            scrollView.normalizedPosition = scrollView.CalculateFocusedScrollPosition(focusPoint);
        }

        public static void FocusOnItem(this ScrollRect scrollView, RectTransform item)
        {
            scrollView.normalizedPosition = scrollView.CalculateFocusedScrollPosition(item);
        }

        private static IEnumerator LerpToScrollPositionCoroutine(this ScrollRect scrollView, Vector2 targetNormalizedPos, float speed)
        {
            Vector2 initialNormalizedPos = scrollView.normalizedPosition;

            float t = 0f;
            while (t < 1f)
            {
                scrollView.normalizedPosition = Vector2.LerpUnclamped(initialNormalizedPos, targetNormalizedPos, 1f - (1f - t) * (1f - t));

                yield return null;
                t += speed * Time.unscaledDeltaTime;
            }

            scrollView.normalizedPosition = targetNormalizedPos;
        }

        public static IEnumerator FocusAtPointCoroutine(this ScrollRect scrollView, Vector2 focusPoint, float speed)
        {
            yield return scrollView.LerpToScrollPositionCoroutine(scrollView.CalculateFocusedScrollPosition(focusPoint), speed);
        }

        public static IEnumerator FocusOnItemCoroutine(this ScrollRect scrollView, RectTransform item, float speed)
        {
            yield return scrollView.LerpToScrollPositionCoroutine(scrollView.CalculateFocusedScrollPosition(item), speed);
        }
    
        public static string FormatNumber(long num)
        {
            // Ensure number has max 3 significant digits (no rounding up can happen)
            long i = (long)Mathf.Pow(10, (int)Mathf.Max(0, Mathf.Log10(num) - 2));
            num = num / i * i;

            if (num >= 1000000000)
                return (num / 1000000000D).ToString("0.##") + "B";
            if (num >= 1000000)
                return (num / 1000000D).ToString("0.##") + "M";
            if (num >= 1000)
                return (num / 1000D).ToString("0.##") + "K";

            return num.ToString("#,0");
        }
        public static int IndexOf<T>(this List<T> list, Func<T, bool> predicate)
        {
            if (list == null)
            {
                throw new ArgumentNullException(nameof(list));
            }

            if (predicate == null)
            {
                throw new ArgumentNullException(nameof(predicate));
            }

            for (int i = 0; i < list.Count; i++)
            {
                if (predicate(list[i]))
                {
                    return i; // Found a match
                }
            }

            return -1; // No match found
        }

        public static void NormalizeScale(GameObject obj, float targetUnitSize = 1.0f)
        {
            if (obj != null)
            {
                // Store the original position of the object
                Vector3 originalPosition = obj.transform.position;

                // Get all MeshRenderer components in the GameObject and its children
                MeshRenderer[] meshRenderers = obj.GetComponentsInChildren<MeshRenderer>(true);

                if (meshRenderers.Length > 0)
                {
                    // Calculate the combined bounds of all mesh renderers
                    Bounds combinedBounds = new Bounds(meshRenderers[0].bounds.center, meshRenderers[0].bounds.size);
                    for (int i = 1; i < meshRenderers.Length; i++)
                    {
                        combinedBounds.Encapsulate(meshRenderers[i].bounds);
                    }

                    // Get the diagonal length of the combined bounds
                    float diagonalLength = combinedBounds.size.magnitude;

                    // Get the initial scale of the object
                    Vector3 initialScale = obj.transform.localScale;

                    // Calculate the scaling factors to fit the object inside a unit cube
                    float scaleFactorX = (initialScale.x / diagonalLength) * targetUnitSize;
                    float scaleFactorY = (initialScale.y / diagonalLength) * targetUnitSize;
                    float scaleFactorZ = (initialScale.z / diagonalLength) * targetUnitSize;

                    // Apply the scaling factors
                    obj.transform.localScale = new Vector3(scaleFactorX, scaleFactorY, scaleFactorZ);

                    // Restore the original position
                    obj.transform.position = originalPosition;
                }
                else
                {
                    Debug.LogError("No MeshRenderer components found on the GameObject or its children.");
                }
            }
            else
            {
                Debug.LogError("GameObject is null. Please provide a valid GameObject.");
            }
        }

        public static int GetMaxEnumValue(params Enum[] enumValues)
        {
            if (enumValues == null || enumValues.Length == 0)
                throw new ArgumentException("At least one enum value must be provided");

            int maxValue = int.MinValue;
            foreach (Enum value in enumValues)
            {
                int intValue = Convert.ToInt32(value);
                if (intValue > maxValue)
                    maxValue = intValue;
            }
            return maxValue;
        }
    
        /// <summary>
        /// Given the initial position in world you get closest collider position from collection of colliders.
        /// </summary>
        /// <returns>Returns closest (stick) position one collider. If the Length of </returns>
        public static Vector3 GetStickPositionOnColliders<T>(this ICollection<T> colliders, Vector3 initialPosition) where T : Collider
        {
            var stickPosition = initialPosition;
            if (colliders.Count <= 0) return stickPosition;
            var minDistance = float.MaxValue;

            foreach (var collider in colliders)
            {
                var closestPointOnCollider = collider.ClosestPoint(initialPosition);
                var sqrtDistance = (closestPointOnCollider - initialPosition).sqrMagnitude;
                if (sqrtDistance < minDistance)
                {
                    minDistance = sqrtDistance;
                    stickPosition = closestPointOnCollider;
                }
            }
        
            return stickPosition;
        }
        
         // Get UI Position from World Position
        public static Vector2 GetWorldUIPosition(Vector3 worldPosition, Transform parent, Camera uiCamera, Camera worldCamera) {
            Vector3 screenPosition = worldCamera.WorldToScreenPoint(worldPosition);
            Vector3 uiCameraWorldPosition = uiCamera.ScreenToWorldPoint(screenPosition);
            Vector3 localPos = parent.InverseTransformPoint(uiCameraWorldPosition);
            return new Vector2(localPos.x, localPos.y);
        }

        public static Vector3 GetWorldPositionFromUIZeroZ() {
            Vector3 vec = GetWorldPositionFromUI(Input.mousePosition, Camera.main);
            vec.z = 0f;
            return vec;
        }

        // Get World Position from UI Position
        public static Vector3 GetWorldPositionFromUI() {
            return GetWorldPositionFromUI(Input.mousePosition, Camera.main);
        }

        public static Vector3 GetWorldPositionFromUI(Camera worldCamera) {
            return GetWorldPositionFromUI(Input.mousePosition, worldCamera);
        }

        public static Vector3 GetWorldPositionFromUI(Vector3 screenPosition, Camera worldCamera) {
            Vector3 worldPosition = worldCamera.ScreenToWorldPoint(screenPosition);
            return worldPosition;
        }
    
        public static Vector3 GetWorldPositionFromUI_Perspective() {
            return GetWorldPositionFromUI_Perspective(Input.mousePosition, Camera.main);
        }

        public static Vector3 GetWorldPositionFromUI_Perspective(Camera worldCamera) {
            return GetWorldPositionFromUI_Perspective(Input.mousePosition, worldCamera);
        }

        public static Vector3 GetWorldPositionFromUI_Perspective(Vector3 screenPosition, Camera worldCamera) {
            Ray ray = worldCamera.ScreenPointToRay(screenPosition);
            Plane xy = new Plane(Vector3.forward, new Vector3(0, 0, 0f));
            float distance;
            xy.Raycast(ray, out distance);
            return ray.GetPoint(distance);
        }
        
        // Draw a UI Sprite
        public static RectTransform DrawSprite(Color color, Transform parent, Vector2 pos, Vector2 size, string name = null) {
            RectTransform rectTransform = DrawSprite(null, color, parent, pos, size, name);
            return rectTransform;
        }
        
        // Draw a UI Sprite
        public static RectTransform DrawSprite(Sprite sprite, Transform parent, Vector2 pos, Vector2 size, string name = null) {
            RectTransform rectTransform = DrawSprite(sprite, Color.white, parent, pos, size, name);
            return rectTransform;
        }
        
        // Draw a UI Sprite
        public static RectTransform DrawSprite(Sprite sprite, Color color, Transform parent, Vector2 pos, Vector2 size, string name = null) {
            // Setup icon
            if (name == null || name == "") name = "Sprite";
            GameObject go = new GameObject(name, typeof(RectTransform), typeof(Image));
            RectTransform goRectTransform = go.GetComponent<RectTransform>();
            goRectTransform.SetParent(parent, false);
            goRectTransform.sizeDelta = size;
            goRectTransform.anchoredPosition = pos;

            Image image = go.GetComponent<Image>();
            image.sprite = sprite;
            image.color = color;

            return goRectTransform;
        }

        public static Text DrawTextUI(string textString, Vector2 anchoredPosition, int fontSize, Font font) {
            return DrawTextUI(textString, GetCanvasTransform(), anchoredPosition, fontSize, font);
        }

        public static Text DrawTextUI(string textString, Transform parent, Vector2 anchoredPosition, int fontSize, Font font) {
            GameObject textGo = new GameObject("Text", typeof(RectTransform), typeof(Text));
            textGo.transform.SetParent(parent, false);
            Transform textGoTrans = textGo.transform;
            textGoTrans.SetParent(parent, false);
            textGoTrans.localPosition = Vector3zero;
            textGoTrans.localScale = Vector3one;

            RectTransform textGoRectTransform = textGo.GetComponent<RectTransform>();
            textGoRectTransform.sizeDelta = new Vector2(0,0);
            textGoRectTransform.anchoredPosition = anchoredPosition;

            Text text = textGo.GetComponent<Text>();
            text.text = textString;
            text.verticalOverflow = VerticalWrapMode.Overflow;
            text.horizontalOverflow = HorizontalWrapMode.Overflow;
            text.alignment = TextAnchor.MiddleLeft;
            if (font == null) font = GetDefaultFont();
            text.font = font;
            text.fontSize = fontSize;

            return text;
        }


        // Parse a float, return default if failed
	    public static float Parse_Float(string txt, float _default) {
		    float f;
		    if (!float.TryParse(txt, out f)) {
			    f = _default;
		    }
		    return f;
	    }
        
        // Parse a int, return default if failed
	    public static int Parse_Int(string txt, int _default) {
		    int i;
		    if (!int.TryParse(txt, out i)) {
			    i = _default;
		    }
		    return i;
	    }
	    public static int Parse_Int(string txt) {
            return Parse_Int(txt, -1);
	    }



        // Get Mouse Position in World with Z = 0f
        public static Vector3 GetMouseWorldPosition() {
            Vector3 vec = GetMouseWorldPositionWithZ(Input.mousePosition, Camera.main);
            vec.z = 0f;
            return vec;
        }
        public static Vector3 GetMouseWorldPositionWithZ() {
            return GetMouseWorldPositionWithZ(Input.mousePosition, Camera.main);
        }
        public static Vector3 GetMouseWorldPositionWithZ(Camera worldCamera) {
            return GetMouseWorldPositionWithZ(Input.mousePosition, worldCamera);
        }
        public static Vector3 GetMouseWorldPositionWithZ(Vector3 screenPosition, Camera worldCamera) {
            Vector3 worldPosition = worldCamera.ScreenToWorldPoint(screenPosition);
            return worldPosition;
        }
        

        // Is Mouse over a UI Element? Used for ignoring World clicks through UI
        public static bool IsPointerOverUI() {
            if (EventSystem.current.IsPointerOverGameObject()) {
                return true;
            } else {
                PointerEventData pe = new PointerEventData(EventSystem.current);
                pe.position =  Input.mousePosition;
                List<RaycastResult> hits = new List<RaycastResult>();
                EventSystem.current.RaycastAll( pe, hits );
                return hits.Count > 0;
            }
        }


        
		// Returns 00-FF, value 0->255
	    public static string Dec_to_Hex(int value) {
		    return value.ToString("X2");
	    }

		// Returns 0-255
	    public static int Hex_to_Dec(string hex) {
		    return Convert.ToInt32(hex, 16);
	    }
        
		// Returns a hex string based on a number between 0->1
	    public static string Dec01_to_Hex(float value) {
		    return Dec_to_Hex((int)Mathf.Round(value*255f));
	    }

		// Returns a float between 0->1
	    public static float Hex_to_Dec01(string hex) {
		    return Hex_to_Dec(hex)/255f;
	    }

        // Get Hex Color FF00FF
	    public static string GetStringFromColor(Color color) {
		    string red = Dec01_to_Hex(color.r);
		    string green = Dec01_to_Hex(color.g);
		    string blue = Dec01_to_Hex(color.b);
		    return red+green+blue;
	    }
        
        // Get Hex Color FF00FFAA
	    public static string GetStringFromColorWithAlpha(Color color) {
		    string alpha = Dec01_to_Hex(color.a);
		    return GetStringFromColor(color)+alpha;
	    }

        // Sets out values to Hex String 'FF'
	    public static void GetStringFromColor(Color color, out string red, out string green, out string blue, out string alpha) {
		    red = Dec01_to_Hex(color.r);
		    green = Dec01_to_Hex(color.g);
		    blue = Dec01_to_Hex(color.b);
		    alpha = Dec01_to_Hex(color.a);
	    }
        
        // Get Hex Color FF00FF
	    public static string GetStringFromColor(float r, float g, float b) {
		    string red = Dec01_to_Hex(r);
		    string green = Dec01_to_Hex(g);
		    string blue = Dec01_to_Hex(b);
		    return red+green+blue;
	    }
        
        // Get Hex Color FF00FFAA
	    public static string GetStringFromColor(float r, float g, float b, float a) {
		    string alpha = Dec01_to_Hex(a);
		    return GetStringFromColor(r,g,b)+alpha;
	    }
        
        // Get Color from Hex string FF00FFAA
	    public static Color GetColorFromString(string color) {
		    float red = Hex_to_Dec01(color.Substring(0,2));
		    float green = Hex_to_Dec01(color.Substring(2,2));
		    float blue = Hex_to_Dec01(color.Substring(4,2));
            float alpha = 1f;
            if (color.Length >= 8) {
                // Color string contains alpha
                alpha = Hex_to_Dec01(color.Substring(6,2));
            }
		    return new Color(red, green, blue, alpha);
	    }


        // Generate random normalized direction
        public static Vector3 GetRandomDir() {
            return new Vector3(UnityEngine.Random.Range(-1f,1f), UnityEngine.Random.Range(-1f,1f)).normalized;
        }
        

        public static Vector3 GetVectorFromAngle(int angle) {
            // angle = 0 -> 360
            float angleRad = angle * (Mathf.PI/180f);
            return new Vector3(Mathf.Cos(angleRad), Mathf.Sin(angleRad));
        }

        public static float GetAngleFromVectorFloat(Vector3 dir) {
            dir = dir.normalized;
            float n = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            if (n < 0) n += 360;

            return n;
        }

        public static int GetAngleFromVector(Vector3 dir) {
            dir = dir.normalized;
            float n = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            if (n < 0) n += 360;
            int angle = Mathf.RoundToInt(n);

            return angle;
        }

        public static int GetAngleFromVector180(Vector3 dir) {
            dir = dir.normalized;
            float n = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            int angle = Mathf.RoundToInt(n);

            return angle;
        }

        public static Vector3 ApplyRotationToVector(Vector3 vec, Vector3 vecRotation) {
            return ApplyRotationToVector(vec, GetAngleFromVectorFloat(vecRotation));
        }

        public static Vector3 ApplyRotationToVector(Vector3 vec, float angle) {
            return Quaternion.Euler(0,0,angle) * vec;
        }
        
        // Create Text in the World
        public static TextMesh CreateWorldText(string text, Transform parent = null, Vector3 localPosition = default(Vector3), int fontSize = 40, Color? color = null, TextAnchor textAnchor = TextAnchor.UpperLeft, TextAlignment textAlignment = TextAlignment.Left, int sortingOrder = sortingOrderDefault) {
            if (color == null) color = Color.white;
            return CreateWorldText(parent, text, localPosition, fontSize, (Color)color, textAnchor, textAlignment, sortingOrder);
        }
        
        // Create Text in the World
        public static TextMesh CreateWorldText(Transform parent, string text, Vector3 localPosition, int fontSize, Color color, TextAnchor textAnchor, TextAlignment textAlignment, int sortingOrder) {
            GameObject gameObject = new GameObject("World_Text", typeof(TextMesh));
            Transform transform = gameObject.transform;
            transform.SetParent(parent, false);
            transform.localPosition = localPosition;
            TextMesh textMesh = gameObject.GetComponent<TextMesh>();
            textMesh.anchor = textAnchor;
            textMesh.alignment = textAlignment;
            textMesh.text = text;
            textMesh.fontSize = fontSize;
            textMesh.color = color;
            textMesh.GetComponent<MeshRenderer>().sortingOrder = sortingOrder;
            return textMesh;
        }
        
        private static readonly Vector3 Vector3zero = Vector3.zero;
        private static readonly Vector3 Vector3one = Vector3.one;
        private static readonly Vector3 Vector3yDown = new Vector3(0,-1);

        public const int sortingOrderDefault = 5000;
        
        // Get Sorting order to set SpriteRenderer sortingOrder, higher position = lower sortingOrder
        public static int GetSortingOrder(Vector3 position, int offset, int baseSortingOrder = sortingOrderDefault) {
            return (int)(baseSortingOrder - position.y) + offset;
        }


        // Get Main Canvas Transform
        private static Transform cachedCanvasTransform;
        public static Transform GetCanvasTransform() {
            if (cachedCanvasTransform == null) {
                Canvas canvas = MonoBehaviour.FindObjectOfType<Canvas>();
                if (canvas != null) {
                    cachedCanvasTransform = canvas.transform;
                }
            }
            return cachedCanvasTransform;
        }

        // Get Default Unity Font, used in text objects if no font given
        public static Font GetDefaultFont() {
            return Resources.GetBuiltinResource<Font>("Arial.ttf");
        }


        // Create a Sprite in the World, no parent
        public static GameObject CreateWorldSprite(string name, Sprite sprite, Vector3 position, Vector3 localScale, int sortingOrder, Color color) {
            return CreateWorldSprite(null, name, sprite, position, localScale, sortingOrder, color);
        }
        
        // Create a Sprite in the World
        public static GameObject CreateWorldSprite(Transform parent, string name, Sprite sprite, Vector3 localPosition, Vector3 localScale, int sortingOrder, Color color) {
            GameObject gameObject = new GameObject(name, typeof(SpriteRenderer));
            Transform transform = gameObject.transform;
            transform.SetParent(parent, false);
            transform.localPosition = localPosition;
            transform.localScale = localScale;
            SpriteRenderer spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
            spriteRenderer.sprite = sprite;
            spriteRenderer.sortingOrder = sortingOrder;
            spriteRenderer.color = color;
            return gameObject;
        }



    }
}