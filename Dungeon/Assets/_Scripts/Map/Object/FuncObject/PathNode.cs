using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathNode : MonoBehaviour {
        //终点所在行减去当前格所在行的绝对值
        //与终点所在列减去当前格所在列的绝对值之和
        //即预估移动耗费
        public float h = 0;
        //当前结点的父结点移动到当前结点的预估移动耗费
        public float g = 0;
        //h+g;
        public float f = 0;

        public int key = 0;

        public Vector2 pos;

        public PathNode parent = null;

        // Use this for initialization
        void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
