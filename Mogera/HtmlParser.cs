using System;
using System.Collections.Generic;
using System.Text;

namespace Mogera
{
    public class HtmlParser : Parser
    {
        private Html HtmlData { get; set; }
        
        public HtmlParser(string data) : base(data)
        {
            // HtmlData = new Html(new List<Element>());
        }

        private Element AnalyzeSpecialTag()
        {
            Element result = new Element(ElementType.Special);
            result.ElementStartedAt = Pos;
            ConsumeChar();// <
            ConsumeChar();// !
            
            // DocType
            if ("dD".Contains(GetChar()))
            {
                // maybe !doctype, until whitespace
                var doc = ConsumeUntil(" ").ToUpper();
                if (doc == "DOCTYPE")
                {
                    ConsumeChar();// whitespace
                    var docType = ConsumeUntil(">");
                    result.DocumentType = docType;
                    result.IsComment = false;
                    result.ElementEndedAt = Pos;
                    ConsumeChar();// >
                    return result;
                }
            }
            
            // Comment
            else if (Original.Substring(Pos, 2) == "--")
            {
                ConsumeChar();// -
                ConsumeChar();// -
                var comment = "";
                while (!IsEof())
                {
                    //コメント内部にハイフンがあった場合の処理。
                    // ハイフンを見つけて、そこから３つが "-->" だったら、終了の合図。
                    comment += ConsumeUntil("-");
                    // Console.WriteLine(comment);
                    if (Original.Substring(Pos, 3) == "-->")
                    {
                        // 残りのハイフンを消す。
                        // 終了地点を ">" に合わせるため、これだけ先にやる。
                        ConsumeChar();// -
                        ConsumeChar();// -
                        result.Comment = comment;
                        result.IsComment = true;
                        result.ElementEndedAt = Pos;
                        ConsumeChar();// >
                        return result;
                    }

                    comment += "-";
                    ConsumeChar();
                }
            }
            return result;
        }
        
        private Element AnalyzeNormalTag()
        {
            Element result = new Element(ElementType.Normal);
            result.ElementStartedAt = Pos;
            result.OpeningStartedAt = Pos;
            ConsumeChar();// <
            
            string openingTagRaw = ConsumeUntil(">");
            result.OpeningEndedAt = Pos;
            
            ConsumeChar();// >
            result.AttributeRaw = openingTagRaw;
            var openingTagData = new ElementParser(openingTagRaw).Parse();
            // openingタグ終了 <h1>
            // 次中身をとる。
            result.TagName = openingTagData.Item1;
            var children = new List<Element>();
            while (!IsEof())
            {
                ConsumeWhiteSpace();
                if (GetNextChar() != "/")
                {
                    if (GetChar() == "<")
                    {
                        children.Add(AnalyzeTag());
                        ConsumeWhiteSpace();
                        continue;
                    }
                    
                    result.EnclosedText = CheckOnlyTrash(ConsumeUntil("<"));
                }
                break;
            }

            result.Children = children;
            result.ClosingStartedAt = Pos;
            result.Attributes = openingTagData.Item2;

            ConsumeChar();// <
            ConsumeWhiteSpace();
            var closingTagRaw = ConsumeUntil(">");
            var closingTagData = new ElementParser(closingTagRaw).Parse();
            
            result.ClosingEndedAt = Pos;
            result.ElementEndedAt = Pos;
            ConsumeChar();// >
            return result;
        }
        
        private Element AnalyzeTag()
        {
            // GetChar() == "<"
            var nextChar = GetNextChar();
            switch (nextChar)
            {
                case "!":
                {
                    // ElementType.Special
                    // Console.WriteLine("entry: special");
                    return AnalyzeSpecialTag();
                }
                default:
                {
                    // ElementType.Normal
                    //         &
                    // ElementType.NoClosing
                    // Console.WriteLine("entry: normal");
                    return AnalyzeNormalTag();
                }
            }
        }

        public List<Element> Parse()
        {
            List<Element> elements = new List<Element>();
            
            while (!IsEof())
            {
                if (GetChar() == "<")
                {
                    var t = AnalyzeTag();
                    elements.Add(t);
                    ConsumeWhiteSpace();
                }
                else
                {
                    Console.WriteLine($"[Parse] unknown object! : '{GetChar()}'");
                    int i = GetChar().ToCharArray()[0];
                    Console.WriteLine($"{i}");
                    break;
                }
            }

            return elements;
        }
    }
}