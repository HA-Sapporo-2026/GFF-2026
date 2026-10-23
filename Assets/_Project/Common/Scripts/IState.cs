// Last assigned by 平田

namespace GFF2026.Common
{
    public interface IState
    {

        /// <summary>
        /// そのStateからこのStateに遷移できるか
        /// </summary>
        /// <param name="from"></param>
        /// <returns></returns>
        bool CanTransitionFrom(IState from);

        /// <summary>
        /// そのステートが開始した時
        /// </summary>
        void Enter();

        /// <summary>
        /// そのステートが終了した時
        /// </summary>
        void Exit();

        /// <summary>
        /// ステートの更新処理
        /// </summary>
        /// <param name="stateMachine"></param>
        void Update(IStateMachine stateMachine);
    }
}