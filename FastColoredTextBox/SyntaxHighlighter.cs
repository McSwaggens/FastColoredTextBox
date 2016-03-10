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

        protected Regex BASICFunctionsRegex;

        protected Regex BASICKeywordRegex;

        protected Regex BASICStringRegex;

        protected Regex CSharpAttributeRegex;

        protected Regex CSharpClassNameRegex;

        protected Regex CSharpOmitableNameRegex;

        protected Regex CSharpPreCompileSpaceNameRegex;

        protected Regex CSharpCommentRegex1;

        protected Regex CSharpCommentRegex2;

        protected Regex CSharpCommentRegex3;

        protected Regex CSharpKeywordRegex;

        protected Regex CSharpNumberRegex;

        protected Regex CSharpStringRegex;

        protected Regex JScriptCommentRegex1;

        protected Regex JScriptCommentRegex2;

        protected Regex JScriptCommentRegex3;

        protected Regex JScriptKeywordRegex;

        protected Regex JScriptNumberRegex;

        protected Regex JScriptStringRegex;

        protected Regex LuaCommentRegex1;

        protected Regex LuaCommentRegex2;

        protected Regex LuaCommentRegex3;

        protected Regex LuaFunctionsRegex;

        protected Regex LuaKeywordRegex;

        protected Regex LuaNumberRegex;

        protected Regex LuaStringRegex;

        protected Regex PASMFunctionsRegex;

        protected Regex PASMStaticClassRegex;

        protected Regex PASMKeywordRegex;

        protected Regex PASMStringRegex;

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

        public Style HtmlEntityStyle { get; set; }

        public Style XmlAttributeStyle { get; set; }

        public Style XmlAttributeValueStyle { get; set; }

        public Style XmlTagBracketStyle { get; set; }

        public Style XmlTagNameStyle { get; set; }

        public Style XmlEntityStyle { get; set; }

        public Style XmlCDataStyle { get; set; }

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
				case Language.SnapScript:
                    SnapScriptSyntaxHighlight(range);
                    break;
                case Language.PASM:
                    PASMSyntaxHighlight(range);
                    break;
                case Language.JS:
                    JScriptSyntaxHighlight(range);
                    break;
                case Language.Lua:
                    LuaSyntaxHighlight(range);
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
                syntaxDescriptor = ParseXmlDescription(xmlDocument);
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
			case Language.SnapScript:
                    CSharpAutoIndentNeeded(sender, args);
                    break;
                case Language.PASM:
                    PASMAutoIndentNeeded(sender, args);
                    break;
                case Language.PHP:
                    PHPAutoIndentNeeded(sender, args);
                    break;
                case Language.JS:
                    CSharpAutoIndentNeeded(sender, args);
                    break;
                case Language.Lua:
                    LuaAutoIndentNeeded(sender, args);
                    break;
            }
        }

        protected void PHPAutoIndentNeeded(object sender, AutoIndentEventArgs args)
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
                        var flag4 = Regex.IsMatch(args.PrevLineText,
                            "^\\s*(if|for|foreach|while|[\\}\\s]*else)\\b[^{]*$");
                        if (flag4)
                        {
                            var flag5 = !Regex.IsMatch(args.PrevLineText, "(;\\s*$)|(;\\s*//)");
                            if (flag5)
                            {
                                args.Shift = args.TabLength;
                            }
                        }
                    }
                }
            }
        }

        protected void SQLAutoIndentNeeded(object sender, AutoIndentEventArgs args)
        {
            var fastColoredTextBox = sender as FastColoredTextBox;
            fastColoredTextBox.CalcAutoIndentShiftByCodeFolding(sender, args);
        }

        protected void HTMLAutoIndentNeeded(object sender, AutoIndentEventArgs args)
        {
            var fastColoredTextBox = sender as FastColoredTextBox;
            fastColoredTextBox.CalcAutoIndentShiftByCodeFolding(sender, args);
        }

        protected void XMLAutoIndentNeeded(object sender, AutoIndentEventArgs args)
        {
            var fastColoredTextBox = sender as FastColoredTextBox;
            fastColoredTextBox.CalcAutoIndentShiftByCodeFolding(sender, args);
        }

        protected void VBAutoIndentNeeded(object sender, AutoIndentEventArgs args)
        {
        }

        protected void CSharpAutoIndentNeeded(object sender, AutoIndentEventArgs args)
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

        public static SyntaxDescriptor ParseXmlDescription(XmlDocument doc)
        {
            var syntaxDescriptor = new SyntaxDescriptor();
            var xmlNode = doc.SelectSingleNode("doc/brackets");
            var flag = xmlNode != null;
            if (flag)
            {
                var flag2 = xmlNode.Attributes["left"] == null || xmlNode.Attributes["right"] == null ||
                            xmlNode.Attributes["left"].Value == "" || xmlNode.Attributes["right"].Value == "";
                if (flag2)
                {
                    syntaxDescriptor.leftBracket = '\0';
                    syntaxDescriptor.rightBracket = '\0';
                }
                else
                {
                    syntaxDescriptor.leftBracket = xmlNode.Attributes["left"].Value[0];
                    syntaxDescriptor.rightBracket = xmlNode.Attributes["right"].Value[0];
                }
                var flag3 = xmlNode.Attributes["left2"] == null || xmlNode.Attributes["right2"] == null ||
                            xmlNode.Attributes["left2"].Value == "" || xmlNode.Attributes["right2"].Value == "";
                if (flag3)
                {
                    syntaxDescriptor.leftBracket2 = '\0';
                    syntaxDescriptor.rightBracket2 = '\0';
                }
                else
                {
                    syntaxDescriptor.leftBracket2 = xmlNode.Attributes["left2"].Value[0];
                    syntaxDescriptor.rightBracket2 = xmlNode.Attributes["right2"].Value[0];
                }
                var flag4 = xmlNode.Attributes["strategy"] == null || xmlNode.Attributes["strategy"].Value == "";
                if (flag4)
                {
                    syntaxDescriptor.bracketsHighlightStrategy = BracketsHighlightStrategy.Strategy2;
                }
                else
                {
                    syntaxDescriptor.bracketsHighlightStrategy =
                        (BracketsHighlightStrategy)
                            Enum.Parse(typeof (BracketsHighlightStrategy), xmlNode.Attributes["strategy"].Value);
                }
            }
            var dictionary = new Dictionary<string, Style>();
            foreach (XmlNode xmlNode2 in doc.SelectNodes("doc/style"))
            {
                var style = ParseStyle(xmlNode2);
                dictionary[xmlNode2.Attributes["name"].Value] = style;
                syntaxDescriptor.styles.Add(style);
            }
            foreach (XmlNode ruleNode in doc.SelectNodes("doc/rule"))
            {
                syntaxDescriptor.rules.Add(ParseRule(ruleNode, dictionary));
            }
            foreach (XmlNode foldingNode in doc.SelectNodes("doc/folding"))
            {
                syntaxDescriptor.foldings.Add(ParseFolding(foldingNode));
            }
            return syntaxDescriptor;
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

        protected void InitCShaprRegex()
        {
            CSharpStringRegex =
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
                );
            CSharpCommentRegex1 = new Regex("//.*$", RegexOptions.Multiline | RegexCompiledOption);
            CSharpCommentRegex2 = new Regex("(/\\*.*?\\*/)|(/\\*.*)", RegexOptions.Singleline | RegexCompiledOption);
            CSharpCommentRegex3 = new Regex("(/\\*.*?\\*/)|(.*\\*/)",
                RegexOptions.Singleline | RegexOptions.RightToLeft | RegexCompiledOption);
            CSharpNumberRegex = new Regex("\\b\\d+[\\.]?\\d*([eE]\\-?\\d+)?[lLdDfF]?\\b|\\b0x[a-fA-F\\d]+\\b",
                RegexCompiledOption);
            CSharpAttributeRegex = new Regex("^\\s*(?<range>\\[.+?\\])\\s*$",
                RegexOptions.Multiline | RegexCompiledOption);
            CSharpClassNameRegex = new Regex("\\b(class|struct|omit)\\s+(?<range>\\w+?)\\b",
                RegexCompiledOption);
            CSharpOmitableNameRegex = new Regex("\\b(omitable)\\b",
                RegexCompiledOption);
            CSharpPreCompileSpaceNameRegex = new Regex("#import|#declare|#enforce",
                RegexCompiledOption);
            CSharpKeywordRegex =
                new Regex(
                    "\\b(self|operator|char|goto|new|for|if|while|void|null|private|public|static|pasm|int16|int32|int64|uint16|uint32|uint64|byte|sbyte|assign|true|false)\\b",
                    RegexCompiledOption);
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
			case Language.SnapScript:
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
                case Language.PHP:
                    StringStyle = RedStyle;
                    CommentStyle = GreenStyle;
                    NumberStyle = RedStyle;
                    VariableStyle = MaroonStyle;
                    KeywordStyle = MagentaStyle;
                    KeywordStyle2 = BlueStyle;
                    KeywordStyle3 = GrayStyle;
                    break;
                case Language.JS:
                    StringStyle = BrownStyle;
                    CommentStyle = GreenStyle;
                    NumberStyle = MagentaStyle;
                    KeywordStyle = BlueStyle;
                    break;
                case Language.Lua:
                    StringStyle = BrownStyle;
                    CommentStyle = GreenStyle;
                    NumberStyle = MagentaStyle;
                    KeywordStyle = BlueBoldStyle;
                    FunctionsStyle = MaroonStyle;
                    break;
            }
        }

        public virtual void SnapScriptSyntaxHighlight(Range range)
        {
            range.tb.CommentPrefix = "//";
            range.tb.LeftBracket = '(';
            range.tb.RightBracket = ')';
            range.tb.LeftBracket2 = '{';
            range.tb.RightBracket2 = '}';
            range.tb.BracketsHighlightStrategy = BracketsHighlightStrategy.Strategy2;
            range.tb.AutoIndentCharsPatterns =
                "\r\n^\\s*[\\w\\.]+(\\s\\w+)?\\s*(?<range>=)\\s*(?<range>[^;]+);\r\n^\\s*(case|default)\\s*[^:]*(?<range>:)\\s*(?<range>[^;]+);\r\n";
            range.ClearStyle(StringStyle, CommentStyle, NumberStyle, AttributeStyle, ClassNameStyle, KeywordStyle, OmitableNameStyle);
            var flag = CSharpStringRegex == null;
            if (flag)
            {
                InitCShaprRegex();
            }
            range.SetStyle(StringStyle, CSharpStringRegex);
            range.SetStyle(CommentStyle, CSharpCommentRegex1);
            range.SetStyle(CommentStyle, CSharpCommentRegex2);
            range.SetStyle(CommentStyle, CSharpCommentRegex3);
            range.SetStyle(NumberStyle, CSharpNumberRegex);
            range.SetStyle(AttributeStyle, CSharpAttributeRegex);
            range.SetStyle(ClassNameStyle, CSharpClassNameRegex);
            range.SetStyle(OmitableNameStyle, CSharpOmitableNameRegex);
            range.SetStyle(PreCompileSpaceNameStyle, CSharpPreCompileSpaceNameRegex);
            range.SetStyle(KeywordStyle, CSharpKeywordRegex);
            foreach (var current in range.GetRanges("^\\s*///.*$", RegexOptions.Multiline))
            {
                current.ClearStyle(StyleIndex.All);
                
                current.SetStyle(CommentStyle);
                foreach (var current3 in current.GetRanges("^\\s*///", RegexOptions.Multiline))
                {
                    current3.ClearStyle(StyleIndex.All);
                    current3.SetStyle(CommentTagStyle);
                }
            }
            range.ClearFoldingMarkers();
        }

        protected void InitJScriptRegex()
        {
            JScriptStringRegex = new Regex("\"\"|''|\".*?[^\\\\]\"|'.*?[^\\\\]'", RegexCompiledOption);
            JScriptCommentRegex1 = new Regex("//.*$", RegexOptions.Multiline | RegexCompiledOption);
            JScriptCommentRegex2 = new Regex("(/\\*.*?\\*/)|(/\\*.*)", RegexOptions.Singleline | RegexCompiledOption);
            JScriptCommentRegex3 = new Regex("(/\\*.*?\\*/)|(.*\\*/)",
                RegexOptions.Singleline | RegexOptions.RightToLeft | RegexCompiledOption);
            JScriptNumberRegex = new Regex("\\b\\d+[\\.]?\\d*([eE]\\-?\\d+)?[lLdDfF]?\\b|\\b0x[a-fA-F\\d]+\\b",
                RegexCompiledOption);
            JScriptKeywordRegex =
                new Regex(
                    "\\b(true|false|break|case|catch|const|continue|default|delete|do|else|export|for|function|if|in|instanceof|new|null|return|switch|this|throw|try|var|void|while|with|typeof)\\b",
                    RegexCompiledOption);
        }

        public virtual void JScriptSyntaxHighlight(Range range)
        {
            range.tb.CommentPrefix = "//";
            range.tb.LeftBracket = '(';
            range.tb.RightBracket = ')';
            range.tb.LeftBracket2 = '{';
            range.tb.RightBracket2 = '}';
            range.tb.BracketsHighlightStrategy = BracketsHighlightStrategy.Strategy2;
            range.tb.AutoIndentCharsPatterns = "\r\n^\\s*[\\w\\.]+(\\s\\w+)?\\s*(?<range>=)\\s*(?<range>[^;]+);\r\n";
            range.ClearStyle(StringStyle, CommentStyle, NumberStyle, KeywordStyle);
            var flag = JScriptStringRegex == null;
            if (flag)
            {
                InitJScriptRegex();
            }
            range.SetStyle(StringStyle, JScriptStringRegex);
            range.SetStyle(CommentStyle, JScriptCommentRegex1);
            range.SetStyle(CommentStyle, JScriptCommentRegex2);
            range.SetStyle(CommentStyle, JScriptCommentRegex3);
            range.SetStyle(NumberStyle, JScriptNumberRegex);
            range.SetStyle(KeywordStyle, JScriptKeywordRegex);
            range.ClearFoldingMarkers();
            range.SetFoldingMarkers("{", "}");
            range.SetFoldingMarkers("/\\*", "\\*/");
        }

        protected void InitLuaRegex()
        {
            LuaStringRegex = new Regex("\"\"|''|\".*?[^\\\\]\"|'.*?[^\\\\]'", RegexCompiledOption);
            LuaCommentRegex1 = new Regex("--.*$", RegexOptions.Multiline | RegexCompiledOption);
            LuaCommentRegex2 = new Regex("(--\\[\\[.*?\\]\\])|(--\\[\\[.*)",
                RegexOptions.Singleline | RegexCompiledOption);
            LuaCommentRegex3 = new Regex("(--\\[\\[.*?\\]\\])|(.*\\]\\])",
                RegexOptions.Singleline | RegexOptions.RightToLeft | RegexCompiledOption);
            LuaNumberRegex = new Regex("\\b\\d+[\\.]?\\d*([eE]\\-?\\d+)?[lLdDfF]?\\b|\\b0x[a-fA-F\\d]+\\b",
                RegexCompiledOption);
            LuaKeywordRegex =
                new Regex(
                    "\\b(and|break|do|else|elseif|end|false|for|function|if|in|local|nil|not|or|repeat|return|then|true|until|while)\\b",
                    RegexCompiledOption);
            LuaFunctionsRegex =
                new Regex(
                    "\\b(assert|collectgarbage|dofile|error|getfenv|getmetatable|ipairs|load|loadfile|loadstring|module|next|pairs|pcall|print|rawequal|rawget|rawset|require|select|setfenv|setmetatable|tonumber|tostring|type|unpack|xpcall)\\b",
                    RegexCompiledOption);
        }

        public virtual void LuaSyntaxHighlight(Range range)
        {
            range.tb.CommentPrefix = "--";
            range.tb.LeftBracket = '(';
            range.tb.RightBracket = ')';
            range.tb.LeftBracket2 = '{';
            range.tb.RightBracket2 = '}';
            range.tb.BracketsHighlightStrategy = BracketsHighlightStrategy.Strategy2;
            range.tb.AutoIndentCharsPatterns = "\r\n^\\s*[\\w\\.]+(\\s\\w+)?\\s*(?<range>=)\\s*(?<range>.+)\r\n";
            range.ClearStyle(StringStyle, CommentStyle, NumberStyle, KeywordStyle, FunctionsStyle);
            var flag = LuaStringRegex == null;
            if (flag)
            {
                InitLuaRegex();
            }
            range.SetStyle(StringStyle, LuaStringRegex);
            range.SetStyle(CommentStyle, LuaCommentRegex1);
            range.SetStyle(CommentStyle, LuaCommentRegex2);
            range.SetStyle(CommentStyle, LuaCommentRegex3);
            range.SetStyle(NumberStyle, LuaNumberRegex);
            range.SetStyle(KeywordStyle, LuaKeywordRegex);
            range.SetStyle(FunctionsStyle, LuaFunctionsRegex);
            range.ClearFoldingMarkers();
            range.SetFoldingMarkers("{", "}");
            range.SetFoldingMarkers("--\\[\\[", "\\]\\]");
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
            range.SetStyle(CommentStyle, LuaCommentRegex1);
            range.SetStyle(CommentStyle, LuaCommentRegex2);
            range.SetStyle(CommentStyle, LuaCommentRegex3);
            range.SetStyle(PASMNumberStyle, LuaNumberRegex);
            range.SetStyle(PASMNumberStyle, PASMPreNumRegex);
            range.SetStyle(KeywordStyle, PASMKeywordRegex);
            range.SetStyle(FunctionsStyle, PASMFunctionsRegex);
            range.SetStyle(StaticClassStyle, PASMStaticClassRegex);
            range.ClearFoldingMarkers();
            range.SetFoldingMarkers("#\\[\\[", "\\]\\]");
        }

        public void InitPASMRegex()
        {
            PASMStringRegex = new Regex("\"\"|''|\".*?[^\\\\]\"|'.*?[^\\\\]'", RegexCompiledOption);
            LuaCommentRegex1 = new Regex("#.*$", RegexOptions.Multiline | RegexCompiledOption);
            LuaCommentRegex2 = new Regex("(#\\[\\[.*?\\]\\])|(--\\[\\[.*)",
                RegexOptions.Singleline | RegexCompiledOption);
            LuaCommentRegex3 = new Regex("(#\\[\\[.*?\\]\\])|(.*\\]\\])",
                RegexOptions.Singleline | RegexOptions.RightToLeft | RegexCompiledOption);
            LuaNumberRegex = new Regex("\\b\\d+[\\.]?\\d*([eE]\\-?\\d+)?[lLdDfF]?\\b|\\b0x[a-fA-F\\d]+\\b",
                RegexCompiledOption);
            PASMPreNumRegex = new Regex(":\\d",
                RegexCompiledOption);
            PASMKeywordRegex = new Regex("\\b(set|mov|pt|calib|re|call|if|im|malloc_c|malloc_p|malloc_d)\\b", RegexCompiledOption);
            PASMFunctionsRegex = new Regex($"\\b(BYTE|INT16|INT32|INT64|MATH|QMATH|VOR|VOP|VORL)\\b", RegexCompiledOption);
            PASMStaticClassRegex = new Regex($"\\b({StaticClasses})\\b", RegexCompiledOption);
        }

        protected void PASMAutoIndentNeeded(object sender, AutoIndentEventArgs args)
        {
        }

        protected void LuaAutoIndentNeeded(object sender, AutoIndentEventArgs args)
        {
            var flag = Regex.IsMatch(args.LineText, "^\\s*(end|until)\\b");
            if (flag)
            {
                args.Shift = -args.TabLength;
                args.ShiftNextLines = -args.TabLength;
            }
            else
            {
                var flag2 = Regex.IsMatch(args.LineText, "\\b(then)\\s*\\S+");
                if (!flag2)
                {
                    var flag3 = Regex.IsMatch(args.LineText, "^\\s*(function|do|for|while|repeat|if)\\b");
                    if (flag3)
                    {
                        args.ShiftNextLines = args.TabLength;
                    }
                    else
                    {
                        var flag4 = Regex.IsMatch(args.LineText, "^\\s*(else|elseif)\\b", RegexOptions.IgnoreCase);
                        if (flag4)
                        {
                            args.Shift = -args.TabLength;
                        }
                    }
                }
            }
        }
    }
}