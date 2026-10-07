using UnityEditor;

namespace GFF2026.EditorTools
{
    /// <summary>
    /// Unity の起動時に README を選択し、インスペクタに表示する。
    /// スクリプトの再コンパイルでは表示し直さないよう、SessionState で1回に限る。
    /// </summary>
    [InitializeOnLoad]
    internal static class ProjectReadmeStartup
    {
        const string ShownKey = "GFF2026.ProjectReadme.ShownThisSession";

        static ProjectReadmeStartup()
        {
            if (SessionState.GetBool(ShownKey, false)) return;

            // 起動直後はアセットの読み込みが終わっていないので、エディタの準備ができてから選択する
            EditorApplication.delayCall += ShowOnStartup;
        }

        static void ShowOnStartup()
        {
            if (SessionState.GetBool(ShownKey, false)) return;
            SessionState.SetBool(ShownKey, true);

            if (EditorApplication.isPlayingOrWillChangePlaymode) return;

            ProjectReadme.Select();
        }
    }
}
