using System;
using System.Collections.Generic;
using System.IO;
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
        private enum BlockType { Heading, Paragraph, List, Code, Quote, Table, Rule }

        private class ListItem
        {
            public int Indent;
            public string Marker;
            public string Text;
        }

        private class Block
        {
            public BlockType Type;
            public int Level;
            public string Text;
            public MessageType Alert = MessageType.None;
            public readonly List<ListItem> Items = new List<ListItem>();
            public readonly List<string[]> Rows = new List<string[]>();
            public float[] ColumnWeights;
            public Vector2 Scroll;
        }

        private static readonly Regex HeadingRegex = new Regex(@"^(#{1,6})\s+(.*)$");
        private static readonly Regex ListRegex = new Regex(@"^(\s*)([-*]|\d+\.)\s+(.*)$");
        private static readonly Regex TableSeparatorRegex = new Regex(@"^[\s|:\-]+$");
        private static readonly Regex AlertRegex = new Regex(@"^\[!(NOTE|TIP|IMPORTANT|WARNING|CAUTION)\]\s*$");
        private static readonly Regex BoldRegex = new Regex(@"\*\*(.+?)\*\*");
        private static readonly Regex LinkRegex = new Regex(@"\[([^\]]+)\]\(([^)]+)\)");
        private static readonly Regex TagRegex = new Regex(@"<[^>]+>");

        private List<Block> _blocks;
        private DateTime _loadedWriteTime;
        private string _error;

        private static Font _monoFont;
        private GUIStyle _h1, _h2, _h3, _body, _cell, _headerCell, _code, _quote;

        private string CodeColor => EditorGUIUtility.isProSkin ? "#E5A06A" : "#A3410E";
        private string LinkColor => EditorGUIUtility.isProSkin ? "#7FB2F0" : "#1F5FAD";
        private Color LineColor => EditorGUIUtility.isProSkin ? new Color(1f, 1f, 1f, 0.15f) : new Color(0f, 0f, 0f, 0.15f);

        protected override void OnHeaderGUI()
        {
            using (new EditorGUILayout.HorizontalScope(EditorStyles.toolbar))
            {
                GUILayout.Label("README.md", EditorStyles.boldLabel);
                GUILayout.FlexibleSpace();
                if (GUILayout.Button("再読み込み", EditorStyles.toolbarButton))
                {
                    _blocks = null;
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

            if (_error != null)
            {
                EditorGUILayout.HelpBox(_error, MessageType.Error);
                return;
            }
            if (_blocks == null) return;

            foreach (Block block in _blocks)
            {
                DrawBlock(block);
            }
            GUILayout.Space(20f);
        }

        // ---------------------------------------------------------------
        // 読み込み
        // ---------------------------------------------------------------

        private void ReloadIfChanged()
        {
            string path = ProjectReadme.MarkdownPath;
            if (!File.Exists(path))
            {
                _blocks = null;
                _error = "README.md が見つかりません。\n" + path;
                return;
            }

            DateTime writeTime = File.GetLastWriteTimeUtc(path);
            if (_blocks != null && writeTime == _loadedWriteTime) return;

            _loadedWriteTime = writeTime;
            _error = null;
            _blocks = Parse(File.ReadAllLines(path, Encoding.UTF8));
        }

        private static List<Block> Parse(string[] rawLines)
        {
            var blocks = new List<Block>();
            string[] lines = Array.ConvertAll(rawLines, l => l.TrimEnd('\r'));
            int i = 0;

            while (i < lines.Length)
            {
                string line = lines[i];
                string trimmed = line.Trim();

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
                    blocks.Add(new Block { Type = BlockType.Code, Text = sb.ToString() });
                    continue;
                }

                // 見出し
                Match heading = HeadingRegex.Match(line);
                if (heading.Success)
                {
                    blocks.Add(new Block
                    {
                        Type = BlockType.Heading,
                        Level = heading.Groups[1].Value.Length,
                        Text = heading.Groups[2].Value
                    });
                    i++;
                    continue;
                }

                // 区切り線
                if (IsRule(trimmed))
                {
                    blocks.Add(new Block { Type = BlockType.Rule });
                    i++;
                    continue;
                }

                // 引用
                if (trimmed.StartsWith(">"))
                {
                    var block = new Block { Type = BlockType.Quote };
                    var quoteLines = new List<string>();
                    while (i < lines.Length && lines[i].TrimStart().StartsWith(">"))
                    {
                        string content = lines[i].TrimStart().Substring(1);
                        if (content.StartsWith(" ")) content = content.Substring(1);
                        quoteLines.Add(content);
                        i++;
                    }

                    if (quoteLines.Count > 0)
                    {
                        Match alert = AlertRegex.Match(quoteLines[0]);
                        if (alert.Success)
                        {
                            block.Alert = alert.Groups[1].Value switch
                            {
                                "WARNING" => MessageType.Warning,
                                "CAUTION" => MessageType.Error,
                                _ => MessageType.Info
                            };
                            quoteLines.RemoveAt(0);
                        }
                    }
                    block.Text = string.Join("\n", quoteLines);
                    blocks.Add(block);
                    continue;
                }

                // 表
                if (trimmed.StartsWith("|"))
                {
                    var block = new Block { Type = BlockType.Table };
                    while (i < lines.Length && lines[i].Trim().StartsWith("|"))
                    {
                        string row = lines[i].Trim();
                        i++;
                        if (TableSeparatorRegex.IsMatch(row)) continue;

                        row = row.Trim('|');
                        string[] cells = row.Split('|');
                        for (int c = 0; c < cells.Length; c++) cells[c] = cells[c].Trim();
                        block.Rows.Add(cells);
                    }
                    block.ColumnWeights = CalcColumnWeights(block.Rows);
                    blocks.Add(block);
                    continue;
                }

                // 箇条書き
                if (ListRegex.IsMatch(line))
                {
                    var block = new Block { Type = BlockType.List };
                    while (i < lines.Length)
                    {
                        string current = lines[i];
                        Match item = ListRegex.Match(current);
                        if (item.Success)
                        {
                            string marker = item.Groups[2].Value;
                            string text = item.Groups[3].Value;
                            if (text.StartsWith("[ ] ")) { marker = "☐"; text = text.Substring(4); }
                            else if (text.StartsWith("[x] ") || text.StartsWith("[X] ")) { marker = "☑"; text = text.Substring(4); }
                            else if (marker == "-" || marker == "*") marker = "•";

                            block.Items.Add(new ListItem
                            {
                                Indent = item.Groups[1].Value.Length / 2,
                                Marker = marker,
                                Text = text
                            });
                            i++;
                            continue;
                        }

                        // インデントされた続きの行は、直前の項目に含める
                        if (current.StartsWith(" ") && current.Trim().Length > 0 && block.Items.Count > 0)
                        {
                            block.Items[block.Items.Count - 1].Text += "\n" + current.Trim();
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
                        string current = lines[i];
                        string t = current.Trim();
                        if (t.Length == 0 || t.StartsWith("```") || t.StartsWith(">") || t.StartsWith("|")
                            || IsRule(t) || HeadingRegex.IsMatch(current) || ListRegex.IsMatch(current))
                        {
                            break;
                        }
                        if (sb.Length > 0) sb.Append('\n');
                        sb.Append(t);
                        i++;
                    }
                    blocks.Add(new Block { Type = BlockType.Paragraph, Text = sb.ToString() });
                }
            }

            return blocks;
        }

        private static bool IsRule(string trimmed) => trimmed == "---" || trimmed == "***" || trimmed == "___";

        private static float[] CalcColumnWeights(List<string[]> rows)
        {
            int columns = 0;
            foreach (string[] row in rows) columns = Mathf.Max(columns, row.Length);

            var weights = new float[columns];
            foreach (string[] row in rows)
            {
                for (int c = 0; c < row.Length; c++)
                {
                    // 全角文字は半角の約2倍の幅として数える
                    float length = 0f;
                    foreach (char ch in row[c].Replace("`", "").Replace("**", ""))
                    {
                        length += ch < 0x80 ? 1f : 2f;
                    }
                    weights[c] = Mathf.Max(weights[c], Mathf.Clamp(length, 6f, 50f));
                }
            }
            return weights;
        }

        // ---------------------------------------------------------------
        // 描画
        // ---------------------------------------------------------------

        private void DrawBlock(Block block)
        {
            switch (block.Type)
            {
                case BlockType.Heading: DrawHeading(block); break;
                case BlockType.Paragraph: DrawParagraph(block); break;
                case BlockType.List: DrawList(block); break;
                case BlockType.Code: DrawCode(block); break;
                case BlockType.Quote: DrawQuote(block); break;
                case BlockType.Table: DrawTable(block); break;
                case BlockType.Rule: DrawRule(); break;
            }
        }

        private void DrawHeading(Block block)
        {
            GUIStyle style = block.Level switch
            {
                1 => _h1,
                2 => _h2,
                _ => _h3
            };
            GUILayout.Space(block.Level <= 2 ? 12f : 8f);
            GUILayout.Label(Inline(block.Text), style);
            if (block.Level <= 2) DrawLine(1f);
            GUILayout.Space(4f);
        }

        private void DrawParagraph(Block block)
        {
            GUILayout.Label(Inline(block.Text), _body);
            GUILayout.Space(6f);
        }

        private void DrawList(Block block)
        {
            foreach (ListItem item in block.Items)
            {
                using (new EditorGUILayout.HorizontalScope())
                {
                    GUILayout.Space(8f + item.Indent * 16f);
                    GUILayout.Label(item.Marker, _body, GUILayout.Width(item.Marker.Length > 1 ? 22f : 14f));
                    GUILayout.Label(Inline(item.Text), _body);
                }
            }
            GUILayout.Space(6f);
        }

        private void DrawCode(Block block)
        {
            var content = new GUIContent(block.Text);
            Vector2 size = _code.CalcSize(content);

            using (new EditorGUILayout.VerticalScope(EditorStyles.helpBox))
            {
                // 横に長い行は折り返さず、横スクロールで見せる
                block.Scroll = EditorGUILayout.BeginScrollView(
                    block.Scroll, false, false,
                    GUI.skin.horizontalScrollbar, GUIStyle.none, GUIStyle.none,
                    GUILayout.Height(size.y + 16f));
                GUILayout.Label(content, _code, GUILayout.Width(size.x), GUILayout.Height(size.y));
                EditorGUILayout.EndScrollView();
            }
            GUILayout.Space(6f);
        }

        private void DrawQuote(Block block)
        {
            using (new EditorGUILayout.HorizontalScope(EditorStyles.helpBox))
            {
                if (block.Alert != MessageType.None)
                {
                    string icon = block.Alert switch
                    {
                        MessageType.Warning => "console.warnicon",
                        MessageType.Error => "console.erroricon",
                        _ => "console.infoicon"
                    };
                    GUILayout.Label(EditorGUIUtility.IconContent(icon), GUILayout.Width(32f), GUILayout.Height(32f));
                }
                GUILayout.Label(Inline(block.Text), _quote);
            }
            GUILayout.Space(6f);
        }

        private void DrawTable(Block block)
        {
            float totalWeight = 0f;
            foreach (float w in block.ColumnWeights) totalWeight += w;

            // インスペクタの左右の余白とスクロールバーの分を引く
            float available = EditorGUIUtility.currentViewWidth - 48f;

            using (new EditorGUILayout.VerticalScope(EditorStyles.helpBox))
            {
                for (int r = 0; r < block.Rows.Count; r++)
                {
                    string[] row = block.Rows[r];
                    GUIStyle style = r == 0 ? _headerCell : _cell;

                    using (new EditorGUILayout.HorizontalScope())
                    {
                        for (int c = 0; c < block.ColumnWeights.Length; c++)
                        {
                            string text = c < row.Length ? row[c] : "";
                            float width = available * block.ColumnWeights[c] / totalWeight;
                            GUILayout.Label(Inline(text), style, GUILayout.Width(width));
                        }
                    }

                    if (r < block.Rows.Count - 1) DrawLine(r == 0 ? 1.5f : 1f);
                }
            }
            GUILayout.Space(6f);
        }

        private void DrawRule()
        {
            GUILayout.Space(8f);
            DrawLine(2f);
            GUILayout.Space(8f);
        }

        private void DrawLine(float thickness)
        {
            Rect rect = GUILayoutUtility.GetRect(1f, thickness, GUILayout.ExpandWidth(true));
            EditorGUI.DrawRect(rect, LineColor);
        }

        /// <summary>インラインの Markdown（太字・インラインコード・リンク）を Unity のリッチテキストに変換する</summary>
        private string Inline(string text)
        {
            string[] parts = text.Split('`');
            var sb = new StringBuilder();
            for (int p = 0; p < parts.Length; p++)
            {
                string part = EscapeTags(parts[p]);
                if (p % 2 == 1)
                {
                    sb.Append("<color=").Append(CodeColor).Append('>').Append(part).Append("</color>");
                }
                else
                {
                    part = BoldRegex.Replace(part, "<b>$1</b>");
                    part = LinkRegex.Replace(part, "<color=" + LinkColor + ">$1</color>");
                    sb.Append(part);
                }
            }
            return sb.ToString();
        }

        /// <summary>本文中の &lt;自分の名前&gt; などが、リッチテキストのタグとして解釈されないようにする</summary>
        private static string EscapeTags(string text) =>
            TagRegex.Replace(text, m => "<​" + m.Value.Substring(1));

        private void InitStyles()
        {
            if (_body != null) return;

            _body = new GUIStyle(EditorStyles.label) { richText = true, wordWrap = true, fontSize = 13 };
            _h1 = new GUIStyle(_body) { fontSize = 24, fontStyle = FontStyle.Bold };
            _h2 = new GUIStyle(_body) { fontSize = 18, fontStyle = FontStyle.Bold };
            _h3 = new GUIStyle(_body) { fontSize = 15, fontStyle = FontStyle.Bold };
            _quote = new GUIStyle(_body);
            _cell = new GUIStyle(_body) { fontSize = 12, padding = new RectOffset(4, 4, 3, 3) };
            _headerCell = new GUIStyle(_cell) { fontStyle = FontStyle.Bold };

            if (_monoFont == null)
            {
                _monoFont = Font.CreateDynamicFontFromOSFont(
                    new[] { "Menlo", "Consolas", "Osaka-Mono", "MS Gothic", "Courier New" }, 12);
            }
            _code = new GUIStyle(EditorStyles.label)
            {
                font = _monoFont,
                fontSize = 12,
                richText = false,
                wordWrap = false,
                padding = new RectOffset(6, 6, 4, 4)
            };
        }
    }
}
