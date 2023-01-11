using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Clarity.Tests
{
	[TestFixture]
	public class DefaultViewLocatorTests
	{
		[Test]
		[ExpectedException(typeof(ArgumentNullException))]
		public void WhenNoViewModelSpecified_ThrowsException()
		{
			var lv = new DefaultViewLocator();
			var view = lv.LocateView<LocatableView>(null, this.GetType().Assembly);
		}

		[Test]
		[ExpectedException(typeof(ArgumentNullException))]
		public void WhenNoAssemblySpecified_ThrowsException()
		{
			var lv = new DefaultViewLocator();
			var view = lv.LocateView<LocatableView>(typeof(LocatableViewModel), null);
		}

		[Test]
		[ExpectedException(typeof(Exception))]
		public void WhenNoViewDefined_ThrowsException()
		{
			var lv = new DefaultViewLocator();
			var view = lv.LocateView(typeof(ViewlessViewModel), this.GetType().Assembly);
		}

		[Test]
		public void WhenPassedValidViewModelAndView_Returns_View()
		{
			var lv = new DefaultViewLocator();
			var view = lv.LocateView<LocatableView>(typeof(LocatableViewModel), this.GetType().Assembly);

			Assert.IsNotNull(view);
			Assert.AreEqual(typeof(LocatableView), view.GetType());
		}
	}

	public class LocatableViewModel : ViewModel
	{
		public LocatableViewModel()
		{
			Items = new List<int>();
		}

		public List<int> Items { get; set; }
	}

	public class ViewlessViewModel:ViewModel
	{

	}

	public class LocatableView : PropertyChangedBase
	{

	}
}
