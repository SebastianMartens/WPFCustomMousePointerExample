using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace WpfMousePointerExample
{
    // ##############################################
    // Disclaimer: 
    // Many thanks to Sean Sexton explaining drag drop stuff in his blog here:
    // http://wpf.2000things.com/tag/drag-and-drop/
    //
    // These examples are based on his work.
    // ##############################################

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Label_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            // drag drop has some payload. Here we simply use the text of the control:
            var data = new DataObject(DataFormats.Text, ((Label)e.Source).Content);
            DragDrop.DoDragDrop((DependencyObject)e.Source, data, DragDropEffects.Copy);
        }

        private void Label_Drop(object sender, DragEventArgs e)
        {
            ((Label)e.Source).Content = e.Data.GetData(DataFormats.Text);
        }

        private Cursor _customCursor;

        /// <summary>
        /// The GiveFeedback event occurs when a drag-and-drop operation is started. 
        /// The GiveFeedback event enables the source of a drag event to modify the appearance of the mouse pointer 
        /// to provide the user with visual feedback during a drag-and-drop operation.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Label_GiveFeedback(object sender, GiveFeedbackEventArgs e)
        {
            switch (e.Effects)
            {                                                
                case DragDropEffects.None:
                    if ((sender as Label) != null)
                    {
                        // Example 1: change mouse cursor to custom text:
                        // -----------------------------------------------
                        //var cursorText = ((Label)sender).Content.ToString();
                        //if (_customCursor == null)
                        //    _customCursor = CursorHelper.CreateCursor(cursorText);

                        // Example 2: change mouse cursor to WPF-UIElement (as image):
                        if (_customCursor == null)
                            _customCursor = CursorHelper.CreateCursor(e.Source as UIElement);

                        e.UseDefaultCursors = false;
                        Mouse.SetCursor(_customCursor);
                    }
                    break;
                default: e.UseDefaultCursors = true;
                    break;
            }
        
            e.Handled = true;
        }
    }
}
