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
using System.Reflection;

namespace Clarity.Internal
{
    internal class ServiceInfo
    {
        public ConstructorInfo Constructor { get; set; }

        public Type Implementation { get; internal set; }

        public object Instance { get; internal set; }

        public Type Interface { get; internal set; }

        public bool OwnsInstance { get; internal set; }

        public bool SingleInstance { get; internal set; }

        public Func<object> Factory { get; internal set; }

        internal void CleanUpInstances()
        {
            var disposableInstance = Instance as IDisposable;

            if (OwnsInstance && disposableInstance != null)
            {
                disposableInstance.Dispose();
            }

            Instance = null;
        }

        public ServiceInfo Clone(bool cloneInstance)
        {
            var si = new ServiceInfo();
            si.Interface = Interface;
            si.Implementation = Implementation;
            si.SingleInstance = SingleInstance;
            si.Constructor = Constructor;

            if (cloneInstance)
            {
                si.Instance = Instance;
                si.OwnsInstance = false;
            }

            return si;
        }
    }
}
