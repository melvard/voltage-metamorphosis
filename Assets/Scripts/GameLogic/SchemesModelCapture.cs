using System.Collections.Generic;
using System.Threading;
using Canvas;
using Cysharp.Threading.Tasks;
using Misc;
using Schemes;
using Schemes.Dashboard;
using Schemes.Device;
using UnityEngine;

namespace GameLogic
{
    public class SchemesModelCapture : MonoBehaviour
    {
        [SerializeField] private Texture2D placeHolderTexture;
        [SerializeField] private LayerMask schemeRenderingCaptureLayerMask;
        [SerializeField] private Camera schemeCaptureCamera;
        [SerializeField] private Transform container;
        [SerializeField] private SchemeDevice schemeDeviceRef;
        
        private SchemeDevice SchemeDeviceRef => EditorDashboard.Instance.SchemeEditor_Debug.GetSchemeDeviceReference();
        public async UniTask CaptureSchemesRenderTextures(List<Scheme> schemes, CancellationToken ct)
        {
            SchemesSaverLoader.OnSchemeAdded += OnSchemeEditedOrAddedHandler;
            SchemesSaverLoader.OnSchemeEdited += OnSchemeEditedOrAddedHandler;
            // your code here for capturing 3d model and assigning them in render texture
            foreach (var scheme in schemes)
            {
                var device = Instantiate(schemeDeviceRef, container);
                device.transform.localPosition = Vector3.zero;
                device.Init(scheme, -1);
                scheme.SchemeData.SchemeVisualsData.UITexture2D = await CaptureSingleObject(device.gameObject, false);;
                Destroy(device.gameObject);
            } 
        }

        private async void OnSchemeEditedOrAddedHandler(SchemeInteractionEventArgs arg0)
        {
            var device = Instantiate(schemeDeviceRef, container);
            device.transform.localPosition = Vector3.zero;
            device.Init(arg0.scheme, -1);
            arg0.scheme.SchemeData.SchemeVisualsData.UITexture2D = await CaptureSingleObject(device.gameObject, false);
            arg0.scheme.SchemeData.SchemeVisualsData.PendingForTextureCapture = false;
            Destroy(device.gameObject);
        }

        // [Button("Screenshot Current Objects")]
        // public void CaptureMoveAndReset(List<GameObject> objectsToCapture, List<string> objectNames)
        // {
        //     StartCoroutine(CaptureObjectsCoroutine(objectsToCapture, objectNames));
        // }
        
        // private async UniTask<List<Texture2D>> CaptureObjectsCoroutine(List<GameObject> objectsToCapture, List<string> objectNames)
        // {
        //     int counter = 0;
        //     List<Texture2D> capturedTextures = new List<Texture2D>();
        //
        //     for (int i = 0; i < objectsToCapture.Count; i++)
        //     {
        //         GameObject objectToCapture = objectsToCapture[i];
        //         string objectName = objectNames[i];
        //
        //         var texture2D = await CaptureSingleObject(objectToCapture, objectName, counter);
        //         capturedTextures.Add(texture2D);
        //         counter++;
        //     }
        //
        //     return capturedTextures;
        // }

        private async UniTask<Texture2D> CaptureSingleObject(GameObject objectToCapture, bool shouldInstantiateFromCaptureObject)
        {
            // Position the cloned object a little forward from the camera at y position 1000
            GameObject clonedObject = objectToCapture;
            if (shouldInstantiateFromCaptureObject)
            {
                clonedObject = Instantiate(objectToCapture);
                // Hide originalObject
                objectToCapture.SetActive(false);
            }
            
            //clonedObject.transform.rotation = Quaternion.identity;

            Utilities.NormalizeScale(clonedObject);

            clonedObject.SetLayerRecursively(schemeRenderingCaptureLayerMask);

            // Use a temporary camera for rendering
            Camera tempCamera = new GameObject("TempCamera").AddComponent<Camera>();
            tempCamera.CopyFrom(schemeCaptureCamera); // Copy the settings from the original camera
            tempCamera.orthographic = true;
            tempCamera.orthographicSize = 0.65f;
            
            tempCamera.transform.position = schemeCaptureCamera.transform.position;
            // Set up the temporary camera for a transparent background
            tempCamera.clearFlags = CameraClearFlags.SolidColor;
            tempCamera.backgroundColor = new Color(0, 0, 0, 0);
            int screenshotWidth = 256;
            int screenshotHeight = 256;
            tempCamera.targetTexture = new RenderTexture(screenshotWidth, screenshotHeight, 24);

            //tempCamera.transform.SetParent(clonedObject.transform);
            //tempCamera.transform.position -= Vector3.forward * 2;
            //tempCamera.transform.rotation = transform.rotation;

            // Render the screenshot
            tempCamera.Render();

            // Introduce a slight delay before capturing the screenshot
            await UniTask.Yield();

            // Create a Texture2D and read pixels from the RenderTexture
            Texture2D screenshot = new Texture2D(screenshotWidth, screenshotHeight, TextureFormat.ARGB32, false);
            RenderTexture.active = tempCamera.targetTexture;
            screenshot.ReadPixels(new Rect(0, 0, screenshotWidth, screenshotHeight), 0, 0);
            screenshot.Apply();

            Destroy(tempCamera.gameObject);

            return screenshot;
            // // Save the screenshot in the Assets folder with a unique name
            // byte[] bytes = screenshot.EncodeToPNG();
            // string screenshotPath = $"Assets/Screenshots/{screenShotName}.png";
            // System.IO.F
            // return screenshot;
        }
    }
    
    
}