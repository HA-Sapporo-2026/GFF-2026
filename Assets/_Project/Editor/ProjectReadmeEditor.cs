using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

namespace GFF2026.EditorTools
{
    /// <summary>
    /// README.md を簡易的に Markdown として解釈し、インスペクタに描画する。
    /// 対応：見出し / 段落 / 箇条書き / チェックボックス / コードブロック / 引用（GitHubの [!WARNING] 等） / 表 / 区切り線 / 太字 / インラインコード
    /// </summary>
    [CustomEditor(typeof(ProjectReadme))]
    internal class ProjectReadmeEditor : Editor
    {
        enum BlockType { Heading, Paragraph, List, Code, Quote, Table, Rule }

        class ListItem
        {
            public int indent;
            public string marker;
            public string text;
        }

        class Block
        {
            public BlockType type;
            public int level;
            public string text;
            public MessageType alert = MessageType.None;
            public readonly List<ListItem> items = new();
            public readonly List<string[]> rows = new();
            public float[] columnWeights;
            public Vector2 scroll;
        }

        static readonly Regex s_headingRegex = new(@"^(#{1,6})\s+(.*)$");
        static readonly Regex s_listRegex = new(@"^(\s*)([-*]|\d+\.)\s+(.*)$");
        static readonly Regex s_tableSeparatorRegex = new(@"^[\s|:\-]+$");
        static readonly Regex s_alertRegex = new(@"^\[!(NOTE|TIP|IMPORTANT|WARNING|CAUTION)\]\s*$");
        static readonly Regex s_linkRegex = new(@"\[([^\]]+)\]\(([^)]+)\)");
        static readonly Regex s_tagRegex = new(@"<[^>]+>");

        List<Block> blocks;
        DateTime loadedWriteTime;
        string error;

        // Mac、Windows の順に、標準で入っている等幅フォント
        static readonly string[] s_monoFontCandidates = { "Menlo", "Consolas", "MS Gothic", "Courier New" };
        static Font s_monoFont;
        static bool s_monoFontResolved;
        GUIStyle h1, h2, h3, body, cell, code, quote;

        string CodeColor => EditorGUIUtility.isProSkin ? "#E5A06A" : "#A3410E";
        string LinkColor => EditorGUIUtility.isProSkin ? "#7FB2F0" : "#1F5FAD";
        Color LineColor => EditorGUIUtility.isProSkin ? new Color(1f, 1f, 1f, 0.15f) : new Color(0f, 0f, 0f, 0.15f);

        protected override void OnHeaderGUI()
        {
            using (new EditorGUILayout.HorizontalScope(EditorStyles.toolbar))
            {
                GUILayout.Label("README.md", EditorStyles.boldLabel);
                GUILayout.FlexibleSpace();
                if (GUILayout.Button("再読み込み", EditorStyles.toolbarButton))
                {
                    blocks = null;
                }
                if (GUILayout.Button("外部エディタで開く", EditorStyles.toolbarButton))
                {
                    EditorUtility.OpenWithDefaultApp(ProjectReadme.MarkdownPath);
                }
            }
        }

        public override void OnInspectorGUI()
        {
            if (Event.current.type == EventType.Layout) ReloadIfChanged();
            InitStyles();

            if (error != null)
            {
                EditorGUILayout.HelpBox(error, MessageType.Error);
                return;
            }
            if (blocks == null) return;

            foreach (var block in blocks)
            {
                DrawBlock(block);
            }
            GUILayout.Space(20f);
        }

        // ---------------------------------------------------------------
        // 読み込み
        // ---------------------------------------------------------------

        void ReloadIfChanged()
        {
            var path = ProjectReadme.MarkdownPath;
            if (!File.Exists(path))
            {
                blocks = null;
                error = "README.md が見つかりません。\n" + path;
                return;
            }

            var writeTime = File.GetLastWriteTimeUtc(path);
            if (blocks != null && writeTime == loadedWriteTime) return;

            loadedWriteTime = writeTime;
            error = null;
            blocks = Parse(File.ReadAllLines(path, Encoding.UTF8));
        }

