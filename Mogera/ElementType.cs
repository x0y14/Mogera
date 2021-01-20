namespace Mogera
{
    public enum ElementType
    {
        Normal,// h1, h2, ...
        Special,// doctype, comment, ...
        NoClosing,// img, br, ...
        Content,// enclose text
    }
}