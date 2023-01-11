using System;
using System.ComponentModel;
using NUnit.Framework;

namespace Clarity.Tests
{
    [TestFixture]
    public class ServiceManagerTests
    {
        [SetUp]
        public void TestInit()
        {
            ServiceManager.Default.UnregisterAll();
        }

        [Test]
        public void TestRegisterInstanceResolvesTheSameInstance()
        {
            var instance = new TestClass();

            ServiceManager.Default.RegisterSingle<ITestInterface>(instance);
            var newInstance = ServiceManager.Default.Resolve<ITestInterface>();

            Assert.AreSame(instance, newInstance);
        }

        [Test]
        [ExpectedException(typeof(InvalidOperationException))]
        public void TestRegisterInterfaceAndWrongImplementationThrowsException()
        {
            ServiceManager.Default.Register(typeof(INotifyPropertyChanged), typeof(TestClass));
        }

        [Test]
        [ExpectedException(typeof(Exception))]
        public void TestRegisterTwoInstancesOfSameTypeThrowsException()
        {
            var instance1 = new TestClass();
            var instance2 = new TestClass();

            ServiceManager.Default.RegisterSingle(instance1);
            ServiceManager.Default.RegisterSingle(instance2);
        }

        [Test]
        [ExpectedException(typeof(Exception))]
        public void TestRegisterTwoImplementationsOfSameTypeThrowsException()
        {
            ServiceManager.Default.Register<TestClass>();
            ServiceManager.Default.Register<TestClass>();
        }

        [Test]
        [ExpectedException(typeof(Exception))]
        public void TestRegisterInterfaceThrowsErrorWhenRegisteringTheSameInterface()
        {
            ServiceManager.Default.Register<ITestInterface, TestClass>();
            ServiceManager.Default.Register(typeof(ITestInterface), typeof(TestClass));
        }

        [Test]
        [ExpectedException(typeof(Exception))]
        public void TestRegisterInstanceThrowsErrorWhenRegisteringTheSameInterface()
        {
            var instance = new TestClass();
            ServiceManager.Default.Register<ITestInterface, TestClass>();
            ServiceManager.Default.RegisterSingle<ITestInterface>(instance);
        }

        [Test]
        [ExpectedException(typeof(Exception))]
        public void TestResolveOnTypeThrowsErrorIfNotRegistered()
        {
            ServiceManager.Default.Resolve(typeof(ITestInterface));
        }

        [Test]
        public void TestResolveOnNonGenericTypeReturnsObject()
        {
            ServiceManager.Default.RegisterSingle<ITestInterface>(new TestClass());
            var instance = ServiceManager.Default.Resolve(typeof(ITestInterface));
            Assert.IsNotNull(instance);
        }

        [Test]
        public void TestResolveOnNonGenericTypeWithSingleInstanceReturnsObject()
        {
            ServiceManager.Default.RegisterSingle<ITestInterface>(new TestClass());
            var instance = ServiceManager.Default.Resolve(typeof(ITestInterface));
            Assert.IsNotNull(instance);
        }

        [Test]
        public void TestAInterfaceIsNotRegistered()
        {
            Assert.IsFalse(ServiceManager.Default.IsRegistered<ITestInterface>());
        }

        [Test]
        public void TestAInterfaceIsRegistered()
        {
            ServiceManager.Default.Register<ITestInterface, TestClass>();
            Assert.IsTrue(ServiceManager.Default.IsRegistered<ITestInterface>());
        }

        [Test]
        public void TestRegisterInstanceReturnsTheSameInstance()
        {
            var instance = new TestClass();

            ServiceManager.Default.RegisterSingle(instance);
            var newInstance = ServiceManager.Default.Resolve<TestClass>();

            Assert.AreSame(instance, newInstance);
        }

        [Test]
        public void TestRegisterUsingFactoryCallsTheFactoryMethodToResolve()
        {
            var instance = new TestClass();

            ServiceManager.Default.Register<ITestInterface>(() => instance);

            var newInstance = ServiceManager.Default.Resolve<ITestInterface>();

            Assert.AreSame(instance, newInstance);
        }

