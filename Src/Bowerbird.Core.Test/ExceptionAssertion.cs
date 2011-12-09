using System;

namespace Bowerbird.Core.Test
{
    public static class Throws
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