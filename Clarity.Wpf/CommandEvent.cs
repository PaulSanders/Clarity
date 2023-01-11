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
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace Clarity.Wpf
{
    public static class CommandEvent
    {
        public static ICommand GetCommand(DependencyObject obj)
        {
            return (ICommand)obj.GetValue(CommandProperty);
        }

        public static void SetCommand(DependencyObject obj, ICommand value)
        {
            obj.SetValue(CommandProperty, value);
        }

        // Using a DependencyProperty as the backing store for Command.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CommandProperty = DependencyProperty.RegisterAttached("Command", typeof(ICommand), typeof(CommandEvent), new PropertyMetadata(null));

        /// <summary>
        /// EventName : The event that should actually execute the IClarityCommand
        /// </summary>
        public static readonly DependencyProperty EventNameProperty = DependencyProperty.RegisterAttached("EventName", typeof(string),
            typeof(CommandEvent), new FrameworkPropertyMetadata(string.Empty, new PropertyChangedCallback(OnEventNameChanged)));

        /// <summary>
        /// Gets the EventName property. 
        /// </summary>
        public static string GetEventName(DependencyObject d)
        {
            return (string)d.GetValue(EventNameProperty);
        }

        /// <summary>
        /// Sets the EventName property. 
        /// </summary>
        public static void SetEventName(DependencyObject d, string value)
        {
            d.SetValue(EventNameProperty, value);
        }

        private static void OnEventNameChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var eventName = (string)e.NewValue;

            if (sender == null || string.IsNullOrEmpty(eventName))
                return;

            var eventHooker = new EventObserver();
            eventHooker.UiElement = sender;

            var info = sender.GetType().GetEvent(eventName, BindingFlags.Public | BindingFlags.Instance);

            if (info != null)
            {
                info.RemoveEventHandler(sender, eventHooker.GetEventHandler(info));
                info.AddEventHandler(sender, eventHooker.GetEventHandler(info));
            }
        }

        public static readonly DependencyProperty CommandParameterProperty = DependencyProperty.RegisterAttached("CommandParameter", typeof(object), typeof(CommandEvent), new UIPropertyMetadata(null));

        /// <summary>        
        /// Gets the CommandParameter property.         
        /// </summary>        
        public static object GetCommandParameter(DependencyObject obj)
        {
            return (object)obj.GetValue(CommandParameterProperty);
        }

        /// <summary>        
        /// Sets the CommandParameter property.         
        /// </summary>        
        public static void SetCommandParameter(DependencyObject obj, object value)
        {
            obj.SetValue(CommandParameterProperty, value);
        }

        /// <summary>
        /// Gets the AncestorType property
        /// </summary>
        public static Type GetAncestorType(DependencyObject obj)
        {
            return (Type)obj.GetValue(AncestorTypeProperty);
        }

        /// <summary>
        /// Sets the AncestorType property. This will ensure that the event will only be fired if the source is with a parent of this type
        /// </summary>
        public static void SetAncestorType(DependencyObject obj, Type value)
        {
            obj.SetValue(AncestorTypeProperty, value);
        }

        public static readonly DependencyProperty AncestorTypeProperty =
            DependencyProperty.RegisterAttached("AncestorType", typeof(Type), typeof(CommandEvent), new UIPropertyMetadata(null));
    }

    sealed class EventObserver
    {
        private MethodInfo _method;
        public EventObserver()
        {
            var flags = BindingFlags.Instance | BindingFlags.NonPublic;
            _method = GetType().GetMethod("OnEventRaised", flags);
        }

        /// <summary>
        /// The DependencyObject, that holds a binding to the actual IClarityCommand to execute
        /// </summary>
        public DependencyObject UiElement { get; set; }

        /// <summary>
        /// Creates a delegate EventHandler that will run the IClarityCommand when the RoutedEvent fires
        /// </summary>
        /// <param name="info">The RoutedEvent EventInfo to handle</param>
        /// <returns>A Delegate that points to a new EventHandler that will be execute the IClarityCommand</returns>
        public Delegate GetEventHandler(EventInfo info)
        {
            info.IfNullThrow("eventInfo");

            if (info.EventHandlerType == null) throw new ArgumentException("EventHandlerType is null");

            return Delegate.CreateDelegate(info.EventHandlerType, this, _method);
        }

        /// <summary>
        /// Runs the IClarityCommand when the requested RoutedEvent fires
        /// </summary>
        private void OnEventRaised(object sender, EventArgs e)
        {
            var cmd = (ICommand)(sender as DependencyObject).GetValue(CommandEvent.CommandProperty);
            var cmdParam = (sender as DependencyObject).GetValue(CommandEvent.CommandParameterProperty);
            var ancestorType = (Type)(sender as DependencyObject).GetValue(CommandEvent.AncestorTypeProperty);
            bool okToFire = true;

            //if an ancestorType is specified, then try and find it in the visual tree
            if (ancestorType != null)
            {
                var args = e as RoutedEventArgs;
                if (args != null)
                {
                    var objType = args.OriginalSource.GetType();
                    var obj = args.OriginalSource as DependencyObject;

                    if (obj != null && obj.GetType() != ancestorType)
                    {
                        okToFire = false;

                        while (objType != null && objType != ancestorType)
                        {
                            obj = VisualTreeHelper.GetParent(obj) as DependencyObject;
                            if (obj == null) break;

                            if (obj.GetType() == ancestorType)
                            {
                                okToFire = true;
                                break;
                            }

                            objType = obj.GetType();
                        }
                    }
                }
            }

            if (okToFire)
            {
                if (cmd != null)
                    cmd.Execute(cmdParam);
            }
        }
    }
}