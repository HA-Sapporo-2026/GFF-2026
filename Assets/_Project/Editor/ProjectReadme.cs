using System.IO;
using UnityEditor;
using UnityEngine;

namespace GFF2026.EditorTools
{
    /// <summary>
    /// リポジトリ直下の README.md をインスペクタに表示するためのアセット。
    /// 中身は持たず、表示は ProjectReadmeEditor が README.md を読んで行う。
    /// </summary>
    public class ProjectReadme : ScriptableObject
    {
        private const string DefaultAssetPath = "Assets/_Project/Editor/README.asset";

        /// <summary>リポジトリ直下の README.md の絶対パス</summary>
        public static string MarkdownPath =>
            Path.GetFullPath(Path.Combine(Application.dataPath, "..", "README.md"));

        [MenuItem("Help/GFF-2026 README", priority = 0)]
        public static void Select()
        {
            ProjectReadme readme = FindOrCreate();
            Selection.activeObject = readme;
        }

        private static ProjectReadme FindOrCreate()
        {
            string[] guids = AssetDatabase.FindAssets("t:" + nameof(ProjectReadme));
            if (guids.Length > 0)
            {
                string path = AssetDatabase.GUIDToAssetPath(guids[0]);
                return AssetDatabase.LoadAssetAtPath<ProjectReadme>(path);
            }

            // 通常はリポジトリに入っているので、ここに来るのはアセットを消してしまったときだけ
            ProjectReadme created = CreateInstance<ProjectReadme>();
            AssetDatabase.CreateAsset(created, DefaultAssetPath);
            AssetDatabase.SaveAssets();
            return created;
        }
    }
}
