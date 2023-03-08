namespace CsvConverter.Domain.Repositories
{
    /// <summary>
    /// CSVファイルリポジトリ
    /// </summary>
    public interface ICsvFileRepository : IInputCsvFileRepository, IOutputCsvFileRepository
    {
    }
}
