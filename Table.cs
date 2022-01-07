using Interfaces;
using Extensions;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using Newtonsoft.Json;

namespace EngineersTools
{
    public class Table : ITable
    {
        #region Constructors
        public Table() { }
        public Table(string title) { Title = title; }
        public Table(int rows, int columns)
        {
            for (int i = 0; i < rows; i++)
            {
                Rows.Add(new Row());
            }

            for (int j = 0; j < columns; j++)
            {
                Columns.Add(new Column());
            }
        }
        public Table(string title, int rows, int columns)
        {
            Title = title;

            for (int i = 0; i < rows; i++)
            {
                Rows.Add(new Row());
            }

            for (int j = 0; j < columns; j++)
            {
                Columns.Add(new Column());
            }
        }

        #endregion Constructors

        #region Properties

        private const string _DEFAULT_TITLE = "Table Title";
        private const string _DEFAULT_COLUMN_PREFIX = "Column";
        private const string _DEFAULT_ROW_PREFIX = "Row";
        private const char _COLUMN_DELIMITER = '|';
        private int _TableWidth = 0;
        public int MinColumnWidth { get; set; } = 9;
        public string Title { get; set; } = _DEFAULT_TITLE;
        private RowCollection _Rows;
        public RowCollection Rows
        {
            get
            {
                if (_Rows == null)
                {
                    _Rows = new RowCollection();
                    _Rows.ItemsAdded += _Rows_ItemsAdded;
                    _Rows.ItemsRemoved += _Rows_ItemsRemoved;
                }
                return _Rows;
            }
            set { _Rows = value; }
        }

        private void _Rows_ItemsRemoved(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (sender is RowCollection)
            {
                foreach (Row row in e.NewItems)
                {
                    _Cells.DeleteRow(row.Position);
                }
            }
        }

        private void _Rows_ItemsAdded(object sender, NotifyCollectionChangedEventArgs e)
        {
        }

        private ColumnCollection _Columns;
        public ColumnCollection Columns
        {
            get
            {
                if (_Columns == null)
                {
                    _Columns = new ColumnCollection();
                    _Columns.ItemsAdded += _Columns_ItemsAdded;
                    _Columns.ItemsRemoved += _Columns_ItemsRemoved;
                }
                return _Columns;
            }
            set { _Columns = value; }
        }

        private void _Columns_ItemsRemoved(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (sender is ColumnCollection)
            {
                foreach (Column column in e.NewItems)
                {
                    _Cells.DeleteColumn(column.Position);
                }
            }
        }

        private void _Columns_ItemsAdded(object sender, NotifyCollectionChangedEventArgs e)
        {
        }

        private CellCollection _Cells;
        public CellCollection Cells
        {
            get
            {
                if (_Cells == null) _Cells = new CellCollection();
                _Cells.ItemsAdded += _Cells_ItemsAdded;
                _Cells.ItemsRemoved += _Cells_ItemsRemoved;
                return _Cells;
            }
        }

        private void _Cells_ItemsRemoved(object sender, NotifyCollectionChangedEventArgs e)
        {

        }

        private void _Cells_ItemsAdded(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (sender is CellCollection)
            {
                foreach (Cell cell in e.NewItems)
                {
                    if (cell.Row > Rows.Count) Rows.Create(cell.Row - Rows.Count);
                    if (cell.Column > Columns.Count) Columns.Create(cell.Column - Columns.Count);
                }
            }
        }

        public string Size
        {
            get
            {
                return string.Format("{0} Rows by {1} Columns", Rows.Count.ToString(), Columns.Count.ToString());
            }
        }

        #endregion Properties

        #region Methods

        public int MaxColumnWidth(int index)
        {
            int minWidth = MinColumnWidth;

            if (index == 0)
            {
                foreach (var row in Rows)
                {
                    if (row.Header.Length > minWidth)
                        minWidth = row.Header.Length;
                }
                return minWidth;
            }

            var column = Columns.Where(c => c.Position == index).FirstOrDefault();
            if (column.Header.Length > minWidth)
                minWidth = column.Header.Length;

            var cellsForColumn = Cells.Where(c => c.Column == index);
            foreach (var cell in cellsForColumn)
            {
                if (cell.Value != null)
                {
                    if (cell.Value.ToString().Length > minWidth)
                    {
                        minWidth = cell.Value.ToString().Length;
                    }
                }
            }

            return minWidth;
        }

        public int MaxColumnWidth(string header)
        {
            var index = Columns.Where(c => c.Header == header).FirstOrDefault().Position;
            return MaxColumnWidth(index);
        }

        private string _GetTableTitle(Alignment alignment = Alignment.Centre)
        {
            var builder = new StringBuilder();

            if (Title.Length > _TableWidth) _TableWidth = Title.Length;

            var firstColumnWidth = MaxColumnWidth(0);
            string headers = _COLUMN_DELIMITER + " ".PadRight(firstColumnWidth) + _COLUMN_DELIMITER;
            foreach (var column in Columns)
            {
                headers = headers + column.Header.PadRight(MaxColumnWidth(column.Header)) + _COLUMN_DELIMITER;
            }
            builder.AppendLine(new string('=', headers.Length));

            switch (alignment)
            {
                case Alignment.Left:
                    builder.AppendLine(Title.PadRight(headers.Length));
                    break;
                case Alignment.Centre:
                    builder.AppendLine(Title.CenterString(headers.Length));
                    break;
                case Alignment.Right:
                    builder.AppendLine(Title.PadLeft(headers.Length));
                    break;
            }

            return builder.ToString();
        }

