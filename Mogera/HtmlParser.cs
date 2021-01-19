using System;
using System.Collections.Generic;

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
            if (GetChar() == "<")
            {
                result.Children = AnalyzeTag();
                result.EnclosedText = null;
            }
            else
            {
                try
                {
                    result.EnclosedText = ConsumeUntil("<");
                }
                catch (Exception e)
                {
                    result.EnclosedText = null;
                    Console.WriteLine(e);
                    throw;
                }
            }

            result.ClosingStartedAt = Pos;
            result.Attributes = openingTagData.Item2;

            ConsumeChar();// <
            var closingTagRaw = ConsumeUntil(">");
            var closingTagData = new ElementParser(closingTagRaw).Parse();
            
            result.ClosingEndedAt = Pos;
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
                    
                    // Console.WriteLine("[Element]");
                    // Console.WriteLine($": Type: {t.Type}, Name: {t.TagName}");
                    // if (t.Attributes != null)
                    // {
                    //     Console.WriteLine(": Attributes");
                    //     foreach (var key in t.Attributes.Keys)
                    //     {
                    //         Console.WriteLine($"\t - '{key}' : '{t.Attributes[key]}'");
                    //     }
                    // }
                    
                    elements.Add(t);

                }
                else
                {
                    Console.WriteLine($"[Parse] unknown object! : '{GetChar()}'");
                    break;
                }
            }

            return elements;
        }
    }
}