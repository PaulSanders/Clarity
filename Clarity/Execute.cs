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
using System.Diagnostics;
using System.Threading;

namespace Clarity
{
    public static class Execute
    {
        private static bool? _inDesignMode;
        private static SynchronizationContext _context;
        private static Action<Action> _marshaller;

        static Execute()
        {
            InitializeWithCurrentContext();
        }

        /// <summary>
        /// Indicates whether or not the framework is in design-time mode.
        /// </summary>
        public static bool InDesignMode
        {
            get
            {
                if (_inDesignMode == null)
                {
                    _inDesignMode = Process.GetCurrentProcess().ProcessName.StartsWith("devenv", StringComparison.Ordinal);
                }

                return _inDesignMode.GetValueOrDefault(false);
            }
        }

        /// <summary>
        /// Initializes the framework using the current <see cref="SynchronizationContext"/>.
        /// </summary>
        public static void InitializeWithCurrentContext()
        {
            _context = SynchronizationContext.Current;
        }

        private static SynchronizationContext CurrentContext
        {
            get
            {
                if (_context == null)
                {
                    _context = new SynchronizationContext();
                }

                return _context;
            }
        }

        /// <summary>
        /// Sets a custom UI thread marshaller.
        /// </summary>
        /// <param name="marshaller">The marshaller.</param>
        public static void SetUIThreadMarshaller(Action<Action> marshaller)
        {
            _marshaller = marshaller;
        }

        /// <summary>
        /// Executes the action on the UI thread.
        /// </summary>
        /// <param name = "action">The action to execute.</param>
        public static void OnUIThread(this Action action)
        {
            if (IsOnUIThread)
            {
                action();
            }
            else
            {
                if (_marshaller == null)
                {
                    CurrentContext.Send((obj) => action(), null);
                }
                else
                {
                    _marshaller(action);
                }
            }
        }

        /// <summary>
        /// Executes the action on the UI thread.
        /// </summary>
        /// <param name = "action">The action to execute.</param>
        public static T OnUIThread<T>(Func<T> action)
        {
            T result = default(T);

            if (IsOnUIThread)
            {
                result = action();
            }
            else
            {
                CurrentContext.Send((obj) => result = action(), null);
            }

            return result;
        }

        /// <summary>
        /// Executes the action on the UI thread.
        /// </summary>
        /// <param name="action">The action to execute.</param>
        /// <param name="parameter">The parameter to pass.</param>
        public static void OnUIThread<T>(this Action<T> action, T parameter)
        {
            if (IsOnUIThread)
            {
                action(parameter);
            }
            else
            {
                CurrentContext.Send((obj) => action(parameter), null);
            }
        }

        /// <summary>
        /// Determines if the current thread is on the UI
        /// </summary>
        public static bool IsOnUIThread
        {
            get
            {
                return Thread.CurrentThread.GetApartmentState() == ApartmentState.STA;
            }
        }

        /// <summary>
        /// Executes the action on the UI thread using an asynchronous message
        /// </summary>
        /// <param name = "action">The action to execute.</param>
        public static void PostOnUIThread(this Action action)
        {
            if (_marshaller == null)
            {
                CurrentContext.Post((obj) => action(), null);
            }
            else
            {
                _marshaller(action);
            }
        }

        /// <summary>
        /// Executes the action on the UI thread using an asynchronous message after a specified delay
        /// </summary>
        /// <param name="action">The action to execute.</param>
        /// <param name="delay">The amount of time to wait before executing the action.</param>
        public static void PostOnUIThreadWithDelay(this Action action, TimeSpan delay)
        {
            var timer = new System.Timers.Timer(delay.TotalMilliseconds);
            timer.Elapsed += (o, e) =>
            {
                timer.Stop();
                PostOnUIThread(action);
                timer.Dispose();
            };

            timer.Start();
        }

        /// <summary>
        /// Returns the UI <see cref="SynchornizationContext"/>
        /// </summary>
        public static SynchronizationContext Context
        {
            get
            {
                return CurrentContext;
            }
        }
    }
}
