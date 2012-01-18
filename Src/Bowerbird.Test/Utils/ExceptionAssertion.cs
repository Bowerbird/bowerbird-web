using System;

namespace Bowerbird.Test.Utils
{
    public static class BowerbirdThrows
    {
        public static bool Exception<T>(Action method)
        {
            try
            {
                method.Invoke();
            }
            catch (Exception ex)
            {
                return ex is T;
            }

            return false;
        }
    }
}