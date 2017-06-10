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

        void SetDirection(int dir)
        {
                switch(dir)
                {
                        case GameConst.Dir_Top:
                        case GameConst.Dir_Bottom:
                                {
                                //boxcollider 在两侧，不用旋转
                                }
                                break;
                        case GameConst.Dir_Left:
                        case GameConst.Dir_Right:
                                {
                                        gameObject.transform.rotation = Quaternion.Euler(0f, 0f, 90f);
                                }
                                break;
                }
        }

        //dir 竖向或横向，根据这个修改碰撞
        public void Init(int id, int roomId, string name, float positionx, float positiony, int dir)
        {
                Init(id, roomId, name, positionx, positiony, GameConst.Order_Way, GameConst.RoomElementType.Way, "Scavengers_SpriteSheet_25");
                SetDirection(dir);
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
