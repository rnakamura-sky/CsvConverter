namespace CsvConverter.Domain.Entities
{
    public sealed class FileDataEntity
    {
        public string FileString { get; }

        public IReadOnlyList<HeaderEntity> Headers { get; }

        public IReadOnlyList<RowEntity> Data { get; }

        public bool HasHeader { get; }

        public FileDataEntity()
        {
            Headers = new List<HeaderEntity>();
            Data = new List<RowEntity>();
            FileString= string.Empty;
        }

        public FileDataEntity(string fileString)
        {
            HasHeader = true;
            FileString = fileString;

            var rowStrings = FileString.Split("\r\n");
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
            return FileString;
        }
    }
}
