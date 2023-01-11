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

namespace Clarity
{
    /// <summary>
    /// Interface for a Message Bus. Provides the ability to publish and subscribe to message types
    /// </summary>
    public interface IMessageBus
    {
        /// <summary>
        /// Subscribes an action to a particular message type. When that message type
        /// is published, this action will be called.
        /// </summary>
        /// <typeparam name="TMessage">The type of message to listen for.</typeparam>
        /// <param name="handler">
        /// The action which will be called when a message of type <typeparamref name="TMessage"/>
        /// is published.
        /// </param>
        void Subscribe<TMessage>(Action<TMessage> handler);

        /// <summary>
        /// Subscribes or Un-subscribes a message for a handler
        /// </summary>
        /// <typeparam name="TMessage">The type of the message to listen for.</typeparam>
        /// <param name="handler">The handler.</param>
        /// <param name="subscribe">If set to <c>true</c> subscribe the handler, if set to <c>false</c> un-subscribe the handler.</param>
        void ToggleSubscription<TMessage>(Action<TMessage> handler, bool subscribe);

        /// <summary>
        /// Unsubscribes an action from a particular message type.
        /// </summary>
        /// <typeparam name="TMessage">The type of message to stop listening for.</typeparam>
        /// <param name="handler">
        /// The action which is to be unsubscribed from messages of type <typeparamref name="TMessage"/>.
        /// </param>
        void Unsubscribe<TMessage>(Action<TMessage> handler);

        /// <summary>
        /// Publishes a message to any subscribers of a particular message type.
        /// </summary>
        /// <typeparam name="TMessage">The type of message to publish.</typeparam>
        /// <param name="message">The message to be published</param>
        void Publish<TMessage>(TMessage message);

		void Publish(object message);

        /// <summary>
        /// Publishes a message to any subscribers of a particular message type on the UI thread.
        /// </summary>
        /// <typeparam name="TMessage">The type of message to publish.</typeparam>
        /// <param name="message">The message to be published</param>
        void PublishOnUIThread<TMessage>(TMessage message);

        /// <summary>
        /// Publishes a message to any subscribers of a particular message type on the background thread
        /// </summary>
        /// <typeparam name="TMessage">The type of message to publish.</typeparam>
        /// <param name="message">The message to be published</param>
        void PublishAsync<TMessage>(TMessage message);

        /// <summary>
        /// Unsubscribes the handler from all message notifications
        /// </summary>
        /// <param name="handler">The handler.</param>
        void Unsubscribe(IDisposed handler);
    }
}