        static List<Block> Parse(string[] rawLines)
        {
            List<Block> blocks = new();
            var lines = Array.ConvertAll(rawLines, l => l.TrimEnd('\r'));
            var i = 0;

            while (i < lines.Length)
            {
                var line = lines[i];
                var trimmed = line.Trim();

                if (trimmed.Length == 0)
                {
                    i++;
                    continue;
                }

                // コードブロック
                if (trimmed.StartsWith("```"))
                {
                    var sb = new StringBuilder();
                    i++;
                    while (i < lines.Length && !lines[i].Trim().StartsWith("```"))
                    {
                        if (sb.Length > 0) sb.Append('\n');
                        sb.Append(lines[i]);
                        i++;
                    }
                    i++; // 閉じの ```
                    blocks.Add(new Block { type = BlockType.Code, text = sb.ToString() });
                    continue;
                }

                // 見出し
                var heading = s_headingRegex.Match(line);
                if (heading.Success)
                {
                    blocks.Add(new Block
                    {
                        type = BlockType.Heading,
                        level = heading.Groups[1].Value.Length,
                        text = heading.Groups[2].Value
                    });
                    i++;
                    continue;
                }

                // 区切り線
                if (IsRule(trimmed))
                {
                    blocks.Add(new Block { type = BlockType.Rule });
                    i++;
                    continue;
                }

                // 引用
                if (trimmed.StartsWith(">"))
                {
                    var block = new Block { type = BlockType.Quote };
                    var quoteLines = new List<string>();
                    while (i < lines.Length && lines[i].TrimStart().StartsWith(">"))
                    {
                        var content = lines[i].TrimStart().Substring(1);
                        if (content.StartsWith(" ")) content = content.Substring(1);
                        quoteLines.Add(content);
                        i++;
                    }

                    if (quoteLines.Count > 0)
                    {
                        var alert = s_alertRegex.Match(quoteLines[0]);
                        if (alert.Success)
                        {
                            block.alert = alert.Groups[1].Value switch
                            {
                                "WARNING" => MessageType.Warning,
                                "CAUTION" => MessageType.Error,
                                _ => MessageType.Info
                            };
                            quoteLines.RemoveAt(0);
                        }
                    }
                    block.text = string.Join("\n", quoteLines);
                    blocks.Add(block);
                    continue;
                }

                // 表
                if (trimmed.StartsWith("|"))
                {
                    var block = new Block { type = BlockType.Table };
                    while (i < lines.Length && lines[i].Trim().StartsWith("|"))
                    {
                        var row = lines[i].Trim();
                        i++;
                        if (s_tableSeparatorRegex.IsMatch(row)) continue;

                        row = row.Trim('|');
                        var cells = row.Split('|');
                        for (var c = 0; c < cells.Length; c++) cells[c] = cells[c].Trim();
                        block.rows.Add(cells);
                    }
                    block.columnWeights = CalcColumnWeights(block.rows);
                    blocks.Add(block);
                    continue;
                }

                // 箇条書き
                if (s_listRegex.IsMatch(line))
                {
                    var block = new Block { type = BlockType.List };
                    while (i < lines.Length)
                    {
                        var current = lines[i];
                        var item = s_listRegex.Match(current);
                        if (item.Success)
                        {
                            var marker = item.Groups[2].Value;
                            var text = item.Groups[3].Value;
                            if (text.StartsWith("[ ] ")) { marker = "☐"; text = text.Substring(4); }
                            else if (text.StartsWith("[x] ") || text.StartsWith("[X] ")) { marker = "☑"; text = text.Substring(4); }
                            else if (marker == "-" || marker == "*") marker = "•";

                            block.items.Add(new ListItem
                            {
                                indent = item.Groups[1].Value.Length / 2,
                                marker = marker,
                                text = text
                            });
                            i++;
                            continue;
                        }

                        // インデントされた続きの行は、直前の項目に含める
                        if (current.StartsWith(" ") && current.Trim().Length > 0 && block.items.Count > 0)
                        {
                            block.items[^1].text += "\n" + current.Trim();
                            i++;
                            continue;
                        }
                        break;
                    }
                    blocks.Add(block);
                    continue;
                }

                // 段落
                {
                    var sb = new StringBuilder();
                    while (i < lines.Length)
                    {
                        var current = lines[i];
                        var t = current.Trim();
                        if (t.Length == 0 || t.StartsWith("```") || t.StartsWith(">") || t.StartsWith("|")
                            || IsRule(t) || s_headingRegex.IsMatch(current) || s_listRegex.IsMatch(current))
                        {
                            break;
                        }
                        if (sb.Length > 0) sb.Append('\n');
                        sb.Append(t);
                        i++;
                    }
                    blocks.Add(new Block { type = BlockType.Paragraph, text = sb.ToString() });
                }
            }

            return blocks;
        }

