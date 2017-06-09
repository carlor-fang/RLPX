using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using LitJson;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

#region data struct
#region map
//地图配置
[Serializable]
public class RoomRate
{
        public int ClassID;
        public int Rate;
}

[Serializable]
public class MapTemplate
{
        public int Level;
        public int Width;
        public int Height;
        public int OverlapCount;
        public List<RoomRate> RoomRateList;
}

[Serializable]
public class MapTemplateList
{
        public List<MapTemplate> list = new List<MapTemplate>();
}
#endregion
#region room
//房间配置
[Serializable]
public class RoomTemplate
{
        public int ClassID;
        public int Width;
        public int Height;
        public int OrnamentCount;
        public int ObstacleRate;
}

[Serializable]
public class RoomTemplateList
{
        public List<RoomTemplate> list = new List<RoomTemplate>();
}
#endregion
#region ornament
//装饰物配置
[Serializable]
public class OrnamentTemplate
{
        public int ClassID;
        public int OrnamentType;
        public int SubType;
}

[Serializable]
public class OrnamentTemplateList
{
        public List<OrnamentTemplate> list = new List<OrnamentTemplate>();
}
#endregion
#endregion

public class DataManager : MonoBehaviour {
        #region declaration
        public static DataManager instance;

        private Dictionary<int, MapTemplate> mapList;
        private Dictionary<int, RoomTemplate> roomList;
        private Dictionary<int, OrnamentTemplate> ornamentList;

        private List<int> ornaments;
        public  List<int> Ornaments { get { return ornaments; } }
        #endregion

        #region inherit
        void Awake()
        {
                if (!instance)
                {
                        instance = this;
                }
                else if (instance != this)
                {
                        Destroy(gameObject);
                }
                Debug.Log("Data mgr awake");
                mapList         = new Dictionary<int, MapTemplate>();
                Debug.Log("Data mgr awake");
                roomList = new Dictionary<int, RoomTemplate>();
                ornamentList = new Dictionary<int, OrnamentTemplate>();
                ornaments = new List<int>();

                LoadFiles();
                Debug.Log("load file over");
        }

        void Start()
        {
        }
        #endregion

        #region private
        void LoadFiles()
        {
                LoadMapTemplate("/StreamingAssets/Setting/MapTemp.json");
                LoadRoomTemplate("/StreamingAssets/Setting/RoomTemp.json");
                LoadOrnamentTemplate("/StreamingAssets/Setting/OrnamentTemp.json");
        }

        string LoadJsonFile(string fileName)
        {
                BinaryFormatter bf = new BinaryFormatter();
                if (!File.Exists(Application.dataPath + fileName))
                {
                        Debug.Log(Application.dataPath + fileName+"not exists");
                        return "";
                }

                StreamReader sr = new StreamReader(Application.dataPath + fileName);
                if (sr == null)
                {
                        Debug.Log(Application.dataPath + fileName + "not stream");
                        return "";
                }

                string json = sr.ReadToEnd();
                if (json.Length <= 0)
                {
                        Debug.Log(Application.dataPath + fileName + "no json length");

                        return "";
                }

                return json;
        }

        //load file
        void LoadMapTemplate(string fileName)
        {
                Debug.Log("load maptemp");
                string json = LoadJsonFile(fileName);
                Debug.Log(json);

                MapTemplateList list = JsonMapper.ToObject<MapTemplateList>(json);
                if (list == null)
                {
                        Debug.Log("load map error");
                        return;
                }
                for (int i = 0; i < list.list.Count; i++)
                {
                        mapList[list.list[i].Level] = list.list[i];
                        int Rate = 0;
                        for (int j = 0; j < list.list[i].RoomRateList.Count; j++)
                        {
                                Rate += list.list[i].RoomRateList[j].Rate;
                                mapList[list.list[i].Level].RoomRateList[j].Rate = Rate;
                        }
                }

                Debug.Log("LoadMapTemplate");
        }

        void LoadRoomTemplate(string fileName)
        {
                string json = LoadJsonFile(fileName);

                RoomTemplateList list = JsonMapper.ToObject<RoomTemplateList>(json);
                if (list == null)
                {
                        return;
                }
                for (int i = 0; i < list.list.Count; i++)
                {
                        roomList[list.list[i].ClassID] = list.list[i];
                }
                Debug.Log("LoadRoomTemplate");
        }

        void LoadOrnamentTemplate(string fileName)
        {
                string json = LoadJsonFile(fileName);

                OrnamentTemplateList list = JsonMapper.ToObject<OrnamentTemplateList>(json);
                if (list == null)
                {
                        return;
                }
                for (int i = 0; i < list.list.Count; i++)
                {
                        ornamentList[list.list[i].ClassID] = list.list[i];
                        ornaments.Add(list.list[i].ClassID);
                }
                Debug.Log("LoadOrnamentTemplate");
        }
        #endregion

        #region public
        public MapTemplate GetMapTemp(int level)
        {
                return mapList[level];
        }

        public RoomTemplate GetRoomTemp(int classID)
        {
                return roomList[classID];
        }

        public OrnamentTemplate GetOrnamentTemp(int classID)
        {
                return ornamentList[classID];
        }
        #endregion
}
