using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : RoomElement {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

        public void Init(int id, int roomId, string name, float positionx, float positiony)
        {
                Init(id, roomId, name, positionx, positiony, GameConst.Order_Door, GameConst.RoomElementType.Door, "Scavengers_SpriteSheet_25");
                GetComponent<BoxCollider2D>().enabled = false;
        }
}
