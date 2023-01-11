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
namespace Clarity
{
    public abstract class WindowResult : PropertyChangedBase
    {
        private string _text;
        public virtual string Text
        {
            get
            {
                return _text;
            }
            set
            {
                SetValue(ref _text, value, () => Text);
            }
        }

        private bool _isDefault;
        public virtual bool IsDefault
        {
            get
            {
                return _isDefault;
            }
            set
            {
                SetValue(ref _isDefault, value, () => IsDefault);
            }
        }

        private bool _isCancel;
        public virtual bool IsCancel
        {
            get
            {
                return _isCancel;
            }
            set
            {
                SetValue(ref _isCancel, value, () => IsCancel);
            }
        }

        public override bool Equals(object obj)
        {
            if (obj == null) return false;
            var result = obj as WindowResult;

            if (result == null) return false;

            return this.GetType() == result.GetType();
        }

        public static bool operator ==(WindowResult a, WindowResult b)
        {
            if ((object)a == null && (object)b == null) return true;
            if ((object)a == null || (object)b == null) return false;

            return a.GetType() == b.GetType();
        }

        public static bool operator !=(WindowResult a, WindowResult b)
        {
            return !(a == b);
        }

        public override int GetHashCode()
        {
            return this.GetType().GetHashCode();
        }
    }

    public sealed class YesResult : WindowResult
    {
        public YesResult()
        {
            Text = "Yes";
            IsDefault = true;
        }
    }

    public sealed class NoResult : WindowResult
    {
        public NoResult()
        {
            Text = "No";
            IsCancel = true;
        }
    }

    public sealed class OkResult : WindowResult
    {
        public OkResult()
        {
            Text = "Okay";
            IsDefault = true;
        }
    }

    public sealed class CancelResult : WindowResult
    {
        public CancelResult()
        {
            Text = "Cancel";
            IsCancel = true;
        }
    }
}
