﻿using CsvConverter.Domain.Repositories;

namespace CsvConverter.Domain.Entities
{
    /// <summary>
    /// Csvファイルを扱うためのクラス
    /// インターフェースを持ち、外部とやり取りを行う
    /// 完全コンストラクタパターンにはなりません。
    /// </summary>
    public sealed class InputCsvFileEntity
    {
        private readonly IInputCsvFileRepository _inputCsvFileRepository;
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

        public FileDataEntity GetData()
        {
            var fileString = _inputCsvFileRepository.GetData(CsvFilePath);
            return new FileDataEntity(fileString);
        }


    }
}
