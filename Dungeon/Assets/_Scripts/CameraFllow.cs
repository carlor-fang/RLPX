using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFllow : MonoBehaviour {
        private Transform fllowObject;

        public float    smoothTime = 0.01f;
        private Vector3 cameraVelocity = Vector3.zero;
        private Camera  mainCamera;

        void Awake()
        {
                mainCamera = Camera.main;        
        }
        // Use this for initialization
        void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
                if (!fllowObject) return;

                transform.position = Vector3.SmoothDamp(transform.position, fllowObject.position + new Vector3(0, 0, -5), 
                        ref cameraVelocity, smoothTime);
	}

        public void SetFllowObject(Transform value)
        {
                fllowObject = value;
        }
}
