using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using Extensions;
using Newtonsoft.Json;
using EngineersTools;
using Interfaces;

namespace TypedTable
{
    public class Table<T> : ITable
    {
        private const string _DEFAULT_TITLE = "Table Title";
        private const string _DEFAULT_COLUMN_PREFIX = "Column";
        private const string _DEFAULT_ROW_PREFIX = "Row";
        private const char _TABLE_COLUMN_DELIMITER = '|';

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

        private string _Title = _DEFAULT_TITLE;
        public string Title
        {
            get { return _Title; }
            set { _Title = value; }
        }

        public string Size
        {
            get
            {
                return string.Format("{0} Rows by {1} Columns", _RowCount.ToString(), _ColumnCount.ToString());
            }
        }

        private int _MinColumnWidth = 9;
        public void SetMinimumColumnWidth(int width)
        {
            _MinColumnWidth = width;
        }

        public int MaxColumnWidth(int index)
        {
            int minWidth = _MinColumnWidth;

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

        private int _TableWidth = 0;

        private string _GetTableTitle(Alignment alignment = Alignment.Centre)
        {
            var builder = new StringBuilder();

            if (Title.Length > _TableWidth) _TableWidth = Title.Length;

            var firstColumnWidth = MaxColumnWidth(0);
            string headers = _TABLE_COLUMN_DELIMITER + " ".PadRight(firstColumnWidth) + _TABLE_COLUMN_DELIMITER;
            foreach (var column in Columns)
            {
                headers = headers + column.Header.PadRight(MaxColumnWidth(column.Header)) + _TABLE_COLUMN_DELIMITER;
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

        private string _GetColumnHeaders(Alignment alignment = Alignment.Centre)
        {
            var builder = new StringBuilder();

            var firstColumnWidth = MaxColumnWidth(0);

            string headers = _TABLE_COLUMN_DELIMITER + " ".PadRight(firstColumnWidth) + _TABLE_COLUMN_DELIMITER;

            switch (alignment)
            {
                case Alignment.Left:
                    foreach (var column in Columns)
                    {
                        headers = headers + column.Header.PadRight(MaxColumnWidth(column.Header)) + _TABLE_COLUMN_DELIMITER;
                    }
                    break;
                case Alignment.Centre:
                    foreach (var column in Columns)
                    {
                        headers = headers + column.Header.CenterString(MaxColumnWidth(column.Header)) + _TABLE_COLUMN_DELIMITER;
                    }
                    break;
                case Alignment.Right:
                    foreach (var column in Columns)
                    {
                        headers = headers + column.Header.PadLeft(MaxColumnWidth(column.Header)) + _TABLE_COLUMN_DELIMITER;
                    }
                    break;
            }

            if (_TableWidth < headers.Length) _TableWidth = headers.Length;

            builder.AppendLine(new string('-', headers.Length));
            builder.AppendLine(headers);
            builder.AppendLine(new string('-', headers.Length));

            return builder.ToString();
        }

        private string GetRowString(int index, Alignment aligment = Alignment.Left)
        {
            var row = Rows.Where(r => r.Position == index).FirstOrDefault();

            return GetRowString(row, aligment);
        }

        private string GetRowString(Row row, Alignment aligment = Alignment.Left)
        {
            string rowString = _TABLE_COLUMN_DELIMITER + row.Header.PadRight(MaxColumnWidth(0)) + _TABLE_COLUMN_DELIMITER;

            switch (aligment)
            {
                case Alignment.Left:
                    foreach (var column in Columns)
                    {
                        var cell = GetCell(row.Position, column.Position);
                        var cellValue = cell.Value == null ? "NULL" : cell.Value.ToString();
                        rowString = rowString + string.Format("{0}{1}", cellValue.PadRight(MaxColumnWidth(column.Header)), _TABLE_COLUMN_DELIMITER);
                    }
                    break;
                case Alignment.Centre:
                    foreach (var column in Columns)
                    {
                        var cell = GetCell(row.Position, column.Position);
                        var cellValue = cell.Value == null ? "NULL" : cell.Value.ToString();
                        rowString = rowString + string.Format("{0}{1}", cellValue.CenterString(MaxColumnWidth(column.Header)), _TABLE_COLUMN_DELIMITER);
                    }
                    break;
                case Alignment.Right:
                    foreach (var column in Columns)
                    {
                        var cell = GetCell(row.Position, column.Position);
                        var cellValue = cell.Value == null ? "NULL" : cell.Value.ToString();
                        rowString = rowString + string.Format("{0}{1}", cellValue.PadLeft(MaxColumnWidth(column.Header)), _TABLE_COLUMN_DELIMITER);
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

        public string ToCsv(bool includeRowHeaders = false)
        {
            var builder = new StringBuilder();

            var rowLine = includeRowHeaders ? "Row Headers" : string.Empty;

            foreach (var column in Columns)
            {
                rowLine = includeRowHeaders ? rowLine + "," + column.Header : rowLine + column.Header;
                if (column != Columns.Last() && !includeRowHeaders) rowLine = rowLine + ",";
            }

            builder.AppendLine(rowLine);

            foreach (var row in Rows)
            {
                rowLine = includeRowHeaders ? row.Header : string.Empty;

                foreach (var column in Columns)
                {
                    var cellValue = GetCell(row.Position, column.Position).Value;
                    rowLine = includeRowHeaders ? rowLine + "," + cellValue : rowLine + cellValue;
                    if (column != Columns.Last() && !includeRowHeaders) rowLine = rowLine + ",";
                }

                builder.AppendLine(rowLine);
            }

            return builder.ToString();
        }

        public string ToJson()
        {
            return JsonConvert.SerializeObject(this, Formatting.Indented);
        }

        public void FromJson(string json)
        {
            // TODO: Need a custom JsonWriter for this cast.
            var table = (Table<T>)JsonConvert.DeserializeObject(json);

            if (this.Title == _DEFAULT_TITLE)
                this.Title = table.Title;

            foreach (var row in table.Rows)
            {
                this.Rows.Add(row);
            }

            foreach (var column in table.Columns)
            {
                this.Columns.Add(column);
            }

            foreach (var cell in table.Cells)
            {
                this.SetCell(cell.Row, cell.Column, cell.Value);
            }
        }

        #region Cells

        private ObservableCollection<Cell<T>>? _Cells;
        private ReadOnlyObservableCollection<Cell<T>> _ROCells;
        public ReadOnlyObservableCollection<Cell<T>> Cells
        {
            get
            {
                InitCells();
                return _ROCells;
            }
        }
        private void InitCells()
        {
            if (_Cells == null)
            {
                _Cells = new ObservableCollection<Cell<T>>();
                _ROCells = new ReadOnlyObservableCollection<Cell<T>>(_Cells);
            }
        }
        private void _PadCells()
        {
            InitCells();

            int maxColumn = 1;
            int maxRow = 1;

            for (int i = maxRow; i <= _RowCount; i++)
            {
                for (int j = maxColumn; j <= _ColumnCount; j++)
                {
                    if (GetCell(i, j) == null)
                    {
                        _Cells.Add(new Cell<T>() { Row = i, Column = j });
                    }
                }
            }
        }
        public Cell<T> GetCell(int row, int column)
        {
            if (row > _RowCount || column > _ColumnCount) throw new IndexOutOfRangeException(string.Format("The row or column requested does not exist."));

            return Cells.Where(c => c.Row == row && c.Column == column).FirstOrDefault();
        }
        public Cell<T> GetCell(string rowHeader, string columnHeader)
        {
            var row = Rows.Where(r => r.Header == rowHeader).FirstOrDefault().Position;
            var column = Columns.Where(c => c.Header == columnHeader).FirstOrDefault().Position;

            return Cells.Where(c => c.Row == row && c.Column == column).FirstOrDefault();
        }
        public void SetCell(int row, int column, T value)
        {
            var cell = GetCell(row, column);
            cell.Value = value;
        }
        public void SetCell(string rowHeader, string columnHeader, T value)
        {
            var cell = GetCell(rowHeader, columnHeader);
            cell.Value = value;
        }

        #endregion Cells

        #region Rows

        private ObservableCollection<Row> _Rows;
        private int _RowCount = 0;
        public int RowCount { get { return _RowCount; } }

        public void AddRows(int number)
        {
            for (int i = 0; i < number; i++)
            {
                this.Rows.Add(new Row());
            }
        }
        public ObservableCollection<Row> Rows
        {
            get
            {
                if (_Rows == null)
                {
                    _Rows = new ObservableCollection<Row>();
                    _Rows.CollectionChanged += _Rows_CollectionChanged;
                }
                return _Rows;
            }
        }
        private void _Rows_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            Row item = (Row)e.NewItems[0];

            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    _RowCount++;
                    item.Position = _RowCount;
                    if (string.IsNullOrEmpty(item.Header)) item.Header = _DEFAULT_ROW_PREFIX + " " + _RowCount;
                    _PadCells();
                    break;
                case NotifyCollectionChangedAction.Remove:
                    _RowCount--;
                    _PadCells();
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
        public void BuildRow(IEnumerable<T> values, string? header = null)
        {
            Row row = null;
            int rowNumber = 0;
            int columnNumber = 1;

            if (string.IsNullOrEmpty(header))
            {
                Rows.Add(new Row());
                rowNumber = RowCount;
            }
            else if (Rows.Where(r => r.Header == header).Count() > 0)
            {
                row = Rows.Where(r => r.Header == header).FirstOrDefault();
                rowNumber = row.Position;
            }
            else
            {
                Rows.Add(new Row(header));
                rowNumber = RowCount;
            }

            foreach (var v in values)
            {
                SetCell(rowNumber, columnNumber, v);
                columnNumber++;
            }
        }

        public void AddRow(params T[] values)
        {
            int rowNumber = 0;
            int columnNumber = 1;

            Rows.Add(new Row());
            rowNumber = RowCount;

            foreach (var v in values)
            {
                SetCell(rowNumber, columnNumber, v);
                columnNumber++;
            }
        }

        #endregion Rows

        #region Columns

        private ObservableCollection<Column> _Columns;
        private int _ColumnCount = 0;
        public int ColumnCount { get { return _ColumnCount; } }

        public void AddColumns(int number)
        {
            for (int i = 0; i < number; i++)
            {
                this.Columns.Add(new Column());
            }
        }
        public ObservableCollection<Column> Columns
        {
            get
            {
                if (_Columns == null)
                {
                    _Columns = new ObservableCollection<Column>();
                    _Columns.CollectionChanged += _Columns_CollectionChanged;
                }
                return _Columns;
            }
        }
        private void _Columns_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            Column item = (Column)e.NewItems[0];

            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    _ColumnCount++;
                    item.Position = _ColumnCount;
                    if (string.IsNullOrEmpty(item.Header)) item.Header = _DEFAULT_COLUMN_PREFIX + " " + _ColumnCount;
                    _PadCells();
                    break;
                case NotifyCollectionChangedAction.Remove:
                    _ColumnCount--;
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

        public string ToCsv()
        {
            throw new NotImplementedException();
        }

        #endregion Columns
    }
}
