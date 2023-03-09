using Microsoft.WindowsAPICodePack.Dialogs;
using System.Linq;

namespace CsvConverter.WPF.Services
{
    public class FileDialogService : ICommonDialogService
    {
        public bool ShowDialog(ICommonDialogSettings settings)
        {
            var dialog = CreateDialogService(settings);
            if (dialog == null)
            {
                return false;
            }

            var ret = dialog.ShowDialog();
            if (ret == CommonFileDialogResult.Ok)
            {
                SetReturnValues(dialog, settings);
                return true;
            }
            return false;
        }

        private static CommonOpenFileDialog CreateDialogService(ICommonDialogSettings settings)
        {
            var fileSettings = settings as FileDialogSettings;
            if (fileSettings is null)
            {
                return null;
            }
            var dialog = new CommonOpenFileDialog();
            if (!fileSettings.IsFolderPicker && fileSettings.Filter != null)
            {
                dialog.Filters.Add(
                    new CommonFileDialogFilter(fileSettings.Filter.DisplayName, fileSettings.Filter.ExtensionList));
            }
            dialog.InitialDirectory = fileSettings.InitialDirectory;
            dialog.Title = fileSettings.Title;
            dialog.IsFolderPicker = fileSettings.IsFolderPicker;

            return dialog;
        }

        private static void SetReturnValues(CommonOpenFileDialog dialog, ICommonDialogSettings settings)
        {
            var fileSettings = settings as FileDialogSettings;
            if (fileSettings is null)
            {
                return;
            }
            fileSettings.FileName = dialog.FileName;
            fileSettings.FileNames = dialog.FileNames.ToList();
        }

    }
}
