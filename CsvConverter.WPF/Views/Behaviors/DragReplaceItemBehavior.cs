using CsvConverter.WPF.ViewModels;
using Microsoft.Xaml.Behaviors;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace CsvConverter.WPF.Views.Behaviors
{
    public class DragReplaceItemBehavior : Behavior<ItemsControl>
    {
        private DragDropObject _dragDropObject = null;

        public ICommand Callback
        {
            get { return (ICommand)GetValue(CallbackProperty); }
            set { SetValue(CallbackProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MyProperty.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CallbackProperty =
            DependencyProperty.Register(
                nameof(Callback),
                typeof(ICommand),
                typeof(DragReplaceItemBehavior),
                new PropertyMetadata(null)
            );

        protected override void OnAttached()
        {
            base.OnAttached();

            AssociatedObject.PreviewMouseLeftButtonDown += OnPreviewMouseLeftButtonDown;
            AssociatedObject.PreviewMouseMove += OnPreviewMouseMove;
            AssociatedObject.PreviewMouseLeftButtonUp += OnPreviewMouseLeftButtonUp;
            AssociatedObject.PreviewDragEnter += OnPreviewDragEnter;
            AssociatedObject.PreviewDragLeave += OnPreviewDragLeave;
            AssociatedObject.PreviewDrop += OnPreviewDrop;
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();

            AssociatedObject.PreviewMouseLeftButtonDown -= OnPreviewMouseLeftButtonDown;
            AssociatedObject.PreviewMouseMove -= OnPreviewMouseMove;
            AssociatedObject.PreviewMouseLeftButtonUp -= OnPreviewMouseLeftButtonUp;
            AssociatedObject.PreviewDragEnter -= OnPreviewDragEnter;
            AssociatedObject.PreviewDragLeave -= OnPreviewDragLeave;
            AssociatedObject.PreviewDrop -= OnPreviewDrop;
        }

        private void OnPreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var control = sender as FrameworkElement;
            var startPosition = e.GetPosition(Window.GetWindow(control));
            var draggedItem = GetTemplatedRootElement(e.OriginalSource as FrameworkElement);
            _dragDropObject = new DragDropObject(startPosition, draggedItem);
        }

        private void OnPreviewMouseMove(object sender, MouseEventArgs e)
        {
            if (_dragDropObject is null)
            {
                return;
            }
            var control = sender as FrameworkElement;
            var current = e.GetPosition(Window.GetWindow(control));
            if (_dragDropObject.CheckStartDragging(current))
            {
                DragDrop.DoDragDrop(control, _dragDropObject.DraggedItem, DragDropEffects.Move);
                _dragDropObject = null;
            }
        }

        private void OnPreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            _dragDropObject = null;
        }

        private void OnPreviewDragEnter(object sender, DragEventArgs e)
        {
            if (_dragDropObject is null)
            {
                return;
            }
            _dragDropObject.IsDroppable = true;
        }
        private void OnPreviewDragLeave(object sender, DragEventArgs e)
        {
            if (_dragDropObject is null)
            {
                return;
            }
            _dragDropObject.IsDroppable = false;
        }

        private void OnPreviewDrop(object sender, DragEventArgs e)
        {

            if (_dragDropObject is null)
            {
                return;
            }

            if (_dragDropObject.IsDroppable)
            {
                var itemsControl = sender as ItemsControl;

                if (itemsControl.ItemContainerGenerator.IndexFromContainer(_dragDropObject.DraggedItem) >= 0)
                {
                    var targetContainer = GetTemplatedRootElement(e.OriginalSource as FrameworkElement);
                    var index = itemsControl.ItemContainerGenerator.IndexFromContainer(targetContainer);
                    if (index >= 0)
                    {
                        if (Callback is null)
                        {
                            return;
                        }

                        if (Callback.CanExecute(index))
                        {
                            Callback.Execute(index);
                        }
                    }
                }
            }
        }


        private static FrameworkElement GetTemplatedRootElement(FrameworkElement element)
        {
            var parent = element.TemplatedParent as FrameworkElement;
            while (parent.TemplatedParent != null)
            {
                parent = parent.TemplatedParent as FrameworkElement;
            }
            return parent;
        }

    }
    internal class DragDropObject
    {
        private static readonly Vector MinimumDragPoint
            = new Vector(
                SystemParameters.MinimumHorizontalDragDistance,
                SystemParameters.MinimumVerticalDragDistance
            );
        private Point startPosition;

        public DragDropObject(Point startPosition, FrameworkElement draggedItem)
        {
            this.startPosition = startPosition;
            DraggedItem = draggedItem;
        }

        public Point Start { get; }

        public FrameworkElement DraggedItem { get; }

        public bool IsDroppable { get; set; }

        public bool CheckStartDragging(Point current)
        {
            return (current - Start).Length - MinimumDragPoint.Length > 0;
        }
    }
}
