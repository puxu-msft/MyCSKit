using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace My
{
    public static partial class ExceptionUtils
    {
        public static T FindInnerException<T>(Exception root, Func<Exception, bool>? predicate = null)
            where T : Exception {
            Exception ex = root;
            while (ex != null)
            {
                if (ex is T) {
                    if (predicate == null || predicate(ex)) {
                        return (T)ex;
                    }
                }

                if (ex is AggregateException) {
                    foreach (var ex0 in ((AggregateException)ex).InnerExceptions)
                    {
                        T ret = FindInnerException<T>(ex0, predicate);
                        if (ret != null) {
                            return ret;
                        }
                    }
                    return default;
                }

                ex = ex.InnerException;
            }
            return default;
        }
    }
}
