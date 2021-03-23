using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Polly;
using Polly.Timeout;
using Polly.Wrap;

namespace WhalesTale
{
    public class Policies
    {
        private const int DefaultTimeOutMS = 3000;
        private const int NumberOfRetries = 3;

        public readonly AsyncPolicyWrap PolicyWrap;
        private readonly TimeSpan _pauseBetweenFailures = TimeSpan.FromSeconds(2);

        public Policies()
        {
            var retryPolicy = Policy
                .Handle<Exception>()
                .WaitAndRetryAsync(retryCount: NumberOfRetries,
                    sleepDurationProvider: i => _pauseBetweenFailures,
                    onRetry: ManageRetryException);

            var timeOutPolicy = Policy
                .TimeoutAsync(timeout: TimeSpan.FromMilliseconds(DefaultTimeOutMS),
                    timeoutStrategy: TimeoutStrategy.Pessimistic,
                    onTimeoutAsync: ManageTimeoutException);

            PolicyWrap = Policy.WrapAsync(retryPolicy, timeOutPolicy);

        }
        private static void ManageRetryException(Exception exception, TimeSpan timeSpan, int retryCount, Context context)
        {
            var action = context != null ? context.First().Key : "unknown method";
            var actionDescription = context != null ? context.First().Value : "unknown description";
            var msg = $"Retry n°{retryCount} of {action} ({actionDescription}) : {exception.Message}";
            Console.WriteLine(msg);
        }

        private static Task ManageTimeoutException(Context context, TimeSpan timeSpan, Task task)
        {
            var action = context != null ? context.First().Key : "unknown method";
            var actionDescription = context != null ? context.First().Value : "unknown description";

            task.ContinueWith(t =>
            {
                if (t.IsFaulted)
                {
                    var msg = $"Running {action} ({actionDescription}) but the execution timed out after {timeSpan.TotalSeconds} seconds, eventually terminated with: {t.Exception}.";
                    Console.WriteLine(msg);
                }
                else if (t.IsCanceled)
                {
                    var msg = $"Running {action} ({actionDescription}) but the execution timed out after {timeSpan.TotalSeconds} seconds, task cancelled.";
                    Console.WriteLine(msg);
                }
            });
            return task;
        }
    }
}
