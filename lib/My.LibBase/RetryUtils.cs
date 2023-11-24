using System;
using System.Threading;
using System.Threading.Tasks;

namespace My
{
    public static partial class RetryUtils
    {
        public static bool DoAndSleep(Func<bool> func, int chance, TimeSpan interval)
        {
            TimeSpan used = TimeSpan.FromSeconds(0);
            while (chance > 0)
            {
                if (func())
                {
                    return true;
                }
                Console.WriteLine($"retry failed, chance={chance} sleep={interval}s total={used}s");
                Thread.Sleep(interval);
                used += interval;
                chance--;
            }
            return false;
        }

        public static async ValueTask<bool> DoAndSleepAsync(Func<ValueTask<bool>> func, int chance, TimeSpan interval) {
            TimeSpan used = TimeSpan.FromSeconds(0);
            while (chance > 0)
            {
                if (await func()) {
                    return true;
                }
                Console.WriteLine($"retry failed, chance={chance} sleep={interval}s total={used}s");
                Thread.Sleep(interval);
                used += interval;
                chance--;
            }
            return false;
        }
    }
}
