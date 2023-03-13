using CsvConverter.Domain.Entities;
using CsvConverter.Domain.Repositories;
using CsvConverter.Domain.ValueObjects;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Unicode;

namespace CsvConverter.Infrastructure.Drive
{
    /// <summary>
    /// 設定ファイルIOリポジトリ
    /// </summary>
    public class SettingDrive : ISettingRepository
    {
        /// <summary>
        /// 設定の読み込み
        /// </summary>
        /// <param name="filePath">ファイルパス</param>
        /// <returns>設定情報</returns>
        /// <exception cref="JsonException"></exception>
        public SettingEntity Read(string filePath)
        {
            string jsonString = File.ReadAllText(filePath);
            var options = new JsonSerializerOptions()
            {
                Encoder = JavaScriptEncoder.Create(UnicodeRanges.All),
                WriteIndented = true,
                Converters =
                {
                    new ColumnTargetConverter(),
                },
            };
            var jsonSetting = JsonSerializer.Deserialize(jsonString, typeof(JsonSettingEntity), options) as JsonSettingEntity;
            if (jsonSetting is null)
            {
                throw new JsonException();
            }
            return jsonSetting.GetSettingEntity();
        }

        /// <summary>
        /// 設定情報保存処理
        /// </summary>
        /// <param name="filePath">ファイルパス</param>
        /// <param name="setting">設定情報</param>
        public void Save(string filePath, SettingEntity setting)
        {
            var options = new JsonSerializerOptions
            {
                Encoder = JavaScriptEncoder.Create(UnicodeRanges.All),
                WriteIndented = true,
                Converters =
                {
                    new ColumnTargetConverter(),
                }
            };
            var jsonEntity = new JsonSettingEntity(setting);
            var jsonString = JsonSerializer.Serialize<object>(jsonEntity, options);
            File.WriteAllText(filePath, jsonString);
        }
    }

    /// <summary>
    /// JsonConverter
    /// ポリモフィズムを使用しているため、変換については独自で実装しています。
    /// </summary>
    public class ColumnTargetConverter : JsonConverter<JsonBaseTargetSettingEntity>
    {
        /// <summary>
        /// データ読み込み
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="typeToConvert"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        /// <exception cref="JsonException"></exception>
        public override JsonBaseTargetSettingEntity? Read(
            ref Utf8JsonReader reader,
            Type typeToConvert,
            JsonSerializerOptions options)
        {
            var readerClone = reader;
            if (readerClone.TokenType != JsonTokenType.StartObject)
            {
                throw new JsonException();
            }

            while (readerClone.Read())
            {
                if (readerClone.TokenType == JsonTokenType.PropertyName)
                {
                    string? propertyName = readerClone.GetString();
                    if (propertyName == nameof(JsonBaseTargetSettingEntity.TargetTypeId))
                    {
                        readerClone.Read();
                        if (readerClone.TokenType != JsonTokenType.Number)
                        {
                            throw new JsonException();
                        }
                        var targetId = readerClone.GetInt32();
                        JsonBaseTargetSettingEntity target = targetId switch
                        {
                            1 => JsonSerializer.Deserialize<JsonInputTargetSettingEntity>(ref reader)!,
                            2 => JsonSerializer.Deserialize<JsonConcatenateTargetSettingEntity>(ref reader)!,
                            _ => throw new JsonException()
                        };
                        return target;
                    }
                }
            }
            throw new JsonException();
        }

        /// <summary>
        /// データ書き込み
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="value"></param>
        /// <param name="options"></param>
        public override void Write(
            Utf8JsonWriter writer,
            JsonBaseTargetSettingEntity value,
            JsonSerializerOptions options)
        {
            if (value.GetType() == typeof(JsonInputTargetSettingEntity))
            {
                JsonSerializer.Serialize(writer, value, typeof(JsonInputTargetSettingEntity), options);
                return;
            }
            if (value.GetType() == typeof(JsonConcatenateTargetSettingEntity))
            {
                JsonSerializer.Serialize(writer, value, typeof(JsonConcatenateTargetSettingEntity), options);
                return;
            }
            JsonSerializer.Serialize(writer, value, typeof(JsonBaseTargetSettingEntity), options);
        }
    }

    /// <summary>
    /// Json用設定管理Entity
    /// Jsonで必要になる処理について、Domainには含めず、Infrastractureで対応するためのクラス
    /// </summary>
    public class JsonSettingEntity
    {
        private SettingEntity _settingEntity;

        public string SettingName { get; set; }

        public IReadOnlyList<HeaderEntity> Headers { get; set; }

        public JsonOutputSettingEntity OutputSetting { get; set; }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public JsonSettingEntity()
            : this(new SettingEntity())
        {

        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="settingEntity"></param>
        public JsonSettingEntity(SettingEntity settingEntity)
        {
            _settingEntity = settingEntity;
            SettingName = _settingEntity.SettingName;
            Headers = _settingEntity.Headers;
            OutputSetting = new JsonOutputSettingEntity(_settingEntity.OutputSetting);
        }

        /// <summary>
        /// Entityの取得
        /// </summary>
        /// <returns></returns>
        public SettingEntity GetSettingEntity()
        {
            return new SettingEntity(
                SettingName,
                Headers,
                OutputSetting.GetOutputSettingEntity());
        }
    }

