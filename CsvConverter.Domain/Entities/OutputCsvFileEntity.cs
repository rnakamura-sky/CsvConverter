using CsvConverter.Domain.Repositories;

namespace CsvConverter.Domain.Entities
{
    /// <summary>
    /// 出力ファイル情報を管理するEntity
    /// </summary>
    public sealed class OutputCsvFileEntity
    {
        /// <summary>
        /// ファイルIOを行うリポジトリ
        /// </summary>
        private IOutputCsvFileRepository _outputCsvFileRepository;

        /// <summary>
        /// ファイルパス
        /// </summary>
        public string CsvFilePath { get; }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="outputCsvFilePath">ファイルパス</param>
        public OutputCsvFileEntity(string outputCsvFilePath)
            : this(new CsvFileAccess(), outputCsvFilePath)
        {
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="outputCsvFileRepository">ファイルIOリポジトリ</param>
        /// <param name="outputCsvFilePath">ファイルパス</param>
        public OutputCsvFileEntity(IOutputCsvFileRepository outputCsvFileRepository, string outputCsvFilePath)
        {
            _outputCsvFileRepository = outputCsvFileRepository;
            CsvFilePath = outputCsvFilePath;
        }

        /// <summary>
        /// ファイルの情報を書き込む
        /// </summary>
        /// <param name="data"></param>
        public void WriteData(FileDataEntity data)
        {
            var fileString = data.GetFileString();
            _outputCsvFileRepository.WriteData(CsvFilePath, fileString);
        }
    }
}
