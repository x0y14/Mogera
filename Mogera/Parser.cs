using System;
using System.Collections.Generic;

namespace Mogera
{
    public class Parser
    {
        protected int Pos { get; set; }
        protected string Original { get; }

        protected Parser(string data)
        {
            Pos = 0;
            Original = data;
        }

        protected string GetChar()
        {
            return Original.Substring(Pos, 1);
        }
        
        protected string GetNextChar()
        {
            // return Charactors[Pos + 1];
            return Original.Substring(Pos + 1, 1);
        }

        protected string ConsumeChar()
        {
            var c = GetChar();
            Pos++;
            return c;
        }

        protected string ConsumeUntil(string target)
        {
            string result = "";
            while (!IsEof())
            {
                if (GetChar() == target)
                {
                    break;
                }
                result += ConsumeChar();
            }

            return result;
        }
        
        protected void ConsumeWhiteSpace()
        {
            var safegard = 0;
            var p = Pos;
            while (!IsEof())
            {
                if (GetChar() == " " || GetChar() == "\n" || GetChar() == "\t" || string.IsNullOrWhiteSpace(GetChar()))
                {
                    ConsumeChar();
                    safegard++;
                }
                else { break; }
            }
        }

        protected bool IsEof()
        {
            return Original.Length <= Pos;
        }

        protected string CheckOnlyTrash(string item)
        {
            string result = "";
            foreach (var c in item.ToCharArray())
            {
                if (c == " ".ToCharArray()[0] || c == "\n".ToCharArray()[0] || c == "\t".ToCharArray()[0])
                {
                    continue;
                }
                else
                {
                    result += c;
                }
            }

            if (result == "")
            {
                return result;
            }

            return item;
        }
    }
}