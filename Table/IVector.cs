namespace BYOS.Interfaces
{
    public interface IVector
    {
        string Header { get; set; }
        int Position { get; set; }
        string ToCsl();
    }
}