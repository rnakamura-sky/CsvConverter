using CsvConverter.Domain.Repositories;

namespace CsvConverter.Domain.Entities
{
    /// <summary>
    /// Csvファイルを扱うためのEntity
    /// インターフェースを持ち、外部とやり取りを行う
    /// 完全コンストラクタパターンにはなりません。
    /// </summary>
    public sealed class InputCsvFileEntity
    {
        /// <summary>
        /// ファイルIOアクセスリポジトリ
        /// </summary>
        private readonly IInputCsvFileRepository _inputCsvFileRepository;
        public string CsvFilePath { get; }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="csvFilePath">ファイルパス</param>
        public InputCsvFileEntity(string csvFilePath)
            : this(new CsvFileAccess(), csvFilePath)
        {

        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="inputCsvFileRepository">ファイルアクセスリポジトリ</param>
        /// <param name="csvFilePath">ファイルパス</param>
        public InputCsvFileEntity(IInputCsvFileRepository inputCsvFileRepository, string csvFilePath)
        {
            _inputCsvFileRepository = inputCsvFileRepository;
            CsvFilePath = csvFilePath;
        }

        /// <summary>
        /// ファイル情報取得
        /// </summary>
        /// <returns></returns>

        public FileDataEntity GetData()
        {
            var fileString = _inputCsvFileRepository.GetData(CsvFilePath);
            return new FileDataEntity(fileString);
        }


    }
}
