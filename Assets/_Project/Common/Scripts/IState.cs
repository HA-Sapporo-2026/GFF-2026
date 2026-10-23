// Last assigned by 平田

namespace GFF2026.Common
{
    public interface IState
    {
        // そのStateからこのStateに遷移できるか
        bool TryTransition(IState from);

        // そのステートが開始した時
        void Enter();

        // そのステートが終了した時
        void Exit();

        // ステートの更新処理
        void Update(IStateMachine stateMachine);
    }
}