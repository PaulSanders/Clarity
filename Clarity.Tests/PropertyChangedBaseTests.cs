using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Threading;

namespace Clarity.Tests
{
	[TestFixture]
	public class PropertyChangedBaseTests
	{
		private int _threadId;

		[TestFixtureSetUp]
		public void SetupContext()
		{
			SynchronizationContext.SetSynchronizationContext(new SynchronizationContext());
			_threadId = Thread.CurrentThread.ManagedThreadId;
		}

		[Test]
		public void TestSettingFirstNameUpdatesValue()
		{
			var test = new PropertyChangedBaseTest();
			test.FirstName = "Paul";

			Assert.AreEqual("Paul", test.FirstName);
		}

		[Test]
		public void TestSettingFirstNameFiresNotifyPropertyChanged()
		{
			var test = new PropertyChangedBaseTest();
			string property = null;
			test.PropertyChanged += (o, e) =>
			{
				property = e.PropertyName;
			};

			test.FirstName = "Paul";
			Assert.AreEqual("FirstName", property);

			test.LastName = "Sanders";
			Assert.AreEqual("LastName", property);
		}

		[Test]
		public void TestSettingOldGitFiresActionWhenChanged()
		{
			var test = new PropertyChangedBaseTest();
			test.Description = string.Empty;

			test.IsOldGit = true;
			Assert.AreEqual("You old git!", test.Description);

			test.IsOldGit = false;
			Assert.AreEqual("Go back to school", test.Description);
		}

		[Test]
		public void TestChangeMonitoringFiresPropertyChangedForFullName()
		{
			var test = new PropertyChangedBaseTest();
			int changeCount = 0;

			test.PropertyChanged += (o, e) =>
				{
					if (e.PropertyName == "FirstName" || e.PropertyName == "LastName") changeCount++;
				};

			test.AutoUpdateFullName();
			test.FirstName = "Paul";

			Assert.AreEqual(1, changeCount);

			test.LastName = "Sanders";

			Assert.AreEqual(2, changeCount);
		}

		[Test]
		public void TestChangeMonitoringFiresPropertyChangedForFullNameWhenMonitoringAnyChange()
		{
			var test = new PropertyChangedBaseTest();
			int changeCount = 0;

			test.PropertyChanged += (o, e) =>
			{
				if (e.PropertyName == "FirstName" || e.PropertyName == "LastName") changeCount++;
			};

			test.AutoUpdateOnAnyChange();
			test.FirstName = "Paul";

			Assert.AreEqual(1, changeCount);

			test.LastName = "Sanders";

			Assert.AreEqual(2, changeCount);
		}

		[Test]
		public void TestCollectionChangesAreMonitored()
		{
			var test = new PropertyChangedBaseTest();

			test.TrackNameCollectionChanges();

			test.Names.Add("Name1");
			Assert.AreEqual("Names", test.CollectionThatHasChanged);
		}

		[Test]
		public void TestCollectionChangesAreMonitoredAfterCallingObserve()
		{
			var test = new PropertyChangedBaseTest();

			test.Names.Add("Name1");
			Assert.IsNull(test.CollectionThatHasChanged);

			test.Names.Add("Name2");
			Assert.AreEqual(null, test.CollectionThatHasChanged);

			test.TrackAllCollectionChanges();
			test.Names.Add("Name3");
			Assert.AreEqual("Names", test.CollectionThatHasChanged);

			test.Numbers.Add(1);
			Assert.AreEqual("Numbers", test.CollectionThatHasChanged);

			test.Numbers.Add(1);
			Assert.AreEqual("Numbers", test.CollectionThatHasChanged);
		}

		[Test]
		public void TestDelayedActionIsFiredAfterGivenTime()
		{
			var test = new PropertyChangedBaseTest();
			var mre = new ManualResetEvent(false);

			bool searchFired = false;

			test.OnChangeOf(() => test.SearchText).ExecuteAfterDelay(() => searchFired = true, TimeSpan.FromSeconds(2));

			test.SearchText = "Pa";
			Thread.Sleep(1000);
			Assert.IsFalse(searchFired);
			Thread.Sleep(1100);
			Assert.IsTrue(searchFired);
		}

