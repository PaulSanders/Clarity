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
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Clarity
{
    /// <summary>
    /// Allows arbitrary object to subscribe and unsubscribe to messages
    /// </summary>
    public sealed class MessageBus : IMessageBus
    {
        private Dictionary<Type, List<WeakAction>> _subscribers = new Dictionary<Type, List<WeakAction>>();

        private object _lock = new object();

        /// <summary>
        /// Subscribes an action to a particular message type. When that message type
        /// is published, this action will be called.
        /// </summary>
        /// <typeparam name="TMessage">The type of message to listen for.</typeparam>
        /// <param name="handler">
        /// The action which will be called when a message of type <typeparamref name="TMessage"/>
        /// is published.
        /// </param>
        public void Subscribe<TMessage>(Action<TMessage> handler)
        {
            lock (_lock)
            {
                if (_subscribers.ContainsKey(typeof(TMessage)))
                {
                    var handlers = _subscribers[typeof(TMessage)];
                    handlers.Add(new WeakAction(handler));
                }
                else
                {
                    var handlers = new List<WeakAction>();
                    handlers.Add(new WeakAction(handler));
                    _subscribers[typeof(TMessage)] = handlers;
                }

                Logger.Debug("Message subscription added for message {0}", typeof(TMessage).Name);
            }
        }

        /// <summary>
        /// Unsubscribes an action from a particular message type.
        /// </summary>
        /// <typeparam name="TMessage">The type of message to stop listening for.</typeparam>
        /// <param name="handler">
        /// The action which is to be unsubscribed from messages of type <typeparamref name="TMessage"/>.
        /// </param>
        public void Unsubscribe<TMessage>(Action<TMessage> handler)
        {
            lock (_lock)
            {
                if (_subscribers.ContainsKey(typeof(TMessage)))
                {
                    var handlers = _subscribers[typeof(TMessage)];

                    WeakAction targetReference = null;
                    foreach (var reference in handlers)
                    {
                        var action = (Action<TMessage>)reference.Target;
                        if ((action.Target == handler.Target) && action.Method.Equals(handler.Method))
                        {
                            targetReference = reference;
                            break;
                        }
                    }

                    handlers.Remove(targetReference);

                    if (handlers.Count == 0)
                    {
                        _subscribers.Remove(typeof(TMessage));
                    }

                    Logger.Debug("Message subscription removed for message {0}", typeof(TMessage).Name);
                }
            }
        }

        /// <summary>
        /// Subscribes or Un-subscribes a message for a handler
        /// </summary>
        /// <typeparam name="TMessage">The type of the message to listen for.</typeparam>
        /// <param name="handler">The handler.</param>
        /// <param name="subscribe">if set to <c>true</c> subscribe the handler, if set to <c>false</c> un-subscribe the handler.</param>
        public void ToggleSubscription<TMessage>(Action<TMessage> handler, bool subscribe)
        {
            if (subscribe)
            {
                Subscribe<TMessage>(handler);
            }
            else
            {
                Unsubscribe<TMessage>(handler);
            }
        }

        /// <summary>
        /// Publishes a message to any subscribers of a particular message type.
        /// </summary>
        /// <typeparam name="TMessage">The type of message to publish.</typeparam>
        /// <param name="message">The message to be published</param>
        public void Publish<TMessage>(TMessage message)
        {
            var subscribers = RefreshAndGetSubscribers<TMessage>();
            if (subscribers == null || subscribers.Count == 0)
            {
                Debug.WriteLine(string.Format("No handlers for {0}", message.GetType().Name));
            }
            else
            {
                Debug.WriteLine(string.Format("Invoking handler(s) for message {0}", message.GetType().Name));
                foreach (var subscriber in subscribers)
                {
                    if (subscriber != null)
                    {
                        subscriber.Invoke(message);
                    }
                }
            }
        }

		/// <summary>
		/// Publishes a message to any subscribers of a particular message type.
		/// </summary>
		/// <param name="message">The message to be published</param>
		public void Publish(object message)
		{
			message.IfNullThrow("message");

			var subscribers = RefreshAndGetSubscribers(message.GetType());
			if (subscribers == null || subscribers.Count == 0)
			{
				Debug.WriteLine(string.Format("No handlers for {0}", message.GetType().Name));
			}
			else
			{
				Debug.WriteLine(string.Format("Invoking handler(s) for message {0}", message.GetType().Name));
				foreach (var subscriber in subscribers)
				{
					
					if (subscriber != null)
					{
						subscriber.Target.DynamicInvoke(message);
					}
				}
			}
		}

        /// <summary>
        /// Publishes a message to any subscribers of a particular message type on the UI thread.
        /// </summary>
        /// <typeparam name="TMessage">The type of message to publish.</typeparam>
        /// <param name="message">The message to be published</param>
        public void PublishOnUIThread<TMessage>(TMessage message)
        {
            if (Execute.IsOnUIThread)
            {
                Publish(message);
            }
            else
            {
                Execute.OnUIThread(() => Publish(message));
            }
        }

        /// <summary>
        /// Publishes a message to any subscribers of a particular message type on the background thread
        /// </summary>
        /// <typeparam name="TMessage">The type of message to publish.</typeparam>
        /// <param name="message">The message to be published</param>
        public void PublishAsync<TMessage>(TMessage message)
        {
            Task.Factory.StartNew(() => Publish(message));
        }

        /// <summary>
        /// Determines if a handler is waiting for a given message
        /// </summary>
        /// <typeparam name="TMessage">The message</typeparam>
        /// <returns>true; if handled, otherwise false</returns>
        public bool IsMessageHandled<TMessage>()
        {
            var subscribers = RefreshAndGetSubscribers<TMessage>();

            return subscribers.Count > 0;
        }

		private List<Action<TMessage>> RefreshAndGetSubscribers<TMessage>()
		{
			var toCall = new List<Action<TMessage>>();
			var messageType = typeof(TMessage);
			var toRemove = new List<WeakAction>();

			lock (_lock)
			{
				if (_subscribers.ContainsKey(messageType))
				{
					var handlers = _subscribers[messageType];
					foreach (var handler in handlers)
					{
						if (handler.IsAlive)
						{
							toCall.Add((Action<TMessage>)handler.Target);
						}
						else
						{
							toRemove.Add(handler);
						}
					}

					foreach (var remove in toRemove)
					{
						handlers.Remove(remove);
					}

					if (handlers.Count == 0)
					{
						_subscribers.Remove(messageType);
					}
				}
			}

			return toCall;
		}
		
		
        private List<WeakAction> RefreshAndGetSubscribers(Type messageType)
        {
			var toCall = new List<WeakAction>();
            var toRemove = new List<WeakAction>();

            lock (_lock)
            {
				if (_subscribers.ContainsKey(messageType))
                {
					var handlers = _subscribers[messageType];
                    foreach (var handler in handlers)
                    {
                        if (handler.IsAlive)
                        {
                            toCall.Add(handler);
                        }
                        else
                        {
                            toRemove.Add(handler);
                        }
                    }

                    foreach (var remove in toRemove)
                    {
                        handlers.Remove(remove);
                    }

                    if (handlers.Count == 0)
                    {
						_subscribers.Remove(messageType);
                    }
                }
            }

            return toCall;
        }

        /// <summary>
        /// Unsubscribes the handler from all message notifications
        /// </summary>
        /// <param name="handler">The handler.</param>
        public void Unsubscribe(IDisposed handler)
        {
            lock (_lock)
            {
                var toRemove = new List<Type>();
                foreach (var key in _subscribers.Keys)
                {
                    var value = _subscribers[key];
                    value.RemoveAll(ar => ar.Target.Target == handler);
                    if (value.Count == 0)
                    {
                        toRemove.Add(key);
                    }
                }

                foreach (var key in toRemove)
                {
                    _subscribers.Remove(key);
                }
            }
        }
    }
}
