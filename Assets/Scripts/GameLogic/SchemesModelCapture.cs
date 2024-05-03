using System.Collections.Generic;
using Schemes;
using Schemes.Dashboard;
using Schemes.Device;
using UnityEngine;

namespace GameLogic
{
    public class SchemesModelCapture : MonoBehaviour
    {
        [SerializeField] private LayerMask schemeRenderingCaptureLayerMask;
        [SerializeField] private Camera schemeCaptureCamera;
        [SerializeField] private Transform container;
        [SerializeField] private SchemeDevice schemeDeviceRef;
        
        private SchemeDevice SchemeDeviceRef => EditorDashboard.Instance.SchemeEditor_Debug.GetSchemeDeviceReference();
        public void CaptureSchemesRenderTextures(List<Scheme> schemes)
        {
            // your code here for capturing 3d model and assigning them in render texture
            foreach (var scheme in schemes)
            {
                // empty render texture
                // var schemeDevice = Instantiate(SchemeDeviceRef, elementContainer);
                // schemeDevice.Init(scheme, -1); // having no idea what -1 will cause
                int width = 1920; // Set the width of the render texture
                int height = 1080; // Set the height of the render texture
                RenderTextureFormat format = RenderTextureFormat.ARGB32; // Set the format of the render texture
                int depth = 0; // Set the depth buffer of the render texture (0 means no depth buffer)

                // Create a render texture descriptor with the specified properties
                RenderTextureDescriptor descriptor = new RenderTextureDescriptor(width, height, format, depth);

                // Create the render texture using the descriptor
                var renderTexture = new RenderTexture(descriptor);
        
                // Set any additional properties of the render texture as needed
                renderTexture.filterMode = FilterMode.Bilinear; // Set the filter mode of the render texture
                renderTexture.autoGenerateMips = false; // Disable mipmaps generation for the render texture
                renderTexture.useMipMap = false; // Disable using mipmaps for the render texture
                // Set any other properties as needed

                scheme.UIRenderTexture = renderTexture;
            } 
        }
    }
}