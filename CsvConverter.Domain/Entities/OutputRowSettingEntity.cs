namespace CsvConverter.Domain.Entities
{
    public class OutputRowSettingEntity
    {
        public int Index { get; }

        public bool IsInputField { get; }

        public string InputHeader { get; }

        public bool IsOutput { get; }

        public OutputRowSettingEntity(int index, bool isInputField, string inputHeader, bool isOutput)
        {
            Index = index;
            IsInputField = isInputField;
            InputHeader = inputHeader;
            IsOutput = isOutput;
        }
    }
}
