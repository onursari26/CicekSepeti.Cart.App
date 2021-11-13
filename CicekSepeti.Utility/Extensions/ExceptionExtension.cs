using System;

namespace CicekSepeti.Utility.Extensions
{
    public static class ExceptionExtension
    {
        public static string GetInnerExceptionMessage(this Exception ex)
        {
            if (ex.InnerException == null)
                return ex.Message;

            return ex.Message + " r\nInnerException:" + GetInnerExceptionMessage(ex.InnerException);
        }
    }
}
