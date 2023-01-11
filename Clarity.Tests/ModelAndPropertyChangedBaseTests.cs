using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading;

namespace Clarity.Tests
{
	[TestFixture]
	public class ModelAndPropertyChangedBaseTests
	{
		[Test]
		public void TestChangeNotificationIsFiredWhenPropertyChanges()
		{
			var testClass = new ModelTestClass();
			string property = string.Empty;
			testClass.PropertyChanged += (o, e) => property = e.PropertyName;

			testClass.FirstName = "Paul";
			Assert.AreEqual("FirstName", property);
		}

		[Test]
		public void TestRelatedPropertyIsValidatedWhenChanged()
		{
			var testClass = new ModelTestClass();
			bool fullNameChanged = false;

			testClass.MonitorChanges();

			testClass.PropertyChanged += (o, e) =>
				{
					if (e.PropertyName == "FullName")
						fullNameChanged = true;
				};

			testClass.LastName = "Rubble";
			Assert.IsTrue(fullNameChanged);
		}

		[Test]
		public void TestAllPropertiesFireHandler()
		{
			var testClass = new ModelTestClass();

			testClass.MonitorAnyChange();
			testClass.FirstName = "Barney";
			Assert.AreEqual(1, testClass.PropertyChangedCount);

			testClass.LastName = "Rubble";
			Assert.AreEqual(2, testClass.PropertyChangedCount);
		}

		[Test]
		public void TestHandlerIsFiredWhenPropertyChanges()
		{
			var testClass = new ModelTestClass();
			bool handled = false;

			testClass.OnChangeOf(() => testClass.FirstName).Execute(() => handled = true);

			testClass.FirstName = "Wilma";
			Assert.IsTrue(handled);
		}

		[Test]
		public void TestRelatedPropertyObservationIsClearedCorrectly()
		{
			var testClass = new ModelTestClass();
			bool fullNameChanged = false;

			testClass.MonitorChanges();

			testClass.PropertyChanged += (o, e) =>
			{
				if (e.PropertyName == "FullName")
					fullNameChanged = true;
			};

			testClass.LastName = "Rubble";
			Assert.IsTrue(fullNameChanged);

			testClass.ClearObservers();
			fullNameChanged = false;
			testClass.LastName = "Flintstone";
			Assert.IsFalse(fullNameChanged);
		}

		[Test]
		public void TestCollectionChangesAreMonitored()
		{
			var testClass = new ModelTestClass();

			Assert.IsFalse(testClass.CollectionHasChanged);
			testClass.List.Add("Hello");
			Assert.IsTrue(testClass.CollectionHasChanged);
		}

		[Test]
		public void TestFirstNameIsInvalidWhenItHasNoValue()
		{
			var testClass = new ModelTestClass();
			string propertyName = string.Empty;
			testClass.PropertyChanged += (o, e) =>
			{
				propertyName = e.PropertyName;
			};

			Assert.IsFalse(testClass.HasErrors);
			testClass.FirstName = string.Empty;

			//force validation as WPF is not used here
			var ignored = testClass["FirstName"];

			Assert.IsFalse(testClass.IsValid());

			Assert.IsTrue(testClass.HasErrors);
			Assert.IsNotNull(Enumerable.FirstOrDefault(testClass.ErrorCollection, (e) => e == "Firstname is required"));

			Assert.IsNotNull(testClass.Error);
		}

		[Test]
		public void TestOnPropertyChangingThrowNullExceptionWhenNoPropertySpecified()
		{
			try
			{
				var testClass = new ModelTestClass();
				Thread.Sleep(500);
				testClass.PerformBadManualChange();
			}
			catch (ArgumentNullException)
			{
				Assert.True(true);
				return;
			}

			Assert.Fail();
		}
	}

	internal class ModelTestClass : ViewModel
	{
		public ModelTestClass()
		{
			FirstName = "Fred";
			LastName = "Flintstone";

			List = new ObservableCollection<string>();

			ObserveCollectionChanges();
		}

		public void MonitorChanges()
		{
			OnChangeOf(() => FirstName, () => LastName).Validate(() => FullName);
		}

		public void MonitorAnyChange()
		{
			base.OnAnyChange<ModelTestClass>().Execute(() => PropertyChangedCount++);
		}

		public int PropertyChangedCount { get; set; }

		protected override void OnCollectionChanged(string propertyName, IList originalItems, IList newItems, IList removedItems)
		{
			base.OnCollectionChanged(propertyName, originalItems, newItems, removedItems);
			CollectionHasChanged = true;
		}

		public bool CollectionHasChanged
		{
			get;
			set;
		}

		private string _firstName;
		public virtual string FirstName
		{
			get
			{
				return _firstName;
			}

			set
			{
				SetValue(ref _firstName, value, () => FirstName);
			}
		}

		private string _lastName;
		[Required]
		public virtual string LastName
		{
			get
			{
				return _lastName;
			}

			set
			{
				SetValue(ref _lastName, value, () => LastName);
			}
		}

		public string FullName
		{
			get
			{
				return FirstName + LastName;
			}
		}

		private int _value;
		public virtual int Value
		{
			get
			{
				return _value;
			}

			set
			{
				SetValue(ref _value, value, () => Value);
			}
		}

		private ObservableCollection<string> _list;
		public virtual ObservableCollection<string> List
		{
			get
			{
				return _list;
			}

			set
			{
				SetValue(ref _list, value, () => List);
			}
		}

		public override ValidationResults OnValidate(string propertyName)
		{
			var results = base.OnValidate(propertyName);
			if (propertyName == this.GetPropertyName(() => FirstName) && string.IsNullOrEmpty(FirstName))
			{
				results.Add("Firstname is required", () => FirstName);
			}

			return results;
		}

		internal void PerformBadManualChange()
		{
			OnPropertyChanging(null, FirstName, "Test");
		}
	}
}