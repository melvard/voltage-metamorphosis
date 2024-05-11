using System;
using GameLogic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace GameCamera
{
    public class CameraController : MonoBehaviour
    {
        [SerializeField] private Camera camera;
        
        public void OnPointerClick()
        {
            var ray = camera.ScreenPointToRay(Input.mousePosition);
            if(Physics.Raycast(ray, out var hit))
            {
                InputsManager.SetApplicationInputFocus(hit.transform.gameObject.layer);
            }
        }
        
        public void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                OnPointerClick();
            }
        }
    }
}