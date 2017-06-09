using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Way : RoomElement {
        private Room room1;
        private Room room2;

        private int  flag;
        public int Flag { get { return flag; } set { flag = value; } }
	
        // Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

        public void Init(int id, int roomId, string name, float positionx, float positiony)
        {
                Init(id, roomId, name, positionx, positiony, GameConst.Order_Way, GameConst.RoomElementType.Way, "Scavengers_SpriteSheet_25");
        }

        public void SetConnectRoom(Room rooma, Room roomb)
        {
                room1 = rooma;
                room2 = roomb;
        }

        public bool ConnectedRoom(Room room)
        {
                return room1 == room || room2 == room;
        }
}
