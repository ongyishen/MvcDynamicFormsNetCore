namespace MvcDynamicForms.NetCore.Helpers
{
    public static class ResponseExtensions
    {
        public static long ToLong(this Response obj)
        {
            return obj.Value.ParseToLong();
        }

        public static long ToLong(this Response obj, long defaultValue)
        {
            return obj.Value.ParseToLong(defaultValue);
        }

        public static long ToInt(this Response obj)
        {
            return obj.Value.ParseToInt();
        }

        public static long ToInt(this Response obj, int defaultValue)
        {
            return obj.Value.ParseToInt(defaultValue);
        }

        public static decimal ToDecimal(this Response obj)
        {
            return obj.Value.ParseToDecimal();
        }

        public static decimal ToDecimal(this Response obj, decimal defaultValue)
        {
            return obj.Value.ParseToDecimal(defaultValue);
        }

        public static bool ToBool(this Response obj)
        {
            return obj.Value.ParseYesNoToBool();
        }

        public static bool ToBool(this Response obj, bool result)
        {
            return obj.Value.ParseYesNoToBool();
        }

    }
}
