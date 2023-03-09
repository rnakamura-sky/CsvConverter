namespace CsvConverter.Domain.Entities
{
    /// <summary>
    /// CSVファイルの情報を管理するEntity
    /// </summary>
    public sealed class FileDataEntity
    {

        /// <summary>
        /// ヘッダー情報
        /// </summary>
        public IReadOnlyList<HeaderEntity> Headers { get; }

        /// <summary>
        /// 行情報
        /// </summary>
        public IReadOnlyList<RowEntity> Data { get; }

        /// <summary>
        /// ヘッダー有無
        /// </summary>
        public bool HasHeader { get; }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public FileDataEntity()
            : this(new List<HeaderEntity>(), new List<RowEntity>())
        {
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="headers">ヘッダー情報</param>
        /// <param name="data">行情報</param>
        public FileDataEntity(IReadOnlyList<HeaderEntity> headers, IReadOnlyList<RowEntity> data)
        {
            ////TODO:それぞれのコンストラクタで設定しているため、共通化が必要
            HasHeader = true;

            Headers = headers;
            Data = data;
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="fileString">CSVファイル全てのデータ</param>
        public FileDataEntity(string fileString)
        {
            HasHeader = true;

            var rowStrings = fileString.Split(Environment.NewLine);
            var isHeader = true;
            Headers = new List<HeaderEntity>();

            var rows = new List<RowEntity>();
            foreach (var rowString in rowStrings)
            {
                if (isHeader)
                {
                    var headerStrings = rowString.Split(",");
                    var headers = new List<HeaderEntity>();
                    var headerIndex = 0;
                    foreach(var headerString in headerStrings)
                    {
                        headers.Add(new HeaderEntity(headerIndex, headerString));
                        headerIndex++;
                    }
                    Headers = headers;
                    isHeader = false;
                    continue;
                }

                ////空白行は無視する
                if (rowString == string.Empty)
                {
                    continue;
                }

                var dataStrings = rowString.Split(",");
                var datas = new List<FieldEntity>();
                var dataIndex = 0;
                foreach(var dataString in dataStrings)
                {
                    var header = Headers is null ? HeaderEntity.None : Headers[dataIndex];
                    datas.Add(new FieldEntity(header, dataString));
                    dataIndex++;
                }
                rows.Add(new RowEntity(datas));
            }
            Data = rows;
        }

        /// <summary>
        /// ファイル情報を文字列として取得
        /// </summary>
        /// <returns></returns>
        public string GetFileString()
        {
            var result = string.Empty;
            
            if (HasHeader)
            {
                var isFirst = true;
                foreach(var header in Headers)
                {
                    if (isFirst)
                    {
                        result += header.HeaderName;
                        isFirst = false;
                        continue;
                    }
                    result += "," + header.HeaderName;
                }
                result += Environment.NewLine;
            }
            foreach(var row in Data)
            {
                var isFirst = true;
                foreach(var field in row.Fields)
                {
                    if (isFirst)
                    {
                        result += field.FieldValue;
                        isFirst = false;
                        continue;
                    }
                    result += "," + field.FieldValue;
                }
                result += Environment.NewLine;
            }

            return result;
        }
    }
}
