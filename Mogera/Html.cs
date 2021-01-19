using System.Collections.Generic;

namespace Mogera
{
    public class Html
    {
        private string RowHtml { get; set; }
        private List<Element> Elements { get; set; }

        public Html(List<Element> elements)
        {
            Elements = elements;
        }
    }
}