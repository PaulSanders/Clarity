using NUnit.Framework;

namespace Clarity.Tests
{
    [TestFixture]
    public class WindowResultTests
    {
        [Test]
        public void TestOkResultEqualityWithOkResultIsSuccessfull()
        {
            Assert.AreEqual(new OkResult(), new OkResult());
        }

        [Test]
        public void TestYesResultEqualityWithNoResultIsSuccessfull()
        {
            Assert.AreNotEqual(new YesResult(), new NoResult());
        }

        [Test]
        public void TestCancelResultEqualityWithCancelResultIsSuccessfull()
        {
            Assert.IsTrue(new CancelResult().Equals(new CancelResult()));
        }

        [Test]
        public void TestOkResultWithNullReturnsFalse()
        {
            Assert.IsFalse(new CancelResult().Equals(null));
        }

        [Test]
        public void TestNullResultsReturnTrue()
        {
            OkResult a = null;
            OkResult b = null;

            Assert.IsTrue(a == b);
        }

        [Test]
        public void TestOkResultWithDifferentTypeReturnsFalse()
        {
            Assert.IsFalse(new CancelResult().Equals("Cancel"));
        }

        [Test]
        public void TestOperatorEqualityReturnsTrue()
        {
            Assert.IsTrue(new OkResult() == new OkResult());
        }

        [Test]
        public void TestNotOperatorEqualityReturnsFalse()
        {
            Assert.IsFalse(new OkResult() != new OkResult());
        }

        [Test]
        public void TestNotOperatorEqualityReturnsTrue()
        {
            Assert.IsTrue(new OkResult() != new YesResult());
        }
    }
}
