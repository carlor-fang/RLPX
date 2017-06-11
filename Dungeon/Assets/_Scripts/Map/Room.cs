using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

/*8*8的房间，其中&为通道预留空间，+为墙壁，-为地板
 *  &&&&&&&&
 *  &++++++&
 *  &+----+&
 *  &+----+&
 *  &+----+&
 *  &+----+&
 *  &++++++&
 *  &&&&&&&&
 */

public class Room : MonoBehaviour {
        #region declaration
        private int roomId;
        public int RoomId { get { return roomId; } }
        private RoomTemplate roomTemp;
        public RoomTemplate RoomTemp { get { return roomTemp; } }
        private Vector3 roomPosition;
        //房间边界，包括通道和墙壁
        private Bounds roomBounds;

        private Dictionary<int, BaseObject> objList;

        private Dictionary<int, int> floorDictionary;
        private List<int> floorList;
        private Dictionary<int, int> wallDictionary;
        private List<int> wallList;
        private Dictionary<int, int> doorDictionary;
        private List<int> doorList;
        private Dictionary<int, int> ornamentDictionary;
        private List<int> ornamentList;
        private Dictionary<int, int> obstacleDictionary;
        private List<int> obstacleList;

        //private List<Way> wayList;

        private Dictionary<int, Room> connectRoomList;
        #endregion

        #region private

        private int GetPosKey(int col, int row)
        {
                return col * 100000 + row;
        }

        private void CreateFloors()
        {
                GameObject Prefab = (ResourceManager.instance.GetAsset<GameObject>("Prefabs/Floor"));
                try
                {
                        for (int i = Mathf.FloorToInt(roomBounds.min.x) + GameConst.RoomOutsize; i < Mathf.FloorToInt(roomBounds.max.x) - GameConst.RoomOutsize; i++)
                        {
                                for (int j = Mathf.FloorToInt(roomBounds.min.y) + GameConst.RoomOutsize; j < Mathf.FloorToInt(roomBounds.max.y) - GameConst.RoomOutsize; j++)
                                {
                                        GameObject floorObject = Instantiate(Prefab);
                                        string name = gameObject.name + "floor_" + i + "_" + j;
                                        Floor floor = floorObject.GetComponent<Floor>();
                                        floor.Init(LevelManager.GetElementID(), roomId, name, i, j);
                                        objList[floor.ID] = floor;
                                        floorList.Add(floor.ID);
                                        floorDictionary[GetPosKey(i, j)] = floor.ID;
                                }
                        }

                        for (int i = 0; i < floorList.Count; i++)
                        {
                                Debug.Log(objList[floorList[i]].Position.x + "." + objList[floorList[i]].Position.y);
                        }
                }
                finally
                {
                        Prefab = null;
                }

        }

        private void CreateWalls()
        {
                Dictionary<int, bool> floorPos = new Dictionary<int, bool>();
                try
                {
                        for (int i = Mathf.FloorToInt(roomBounds.min.x) + GameConst.RoomOutsize; i < Mathf.FloorToInt(roomBounds.max.x) - GameConst.RoomOutsize; i++)
                        {
                                for (int j = Mathf.FloorToInt(roomBounds.min.y) + GameConst.RoomOutsize; j < Mathf.FloorToInt(roomBounds.max.y) - GameConst.RoomOutsize; j++)
                                {
                                        floorPos[GetPosKey(i, j)] = true;
                                }
                        }
                        GameObject Prefab = (ResourceManager.instance.GetAsset<GameObject>("Prefabs/Wall"));
                        try
                        {
                                for (int i = Mathf.FloorToInt(roomBounds.min.x) + GameConst.RoomWaySize; i < Mathf.FloorToInt(roomBounds.max.x) - GameConst.RoomWaySize; i++)
                                {
                                        for (int j = Mathf.FloorToInt(roomBounds.min.y) + GameConst.RoomWaySize; j < Mathf.FloorToInt(roomBounds.max.y) - GameConst.RoomWaySize; j++)
                                        {
                                                if (floorPos.ContainsKey(GetPosKey(i, j)))
                                                        continue;

                                                GameObject wallObject = Instantiate(Prefab);
                                                string name = gameObject.name + "_wall_" + i + "_" + j;
                                                Wall wall = wallObject.GetComponent<Wall>();
                                                wall.Init(LevelManager.GetElementID(), roomId, name, i, j);
                                                objList[wall.ID] = wall;
                                                wallList.Add(wall.ID);
                                                wallDictionary[GetPosKey(i, j)] = wall.ID;
                                        }
                                }

                        }
                        finally
                        {
                                Prefab = null;
                        }
                }
                finally
                {
                        floorPos = null;
                }

        }

