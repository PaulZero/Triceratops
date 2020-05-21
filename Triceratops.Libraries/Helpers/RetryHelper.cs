using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Triceratops.Libraries.Helpers
{
    public static class RetryHelper
    {
        public static readonly TimeSpan DefaultInterval = TimeSpan.FromMilliseconds(500);

        public static readonly uint DefaultRetryLimit = 10;

        /// <summary>
        /// Attempt a task until it finishes.
        /// </summary>
        /// <param name="task">The task to attempt, must return a bool value indicating the task has finished properly</param>
        /// <param name="retryLimit">How many times to try the task before giving up entirely (default 10)</param>
        /// <param name="interval">How long to wait after the task has failed (default 500 milliseconds)</param>
        /// <returns></returns>
        public static async Task<bool> RetryTask(Func<int, Task<bool>> taskCallback, uint? retryLimit = null, TimeSpan? interval = null)
        {
            var maxRetries = retryLimit ?? DefaultRetryLimit;
            var retryDelay = interval ?? DefaultInterval;
            var retries = 0;

            do
            {
                var task = taskCallback(retries);

                try
                {
                    if (await task)
                    {
                        return true;
                    }

                    await Task.Delay(retryDelay);
                }
                catch
                {
                    await Task.Delay(retryDelay);
                }
                finally
                {
                    retries++;
                }
            }
            while (retries < maxRetries);

            return false;
        }
    }
}
