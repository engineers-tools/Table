using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;

namespace EngineersTools
{
    public class CellCollection : ReadOnlyObservableCollection<Cell>
    {
        private static ObservableCollection<Cell> _Cells = new ObservableCollection<Cell>();

        public CellCollection() : base(_Cells)
        {
            CollectionChanged += _CollectionChanged;
        }

        private void _CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    ItemsAdded?.Invoke(this, e);
                    break;
                case NotifyCollectionChangedAction.Remove:
                    ItemsRemoved?.Invoke(this, e);
                    break;
                case NotifyCollectionChangedAction.Replace:
                    break;
                case NotifyCollectionChangedAction.Move:
                    break;
                case NotifyCollectionChangedAction.Reset:
                    break;
                default:
                    break;
            }
        }

        public delegate void ItemsAddedHandler(object sender, NotifyCollectionChangedEventArgs e);
        public event ItemsAddedHandler ItemsAdded;

        public delegate void ItemsRemovedHandler(object sender, NotifyCollectionChangedEventArgs e);
        public event ItemsRemovedHandler ItemsRemoved;

        public object this[int row, int column]
        {
            get
            {
                return _GetCell(row, column)?.Value;
            }

            set
            {
                var cell = _GetCell(row, column);
                if (cell == null)
                {
                    cell = new Cell() { Row = row, Column = column, Value = value };
                    _Cells.Add(cell);
                }
                else
                {
                    _Cells[_Cells.IndexOf(cell)].Value = value;
                }
            }
        }

        public void DeleteRow(int rowNumber)
        {

        }

        public void DeleteColumn(int columnNumber)
        {

        }

        private Cell _GetCell(int row, int column)
        {
            return _Cells.Where(c => c.Row == row && c.Column == column).FirstOrDefault();
        }
    }
}