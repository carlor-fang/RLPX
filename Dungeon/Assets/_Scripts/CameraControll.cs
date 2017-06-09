using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControll : MonoBehaviour {

        private Transform fllowObject;

        public float smoothTime = 0.01f;
        private Vector3 cameraVelocity = Vector3.zero;

        // Use this for initialization
        void Start ()
        {
	}
	
	// Update is called once per frame
	void Update ()
        {


                float zoom = Input.GetAxis("Mouse ScrollWheel");
                if (zoom != 0)
                        GetComponent<Camera>().orthographicSize += zoom;

                if (fllowObject)
                {
                        //transform.position = Vector3.SmoothDamp(transform.position, fllowObject.position + new Vector3(0, 0, -5),
                        //        ref cameraVelocity, smoothTime);
                        transform.position = fllowObject.position + new Vector3(0, 0, -5);
                }

        }

        public void SetFllowObject(Transform value)
        {
                fllowObject = value;
        }
}
