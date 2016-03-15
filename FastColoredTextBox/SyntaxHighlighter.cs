using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Text.RegularExpressions;
using System.Xml;

namespace FastColoredTextBoxNS
{
    public class SyntaxHighlighter : IDisposable
    {

        public string StaticClasses;
        protected static readonly Platform platformType = PlatformType.GetOperationSystemPlatform();

        public readonly Style BlackStyle = new TextStyle(Brushes.Black, null, FontStyle.Regular);

        public readonly Style BlueBoldStyle = new TextStyle(Brushes.Blue, null, FontStyle.Bold);

        public readonly Style BlueStyle = new TextStyle(new Pen(Color.FromArgb(86, 156, 214)).Brush, null,
            FontStyle.Regular);

        public readonly Style BoldStyle = new TextStyle(null, null, FontStyle.Bold | FontStyle.Underline);
        public readonly Style ClassStyle = new TextStyle(new Pen(Color.FromArgb(217, 72, 125)).Brush, null, FontStyle.Bold | FontStyle.Italic);

        public readonly Style PreCompileStyle = new TextStyle(new Pen(Color.FromArgb(100, 100, 100)).Brush, null, FontStyle.Regular);

        public readonly Style BrownStyle = new TextStyle(new Pen(Color.FromArgb(206, 145, 120)).Brush, null,
            FontStyle.Italic);

        protected readonly Dictionary<string, SyntaxDescriptor> descByXMLfileNames =
            new Dictionary<string, SyntaxDescriptor>();

        public readonly Style GrayStyle = new TextStyle(Brushes.Gray, null, FontStyle.Regular);

        public readonly Style GreenStyle = new TextStyle(new Pen(Color.FromArgb(96, 139, 78)).Brush, null,
            FontStyle.Italic);

        public readonly Style MagentaStyle = new TextStyle(new Pen(Color.FromArgb(181, 206, 168)).Brush, null,
            FontStyle.Regular);

        public readonly Style MaroonStyle = new TextStyle(new Pen(Color.FromArgb(206, 145, 120)).Brush, null,
            FontStyle.Regular);

        public readonly Style OperatorStyle = new TextStyle(new Pen(Color.FromArgb(217, 72, 125)).Brush, null,
            FontStyle.Bold | FontStyle.Italic);

        public readonly Style StaticClassStyle = new TextStyle(new Pen(Color.FromArgb(165, 194, 97)).Brush, null,
            FontStyle.Bold);

        public readonly Style PASMNumberStyle = new TextStyle(new Pen(Color.FromArgb(242, 180, 65)).Brush, null,
            FontStyle.Italic | FontStyle.Bold);

        public readonly Style PASMPreNumStyle = new TextStyle(new Pen(Color.FromArgb(255, 5, 5)).Brush, null,
            FontStyle.Italic | FontStyle.Bold);

        public readonly Style RedStyle = new TextStyle(new Pen(Color.FromArgb(217, 72, 125)).Brush, null,
            FontStyle.Regular);

        protected Regex PuffinAttributeRegex;

        protected Regex PuffinClassNameRegex;

        protected Regex PuffinOmitableNameRegex;

        protected Regex PuffinPreCompileSpaceNameRegex;

        protected Regex PuffinCommentRegex1;

        protected Regex PuffinCommentRegex2;

        protected Regex PuffinCommentRegex3;

        protected Regex PuffinKeywordRegex;

        protected Regex PuffinNumberRegex;

        protected Regex PuffinStringRegex;

        protected Regex PuffinPreCompileRegex;

        protected Regex PuffinImportLibraryRegex;

        protected Regex PASMCommentRegex1;

        protected Regex PASMCommentRegex2;

        protected Regex PASMCommentRegex3;

        protected Regex PASMFunctionsRegex;

        protected Regex PASMKeywordRegex;

        protected Regex PASMNumberRegex;

        protected Regex PASMStringRegex;

        protected Regex StaticClassRegex;

        protected Regex PASMPreNumRegex;

        public static RegexOptions RegexCompiledOption
        {
            get
            {
                var flag = platformType == Platform.X86;
                RegexOptions result;
                if (flag)
                {
                    result = RegexOptions.Compiled;
                }
                else
                {
                    result = RegexOptions.None;
                }
                return result;
            }
        }

        public Style StringStyle { get; set; }

        public Style CommentStyle { get; set; }

        public Style NumberStyle { get; set; }

        public Style AttributeStyle { get; set; }

        public Style ClassNameStyle { get; set; }

        public Style OmitableNameStyle { get; set; }

        public Style PreCompileSpaceNameStyle { get; set; }