        static bool IsRule(string trimmed) => trimmed == "---" || trimmed == "***" || trimmed == "___";

        static float[] CalcColumnWeights(List<string[]> rows)
        {
            var columns = rows.Aggregate(0, (current, row) => Mathf.Max(current, row.Length));

            var weights = new float[columns];
            foreach (var row in rows)
            {
                for (var i = 0; i < row.Length; i++)
                {
                    // 全角文字は半角の約2倍の幅として数える
                    var length = row[i].Replace("`", "").Replace("**", "").Sum(ch => ch < 0x80 ? 1f : 2f);
                    weights[i] = Mathf.Max(weights[i], Mathf.Clamp(length, 6f, 50f));
                }
            }
            return weights;
        }

        // ---------------------------------------------------------------
        // 描画
        // ---------------------------------------------------------------

        void DrawBlock(Block block)
        {
            switch (block.type)
            {
                case BlockType.Heading: DrawHeading(block); break;
                case BlockType.Paragraph: DrawParagraph(block); break;
                case BlockType.List: DrawList(block); break;
                case BlockType.Code: DrawCode(block); break;
                case BlockType.Quote: DrawQuote(block); break;
                case BlockType.Table: DrawTable(block); break;
                case BlockType.Rule: DrawRule(); break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        void DrawHeading(Block block)
        {
            var style = block.level switch
            {
                1 => h1,
                2 => h2,
                _ => h3
            };
            GUILayout.Space(block.level <= 2 ? 12f : 8f);
            RichLabel(block.text, style, true);
            if (block.level <= 2) DrawLine(1f);
            GUILayout.Space(4f);
        }

        void DrawParagraph(Block block)
        {
            RichLabel(block.text, body, false);
            GUILayout.Space(6f);
        }

        void DrawList(Block block)
        {
            foreach (var item in block.items)
            {
                using (new EditorGUILayout.HorizontalScope())
                {
                    GUILayout.Space(8f + item.indent * 16f);
                    GUILayout.Label(item.marker, body, GUILayout.Width(item.marker.Length > 1 ? 22f : 14f));
                    RichLabel(item.text, body, false);
                }
            }
            GUILayout.Space(6f);
        }

        void DrawCode(Block block)
        {
            var content = new GUIContent(block.text);
            var size = code.CalcSize(content);

            using (new EditorGUILayout.VerticalScope(EditorStyles.helpBox))
            {
                // 横に長い行は折り返さず、横スクロールで見せる
                block.scroll = EditorGUILayout.BeginScrollView(
                    block.scroll, false, false,
                    GUI.skin.horizontalScrollbar, GUIStyle.none, GUIStyle.none,
                    GUILayout.Height(size.y + 16f));
                GUILayout.Label(content, code, GUILayout.Width(size.x), GUILayout.Height(size.y));
                EditorGUILayout.EndScrollView();
            }
            GUILayout.Space(6f);
        }

        void DrawQuote(Block block)
        {
            using (new EditorGUILayout.HorizontalScope(EditorStyles.helpBox))
            {
                if (block.alert != MessageType.None)
                {
                    var icon = block.alert switch
                    {
                        MessageType.Warning => "console.warnicon",
                        MessageType.Error => "console.erroricon",
                        _ => "console.infoicon"
                    };
                    GUILayout.Label(EditorGUIUtility.IconContent(icon), GUILayout.Width(32f), GUILayout.Height(32f));
                }
                RichLabel(block.text, quote, false);
            }
            GUILayout.Space(6f);
        }

        void DrawTable(Block block)
        {
            var totalWeight = block.columnWeights.Sum();

            // インスペクタの左右の余白とスクロールバーの分を引く
            var available = EditorGUIUtility.currentViewWidth - 48f;

            using (new EditorGUILayout.VerticalScope(EditorStyles.helpBox))
            {
                for (var r = 0; r < block.rows.Count; r++)
                {
                    var row = block.rows[r];
                    var isHeader = r == 0;

                    using (new EditorGUILayout.HorizontalScope())
                    {
                        for (var c = 0; c < block.columnWeights.Length; c++)
                        {
                            var text = c < row.Length ? row[c] : "";
                            var width = available * block.columnWeights[c] / totalWeight;
                            RichLabel(text, cell, isHeader, GUILayout.Width(width));
                        }
                    }

                    if (r < block.rows.Count - 1) DrawLine(r == 0 ? 1.5f : 1f);
                }
            }
            GUILayout.Space(6f);
        }

        /// <summary>
        /// Markdown の1ブロック分の文字列をラベルとして描く。
        /// エディタ標準のフォントには日本語がなく、FontStyle.Bold や &lt;b&gt; では一部の文字が太くならないため、
        /// 太字は同じ文字を横に少しずらして重ねて描き、太く見せる
        /// </summary>
        /// <param name="markdown">Markdown の文字列</param>
        /// <param name="style">描画に使うスタイル</param>
        /// <param name="bold">true のときは全体を太字にする（見出し・表の見出し行）</param>
        void RichLabel(string markdown, GUIStyle style, bool bold, params GUILayoutOption[] options)
        {
            var (text, boldOnly) = ToRichText(markdown, style);
            var content = new GUIContent(text);
            var rect = GUILayoutUtility.GetRect(content, style, options);
            if (Event.current.type != EventType.Repaint) return;

            style.Draw(rect, content, false, false, false, false);

            // 全体が太字なら同じ文字列を、一部だけなら太字以外を透明にした文字列を重ねる。
            // 色のタグは文字の並びを変えないので、太字の部分にだけぴったり重なる
            var overlay = bold ? text : boldOnly;
            if (overlay == null) return;

            // Retina では 0.5 で1ピクセル、通常のディスプレイでは 1 で1ピクセルずれる
            rect.x += 1f / EditorGUIUtility.pixelsPerPoint;
            style.Draw(rect, new GUIContent(overlay), false, false, false, false);
        }

        void DrawRule()
        {
            GUILayout.Space(8f);
            DrawLine(2f);
            GUILayout.Space(8f);
        }

        void DrawLine(float thickness)
        {
            var rect = GUILayoutUtility.GetRect(1f, thickness, GUILayout.ExpandWidth(true));
            EditorGUI.DrawRect(rect, LineColor);
        }

        /// <summary>
        /// インラインの Markdown（太字・インラインコード・リンク）を Unity のリッチテキストに変換する。
        /// text は通常の描画用、boldOnly は太字の部分だけが見えて他は透明になる重ね描き用（太字がなければ null）
        /// </summary>
        (string text, string boldOnly) ToRichText(string markdown, GUIStyle style)
        {
            var visible = "#" + ColorUtility.ToHtmlStringRGBA(style.normal.textColor);
            var text = new StringBuilder();
            var boldOnly = new StringBuilder("<color=#00000000>");
            var isBold = false;
            var hasBold = false;

            // ` で区切ると、奇数番目がインラインコードになる
            var parts = markdown.Split('`');
            for (var p = 0; p < parts.Length; p++)
            {
                if (p % 2 == 1)
                {
                    var codeText = EscapeTags(parts[p]);
                    text.Append("<color=").Append(CodeColor).Append('>').Append(codeText).Append("</color>");
                    // 重ねる色は元の色と同じにする。違う色で重ねると色が混ざって薄く見える
                    AppendBoldOnly(boldOnly, codeText, isBold, CodeColor);
                    continue;
                }

                // ** はインラインコードをまたぐことがあるので、出てくるたびに太字の状態を切り替える
                var pieces = parts[p].Split(new[] { "**" }, StringSplitOptions.None);
                for (var i = 0; i < pieces.Length; i++)
                {
                    if (i > 0)
                    {
                        isBold = !isBold;
                        hasBold |= isBold;
                    }

                    var piece = EscapeTags(pieces[i]);
                    var linked = s_linkRegex.Replace(piece, "<color=" + LinkColor + ">$1</color>");
                    text.Append(linked);

                    // 太字の中のリンクは、内側の色の指定が優先されてリンクの色で重なる。
                    // 太字でない部分は透明のままにしたいので、色の指定を外す
                    AppendBoldOnly(boldOnly, isBold ? linked : s_linkRegex.Replace(piece, "$1"), isBold, visible);
                }
            }

            return (text.ToString(), hasBold ? boldOnly.Append("</color>").ToString() : null);
        }

        static void AppendBoldOnly(StringBuilder sb, string text, bool isBold, string color)
        {
            if (isBold)
            {
                sb.Append("<color=").Append(color).Append('>').Append(text).Append("</color>");
            }
            else
            {
                sb.Append(text);
            }
        }

        /// <summary>
        /// 本文中の &lt;自分の名前&gt; などが、リッチテキストのタグとして解釈されないようにする
        /// </summary>
        static string EscapeTags(string text) => s_tagRegex.Replace(text, m => "<​" + m.Value[1..]);

        void InitStyles()
        {
            if (body != null)
            {
                return;
            }

            // 太字は FontStyle.Bold を使わず、RichLabel で重ねて描いて太く見せる
            body = new GUIStyle(EditorStyles.label) { richText = true, wordWrap = true, fontSize = 13 };
            h1 = new GUIStyle(body) { fontSize = 24 };
            h2 = new GUIStyle(body) { fontSize = 18 };
            h3 = new GUIStyle(body) { fontSize = 15 };
            quote = new GUIStyle(body);
            cell = new GUIStyle(body) { fontSize = 12, padding = new RectOffset(4, 4, 3, 3) };

            if (!s_monoFontResolved)
            {
                s_monoFont = CreateMonoFont();
                s_monoFontResolved = true;
            }
            code = new GUIStyle(EditorStyles.label)
            {
                font = s_monoFont,
                fontSize = 12,
                richText = false,
                wordWrap = false,
                padding = new RectOffset(6, 6, 4, 4)
            };
        }

        /// <summary>
        /// コードブロック用の等幅フォントを作る。
        /// OS にないフォント名を渡すと警告が出るので、インストールされているものだけを候補から選ぶ。
        /// どれもなければ null を返し、エディタ標準のフォントで描く
        /// </summary>
        static Font CreateMonoFont()
        {
            var installed = new HashSet<string>(Font.GetOSInstalledFontNames());
            foreach (var name in s_monoFontCandidates)
            {
                if (installed.Contains(name))
                {
                    return Font.CreateDynamicFontFromOSFont(name, 12);
                }
            }
            return null;
        }
    }
}
