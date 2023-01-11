using NUnit.Framework;
using System;
using System.Collections.ObjectModel;
using System.Linq;

namespace Clarity.Tests
{
	[TestFixture]
	public class ExtensionsTests
	{
		[Test]
		[ExpectedException(typeof(ArgumentNullException))]
		public void AddRange_WhenCollectionIsNull_ThrowsArgumentNullException()
		{
			var col = new ObservableCollection<string>();
			Extensions.AddRange<string>(null, null);
		}

		[Test]
		public void AddRange_WhenItemsIsNull_NoChangesMade()
		{
			var col = new ObservableCollection<string>();
			Extensions.AddRange(col, null);
		}

		[Test]
		public void AddRange_WhenItemsIsEmpty_NoChangesMade()
		{
			var col = new ObservableCollection<string>();
			string[] items = new string[] { };
			Extensions.AddRange(col, items);
		}

		[Test]
		public void AddRange_WhenAddingItems_AllItemsAreAdded()
		{
			var col = new ObservableCollection<string>();
			col.Add("a");

			string[] items = new string[] { "b", "c", "d", "e" };

			Assert.AreEqual(1, col.Count);
			Extensions.AddRange(col, items);
			Assert.AreEqual(5, col.Count);
		}

		[Test]
		public void GetAttributes_WhenMemberIsNull_ReturnsEmptyEnumerable()
		{
			var results = Extensions.GetAttributes<TestAttribute>(null, false);

			Assert.IsFalse(results.Any());
		}

		[Test]
		public void GetAttributes_WhenMemberHasAttribute_ReturnsEnumerable()
		{
			var type = this.GetType();
			var method = type.GetMethod("GetAttributes_WhenMemberIsNull_ReturnsEmptyEnumerable");
			var results = method.GetAttributes<TestAttribute>(false);

			Assert.AreEqual(1, results.Count());
		}

		[Test]
		[ExpectedException(typeof(ArgumentNullException))]
		public void IfNullThrow_WhenParameterIsNull_ThrowsArgumentNullException()
		{
			object test = null;

			test.IfNullThrow("test");
		}

		[Test]
		[ExpectedException(typeof(ArgumentNullException))]
		public void IfNullThrow_WhenParameterIsEmpty_ThrowsArgumentNullException()
		{
			string test = "";

			test.IfNullThrow("test");
		}

		[Test]
		[ExpectedException(typeof(ArgumentNullException))]
		public void IfNullThrow_WhenParameterNameIsNull_ThrowsArgumentNullException()
		{
			string arg = "test";
			arg.IfNullThrow(null);
		}

		[Test]
		[ExpectedException(typeof(ArgumentNullException))]
		public void IfNullThrow_WhenParameterNameIsEmpty_ThrowsArgumentNullException()
		{
			string arg = "test";
			arg.IfNullThrow(string.Empty);
		}

		[Test]
		[ExpectedException(typeof(ArgumentNullException))]
		public void GetProperty_WhenObjectIsNull_ThrowsArgumentNullException()
		{
			ViewModel vm = null;
			vm.GetProperty<string>("Title");
		}

		[Test]
		[ExpectedException(typeof(ArgumentNullException))]
		public void GetProperty_WhenPassedNull_ThrowsArgumentNullException()
		{
			var vm = new LocatableViewModel();
			vm.GetProperty<string>(null);
		}

		[Test]
		[ExpectedException(typeof(ArgumentException))]
		public void GetProperty_WhenGivenInvalidPropertyName_ThrowsArgumentNullException()
		{
			var vm = new LocatableViewModel();
			vm.GetProperty<string>("InvalidPropertyName");
		}

		[Test]
		public void GetProperty_WhenPassedValidProperty_ReturnsExpectedValue()
		{
			var vm = new LocatableViewModel();
			vm.Title = "Test";

			var value = vm.GetProperty<string>("Title");

			Assert.AreEqual(vm.Title, value);
		}

		[Test]
		[ExpectedException(typeof(ArgumentException))]
		public void GetProperty_WhenPassedNonReadableProperty_ThrowsArgumentException()
		{
			var obj = new ExtensionTester();

			var value = obj.GetProperty<Guid>("UnreadableId");
		}


		[Test]
		[ExpectedException(typeof(ArgumentNullException))]
		public void SetProperty_WhenObjectIsNull_ThrowsArgumentNullException()
		{
			ViewModel vm = null;
			vm.SetProperty("Title","test");
		}

		[Test]
		[ExpectedException(typeof(ArgumentException))]
		public void SetProperty_WhenGivenInvalidPropertyName_ThrowsArgumentException()
		{
			var vm = new LocatableViewModel();
			vm.SetProperty("InvalidPropertyName", null);
		}

		[Test]
		[ExpectedException(typeof(ArgumentException))]
		public void SetProperty_WhenGivenReadonlyPropertyName_ThrowsArgumentException()
		{
			var ex = new ExtensionTester();
			ex.SetProperty("UnwritableName", "Wilma");
		}

		[Test]
		public void SetProperty_WhenPassedValidProperty_ValueIsSet()
		{
			var vm = new LocatableViewModel();
			vm.Title = "Test";

			vm.SetProperty("Title", "NewTitle");

			Assert.AreEqual("NewTitle", vm.Title);
		}

		[Test]
		public void GetPropertyNameWithUnaryExpression()
		{
			var vm = new LocatableViewModel();
			var name = vm.GetPropertyName(() => vm.Items.Count);
			Assert.AreEqual("Count", name);
		}
	}

	class ExtensionTester
	{
		private Guid _unreadableId;
		public virtual Guid UnreadableId
		{
			set
			{
				_unreadableId = value;
			}
		}

		public string UnwritableName
		{
			get
			{
				return "Fred";
			}
		}
	}
}
