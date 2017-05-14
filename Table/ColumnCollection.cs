namespace BYOS.Table
{
    public class ColumnCollection : AbstractVectorCollection
    {
        public ColumnCollection()
        {
            DefaultPrefix = "Column ";
        }

        public ColumnCollection Create(int number)
        {
            for (int i = 0; i < number; i++)
            {
                Add(new Column());
            }

            return this;
        }
    }
}