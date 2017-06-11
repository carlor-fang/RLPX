using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomElement : BaseObject {
        #region declaration
        private GameConst.RoomElementType elementType;
        public GameConst.RoomElementType ElementType { get { return elementType; } set { elementType = value; } }
        private int ornamentId;
        public  int OrnamentId { get { return ornamentId; } set { ornamentId = value; } }
        #endregion

        // Use this for initialization
        void Start ()
        {
		
	}
	
	// Update is called once per frame
	void Update ()
        {
		
	}

        #region public
        public void Init(int id, int roomId, string name, float positionx, float positiony, int zorder, GameConst.RoomElementType type, string imageFile)
        {
                elementType = type;
                ornamentId  = 0;
                //element image
                //SpriteRenderer sr   = GetComponent<SpriteRenderer>();
                //Texture2D texture2d = (Texture2D)Resources.Load(imageFile);
                //Sprite sp           = Sprite.Create(texture2d, sr.sprite.textureRect, new Vector2(0.5f, 0.5f));
                //sr.sprite           = sp;
                base.Init(id, roomId, name, positionx, positiony, GameConst.MapElementZ, zorder, imageFile);
                ObjType = type;
        }

        #endregion
}