        void DestroyWallAndBuildDoor(Wall wall)
        {
                GameObject Prefab = (ResourceManager.instance.GetAsset<GameObject>("Prefabs/Door"));
                try
                {
                        GameObject doorObject = Instantiate(Prefab);
                        string name = gameObject.name + "_door" + wall.Position.x + "_" + wall.Position.y;
                        Door door = doorObject.GetComponent<Door>();
                        door.Init(LevelManager.GetElementID(), roomId, name, wall.Position.x, wall.Position.y);
                        objList[door.ID] = door;
                        doorList.Add(door.ID);
                        doorDictionary[GetPosKey(Mathf.FloorToInt(wall.Position.x), Mathf.FloorToInt(wall.Position.y))] = door.ID;

                        wall.SetEnabled(false);
                }
                finally
                {
                        Prefab = null;
                }

        }

        void AddDoorExcute(int dir, int doorPos)
        {
                for (int i = 0; i < wallList.Count; i++)
                {
                        Vector3 p = objList[wallList[i]].Position;
                        int x = Mathf.FloorToInt(p.x);
                        int y = Mathf.FloorToInt(p.y);
                        switch (dir)
                        {
                                case GameConst.Dir_Left:
                                        {
                                                if (y == doorPos && x - 1 == Mathf.FloorToInt(roomBounds.min.x))
                                                {
                                                        for (int j = 0; j < 1; j++)
                                                        {
                                                                DestroyWallAndBuildDoor(objList[wallList[i + j]] as Wall);
                                                        }

                                                        return;
                                                }
                                        }
                                        break;
                                case GameConst.Dir_Right:
                                        {
                                                if (y == doorPos && x + 2 == Mathf.FloorToInt(roomBounds.max.x))
                                                {
                                                        for (int j = 0; j < 1; j++)
                                                        {
                                                                DestroyWallAndBuildDoor(objList[wallList[i + j]] as Wall);
                                                        }

                                                        return;
                                                }
                                        }
                                        break;
                                case GameConst.Dir_Top:
                                        {
                                                if (x == doorPos && y + 2 == Mathf.FloorToInt(roomBounds.max.y))
                                                {
                                                        for (int j = 0; j < 1; j++)
                                                        {
                                                                DestroyWallAndBuildDoor(objList[wallList[i + j]] as Wall);
                                                        }
                                                        return;
                                                }

                                        }
                                        break;
                                case GameConst.Dir_Bottom:
                                        {
                                                if (x == doorPos && y - 1 == Mathf.FloorToInt(roomBounds.min.y))
                                                {
                                                        for (int j = 0; j < 1; j++)
                                                        {
                                                                DestroyWallAndBuildDoor(objList[wallList[i + j]] as Wall);
                                                        }
                                                        return;
                                                }

                                        }
                                        break;
                        }
                }
        }
        /*
        void BuildWay(Room room, Vector3 wayPosition)
        {
                GameObject Prefab = (ResourceManager.instance.GetAsset<GameObject>("Prefabs/Way"));
                try
                {
                        switch (Mathf.FloorToInt(wayPosition.x))
                        {
                                case GameConst.Dir_Left:
                                        {
                                                int posx = Mathf.FloorToInt(roomBounds.min.x);
                                                int posy = Mathf.FloorToInt(wayPosition.y);
                                                for (int i = 0; i < wayPosition.z; i++)
                                                {
                                                        for (int j = 0; j < GameConst.WayWidth; j++)
                                                        {
                                                                GameObject wayObject = Instantiate(Prefab);
                                                                Way way = wayObject.GetComponent<Way>();
                                                                way.Init(posx - i, posy + j);
                                                                wayList.Add(way);
                                                        }
                                                }
                                        }
                                        break;
                                case GameConst.Dir_Right:
                                        {
                                                int posx = Mathf.FloorToInt(roomBounds.max.x);
                                                int posy = Mathf.FloorToInt(wayPosition.y);
                                                for (int i = 0; i < wayPosition.z; i++)
                                                {
                                                        for (int j = 0; j < GameConst.WayWidth; j++)
                                                        {
                                                                GameObject wayObject = Instantiate(Prefab);
                                                                Way way = wayObject.GetComponent<Way>();
                                                                way.Init(posx + i, posy + j);
                                                                wayList.Add(way);
                                                        }
                                                }
                                        }
                                        break;
                                case GameConst.Dir_Top:
                                        {
                                                int posy = Mathf.FloorToInt(roomBounds.max.y);
                                                int posx = Mathf.FloorToInt(wayPosition.y);
                                                for (int i = 0; i < wayPosition.z; i++)
                                                {
                                                        for (int j = 0; j < GameConst.WayWidth; j++)
                                                        {
                                                                GameObject wayObject = Instantiate(Prefab);
                                                                Way way = wayObject.GetComponent<Way>();
                                                                way.Init(posx + j, posy + i);
                                                                wayList.Add(way);
                                                        }
                                                }
                                        }
                                        break;

                                case GameConst.Dir_Bottom:
                                        {
                                                int posy = Mathf.FloorToInt(roomBounds.min.y);
                                                int posx = Mathf.FloorToInt(wayPosition.y);
                                                for (int i = 0; i < wayPosition.z; i++)
                                                {
                                                        for (int j = 0; j < GameConst.WayWidth; j++)
                                                        {
                                                                GameObject wayObject = Instantiate(Prefab);
                                                                Way way = wayObject.GetComponent<Way>();
                                                                way.Init(posx + j, posy - i);
                                                                wayList.Add(way);
                                                        }
                                                }
                                        }
                                        break;
                        }

                }
                finally
                {
                        Prefab = null;
                }
        }
        */
        BaseObject FindBlankObject(List<int> list, int exceptx1, int excepty1, int exceptx2, int excepty2)
        {
                int repeatCount = 10;
                while (repeatCount > 0)
                {
                        int index = Mathf.FloorToInt(UnityEngine.Random.value * list.Count);
                        RoomElement ele = objList[list[index]] as RoomElement;
                        if (ele.OrnamentId == 0 && ele.Position.x != exceptx1 && ele.Position.y != excepty1
                                 && ele.Position.x != exceptx2 && ele.Position.y != excepty2)
                                return objList[list[index]];
                        repeatCount--;
                }

                for (int i = 0; i < list.Count; i++)
                {
                        RoomElement ele = objList[list[i]] as RoomElement;
                        if (ele.OrnamentId == 0 && ele.Position.x != exceptx1 && ele.Position.y != excepty1
                                 && ele.Position.x != exceptx2 && ele.Position.y != excepty2)
                                return objList[list[i]];
                }

                return null;
        }

