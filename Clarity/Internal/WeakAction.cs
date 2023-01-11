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

namespace Clarity.Internal
{
    /// <summary>
    /// Maintains a weak reference to an action/delegate.
    /// </summary>
    internal sealed class WeakAction
    {
        private WeakReference WeakReference { get; set; }

        /// <summary>
        /// Gets the action/delegate this reference targets.
        /// </summary>
        public Delegate Target { get; private set; }

        /// <summary>
        /// Gets whether the reference is still alive.
        /// </summary>
        public bool IsAlive
        {
            get { return ((IDisposed)Target.Target).IsDisposed == false && WeakReference.IsAlive; }
        }

        /// <summary>
        /// Constructor for WeakAction.
        /// </summary>
        /// <param name="action">The action/delegate to be referenced.</param>
        /// <exception cref="System.ArgumentNullException">action</exception>
        /// <exception cref="System.InvalidOperationException">
        /// Handler must be a non-static member
        /// or
        /// Handler must be on object that implements IDisposed
        /// </exception>        
        public WeakAction(Delegate action)
        {
            if (action == null)
            {
                throw new ArgumentNullException("action");
            }

            Target = action;
            if (action.Target is IDisposed)
            {
                WeakReference = new WeakReference(action.Target);
            }
            else if (action.Target == null)
            {
                throw new InvalidOperationException("Handler must be a non-static member");
            }
            else
            {
                throw new InvalidOperationException("Handler must be on object that implements IDisposed");
            }
        }
    }
}
