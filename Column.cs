using System;
using Interfaces;

namespace EngineersTools
{
    public class Column : IVector
    {
        public string Header { get; set; }
        public int Position { get; set; }

        public Column() { }
        public Column(string header)
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