        void AddOrnamentExcute(RoomElement element, OrnamentTemplate temp)
        {
                GameObject Prefab = (ResourceManager.instance.GetAsset<GameObject>("Prefabs/Ornament"));
                try
                {
                        GameObject ornamentObject = Instantiate(Prefab);
                        string name = gameObject.name + "Ornament_" + element.Position.x + "_" + element.Position.y;
                        OrnamentObject ornament = ornamentObject.GetComponent<OrnamentObject>();
                        ornament.Init(LevelManager.GetElementID(), roomId, name, element.Position.x, element.Position.y, temp);
                        objList[ornament.ID] = ornament;
                        ornamentList.Add(ornament.ID);
                        ornamentDictionary[GetPosKey(Mathf.FloorToInt(element.Position.x), Mathf.FloorToInt(element.Position.y))] = ornament.ID;

                        element.OrnamentId = ornament.ID;
                }
                finally
                {
                        Prefab = null;
                }
        }

        void AddOrnament(int classId)
        {
                OrnamentTemplate temp = DataManager.instance.GetOrnamentTemp(classId);
                if (temp == null) return;

                switch (temp.OrnamentType)
                {
                        case (int)GameConst.OrnamentType.Floor:
                                {
                                        Floor floor = FindBlankObject(floorList, -1, -1, -1, -1) as Floor;
                                        if (floor)
                                        {
                                                AddOrnamentExcute(floor, temp);                                             
                                        }
                                }
                                break;
                        case (int)GameConst.OrnamentType.Wall:
                                {
                                        Wall wall = FindBlankObject(wallList, -1, -1, -1, -1) as Wall;
                                        if (wall)
                                        {
                                                AddOrnamentExcute(wall, temp);
                                        }
                                }
                                break;
                        case (int)GameConst.OrnamentType.Door:
                                {
                                        Door door = FindBlankObject(doorList, -1, -1, -1, -1) as Door;
                                        if (door)
                                        {
                                                AddOrnamentExcute(door, temp);
                                        }
                                }
                                break;
                }

        }

