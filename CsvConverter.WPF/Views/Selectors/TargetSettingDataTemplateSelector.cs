using CsvConverter.Domain.ValueObjects;
using CsvConverter.WPF.ViewModels;
using System.Windows;
using System.Windows.Controls;

namespace CsvConverter.WPF.Views.Selectors
{
    public class TargetSettingDataTemplateSelector : DataTemplateSelector
    {
        public DataTemplate InputTargetDataTemplate { get; set; }
        public DataTemplate ConcatenateTargetDataTemplate { get; set; }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            var viewModel = item as CreateOutputColumnViewModelTargetBase;
            if (viewModel is null)
            {
                return base.SelectTemplate(item, container);
            }

            if (viewModel.TargetSettingType == TargetSettingType.Input)
            {
                return InputTargetDataTemplate;
            }
            if (viewModel.TargetSettingType == TargetSettingType.Concatenate)
            {
                return ConcatenateTargetDataTemplate;
            }

            return base.SelectTemplate(item, container);
        }
    }
}
