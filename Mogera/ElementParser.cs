using System;
using System.Collections.Generic;

namespace Mogera
{
    public class ElementParser : Parser
    {
        public ElementParser(string data): base(data) {}
        public (string, Dictionary<string, dynamic>) Parse()
        {
            if (GetChar() == "/")
            {
                var t = "";
                while (!IsEof())
                {
                    t += ConsumeChar();
                }

                t = t.Substring(1, t.Length - 1);

                return (t, null);
            }

            if (!Original.Contains(" "))
            {
                // has no attributes
                return (Original, new Dictionary<string, dynamic>());
            }
            
            var tagName = ConsumeUntil(" ");
            ConsumeChar();// whiteSpace
            ConsumeWhiteSpace();
            
            var attrs = new Dictionary<string, dynamic>();

            // bool? isSingleQuotation = null;
            
            while (!IsEof())
            {
                var key = ConsumeUntil("=");
                if (key.Contains(" "))// ' id = "..." ' みたいにイコールまでに空白が入っていた場合の処理。
                {
                    key = key.Replace(" ", "");
                }

                ConsumeChar();// =
                ConsumeWhiteSpace();
                if (GetChar() == "\"")
                {
                    ConsumeChar();// "
                    // Console.WriteLine("found double quotation!");
                    // isSingleQuotation = true;
                    var value = ConsumeUntil("\"");
                    // Console.WriteLine($": '{value}'");
                    ConsumeChar();// "
                    attrs[key] = value;
                }
                else if (GetChar() == "\'")
                {
                    ConsumeChar();// '
                    // Console.WriteLine("found single quotation!");
                    // isSingleQuotation = false;
                    var value = ConsumeUntil("\'");
                    // Console.WriteLine($": '{value}'");
                    ConsumeChar();// '
                    attrs[key] = value;
                }
                else
                {
                    throw new Exception("element tag data, value, not found");
                }
            }
            
            return (tagName, attrs);
        }
    }
}