        [Test]
        public void TestRegisterUsingTypeReturnsCorrectType()
        {
            var instance = new TestClass();

            ServiceManager.Default.Register(typeof(ITestInterface), instance);

            var newInstance = ServiceManager.Default.Resolve<ITestInterface>();

            Assert.AreSame(instance, newInstance);
        }

        [Test]
        public void TestRegisterSingleReturnsSameInstance()
        {
            var instance = new TestClass();

            ServiceManager.Default.RegisterSingle(instance);

            var newInstance = ServiceManager.Default.Resolve<TestClass>();

            Assert.AreSame(instance, newInstance);
        }

        [Test]
        public void TestRegisterSingleForTypeReturnsSameInstance()
        {
            ServiceManager.Default.RegisterSingle<TestClass>();

            var instance1 = ServiceManager.Default.Resolve<TestClass>();
            var instance2 = ServiceManager.Default.Resolve<TestClass>();

            Assert.AreSame(instance1, instance2);
        }

        [Test]
        public void TestRegisterSingleWithUntypedValueReturnsCorrectInstance()
        {
            var vm=new LocatableViewModel();
            ServiceManager.Default.RegisterSingle(vm);

            var resolvedVm = ServiceManager.Default.Resolve<LocatableViewModel>();
            Assert.IsNotNull(resolvedVm);
            Assert.AreEqual(typeof(LocatableViewModel), resolvedVm.GetType());
        }

        [Test]
        [ExpectedException(typeof(Exception))]
        public void TestRegisterSingleThrowsExceptionWhenTheSameTypeIsRegistered()
        {
            ServiceManager.Default.RegisterSingle(new LocatableViewModel());
            ServiceManager.Default.RegisterSingle(new LocatableViewModel());
        }

        [Test]
        public void TestRegisterInterfaceAndImplementationResolvesNewInstance()
        {
            ServiceManager.Default.Register<ITestInterface, TestClass>();
            var instance1 = ServiceManager.Default.Resolve<ITestInterface>();
            var instance2 = ServiceManager.Default.Resolve<ITestInterface>();

            Assert.AreNotSame(instance1, instance2);
        }

        [Test]
        public void TestRegisterInterfaceAndImplementationResolvesTheSameInstanceWhenSpecifyingSingleInstance()
        {
            ServiceManager.Default.RegisterSingle<ITestInterface, TestClass>();
            var instance1 = ServiceManager.Default.Resolve<ITestInterface>();
            var instance2 = ServiceManager.Default.Resolve<ITestInterface>();

            Assert.AreSame(instance1, instance2);
        }

        [Test]
        public void TestRegisterInterfaceAndImplementatoinResolvesNewInstance()
        {
            ServiceManager.Default.Register<ITestInterface, TestClass>();
            var instance1 = ServiceManager.Default.Resolve<ITestInterface>();
            var instance2 = ServiceManager.Default.Resolve<ITestInterface>();

            Assert.AreNotSame(instance1, instance2);
        }

        [Test]
        public void TestUnRegisterInterfaceCorrectlyRemovesRegistration()
        {
            ServiceManager.Default.Register<ITestInterface, TestClass>();
            ServiceManager.Default.Unregister<ITestInterface>();
            Assert.IsFalse(ServiceManager.Default.IsRegistered<ITestInterface>());
        }

        [Test]
        public void TestCloneWithInstancesResolvesTheSameAsParent()
        {
            ServiceManager.Default.UnregisterAll();

            ServiceManager.Default.RegisterSingle<ITestInterface, TestClass>();
            var instance1 = ServiceManager.Default.Resolve<ITestInterface>();

            var serviceManager = ServiceManager.Default.Clone(true);
            var instance2 = serviceManager.Resolve<ITestInterface>();

            Assert.AreSame(instance1, instance2);
        }

