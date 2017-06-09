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

        private List<Floor> floorList;
        private List<Wall> wallList;
        private List<Door> doorList;
        private List<OrnamentObject> ornamentList;
        private List<ObstacleObject> obstacleList;
        //private List<Way> wayList;

        private Dictionary<int, Room> connectRoomList;
        #endregion

        #region private
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
                                        floorList.Add(floor);
                                }
                        }
                }
                finally
                {
                        Prefab = null;
                }

        }

        private int GetPosKey(int col, int row)
        {
                return col * 100000 + row;
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
                                                wallList.Add(wall);
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
                        doorList.Add(door);

                        wallList.Remove(wall);
                        wall.Clear();
                        Destroy(wall);
                        wall = null;
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
                        Vector3 p = wallList[i].Position;
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
                                                                DestroyWallAndBuildDoor(wallList[i + j]);
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
                                                                DestroyWallAndBuildDoor(wallList[i + j]);
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
                                                                DestroyWallAndBuildDoor(wallList[i + j]);
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
                                                                DestroyWallAndBuildDoor(wallList[i + j]);
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
        T FindBlankObject<T>(List<T> list, int exceptx1, int excepty1, int exceptx2, int excepty2)
        {
                int repeatCount = 10;
                while (repeatCount > 0)
                {
                        int index = Mathf.FloorToInt(UnityEngine.Random.value * list.Count);
                        RoomElement ele = list[index] as RoomElement;
                        if (ele.OrnamentId == 0 && ele.Position.x != exceptx1 && ele.Position.y != excepty1
                                 && ele.Position.x != exceptx2 && ele.Position.y != excepty2)
                                return list[index];
                        repeatCount--;
                }

                for (int i = 0; i < list.Count; i++)
                {
                        RoomElement ele = list[i] as RoomElement;
                        if (ele.OrnamentId == 0 && ele.Position.x != exceptx1 && ele.Position.y != excepty1
                                 && ele.Position.x != exceptx2 && ele.Position.y != excepty2)
                                return list[i];
                }

                return default(T);
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
                        ornamentList.Add(ornament);

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
                                        Floor floor = FindBlankObject(floorList, -1, -1, -1, -1);
                                        if (floor)
                                        {
                                                AddOrnamentExcute(floor, temp);                                             
                                        }
                                }
                                break;
                        case (int)GameConst.OrnamentType.Wall:
                                {
                                        Wall wall = FindBlankObject(wallList, -1, -1, -1, -1);
                                        if (wall)
                                        {
                                                AddOrnamentExcute(wall, temp);
                                        }
                                }
                                break;
                        case (int)GameConst.OrnamentType.Door:
                                {
                                        Door door = FindBlankObject(doorList, -1, -1, -1, -1);
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

                //Floor floor = FindBlankObject(floorList, Mathf.FloorToInt(roomBounds.min.x + GameConst.RoomOutsize + 1), Mathf.FloorToInt(roomBounds.min.y + GameConst.RoomOutsize + 1),
                //        Mathf.FloorToInt(roomBounds.max.x - GameConst.RoomOutsize - 1), Mathf.FloorToInt(roomBounds.max.y - GameConst.RoomOutsize - 1));

                GameObject Prefab = (ResourceManager.instance.GetAsset<GameObject>("Prefabs/Obstacle"));
                try
                {
                        GameObject obstacleObject = Instantiate(Prefab);
                        string name = gameObject.name + "Obstacle_" + floor.Position.x + "_" + floor.Position.y;
                        ObstacleObject obstacle = obstacleObject.GetComponent<ObstacleObject>();
                        obstacle.Init(LevelManager.GetElementID(), roomId, name, floor.Position.x, floor.Position.y);
                        obstacleList.Add(obstacle);

                        floor.OrnamentId = obstacle.ID;
                }
                finally
                {
                        Prefab = null;
                }

                return true;
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
                while (floorList.Count > 0)
                {
                        Floor floor = floorList[0];
                        floorList.RemoveAt(0);
                        floor.Clear();
                        Destroy(floor);
                        floor = null;
                }
                while (wallList.Count > 0)
                {
                        Wall wall = wallList[0];
                        wallList.RemoveAt(0);
                        wall.Clear();
                        Destroy(wall);
                        wall = null;
                }
                while (doorList.Count > 0)
                {
                        Door door = doorList[0];
                        doorList.RemoveAt(0);
                        door.Clear();
                        Destroy(door);
                        door = null;
                }
                connectRoomList.Clear();
                while (ornamentList.Count > 0)
                {
                        OrnamentObject ornament = ornamentList[0];
                        ornamentList.RemoveAt(0);
                        ornament.Clear();
                        Destroy(ornament);
                        ornament = null;
                }
                while (obstacleList.Count > 0)
                {
                        ObstacleObject obstacle = obstacleList[0];
                        obstacleList.RemoveAt(0);
                        obstacle.Clear();
                        Destroy(obstacle);
                        obstacle = null;
                }

                floorList       = null;
                wallList        = null;
                doorList        = null;
                connectRoomList = null;
                ornamentList    = null;
                obstacleList    = null;
                //roomBounds = null;
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

                floorList       = new List<Floor>();
                wallList        = new List<Wall>();
                doorList        = new List<Door>();
                ornamentList    = new List<OrnamentObject>();
                obstacleList    = new List<ObstacleObject>();
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
                        if (floorList[index].OrnamentId == 0)
                                return floorList[index].Position;
                }
                //int key = GetPosKey(Mathf.FloorToInt(floorList[index].Position.x), Mathf.FloorToInt(floorList[index].Position.y));

                //return floorList[index].Position;
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
                        if (ornamentList[i].OrnamentTemp.OrnamentType == (int)GameConst.OrnamentType.Floor)
                                count--;
                }

                count = Mathf.FloorToInt(count * 60 / 100);

                for (int i = 0; i < floorList.Count; i++)
                {
                        if (floorList[i].OrnamentId != 0) continue;
                        if (UnityEngine.Random.value < 0.5) continue;

                        if (!AddObstacle(floorList[i])) continue;

                        count--;
                        if (count <= 0) break;
                }
        }
        #endregion
}
