using GFF2026.Common;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Sandbox.Hirata
{
    public class StateMachineTester : MonoBehaviour
    {
        private StateMachine stateMachine;

        void Start()
        {
            // StateMachineを作成する
            stateMachine = new StateMachine();

            // ステートを登録する
            stateMachine.Register(new IdleState());
            stateMachine.Register(new MoveState());

            // 最初のステートをIdleStateにする
            stateMachine.SetInitialState<IdleState>();
        }

        void Update()
        {
            // 現在のステートを更新する
            stateMachine.Update();

            if (Keyboard.current == null)
            {
                return;
            }

           

            
        }
    }
}