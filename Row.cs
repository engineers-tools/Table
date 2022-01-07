using System;
using Interfaces;

namespace EngineersTools
{
    public class Row : IVector
    {
        public string Header { get; set; }
        public int Position { get; set; }

        public Row() { }
        public Row(string header)
        {
            Header = header;
        }

        public override string ToString()
        {
            return base.ToString();
        }

        public string ToCsl()
        {
            throw new NotImplementedException();
        }
    }
}