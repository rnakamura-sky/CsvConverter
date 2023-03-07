using Microsoft.Xaml.Behaviors;
using System.Windows;
using System.Windows.Controls;

namespace CsvConverter.WPF.Views.Behaviors
{
    public class FileDragDropBehavior : Behavior<TextBox>
    {
        protected override void OnAttached()
        {
            base.OnAttached();
            AssociatedObject.PreviewDragOver += DragOverFile;
            AssociatedObject.Drop += DropFile;
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();
            AssociatedObject.PreviewDragOver -= DragOverFile;
            AssociatedObject.Drop -= DropFile;
        }

        private void DragOverFile(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop, true))
            {
                e.Effects = DragDropEffects.Copy;
            }
            else
            {
                e.Effects = DragDropEffects.None;
            }
            e.Handled = true;
        }

        private void DropFile(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop, true))
            {
                var dropFiles = e.Data.GetData(DataFormats.FileDrop) as string[];
                if (dropFiles is null)
                {
                    return;
                }
                AssociatedObject.Text = dropFiles[0];
            }
        }
    }
}
