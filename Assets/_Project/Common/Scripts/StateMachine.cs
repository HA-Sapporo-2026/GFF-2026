// Last assigned by 平田
using System.Collections.Generic;
using System.Linq;

namespace GFF2026.Common
{
    public interface IStateMachine
    {


        /// <summary>
        /// 指定したステートに遷移できるか確認し、
        /// 可能なら実際に遷移する
        /// </summary>
        /// <typeparam name="TState"></typeparam>
        /// <returns></returns>
        bool TryChangeState<TState>() where TState : IState;
    }

    public class StateMachine : IStateMachine
    {
        /// <summary>
        /// StateMachineが管理するステートを保存する
        /// HashSetを使って、ステートのインスタンスをまとめて管理する
        /// </summary>
        readonly HashSet<IState> states = new();

        /// <summary>
        /// 現在実行しているステート
        /// 最初は何も設定されていないためnull
        /// </summary>
        IState currentState;


        /// <summary>
        /// ステートをStateMachineに登録する
        /// </summary>
        /// <param name="state"></param>
        public void Register(IState state)
        {
            // 登録するステートを管理リストに追加する
            states.Add(state);
        }


        /// <summary>
        /// 指定したステートへの遷移を試みる
        /// 遷移できたらtrue、できなければfalseを返す
        /// </summary>
        /// <typeparam name="TState"></typeparam>
        /// <returns></returns>
        public bool TryChangeState<TState>() where TState : IState
        {
            // 登録済みのステートから、指定した型のステートを探す
            IState nextState = states.First(x => x is TState);

            // 遷移先のステートに、現在のステートから遷移できるか確認する
            if (!nextState.TryTransition(currentState))
            {
                // 遷移できない場合は、何も変更せずfalseを返す
                return false;
            }

            // 現在のステートがあれば、終了処理を実行する
            currentState?.Exit();

            // 遷移先のステートを現在のステートに設定する
            currentState = nextState;

            // 新しいステートの開始処理を実行する
            currentState.Enter();

            // 遷移が完了したことを呼び出し元に伝える
            return true;
        }


        /// <summary>
        /// ゲーム開始時などに、最初のステートを設定する
        /// </summary>
        /// <typeparam name="TState"></typeparam>
        public void SetInitialState<TState>() where TState : IState
        {
            // 登録済みのステートから、指定した型のステートを探す
            IState initialState = states.First(x => x is TState);

            // 見つけたステートを現在のステートに設定する
            currentState = initialState;

            // 最初のステートの開始処理を実行する
            currentState.Enter();
        }


        /// <summary>
        /// 現在のステートを更新する
        /// </summary>
        public void Update()
        {
            // 現在のステートが存在する場合、その更新処理を実行する
            // StateMachine自身を渡して、ステート側からも管理機能を使えるようにする
            currentState?.Update(this);
        }
    }
}