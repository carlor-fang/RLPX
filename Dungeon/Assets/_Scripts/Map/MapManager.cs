using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapManager : MonoBehaviour {
        #region declaration
        //room
        public RoomManager roomMgr;

        #endregion

        #region private
        void ClearScene()
        {
                roomMgr.ClearScene();
        }

        #endregion

        #region public
        public void Init ()
        {
                roomMgr.Init();
	}

        // Init scene
        public Room InitScene(MapTemplate mapTemp)
        {
                ClearScene();
                return roomMgr.InitScene(mapTemp);
        }
        #endregion
}
