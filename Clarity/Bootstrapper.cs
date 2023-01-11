// ****************************************************************************
// <copyright>
// Copyright © Paul Sanders 2014
// </copyright>
// ****************************************************************************
// <author>Paul Sanders</author>
// <project>Clarity</project>
// <web>http://clarity.codeplex.com</web>
// <license>
// See license.txt in this solution
// </license>
// ****************************************************************************

using Clarity.Commands;
using System.Reflection;
namespace Clarity
{
	/// <summary>
	/// Provides default bootstrapping functionality for an application
	/// </summary>
	public class Bootstrapper
	{
		/// <summary>
		/// Constructs and initializes the object
		/// </summary>
		public Bootstrapper()
		{
			Initialize();
		}

		private void Initialize()
		{
			Execute.InitializeWithCurrentContext();

			OnInitialiseLogging();
			OnRegisterDefaultItems();
			OnRegisterCommandBuilder();
			OnRegisterViewModels();
		}


		/// <summary>
		/// Called when to initialise the logger.
		/// </summary>
		protected virtual void OnInitialiseLogging()
		{
			Logger.SetLogger(new DiagnosticLogger());
		}

		/// <summary>
		/// Called when to register default items.
		/// </summary>
		protected virtual void OnRegisterDefaultItems()
		{
			ServiceManager.Default.RegisterSingle<IMessageBus, MessageBus>();
			ServiceManager.Default.RegisterSingle<IViewLocator, DefaultViewLocator>();
		}

		/// <summary>
		/// Registers the default <see cref="ICommandBuilder"/>
		/// </summary>
		protected virtual void OnRegisterCommandBuilder()
		{
			ServiceManager.Default.RegisterSingle<ICommandBuilder, DefaultCommandBuilder>();
		}

		/// <summary>
		/// Registers all view models from the entry assembly.
		/// </summary>
		protected virtual void OnRegisterViewModels()
		{
			var assy = Assembly.GetEntryAssembly();//will be null in unit tests
			if (assy != null)
			{
				ServiceManager.Default.RegisterTypesFromAssembly(typeof(ViewModel), assy);
			}
		}

		/// <summary>
		/// Registers T into the default <see cref="ServiceManager"/>
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <returns>The current bootstrapper to allow for fluent syntax</returns>
		public Bootstrapper Register<T>() where T : new()
		{
			ServiceManager.Default.Register<T>();
			return this;
		}

		/// <summary>
		/// Registers an implementation type T for interface I
		/// </summary>
		/// <typeparam name="I">The interface to be registered</typeparam>
		/// <typeparam name="T">The implementation for the interface</typeparam>
		/// <returns>The current bootstrapper to allow for fluent syntax</returns>
		public Bootstrapper Register<I, T>() where T : I
		{
			ServiceManager.Default.Register<I, T>();
			return this;
		}

		/// <summary>
		/// Registers an implementation type T for interface I as a singleton
		/// </summary>
		/// <typeparam name="I">The interface to be registered</typeparam>
		/// <typeparam name="T">The implementation for the interface</typeparam>
		/// <returns>The current bootstrapper to allow for fluent syntax</returns>
		public Bootstrapper RegisterSingle<I, T>() where T : I
		{
			ServiceManager.Default.RegisterSingle<I, T>();
			return this;
		}

		/// <summary>
		/// Registers an instance as a singleton
		/// </summary>
		/// <typeparam name="T">The instance to return</typeparam>
		/// <returns>The current bootstrapper to allow for fluent syntax</returns>
		public Bootstrapper RegisterSingle<T>(T instance)
		{
			ServiceManager.Default.RegisterSingle(instance);
			return this;
		}

		/// <summary>
		/// Registers a factory method to return an instance for the given interface
		/// </summary>
		/// <typeparam name="I">The interface to be registered</typeparam>
		/// <param name="factory">The factory to build the implementation</param>
		/// <returns>The current bootstrapper to allow for fluent syntax</returns>
		public Bootstrapper Register<I>(System.Func<I> factory)
		{
			ServiceManager.Default.Register(factory);
			return this;
		}
	}
}
