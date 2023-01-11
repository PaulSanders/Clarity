using NUnit.Framework;

namespace Clarity.Tests
{
	[TestFixture]
	public class BootstrapperTests
	{
		[SetUp]
		public void Init()
		{
			ServiceManager.Default.UnregisterAll();
		}

		[Test]
		public void EnsureTheBootstrapperIsFullyInitializedAfterConstruction()
		{
			var bs = new TestBootstrapper();
			Assert.IsTrue(bs.InitializeLoggingCalled);
			Assert.IsTrue(bs.RegisterCommandBuilderCalled);
			Assert.IsTrue(bs.RegisterDefaultItemsCalled);
			Assert.IsTrue(bs.RegisterViewModelsCalled);
		}

		[Test]
		public void EnsureRegisterUsesDefaultServiceManagerToRegisterInterface()
		{
			var bs = new TestBootstrapper();
			bs.Register<IClosable, CloseableViewModelTestClass>();

			Assert.IsTrue(ServiceManager.Default.IsRegistered<IClosable>());
		}

		[Test]
		public void EnsureRegisterSingleUsesDefaultServiceManagerToRegisterInterface()
		{
			var bs = new TestBootstrapper();
			bs.RegisterSingle<IClosable, CloseableViewModelTestClass>();

			Assert.IsTrue(ServiceManager.Default.IsRegistered<IClosable>());
		}

		[Test]
		public void EnsureRegisterUsesDefaultServiceManagerToRegisterClass()
		{
			var bs = new TestBootstrapper();
			bs.Register<CloseableViewModelTestClass>();

			Assert.IsTrue(ServiceManager.Default.IsRegistered<CloseableViewModelTestClass>());
		}

		[Test]
		public void EnsureRegisterSingleUsesDefaultServiceManagerToRegisterClass()
		{
			var bs = new TestBootstrapper();
			bs.RegisterSingle(new CloseableViewModelTestClass());

			Assert.IsTrue(ServiceManager.Default.IsRegistered<CloseableViewModelTestClass>());
		}

		[Test]
		public void EnsureRegisterUsesDefaultServiceManagerToRegisterFactory()
		{
			var bs = new TestBootstrapper();
			bs.Register<IClosable>(() => new CloseableViewModelTestClass());

			Assert.IsTrue(ServiceManager.Default.IsRegistered<IClosable>());
		}
	}

	public class TestBootstrapper : Bootstrapper
	{
		public bool InitializeLoggingCalled { get; set; }

		protected override void OnInitialiseLogging()
		{
			base.OnInitialiseLogging();
			InitializeLoggingCalled = true;
		}

		public bool RegisterCommandBuilderCalled { get; set; }

		protected override void OnRegisterCommandBuilder()
		{
			base.OnRegisterCommandBuilder();
			RegisterCommandBuilderCalled = true;
		}

		public bool RegisterDefaultItemsCalled { get; set; }

		protected override void OnRegisterDefaultItems()
		{
			base.OnRegisterDefaultItems();
			RegisterDefaultItemsCalled = true;
		}

		public bool RegisterViewModelsCalled { get; set; }

		protected override void OnRegisterViewModels()
		{
			base.OnRegisterViewModels();
			RegisterViewModelsCalled = true;
		}
	}
}