namespace Magnum.Common.Specs.Threading
{
    using Common.Threading;
    using MbUnit.Framework;

    [TestFixture]
    public class UpgradeableLock_Specs
    {
        [Test]
        public void Go_straight_to_write_lock()
        {
            var ulock = new UpgradeableLock();
            using (ulock.EnterWriteLock())
            {
                Assert.IsTrue(ulock.IsWriteLockHeld);
            }
        }

        [Test]
        public void Should_be_upgradeable()
        {
            var ulock = new UpgradeableLock();
            Assert.IsFalse(ulock.IsUpgradeableReadLockHeld);

            using (UpgradeableLock.EnterUpgradableReader token = ulock.EnterUpgradableRead())
            {
                using (token.Upgrade())
                {
                    Assert.IsTrue(ulock.IsWriteLockHeld);
                }
                Assert.IsFalse(ulock.IsWriteLockHeld);
            }
        }

        [Test]
        public void Read_only()
        {
            var ulock = new UpgradeableLock();
            using (ulock.EnterReadOnlyLock())
            {
                Assert.IsTrue(ulock.IsReadOnlyLockHeld);
            }
            Assert.IsFalse(ulock.IsReadOnlyLockHeld);
        }
    }
}