		[Test]
		public void TestDelayedActionIsFiredOnceChangesHaveStoppedBeingMade()
		{
			var test = new PropertyChangedBaseTest();
			var mre = new ManualResetEvent(false);

			int searchFiredCount = 0;

			test.OnChangeOf(() => test.SearchText).ExecuteAfterDelay(() => searchFiredCount++, TimeSpan.FromSeconds(2));

			test.SearchText = "Pa";
			Thread.Sleep(1000);
			Assert.AreEqual(0, searchFiredCount);
			test.SearchText = "Paul";
			Thread.Sleep(1100);
			Assert.AreEqual(0, searchFiredCount);
			Thread.Sleep(1100);
			Assert.AreEqual(1, searchFiredCount);
		}

		[Test]
		public void TestDelayedActionIsFiredForEachActionThatIsLongerThanDelay()
		{
			var test = new PropertyChangedBaseTest();
			var mre = new ManualResetEvent(false);

			int searchFiredCount = 0;

			test.OnChangeOf(() => test.SearchText).ExecuteAfterDelay(() => searchFiredCount++, TimeSpan.FromSeconds(1));

			test.SearchText = "He";
			Thread.Sleep(500);
			Assert.AreEqual(0, searchFiredCount);
			test.SearchText = "Hello";
			Thread.Sleep(1100);
			Assert.AreEqual(1, searchFiredCount);

			test.SearchText = "World";
			Thread.Sleep(1200);
			Assert.AreEqual(2, searchFiredCount);
		}
	}

	internal class PropertyChangedBaseTest : PropertyChangedBase
	{
		public PropertyChangedBaseTest()
		{
			_names = new ObservableCollection<string>();
			_numbers = new ObservableCollection<int>();
			IsOldGit = false;
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

		public void AutoUpdateFullName()
		{
			OnChangeOf(() => FirstName, () => LastName).Validate(() => FullName);
		}

		public void AutoUpdateOnAnyChange()
		{
			OnAnyChange<PropertyChangedBaseTest>().Execute(() => NotifyPropertyChanged(() => FullName));
		}

		public string FullName
		{
			get
			{
				return _firstName + _lastName;
			}
		}

		private ObservableCollection<string> _names;
		public virtual ObservableCollection<string> Names
		{
			get
			{
				return _names;
			}

			set
			{
				SetValue(ref _names, value, () => Names);
			}
		}

		private ObservableCollection<int> _numbers;
		public virtual ObservableCollection<int> Numbers
		{
			get
			{
				return _numbers;
			}

			set
			{
				SetValue(ref _numbers, value, () => Numbers);
			}
		}

		public void TrackNameCollectionChanges()
		{
			ObserveCollection(() => Names);
		}

		public void TrackAllCollectionChanges()
		{
			ObserveCollectionChanges();
		}

		protected override void OnCollectionChanged(string propertyName, IList originalItems, IList newItems, IList removedItems)
		{
			CollectionThatHasChanged = propertyName;
		}

		private string _collectionThatHasChanged;
		public virtual string CollectionThatHasChanged
		{
			get
			{
				return _collectionThatHasChanged;
			}

			set
			{
				SetValue(ref _collectionThatHasChanged, value, () => CollectionThatHasChanged);
			}
		}

		private bool _isOldGit;
		public virtual bool IsOldGit
		{
			get
			{
				return _isOldGit;
			}

			set
			{
				SetValue(ref _isOldGit, value, () => IsOldGit, () => Description = _isOldGit ? "You old git!" : "Go back to school");
			}
		}

		private string _description;
		public virtual string Description
		{
			get
			{
				return _description;
			}

			set
			{
				SetValue(ref _description, value, () => Description);
			}
		}

		private string _searchText;
		public virtual string SearchText
		{
			get
			{
				return _searchText;
			}

			set
			{
				SetValue(ref _searchText, value, () => SearchText);
			}
		}
	}
}