        bool AddObstacle(Floor floor)
        {
                if (floor.Position.x == Mathf.FloorToInt(roomBounds.min.x + GameConst.RoomOutsize)) return false;
                if (floor.Position.x == Mathf.FloorToInt(roomBounds.max.x - GameConst.RoomOutsize - 1)) return false;
                if (floor.Position.y == Mathf.FloorToInt(roomBounds.min.y + GameConst.RoomOutsize)) return false;
                if (floor.Position.y == Mathf.FloorToInt(roomBounds.max.y - GameConst.RoomOutsize - 1)) return false;

                GameObject Prefab = (ResourceManager.instance.GetAsset<GameObject>("Prefabs/Obstacle"));
                try
                {
                        GameObject obstacleObject = Instantiate(Prefab);
                        string name = gameObject.name + "Obstacle_" + floor.Position.x + "_" + floor.Position.y;
                        ObstacleObject obstacle = obstacleObject.GetComponent<ObstacleObject>();
                        obstacle.Init(LevelManager.GetElementID(), roomId, name, floor.Position.x, floor.Position.y);
                        objList[obstacle.ID] = obstacle;
                        obstacleList.Add(obstacle.ID);
                        obstacleDictionary[GetPosKey(Mathf.FloorToInt(floor.Position.x), Mathf.FloorToInt(floor.Position.y))] = obstacle.ID;

                        floor.OrnamentId = obstacle.ID;
                }
                finally
                {
                        Prefab = null;
                }

                return true;
        }

        private void ClearList(List<int> list)
        {
                for (int i = 0; i < list.Count; i++)
                {
                        BaseObject obj = objList[list[i]];
                        objList.Remove(list[i]);
                        obj.Clear();
                        Destroy(obj);
                        obj = null;
                }
        }
        #endregion

        #region public
        public RoomTemplate GetTemp()
        {
                return roomTemp;
        }

        public Vector3 GetPosition()
        {
                return roomPosition;
        }

        public Bounds GetBounds()
        {
                return roomBounds;
        }

        public int GetId()
        {
                return roomId;
        }

        public void Clear()
        {
                ClearList(floorList);
                floorList.Clear();
                floorDictionary.Clear();
                floorList = null;
                floorDictionary = null;

                ClearList(wallList);
                wallList.Clear();
                wallDictionary.Clear();
                wallList = null;
                wallDictionary = null;

                ClearList(doorList);
                doorList.Clear();
                doorDictionary.Clear();
                doorList = null;
                doorDictionary = null;

                ClearList(ornamentList);
                ornamentList.Clear();
                ornamentDictionary.Clear();
                ornamentList = null;
                ornamentDictionary = null;

                ClearList(obstacleList);
                obstacleList.Clear();
                obstacleDictionary.Clear();
                obstacleList = null;
                obstacleDictionary = null;

                connectRoomList.Clear();
                connectRoomList = null;
        }

