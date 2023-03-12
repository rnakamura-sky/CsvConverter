﻿namespace CsvConverter.Domain.Entities
{
    /// <summary>
    /// 出力設定管理Entity
    /// </summary>
    public sealed class OutputSettingEntity
    {
        /// <summary>
        /// None
        /// </summary>
        public static readonly OutputSettingEntity None = new();

        /// <summary>
        /// 出力行設定情報
        /// </summary>
        public IReadOnlyList<OutputColumnSettingEntity> ColumnSettings { get; }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        private OutputSettingEntity()
        {
            ColumnSettings = new List<OutputColumnSettingEntity>();
        }

        /// <summary>
        /// コンストラクタ
        /// 入力されたファイル情報をそのまま出力行設定に登録を行う
        /// </summary>
        /// <param name="InputHeaders">ヘッダー情報</param>
        public OutputSettingEntity(IReadOnlyList<HeaderEntity> InputHeaders)
        {
            var rowSettings = new List<OutputColumnSettingEntity>();
            int index = 0;
            foreach (var header in InputHeaders)
            {
                rowSettings.Add(new OutputColumnSettingEntity(
                                        index,
                                        header.HeaderName,
                                        true,
                                        new InputTargetSettingEntity(header.HeaderName)));
                index++;
            }
            ColumnSettings = rowSettings;
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="columnSettings">列設定</param>
        public OutputSettingEntity(IReadOnlyList<OutputColumnSettingEntity> columnSettings)
        {
            ColumnSettings = columnSettings;
        }

        /// <summary>
        /// ファイル情報作成
        /// 持っている設定と与えられたファイル情報で出力ファイル情報を作成します。
        /// </summary>
        /// <param name="data">ファイル情報</param>
        /// <returns>出力ファイル情報</returns>
        public FileDataEntity CreateFileData(FileDataEntity data)
        {
            var inputHeaders = data.Headers;

            var headers = new List<HeaderEntity>();
            int index = 0;

            var outputRowSettings = ColumnSettings.Where(x => x.IsOutput).OrderBy(x => x.Index).ToList();
            foreach (var row in outputRowSettings)
            {
                if (row.IsOutput)
                {
                    headers.Add(new HeaderEntity(index, row.OutputHeader));
                    index++;
                }
            }

            var outputRowList = new List<RowEntity>();
            foreach (var row in data.Data)
            {
                var outputRowEntity = new RowEntity();
                for (index = 0; index < outputRowSettings.Count; ++index)
                {
                    var outputHeader = headers[index];
                    var outputRowSetting = outputRowSettings[index];

                    var fieldValue = outputRowSetting.CreateFieldValue(row, outputRowEntity);
                    outputRowEntity = outputRowEntity.Add(new FieldEntity(outputHeader, fieldValue));
                }
                outputRowList.Add(outputRowEntity);
            }

            return new FileDataEntity(headers, outputRowList);
        }
    }
}
