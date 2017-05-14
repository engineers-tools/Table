using BYOS.Table;
using static System.Console;

namespace ConsoleApplication
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var table = new Table("My New Table",4,3);
            var embededTable = new Table("Embeded",2,2);

            // table.Columns[1].Header = "First";
            // table.Cells[2,1] = "Pedo";
            // table.Cells[1,1] = 67.0m;
            table.Cells[1,1] = embededTable;
            // table.Cells[5,1] = "Pipi";
            // table.Cells[2,1] = embededTable;

            // WriteLine(table.Cells[1,1]);
            WriteLine(table.ToString());
            // WriteLine(table.ToCsv());
            // WriteLine(table.ToJson());
            // WriteLine(table.Size);

            // WriteLine("Press any key ...");
            // ReadKey();
        }
    }
}
