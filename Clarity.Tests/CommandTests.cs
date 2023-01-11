using Clarity.Commands;
using NUnit.Framework;
using System;

namespace Clarity.Tests
{
	[TestFixture]
	internal class CommandTests
	{
		[Test]
		[ExpectedException(typeof(ArgumentNullException))]
		public void TestConstructionFailsWithNullAction()
		{
			var cmd = new SimpleCommand(null);
		}

		[Test]
		[ExpectedException(typeof(ArgumentNullException))]
		public void TestConstructionOfDelegateFailsWithNullAction()
		{
			var cmd = new DelegateCommand<int>(null);
		}

		[Test]
		[ExpectedException(typeof(ArgumentNullException))]
		public void TestConstructionFailsWithNullActionAndCanExecute()
		{
			var cmd = new SimpleCommand(null, null);
		}

		[Test]
		[ExpectedException(typeof(ArgumentNullException))]
		public void TestConstructionOfDelegateFailsWithNullActionAndCanExecute()
		{
			var cmd = new DelegateCommand<int>(null, null);
		}

		[Test]
		[ExpectedException(typeof(ArgumentNullException))]
		public void TestConstructionFailsWithNullCanExecute()
		{
			var cmd = new SimpleCommand(() => Logger.Debug("not executed"), null);
		}

		[Test]
		[ExpectedException(typeof(ArgumentNullException))]
		public void TestConstructionOfDelegateFailsWithNullCanExecute()
		{
			var cmd = new DelegateCommand<int>((i) => Logger.Debug("not executed"), null);
		}

		[Test]
		public void TestConstructionWithValidArgumentsIsValid()
		{
			bool executed = false;
			var cmd = new SimpleCommand(() => executed = true, () => true);
			Assert.IsNotNull(cmd);
			Assert.IsFalse(executed);
		}

		[Test]
		public void TestConstructionOfDelegateWithValidArgumentsIsValid()
		{
			bool executed = false;
			var cmd = new DelegateCommand<int>((i) => executed = true, (i) => true);
			Assert.IsNotNull(cmd);
			Assert.IsFalse(executed);
		}

		[Test]
		public void TestIsBusyIsTrueWhenExecuting()
		{
			bool executed = false;
			bool? wasBusy = null;
			var action = new Action(() => executed = true);
			var cmd = new SimpleCommand(action);

			cmd.OnChangeOf(() => cmd.IsBusy).Execute(() =>
			{
				if (wasBusy == null) wasBusy = cmd.IsBusy;
			});
			Assert.IsNotNull(cmd);
			cmd.Execute(null);

			Assert.IsTrue(executed);
			Assert.IsTrue(wasBusy.Value);
		}

		[Test]
		public void TestDelegateIsBusyIsTrueWhenExecuting()
		{
			bool executed = false;
			bool? wasBusy = null;
			var cmd = new DelegateCommand<int>((i) => executed = true);

			cmd.OnChangeOf(() => cmd.IsBusy).Execute(() =>
			{
				if (wasBusy == null) wasBusy = cmd.IsBusy;
			});

			Assert.IsNotNull(cmd);
			cmd.Execute(1);

			Assert.IsTrue(executed);
			Assert.IsTrue(wasBusy.Value);
		}
	}
}