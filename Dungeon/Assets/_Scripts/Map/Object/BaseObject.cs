using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseObject : MonoBehaviour {
        private int id;
        public int ID {get { return id; }}

        protected int roomId;
        public int RoomId {get { return RoomId; } }

        private string name;
        public string Name {get { return name; } set { name = value; } }

        private GameConst.ObjectState state;
        public GameConst.ObjectState State { get { return state; } set { state = value; } }

        private Vector3 position;
        public Vector3 Position { get { return position; } set { position = value; } }

        private string imageName;
        public string ImageName { get { return imageName; } }

        void Awake()
        {
                state = GameConst.ObjectState.None;        
        }
        // Use this for initialization
        void Start ()
        {
		
	}
	
	// Update is called once per frame
	void Update ()
        {
		
	}

        public virtual void Clear()
        {
                name = null;
                imageName = null;
                Destroy(gameObject);
        }

        public virtual void Init(int id, int roomId, string name, float positionx, float positiony, float positionz, int zorder, string imageFile)
        {
                this.id = id;
                this.roomId = roomId;
                this.name = name;
                this.state = GameConst.ObjectState.Deactivate;

                gameObject.name = name;
                SpriteRenderer render = GetComponent<SpriteRenderer>();
                render.sortingOrder = zorder;

                position = new Vector3(positionx, positiony, GameConst.MapElementZ);
                imageName = imageFile;

                gameObject.transform.position = new Vector3(positionx, positiony, positionz);

        }

        public virtual void Init(int id, int roomId, string name, float positionx, float positiony, float positionz, int zorder, string imageFile, Rect rect)
        {
                Init(id, roomId, name, positionx, positiony, positionz, zorder, imageFile);

                SpriteRenderer sr   = GetComponent<SpriteRenderer>();
                Texture2D texture2d = (Texture2D)Resources.Load(imageFile);
                Sprite sp = Sprite.Create(texture2d, rect, new Vector2(0.5f, 0.5f), GameConst.MapElementSize);
                sr.sprite = sp;
        }
}
