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
            while (!IsEof())
            {
                if (string.IsNullOrWhiteSpace(GetChar()) || GetChar() == "\n")
                {
                    ConsumeChar();
                }
                else { break; }
            }
        }

        protected bool IsEof()
        {
            return Original.Length <= Pos;
        }
    }
}