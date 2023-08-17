namespace IntermediateAPI.Utilities
{
    public class DateFormatter
    {
        public static string TransformToYYYYMMDD(string inputDate)
        {
            if (DateTime.TryParse(inputDate, out DateTime date))
            {
                return date.ToString("yyyy-MM-dd");
            }
            else
            {
                throw new ArgumentException("Invalid date format");
            }
        }

        public static string RemoveTimePart(string dateTimeString)
        {
            int tIndex = dateTimeString.IndexOf('T');

            if (tIndex >= 0)
            {
                return dateTimeString.Substring(0, tIndex);
            }
            else
            {
                return dateTimeString;
            }
        }
    }
}
