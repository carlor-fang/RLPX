using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour {
        static public int ELEMIDSEED = 0;

        public MapManager mapMgr;

        private int curLevel;
        private Player hero;
        public Player Hero { get { return hero; } }
        private MonsterObject monster;
        public MonsterObject Monster { get { return monster; } }

        private Dictionary<Vector3, int> objectPositions = new Dictionary<Vector3, int>();

        static public int GetElementID()
        {
                return ++ELEMIDSEED;
        }

        void BornPlayer(Room room)
        {
                GameObject Prefab = (ResourceManager.instance.GetAsset<GameObject>("Prefabs/Player"));
                GameObject playerObject = Instantiate(Prefab);
                string name = "Player";
                hero = playerObject.GetComponent<Player>();
                Vector3 pos = room.GetRandomBlankFloor(objectPositions);
                hero.Init(0, room.RoomId, name, pos.x, pos.y);
                objectPositions[pos] = 1;
        }

        void TestBornMonster(Room room)
        {
                GameObject Prefab = (ResourceManager.instance.GetAsset<GameObject>("Prefabs/MonsterObject"));
                GameObject monsterObject = Instantiate(Prefab);
                string name = "Monster";
                monster = monsterObject.GetComponent<MonsterObject>();
                Vector3 pos = room.GetRandomBlankFloor(objectPositions);
                monster.Init(0, room.RoomId, name, pos.x, pos.y);
                monster.GetComponent<AutoMoveObject>().SetRoom(room);
                objectPositions[pos] = 1;
        }

        void ClearScene()
        {
                objectPositions.Clear();
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
                ClearScene();

                Room room = mapMgr.InitScene(DataManager.instance.GetMapTemp(curLevel));
                if (!room) return;

                BornPlayer(room);
                TestBornMonster(room);
                Camera camera = FindObjectOfType(typeof(Camera)) as Camera;
                camera.GetComponent<CameraControll>().SetFllowObject(hero.transform);
                //SetCamera(pos);
        }
}
