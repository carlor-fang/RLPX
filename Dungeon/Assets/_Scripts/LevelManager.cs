using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour {
        static public int ELEMIDSEED = 0;

        public MapManager mapMgr;

        private int curLevel;
        private Player hero;
        public Player Hero { get { return hero; } }

        static public int GetElementID()
        {
                return ++ELEMIDSEED;
        }

        void BornPlayer(Room room)
        {
                GameObject Prefab = (ResourceManager.instance.GetAsset<GameObject>("Prefabs/Player"));
                try
                {
                        GameObject playerObject = Instantiate(Prefab);
                        string name = "Player";
                        hero = playerObject.GetComponent<Player>();
                        Vector3 pos = room.GetRandomBlankFloor();
                        hero.Init(0, room.RoomId, name, pos.x, pos.y);
                }
                finally
                {
                        Prefab = null;
                }
        }

        #region camera
        void SetCamera(Vector3 pos)
        {
                GameObject camera = GameObject.FindGameObjectWithTag("MainCamera");
                if (camera == null)
                        return;

                if (mapMgr.roomMgr.StartRoom == null)
                {
                        camera.transform.position = Vector3.zero;
                }
                else
                {
                        camera.transform.position = mapMgr.roomMgr.StartRoom.GetBounds().center;
                }
        }
        #endregion

        public void Init()
        {
                curLevel = 1;
                mapMgr.Init();
        }

        public void InitScene()
        {
                Room room = mapMgr.InitScene(DataManager.instance.GetMapTemp(curLevel));
                if (!room) return;

                BornPlayer(room);
                Camera camera = FindObjectOfType(typeof(Camera)) as Camera;
                camera.GetComponent<CameraControll>().SetFllowObject(hero.transform);
                //SetCamera(pos);
        }
}