        public Style KeywordStyle { get; set; }

        public Style CommentTagStyle { get; set; }

        public Style AttributeValueStyle { get; set; }

        public Style TagBracketStyle { get; set; }

        public Style TagNameStyle { get; set; }

        public Style VariableStyle { get; set; }

        public Style KeywordStyle2 { get; set; }

        public Style KeywordStyle3 { get; set; }

        public Style StatementsStyle { get; set; }

        public Style FunctionsStyle { get; set; }

        public Style TypesStyle { get; set; }

        public void Dispose()
        {
            foreach (var current in descByXMLfileNames.Values)
            {
                current.Dispose();
            }
        }

        public virtual void HighlightSyntax(Language language, Range range)
        {
            switch (language)
            {
				case Language.Puffin:
                    PuffinSyntaxHighlight(range);
                    break;
                case Language.PASM:
                    PASMSyntaxHighlight(range);
                    break;
            }
        }

        public virtual void HighlightSyntax(string XMLdescriptionFile, Range range)
        {
            SyntaxDescriptor syntaxDescriptor = null;
            var flag = !descByXMLfileNames.TryGetValue(XMLdescriptionFile, out syntaxDescriptor);
            if (flag)
            {
                var xmlDocument = new XmlDocument();
                var path = XMLdescriptionFile;
                var flag2 = !File.Exists(path);
                if (flag2)
                {
                    path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, Path.GetFileName(path));
                }
                xmlDocument.LoadXml(File.ReadAllText(path));
                descByXMLfileNames[XMLdescriptionFile] = syntaxDescriptor;
            }
            HighlightSyntax(syntaxDescriptor, range);
        }

        public virtual void AutoIndentNeeded(object sender, AutoIndentEventArgs args)
        {
            var tb = (sender as FastColoredTextBox);
            Language language = tb.Language;
            switch (language)
            {
			case Language.Puffin:
                    PuffinAutoIndentNeeded(sender, args);
                    break;
                case Language.PASM:
                    PASMAutoIndentNeeded(sender, args);
                    break;
            }
        }

