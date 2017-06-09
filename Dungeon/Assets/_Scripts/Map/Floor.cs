using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Floor : RoomElement {

	// Use this for initialization
	void Start ()
        {
		
	}
	
	// Update is called once per frame
	void Update ()
        {
		
	}

        #region public
        public void Init(int id, int roomId, string name, float positionx, float positiony)
        {
                Init(id, roomId, name, positionx, positiony, GameConst.Order_Floor, GameConst.RoomElementType.Floor, "Scavengers_SpriteSheet_32");
        }
        #endregion
}
