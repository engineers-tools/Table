using Interfaces;

namespace EngineersTools
{
    public class Cell<T> : ICell
    {
        public int Row { get; set; }
        public int Column { get; set; }
        public T Value { get; set; }

        public string Type
        {
            get
            {
                return typeof(T).ToString();
            }
        }
    }
}