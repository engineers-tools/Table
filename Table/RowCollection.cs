namespace BYOS.Table
{
    public class RowCollection : AbstractVectorCollection
    {
        public RowCollection()
        {
            DefaultPrefix = "Row ";
        }

        public RowCollection Create(int number)
        {
            for (int i = 0; i < number; i++)
            {
                Add(new Row());
            }

            return this;
        }
    }
}