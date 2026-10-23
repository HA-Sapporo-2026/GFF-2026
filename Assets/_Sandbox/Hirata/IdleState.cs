using GFF2026.Common;
using UnityEngine;

namespace Sandbox.Hirata
{
    public class IdleState : IState
    {
        public bool CanTransitionFrom(IState from)
        {
            // MoveStateからIdleStateへ遷移できる
            return from is MoveState;
        }

        public void Enter()
        {
            Debug.Log("IdleState: Enter");
        }

        public void Exit()
        {
            Debug.Log("IdleState: Exit");
        }

        public void Update(IStateMachine stateMachine)
        {
            // 120フレームごとに更新処理を確認する
            if (Time.frameCount % 120 == 0)
            {
                Debug.Log("IdleState: Update");
            }
        }
    }
}