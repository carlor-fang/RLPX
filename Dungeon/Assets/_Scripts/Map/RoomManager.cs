using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class RoomManager : MonoBehaviour {
        #region declaration
        private List<Room> roomList;
        private Room startRoom;
        public Room StartRoom { get { return startRoom; } }
        private int idSeed;

        private Dictionary<int, Way>  wayList;
        Dictionary<int, Room>         roomQueue;
        #endregion

        #region create room
        private RoomTemplate GetRandomRoomTemp(MapTemplate mapTemp)
        {
                float randomRate = UnityEngine.Random.value * 100;

                for (int i = 0; i < mapTemp.RoomRateList.Count; i++)
                {
                        float rate = mapTemp.RoomRateList[i].Rate;
                        if (rate >= randomRate)
                        {
                                int classID = mapTemp.RoomRateList[i].ClassID;
                                return DataManager.instance.GetRoomTemp(classID);
                        }
                }

                return null;
        }

        private void GetRandomRoomPosition(MapTemplate mapTemp, RoomTemplate roomTemp, ref Vector3 position)
        {
                //取随机位置（不要超出边界）
                position.x = Mathf.FloorToInt(UnityEngine.Random.value * (mapTemp.Width - roomTemp.Width / 2) + roomTemp.Width / 2);
                position.y = Mathf.FloorToInt(UnityEngine.Random.value * (mapTemp.Height - roomTemp.Height / 2) + roomTemp.Height / 2);
                //bounds
                Vector3 size = new Vector3(roomTemp.Width, roomTemp.Height, 1);
                Bounds newBounds = new Bounds(position, size);

                try
                {
                        //检测是否重叠
                        for (int i = 0; i < roomList.Count; i++)
                        {
                                Bounds bounds = roomList[i].GetBounds();
                                if (bounds.Intersects(newBounds))
                                {
                                        //重叠
                                        position.x = 0f;
                                        position.y = 0f;
                                        position.z = 0f;
                                        return;
                                }
                        }
                }
                finally
                {
                        //newBounds = null;
                        //size = null;
                }
        }

        private void AddRoom(MapTemplate mapTemp, RoomTemplate roomTemp, Vector3 position)
        {
                GameObject Prefab = (ResourceManager.instance.GetAsset<GameObject>("Prefabs/Room"));
                try
                {
                        GameObject roomObject = Instantiate(Prefab);
                        roomObject.name = "room_" + roomList.Count;
                        Room room = roomObject.GetComponent<Room>();
                        room.Init(idSeed++, roomTemp, position);
                        roomList.Add(room);
                }
                catch(Exception e)
                {
                        Console.WriteLine("{0} exception.", e.Message);
                }
        }      

        private void CreateRooms(MapTemplate mapTemp)
        {
                while (true)
                {
                        //算法1：在一个尺寸下，检测n次位置重叠
                        //RoomTemplate roomTemp = GetRandomRoomTemp(mapTemp);
                        //if (roomTemp != null)
                        //{
                        //        Vector3 position = Vector3.zero;
                        //        int checkCount = 0;
                        //        while (position == Vector3.zero && checkCount < mapTemp.OverlapCount)
                        //        {
                        //                GetRandomRoomPosition(mapTemp, roomTemp, ref position);
                        //                checkCount++;
                        //        }
                        //        if (position == Vector3.zero)
                        //        {
                        //                //没有空间生成新的房间
                        //                break;
                        //        }

                        //        AddRoom(mapTemp, roomTemp, position);

                        //}

                        //算法2：每次重叠后，重新取得尺寸和位置，进行检测
                        RoomTemplate roomTemp = null;
                        Vector3 position = Vector3.zero;
                        int checkCount = 0;
                        while (position == Vector3.zero && checkCount < mapTemp.OverlapCount)
                        {
                                roomTemp = GetRandomRoomTemp(mapTemp);
                                if (roomTemp != null)
                                {
                                        GetRandomRoomPosition(mapTemp, roomTemp, ref position);
                                        checkCount++;
                                }
                        }
                        if (position == Vector3.zero)
                        {
                                //没有空间生成新的房间
                                break;
                        }
                        AddRoom(mapTemp, roomTemp, position);
                }

        }
        #endregion

        #region create ways
        #region 以外围随机一个房间为起始房间
        Room GetRandomOutsizeRoom()
        {
                if (roomList.Count <= 0)
                        return null;

                int randomRate = Mathf.FloorToInt(UnityEngine.Random.value * 4);

                Room tempRoom = roomList[0];
                Bounds tempBounds = roomList[0].GetBounds();
                for (int i = 1; i < roomList.Count; i++)
                {
                        Bounds bounds = roomList[i].GetBounds();
                        switch (randomRate)
                        {
                                case 1:// "right":
                                        {
                                                if (bounds.max.x > tempBounds.max.x)
                                                {
                                                        tempRoom = roomList[i];
                                                        tempBounds = bounds;
                                                }
                                        }
                                        break;
                                case 2: //"top":
                                        {
                                                if (bounds.min.y < tempBounds.min.y)
                                                {
                                                        tempRoom = roomList[i];
                                                        tempBounds = bounds;
                                                }
                                        }
                                        break;
                                case 3: //"bottom":
                                        {

                                                if (bounds.max.y > tempBounds.max.y)
                                                {
                                                        tempRoom = roomList[i];
                                                        tempBounds = bounds;
                                                }
                                        }
                                        break;
                                default://case 0://"left":
                                        {
                                                if (bounds.min.x < tempBounds.min.x)
                                                {
                                                        tempRoom = roomList[i];
                                                        tempBounds = bounds;
                                                }
                                        }
                                        break;


                        }
                }

                return tempRoom;
        }
        #endregion

        void AddWay(int startx, int starty, int endx, int endy, int dir, Room fromRoom, Room toRoom)
        {
                GameObject Prefab = (ResourceManager.instance.GetAsset<GameObject>("Prefabs/Way"));
                for (int i = startx; i < endx; i++)
                {
                        for (int j = starty; j < endy; j++)
                        {
                                if (wayList.ContainsKey(i * 100000 + j)) continue;

                                GameObject wayObject = Instantiate(Prefab);
                                Way way = wayObject.GetComponent<Way>();
                                way.Init(LevelManager.GetElementID(), 0, "way_"+i+"_"+j, i, j, dir);
                                wayList[i * 100000 + j] = way;
                                way.SetConnectRoom(fromRoom, toRoom);
                                way.Flag = i * 100000 + j;
                        }
                }
        }

        #region 检测门位置合法性
        bool CheckDoorPosition(Bounds from, Bounds to, int dir, int doorPosition)
        {
                switch(dir)
                {
                        case GameConst.Dir_Left:
                        case GameConst.Dir_Right:
                                {
                                        if (from.max.y - 1 == doorPosition || from.min.y + 1 == doorPosition ||
                                                to.max.y - 1 == doorPosition || to.min.y + 1 == doorPosition)
                                                return false;
                                }
                                break;
                        case GameConst.Dir_Top:
                        case GameConst.Dir_Bottom:
                                {
                                        if (from.min.x + 1 == doorPosition || from.max.x - 1 == doorPosition ||
                                                to.min.x + 1 == doorPosition || to.max.x - 1 == doorPosition)
                                                return false;
                                }
                                break;
                }
                return true;
        }
        #endregion

        #region 房间距
        int GetRectDistance(Bounds from, Bounds to, Room fromRoom, Room toRoom, out int dir, out int doorPosition)
        {
                int dis = 100000;
                #region 右            
                if (from.center.x < to.center.x)
                {
                        #region 右上
                        if (from.center.y < to.center.y)
                        {
                                //x 相交， y不相交
                                if (from.max.x - to.min.x >= GameConst.OverlapWidth)
                                {
                                        //通道在上方，计算距离
                                        dis = Mathf.CeilToInt(to.min.y - from.max.y);
                                        //通道不要太长
                                        if (dis <= GameConst.WayMaxLength)
                                        {
                                                dir = GameConst.Dir_Top;
                                                if (from.max.x >= to.max.x)
                                                        doorPosition = Mathf.FloorToInt((to.max.x - to.min.x) / 2 + to.min.x);
                                                else if (to.min.x <= from.min.x)
                                                        doorPosition = Mathf.FloorToInt((from.max.x - from.min.x) / 2 + from.min.x);
                                                else
                                                        doorPosition = Mathf.FloorToInt((from.max.x - to.min.x) / 2 + to.min.x);
                                                if (CheckDoorPosition(from, to, dir, doorPosition))
                                                {
                                                        AddWay(doorPosition, Mathf.FloorToInt(from.max.y) - 1, doorPosition + GameConst.WayWidth, Mathf.FloorToInt(from.max.y + dis + 1), dir, fromRoom, toRoom);
                                                        return dis;
                                                }
                                        }
                                }
                                //y相交
                                else if (from.max.y - to.min.y >= GameConst.OverlapWidth)
                                {
                                        //通道在右方

                                        dis = Mathf.CeilToInt(to.min.x - from.max.x);
                                        if (dis <= GameConst.WayMaxLength)
                                        {
                                                dir = GameConst.Dir_Right;
                                                if (from.max.y >= to.max.y)
                                                        doorPosition = Mathf.FloorToInt((to.max.y - to.min.y) / 2 + to.min.y);
                                                else if (to.min.y <= from.min.y)
                                                        doorPosition = Mathf.FloorToInt((from.max.y - from.min.y) / 2 + from.min.y);
                                                else
                                                        doorPosition = Mathf.FloorToInt((from.max.y - to.min.y) / 2 + to.min.y);
                                                if (CheckDoorPosition(from, to, dir, doorPosition))
                                                {
                                                        AddWay(Mathf.FloorToInt(from.max.x) - 1, doorPosition, Mathf.FloorToInt(from.max.x) + dis + 1, doorPosition + GameConst.WayWidth, dir, fromRoom, toRoom);
                                                        return dis;
                                                }
                                        }
                                }
                        }
                        #endregion
                        #region 右下
                        else
                        {
                                //x 相交， y不相交
                                if (from.max.x - to.min.x >= GameConst.OverlapWidth)
                                {
                                        //通道在下方，计算距离
                                        dis = Mathf.CeilToInt(from.min.y - to.max.y);
                                        //通道不要太长
                                        if (dis <= GameConst.WayMaxLength)
                                        {
                                                dir = GameConst.Dir_Bottom;
                                                if (from.max.x >= to.max.x)
                                                        doorPosition = Mathf.FloorToInt((to.max.x - to.min.x) / 2 + to.min.x);
                                                else if (to.min.x <= from.min.x)
                                                        doorPosition = Mathf.FloorToInt((from.max.x - from.min.x) / 2 + from.min.x);
                                                else
                                                        doorPosition = Mathf.FloorToInt((from.max.x - to.min.x) / 2 + to.min.x);
                                                if (CheckDoorPosition(from, to, dir, doorPosition))
                                                        return dis;
                                        }
                                }
                                //y相交
                                else if (to.max.y - from.min.y >= GameConst.OverlapWidth)
                                {
                                        //通道在右方
                                        dis = Mathf.CeilToInt(to.min.x - from.max.x);
                                        if (dis <= GameConst.WayMaxLength)
                                        {
                                                dir = GameConst.Dir_Right;
                                                if (from.min.y <= to.min.y)
                                                        doorPosition = Mathf.FloorToInt((to.max.y - to.min.y) / 2 + to.min.y);
                                                else if (from.max.y <= to.max.y)
                                                        doorPosition = Mathf.FloorToInt((from.max.y - from.min.y) / 2 + from.min.y);
                                                else
                                                        doorPosition = Mathf.FloorToInt((to.max.y - from.min.y) / 2 + from.min.y);
                                                if (CheckDoorPosition(from, to, dir, doorPosition))
                                                {
                                                        AddWay(Mathf.FloorToInt(from.max.x) - 1, doorPosition, Mathf.FloorToInt(from.max.x) + dis + 1, doorPosition + GameConst.WayWidth, dir, fromRoom, toRoom);
                                                        return dis;
                                                }
                                        }
                                }
                        }
                        #endregion
                }
                #endregion
                #region 左
                else if (from.center.x > to.center.x)
                {
                        #region 左上
                        if (from.center.y < to.center.y)
                        {
                                //x 相交， y不相交
                                if (to.max.x - from.min.x >= GameConst.OverlapWidth)
                                {
                                        //通道在上方，计算距离
                                        dis = Mathf.CeilToInt(to.min.y - from.max.y);
                                        //通道不要太长
                                        if (dis <= GameConst.WayMaxLength)
                                        {
                                                dir = GameConst.Dir_Top;
                                                if (from.min.x <= to.min.x)
                                                        doorPosition = Mathf.FloorToInt((to.max.x - to.min.x) / 2 + to.min.x);
                                                else if (to.max.x >= from.max.x)
                                                        doorPosition = Mathf.FloorToInt((from.max.x - from.min.x) / 2 + from.min.x);
                                                else
                                                        doorPosition = Mathf.FloorToInt((to.max.x - from.min.x) / 2 + from.min.x);
                                                if (CheckDoorPosition(from, to, dir, doorPosition))
                                                {
                                                        AddWay(doorPosition, Mathf.FloorToInt(from.max.y) - 1, doorPosition + GameConst.WayWidth, Mathf.FloorToInt(from.max.y + dis + 1), dir, fromRoom, toRoom);
                                                        return dis;
                                                }
                                        }
                                }
                                //y相交
                                else if (from.max.y - to.min.y >= GameConst.OverlapWidth)
                                {
                                        //通道在左方
                                        dis = Mathf.CeilToInt(from.min.x - to.max.x);
                                        if (dis <= GameConst.WayMaxLength)
                                        {
                                                dir = GameConst.Dir_Left;
                                                if (from.max.y >= to.max.y)
                                                        doorPosition = Mathf.FloorToInt((to.max.y - to.min.y) / 2 + to.min.y);
                                                else if (to.min.y <= from.min.y)
                                                        doorPosition = Mathf.FloorToInt((from.max.y - from.min.y) / 2 + from.min.y);
                                                else
                                                        doorPosition = Mathf.FloorToInt((from.max.y - to.min.y) / 2 + to.min.y);
                                                if (CheckDoorPosition(from, to, dir, doorPosition))
                                                        return dis;
                                        }
                                }
                        }
                        #endregion
                        #region 左下
                        else
                        {
                                //x 相交， y不相交
                                if (to.max.x - from.min.x >= GameConst.OverlapWidth)
                                {
                                        //通道在下方，计算距离
                                        dis = Mathf.CeilToInt(from.min.y - to.max.y);
                                        //通道不要太长
                                        if (dis <= GameConst.WayMaxLength)
                                        {
                                                dir = GameConst.Dir_Bottom;
                                                if (from.min.x <= to.min.x)
                                                        doorPosition = Mathf.FloorToInt((to.max.x - to.min.x) / 2 + to.min.x);
                                                else if (to.max.x >= from.max.x)
                                                        doorPosition = Mathf.FloorToInt((from.max.x - from.min.x) / 2 + from.min.x);
                                                else
                                                        doorPosition = Mathf.FloorToInt((to.max.x - from.min.x) / 2 + from.min.x);
                                                if (CheckDoorPosition(from, to, dir, doorPosition))
                                                        return dis;
                                        }
                                }
                                //y相交
                                else if (to.max.y - from.min.y >= GameConst.OverlapWidth)
                                {
                                        //通道在左方
                                        dis = Mathf.CeilToInt(from.min.x - to.max.x);
                                        if (dis <= GameConst.WayMaxLength)
                                        {
                                                dir = GameConst.Dir_Left;
                                                if (from.min.y <= to.min.y)
                                                        doorPosition = Mathf.FloorToInt((to.max.y - to.min.y) / 2 + to.min.y);
                                                else if (to.max.y >= from.max.y)
                                                        doorPosition = Mathf.FloorToInt((from.max.y - from.min.y) / 2 + from.min.y);
                                                else
                                                        doorPosition = Mathf.FloorToInt((to.max.y - from.min.y) / 2 + from.min.y);
                                                if (CheckDoorPosition(from, to, dir, doorPosition))
                                                        return dis;
                                        }
                                }
                        }
                        #endregion
                }
                #endregion
                #region x相等
                else
                {
                        //正上
                        if (from.center.y < to.center.y)
                        {
                                //通道在上方，计算距离
                                dis = Mathf.CeilToInt(to.min.y - from.max.y);
                                //通道不要太长
                                if (dis <= GameConst.WayMaxLength)
                                {
                                        dir = GameConst.Dir_Top;
                                        doorPosition = Mathf.FloorToInt((from.max.x - from.min.x) / 2);
                                        //doorPosition = Mathf.FloorToInt(Mathf.Abs(from.min.x - to.min.x) / 2 + Mathf.Min(to.min.x, from.min.x));
                                        if (CheckDoorPosition(from, to, dir, doorPosition))
                                        {
                                                AddWay(doorPosition, Mathf.FloorToInt(from.max.y) - 1, doorPosition + GameConst.WayWidth, Mathf.FloorToInt(from.max.y + dis + 1), dir, fromRoom, toRoom);
                                                return dis;
                                        }
                                }
                        }
                        //正下
                        else
                        {
                                //通道在下方，计算距离
                                dis = Mathf.CeilToInt(from.min.y - to.max.y);
                                //通道不要太长
                                if (dis <= GameConst.WayMaxLength)
                                {
                                        dir = GameConst.Dir_Bottom;
                                        doorPosition = Mathf.FloorToInt((from.max.x - from.min.x) / 2);
                                        //doorPosition = Mathf.FloorToInt(Mathf.Abs(from.min.x - to.min.x) / 2 + Mathf.Min(to.min.x, from.min.x));
                                        if (CheckDoorPosition(from, to, dir, doorPosition))
                                                return dis;
                                }

                        }
                }
                #endregion

                dis = 100000;
                dir = GameConst.Dir_Left;
                doorPosition = 0;
                return dis;
        }
        #endregion
        /*线性连接方式

        #region 联通最近房间
        void GetClosetRoom(Room room)
        {
                Bounds bounds   = room.GetBounds();
                Room closetRoom = null;
                int dir         = GameConst.Dir_Left;
                int doorPos     = 0;
                int closetDis   = 100000; 

                for (int i = 0; i < roomList.Count; i++)
                {
                        //不能为自己
                        if (roomList[i] == room) continue;

                        //已经存在通道
                        if (roomList[i].HasWay()) continue;

                        int curDir;
                        int curDoorPos;
                        int curDis = GetRectDistance(bounds, roomList[i].GetBounds(), out curDir, out curDoorPos);
                        if (curDis >= closetDis) continue;

                        dir = curDir;
                        doorPos = curDoorPos;
                        closetDis = curDis;
                        closetRoom = roomList[i];
                }

                if (closetRoom)
                {
                        room.AddWay(closetRoom, dir, doorPos, closetDis);
                        GetClosetRoom(closetRoom);
                }
        }
        #endregion

        void CreateWays()
        {
                startRoom = GetRandomOutsizeRoom();

                if (startRoom != null)
                {
                        GetClosetRoom(startRoom);
                        for (int i = 0; i < roomList.Count; i++)
                        {
                                roomList[i].BuildWays();
                        }
                }

        }
        */

        void CreateDoors()
        {
                for (int i = 0; i < roomList.Count; i++)
                {
                        Room room = roomList[i];
                        Bounds from = room.GetBounds();

                        for (int j = 0; j < roomList.Count; j++)
                        {
                                if (roomList[j] == room) continue;

                                int dir, doorPos;
                                int dis = GetRectDistance(from, roomList[j].GetBounds(), room, roomList[i], out dir, out doorPos);
                                if (dis <= GameConst.WayMaxLength)
                                {
                                        room.AddDoor(roomList[j], dir, doorPos, dis);
                                }
                        }
                }
        }
        #endregion

        #region invalid room
        void RemoveRoom(Room room)
        {
                List<Way> tempList = new List<Way>();
                try
                {
                        foreach(var item in wayList)
                        {
                                tempList.Add(item.Value);
                        }
                        for (int i = 0; i < tempList.Count; i++)
                        {
                                if (!tempList[i].ConnectedRoom(room)) continue;

                                Way way = tempList[i];
                                wayList.Remove(way.Flag);
                                tempList.RemoveAt(i);
                                way.Clear();
                                Destroy(way);
                                way = null;
                        }
                }
                finally
                {
                        tempList.Clear();
                        tempList = null;
                }

                roomList.Remove(room);
                room.Clear();
                Destroy(room);
                room = null;
        }

        void CollectConnectRoom(Room room)
        {
                Room connectRoom = room.GetConnectRoom(roomQueue);
                while (connectRoom != null)
                {
                        roomQueue[connectRoom.GetId()] = connectRoom;
                        CollectConnectRoom(connectRoom);

                        connectRoom = room.GetConnectRoom(roomQueue);
                }
        }

        void RemoveInvalidRooms()
        {
                int oldCount = roomList.Count;
                while (!startRoom)
                {
                        startRoom = GetRandomOutsizeRoom();
                        if (startRoom.GetConnectRoomCount() != 0) break;

                        RemoveRoom(startRoom);
                }

                //收集可连通房间
                Room temp = startRoom;
                roomQueue[startRoom.GetId()] = startRoom;
                CollectConnectRoom(startRoom);

                //删除不可连通房间
                for (int i = roomList.Count - 1; i >= 0; i--)
                {
                        if (roomQueue.ContainsKey(roomList[i].GetId()))
                                continue;

                        RemoveRoom(roomList[i]);
                }
        }
        #endregion
        void CreateOrnaments()
        {
                for (int i = 0; i < roomList.Count; i++)
                {
                        roomList[i].CreateOrnaments();
                }
        }

        void CreateObstacles()
        {
                for (int i = 0; i < roomList.Count; i++)
                {
                        roomList[i].CreateObstacles();
                }
        }

        #region public
        public void Init ()
        {
		roomList        = new List<Room>();
                wayList         = new Dictionary<int, Way>();
                roomQueue       = new Dictionary<int, Room>();
        }
	
        //clear rooms
        public void ClearScene()
        {
                while (roomList.Count > 0)
                {
                        Room room = roomList[0];
                        roomList.RemoveAt(0);
                        room.Clear();
                        Destroy(room);
                        room = null;
                }

                List<Way> tempList = new List<Way>();
                try
                {
                        
                        foreach (int key in wayList.Keys)
                        {
                                tempList.Add(wayList[key]);

                        }

                        for (int i = tempList.Count - 1; i >= 0; i--)
                        {
                                Way way = tempList[i];
                                wayList.Remove(way.Flag);
                                tempList.RemoveAt(i);
                                way.Clear();
                                Destroy(way);
                                way = null;
                        }

                        wayList.Clear();
                }
                finally
                {
                        tempList = null;
                }

                roomQueue.Clear();
                startRoom = null;

                idSeed = 0;
        }

        // create rooms
        public Room InitScene(MapTemplate mapTemp)
        {
                while (true)
                {
                        ClearScene();

                        AddRoom(mapTemp, DataManager.instance.GetRoomTemp(1), new Vector3(6, 26, 0.5f));
                        AddRoom(mapTemp, DataManager.instance.GetRoomTemp(1), new Vector3(7, 11, 0.5f));
                        //AddRoom(mapTemp, DataManager.instance.GetRoomTemp(6), new Vector3(72, 80, 0.5f));
                        //AddRoom(mapTemp, DataManager.instance.GetRoomTemp(2), new Vector3(10, 10, 0.5f));

                        CreateRooms(mapTemp);
                        CreateDoors();

                        int oldCount = roomList.Count;
                        RemoveInvalidRooms();
                        //可连通房间比例大于80%， 地图生成成功

                        float newCount = roomList.Count;
                        float rate = newCount / oldCount;
                        if (rate >= 0.8f)
                        {
                                CreateOrnaments();
                                CreateObstacles();
                                break;
                        }
                }

                return startRoom;
        }
        #endregion
}
