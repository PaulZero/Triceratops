using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Triceratops.Libraries.Helpers;

namespace Triceratops.Libraries.Tests.Helpers
{
    [TestClass]
    public class RetryHelperTest
    {
        protected uint ActualRetries { get; set; }

        protected uint? SucceedAfter { get; set; }

        [TestCleanup]
        public void Cleanup()
        {
            ActualRetries = 0;
            SucceedAfter = null;
        }

        [TestMethod]
        public async Task TestThatRetryHelperActuallyRetriesButFailsUpToTheLimit()
        {
            var expectedRetries = RetryHelper.DefaultRetryLimit;

            await RunTest(expectedRetries);

            Assert.AreEqual(expectedRetries, ActualRetries);
        }

        [TestMethod]
        public async Task TestThatRetryHelperCanActuallyHandleSuccess()
        {
            var expectedRetries = 3u;
            SucceedAfter = 3;

            await RunTest(expectedRetries);

            Assert.AreEqual(expectedRetries, ActualRetries);
        }

        private async Task RunTest(uint retries)
        {
            await RetryHelper.RetryTask(retry => AttemptRetry(), retries, TimeSpan.Zero);
        }

        private Task<bool> AttemptRetry()
        {
            ActualRetries++;

            if (SucceedAfter == ActualRetries)
            {
                return Task.FromResult(true);
            }

            return Task.FromResult(false);
        }
    }
}
