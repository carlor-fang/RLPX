using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ActiveObject : BaseObject {
        struct AOAction
        {
                public GameConst.ObjectAction action;
                public Vector3 pos;
                public int skillId;
        }

        //component
        protected MoveObject            moveObject;

        private GameConst.ObjectAction  curAction;
        public  GameConst.ObjectAction  CurAction { get { return curAction; } }
        private AOAction                nextAction;

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update ()
        {
                if (State == GameConst.ObjectState.Activate)
                {
                        UpdateAction();
                }
	}

        public override void Clear()
        {
                base.Clear();
        }

        public override void Init(int id, int roomId, string name, float positionx, float positiony, float positionz, int zorder, string imageFile)
        {
                base.Init(id, roomId, name, positionx, positiony, positionz, zorder, imageFile);

                curAction = GameConst.ObjectAction.None;
        }

        #region action
        private void UpdateAction()
        {
                if (curAction == GameConst.ObjectAction.None)
                {
                        if (! NextActionProc())
                        {
                                SetNextAction(GameConst.ObjectAction.Idle);  
                        }
                }
                else if (curAction == GameConst.ObjectAction.Idle)
                {
                        NextActionProc();
                }
                else if (curAction == GameConst.ObjectAction.Move)
                {
                }
        }

        private bool NextActionProc()
        {
                if (nextAction.action == GameConst.ObjectAction.None)
                        return false;

                GameConst.ObjectAction action = nextAction.action;
                nextAction.action = GameConst.ObjectAction.None;

                //if (action == GameConst.ObjectAction.Move)
                if (action == GameConst.ObjectAction.Attak)
                        CastSkillProc(nextAction.skillId);
                else if (action == GameConst.ObjectAction.Hit)
                        SetCurAction(GameConst.ObjectAction.Hit, 0);
                else if (action == GameConst.ObjectAction.Idle)
                        SetCurAction(GameConst.ObjectAction.Idle, 0);
                else
                        return false;

                return true;
        }

        private void SetNextAction(GameConst.ObjectAction value)
        {
                nextAction.action = value;
        }

        private void SetCurAction(GameConst.ObjectAction value, int skillId)
        {
                curAction = value;
        }
        #region attack
        private void CastSkillProc(int skillId)
        {

        }
        #endregion
        #endregion
}
