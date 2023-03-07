using CsvConverter.Domain.Repositories;

namespace CsvConverter.Domain.Entities
{
    /// <summary>
    /// Csvファイルを扱うためのクラス
    /// インターフェースを持ち、外部とやり取りを行う
    /// 完全コンストラクタパターンにはなりません。
    /// </summary>
    public class CsvFileEntity
    {
        private ICsvFileRepository _csvFileRepository;
        public string CsvFilePath { get; }

        public CsvFileEntity(string csvFilePath)
            : this(new CsvFileAccess(), csvFilePath)
        {

        }

        public CsvFileEntity(ICsvFileRepository csvFileRepository, string csvFilePath)
        {
            _csvFileRepository = csvFileRepository;
            CsvFilePath = csvFilePath;
        }

        public string GetData()
        {
            return _csvFileRepository.GetData();
        }

        public void WriteData(string data)
        {
            _csvFileRepository.WriteData(data);
        }

    }
}
