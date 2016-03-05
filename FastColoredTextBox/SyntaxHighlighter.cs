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
        protected static readonly Platform platformType = PlatformType.GetOperationSystemPlatform();

        public readonly Style BlackStyle = new TextStyle(Brushes.Black, null, FontStyle.Regular);

        public readonly Style BlueBoldStyle = new TextStyle(Brushes.Blue, null, FontStyle.Bold);

        public readonly Style BlueStyle = new TextStyle(new Pen(Color.FromArgb(86, 156, 214)).Brush, null,
            FontStyle.Regular);

        public readonly Style BoldStyle = new TextStyle(null, null, FontStyle.Bold | FontStyle.Underline);
        public readonly Style ClassStyle = new TextStyle(new Pen(Color.FromArgb(160, 60, 190)).Brush, null, FontStyle.Bold | FontStyle.Italic);

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

        public readonly Style OperatorStyle = new TextStyle(new Pen(Color.FromArgb(255, 0, 255)).Brush, null,
            FontStyle.Regular);

        public readonly Style RedStyle = new TextStyle(new Pen(Color.FromArgb(206, 145, 120)).Brush, null,
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

<<<<<<< HEAD
        protected Regex PHPCommentRegex1,
                      PHPCommentRegex2,
                      PHPCommentRegex3;

        protected Regex PHPKeywordRegex1,
                      PHPKeywordRegex2,
                      PHPKeywordRegex3;

        protected Regex PHPNumberRegex;
        protected Regex PHPStringRegex;
        protected Regex PHPVarRegex;

        protected Regex SQLCommentRegex1,
                      SQLCommentRegex2,
                      SQLCommentRegex3, 
                      SQLCommentRegex4;

        protected Regex SQLFunctionsRegex;
        protected Regex SQLKeywordsRegex;
        protected Regex SQLNumberRegex;
        protected Regex SQLStatementsRegex;
        protected Regex SQLStringRegex;
        protected Regex SQLTypesRegex;
        protected Regex SQLVarRegex;
        protected Regex VBClassNameRegex;
        protected Regex VBCommentRegex;
        protected Regex VBKeywordRegex;
        protected Regex VBNumberRegex;
        protected Regex VBStringRegex;
=======
        protected Regex PASMFunctionsRegex;

        protected Regex PASMKeywordRegex;

        protected Regex PASMStringRegex;
>>>>>>> refs/remotes/origin/PashLang-FCTB

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
                case Language.CrocScript:
                    CSharpSyntaxHighlight(range);
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
                case Language.CrocScript:
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
                case Language.CrocScript:
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

        public virtual void CSharpSyntaxHighlight(Range range)
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
<<<<<<< HEAD
            //set folding markers
            range.SetFoldingMarkers("<head", "</head>", RegexOptions.IgnoreCase);
            range.SetFoldingMarkers("<body", "</body>", RegexOptions.IgnoreCase);
            range.SetFoldingMarkers("<table", "</table>", RegexOptions.IgnoreCase);
            range.SetFoldingMarkers("<form", "</form>", RegexOptions.IgnoreCase);
            range.SetFoldingMarkers("<div", "</div>", RegexOptions.IgnoreCase);
            range.SetFoldingMarkers("<script", "</script>", RegexOptions.IgnoreCase);
            range.SetFoldingMarkers("<tr", "</tr>", RegexOptions.IgnoreCase);
        }

        protected void InitXMLRegex()
        {
            XMLCommentRegex1 = new Regex(@"(<!--.*?-->)|(<!--.*)", RegexOptions.Singleline | RegexCompiledOption);
            XMLCommentRegex2 = new Regex(@"(<!--.*?-->)|(.*-->)",
                                          RegexOptions.Singleline | RegexOptions.RightToLeft | RegexCompiledOption);
            XMLTagRegex = new Regex(@"<\?|<|/>|</|>|\?>", RegexCompiledOption);
            XMLTagNameRegex = new Regex(@"<[?](?<range1>[x][m][l]{1})|<(?<range>[!\w:]+)", RegexCompiledOption);
            XMLEndTagRegex = new Regex(@"</(?<range>[\w:]+)>", RegexCompiledOption);
            XMLTagContentRegex = new Regex(@"<[^>]+>", RegexCompiledOption);
            XMLAttrRegex =
                new Regex(
                    @"(?<range>[\w\d\-\:]+)[ ]*=[ ]*'[^']*'|(?<range>[\w\d\-\:]+)[ ]*=[ ]*""[^""]*""|(?<range>[\w\d\-\:]+)[ ]*=[ ]*[\w\d\-\:]+",
                    RegexCompiledOption);
            XMLAttrValRegex =
                new Regex(
                    @"[\w\d\-]+?=(?<range>'[^']*')|[\w\d\-]+[ ]*=[ ]*(?<range>""[^""]*"")|[\w\d\-]+[ ]*=[ ]*(?<range>[\w\d\-]+)",
                    RegexCompiledOption);
            XMLEntityRegex = new Regex(@"\&(amp|gt|lt|nbsp|quot|apos|copy|reg|#[0-9]{1,8}|#x[0-9a-f]{1,8});",
                                        RegexCompiledOption | RegexOptions.IgnoreCase);
            XMLCDataRegex = new Regex(@"<!\s*\[CDATA\s*\[(?<text>(?>[^]]+|](?!]>))*)]]>", RegexCompiledOption | RegexOptions.IgnoreCase); // http://stackoverflow.com/questions/21681861/i-need-a-regex-that-matches-cdata-elements-in-html
            XMLFoldingRegex = new Regex(@"<(?<range>/?\w+)\s[^>]*?[^/]>|<(?<range>/?\w+)\s*>", RegexOptions.Singleline | RegexCompiledOption);
        }

        /// <summary>
        /// Highlights XML code
        /// </summary>
        /// <param name="range"></param>
        public virtual void XMLSyntaxHighlight(Range range)
        {
            range.tb.CommentPrefix = null;
            range.tb.LeftBracket = '<';
            range.tb.RightBracket = '>';
            range.tb.LeftBracket2 = '(';
            range.tb.RightBracket2 = ')';
            range.tb.AutoIndentCharsPatterns = @"";
            //clear style of changed range
            range.ClearStyle(CommentStyle, XmlTagBracketStyle, XmlTagNameStyle, XmlAttributeStyle, XmlAttributeValueStyle,
                             XmlEntityStyle, XmlCDataStyle);

            //
            if (XMLTagRegex == null)
            {
                InitXMLRegex();
            }

            //xml CData
            range.SetStyle(XmlCDataStyle, XMLCDataRegex);

            //comment highlighting
            range.SetStyle(CommentStyle, XMLCommentRegex1);
            range.SetStyle(CommentStyle, XMLCommentRegex2);

            //tag brackets highlighting
            range.SetStyle(XmlTagBracketStyle, XMLTagRegex);

            //tag name
            range.SetStyle(XmlTagNameStyle, XMLTagNameRegex);

            //end of tag
            range.SetStyle(XmlTagNameStyle, XMLEndTagRegex);

            //attributes
            range.SetStyle(XmlAttributeStyle, XMLAttrRegex);

            //attribute values
            range.SetStyle(XmlAttributeValueStyle, XMLAttrValRegex);

            //xml entity
            range.SetStyle(XmlEntityStyle, XMLEntityRegex);

            //clear folding markers
            range.ClearFoldingMarkers();

            //set folding markers
            XmlFolding(range);
        }

        private void XmlFolding(Range range)
        {
            var stack = new Stack<XmlFoldingTag>();
            var id = 0;
            var fctb = range.tb;
            //extract opening and closing tags (exclude open-close tags: <TAG/>)
            foreach (var r in range.GetRanges(XMLFoldingRegex))
            {
                var tagName = r.Text;
                var iLine = r.Start.iLine;
                //if it is opening tag...
                if (tagName[0] != '/')
                {
                    // ...push into stack
                    var tag = new XmlFoldingTag {Name = tagName, id = id++, startLine = r.Start.iLine};
                    stack.Push(tag);
                    // if this line has no markers - set marker
                    if (string.IsNullOrEmpty(fctb[iLine].FoldingStartMarker))
                        fctb[iLine].FoldingStartMarker = tag.Marker;
                }
                else
                {
                    //if it is closing tag - pop from stack
                    if (stack.Count > 0)
                    {
                        var tag = stack.Pop();
                        //compare line number
                        if (iLine == tag.startLine)
                        {
                            //remove marker, because same line can not be folding
                            if (fctb[iLine].FoldingStartMarker == tag.Marker) //was it our marker?
                                fctb[iLine].FoldingStartMarker = null;
                        }
                        else
                        {
                            //set end folding marker
                            if (string.IsNullOrEmpty(fctb[iLine].FoldingEndMarker))
                                fctb[iLine].FoldingEndMarker = tag.Marker;
                        }
                    }
                }
            }
        }

        class XmlFoldingTag
		{
			public string Name;
			public int id;
			public int startLine;
			public string Marker { get { return Name + id; } }
		}

        protected void InitSQLRegex()
        {
            SQLStringRegex = new Regex(@"""""|''|"".*?[^\\]""|'.*?[^\\]'", RegexCompiledOption);
            SQLNumberRegex = new Regex(@"\b\d+[\.]?\d*([eE]\-?\d+)?\b", RegexCompiledOption);
            SQLCommentRegex1 = new Regex(@"--.*$", RegexOptions.Multiline | RegexCompiledOption);
            SQLCommentRegex2 = new Regex(@"(/\*.*?\*/)|(/\*.*)", RegexOptions.Singleline | RegexCompiledOption);
            SQLCommentRegex3 = new Regex(@"(/\*.*?\*/)|(.*\*/)", RegexOptions.Singleline | RegexOptions.RightToLeft | RegexCompiledOption);
            SQLCommentRegex4 = new Regex(@"#.*$", RegexOptions.Multiline | RegexCompiledOption);
            SQLVarRegex = new Regex(@"@[a-zA-Z_\d]*\b", RegexCompiledOption);
            SQLStatementsRegex = new Regex(@"\b(ALTER APPLICATION ROLE|ALTER ASSEMBLY|ALTER ASYMMETRIC KEY|ALTER AUTHORIZATION|ALTER BROKER PRIORITY|ALTER CERTIFICATE|ALTER CREDENTIAL|ALTER CRYPTOGRAPHIC PROVIDER|ALTER DATABASE|ALTER DATABASE AUDIT SPECIFICATION|ALTER DATABASE ENCRYPTION KEY|ALTER ENDPOINT|ALTER EVENT SESSION|ALTER FULLTEXT CATALOG|ALTER FULLTEXT INDEX|ALTER FULLTEXT STOPLIST|ALTER FUNCTION|ALTER INDEX|ALTER LOGIN|ALTER MASTER KEY|ALTER MESSAGE TYPE|ALTER PARTITION FUNCTION|ALTER PARTITION SCHEME|ALTER PROCEDURE|ALTER QUEUE|ALTER REMOTE SERVICE BINDING|ALTER RESOURCE GOVERNOR|ALTER RESOURCE POOL|ALTER ROLE|ALTER ROUTE|ALTER SCHEMA|ALTER SERVER AUDIT|ALTER SERVER AUDIT SPECIFICATION|ALTER SERVICE|ALTER SERVICE MASTER KEY|ALTER SYMMETRIC KEY|ALTER TABLE|ALTER TRIGGER|ALTER USER|ALTER VIEW|ALTER WORKLOAD GROUP|ALTER XML SCHEMA COLLECTION|BULK INSERT|CREATE AGGREGATE|CREATE APPLICATION ROLE|CREATE ASSEMBLY|CREATE ASYMMETRIC KEY|CREATE BROKER PRIORITY|CREATE CERTIFICATE|CREATE CONTRACT|CREATE CREDENTIAL|CREATE CRYPTOGRAPHIC PROVIDER|CREATE DATABASE|CREATE DATABASE AUDIT SPECIFICATION|CREATE DATABASE ENCRYPTION KEY|CREATE DEFAULT|CREATE ENDPOINT|CREATE EVENT NOTIFICATION|CREATE EVENT SESSION|CREATE FULLTEXT CATALOG|CREATE FULLTEXT INDEX|CREATE FULLTEXT STOPLIST|CREATE FUNCTION|CREATE INDEX|CREATE LOGIN|CREATE MASTER KEY|CREATE MESSAGE TYPE|CREATE PARTITION FUNCTION|CREATE PARTITION SCHEME|CREATE PROCEDURE|CREATE QUEUE|CREATE REMOTE SERVICE BINDING|CREATE RESOURCE POOL|CREATE ROLE|CREATE ROUTE|CREATE RULE|CREATE SCHEMA|CREATE SERVER AUDIT|CREATE SERVER AUDIT SPECIFICATION|CREATE SERVICE|CREATE SPATIAL INDEX|CREATE STATISTICS|CREATE SYMMETRIC KEY|CREATE SYNONYM|CREATE TABLE|CREATE TRIGGER|CREATE TYPE|CREATE USER|CREATE VIEW|CREATE WORKLOAD GROUP|CREATE XML INDEX|CREATE XML SCHEMA COLLECTION|DELETE|DISABLE TRIGGER|DROP AGGREGATE|DROP APPLICATION ROLE|DROP ASSEMBLY|DROP ASYMMETRIC KEY|DROP BROKER PRIORITY|DROP CERTIFICATE|DROP CONTRACT|DROP CREDENTIAL|DROP CRYPTOGRAPHIC PROVIDER|DROP DATABASE|DROP DATABASE AUDIT SPECIFICATION|DROP DATABASE ENCRYPTION KEY|DROP DEFAULT|DROP ENDPOINT|DROP EVENT NOTIFICATION|DROP EVENT SESSION|DROP FULLTEXT CATALOG|DROP FULLTEXT INDEX|DROP FULLTEXT STOPLIST|DROP FUNCTION|DROP INDEX|DROP LOGIN|DROP MASTER KEY|DROP MESSAGE TYPE|DROP PARTITION FUNCTION|DROP PARTITION SCHEME|DROP PROCEDURE|DROP QUEUE|DROP REMOTE SERVICE BINDING|DROP RESOURCE POOL|DROP ROLE|DROP ROUTE|DROP RULE|DROP SCHEMA|DROP SERVER AUDIT|DROP SERVER AUDIT SPECIFICATION|DROP SERVICE|DROP SIGNATURE|DROP STATISTICS|DROP SYMMETRIC KEY|DROP SYNONYM|DROP TABLE|DROP TRIGGER|DROP TYPE|DROP USER|DROP VIEW|DROP WORKLOAD GROUP|DROP XML SCHEMA COLLECTION|ENABLE TRIGGER|EXEC|EXECUTE|REPLACE|FROM|INSERT|MERGE|OPTION|OUTPUT|SELECT|TOP|TRUNCATE TABLE|UPDATE|UPDATE STATISTICS|WHERE|WITH|INTO|IN|SET)\b", RegexOptions.IgnoreCase | RegexCompiledOption);
            SQLKeywordsRegex = new Regex(@"\b(ADD|ALL|AND|ANY|AS|ASC|AUTHORIZATION|BACKUP|BEGIN|BETWEEN|BREAK|BROWSE|BY|CASCADE|CHECK|CHECKPOINT|CLOSE|CLUSTERED|COLLATE|COLUMN|COMMIT|COMPUTE|CONSTRAINT|CONTAINS|CONTINUE|CROSS|CURRENT|CURRENT_DATE|CURRENT_TIME|CURSOR|DATABASE|DBCC|DEALLOCATE|DECLARE|DEFAULT|DENY|DESC|DISK|DISTINCT|DISTRIBUTED|DOUBLE|DUMP|ELSE|END|ERRLVL|ESCAPE|EXCEPT|EXISTS|EXIT|EXTERNAL|FETCH|FILE|FILLFACTOR|FOR|FOREIGN|FREETEXT|FULL|FUNCTION|GOTO|GRANT|GROUP|HAVING|HOLDLOCK|IDENTITY|IDENTITY_INSERT|IDENTITYCOL|IF|INDEX|INNER|INTERSECT|IS|JOIN|KEY|KILL|LIKE|LINENO|LOAD|NATIONAL|NOCHECK|NONCLUSTERED|NOT|NULL|OF|OFF|OFFSETS|ON|OPEN|OR|ORDER|OUTER|OVER|PERCENT|PIVOT|PLAN|PRECISION|PRIMARY|PRINT|PROC|PROCEDURE|PUBLIC|RAISERROR|READ|READTEXT|RECONFIGURE|REFERENCES|REPLICATION|RESTORE|RESTRICT|RETURN|REVERT|REVOKE|ROLLBACK|ROWCOUNT|ROWGUIDCOL|RULE|SAVE|SCHEMA|SECURITYAUDIT|SHUTDOWN|SOME|STATISTICS|TABLE|TABLESAMPLE|TEXTSIZE|THEN|TO|TRAN|TRANSACTION|TRIGGER|TSEQUAL|UNION|UNIQUE|UNPIVOT|UPDATETEXT|USE|USER|VALUES|VARYING|VIEW|WAITFOR|WHEN|WHILE|WRITETEXT)\b", RegexOptions.IgnoreCase | RegexCompiledOption);
            SQLFunctionsRegex = new Regex(@"(@@CONNECTIONS|@@CPU_BUSY|@@CURSOR_ROWS|@@DATEFIRST|@@DATEFIRST|@@DBTS|@@ERROR|@@FETCH_STATUS|@@IDENTITY|@@IDLE|@@IO_BUSY|@@LANGID|@@LANGUAGE|@@LOCK_TIMEOUT|@@MAX_CONNECTIONS|@@MAX_PRECISION|@@NESTLEVEL|@@OPTIONS|@@PACKET_ERRORS|@@PROCID|@@REMSERVER|@@ROWCOUNT|@@SERVERNAME|@@SERVICENAME|@@SPID|@@TEXTSIZE|@@TRANCOUNT|@@VERSION)\b|\b(ABS|ACOS|APP_NAME|ASCII|ASIN|ASSEMBLYPROPERTY|AsymKey_ID|ASYMKEY_ID|asymkeyproperty|ASYMKEYPROPERTY|ATAN|ATN2|AVG|CASE|CAST|CEILING|Cert_ID|Cert_ID|CertProperty|CHAR|CHARINDEX|CHECKSUM_AGG|COALESCE|COL_LENGTH|COL_NAME|COLLATIONPROPERTY|COLLATIONPROPERTY|COLUMNPROPERTY|COLUMNS_UPDATED|COLUMNS_UPDATED|CONTAINSTABLE|CONVERT|COS|COT|COUNT|COUNT_BIG|CRYPT_GEN_RANDOM|CURRENT_TIMESTAMP|CURRENT_TIMESTAMP|CURRENT_USER|CURRENT_USER|CURSOR_STATUS|DATABASE_PRINCIPAL_ID|DATABASE_PRINCIPAL_ID|DATABASEPROPERTY|DATABASEPROPERTYEX|DATALENGTH|DATALENGTH|DATEADD|DATEDIFF|DATENAME|DATEPART|DAY|DB_ID|DB_NAME|DECRYPTBYASYMKEY|DECRYPTBYCERT|DECRYPTBYKEY|DECRYPTBYKEYAUTOASYMKEY|DECRYPTBYKEYAUTOCERT|DECRYPTBYPASSPHRASE|DEGREES|DENSE_RANK|DIFFERENCE|ENCRYPTBYASYMKEY|ENCRYPTBYCERT|ENCRYPTBYKEY|ENCRYPTBYPASSPHRASE|ERROR_LINE|ERROR_MESSAGE|ERROR_NUMBER|ERROR_PROCEDURE|ERROR_SEVERITY|ERROR_STATE|EVENTDATA|EXP|FILE_ID|FILE_IDEX|FILE_NAME|FILEGROUP_ID|FILEGROUP_NAME|FILEGROUPPROPERTY|FILEPROPERTY|FLOOR|fn_helpcollations|fn_listextendedproperty|fn_servershareddrives|fn_virtualfilestats|fn_virtualfilestats|FORMATMESSAGE|FREETEXTTABLE|FULLTEXTCATALOGPROPERTY|FULLTEXTSERVICEPROPERTY|GETANSINULL|GETDATE|GETUTCDATE|GROUPING|HAS_PERMS_BY_NAME|HOST_ID|HOST_NAME|IDENT_CURRENT|IDENT_CURRENT|IDENT_INCR|IDENT_INCR|IDENT_SEED|IDENTITY\(|INDEX_COL|INDEXKEY_PROPERTY|INDEXPROPERTY|IS_MEMBER|IS_OBJECTSIGNED|IS_SRVROLEMEMBER|ISDATE|ISDATE|ISNULL|ISNUMERIC|Key_GUID|Key_GUID|Key_ID|Key_ID|KEY_NAME|KEY_NAME|LEFT|LEN|LOG|LOG10|LOWER|LTRIM|MAX|MIN|MONTH|NCHAR|NEWID|NTILE|NULLIF|OBJECT_DEFINITION|OBJECT_ID|OBJECT_NAME|OBJECT_SCHEMA_NAME|OBJECTPROPERTY|OBJECTPROPERTYEX|OPENDATASOURCE|OPENQUERY|OPENROWSET|OPENXML|ORIGINAL_LOGIN|ORIGINAL_LOGIN|PARSENAME|PATINDEX|PATINDEX|PERMISSIONS|PI|POWER|PUBLISHINGSERVERNAME|PWDCOMPARE|PWDENCRYPT|QUOTENAME|RADIANS|RAND|RANK|REPLICATE|REVERSE|RIGHT|ROUND|ROW_NUMBER|ROWCOUNT_BIG|RTRIM|SCHEMA_ID|SCHEMA_ID|SCHEMA_NAME|SCHEMA_NAME|SCOPE_IDENTITY|SERVERPROPERTY|SESSION_USER|SESSION_USER|SESSIONPROPERTY|SETUSER|SIGN|SignByAsymKey|SignByCert|SIN|SOUNDEX|SPACE|SQL_VARIANT_PROPERTY|SQRT|SQUARE|STATS_DATE|STDEV|STDEVP|STR|STUFF|SUBSTRING|SUM|SUSER_ID|SUSER_NAME|SUSER_SID|SUSER_SNAME|SWITCHOFFSET|SYMKEYPROPERTY|symkeyproperty|sys\.dm_db_index_physical_stats|sys\.fn_builtin_permissions|sys\.fn_my_permissions|SYSDATETIME|SYSDATETIMEOFFSET|SYSTEM_USER|SYSTEM_USER|SYSUTCDATETIME|TAN|TERTIARY_WEIGHTS|TEXTPTR|TODATETIMEOFFSET|TRIGGER_NESTLEVEL|TYPE_ID|TYPE_NAME|TYPEPROPERTY|UNICODE|UPDATE\(|UPPER|USER_ID|USER_NAME|USER_NAME|VAR|VARP|VerifySignedByAsymKey|VerifySignedByCert|XACT_STATE|YEAR)\b", RegexOptions.IgnoreCase | RegexCompiledOption);
            SQLTypesRegex =
                new Regex(
                    @"\b(BIGINT|NUMERIC|BIT|SMALLINT|DECIMAL|SMALLMONEY|INT|TINYINT|MONEY|FLOAT|REAL|DATE|DATETIMEOFFSET|DATETIME2|SMALLDATETIME|DATETIME|TIME|CHAR|VARCHAR|TEXT|NCHAR|NVARCHAR|NTEXT|BINARY|VARBINARY|IMAGE|TIMESTAMP|HIERARCHYID|TABLE|UNIQUEIDENTIFIER|SQL_VARIANT|XML)\b",
                    RegexOptions.IgnoreCase | RegexCompiledOption);
        }

        /// <summary>
        /// Highlights SQL code
        /// </summary>
        /// <param name="range"></param>
        public virtual void SQLSyntaxHighlight(Range range)
        {
            range.tb.CommentPrefix = "--";
            range.tb.LeftBracket = '(';
            range.tb.RightBracket = ')';
            range.tb.LeftBracket2 = '\x0';
            range.tb.RightBracket2 = '\x0';

            range.tb.AutoIndentCharsPatterns = @"";
            //clear style of changed range
            range.ClearStyle(CommentStyle, StringStyle, NumberStyle, VariableStyle, StatementsStyle, KeywordStyle,
                             FunctionsStyle, TypesStyle);
            //
            if (SQLStringRegex == null)
                InitSQLRegex();
            //comment highlighting
            range.SetStyle(CommentStyle, SQLCommentRegex1);
            range.SetStyle(CommentStyle, SQLCommentRegex2);
            range.SetStyle(CommentStyle, SQLCommentRegex3);
            range.SetStyle(CommentStyle, SQLCommentRegex4);
            //string highlighting
            range.SetStyle(StringStyle, SQLStringRegex);
            //number highlighting
            range.SetStyle(NumberStyle, SQLNumberRegex);
            //types highlighting
            range.SetStyle(TypesStyle, SQLTypesRegex);
            //var highlighting
            range.SetStyle(VariableStyle, SQLVarRegex);
            //statements
            range.SetStyle(StatementsStyle, SQLStatementsRegex);
            //keywords
            range.SetStyle(KeywordStyle, SQLKeywordsRegex);
            //functions
            range.SetStyle(FunctionsStyle, SQLFunctionsRegex);

            //clear folding markers
            range.ClearFoldingMarkers();
            //set folding markers
            range.SetFoldingMarkers(@"\bBEGIN\b", @"\bEND\b", RegexOptions.IgnoreCase);
            //allow to collapse BEGIN..END blocks
            range.SetFoldingMarkers(@"/\*", @"\*/"); //allow to collapse comment block
        }

        protected void InitPHPRegex()
        {
            PHPStringRegex = new Regex(@"""""|''|"".*?[^\\]""|'.*?[^\\]'", RegexCompiledOption);
            PHPNumberRegex = new Regex(@"\b\d+[\.]?\d*\b", RegexCompiledOption);
            PHPCommentRegex1 = new Regex(@"(//|#).*$", RegexOptions.Multiline | RegexCompiledOption);
            PHPCommentRegex2 = new Regex(@"(/\*.*?\*/)|(/\*.*)", RegexOptions.Singleline | RegexCompiledOption);
            PHPCommentRegex3 = new Regex(@"(/\*.*?\*/)|(.*\*/)",
                                         RegexOptions.Singleline | RegexOptions.RightToLeft | RegexCompiledOption);
            PHPVarRegex = new Regex(@"\$[a-zA-Z_\d]*\b", RegexCompiledOption);
            PHPKeywordRegex1 =
                new Regex(
                    @"\b(die|echo|empty|exit|eval|include|include_once|isset|list|require|require_once|return|print|unset)\b",
                    RegexCompiledOption);
            PHPKeywordRegex2 =
                new Regex(
                    @"\b(abstract|and|array|as|break|case|catch|cfunction|class|clone|const|continue|declare|default|do|else|elseif|enddeclare|endfor|endforeach|endif|endswitch|endwhile|extends|final|for|foreach|function|global|goto|if|implements|instanceof|interface|namespace|new|or|private|protected|public|static|switch|throw|try|use|var|while|xor)\b",
                    RegexCompiledOption);
            PHPKeywordRegex3 = new Regex(@"__CLASS__|__DIR__|__FILE__|__LINE__|__FUNCTION__|__METHOD__|__NAMESPACE__",
                                         RegexCompiledOption);
        }

        /// <summary>
        /// Highlights PHP code
        /// </summary>
        /// <param name="range"></param>
        public virtual void PHPSyntaxHighlight(Range range)
        {
            range.tb.CommentPrefix = "//";
            range.tb.LeftBracket = '(';
            range.tb.RightBracket = ')';
            range.tb.LeftBracket2 = '{';
            range.tb.RightBracket2 = '}';
            range.tb.BracketsHighlightStrategy = BracketsHighlightStrategy.Strategy2;
            //clear style of changed range
            range.ClearStyle(StringStyle, CommentStyle, NumberStyle, VariableStyle, KeywordStyle, KeywordStyle2,
                             KeywordStyle3);

            range.tb.AutoIndentCharsPatterns
                = @"
^\s*\$[\w\.\[\]\'\""]+\s*(?<range>=)\s*(?<range>[^;]+);
";

            //
            if (PHPStringRegex == null)
                InitPHPRegex();
            //string highlighting
            range.SetStyle(StringStyle, PHPStringRegex);
            //comment highlighting
            range.SetStyle(CommentStyle, PHPCommentRegex1);
            range.SetStyle(CommentStyle, PHPCommentRegex2);
            range.SetStyle(CommentStyle, PHPCommentRegex3);
            //number highlighting
            range.SetStyle(NumberStyle, PHPNumberRegex);
            //var highlighting
            range.SetStyle(VariableStyle, PHPVarRegex);
            //keyword highlighting
            range.SetStyle(KeywordStyle, PHPKeywordRegex1);
            range.SetStyle(KeywordStyle2, PHPKeywordRegex2);
            range.SetStyle(KeywordStyle3, PHPKeywordRegex3);

            //clear folding markers
            range.ClearFoldingMarkers();
            //set folding markers
            range.SetFoldingMarkers("{", "}"); //allow to collapse brackets block
            range.SetFoldingMarkers(@"/\*", @"\*/"); //allow to collapse comment block
=======
>>>>>>> refs/remotes/origin/PashLang-FCTB
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
            range.SetStyle(NumberStyle, LuaNumberRegex);
            range.SetStyle(KeywordStyle, PASMKeywordRegex);
            range.SetStyle(FunctionsStyle, PASMFunctionsRegex);
            range.ClearFoldingMarkers();
            range.SetFoldingMarkers("#\\[\\[", "\\]\\]");
        }

        protected void InitPASMRegex()
        {
            PASMStringRegex = new Regex("\"\"|''|\".*?[^\\\\]\"|'.*?[^\\\\]'", RegexCompiledOption);
            LuaCommentRegex1 = new Regex("#.*$", RegexOptions.Multiline | RegexCompiledOption);
            LuaCommentRegex2 = new Regex("(#\\[\\[.*?\\]\\])|(--\\[\\[.*)",
                RegexOptions.Singleline | RegexCompiledOption);
            LuaCommentRegex3 = new Regex("(#\\[\\[.*?\\]\\])|(.*\\]\\])",
                RegexOptions.Singleline | RegexOptions.RightToLeft | RegexCompiledOption);
            LuaNumberRegex = new Regex("\\b\\d+[\\.]?\\d*([eE]\\-?\\d+)?[lLdDfF]?\\b|\\b0x[a-fA-F\\d]+\\b",
                RegexCompiledOption);
            PASMKeywordRegex = new Regex("\\b(set|mov|pt|calib|re|call|if|im)\\b", RegexCompiledOption);
            PASMFunctionsRegex = new Regex("\\b(BYTE|INT16|INT32|INT64|MATH|QMATH|VOR|VOP|VORL|MALLOC)\\b", RegexCompiledOption);
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