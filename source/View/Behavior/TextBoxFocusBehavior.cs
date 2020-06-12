using System.Windows;
using System.Windows.Controls;
using System.Windows.Interactivity;

namespace HttpHeadersViewer.View.Behavior
{
    internal class TextBoxFocusBehavior : Behavior<TextBox>
    {
        #region SubscribeDescribeEvents
        protected override void OnAttached()
        {
            AssociatedObject.IsEnabledChanged += OnEnableChanged;
            AssociatedObject.TextChanged += OnTextChanged;
        }

        protected override void OnDetaching()
        {
            AssociatedObject.IsEnabledChanged -= OnEnableChanged;
            AssociatedObject.TextChanged -= OnTextChanged;
        }
        #endregion

        #region EventHandlers
        private void OnEnableChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            Focus(sender);
        }

        private void OnTextChanged(object sender, TextChangedEventArgs e)
        {
            Focus(sender);
        }
        #endregion

        #region Methods
        private static void Focus(object sender)
        {
            if (sender is TextBox textBox)
            {
                textBox.Focus();
            }
        }
        #endregion
    }
}