        [Test]
        public void TestCloneWithoutInstancesResolvesTheSameAsParent()
        {
            ServiceManager.Default.RegisterSingle<ITestInterface, TestClass>();
            var instance1 = ServiceManager.Default.Resolve<ITestInterface>();
            instance1.Name = "Test";

            var serviceManager = ServiceManager.Default.Clone(false);
            var instance2 = serviceManager.Resolve<ITestInterface>();

            Assert.AreNotSame(instance1, instance2);
        }

        [Test]
        public void TestCloneWithoutInstancesResolvesTheSameAsClonedServiceManagerButNotAsParent()
        {
            ServiceManager.Default.RegisterSingle<ITestInterface, TestClass>();
            var instance1 = ServiceManager.Default.Resolve<ITestInterface>();

            var serviceManager = ServiceManager.Default.Clone(false);
            var instance2 = serviceManager.Resolve<ITestInterface>();
            var instance3 = serviceManager.Resolve<ITestInterface>();

            Assert.AreNotSame(instance1, instance2);
            Assert.AreSame(instance2, instance3);
        }

        [Test]
        public void TestCreateInstanceCreatesAnObject()
        {
            ServiceManager.Default.Register<ITestInterface, TestClassWithValueConstructor>();
            var instance = ServiceManager.Default.CreateInstance<ITestInterface>(true);

            Assert.IsNotNull(instance);
            Assert.IsTrue(typeof(ITestInterface).IsAssignableFrom(instance.GetType()));
        }

        [Test]
        [ExpectedException(typeof(InvalidOperationException))]
        public void TestCreateInstanceThrowsExceptionOnSingleInstanceObjects()
        {
            ServiceManager.Default.RegisterSingle<ITestInterface, TestClass>();
            var instance = ServiceManager.Default.CreateInstance<ITestInterface>(true);
        }

        [Test]
        public void TestServiceManagerIsCleanedUpOnDispose()
        {
            var serviceManager = new ServiceManager();
            serviceManager.RegisterSingle<ITestInterface, TestClass>();

            var instance = serviceManager.Resolve<ITestInterface>();
            serviceManager.Dispose();
            Assert.IsFalse(serviceManager.IsRegistered<ITestInterface>());
        }

        [Test]
        public void TestInstanceIsResolvedWithResolvableConstructor()
        {
            var serviceManager = new ServiceManager();
            serviceManager.Register<ITestInterface, TestClass>();
            serviceManager.Register<ITestInterface2, TestClass2>();

            var instance = serviceManager.Resolve<ITestInterface2>();
            Assert.IsNotNull(instance);
            Assert.IsNotNull(instance.Inner);
        }

        [Test]
        [ExpectedException(typeof(Exception))]
        public void TestResolveThrowsExceptionWhenConstructionParameterIsNotRegistered()
        {
            var serviceManager = new ServiceManager();
            serviceManager.Register<ITestInterface2, TestClass2>();

            var instance = serviceManager.Resolve<ITestInterface2>();
        }

        [Test]
        [ExpectedException(typeof(Exception))]
        public void TestResolveThrowsExceptionWhenConstructionWithValueTypeParameterIsCalled()
        {
            var serviceManager = new ServiceManager();
            serviceManager.Register<ITestInterface2, TestClass3>();

            var instance = serviceManager.Resolve<ITestInterface2>();
        }

        [Test]
        public void TestIdentityIsUnique()
        {
            var serviceManager = new ServiceManager();
            var id = serviceManager.Identity;

            var serviceManager2 = new ServiceManager();
            var id2 = serviceManager2.Identity;

            Assert.AreNotEqual(id, id2);
        }

        [Test]
        public void TestNoneResolvableParameterGetsPassedToConstructor()
        {
            var serviceManager = new ServiceManager();
            serviceManager.Register<ITestInterface4, TestClass4>();
            serviceManager.Register<ITestInterface2, TestClass3>();
            serviceManager.Register<ITestInterface, TestClass>();

            var instance = serviceManager.Resolve<ITestInterface4>(10);

            Assert.IsNotNull(instance);
            Assert.AreEqual(10, instance.Value);
        }

