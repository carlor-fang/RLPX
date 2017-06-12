using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class AutoMoveObject : MoveObject {
        private Vector2[] dirPos8 = {
                new Vector2(-1, 0),
                //new Vector2(-1, 1),
                new Vector2(0, 1),
                //new Vector2(1, 1),
                new Vector2(1, 0),
                //new Vector2(1, -1),
                new Vector2(0, -1)
                //new Vector2(-1, -1)
                                };
        private bool isNewTarget;
        private Vector2 target;
        private Room curRoom;

        private Dictionary<int, PathNode> openList;
        private Dictionary<int, PathNode> closeList;

        private PathNode curNode;
        private List<PathNode> pathList;

        void Awake()
        {
                openList  = new Dictionary<int, PathNode>();
                closeList = new Dictionary<int, PathNode>();
                pathList  = new List<PathNode>();

                rb2D = GetComponent<Rigidbody2D>();

                inverseMoveTime = 1f / moveTime;
        }

        void Update()
        {
                if (curNode != null)
                {
                        if (transform.position.x != curNode.pos.x || transform.position.y != curNode.pos.y)
                        {
                                //AutoMove();
                        }
                        else if (isNewTarget)
                        {
                                ClearPath();
                                FindPath();
                        }
                        else
                        {
                                Destroy(curNode);
                                curNode = null;

                                pathList.RemoveAt(pathList.Count - 1);
                                if (pathList.Count > 0)
                                {
                                        curNode = pathList[pathList.Count - 1];
                                        AutoMove();
                                }
                                else
                                        curNode = null;

                                //if (curNode != null)
                                //        Debug.Log("------curNode:" + curNode.pos);
                                //else
                                //        Debug.Log("over------");
                        }
                }
        }

        private PathNode NewPathNode(PathNode parent)
        {
                GameObject Prefab = (ResourceManager.instance.GetAsset<GameObject>("Prefabs/PathNode"));
                GameObject gameObject = Instantiate(Prefab);
                PathNode   node = gameObject.GetComponent<PathNode>();
                node.parent = parent;

                return node;
        }

        private PathNode FindPath(PathNode node, Vector2 end)
        {
                for (int i = 0; i < dirPos8.Length; i++)
                {
                        int x = Mathf.FloorToInt(node.pos.x + dirPos8[i].x);
                        int y = Mathf.FloorToInt(node.pos.y + dirPos8[i].y);
                        int key = GetPosKey(x, y);
                        if (closeList.ContainsKey(key)) continue;
                        if (!curRoom.IsInBounds(x, y)) continue;
                        if (curRoom.IsBlock(x, y)) continue;

                        if (openList.ContainsKey(key)) continue;
                        PathNode temp = NewPathNode(node);
                        temp.parent = node;
                        temp.pos = new Vector2(x, y);
                        temp.h = (Mathf.Abs(end.x - temp.pos.x) + Mathf.Abs(end.y - temp.pos.y)) * 10;
                        temp.g = node.g + (Mathf.Abs(dirPos8[i].x) + Mathf.Abs(dirPos8[i].y)) * 10;
                        temp.f = temp.h + temp.g;
                        temp.key = key;
                        openList[temp.key] = temp;
                        if (temp.pos.x == end.x && temp.pos.y == end.y)
                                return temp;
                }
                openList.Remove(node.key);
                closeList[node.key] = node;

                List<int> tempList = new List<int>();
                foreach (int key in openList.Keys)
                {
                        tempList.Add(key);
                }

                Comparison<int> comparison = new Comparison<int>((int a, int b) =>
                {
                        if (openList[a].f < openList[b].f)
                                return -1;
                        else if (openList[a].f == openList[b].f)
                                return 0;
                        else
                                return 1;
                }
                );
                tempList.Sort(comparison);

                for (int i = 0; i < tempList.Count; i++)
                {
                        if (!openList.ContainsKey(tempList[i]))
                                continue;
                        PathNode temp = FindPath(openList[tempList[i]], end);
                        if (temp != null)
                                return temp;
                }

                return null;
        }

        private int GetPosKey(float col, float row)
        {
                return Mathf.FloorToInt(col * 100000 + row);
        }

        private void FindPath()
        {
                Vector2 start = transform.position;
                Vector2 end   = target;

                PathNode startNode = NewPathNode(null);
                startNode.pos = start;
                startNode.key = GetPosKey(start.x, start.y);
                startNode.h = (Mathf.Abs(Mathf.FloorToInt(start.x - end.x))
                        + Mathf.Abs(Mathf.FloorToInt(start.y - end.y))) * 10;
                startNode.f = startNode.h + startNode.g;

                openList[startNode.key] = startNode;

                PathNode endNode = FindPath(startNode, end);
                if (endNode != null)
                {
                        PathNode temp = endNode.parent;
                        while (temp != null && temp.parent != null)
                        {
                                pathList.Add(temp);
                                if (temp.parent == startNode)
                                        break;
                                else
                                        temp = temp.parent;
                        }

                        //for (int i = pathList.Count - 1; i >= 0; i--)
                        //{
                        //        Debug.Log("//////////////path node:" + pathList[i].pos);
                        //}
                        if (pathList.Count > 0)
                        {
                                curNode = pathList[pathList.Count - 1];
                                AutoMove();
                                //Debug.Log("//////////////curNode :" + curNode.pos);
                        }
                }
                isNewTarget = false;
        }

        void ClearPath()
        {
                List<int> tempList = new List<int>();
                foreach(int key in openList.Keys)
                {
                        tempList.Add(key);
                }
                for (int i = 0; i < tempList.Count; i++)
                {
                        PathNode temp = openList[tempList[i]];
                        openList.Remove(tempList[i]);
                        Destroy(temp);
                }
                tempList.Clear();

                foreach (int key in closeList.Keys)
                {
                        tempList.Add(key);
                }
                for (int i = 0; i < tempList.Count; i++)
                {
                        PathNode temp = closeList[tempList[i]];
                        closeList.Remove(tempList[i]);
                        Destroy(temp);
                }
                tempList.Clear();

                pathList.Clear();
                curNode = null;
        }

        public void SetRoom(Room room)
        {
                curRoom = room;
        }

        public void SetTarget(Vector2 value)
        {
                if (curRoom.IsInBounds(value.x, value.y) && !curRoom.IsBlock(value.x, value.y))
                {
                        target = value;
                        isNewTarget = true;
                        if (curNode == null)
                        {
                                ClearPath();
                                FindPath();
                        }
                }
        }

        //MoveEnemy is called by the GameManger each turn to tell each Enemy to try to move towards the player.
        public void AutoMove()
        {
                //Declare variables for X and Y axis move directions, these range from -1 to 1.
                //These values allow us to choose between the cardinal directions: up, down, left and right.
                if (curNode != null)
                {
                        int xDir = Mathf.FloorToInt(curNode.pos.x - transform.position.x);
                        int yDir = Mathf.FloorToInt(curNode.pos.y - transform.position.y);

                        if (xDir != 0 || yDir != 0)
                                AttemptMove<Player>(xDir, yDir);
                }
        }
}