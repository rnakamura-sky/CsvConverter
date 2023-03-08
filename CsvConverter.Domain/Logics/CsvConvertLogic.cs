using CsvConverter.Domain.Entities;

namespace CsvConverter.Domain.Logics
{
    public class CsvConvertLogic : ICsvConvertLogic
    {

        public void Execute(InputCsvFileEntity inputCsvFile, OutputCsvFileEntity outputCsvFile, OutputSettingEntity setting)
        {
            var data = inputCsvFile.GetData();

            var outputSetting = setting != OutputSettingEntity.None ? setting : new OutputSettingEntity(data);

            var outputData = outputSetting.CreateFileData(data);

            outputCsvFile.WriteData(outputData);
        }
    }
}