        private string _GetColumnHeaders(Alignment alignment = Alignment.Centre, bool includeRowSeparators = true, char delimiter = _COLUMN_DELIMITER)
        {
            var builder = new StringBuilder();

            var firstColumnWidth = MaxColumnWidth(0);

            string headers = (alignment == Alignment.None) ? delimiter + "Row Title" + delimiter : delimiter + "Row Title".PadRight(firstColumnWidth) + delimiter;

            switch (alignment)
            {
                case Alignment.Left:
                    foreach (var column in Columns)
                    {
                        headers = headers + column.Header.PadRight(MaxColumnWidth(column.Header)) + delimiter;
                    }
                    break;
                case Alignment.Centre:
                    foreach (var column in Columns)
                    {
                        headers = headers + column.Header.CenterString(MaxColumnWidth(column.Header)) + delimiter;
                    }
                    break;
                case Alignment.Right:
                    foreach (var column in Columns)
                    {
                        headers = headers + column.Header.PadLeft(MaxColumnWidth(column.Header)) + delimiter;
                    }
                    break;
                case Alignment.None:
                    foreach (var column in Columns)
                    {
                        headers = headers + column.Header + delimiter;
                    }
                    break;
            }

            if (_TableWidth < headers.Length) _TableWidth = headers.Length;

            if (includeRowSeparators) builder.AppendLine(new string('-', headers.Length));
            builder.AppendLine(headers);
            if (includeRowSeparators) builder.AppendLine(new string('-', headers.Length));

            return builder.ToString();
        }

        private string GetRowString(int index, Alignment aligment = Alignment.Left, char delimiter = _COLUMN_DELIMITER)
        {
            var row = Rows.Where(r => r.Position == index).FirstOrDefault();

            return GetRowString(row, aligment, delimiter);
        }

        private string GetRowString(IVector row, Alignment aligment = Alignment.Left, char delimiter = _COLUMN_DELIMITER)
        {
            string rowString = (aligment == Alignment.None) ? delimiter + row.Header + delimiter : delimiter + row.Header.PadRight(MaxColumnWidth(0)) + delimiter;

            switch (aligment)
            {
                case Alignment.Left:
                    foreach (var column in Columns)
                    {
                        var cell = Cells[row.Position, column.Position];
                        var cellValue = cell == null ? "NULL" : cell.ToString();
                        rowString = rowString + string.Format("{0}{1}", cellValue.PadRight(MaxColumnWidth(column.Header)), delimiter);
                    }
                    break;
                case Alignment.Centre:
                    foreach (var column in Columns)
                    {
                        var cell = Cells[row.Position, column.Position];
                        var cellValue = cell == null ? "NULL" : cell.ToString();
                        rowString = rowString + string.Format("{0}{1}", cellValue.CenterString(MaxColumnWidth(column.Header)), delimiter);
                    }
                    break;
                case Alignment.Right:
                    foreach (var column in Columns)
                    {
                        var cell = Cells[row.Position, column.Position];
                        var cellValue = cell == null ? "NULL" : cell.ToString();
                        rowString = rowString + string.Format("{0}{1}", cellValue.PadLeft(MaxColumnWidth(column.Header)), delimiter);
                    }
                    break;
                case Alignment.None:
                    foreach (var column in Columns)
                    {
                        var cell = Cells[row.Position, column.Position];
                        var cellValue = cell == null ? "NULL" : cell.ToString();
                        rowString = rowString + string.Format("{0}{1}", cellValue, delimiter);
                    }
                    break;
            }

            return rowString;
        }

        public override string ToString()
        {
            var tableBuilder = new StringBuilder();

            tableBuilder.Append(_GetTableTitle());

            tableBuilder.Append(_GetColumnHeaders());

            foreach (var row in Rows)
            {
                var rowString = GetRowString(row, Alignment.Centre);
                tableBuilder.AppendLine(rowString);
                tableBuilder.AppendLine(new string('-', rowString.Length));
            }

            return tableBuilder.ToString();
        }

        public string ToJson()
        {
            return JsonConvert.SerializeObject(this, Formatting.Indented);
        }

        public string ToCsv()
        {
            var tableBuilder = new StringBuilder();

            tableBuilder.Append(_GetColumnHeaders(alignment: Alignment.None, includeRowSeparators: false, delimiter: ',').Remove(0, 1).TrimEnd(','));

            foreach (var row in Rows)
            {
                tableBuilder.AppendLine(GetRowString(row, Alignment.None, ',').Remove(0, 1).TrimEnd(','));
            }

            return tableBuilder.ToString();
        }

        #endregion Methods
    }
}