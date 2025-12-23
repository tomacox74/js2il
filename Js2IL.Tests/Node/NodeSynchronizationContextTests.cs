using System;
using JavaScriptRuntime.EngineCore;
using Xunit;

namespace Js2IL.Tests.Node
{
    public class NodeSynchronizationContextTests
    {
        /// <summary>
        /// Regression test for the bug where canceled intervals would cause the event loop to hang.
        /// 
        /// The bug: When clearInterval was called, the canceled timer remained in the timer queue
        /// until it became "due". HasPendingWork() would return true (timer in queue), but
        /// TryPromoteDueTimerToMacro wouldn't remove it until due time. This caused infinite loops.
        /// 
        /// The fix: TryPromoteDueTimerToMacro now removes canceled intervals from the front of
        /// the queue immediately, even if they're not due yet.
        /// </summary>
        [Fact]
        public void CanceledInterval_DoesNotCauseInfiniteLoop()
        {
            // Arrange
            var mockTickSource = new MockTickSource();
            var waitCount = 0;
            var mockWaitHandle = new MockWaitHandle(
                onSet: () => { },
                onWaitOne: (ms) =>
                {
                    waitCount++;
                    // Simulate time passing on each wait
                    mockTickSource.Increment(TimeSpan.FromMilliseconds(ms));
                    
                    // Safety: fail test if we've waited too many times (indicates infinite loop)
                    if (waitCount > 100)
                    {
                        throw new Exception("Test detected infinite loop - too many waits");
                    }
                });

            var ctx = new NodeSychronizationContext(mockTickSource, mockWaitHandle);
            var scheduler = (IScheduler)ctx;

            var callbackCount = 0;
            object? intervalHandle = null;

            // Schedule an interval that cancels itself after first execution
            intervalHandle = scheduler.ScheduleInterval(() =>
            {
                callbackCount++;
                if (callbackCount >= 1)
                {
                    // Cancel the interval - this is where the bug would manifest
                    scheduler.CancelInterval(intervalHandle!);
                }
            }, TimeSpan.FromMilliseconds(100));

            // Act - Run the event loop
            // Before the fix, this would hang forever because:
            // 1. First callback runs and cancels the interval
            // 2. But the NEXT scheduled timer entry is already in the queue (due in 100ms)
            // 3. HasPendingWork() returns true (timer in queue)
            // 4. TryPromoteDueTimerToMacro doesn't remove it (not due yet)
            // 5. Loop waits, but timer is canceled so nothing happens when it becomes due
            // 6. But wait - actually the timer WOULD be removed when due... let me trace more carefully

            // Actually the more insidious case is when the canceled timer is NOT at the front
            // or when we're checking HasPendingWork before the timer is due.
            // Let's simulate the event loop:
            
            int iterations = 0;
            while (ctx.HasPendingWork() && iterations < 50)
            {
                ctx.RunOneIteration();
                ctx.WaitForWorkOrNextTimer();
                iterations++;
            }

            // Assert
            Assert.Equal(1, callbackCount); // Callback should have run exactly once
            Assert.False(ctx.HasPendingWork()); // No pending work should remain
            Assert.True(iterations < 50, $"Event loop took too many iterations ({iterations}), possible bug");
        }

        /// <summary>
        /// Test that a canceled interval that's scheduled far in the future
        /// is removed immediately, not waiting until it becomes due.
        /// </summary>
        [Fact]
        public void CanceledInterval_FarInFuture_RemovedImmediately()
        {
            // Arrange
            var mockTickSource = new MockTickSource();
            var mockWaitHandle = new MockWaitHandle(
                onSet: () => { },
                onWaitOne: (ms) =>
                {
                    // Don't advance time - we want to test that the canceled interval
                    // is removed even though it's not due yet
                });

            var ctx = new NodeSychronizationContext(mockTickSource, mockWaitHandle);
            var scheduler = (IScheduler)ctx;

            // Schedule an interval 1 hour in the future
            var handle = scheduler.ScheduleInterval(() => { }, TimeSpan.FromHours(1));
            
            Assert.True(ctx.HasPendingWork()); // Timer is pending

            // Cancel it
            scheduler.CancelInterval(handle);

            // Act - Run one iteration (this should clean up the canceled interval)
            ctx.RunOneIteration();

            // Assert - HasPendingWork should now be false because the canceled interval
            // was removed even though it wasn't due yet
            Assert.False(ctx.HasPendingWork());
        }

        /// <summary>
        /// Test that multiple intervals can be canceled and the event loop exits cleanly.
        /// </summary>
        [Fact]
        public void MultipleIntervals_AllCanceled_EventLoopExits()
        {
            // Arrange
            var mockTickSource = new MockTickSource();
            var mockWaitHandle = new MockWaitHandle(
                onSet: () => { },
                onWaitOne: (ms) => mockTickSource.Increment(TimeSpan.FromMilliseconds(ms)));

            var ctx = new NodeSychronizationContext(mockTickSource, mockWaitHandle);
            var scheduler = (IScheduler)ctx;

            var handles = new object[3];
            var counts = new int[3];

            // Schedule 3 intervals
            for (int i = 0; i < 3; i++)
            {
                int index = i;
                handles[i] = scheduler.ScheduleInterval(() => counts[index]++, TimeSpan.FromMilliseconds(50d * (i + 1)));
            }

            // Cancel all of them immediately
            foreach (var handle in handles)
            {
                scheduler.CancelInterval(handle);
            }

            // Act
            int iterations = 0;
            while (ctx.HasPendingWork() && iterations < 10)
            {
                ctx.RunOneIteration();
                iterations++;
            }

            // Assert
            Assert.False(ctx.HasPendingWork());
            Assert.Equal(0, counts[0]); // No callbacks should have run
            Assert.Equal(0, counts[1]);
            Assert.Equal(0, counts[2]);
        }
    }
}
