namespace CsvConverter.Domain.Entities
{
    public sealed class OutputSettingEntity
    {
        public static readonly OutputSettingEntity None = new();

        public IReadOnlyList<OutputColumnSettingEntity> RowSettings { get; }

        private OutputSettingEntity()
        {
            RowSettings= new List<OutputColumnSettingEntity>();
        }


        public OutputSettingEntity(FileDataEntity fileDataEntity)
        {
            var rowSettings = new List<OutputColumnSettingEntity>();
            int index = 0;
            foreach (var header in fileDataEntity.Headers)
            {
                rowSettings.Add(new OutputColumnSettingEntity(index, true, header.Header, true));
                index++;
            }
            RowSettings = rowSettings;
        }

        public OutputSettingEntity(IReadOnlyList<OutputColumnSettingEntity> rowSettings)
        {
            RowSettings = rowSettings;
        }

        public FileDataEntity CreateFileData(FileDataEntity data)
        {
            var headers = new List<HeaderEntity>();
            int index = 0;

            var outputRowSettings = RowSettings.OrderBy(x => x.Index).ToList();
            foreach (var row in outputRowSettings)
            {
                if (row.IsOutput)
                {
                    headers.Add(new HeaderEntity(index, row.InputHeader));
                    index++;
                }
            }

            var outputRowList = new List<RowEntity>();
            foreach (var row in data.Data)
            {
                var outputData = new List<FieldEntity>();
                for (index = 0; index < outputRowSettings.Count; ++index)
                {
                    var outputHeader = headers[index];
                    var outputRowSetting = outputRowSettings[index];

                    var fieldValue = row.GetField(outputRowSetting.InputHeader).Data;
                    outputData.Add(new FieldEntity(outputHeader, fieldValue));
                }
                outputRowList.Add(new RowEntity(outputData));
            }

            return new FileDataEntity(headers, outputRowList);
        }
    }
}
