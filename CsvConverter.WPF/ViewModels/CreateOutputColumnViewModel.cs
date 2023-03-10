using CsvConverter.Domain.Entities;
using CsvConverter.Domain.ValueObjects;
using CsvConverter.WPF.Services;
using Prism.Commands;
using Prism.Services.Dialogs;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;

namespace CsvConverter.WPF.ViewModels
{
    public class CreateOutputColumnViewModel : ViewModelBase, IDialogAware
    {
        private IMessageService _messageService;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public CreateOutputColumnViewModel()
            : this(new MessageService())
        {

        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public CreateOutputColumnViewModel(IMessageService messageService)
        {
            _messageService = messageService;

            TargetSettingTypes = new ObservableCollection<TargetSettingType>();
            foreach (var targetSettingType in TargetSettingType.GetTargetSettingTypes())
            {
                TargetSettingTypes.Add(targetSettingType);
            }

            CreateCommand = new DelegateCommand(ExecuteCreateCommand);
            CancelCommand = new DelegateCommand(ExecuteCancelCommand);
        }

        /// <summary>
        /// ダイアログ閉じるリクエストイベント
        /// </summary>
        public event Action<IDialogResult> RequestClose;

        /// <summary>
        /// ダイアログタイトル
        /// </summary>
        public string Title => "新規項目作成";

        /// <summary>
        /// 項目名
        /// </summary>
        private string _headerName = string.Empty;

        /// <summary>
        /// 項目名
        /// </summary>
        [Display(Name = "新規項目名")]
        [Required(ErrorMessage = "必須入力です。")]
        public string HeaderName
        {
            get { return _headerName; }
            set { SetValidateProperty(ref _headerName, value); }
        }

        /// <summary>
        /// 出力要否
        /// </summary>
        private bool _isOutput = true;
        
        /// <summary>
        /// 出力要否
        /// </summary>
        public bool IsOutput
        {
            get { return _isOutput; }
            set { SetProperty(ref _isOutput, value); }
        }

        /// <summary>
        /// 入力項目リスト
        /// </summary>
        private ObservableCollection<HeaderEntity> _inputHeaders = new();

        /// <summary>
        /// 入力項目リスト
        /// </summary>
        public ObservableCollection<HeaderEntity> InputHeaders
        {
            get { return _inputHeaders; }
            set { SetProperty(ref _inputHeaders, value); }
        }

        /// <summary>
        /// 出力項目リスト
        /// </summary>
        private ObservableCollection<HeaderEntity> _outputHeaders = new();

        /// <summary>
        /// 出力項目リスト
        /// </summary>
        public ObservableCollection<HeaderEntity> OutputHeaders
        {
            get { return _outputHeaders; }
            set { SetProperty(ref _outputHeaders, value); }
        }

        /// <summary>
        /// ターゲット設定タイプリスト
        /// </summary>
        private ObservableCollection<TargetSettingType> _targetSettingTypes;

        /// <summary>
        /// ターゲット設定タイプリスト
        /// </summary>
        public ObservableCollection<TargetSettingType> TargetSettingTypes
        {
            get { return _targetSettingTypes; }
            set { SetProperty(ref _targetSettingTypes, value); }
        }

        /// <summary>
        /// 選択ターゲット設定
        /// </summary>
        private TargetSettingType _selectedTargetSettingType;

        /// <summary>
        /// 選択ターゲット設定
        /// </summary>
        [Display(Name = "項目タイプ")]
        [Required(ErrorMessage = "選択してください。")]
        public TargetSettingType SelectedTargetSettingType
        {
            get { return _selectedTargetSettingType; }
            set
            {
                if (SetValidateProperty(ref _selectedTargetSettingType, value))
                {
                    if (value is null)
                    {
                        Target = null;
                    }
                    if (value == TargetSettingType.Input)
                    {
                        Target = new CreateOutputColumnViewModelTargetInput(InputHeaders);
                    }
                    if (value == TargetSettingType.Concatenate)
                    {
                        Target = new CreateOutputColumnViewModelTargetConcatenate(OutputHeaders);
                    }
                }
            }
        }

        /// <summary>
        /// ターゲット設定
        /// </summary>
        private CreateOutputColumnViewModelTargetBase _target;

        /// <summary>
        /// ターゲット設定
        /// </summary>
        public CreateOutputColumnViewModelTargetBase Target
        {
            get => _target;
            set => SetProperty(ref _target, value);
        }

        /// <summary>
        /// 新規項目作成コマンド
        /// </summary>
        public DelegateCommand CreateCommand { get; }

        /// <summary>
        /// 新規項目作成キャンセルコマンド
        /// </summary>
        public DelegateCommand CancelCommand { get; }

        /// <summary>
        /// ダイアログクローズ可否
        /// </summary>
        /// <returns></returns>
        public bool CanCloseDialog() => true;

        /// <summary>
        /// ダイアログクローズ処理
        /// </summary>
        public void OnDialogClosed()
        {
        }

        /// <summary>
        /// ダイアログオープン処理
        /// </summary>
        /// <param name="parameters"></param>
        public void OnDialogOpened(IDialogParameters parameters)
        {
            var inputHeaders = parameters.GetValue<IReadOnlyList<HeaderEntity>>(nameof(InputHeaders));
            foreach (var header in inputHeaders)
            {
                InputHeaders.Add(header);
            }
            var outputHeaders = parameters.GetValue<IReadOnlyList<HeaderEntity>>(nameof(OutputHeaders));
            foreach (var header in outputHeaders)
            {
                OutputHeaders.Add(header);
            }
        }

        /// <summary>
        /// 新規項目作成処理
        /// </summary>
        private void ExecuteCreateCommand()
        {
            if (!ValidateAllObjects())
            {
                ////保持しているサブのViewModelに対してもValidationを行う
                Target?.ValidateAllObjects();
                var message = GetFirstErrorMessage();
                _messageService.ShowDialog(message);
                return;
            }

            if (!Target.ValidateAllObjects())
            {
                var message = Target.GetFirstErrorMessage();
                _messageService.ShowDialog(message);
                return;
            }
            var targetSetting = Target.GetTargetSettingEntity();
            var column = new OutputColumnSettingEntity(
                0, HeaderName, IsOutput, targetSetting);
            var parameters = new DialogParameters()
            {
                {nameof(OutputColumnSettingEntity), column},
            };
            var result = new DialogResult(ButtonResult.OK, parameters);

            RequestClose?.Invoke(result);
        }

        /// <summary>
        /// 新規項目作成キャンセル処理
        /// </summary>
        private void ExecuteCancelCommand()
        {
            RequestClose?.Invoke(new DialogResult(ButtonResult.Cancel));
        }
    }
}
