using Confluent.Kafka.Core.Threading.Internal;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Confluent.Kafka.Core.Tests.Core.Threading
{
    public class AsyncLockTests
    {
        [Fact]
        public async Task LockAsync_ShouldAcquireLock()
        {
            // Arrange
            var options = new AsyncLockOptions
            {
                MaxDegreeOfParallelism = 1,
                HandleLockByKey = false
            };

            var asyncLock = new AsyncLock(options);

            var context = AsyncLockContext.Empty;
            var token = CancellationToken.None;

            // Act & Assert
            Assert.Equal(1, asyncLock.CurrentCount);

            using var releaser = await asyncLock.LockAsync(context, token);

            Assert.Equal(0, asyncLock.CurrentCount);
        }

        [Fact]
        public async Task LockAsync_ShouldNotAcquireLock()
        {
            // Arrange
            var options = new AsyncLockOptions
            {
                MaxDegreeOfParallelism = 1,
                HandleLockByKey = false
            };

            var asyncLock = new AsyncLock(options);

            var context1 = AsyncLockContext.Empty;
            var context2 = AsyncLockContext.Empty;
            var token = CancellationToken.None;

            // Act & Assert
            Assert.Equal(1, asyncLock.CurrentCount);

            using var releaser1 = await asyncLock.LockAsync(context1, token);

            Assert.Equal(0, asyncLock.CurrentCount);

            var releaser2 = asyncLock.LockAsync(context2, token);

            Assert.False(releaser2.IsCompleted);

            Assert.Equal(0, asyncLock.CurrentCount);
        }

        [Fact]
        public async Task LockAsync_ShouldReleaseLock_WhenDisposed()
        {
            // Arrange
            var options = new AsyncLockOptions
            {
                MaxDegreeOfParallelism = 1,
                HandleLockByKey = false
            };

            var asyncLock = new AsyncLock(options);

            var context = AsyncLockContext.Empty;
            var token = CancellationToken.None;

            // Act & Assert
            Assert.Equal(1, asyncLock.CurrentCount);

            using (var releaser = await asyncLock.LockAsync(context, token))
            {
                Assert.Equal(0, asyncLock.CurrentCount);
            }

            Assert.Equal(1, asyncLock.CurrentCount);
        }

        [Fact]
        public async Task LockAsync_ShouldAllowParallelLocks_UpToMaxDegreeOfParallelism()
        {
            // Arrange
            var options = new AsyncLockOptions
            {
                MaxDegreeOfParallelism = 2,
                HandleLockByKey = false
            };

            var asyncLock = new AsyncLock(options);

            var context1 = AsyncLockContext.Empty;
            var context2 = AsyncLockContext.Empty;
            var token = CancellationToken.None;

            // Act & Assert
            Assert.Equal(2, asyncLock.CurrentCount);

            using var releaser1 = asyncLock.LockAsync(context1, token);
            using var releaser2 = asyncLock.LockAsync(context2, token);

            await Task.WhenAll(releaser1, releaser2);

            Assert.Equal(0, asyncLock.CurrentCount);
        }

        [Fact]
        public async Task LockAsync_HandleLocksByDifferentKeys_ShouldAcquireLock()
        {
            // Arrange
            var options = new AsyncLockOptions
            {
                MaxDegreeOfParallelism = 2,
                HandleLockByKey = true,
                LockKeyHandler = context => context.Items["key"]
            };

            var asyncLock = new AsyncLock(options);

            var context1 = new AsyncLockContext { ["key"] = "lock1" };
            var context2 = new AsyncLockContext { ["key"] = "lock2" };

            var token = CancellationToken.None;

            // Act & Assert
            Assert.Equal(2, asyncLock.CurrentCount);

            using var releaser1 = await asyncLock.LockAsync(context1, token);

            Assert.Equal(1, asyncLock.CurrentCount);

            using var releaser2 = await asyncLock.LockAsync(context2, token);

            Assert.Equal(0, asyncLock.CurrentCount);
        }

        [Fact]
        public async Task LockAsync_HandleLocksBySameKeys_ShouldNotAcquireLock()
        {
            // Arrange
            var options = new AsyncLockOptions
            {
                MaxDegreeOfParallelism = 2,
                HandleLockByKey = true,
                LockKeyHandler = context => context.Items["key"]
            };

            var asyncLock = new AsyncLock(options);

            var context1 = new AsyncLockContext { ["key"] = "lock1" };
            var context2 = new AsyncLockContext { ["key"] = "lock1" };

            var token = CancellationToken.None;

            // Act & Assert
            Assert.Equal(2, asyncLock.CurrentCount);

            Task releaser2;

            using (var releaser1 = await asyncLock.LockAsync(context1, token))
            {
                Assert.Equal(1, asyncLock.CurrentCount);

                releaser2 = asyncLock.LockAsync(context2, token);

                Assert.False(releaser2.IsCompleted);

                Assert.Equal(0, asyncLock.CurrentCount);
            }

            await releaser2;

            Assert.True(releaser2.IsCompleted);

            Assert.Equal(1, asyncLock.CurrentCount);
        }

        [Fact]
        public async Task LockAsync_ShouldThrowTaskCanceledException_WhenCancellationRequested()
        {
            // Arrange
            var options = new AsyncLockOptions
            {
                MaxDegreeOfParallelism = 1,
                HandleLockByKey = false
            };

            var asyncLock = new AsyncLock(options);

            var context = AsyncLockContext.Empty;
            var cts = new CancellationTokenSource();
            cts.Cancel();

            // Act & Assert
            await Assert.ThrowsAsync<TaskCanceledException>(
                async () => await asyncLock.LockAsync(context, cts.Token));
        }

        [Fact]
        public async Task LockAsync_ShouldThrowObjectDisposedException_WhenDisposed()
        {
            // Arrange
            var options = new AsyncLockOptions
            {
                MaxDegreeOfParallelism = 1,
                HandleLockByKey = false
            };

            var asyncLock = new AsyncLock(options);

            var context = AsyncLockContext.Empty;
            var token = CancellationToken.None;

            // Act
            asyncLock.Dispose();

            // Assert
            await Assert.ThrowsAsync<ObjectDisposedException>(
                async () => await asyncLock.LockAsync(context, token));
        }
    }
}
