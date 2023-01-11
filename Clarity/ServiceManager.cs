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

using Clarity.Internal;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Clarity
{
    public class ServiceManager : IServiceManager, IDisposable
    {
        #region static accessor

        private static IServiceManager _default = new ServiceManager();

        /// <summary>
        /// Returns the default <see cref="IServiceManager"/>
        /// </summary>
        public static IServiceManager Default
        {
            get
            {
                return _default;
            }

            set
            {
                _default = value;
            }
        }

        #endregion static accessor

        private readonly Dictionary<Type, ServiceInfo> _registeredItems = new Dictionary<Type, ServiceInfo>();

        #region construction

        /// <summary>
        /// Creates a new ServiceManager instance
        /// </summary>
        public ServiceManager()
        {
        }

        /// <summary>
        /// Creates a new instance of the ServiceManager, inheriting registrations from the Default ServiceManager
        /// </summary>
        /// <param name="inheritDefaultRegistrations">if true, will inherit registrations</param>
        /// <param name="cloneInstances">If true, references to instantiated objects will be copied</param>
        private ServiceManager(bool inheritDefaultRegistrations, bool cloneInstances)
        {
            if (inheritDefaultRegistrations)
            {
                var enumerator = _default.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    ServiceInfo value = (ServiceInfo)enumerator.Current;
                    if (value.Interface == null)
                    {
                        _registeredItems.Add(value.Implementation, value.Clone(cloneInstances));
                    }
                    else
                    {
                        _registeredItems.Add(value.Interface, value.Clone(cloneInstances));
                    }
                }
            }
        }

        #endregion construction

        #region registration

        /// <summary>
        /// Always creates an instance of the supplied type.
        /// </summary>
        /// <typeparam name="T">The interface of the type to create.</typeparam>
        /// <param name="args">Optional arguments to pass to the constructor.</param>
        /// <returns>An instance of the supplied type initialized with any optional arguments.</returns>
        /// <exception cref="System.InvalidOperationException">Use Resolve methods for lifetime managed objects.</exception>
        public T CreateInstance<T>(params object[] args)
        {
            var t = typeof(T);

            var info = GetServiceInfo(t);

            if (info.SingleInstance)
            {
                throw new InvalidOperationException("Use Resolve methods for lifetime managed objects.");
            }

            return (T)Activator.CreateInstance(info.Implementation, args);
        }

        /// <summary>
        /// Determines whether the specified type is registered.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>
        /// true; if registered, otherwise false
        /// </returns>
        public bool IsRegistered(Type type)
        {
            return _registeredItems.ContainsKey(type);
        }

        /// <summary>
        /// Determines whether the interface is registered.
        /// </summary>
        /// <returns>
        /// true; if registered, otherwise false
        /// </returns>
        public bool IsRegistered<I>()
        {
            return IsRegistered(typeof(I));
        }

        /// <summary>
        /// Registers and interface type and the implementation type for future use. This will create a new instance each time the interface is resolved
        /// </summary>
        /// <typeparam name="I">The interface to register</typeparam>
        /// <typeparam name="T">The implementation to register</typeparam>
        public void Register<I, T>() where T : I
        {
            Register(typeof(I), typeof(T), false);
        }

        public void Register<I>(Func<I> factory)
        {
            if (!_registeredItems.ContainsKey(typeof(I)))
            {
                var func = new Func<object>(() => factory());

                TryRegister(typeof(I), new ServiceInfo() { Interface = typeof(I), Factory = func });
                return;
            }
        }

        /// <summary>
        /// Registers and interface type and the implementation type for future use.
        /// </summary>
        /// <typeparam name="I">The interface to register</typeparam>
        /// <typeparam name="T">The implementation to register</typeparam>
        /// <param name="singleInstance">Indicates how instantiation should occur</param>
        public void RegisterSingle<I, T>() where T : I
        {
            RegisterSingle(typeof(I), typeof(T));
        }

        public void Register(Type iface, Type imp)
        {
            Register(iface, imp, false);
        }

        public void RegisterSingle(Type iface, Type imp)
        {
            Register(iface, imp, true);
        }

        private void Register(Type iface, Type imp, bool singleInstance)
        {
            if (!_registeredItems.ContainsKey(iface))
            {
                var constructor = ValidateTypes(iface, imp);
                TryRegister(iface, new ServiceInfo() { Interface = iface, Implementation = imp, SingleInstance = singleInstance, Constructor = constructor });
                return;
            }

            throw new Exception("Interface '" + iface.Name + "' already registered");
        }

        /// <summary>
        /// Registers and concrete type. This instance will always be returned
        /// </summary>
        /// <param name="instance">The instance to register</param>
        public void RegisterSingle<T>(T instance)
        {
            Type t = typeof(T);

            if (!_registeredItems.ContainsKey(t))
            {
                TryRegister(t, new ServiceInfo() { Interface = t, Instance = instance, SingleInstance = true });
                return;
            }

            throw new Exception("Instance '" + t.Name + "' already registered");
        }

        /// <summary>
        /// Registers instance as a specific type
        /// </summary>
        /// <param name="type"></param>
        /// <param name="instance"></param>
        public void Register(Type type, object instance)
        {
            if (!_registeredItems.ContainsKey(type))
            {
                TryRegister(type, new ServiceInfo() { Interface = type, Instance = instance, SingleInstance = true });
                return;
            }
        }

        /// <summary>
        /// Registers the specified implementation.
        /// </summary>
        /// <exception cref="System.Exception">Interface already registered</exception>
        public void Register<T>() where T : new()
        {
            Type t = typeof(T);

            Register(t, false);
        }

        /// <summary>
        /// Registers the specified single instance.
        /// </summary>
        /// <exception cref="System.Exception">Type already registered</exception>
        public void RegisterSingle<T>()
        {
            Type t = typeof(T);

            Register(t, true);
        }

        private void Register(Type t, bool singleInstance = false)
        {
            if (!_registeredItems.ContainsKey(t))
            {
                var constructor = t.GetConstructors()[0];
                TryRegister(t, new ServiceInfo() { Implementation = t, Constructor = constructor, SingleInstance = singleInstance });
                return;
            }

            throw new Exception("Type '" + t.Name + "' already registered");
        }

        /// <summary>
        /// Registers all types that inherit the base interface.
        /// </summary>
        /// <param name="baseType">Type of the base.</param>
        /// <param name="assemblyToScan">The assembly to scan.</param>
        public void RegisterTypesFromAssemblyAsSingle(Type baseType, Assembly assemblyToScan)
        {
            RegisterTypesFromAssembly(baseType, assemblyToScan, true);
        }

        /// <summary>
        /// Registers all types that inherit the base interface.
        /// </summary>
        /// <param name="baseType">Type of the base.</param>
        /// <param name="assemblyToScan">The assembly to scan.</param>
        public void RegisterTypesFromAssembly(Type baseType, Assembly assemblyToScan)
        {
            RegisterTypesFromAssembly(baseType, assemblyToScan, false);
        }

        private void RegisterTypesFromAssembly(Type baseType, Assembly assemblyToScan, bool singleInstance)
        {
            baseType.IfNullThrow("baseType");
            assemblyToScan.IfNullThrow("assemblyToScan");

            var types = assemblyToScan.GetExportedTypes();

            foreach (var type in types.Where(t => t.IsClass && !t.IsAbstract && baseType.IsAssignableFrom(t)))
            {
                var interfaces = type.GetInterfaces().Where(i => i != baseType && baseType.IsAssignableFrom(i));
                var iface = DetermineMostSpecificInterface(interfaces);

                if (ReferenceEquals(null, iface))
                {
                    if (!IsRegistered(type))
                    {
                        Register(type, singleInstance);
                    }
                }
                else
                {
                    Register(iface, type, singleInstance);
                }
            }
        }

        private static Type DetermineMostSpecificInterface(IEnumerable<Type> interfaces)
        {
            var best = interfaces.FirstOrDefault();

            if (ReferenceEquals(null, best))
            {
                return null;
            }

            bool ok = false;
            if (interfaces.Count() <= 1)
            {
                return best;
            }

            while (!ok)
            {
                Type last = best;
                foreach (var t in interfaces)
                {
                    if (t != best)
                    {
                        if (!t.IsAssignableFrom(best))
                        {
                            best = t;
                        }
                    }
                }

                if (last == best)
                {
                    break;
                }
            }

            return best;
        }

        private void TryRegister(Type key, ServiceInfo info)
        {
            _registeredItems.Add(key, info);
        }

        /// <summary>
        /// Clears out all registered information.
        /// This is to support re-registration via unit tests.
        /// </summary>
        public void UnregisterAll()
        {
            _registeredItems.Clear();
        }

        private static ConstructorInfo ValidateTypes(Type i, Type t)
        {
            if (i.IsInterface)
            {
                if (t.IsClass && i.IsAssignableFrom(t))
                {
                }
                else
                {
                    throw new InvalidOperationException("The type '" + t.Name + "' is either not a class, or does not implement '" + i.Name + "'");
                }
            }
            else
            {
                throw new InvalidOperationException("'" + i.Name + "' must be an interface");
            }

            var constructors = t.GetConstructors();

            return constructors.Length == 1 ? constructors[0] : null;
        }

        #endregion registration

        private object _thisLock = new object();
        private string _guid;

        /// <summary>
        /// Returns a unique id for this instance
        /// </summary>
        public string Identity
        {
            get
            {
                lock (_thisLock)
                {
                    if (_guid == null)
                    {
                        _guid = Guid.NewGuid().ToString();
                    }
                }

                return _guid;
            }
        }

        /// <summary>
        /// Creates a copy of the <see cref="IServiceManager"/>
        /// </summary>
        /// <param name="cloneInstances">if true, references to instantiated objects will also be copied</param>
        /// <returns>A new <see cref="IServiceManager"/> containing the same registrations as the current instance</returns>
        public IServiceManager Clone(bool cloneInstances)
        {
            ServiceManager sm = new ServiceManager(true, cloneInstances);
            return sm;
        }

        public void Dispose()
        {
            foreach (var registeredItem in _registeredItems.Values)
            {
                registeredItem.CleanUpInstances();
            }

            _registeredItems.Clear();
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Returns an enumeration of all registrations
        /// </summary>
        public IEnumerator GetEnumerator()
        {
            return _registeredItems.Values.GetEnumerator();
        }

        /// <summary>
        /// Removes a registered interface
        /// </summary>
        /// <typeparam name="I">The interface to find and remove</typeparam>
        public void Unregister<I>()
        {
            if (_registeredItems.ContainsKey(typeof(I)))
            {
                _registeredItems.Remove(typeof(I));
            }
        }

        #region resolve
        public T TryResolve<T>(params object[] values)
        {
            if (_registeredItems.ContainsKey(typeof(T)))
            {
                return (T)TryResolve(typeof(T), false, values);
            }

            return default(T);
        }

        private object TryResolve(Type t, bool throwException, params object[] values)
        {
            if (_registeredItems.ContainsKey(t) == false)
            {
                if (throwException)
                {
                    throw new Exception("The type '" + t.Name + "' has not been registered!");
                }
                else
                {
                    return null;
                }
            }

            var info = _registeredItems[t];

            if (info.SingleInstance && info.Instance != null)
            {
                if (info.Instance is IDisposed && ((IDisposed)info.Instance).IsDisposed)
                {
                    info.Instance = null;
                    info.OwnsInstance = false;
                }
                else
                {
                    return info.Instance;
                }
            }

            object instance;

            if (info.Factory != null)
            {
                instance = info.Factory.Invoke();
            }
            else if (info.Implementation == null && info.Interface.IsInterface)
            {
                instance = Resolve(info.Interface);
            }
            else
            {
                var ctor = DetermineBestResolvableConstructor(info, values);
                var parameters = ctor.GetParameters();

                if (parameters.Length == 0)
                {
                    if (info.Implementation == null && !info.Interface.IsInterface)
                    {
                        // The Activator can cache calls to default constructors so we use that as the fast path.
                        instance = Activator.CreateInstance(info.Interface);
                    }
                    else
                    {
                        // The Activator can cache calls to default constructors so we use that as the fast path.
                        instance = Activator.CreateInstance(info.Implementation);
                    }
                }
                else
                {
                    List<object> parameterValues = new List<object>();
                    int i = 0;
                    foreach (var p in parameters)
                    {
                        if (IsRegistered(p.ParameterType))
                        {
                            if (throwException)
                            {
                                parameterValues.Add(Resolve(p.ParameterType, values.Skip(i + 1)));
                            }
                            else
                            {
                                var value = TryResolve(p.ParameterType, false, values.Skip(i + 1));
                                if (value == null)
                                {
                                    return null;
                                }

                                parameterValues.Add(value);
                            }
                        }
                        else
                        {
                            if (i < values.Length && values[i].GetType() == p.ParameterType)
                            {
                                parameterValues.Add(values[i++]);
                            }
                        }
                    }

                    ////var resolvedParams = parameters.Select(p => Resolve(p.ParameterType, forceSingleInstance)).ToArray();
                    if (parameters.Length == parameterValues.Count)
                    {
                        instance = ctor.Invoke(parameterValues.ToArray());
                    }
                    else
                    {
                        throw new Exception("Parameter count mismatch. Expected " + parameters.Length + " but only encountered " + parameterValues.Count);
                    }
                }
            }

            if (info.SingleInstance)
            {
                info.Instance = instance;
                info.OwnsInstance = true;
            }

            return instance;
        }

        /// <summary>
        /// Resolves an instance based on the registration info
        /// </summary>
        /// <typeparam name="T">The interface to resolve</typeparam>
        /// <param name="values">A list of specified parameters to be injected into the constructor where possible</param>
        /// <returns>An instance complying to the specified interface</returns>
        public T Resolve<T>(params object[] values)
        {
            return (T)Resolve(typeof(T), values);
        }

        /// <summary>
        /// Resolves an instance based on the registration info
        /// </summary>
        /// <param name="t">The type of interface to resolve</param>
        /// <returns>An instance complying to the specified interface</returns>
        public object Resolve(Type t, params object[] values)
        {
            if (_registeredItems.ContainsKey(t) == false)
            {
                throw new Exception("The type '" + t.Name + "' has not been registered!");
            }

            return TryResolve(t, true, values);
        }

        private ConstructorInfo DetermineBestResolvableConstructor(ServiceInfo info, params object[] values)
        {
            var ctor = info.Constructor;

            if (ctor == null)
            {
                if (info.Implementation == null)
                {
                    return null;
                }
                else
                {
                    int i = 0;
                    foreach (var ct in info.Implementation.GetConstructors())
                    {
                        bool isValid = true;
                        foreach (var p in ct.GetParameters())
                        {
                            if (!_registeredItems.ContainsKey(p.ParameterType))
                            {
                                if (i < values.Length)
                                {
                                    if (values[i++].GetType() != p.ParameterType)
                                    {
                                        isValid = false;
                                        break;
                                    }
                                }
                            }
                        }

                        if (isValid)
                        {
                            ctor = ct;
                            break;
                        }
                    }

                    //var ctors = from c in info.Implementation.GetConstructors()
                    //            let p = c.GetParameters()
                    //            orderby p.Length descending
                    //            where CanResolveParameters(p)
                    //            select c;

                    //ctor = ctors.FirstOrDefault();
                }

                if (ctor == null)
                {
                    throw new Exception("Type '" + info.Implementation.Name + "' no constructor with resolvable parameters found.");
                }
            }

            return ctor;
        }

        #endregion resolve

        #region private methods

        /// <summary>
        /// Gets the service info entry for a type.
        /// </summary>
        /// <param name="t">The type to find.</param>
        /// <returns>A ServiceInfo entry.</returns>
        /// <exception cref="System.Exception">The type ' + t.Name + ' has not been registered!</exception>
        private ServiceInfo GetServiceInfo(Type t)
        {
            if (_registeredItems.ContainsKey(t) == false)
            {
                throw new Exception("The type '" + t.Name + "' has not been registered!");
            }

            return _registeredItems[t];
        }

        #endregion private methods
    }
}
