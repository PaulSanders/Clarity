using NUnit.Framework;
using System;
using System.Threading;
using Clarity;

namespace Clarity.Tests
{
    [TestFixture]
    public class MessageBusTests : Disposable
    {
        [Test]
        public void TestMessagePublished()
        {
            var messageBus = new MessageBus();
            _action1Counter = 0;
            messageBus.Subscribe<MyMessage>(Action1Counter);
            messageBus.Publish(new MyMessage());

            Assert.IsTrue(_action1Counter == 1, "Message not fired");
        }

        [Test]
        public void TestMessageSubscriptionIsToggled()
        {
            var messageBus = new MessageBus();

            messageBus.ToggleSubscription<MyMessage>(Action1Counter, true);
            Assert.IsTrue(messageBus.IsMessageHandled<MyMessage>());

            messageBus.ToggleSubscription<MyMessage>(Action1Counter, false);
            Assert.IsFalse(messageBus.IsMessageHandled<MyMessage>());
        }

        [Test]
        public void TestSubscribingWithNullHandlerThrowsException()
        {
            try
            {
                var messageBus = new MessageBus();
                _action1Counter = 0;
                messageBus.Subscribe<MyMessage>(null);
            }
            catch (ArgumentNullException ex)
            {
                StringAssert.IsMatch("action", ex.ParamName);
                return;
            }

            Assert.Fail();
        }

        [Test]
        public void TestSubscribingWithHanlderOnNonDisposableTargetThrowsException()
        {
            try
            {
                var messageBus = new MessageBus();
                var handler = new TestHandler();
                handler.Handle(messageBus);
            }
            catch (InvalidOperationException)
            {
                Assert.True(true);
                return;
            }

            Assert.Fail();
        }

        [Test]
        public void TestMessagePublishedWithMultipleHandlers()
        {
            var messageBus = new MessageBus();
            _action1Counter = 0;

            messageBus.Subscribe<MyMessage>(Action1Counter);
            messageBus.Subscribe<MyMessage>(OtherAction1Counter);
            messageBus.Publish(new MyMessage());

            Assert.IsTrue(_action1Counter == 2, "Expected two handlers to fire");
        }

        [Test]
        public void TestMessagePublishedAsync()
        {
            var messageBus = new MessageBus();

            messageBus.Subscribe<MyMessage>(AsyncHandler);
            var msg = new MyMessage() { Done = new AutoResetEvent(false) };

            messageBus.PublishAsync(msg);

            var result = msg.Done.WaitOne(TimeSpan.FromSeconds(2));

            Assert.IsTrue(result, "Handler not called");
        }

        [Test]
        [RequiresSTA]
        public void TestMessagePublishedOnUIThreadFromUIThreadDoesNotMakeThreadedCall()
        {
            var messageBus = new MessageBus();

            messageBus.Subscribe<MyMessage>(SetMessageHandler);
            var msg = new MyMessage() { Done = new AutoResetEvent(false) };

            messageBus.PublishOnUIThread(msg);

            Assert.IsTrue(msg.SomeProperty, "Handler not called");
        }

        private int _action1Counter;
        private int _action2Counter;

        [Test]
        public void TestMessageUnsubscribes()
        {
            var messageBus = new MessageBus();
            _action1Counter = 0;

            messageBus.Subscribe<MyMessage>(Action1Counter);
            messageBus.Publish(new MyMessage());
            messageBus.Unsubscribe<MyMessage>(Action1Counter);
            messageBus.Publish(new MyMessage());

            Assert.AreEqual(1, _action1Counter, "Message not fired for handler 1");
        }

        [Test]
        public void TestMessagePublishesToCorrectHandler()
        {
            var messageBus = new MessageBus();
            _action1Counter = 0;
            _action2Counter = 0;

            messageBus.Subscribe<MyMessage>(Action1Counter);
            messageBus.Subscribe<MyOtherMessage>(Action2Counter);

            messageBus.Publish(new MyMessage());
            messageBus.Publish(new MyOtherMessage());
            messageBus.Publish(new MyOtherMessage());

            Assert.AreEqual(1, _action1Counter, "Expected handler 1 to be fired 1 time");
            Assert.AreEqual(2, _action2Counter, "Expected handler 2 to be fired 2 times");
        }

