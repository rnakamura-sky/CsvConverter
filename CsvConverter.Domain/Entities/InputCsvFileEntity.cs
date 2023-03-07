using CsvConverter.Domain.Repositories;

namespace CsvConverter.Domain.Entities
{
    /// <summary>
    /// Csvファイルを扱うためのクラス
    /// インターフェースを持ち、外部とやり取りを行う
    /// 完全コンストラクタパターンにはなりません。
    /// </summary>
    public class InputCsvFileEntity
    {
        private IInputCsvFileRepository _inputCsvFileRepository;
        public string CsvFilePath { get; }

        public InputCsvFileEntity(string csvFilePath)
            : this(new CsvFileAccess(), csvFilePath)
        {

        }

        public InputCsvFileEntity(IInputCsvFileRepository inputCsvFileRepository, string csvFilePath)
        {
            _inputCsvFileRepository = inputCsvFileRepository;
            CsvFilePath = csvFilePath;
        }

        public string GetData()
        {
            return _inputCsvFileRepository.GetData();
        }


    }
}
