using System;
using NSubstitute;
using NUnit.Framework;

namespace Clarity.Tests
{
	[TestFixture]
	public class DisposableTests
	{
		[Test]
		public void TestDisposeEventFiresWhenDisposing()
		{
			var item = new DisposableTest();
			item.Dispose();

			Assert.IsTrue(item.IsDisposed);
		}

		[Test]
		public void TestObjectIdIsUnique()
		{
			var item1 = new DisposableTest();
			var item2 = new DisposableTest();

			Assert.AreNotEqual(item1.ObjectId, item2.ObjectId);
		}

		[Test]
		public void TestCanBeUsed()
		{
			var item1 = new DisposableTest();
			item1.DoSomething();
			Assert.Pass();
		}

		[Test]
		[ExpectedException(typeof(ObjectDisposedException))]
		public void TestCanNotBeUsed()
		{
			var item1 = new DisposableTest();
			item1.Dispose();
			item1.DoSomething();
		}

		[Test]
		public void TestMessageBusIsRegisterdWhenInDispose()
		{
			ServiceManager.Default.Register<IMessageBus, MessageBus>();

			var item = new DisposableTest();

			Assert.IsTrue(item.IsMessageBusAvailable());
			item.Dispose();
			Assert.IsFalse(item.IsMessageBusAvailable());
		}

		[Test]
		public void TestFinalizer()
		{

			var item = new DisposableTest();
			item.Dispose();

			Assert.IsTrue(item.OnDisposedCalled);
			item = null;
			for (int i = 0; i < 20;i++ )
				System.Threading.Thread.Sleep(300);

			Assert.AreEqual(0, DisposedCount);
		}

		public static int DisposedCount;
	}

	class DisposableTest : Disposable
	{
		public void DoSomething()
		{
			EnsureNotDisposed();
		}

		public bool IsMessageBusAvailable()
		{
			return MessageBus != null;
		}

		protected override void OnDispose()
		{
			OnDisposedCalled = true;
			base.OnDispose();
		}

		~DisposableTest()
		{
			DisposableTests.DisposedCount++;
		}

		public bool OnDisposedCalled { get; private set; }
	}
}
