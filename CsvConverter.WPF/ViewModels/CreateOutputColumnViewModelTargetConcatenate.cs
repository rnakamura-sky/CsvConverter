using CsvConverter.Domain.Entities;
using CsvConverter.Domain.ValueObjects;
using CsvConverter.WPF.ValidationAttributes;
using Prism.Commands;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;

namespace CsvConverter.WPF.ViewModels
{
    /// <summary>
    /// 結合設定用ViewModel
    /// </summary>
    public class CreateOutputColumnViewModelTargetConcatenate : CreateOutputColumnViewModelTargetBase
	{
        /// <summary>
        /// 結合項目選択用リスト
        /// </summary>
        private ObservableCollection<HeaderEntity> _headers;

        /// <summary>
        /// 結合項目選択用リスト
        /// </summary>
        public ObservableCollection<HeaderEntity> Headers
        {
            get { return _headers; }
            set { SetProperty(ref _headers, value); }
        }

        /// <summary>
        /// 結合項目選択
        /// </summary>
        private HeaderEntity _selectedHeader;

        /// <summary>
        /// 結合項目選択
        /// </summary>
        public HeaderEntity SelectedHeader
        {
            get => _selectedHeader;
            set
            {
                if (SetProperty(ref _selectedHeader, value))
                {
                    SelectCommand.RaiseCanExecuteChanged();
                }
            }
        }

        /// <summary>
        /// 結合選択項目リスト
        /// </summary>
        private ObservableCollection<HeaderEntity> _concatenateHeaders = new();

        /// <summary>
        /// 結合選択項目リスト
        /// </summary>
        [Display(Name = "結合項目リスト")]
        [CollectionCount(ErrorMessage = "項目を選択してください。")]
        public ObservableCollection<HeaderEntity> ConcatenateHeaders
        {
            get { return _concatenateHeaders; }
            set { SetValidateProperty(ref _concatenateHeaders, value); }
        }

        /// <summary>
        /// 結合選択項目
        /// </summary>
        private HeaderEntity _selectedConcatenateHeader;

        /// <summary>
        /// 結合選択項目
        /// </summary>
        public HeaderEntity SelectedConcatenateHeader
        {
            get => _selectedConcatenateHeader;
            set
            {
                if (SetProperty(ref _selectedConcatenateHeader, value))
                {
                    DeleteCommand.RaiseCanExecuteChanged();
                }
            }
        }

        /// <summary>
        /// 項目選択コマンド
        /// </summary>
        public DelegateCommand SelectCommand { get; }

        /// <summary>
        /// 項目削除コマンド
        /// </summary>
        public DelegateCommand DeleteCommand { get; }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="outputHeaders"></param>
        public CreateOutputColumnViewModelTargetConcatenate(IReadOnlyList<HeaderEntity> outputHeaders)
            : base(TargetSettingType.Concatenate)
        {
            Headers = new ObservableCollection<HeaderEntity>();
            foreach (var header in outputHeaders)
            {
                Headers.Add(header);
            }

            SelectCommand = new DelegateCommand(ExecuteSelectCommand, CanSelectCommand);
            DeleteCommand = new DelegateCommand(ExecuteDeleteCommand, CanDeleteCommand);

            ////ConcatenateHeadersの要素が変わった時にもvalidateしたいので、変更があった時に
            ////検証が実行されるようにする
            ConcatenateHeaders.CollectionChanged += (sender, e) =>
            {
                ValidateProperty(ConcatenateHeaders, nameof(ConcatenateHeaders));
            };
        }

        /// <summary>
        /// ターゲット設定取得
        /// </summary>
        /// <returns>ターゲット設定</returns>
        public override BaseTargetSettingEntity GetTargetSettingEntity()
        {
            var headers = new List<HeaderEntity>();
            foreach (var concatenateHeader in ConcatenateHeaders)
            {
                headers.Add(concatenateHeader);
            }
            return new ConcatenateTargetSettingEntity(headers);
        }

        /// <summary>
        /// 項目選択コマンド実行
        /// </summary>
        private void ExecuteSelectCommand()
        {
            ConcatenateHeaders.Add(SelectedHeader);
            SelectedHeader = null;
        }

        /// <summary>
        /// 項目選択コマンド実行可否取得
        /// </summary>
        /// <returns></returns>
        private bool CanSelectCommand()
        {
            return SelectedHeader != null;
        }

        /// <summary>
        /// 項目削除コマンド実行
        /// </summary>
        private void ExecuteDeleteCommand()
        {
            ConcatenateHeaders.Remove(SelectedConcatenateHeader);

        }

        /// <summary>
        /// 項目削除コマンド実行可否取得
        /// </summary>
        private bool CanDeleteCommand()
        {
            return SelectedConcatenateHeader != null;
        }
    }
}
