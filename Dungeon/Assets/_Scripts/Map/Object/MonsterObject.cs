using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterObject : ActiveObject {
        public float fixTargetTime;

        private float curTime;
	// Use this for initialization
	void Awake () {
                curTime = 0f;
	}
	
	// Update is called once per frame
	void Update ()
        {
                curTime += Time.deltaTime;
	        if (curTime >= fixTargetTime)
                {
                        //GetComponent<AutoMoveObject>().SetTarget(GameManager.instance.levelMgr.Hero.transform.position);
                        curTime = 0f;
                }
	}

        public void Init(int id, int roomId, string name, float positionx, float positiony)
        {
                base.Init(id, roomId, name, positionx, positiony, GameConst.MapElementZ, GameConst.Order_Object, "Scavengers_SpriteSheet_32");
                ObjType = GameConst.RoomElementType.Monster;
        }
}
