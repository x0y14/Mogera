using System;
using System.Collections.Generic;

namespace Mogera
{
    public class Element
    {
        public string TagName { get; set; }
        public ElementType Type { get; set; }
        public string DocumentType { get; set; }// sp
        public string Comment { get; set; }// sp
        
        public bool IsComment { get; set; }// sp
        
        //idea (直置きでないということ。)
        // public SpecialElement SpElement
        // SpecialElement.IsComment
        //               .Comment
        //               .DocumentType
        
        public string AttributeRaw { get; set; }
        public Dictionary<string, dynamic> Attributes { get; set; }
        public List<Element> Children { get; set; }// Element
        
        public string EnclosedText { get; set; }
        public string Content { get; set; }
        public int OpeningStartedAt { get; set; }//開始タグの
        public int OpeningEndedAt { get; set; }
        public int ClosingStartedAt { get; set; }// 終了タグの
        public int ClosingEndedAt { get; set; }
        public  int ElementStartedAt { get; set; }// タグ全体の
        public  int ElementEndedAt { get; set; }

        public Element(ElementType type)
        {
            Type = type;
        }
    }
}