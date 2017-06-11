using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wall : RoomElement {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

        #region
        public void Init(int id, int roomId, string name, float positionx, float positiony)
        {
                base.Init(id, roomId, name, positionx, positiony, GameConst.Order_Wall, GameConst.RoomElementType.Wall, "Scavengers_SpriteSheet_25");
                
        }

        public void SetEnabled(bool value)
        {
                GetComponent<BoxCollider2D>().enabled = value;
        }
        #endregion
}
