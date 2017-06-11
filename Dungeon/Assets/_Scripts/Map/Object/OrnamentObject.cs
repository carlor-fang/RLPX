using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrnamentObject : BaseObject {
        public Rect[] imagesRect;
        private OrnamentTemplate ornamentTemp;
        public OrnamentTemplate OrnamentTemp { get { return ornamentTemp; } }

	// Use this for initialization
	void Awake ()
        {
	}
	
	// Update is called once per frame
	void Update () {
		
	}

        #region public
        public void Init(int id, int roomId, string name, float positionx, float positiony, OrnamentTemplate temp)
        {
                ornamentTemp = temp;
                SpriteRenderer sr = GetComponent<SpriteRenderer>();
                Rect rect = Rect.zero;
                if (imagesRect.Length > ornamentTemp.OrnamentType)
                        rect = imagesRect[ornamentTemp.OrnamentType];

                Init(id, roomId, name, positionx, positiony, GameConst.MapElementZ, GameConst.Order_Ornament, "Scavengers_SpriteSheet", rect);

                ObjType = GameConst.RoomElementType.Ornament;
        }
        #endregion

}
