namespace CsvConverter.WPF.Services
{
    public class ExtensionFilter
    {
        public string DisplayName { get; set; }
        public string ExtensionList { get; set; }

        public ExtensionFilter(string displayName, string extensionList)
        {
            DisplayName = displayName;
            ExtensionList = extensionList;
        }
    }
}
