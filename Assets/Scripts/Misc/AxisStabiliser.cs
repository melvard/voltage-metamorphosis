using Sirenix.OdinInspector;
using UnityEngine;

namespace Misc
{
    public class AxisStabiliser : MonoBehaviour
    {
        [SerializeField] private bool smoothStabilization;

        [ShowIf("smoothStabilization")] [SerializeField]
        private float smoothDampTime;
    
        [HorizontalGroup("Axis Toggles")] [SerializeField][LabelText("X")]
        private bool stabilizeX;
        [HorizontalGroup("Axis Toggles")] [SerializeField][LabelText("Y")]
        private bool stabilizeY;
        [HorizontalGroup("Axis Toggles")] [SerializeField] [LabelText("Z")]
        private bool stabilizeZ;
    
        [HorizontalGroup("Axis world values")] [SerializeField][ShowIf("stabilizeX")]
        private float x;
        [HorizontalGroup("Axis world values")] [SerializeField][ShowIf("stabilizeY")]
        private float y;
        [HorizontalGroup("Axis world values")] [SerializeField][ShowIf("stabilizeZ")]
        private float z;


        private Vector3 _smoothDampCurrentVelocity;

        private float _smoothDampXVelocity;
        private float _smoothDampYVelocity;
        private float _smoothDampZVelocity;
        void Update()
        {
            var position = transform.position;
            float xValue = position.x;
            float yValue = position.y;
            float zValue = position.z;
            if (stabilizeX)
            {
                xValue = smoothStabilization ? GetDampedValueOnXAxis(xValue, x) : x;
            }
            if (stabilizeY)
            {
                yValue = smoothStabilization ? GetDampedValueOnYAxis(yValue, y) : y;
            }
            if (stabilizeZ)
            {
                zValue = smoothStabilization ? GetDampedValueOnZAxis(zValue, z) : z;
            }
            var targetValue = new Vector3(xValue, yValue, zValue);
        
            transform.position = targetValue;
        }

        private float GetDampedValueOnXAxis(float actualVal, float targetValue)
        {
            return Mathf.SmoothDamp(actualVal, targetValue, ref _smoothDampXVelocity, smoothDampTime);
        }
        private float GetDampedValueOnYAxis(float actualVal, float targetValue)
        {
            return Mathf.SmoothDamp(actualVal, targetValue, ref _smoothDampYVelocity, smoothDampTime);
        }
    
        private float GetDampedValueOnZAxis(float actualVal, float targetValue)
        {
            return Mathf.SmoothDamp(actualVal, targetValue, ref _smoothDampZVelocity, smoothDampTime);
        }
    
    
    }
}
