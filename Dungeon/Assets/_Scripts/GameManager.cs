using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {
        #region declaration
        public static GameManager instance;

        //地图
        public LevelManager levelMgr;
        #endregion

        #region inherit
        // Use this for initialization
        void Awake ()
        {
                if (! instance)
                {
                        instance = this;
                }
                else if (instance != this)
                {
                        Destroy(gameObject);
                }

                levelMgr.Init();
        }

        void Start()
        {
                levelMgr.InitScene();
        }

        // Update is called once per frame
        void Update ()
        {
		
	}
        #endregion
}
