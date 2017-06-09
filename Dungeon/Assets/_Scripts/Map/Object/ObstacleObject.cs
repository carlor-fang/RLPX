using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleObject : BaseObject {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

        public void Init(int id, int roomId, string name, float positionx, float positiony)
        {
                Init(id, roomId, name, positionx, positiony, GameConst.MapElementZ, GameConst.Order_Ostacle, "Scavengers_SpriteSheet_28");
        }
}
