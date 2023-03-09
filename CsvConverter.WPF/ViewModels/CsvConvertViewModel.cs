using CsvConverter.Domain.Entities;
using CsvConverter.Domain.Logics;
using CsvConverter.Domain.Repositories;
using CsvConverter.WPF.Services;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Services.Dialogs;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace CsvConverter.WPF.ViewModels
{
    /// <summary>
    /// CSVファイル変換ViewModel
    /// </summary>
    public class CsvConvertViewModel : BindableBase
    {
        private readonly IDialogService _dialogService;
        private readonly IMessageService _messageService;

        /// <summary>
        /// 出力ファイル作成ロジック
        /// </summary>
        private readonly ICsvConvertLogic _logic;

        /// <summary>
        /// CSVファイルリポジトリ
        /// </summary>
        private readonly ICsvFileRepository _csvFileRepository;


        /// <summary>
        /// コンストラクタ
        /// </summary>
        public CsvConvertViewModel(IDialogService dialogService)
            : this(dialogService, new MessageService(), new CsvConvertLogic(), new CsvFileAccess())
        {

        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="logic">出力ファイル作成ロジック</param>
        /// <param name="csvFileRepository">CSVファイルリポジトリ</param>
        public CsvConvertViewModel(
            IDialogService dialogService,
            IMessageService messageService,
            ICsvConvertLogic logic,
            ICsvFileRepository csvFileRepository)
        {
            _dialogService = dialogService;
            _messageService = messageService;
            _logic = logic;
            _csvFileRepository = csvFileRepository;

            ExecuteCommand = new DelegateCommand(ExecuteCommandExecute, CanExecuteCommand);
            InputCommand = new DelegateCommand(ExecuteInputCommand, CanInputCommand);
            ReplaceOutputColumnCommand = new DelegateCommand<int?>(ExecuteReplaceOutputColumnCommand);
            CreateCommand = new DelegateCommand(ExecuteCreateCommand);
        }

        private string _inputCsvFilePath = string.Empty;
        
        /// <summary>
        /// 入力CSVファイルパス
        /// </summary>
        public string InputCsvFilePath
        {
            get { return _inputCsvFilePath; }
            set
            {
                if (SetProperty(ref _inputCsvFilePath, value))
                {
                    ExecuteCommand.RaiseCanExecuteChanged();
                    InputCommand.RaiseCanExecuteChanged();
                }
            }
        }

        private string _outputCsvFilePath = string.Empty;

        /// <summary>
        /// 出力CSVファイルパス
        /// </summary>
        public string OutputCsvFilePath
        {
            get { return _outputCsvFilePath; }
            set
            {
                if (SetProperty(ref _outputCsvFilePath, value))
                {
                    ExecuteCommand.RaiseCanExecuteChanged();
                }
            }
        }

        private ObservableCollection<HeaderEntity> _inputHeaders = new();
        
        /// <summary>
        /// 入力項目リスト
        /// </summary>
        public ObservableCollection<HeaderEntity> InputHeaders
        {
            get => _inputHeaders;
            set => SetProperty(ref _inputHeaders, value);
        }

        private ObservableCollection<CsvConvertViewModelHeader> _outputColumns = new();

        /// <summary>
        /// 出力行情報
        /// </summary>
        public ObservableCollection<CsvConvertViewModelHeader> OutputColumns {
            get => _outputColumns;
            private set => SetProperty(ref _outputColumns, value);
        }

        /// <summary>
        /// 選択列インデックス
        /// </summary>
        private int _selectedOutputColumnIndex;
        public int SelectedOutputColumnIndex
        {
            get { return _selectedOutputColumnIndex; }
            set { SetProperty(ref _selectedOutputColumnIndex, value); }
        }

        /// <summary>
        /// 出力ファイル作成実行コマンド
        /// </summary>
        public DelegateCommand ExecuteCommand { get; }

        /// <summary>
        /// 入力ファイル情報取得コマンド
        /// </summary>
        public DelegateCommand InputCommand { get; }

        /// <summary>
        /// 出力項目並び替えコマンド
        /// </summary>
        public DelegateCommand<int?> ReplaceOutputColumnCommand { get; }

        /// <summary>
        /// 新出力項目作成コマンド
        /// </summary>
        public DelegateCommand CreateCommand { get; }

        /// <summary>
        /// 出力ファイル作成コマンド実行
        /// </summary>
        private void ExecuteCommandExecute()
        {
            var inputCsvFile = new InputCsvFileEntity(_csvFileRepository, InputCsvFilePath);
            var outputCsvFile = new OutputCsvFileEntity(_csvFileRepository, OutputCsvFilePath);

            var outputSetting = GetOutputSetting();
            _logic.Execute(inputCsvFile, outputCsvFile, outputSetting);

            _messageService.ShowDialog("出力が完了しました");
        }

        /// <summary>
        /// 出力ファイル作成コマンド実行可否
        /// </summary>
        /// <returns>ボタン可否</returns>
        private bool CanExecuteCommand()
        {
            if (InputCsvFilePath == string.Empty)
            {
                return false;
            }
            if (OutputCsvFilePath == string.Empty)
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// 入力ファイル情報取得コマンド実行
        /// </summary>
        private void ExecuteInputCommand()
        {
            var inputCsvFile = new InputCsvFileEntity(_csvFileRepository, InputCsvFilePath);
            var fileData = inputCsvFile.GetData();

            InputHeaders.Clear();
            foreach(var header in fileData.Headers)
            {
                InputHeaders.Add(header);
            }

            var outputSetting = new OutputSettingEntity(fileData);

            OutputColumns.Clear();
            foreach (var columnSetting in outputSetting.ColumnSettings)
            {
                OutputColumns.Add(new CsvConvertViewModelHeader(columnSetting));
            }
        }

        /// <summary>
        /// 入力ファイル情報取得コマンド可否
        /// </summary>
        /// <returns>ボタン可否</returns>
        private bool CanInputCommand()
        {
            if (InputCsvFilePath == string.Empty)
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// 出力項目並び替えコマンド実行
        /// </summary>
        /// <param name="index">並び替え位置</param>
        private void ExecuteReplaceOutputColumnCommand(int? index)
        {
            if (index is null)
            {
                return;
            }
            OutputColumns.Move(SelectedOutputColumnIndex, index.Value);
        }

        /// <summary>
        /// 出力設定情報取得
        /// </summary>
        /// <returns>出力設定情報Entity</returns>
        private OutputSettingEntity GetOutputSetting()
        {
            if (OutputColumns.Count == 0)
            {
                return OutputSettingEntity.None;
            }
            var settingColumns = new List<OutputColumnSettingEntity>();
            var index = 0;
            foreach (var row in OutputColumns)
            {
                settingColumns.Add(row.GetColumnSettingEntity(index));
                index++;
            }

            return new OutputSettingEntity(settingColumns);
        }

        /// <summary>
        /// 新出力項目作成コマンド実行
        /// </summary>
        private void ExecuteCreateCommand()
        {
            var inputHeaders = new List<HeaderEntity>();
            foreach(var header in InputHeaders)
            {
                inputHeaders.Add(header);
            }
            var outputHeaders = new List<HeaderEntity>();
            foreach(var column in OutputColumns)
            {
                outputHeaders.Add(column.GetHeader());
            }

            var parameters = new DialogParameters();
            parameters.Add(nameof(CreateOutputColumnViewModel.InputHeaders), inputHeaders);
            parameters.Add(nameof(CreateOutputColumnViewModel.OutputHeaders), outputHeaders);
            _dialogService.ShowDialog(
                nameof(Views.CreateOutputColumnView),
                parameters,
                result => {
                    if (result.Result == ButtonResult.OK)
                    {
                        var newOutputColumnSettingEntity
                            = result.Parameters.GetValue<OutputColumnSettingEntity>(nameof(OutputColumnSettingEntity));
                        OutputColumns.Add(new CsvConvertViewModelHeader(newOutputColumnSettingEntity));
                    }
                });
        }
    }
}
