using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveObject : MonoBehaviour {

        public float inverseMoveTime;
        public float moveTime = 0.1f;

        private Vector3 targetPosition;
        private List<Vector3> targetPosList;

        public LayerMask blockingLayer;
        protected BoxCollider2D boxCollider;
        protected Rigidbody2D rb2D;

        // Use this for initialization
        void Awake () {
                boxCollider = GetComponent<BoxCollider2D>();
                rb2D = GetComponent<Rigidbody2D>();

                inverseMoveTime = 1f / moveTime;

                targetPosList = new List<Vector3>();
        }

        // Update is called once per frame
        void Update () {
                
	}

        private bool Move(int xDir, int yDir)
        {
                //Store start position to move from, based on objects current transform position.
                Vector2 start = new Vector2(Mathf.FloorToInt(transform.position.x), Mathf.FloorToInt(transform.position.y)); // transform.position;

                // Calculate end position based on the direction parameters passed in when calling Move.
                targetPosition = start + new Vector2(xDir, yDir);

                StartCoroutine(SmoothMovement(targetPosition));

                return true;
        }

        private bool Move(int xDir, int yDir, out RaycastHit2D hit)
        {
                //Store start position to move from, based on objects current transform position.
                Vector2 start = new Vector2(Mathf.FloorToInt(transform.position.x), Mathf.FloorToInt(transform.position.y)); // transform.position;

                // Calculate end position based on the direction parameters passed in when calling Move.
                targetPosition = start + new Vector2(xDir, yDir);

                //Disable the boxCollider so that linecast doesn't hit this object's own collider.
                boxCollider.enabled = false;

                //Cast a line from start point to end point checking collision on blockingLayer.
                hit = Physics2D.Linecast(start, targetPosition, blockingLayer);

                //Re-enable boxCollider after linecast
                boxCollider.enabled = true;

                //Check if anything was hit
                if (hit.transform == null)
                {
                        //If nothing was hit, start SmoothMovement co-routine passing in the Vector2 end as destination
                        StartCoroutine(SmoothMovement(targetPosition));

                        //Return true to say that Move was successful
                        return true;
                }

                //If something was hit, return false, Move was unsuccesful.
                return false;
        }

        protected IEnumerator SmoothMovement(Vector3 end)
        {
                //Calculate the remaining distance to move based on the square magnitude of the difference between current position and end parameter. 
                //Square magnitude is used instead of magnitude because it's computationally cheaper.
                float sqrRemainingDistance = (transform.position - end).sqrMagnitude;

                //While that distance is greater than a very small amount (Epsilon, almost zero):
                while (sqrRemainingDistance > float.Epsilon)
                {
                        //Find a new position proportionally closer to the end, based on the moveTime
                        Vector3 newPostion = Vector3.MoveTowards(rb2D.position, end, inverseMoveTime * Time.deltaTime);

                        //Debug.Log(rb2D.position);
                        //Call MovePosition on attached Rigidbody2D and move it to the calculated position.
                        rb2D.MovePosition(newPostion);
                        //gameObject.transform.position = newPostion;

                        //Recalculate the remaining distance after moving.
                        sqrRemainingDistance = (transform.position - end).sqrMagnitude;

                        //Return and loop until sqrRemainingDistance is close enough to zero to end the function
                        yield return null;
                }


        }

        public void AttemptMove<T>(int xDir, int yDir)
        where T : Component
        {

                //Set canMove to true if Move was successful, false if failed.
                if (gameObject.tag == "Player")
                {
                        //Hit will store whatever our linecast hits when Move is called.
                        bool canMove = false;
                        RaycastHit2D hit;

                        canMove = Move(xDir, yDir, out hit);
                        if (hit.transform == null)
                                //If nothing was hit, return and don't execute further code.
                                return;

                        T hitComponent = hit.transform.GetComponent<T>();

                        //If canMove is false and hitComponent is not equal to null, meaning MovingObject is blocked and has hit something it can interact with.
                        if (!canMove && hitComponent != null)

                                //Call the OnCantMove function and pass it hitComponent as a parameter.
                                OnCantMove(hitComponent);
                }
                else
                {
                        Move(xDir, yDir);
                }
        }

        public virtual void OnCantMove<T>(T component)
                where T : Component
        {
        }

}
