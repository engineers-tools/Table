namespace Interfaces
{
    public interface ITable
    {
        string Title { get; set; }
        string ToJson();
        string ToCsv();
    }
}