    public class JsonOutputSettingEntity
    {
        private OutputSettingEntity _outputSettingEntity;

        public IReadOnlyList<JsonOutputColumnSettingEntity> ColumnSettings { get; set; }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public JsonOutputSettingEntity()
            : this(OutputSettingEntity.None)
        {

        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="outputSettingEntity"></param>
        public JsonOutputSettingEntity(OutputSettingEntity outputSettingEntity)
        {
            _outputSettingEntity = outputSettingEntity;
            var columns = new List<JsonOutputColumnSettingEntity>();
            foreach (var columnEntity in outputSettingEntity.ColumnSettings)
            {
                columns.Add(new JsonOutputColumnSettingEntity(columnEntity));
            }
            ColumnSettings = columns;
        }

        /// <summary>
        /// Entityの取得
        /// </summary>
        /// <returns></returns>
        public OutputSettingEntity GetOutputSettingEntity()
        {
            var columnEntities = new List<OutputColumnSettingEntity>();
            foreach (var entity in ColumnSettings)
            {
                columnEntities.Add(entity.GetOutputColumnSettingEntity());
            }

            return new OutputSettingEntity(columnEntities);
        }
    }

    public class JsonOutputColumnSettingEntity
    {
        public int Index { get; set; }
        public bool IsOutput { get; set; }
        public string OutputHeader { get; set; }
        public JsonBaseTargetSettingEntity TargetSetting { get; set; }

        /// <summary>
        /// コンストラクタ
        /// TODO:Jsonでの変換で抽象クラスの実装や、オブジェクトのnull許容が必要になってしまうのでちょっと書き方を変えたい
        /// </summary>
        public JsonOutputColumnSettingEntity() { }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="outputColumnSettingEntity"></param>
        public JsonOutputColumnSettingEntity(OutputColumnSettingEntity outputColumnSettingEntity)
        {
            Index = outputColumnSettingEntity.Index;
            IsOutput= outputColumnSettingEntity.IsOutput;
            OutputHeader = outputColumnSettingEntity.OutputHeader;
            TargetSetting = JsonTargetSettingEntityFactory.Create(outputColumnSettingEntity.TargetSetting);
        }

        /// <summary>
        /// Entity取得
        /// </summary>
        /// <returns></returns>
        public OutputColumnSettingEntity GetOutputColumnSettingEntity()
        {
            return new OutputColumnSettingEntity(
                Index, OutputHeader, IsOutput, TargetSetting.GetBaseTargetSettingEntity());
        }
    }


    public abstract class JsonBaseTargetSettingEntity
    {

        public int TargetTypeId { get; set; }

        /// <summary>
        /// コンストラクタ
        /// TargetTypeIdを新たに設定してファイルでの識別はこのIDで行う
        /// </summary>
        /// <param name="targetTypeId"></param>
        public JsonBaseTargetSettingEntity(int targetTypeId)
        {
            TargetTypeId = targetTypeId;
        }

        /// <summary>
        /// Entity取得
        /// </summary>
        /// <returns></returns>
        public abstract BaseTargetSettingEntity GetBaseTargetSettingEntity();
    }

    public static class JsonTargetSettingEntityFactory
    {
        public static JsonBaseTargetSettingEntity Create(BaseTargetSettingEntity entity)
        {
            if (entity is InputTargetSettingEntity inputTargetSettingEntity)
            {
                return new JsonInputTargetSettingEntity(inputTargetSettingEntity);
            }
            if (entity is ConcatenateTargetSettingEntity concatenateTargetSettingEntity)
            {
                return new JsonConcatenateTargetSettingEntity(concatenateTargetSettingEntity);
            }
            throw new NotImplementedException();
        }
    }

    public class JsonInputTargetSettingEntity : JsonBaseTargetSettingEntity
    {
        public string InputHeaderName { get; set; }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public JsonInputTargetSettingEntity()
            : base(TargetSettingType.Input.Value)
        {
            InputHeaderName = string.Empty;
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="targetSettingEntity"></param>
        public JsonInputTargetSettingEntity(InputTargetSettingEntity targetSettingEntity)
            : base(TargetSettingType.Input.Value)
        {
            InputHeaderName = targetSettingEntity.InputHeaderName;
        }

        /// <summary>
        /// Entity取得
        /// </summary>
        /// <returns></returns>
        public override BaseTargetSettingEntity GetBaseTargetSettingEntity()
        {
            return new InputTargetSettingEntity(InputHeaderName);
        }
    }

    public class JsonConcatenateTargetSettingEntity : JsonBaseTargetSettingEntity
    {
        public List<HeaderEntity> OutputHeaderEntities { get; set; }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public JsonConcatenateTargetSettingEntity()
            : base(TargetSettingType.Concatenate.Value)
        {
            OutputHeaderEntities = new List<HeaderEntity>();
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="targetSettingEntity"></param>
        public JsonConcatenateTargetSettingEntity(ConcatenateTargetSettingEntity targetSettingEntity)
            : base(TargetSettingType.Concatenate.Value)
        {
            OutputHeaderEntities = targetSettingEntity.OutputHeaderEntities;
        }

        /// <summary>
        /// Entity取得
        /// </summary>
        /// <returns></returns>
        public override BaseTargetSettingEntity GetBaseTargetSettingEntity()
        {
            return new ConcatenateTargetSettingEntity(OutputHeaderEntities);
        }
    }
}
