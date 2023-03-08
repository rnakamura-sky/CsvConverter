namespace CsvConverter.Domain.Entities
{
    public sealed class FileDataEntity
    {

        public IReadOnlyList<HeaderEntity> Headers { get; }

        public IReadOnlyList<RowEntity> Data { get; }

        public bool HasHeader { get; }

        public FileDataEntity()
            : this(new List<HeaderEntity>(), new List<RowEntity>())
        {
        }

        public FileDataEntity(IReadOnlyList<HeaderEntity> headers, IReadOnlyList<RowEntity> data)
        {
            ////TODO:それぞれのコンストラクタで設定しているため、共通化が必要
            HasHeader = true;

            Headers = headers;
            Data = data;
        }

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
                        result += header.Header;
                        isFirst = false;
                        continue;
                    }
                    result += "," + header.Header;
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
                        result += field.Data;
                        isFirst = false;
                        continue;
                    }
                    result += "," + field.Data;
                }
                result += Environment.NewLine;
            }

            return result;
        }
    }
}
