using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Main : MonoBehaviour {
        public GameObject gameConst;

        public GameObject resMgr;
        public GameObject dataMgr;

        public GameObject gameMgr;

	void Awake ()
        {
		if (GameManager.instance == null)
                {
                        Instantiate(gameMgr);
                }

                if (ResourceManager.instance == null)
                {
                        Instantiate(resMgr);
                }

                if (DataManager.instance == null)
                {
                        Instantiate(dataMgr);
                }
	}

        void Start()
        {
        }

        void Update()
        {
                //left click
                if (Input.GetMouseButtonDown(1))
                {
                        Vector2 sp = Input.mousePosition;
                        Vector2 wp = GetComponent<Camera>().ScreenToWorldPoint(sp);
                        wp.x = Mathf.RoundToInt(wp.x);
                        wp.y = Mathf.RoundToInt(wp.y);               
                        //GameManager.instance.levelMgr.Monster.GetComponent<AutoMoveObject>().SetTarget(wp);
                }

                if (Input.GetKeyDown(KeyCode.R))
                {
                        GameManager.instance.Reset();
                }
                
        }
}
