using System.Collections.Generic;

namespace CsvConverter.WPF.Services
{
    public class FileDialogSettings : ICommonDialogSettings
    {
        public string InitialDirectory { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;

        public ExtensionFilter Filter { get; set; } = null;

        public int FilterIndex { get; set; } = 0;

        public string FileName { get; set; } = string.Empty;

        public bool Multiselect { get; set; } = false;

        public bool IsFolderPicker { get; set; } = false;

        public List<string> FileNames { get; set; } = new();
    }
}