        protected void PuffinAutoIndentNeeded(object sender, AutoIndentEventArgs args)
        {
            var flag = Regex.IsMatch(args.LineText, "^[^\"']*\\{.*\\}[^\"']*$");
            if (!flag)
            {
                var flag2 = Regex.IsMatch(args.LineText, "^[^\"']*\\{");
                if (flag2)
                {
                    args.ShiftNextLines = args.TabLength;
                }
                else
                {
                    var flag3 = Regex.IsMatch(args.LineText, "}[^\"']*$");
                    if (flag3)
                    {
                        args.Shift = -args.TabLength;
                        args.ShiftNextLines = -args.TabLength;
                    }
                    else
                    {
                        var flag4 = Regex.IsMatch(args.LineText, "^\\s*\\w+\\s*:\\s*($|//)") &&
                                    !Regex.IsMatch(args.LineText, "^\\s*default\\s*:");
                        if (flag4)
                        {
                            args.Shift = -args.TabLength;
                        }
                        else
                        {
                            var flag5 = Regex.IsMatch(args.LineText, "^\\s*(case|default)\\b.*:\\s*($|//)");
                            if (flag5)
                            {
                                args.Shift = -args.TabLength/2;
                            }
                            else
                            {
                                var flag6 = Regex.IsMatch(args.PrevLineText,
                                    "^\\s*(if|for|foreach|while|[\\}\\s]*else)\\b[^{]*$");
                                if (flag6)
                                {
                                    var flag7 = !Regex.IsMatch(args.PrevLineText, "(;\\s*$)|(;\\s*//)");
                                    if (flag7)
                                    {
                                        args.Shift = args.TabLength;
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        protected static FoldingDesc ParseFolding(XmlNode foldingNode)
        {
            var foldingDesc = new FoldingDesc();
            foldingDesc.startMarkerRegex = foldingNode.Attributes["start"].Value;
            foldingDesc.finishMarkerRegex = foldingNode.Attributes["finish"].Value;
            var xmlAttribute = foldingNode.Attributes["options"];
            var flag = xmlAttribute != null;
            if (flag)
            {
                foldingDesc.options = (RegexOptions) Enum.Parse(typeof (RegexOptions), xmlAttribute.Value);
            }
            return foldingDesc;
        }

        protected static RuleDesc ParseRule(XmlNode ruleNode, Dictionary<string, Style> styles)
        {
            var ruleDesc = new RuleDesc();
            ruleDesc.pattern = ruleNode.InnerText;
            var xmlAttribute = ruleNode.Attributes["style"];
            var xmlAttribute2 = ruleNode.Attributes["options"];
            var flag = xmlAttribute == null;
            if (flag)
            {
                throw new Exception("Rule must contain style name.");
            }
            var flag2 = !styles.ContainsKey(xmlAttribute.Value);
            if (flag2)
            {
                throw new Exception("Style '" + xmlAttribute.Value + "' is not found.");
            }
            ruleDesc.style = styles[xmlAttribute.Value];
            var flag3 = xmlAttribute2 != null;
            if (flag3)
            {
                ruleDesc.options = (RegexOptions) Enum.Parse(typeof (RegexOptions), xmlAttribute2.Value);
            }
            return ruleDesc;
        }

        protected static Style ParseStyle(XmlNode styleNode)
        {
            var xmlAttribute = styleNode.Attributes["type"];
            var xmlAttribute2 = styleNode.Attributes["color"];
            var xmlAttribute3 = styleNode.Attributes["backColor"];
            var xmlAttribute4 = styleNode.Attributes["fontStyle"];
            var xmlAttribute5 = styleNode.Attributes["name"];
            SolidBrush foreBrush = null;
            var flag = xmlAttribute2 != null;
            if (flag)
            {
                foreBrush = new SolidBrush(ParseColor(xmlAttribute2.Value));
            }
            SolidBrush backgroundBrush = null;
            var flag2 = xmlAttribute3 != null;
            if (flag2)
            {
                backgroundBrush = new SolidBrush(ParseColor(xmlAttribute3.Value));
            }
            var fontStyle = FontStyle.Regular;
            var flag3 = xmlAttribute4 != null;
            if (flag3)
            {
                fontStyle = (FontStyle) Enum.Parse(typeof (FontStyle), xmlAttribute4.Value);
            }
            return new TextStyle(foreBrush, backgroundBrush, fontStyle);
        }

        protected static Color ParseColor(string s)
        {
            var flag = s.StartsWith("#");
            Color result;
            if (flag)
            {
                var flag2 = s.Length <= 7;
                if (flag2)
                {
                    result = Color.FromArgb(255,
                        Color.FromArgb(int.Parse(s.Substring(1), NumberStyles.AllowHexSpecifier)));
                }
                else
                {
                    result = Color.FromArgb(int.Parse(s.Substring(1), NumberStyles.AllowHexSpecifier));
                }
            }
            else
            {
                result = Color.FromName(s);
            }
            return result;
        }

        public void HighlightSyntax(SyntaxDescriptor desc, Range range)
        {
            range.tb.ClearStylesBuffer();
            int num;
            for (var i = 0; i < desc.styles.Count; i = num + 1)
            {
                range.tb.Styles[i] = desc.styles[i];
                num = i;
            }
            var oldBrackets = RememberBrackets(range.tb);
            range.tb.LeftBracket = desc.leftBracket;
            range.tb.RightBracket = desc.rightBracket;
            range.tb.LeftBracket2 = desc.leftBracket2;
            range.tb.RightBracket2 = desc.rightBracket2;
            range.ClearStyle(desc.styles.ToArray());
            foreach (var current in desc.rules)
            {
                range.SetStyle(current.style, current.Regex);
            }
            range.ClearFoldingMarkers();
            foreach (var current2 in desc.foldings)
            {
                range.SetFoldingMarkers(current2.startMarkerRegex, current2.finishMarkerRegex, current2.options);
            }
            RestoreBrackets(range.tb, oldBrackets);
        }

        protected void RestoreBrackets(FastColoredTextBox tb, char[] oldBrackets)
        {
            tb.LeftBracket = oldBrackets[0];
            tb.RightBracket = oldBrackets[1];
            tb.LeftBracket2 = oldBrackets[2];
            tb.RightBracket2 = oldBrackets[3];
        }

        protected char[] RememberBrackets(FastColoredTextBox tb)
        {
            return new[]
            {
                tb.LeftBracket,
                tb.RightBracket,
                tb.LeftBracket2,
                tb.RightBracket2
            };
        }

        protected void InitPuffinRegex()
        {
            //PuffinStringRegex = new Regex( @"""""|@""""|''|@"".*?""|(?<!@)(?<range>"".*?[^\\]"")|'.*?[^\\]'", RegexCompiledOption);

            PuffinStringRegex =
                new Regex(
                    @"
                            # Character definitions:
                            '
                            (?> # disable backtracking
                              (?:
                                \\[^\r\n]|    # escaped meta char
                                [^'\r\n]      # any character except '
                              )*
                            )
                            '?
                            |
                            # Normal string & verbatim strings definitions:
                            (?<verbatimIdentifier>@)?         # this group matches if it is an verbatim string
                            ""
                            (?> # disable backtracking
                              (?:
                                # match and consume an escaped character including escaped double quote ("") char
                                (?(verbatimIdentifier)        # if it is a verbatim string ...
                                  """"|                         #   then: only match an escaped double quote ("") char
                                  \\.                         #   else: match an escaped sequence
                                )
                                | # OR
            
                                # match any char except double quote char ("")
                                [^""]
                              )*
                            )
                            ""
                        ",
                    RegexOptions.ExplicitCapture | RegexOptions.Singleline | RegexOptions.IgnorePatternWhitespace |
                    RegexCompiledOption
                    ); //thanks to rittergig for this regex

            PuffinCommentRegex1 = new Regex(@"//.*$", RegexOptions.Multiline | RegexCompiledOption);
            PuffinCommentRegex2 = new Regex(@"(/\*.*?\*/)|(/\*.*)", RegexOptions.Singleline | RegexCompiledOption);
            PuffinCommentRegex3 = new Regex(@"(/\*.*?\*/)|(.*\*/)",
                                            RegexOptions.Singleline | RegexOptions.RightToLeft | RegexCompiledOption);
            PuffinNumberRegex = new Regex(@"\b\d+[\.]?\d*([eE]\-?\d+)?[lLdDfF]?\b|\b0x[a-fA-F\d]+\b",
                                          RegexCompiledOption);
            PuffinAttributeRegex = new Regex(@"^\s*(?<range>\[.+?\])\s*$", RegexOptions.Multiline | RegexCompiledOption);
            PuffinClassNameRegex = new Regex(@"\b(class|struct|enum|interface)\s+(?<range>\w+?)\b", RegexCompiledOption);
            PuffinKeywordRegex =
                new Regex(
                    @"\b(public|private|protected|static|int|bool|long|short|byte|char|float|double|dataset|null|nullptr|void|if|for|while|do|return|switch|case|__EOF|__pasm|enum|class|struct|extern|interface|abstract|sealed|else|break|contunue|uint|ubyte|ulong|ushort|extends)\b",
                    RegexCompiledOption);
            PuffinPreCompileRegex = new Regex("#(import|region|endregion)?", RegexCompiledOption);
            PuffinImportLibraryRegex = new Regex("(?<=<)(.*?)(?=>)");
        }


        public void InitStyleSchema(Language lang)
        {
            switch (lang)
            {
                case Language.PASM:
                    StringStyle = BrownStyle;
                    CommentStyle = GreenStyle;
                    NumberStyle = MagentaStyle;
                    AttributeStyle = GreenStyle;
                    ClassNameStyle = BoldStyle;
                    KeywordStyle = BlueStyle;
                    FunctionsStyle = OperatorStyle;
                    CommentTagStyle = GrayStyle;
                    break;
			case Language.Puffin:
                    StringStyle = BrownStyle;
                    CommentStyle = GreenStyle;
                    NumberStyle = MagentaStyle;
                    AttributeStyle = GreenStyle;
                    ClassNameStyle = BoldStyle;
                    OmitableNameStyle = ClassStyle;
                    PreCompileSpaceNameStyle = PreCompileStyle;
                    KeywordStyle = BlueStyle;
                    CommentTagStyle = GrayStyle;
                    break;
            }
        }

        public virtual void PuffinSyntaxHighlight(Range range)
        {
            range.tb.CommentPrefix = "//";
            range.tb.LeftBracket = '(';
            range.tb.RightBracket = ')';
            range.tb.LeftBracket2 = '{';
            range.tb.RightBracket2 = '}';
            range.tb.BracketsHighlightStrategy = BracketsHighlightStrategy.Strategy2;

            range.tb.AutoIndentCharsPatterns
                = @"
^\s*[\w\.]+(\s\w+)?\s*(?<range>=)\s*(?<range>[^;]+);
^\s*(case|default)\s*[^:]*(?<range>:)\s*(?<range>[^;]+);
";
            //clear style of changed range
            range.ClearStyle(StringStyle, CommentStyle, NumberStyle, AttributeStyle, ClassNameStyle, KeywordStyle);
            //
            if (PuffinStringRegex == null)
                InitPuffinRegex();
            //string highlighting
            range.SetStyle(StringStyle, PuffinStringRegex);
            //comment highlighting
            range.SetStyle(CommentStyle, PuffinCommentRegex1);
            range.SetStyle(CommentStyle, PuffinCommentRegex2);
            range.SetStyle(CommentStyle, PuffinCommentRegex3);
            //number highlighting
            range.SetStyle(NumberStyle, PuffinNumberRegex);
            //attribute highlighting
            range.SetStyle(AttributeStyle, PuffinAttributeRegex);
            //class name highlighting
            range.SetStyle(ClassNameStyle, PuffinClassNameRegex);
            //keyword highlighting
            range.SetStyle(KeywordStyle, PuffinKeywordRegex);

            range.SetStyle(StaticClassStyle, PuffinImportLibraryRegex);

            //Pre Compile highlighting
            range.SetStyle(PreCompileStyle, PuffinPreCompileRegex);

            //find document comments
            foreach (Range r in range.GetRanges(@"^\s*///.*$", RegexOptions.Multiline))
            {
                //remove C# highlighting from this fragment
                r.ClearStyle(StyleIndex.All);
                r.SetStyle(CommentStyle);
                
                //prefix '///'
                foreach (Range rr in r.GetRanges(@"^\s*///", RegexOptions.Multiline))
                {
                    rr.ClearStyle(StyleIndex.All);
                    rr.SetStyle(CommentTagStyle);
                }
            }

            //clear folding markers
            range.ClearFoldingMarkers();
            //set folding markers
            range.SetFoldingMarkers("{", "}"); //allow to collapse brackets block
            range.SetFoldingMarkers(@"#region\b", @"#endregion\b"); //allow to collapse #region blocks
            range.SetFoldingMarkers(@"/\*", @"\*/"); //allow to collapse comment block
        }

        public virtual void PASMSyntaxHighlight(Range range)
        {
            range.tb.CommentPrefix = "#";
            range.tb.BracketsHighlightStrategy = BracketsHighlightStrategy.Strategy2;
            range.tb.AutoIndentCharsPatterns = "\r\n^\\s*[\\w\\.]+(\\s\\w+)?\\s*(?<range>=)\\s*(?<range>.+)\r\n";
            range.ClearStyle(StringStyle, CommentStyle, NumberStyle, KeywordStyle, FunctionsStyle);
            var flag = PASMStringRegex == null;
            if (flag)
            {
                InitPASMRegex();
            }
            range.SetStyle(StringStyle, PASMStringRegex);
            range.SetStyle(CommentStyle, PASMCommentRegex1);
            range.SetStyle(CommentStyle, PASMCommentRegex2);
            range.SetStyle(CommentStyle, PASMCommentRegex3);
            range.SetStyle(PASMNumberStyle, PASMNumberRegex);
            range.SetStyle(PASMNumberStyle, PASMPreNumRegex);
            range.SetStyle(KeywordStyle, PASMKeywordRegex);
            range.SetStyle(FunctionsStyle, PASMFunctionsRegex);
            range.SetStyle(StaticClassStyle, StaticClassRegex);
            range.ClearFoldingMarkers();
            range.SetFoldingMarkers("#\\[\\[", "\\]\\]");
        }

        public void InitPASMRegex()
        {
            PASMStringRegex = new Regex("\"\"|''|\".*?[^\\\\]\"|'.*?[^\\\\]'", RegexCompiledOption);
            PASMCommentRegex1 = new Regex("#.*$", RegexOptions.Multiline | RegexCompiledOption);
            PASMCommentRegex2 = new Regex("(#\\[\\[.*?\\]\\])|(--\\[\\[.*)",
                RegexOptions.Singleline | RegexCompiledOption);
            PASMCommentRegex3 = new Regex("(#\\[\\[.*?\\]\\])|(.*\\]\\])",
                RegexOptions.Singleline | RegexOptions.RightToLeft | RegexCompiledOption);
            PASMNumberRegex = new Regex("\\b\\d+[\\.]?\\d*([eE]\\-?\\d+)?[lLdDfF]?\\b|\\b0x[a-fA-F\\d]+\\b",
                RegexCompiledOption);
            PASMPreNumRegex = new Regex(":\\d",
                RegexCompiledOption);
            PASMKeywordRegex = new Regex("\\b(set|mov|pt|calib|re|call|if|im|malloc_c|malloc_p|malloc_d|free)\\b", RegexCompiledOption);
            PASMFunctionsRegex = new Regex($"\\b(BYTE|SINT16|SINT32|SINT64|INT16|INT32|INT64|QMATH|VOR|VOP|VORL|PAR|PARC|PTR|FLOAT|DOUBLE|MATHF)\\b", RegexCompiledOption);
            StaticClassRegex = new Regex($"\\b({StaticClasses})\\b", RegexCompiledOption);
        }

        protected void PASMAutoIndentNeeded(object sender, AutoIndentEventArgs args)
        {
        }
    }
}