        [Test]
        public void TestNoneResolvableParameterGetsPassedToConstructor2()
        {
            var serviceManager = new ServiceManager();
            serviceManager.Register<ITestInterface2, TestClass2>();
            serviceManager.Register<ITestInterface, TestClass>();
            serviceManager.Register<ITestInterface5, TestClass5>();

            DateTime date = DateTime.Now;
            var instance = serviceManager.Resolve<ITestInterface5>(5, date);

            Assert.IsNotNull(instance);
            Assert.AreEqual(5, instance.Value);
            Assert.AreEqual(date, instance.Date);
            Assert.IsNotNull(instance.Test);
            Assert.IsNotNull(instance.Test2);
        }

        [Test]
        public void TestRegisterTypesFromAssemblyRegistersViewModelsThatInheritViewModel()
        {
            var serviceManager = new ServiceManager();
            serviceManager.RegisterTypesFromAssembly(typeof(ViewModel), System.Reflection.Assembly.GetExecutingAssembly());
            Assert.IsTrue(serviceManager.IsRegistered<LocatableViewModel>());
            Assert.IsTrue(serviceManager.IsRegistered<ViewlessViewModel>());
        }

        [Test]
        public void TestRegisterTypesFromAssemblyAsSingletonRegistersViewModelsThatInheritViewModel()
        {
            var serviceManager = new ServiceManager();
            serviceManager.RegisterTypesFromAssemblyAsSingle(typeof(ViewModel), System.Reflection.Assembly.GetExecutingAssembly());
            Assert.IsTrue(serviceManager.IsRegistered<LocatableViewModel>());
            Assert.IsTrue(serviceManager.IsRegistered<ViewlessViewModel>());

            var vm1 = serviceManager.Resolve<LocatableViewModel>();
            var vm2 = serviceManager.Resolve<LocatableViewModel>();

            Assert.IsTrue(ReferenceEquals(vm1, vm2));
        }
    }

    internal class TestClass : ITestInterface
    {
        public TestClass()
        {
        }

        public bool Fired { get; set; }

        public string Name { get; set; }

        public void Test()
        {
            throw new NotImplementedException();
        }
    }

    internal class TestClassWithValueConstructor : ITestInterface
    {
        public TestClassWithValueConstructor(bool fired)
        {
            Fired = fired;
        }

        public bool Fired { get; set; }

        public string Name { get; set; }

        public void Test()
        {
            throw new NotImplementedException();
        }
    }

    internal interface ITestInterface
    {
        void Test();

        bool Fired { get; set; }

        string Name { get; set; }
    }

    internal interface ITestInterface2
    {
        ITestInterface Inner { get; set; }
    }

    internal class TestClass2 : ITestInterface2
    {
        public TestClass2(ITestInterface firstInstance)
        {
            Inner = firstInstance;
        }

        public ITestInterface Inner
        {
            get;
            set;
        }
    }

    internal class TestClass3 : ITestInterface2
    {
        public TestClass3(bool someParameter)
        {
        }

        public ITestInterface Inner
        {
            get;
            set;
        }
    }

    internal interface ITestInterface4
    {
        int Value { get; set; }
    }

    internal interface ITestInterface5 : ITestInterface4
    {
        DateTime Date { get; set; }
        ITestInterface Test { get; set; }
        ITestInterface2 Test2 { get; set; }
    }

    internal class TestClass4 : ITestInterface4
    {
        public TestClass4(ITestInterface test, int amount)
        {
            Value = amount;
        }

        public int Value { get; set; }
    }

    internal class TestClass5 : ITestInterface5
    {
        public TestClass5(ITestInterface test, int amount, ITestInterface2 test2, DateTime date)
        {
            Value = amount;
            Test = test;
            Test2 = test2;
            Date = date;
        }

        public int Value { get; set; }
        public DateTime Date { get; set; }
        public ITestInterface Test { get; set; }
        public ITestInterface2 Test2 { get; set; }
    }

}
