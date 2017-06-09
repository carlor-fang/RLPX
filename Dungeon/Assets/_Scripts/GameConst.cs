using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameConst : MonoBehaviour {
        #region declaration
        public static GameConst instance;

        #endregion

        #region const
        public const int RoomWallSize = 1;
        public const int RoomWaySize  = 1;
        public const int RoomOutsize  = RoomWallSize + RoomWaySize;

        public const int        MapElementSize = 32;
        public const float      MapElementZ    = 0;

        public const int Dir_Left       = 1;
        public const int Dir_Top        = 2;
        public const int Dir_Right      = 3;
        public const int Dir_Bottom     = 4;

        public const int WayMaxLength   = 8;//通道最大长度
        public const int WayWidth       = 1;//通道宽度
        public const int OverlapWidth   = WayWidth + RoomWaySize;

        public const int Order_Floor    = 0;
        public const int Order_Wall     = 1;
        public const int Order_Way      = 0;
        public const int Order_Door     = 2;
        public const int Order_Object   = 10;
        public const int Order_Ornament = 3;
        public const int Order_Ostacle  = 4;


        #endregion

        #region inherit
        void Awake()
        {
                if (!instance)
                {
                        instance = this;
                }
                else if (instance != this)
                {
                        Destroy(gameObject);
                }
        }
        #endregion

        #region enum
        public enum RoomElementType
        {
                Floor,
                Wall,
                Door,

                Way,
                Ornament,
                Object 
        }

        public enum ObjectAction
        {
                None,
                Idle,
                Move,
                Attak,
                Hit
        }

        public enum ObjectState
        {
	        None	   = 0,
	        Deactivate = 1,
	        Activate   = 2,
        }

        public enum OrnamentType
        {
                Floor,
                Wall,
                Door
        }
        #endregion
}
