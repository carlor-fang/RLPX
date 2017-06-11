using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : ActiveObject {

	// Use this for initialization
	void Awake ()
        {
                moveObject = GetComponent<MoveObject>();
        }

        // Update is called once per frame
        void Update()
        {
                int horizontal = 0;     //Used to store the horizontal move direction.
                int vertical = 0;               //Used to store the vertical move direction.

                //Check if we are running either in the Unity editor or in a standalone build.
#if UNITY_STANDALONE || UNITY_WEBPLAYER

                //Get input from the input manager, round it to an integer and store in horizontal to set x axis move direction
                horizontal = (int)(Input.GetAxisRaw("Horizontal"));

                //Get input from the input manager, round it to an integer and store in vertical to set y axis move direction
                vertical = (int)(Input.GetAxisRaw("Vertical"));

                //Check if moving horizontally, if so set vertical to zero.
                if (horizontal != 0)
                {
                        vertical = 0;
                }
                //Check if we are running on iOS, Android, Windows Phone 8 or Unity iPhone
#elif UNITY_IOS || UNITY_ANDROID || UNITY_WP8 || UNITY_IPHONE
			
			//Check if Input has registered more than zero touches
			if (Input.touchCount > 0)
			{
				//Store the first touch detected.
				Touch myTouch = Input.touches[0];
				
				//Check if the phase of that touch equals Began
				if (myTouch.phase == TouchPhase.Began)
				{
					//If so, set touchOrigin to the position of that touch
					touchOrigin = myTouch.position;
				}
				
				//If the touch phase is not Began, and instead is equal to Ended and the x of touchOrigin is greater or equal to zero:
				else if (myTouch.phase == TouchPhase.Ended && touchOrigin.x >= 0)
				{
					//Set touchEnd to equal the position of this touch
					Vector2 touchEnd = myTouch.position;
					
					//Calculate the difference between the beginning and end of the touch on the x axis.
					float x = touchEnd.x - touchOrigin.x;
					
					//Calculate the difference between the beginning and end of the touch on the y axis.
					float y = touchEnd.y - touchOrigin.y;
					
					//Set touchOrigin.x to -1 so that our else if statement will evaluate false and not repeat immediately.
					touchOrigin.x = -1;
					
					//Check if the difference along the x axis is greater than the difference along the y axis.
					if (Mathf.Abs(x) > Mathf.Abs(y))
						//If x is greater than zero, set horizontal to 1, otherwise set it to -1
						horizontal = x > 0 ? 1 : -1;
					else
						//If y is greater than zero, set horizontal to 1, otherwise set it to -1
						vertical = y > 0 ? 1 : -1;
				}
			}
			
#endif //End of mobile platform dependendent compilation section started above with #elif
                //Check if we have a non-zero value for horizontal or vertical
                if (horizontal != 0 || vertical != 0)
                {
                        Debug.Log("CALL CALL");
                        //Call AttemptMove passing in the generic parameter Wall, since that is what Player may interact with if they encounter one (by attacking it)
                        //Pass in horizontal and vertical as parameters to specify the direction to move Player in.
                        moveObject.AttemptMove<Wall>(horizontal, vertical);
                }

        }


        public void Init(int id, int roomId, string name, float positionx, float positiony)
        {
                base.Init(id, roomId, name, positionx, positiony, GameConst.MapElementZ, GameConst.Order_Object, "Scavengers_SpriteSheet_32");
                ObjType = GameConst.RoomElementType.Player;
        }

        public void ChangeRoom(int roomId)
        {
                this.roomId = roomId;
        }
}
