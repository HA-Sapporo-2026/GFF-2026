using GFF2026.Common;
using UnityEngine;

namespace Sandbox.Hirata
{
    public class MoveState : IState
    {
        public bool CanTransitionFrom(IState from)
        {
            // IdleStateからMoveStateへ遷移できる
            return from is IdleState;
        }

        public void Enter()
        {
            Debug.Log("MoveState: Enter");
        }

        public void Exit()
        {
            Debug.Log("MoveState: Exit");
        }

        public void Update(IStateMachine stateMachine)
        {
            // 120フレームごとに更新処理を確認する
            if (Time.frameCount % 120 == 0)
            {
                Debug.Log("MoveState: Update");
            }
        }
    }
}