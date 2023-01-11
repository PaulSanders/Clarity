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
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace Clarity
{
	public abstract class Disposable : IDisposed, ISerializable
	{
		private readonly object _synclock = new object();

		private Guid _objectId;

		public Disposable()
		{
		}

		public Disposable(SerializationInfo info, StreamingContext context)
		{
		}

		~Disposable()
		{
			Dispose();
		}

		[OnSerializing]
		private void OnSerializing(StreamingContext context)
		{
		}

		[OnDeserializing]
		private void OnDeserializing(StreamingContext context)
		{
		}

		/// <summary>
		/// Gets the unique instance identifier of this object
		/// </summary>
		/// <value>
		/// The unique identifier.
		/// </value>
		public Guid ObjectId
		{
			get
			{
				if (_objectId == Guid.Empty)
				{
					_objectId = Guid.NewGuid();
				}

				return _objectId;
			}
		}

		/// <summary>
		/// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
		/// </summary>
		public void Dispose()
		{
			lock (_synclock)
			{
				if (!IsDisposed && !IsDisposing)
				{
					IsDisposing = true;

					OnDispose();
					GC.SuppressFinalize(this);

					IsDisposing = false;
				}

				IsDisposed = true;
			}
		}

		protected IMessageBus MessageBus
		{
			get
			{
				if (!IsDisposed)
				{
					return ServiceManager.Default.TryResolve<IMessageBus>();
				}

				return null;
			}
		}

		/// <summary>
		/// Called when disposing the resource.
		/// </summary>
		protected virtual void OnDispose()
		{
			var mbus = ServiceManager.Default.TryResolve<IMessageBus>();
			if (mbus != null) mbus.Unsubscribe(this);
		}

		/// <summary>
		/// Ensures the object is not disposed.
		/// </summary>
		/// <exception cref="System.ObjectDisposedException">Thrown if it is disposed</exception>
		protected void EnsureNotDisposed()
		{
			if (IsDisposed)
			{
				throw new ObjectDisposedException(this.GetType().Name);
			}
		}

		/// <summary>
		/// Gets a value indicating whether this instance is disposed.
		/// </summary>
		/// <value>
		/// <c>true</c> if this instance is disposed; otherwise, <c>false</c>.
		/// </value>
		[NonSerialized]
		private bool isDisposed;

		[XmlIgnore]
		public bool IsDisposed { get { return isDisposed; } private set { isDisposed = value; } }

		/// <summary>
		/// Gets a value indicating whether this instance is disposing.
		/// </summary>
		/// <value>
		/// <c>true</c> if this instance being disposed; otherwise, <c>false</c>.
		/// </value>
		[NonSerialized]
		private bool isDisposing;

		[XmlIgnore]
		public bool IsDisposing { get { return isDisposing; } private set { isDisposing = value; } }

		public void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			//
		}
	}
}