        [Test]
        public void TestCorrectMessageUnsubscribesWithMoreThanOneSubscription()
        {
            var messageBus = new MessageBus();
            _action1Counter = 0;
            _action2Counter = 0;

            messageBus.Subscribe<MyMessage>(Action1Counter);
            messageBus.Subscribe<MyOtherMessage>(Action2Counter);

            messageBus.Publish(new MyMessage());
            messageBus.Publish(new MyOtherMessage());
            messageBus.Publish(new MyOtherMessage());

            Assert.AreEqual(1, _action1Counter, "Expected handler 1 to be fired 1 time");
            Assert.AreEqual(2, _action2Counter, "Expected handler 2 to be fired 2 times");

            messageBus.Unsubscribe<MyMessage>(Action1Counter);

            messageBus.Publish(new MyMessage());
            Assert.AreEqual(1, _action1Counter, "Expected handler 1 to not increment counter");
        }

        [Test]
        public void TestMessagesUnsubscribedWhenHandlerUnsubscribed()
        {
            var messageBus = new MessageBus();
            _action1Counter = 0;
            _action2Counter = 0;

            messageBus.Subscribe<MyMessage>(Action1Counter);
            messageBus.Subscribe<MyOtherMessage>(Action2Counter);

            messageBus.Publish(new MyMessage());
            messageBus.Publish(new MyOtherMessage());
            messageBus.Publish(new MyOtherMessage());

            Assert.AreEqual(1, _action1Counter, "Expected handler 1 to be fired 1 time");
            Assert.AreEqual(2, _action2Counter, "Expected handler 2 to be fired 2 times");

            messageBus.Unsubscribe(this);

            Assert.IsFalse(messageBus.IsMessageHandled<MyMessage>(), "Expected MyMessage to be unhandled");
            Assert.IsFalse(messageBus.IsMessageHandled<MyOtherMessage>(), "Expected MyOtherMessage to be unhandled");
        }

        [Test]
        public void TestMessageUnsubscribesWhenControllerIsDisposed()
        {
            var messageBus = new MessageBus();
            using (var controller = new MyTestController(messageBus))
            {
                messageBus.Publish(new MyMessage());
                Assert.IsTrue(controller.MessageFired, "Message not handled");
                controller.MessageFired = false;
            }

            Assert.IsFalse(messageBus.IsMessageHandled<MyMessage>(), "Message is still handled!");
        }

        [Test]
        public void TestMessageUnsubscribesWhenControllerIsDisposedManually()
        {
            var messageBus = new MessageBus();
            var controller = new MyTestController(messageBus);
            messageBus.Publish(new MyMessage());
            Assert.IsTrue(controller.MessageFired, "Message not handled");
            controller.MessageFired = false;
            controller.Dispose();

            Assert.IsFalse(messageBus.IsMessageHandled<MyMessage>(), "Message is still handled!");
        }

        [Test]
        public void TestMessageIsStillSubscribedWhenControllerIsSetToNull()
        {
            var messageBus = new MessageBus();
            var controller = new MyTestController(messageBus);
            messageBus.Publish(new MyMessage());
            Assert.IsTrue(controller.MessageFired, "Message not handled");
            controller.MessageFired = false;
            controller = null;

            Assert.IsTrue(messageBus.IsMessageHandled<MyMessage>(), "Message is unhandled!");
        }

        private void Action1Counter(MyMessage msg)
        {
            _action1Counter++;
        }

        private void OtherAction1Counter(MyMessage msg)
        {
            _action1Counter++;
        }

        private void Action2Counter(MyOtherMessage msg)
        {
            _action2Counter++;
        }

        private void AsyncHandler(MyMessage msg)
        {
            msg.Done.Set();
        }

        private void SetMessageHandler(MyMessage msg)
        {
            msg.SomeProperty = true;
            msg.Done.Set();
        }
    }

    internal class TestHandler
    {
        internal void Handle(MessageBus messageBus)
        {
            messageBus.Subscribe<MyMessage>(HandleMessage);
        }

        private void HandleMessage(MyMessage msg)
        {
        }
    }

    public class MyTestController : Disposable
    {
        private IMessageBus _messageBus;

        public MyTestController(IMessageBus messagebus)
        {
            _messageBus = messagebus;
            _messageBus.Subscribe<MyMessage>(HandleMessage);
        }

        public void HandleMessage(MyMessage msg)
        {
            MessageFired = true;
        }

        public bool MessageFired { get; set; }
    }

    public class MyMessage
    {
        public AutoResetEvent Done { get; set; }

        private bool _someProperty;
        public virtual bool SomeProperty
        {
            get
            {
                return _someProperty;
            }

            set
            {
                if (OnlySetOnUIThread && Thread.CurrentThread.GetApartmentState() == ApartmentState.STA)
                {
                    _someProperty = value;
                }
                else if (!OnlySetOnUIThread)
                {
                    _someProperty = value;
                }
            }
        }

        public bool OnlySetOnUIThread { get; set; }
    }

    public class MyOtherMessage
    {
    }
}
