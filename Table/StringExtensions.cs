namespace  BYOS.Extensions
{
    public static class StringExtensions
    {
        public static string CenterString(this string stringToCenter, int totalLength)
        {
            return stringToCenter.PadLeft(((totalLength - stringToCenter.Length) / 2)
                                + stringToCenter.Length)
                       .PadRight(totalLength);
        }
    }

    public enum Alignment
    {
        Left,
        Centre,
        Right,
        None
    }
}