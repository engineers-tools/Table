using BYOS.Interfaces;

namespace BYOS.Table
{
    public class Cell : ICell
    {
        public Cell() { }
        public int Row { get; set; }
        public int Column { get; set; }
        public object Value { get; set; } = null;

        public string Type
        {
            get
            {
                return Value.GetType().ToString();
            }
        }
    }
}