        public void Init(int id, RoomTemplate temp, Vector3 position)
        {
                roomId = id;
                roomTemp = temp;
                roomPosition = position;
                Vector3 size = new Vector3(temp.Width, temp.Height, 1.0f);
                roomBounds = new Bounds(position, size);
                //roomBounds.SetMinMax(new Vector3(Mathf.CeilToInt(roomBounds.min.x), Mathf.CeilToInt(roomBounds.min.y), 1.0f),
                //        new Vector3(Mathf.CeilToInt(roomBounds.max.x), Mathf.CeilToInt(roomBounds.max.y), 1.0f));

                objList         = new Dictionary<int, BaseObject>();

                floorDictionary = new Dictionary<int, int>();
                floorList       = new List<int>();
                wallDictionary  = new Dictionary<int, int>();
                wallList        = new List<int>();
                doorDictionary  = new Dictionary<int, int>();
                doorList        = new List<int>();
                ornamentDictionary = new Dictionary<int, int>();
                ornamentList    = new List<int>();
                obstacleDictionary = new Dictionary<int, int>();
                obstacleList = new List<int>();
                //wayList         = new List<Way>();
                connectRoomList = new Dictionary<int, Room>();

                CreateFloors();
                CreateWalls();
                //门在所有房间生成完后，按照一定的随机规律生成
        }

        public void AddDoor(Room room, int dir, int doorPosition, int dis)
        {
                int id = room.GetId();

                connectRoomList[id] = room;

                AddDoorExcute(dir, doorPosition);
        }

        public int GetConnectRoomCount()
        {
                return connectRoomList.Count;
        }

        public Room GetConnectRoom(Dictionary<int, Room> roomQueue)
        {
                foreach (var Item in connectRoomList)
                {
                        if (! roomQueue.ContainsKey(Item.Key))
                                return Item.Value;
                }
                return null;
        }

        ///待改
        public Vector3 GetRandomBlankFloor()
        {
                while (true)
                {
                        int index = Mathf.FloorToInt(UnityEngine.Random.value * floorList.Count);
                        RoomElement roomEle = objList[floorList[index]] as RoomElement;
                        if (roomEle && roomEle.OrnamentId == 0)
                                return roomEle.Position;
                }
        }

        public void CreateOrnaments()
        {
                int count = Mathf.FloorToInt(UnityEngine.Random.value * roomTemp.OrnamentCount);
                for (int i = 0; i < count; i++)
                {
                        float value = UnityEngine.Random.value * DataManager.instance.Ornaments.Count;
                        int index = Mathf.FloorToInt(value);
                        int classId = DataManager.instance.Ornaments[index];
                        AddOrnament(classId);
                }
        }

        public void CreateObstacles()
        {
                int count = floorList.Count
                        - (Mathf.FloorToInt(roomBounds.size.y) - GameConst.RoomOutsize * 2) * 2 //上下地板边
                        - (Mathf.FloorToInt(roomBounds.size.x) - GameConst.RoomOutsize * 2 - 2) * 2; //左右地板边
                for (int i = 0; i < ornamentList.Count; i++)
                {
                        OrnamentObject ornamentObject = objList[ornamentList[i]] as OrnamentObject;
                        if (ornamentObject.OrnamentTemp.OrnamentType == (int)GameConst.OrnamentType.Floor)
                                count--;
                }

                count = Mathf.FloorToInt(count * 60 / 100);

                for (int i = 0; i < floorList.Count; i++)
                {
                        Floor floor = objList[floorList[i]] as Floor;
                        if (floor.OrnamentId != 0) continue;
                        if (UnityEngine.Random.value < 0.5) continue;

                        if (!AddObstacle(floor)) continue;

                        count--;
                        if (count <= 0) break;
                }
        }

        public bool IsInBounds(float x, float y)
        {
                if (x < roomBounds.min.x + GameConst.RoomOutsize) return false;
                if (y < roomBounds.min.y + GameConst.RoomOutsize) return false;
                if (x > roomBounds.max.x - GameConst.RoomOutsize) return false;
                if (y > roomBounds.max.y - GameConst.RoomOutsize) return false;

                return true;
        }

        public bool IsBlock(float x, float y)
        {
                int poskey = GetPosKey(Mathf.FloorToInt(x), Mathf.FloorToInt(y));
                if (wallDictionary.ContainsKey(poskey)) return true;
                if (obstacleDictionary.ContainsKey(poskey)) return true;

                return false;
        }
        #endregion
}
