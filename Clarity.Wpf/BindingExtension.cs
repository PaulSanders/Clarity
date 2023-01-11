namespace Sol.Core.WPF
{
    public class BindingExtension : System.Windows.Data.Binding
    {
        public BindingExtension()
        {
            ConverterCulture = System.Globalization.CultureInfo.CurrentCulture;
        }

        public BindingExtension(string path)
        {
            Path = new System.Windows.PropertyPath(path);
            ConverterCulture = System.Globalization.CultureInfo.CurrentCulture;
        }
    }
}
