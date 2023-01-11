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
using System;
using System.Collections;
using System.Reflection;

namespace Clarity
{
    public interface IServiceManager : IDisposable
    {
        /// <summary>
        /// Determines whether the specified type is registered.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>true; if registered, otherwise false</returns>
        bool IsRegistered(Type type);

        /// <summary>
        /// Determines whether the interface is registered.
        /// </summary>
        /// <returns>true; if registered, otherwise false</returns>
        bool IsRegistered<I>();

        /// <summary>
        /// Registers a implementation type for future use. This will create a new instance each time the interface is resolved
        /// </summary>
        /// <typeparam name="T">The implementation to register</typeparam>
        void Register<T>() where T : new();

        /// <summary>
        /// Registers and interface type and the implementation type for future use. This will create a new instance each time the interface is resolved
        /// </summary>
        /// <typeparam name="I">The interface to register</typeparam>
        /// <typeparam name="T">The implementation to register</typeparam>
        void Register<I, T>() where T : I;

        /// <summary>
        /// Registers and interface type and the implementation type for future use. This will create a new instance each time the interface is resolved
        /// </summary>
        void Register(Type iface, Type imp);

        void Register<I>(Func<I> factory);

        /// <summary>
        /// Registers and interface type and the implementation type for future use. This will return the same instance each time the interface is resolved
        /// </summary>
        void RegisterSingle(Type iface, Type imp);

        /// <summary>
        /// Registers and interface type and the implementation type for future use. This will return the same instance each time the interface is resolved
        /// <typeparam name="I">The interface to register</typeparam>
        /// <typeparam name="T">The implementation to register</typeparam>
        /// </summary>
        void RegisterSingle<I, T>() where T : I;

        void RegisterSingle<T>();

        void Register(Type type, object instance);

        /// <summary>
        /// Registers all types that inherit the interface or base type.
        /// </summary>
        /// <param name="baseType">Type of the base.</param>
        /// <param name="assemblyToScan">The assembly to scan.</param>
        void RegisterTypesFromAssemblyAsSingle(Type baseType, Assembly assemblyToScan);

        /// <summary>
        /// Registers all types that inherit the interface or base type.
        /// </summary>
        /// <param name="baseType">Type of the base.</param>
        /// <param name="assemblyToScan">The assembly to scan.</param>
        void RegisterTypesFromAssembly(Type baseType, Assembly assemblyToScan);

        /// <summary>
        /// Resolves an instance based on the registration info
        /// </summary>
        /// <typeparam name="T">The interface to resolve</typeparam>
        /// <param name="values">A list of specified parameters to be injected into the constructor where possible</param>
        /// <returns>An instance complying to the specified interface if known, otherwise null</returns>
        T TryResolve<T>(params object[] values);

        /// <summary>
        /// Resolves an instance based on the registration info
        /// </summary>
        /// <typeparam name="T">The interface to resolve</typeparam>
        /// <param name="values">A list of specified parameters to be injected into the constructor where possible</param>
        /// <returns>An instance complying to the specified interface</returns>
        T Resolve<T>(params object[] values);

        /// <summary>
        /// Resolves an instance based on the registration info
        /// </summary>
        /// <param name="t">The type of interface to resolve</param>
        /// <returns>An instance complying to the specified interface</returns>
        object Resolve(Type t, params object[] values);

        /// <summary>
        /// Removes a registered interface
        /// </summary>
        /// <typeparam name="I">The interface to find and remove</typeparam>
        void Unregister<I>();

        /// <summary>
        /// Clears out all registered information.
        /// This is to support re-registration via unit tests.
        /// </summary>
        void UnregisterAll();

        /// <summary>
        /// Always creates an instance of the supplied type.
        /// </summary>
        /// <typeparam name="T">The interface of the type to create.</typeparam>
        /// <param name="args">Optional arguments to pass to the constructor.</param>
        /// <returns>An instance of the supplied type initialized with any optional arguments.</returns>
        T CreateInstance<T>(params object[] args);

        /// <summary>
        /// Returns an enumeration of all registrations
        /// </summary>
        IEnumerator GetEnumerator();

        /// <summary>
        /// Creates a copy of the <see cref="IServiceManager"/>
        /// </summary>
        /// <param name="cloneInstances">if true, references to instantiated objects will also be copied</param>
        /// <returns>A new <see cref="IServiceManager"/> containing the same registrations as the current instance</returns>
        IServiceManager Clone(bool cloneInstances);

        void RegisterSingle<T>(T instance);
    }
}
