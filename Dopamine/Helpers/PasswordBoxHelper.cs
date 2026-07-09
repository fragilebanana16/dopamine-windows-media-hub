using System.Windows;
using System.Windows.Controls;

namespace Dopamine.Helpers
{
    public static class PasswordBoxHelper
    {
        public static readonly DependencyProperty BindablePasswordProperty =
            DependencyProperty.RegisterAttached("BindablePassword", typeof(string), typeof(PasswordBoxHelper),
                new FrameworkPropertyMetadata(string.Empty, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnBindablePasswordChanged));

        public static string GetBindablePassword(DependencyObject obj) => (string)obj.GetValue(BindablePasswordProperty);
        public static void SetBindablePassword(DependencyObject obj, string value) => obj.SetValue(BindablePasswordProperty, value);

        private static void OnBindablePasswordChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is PasswordBox passwordBox)
            {
                passwordBox.PasswordChanged -= PasswordBox_PasswordChanged;
                if (passwordBox.Password != (string)e.NewValue)
                {
                    passwordBox.Password = (string)e.NewValue ?? string.Empty;
                }
                passwordBox.PasswordChanged += PasswordBox_PasswordChanged;
            }
        }

        private static void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (sender is PasswordBox passwordBox)
            {
                SetBindablePassword(passwordBox, passwordBox.Password);
            }
        }
    }
}
