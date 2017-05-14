using System;
using BYOS.Interfaces;

namespace BYOS.Table
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