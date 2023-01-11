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
using System.Timers;

namespace Clarity.Internal
{
    internal class ActionInfo : Disposable
    {
        public ActionInfo(Action action)
        {
            action.IfNullThrow("action");
            Action = action;
        }

        public ActionInfo(Action action, TimeSpan delayTime)
        {
            action.IfNullThrow("action");
            delayTime.IfNullThrow("delayTime");

            Action = action;
            DelayTime = delayTime;
        }

        public Action Action { get; private set; }
        public TimeSpan DelayTime { get; private set; }
        public bool ExecuteOnUIThread { get; set; }

        public bool HasDelay
        {
            get
            {
                return DelayTime != TimeSpan.Zero;
            }
        }

        internal void Execute()
        {
            if (HasDelay)
            {
                StopTimer();
                StartTimer();
            }
            else
            {
                if (Action != null)
                {
                    if (!(Action.Target is Disposable) || Action.Target is Disposable && ((Disposable)Action.Target).IsDisposed == false)
                    {
                        Action.Invoke();
                    }
                }
            }
        }

        #region timer
        private Timer _executeTimer;
        private void StopTimer()
        {
            if (_executeTimer != null)
            {
                _executeTimer.Stop();
                _executeTimer.Elapsed -= InvokeAction;
            }
            _executeTimer = null;
        }

        private void StartTimer()
        {
            _executeTimer = new Timer(DelayTime.TotalMilliseconds);
            _executeTimer.Elapsed += InvokeAction;
            _executeTimer.Start();
        }

        void InvokeAction(object sender, ElapsedEventArgs e)
        {
            StopTimer();

            if (!IsDisposed)
            {
                if (ExecuteOnUIThread)
                    Clarity.Execute.OnUIThread(() => Action.Invoke());
                else
                    Action.Invoke();
            }
        }
        #endregion

        protected override void OnDispose()
        {
            base.OnDispose();
            StopTimer();

            Action = null;
        }
    }
}
