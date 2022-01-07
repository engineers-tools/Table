# Table
A general abstraction of a Table object

Table is a simple project I created to be used as part of larger projects. It allows me to quickly add an abstraction to use data tables
and related functionality.

## How to use Table

```CS
using EngineersTools;

// Create a new table with
// Title: 'My New Table'
// 4 Rows x 3 Columns
var table = new Table("My New Table", 4, 3);

// Add a header to the fisrt column
table.Columns[1].Header = "First";

// Add text to cell on Row 2, Col 1
table.Cells[2, 1] = "Content 2,1";
// Add a number to cell on Row 1, Col 1 
table.Cells[1, 1] = 67.0m;
```

```CS
// Output the size of the table
Console.WriteLine(table.Size);
```

![SizeOutput](/img/SizeOutput.png)

``` CS
// Output the content of cell Row 1, Col 2
Console.WriteLine(table.Cells[1, 1]);
```

![SingleCellOutput](/img/SingleCellOutput.png)

```CS
// Output the table as a string
Console.WriteLine(table.ToString());
```

![ToStringOutput](/img/ToStringOutput.png)

```CS
// Output the table as CSV
Console.WriteLine(table.ToCsv());
```

![ToCsvOutput](/img/ToCsvOutput.png)

```CS
// Output the table as JSON
Console.WriteLine(table.ToJson());
```

![ToJsonOutput](/img/ToJsonOutput.png)
