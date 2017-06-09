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
        }
}
