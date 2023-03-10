using CsvConverter.Domain.Entities;
using Prism.Mvvm;

namespace CsvConverter.WPF.ViewModels
{
    /// <summary>
    /// ヘッダー情報ViewModel
    /// </summary>
    public class CsvConvertViewModelHeader : BindableBase
    {
        /// <summary>
        /// 出力設定項目Entity
        /// </summary>
        private readonly OutputColumnSettingEntity _entity;

        private string _fieldName;

        /// <summary>
        /// ヘッダー名
        /// </summary>
        public string FieldName
        {
            get { return _fieldName; }
            set { SetProperty(ref _fieldName, value); }
        }

        private bool _isOutput;

        /// <summary>
        /// 出力有無
        /// </summary>
        public bool IsOutput
        {
            get { return _isOutput; }
            set { SetProperty(ref _isOutput, value); }
        }

        private string _fieldContent = string.Empty;
        
        /// <summary>
        /// 項目出力概要
        /// </summary>
        public string FieldContent
        {
            get { return _fieldContent; }
            set { SetProperty(ref _fieldContent, value); }
        }

        private string _intputFieldName = string.Empty;
        
        /// <summary>
        /// 入力項目名
        /// </summary>
        public string InputFieldName
        {
            get { return _intputFieldName; }
            set { SetProperty(ref _intputFieldName, value); }
        }

        public CsvConvertViewModelHeader(OutputColumnSettingEntity entity)
        {
            _entity = entity;
            FieldName = _entity.OutputHeader;
            IsOutput = _entity.IsOutput;
            FieldContent = _entity.GetFieldContent();
            InputFieldName = _entity.GetInputFieldName();
        }

        /// <summary>
        /// 出力行情報Entity取得
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public OutputColumnSettingEntity GetColumnSettingEntity(int index)
        {
            return new OutputColumnSettingEntity(
                index,
                FieldName,
                IsOutput,
                _entity.TargetSetting);
        }

        /// <summary>
        /// ヘッダー情報取得
        /// </summary>
        /// <returns></returns>
        public HeaderEntity GetHeader()
        {
            return new HeaderEntity(0, FieldName);
        }
    }
}
