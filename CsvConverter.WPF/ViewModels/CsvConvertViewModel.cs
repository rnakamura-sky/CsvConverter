using CsvConverter.Domain.Entities;
using CsvConverter.Domain.Logics;
using CsvConverter.Domain.Repositories;
using CsvConverter.Infrastructure.Drive;
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
        private readonly ICommonDialogService _commonDialogService;

        /// <summary>
        /// 出力ファイル作成ロジック
        /// </summary>
        private readonly ICsvConvertLogic _logic;

        /// <summary>
        /// CSVファイルリポジトリ
        /// </summary>
        private readonly ICsvFileRepository _csvFileRepository;

        /// <summary>
        /// 設定情報
        /// </summary>
        private readonly ISettingRepository _settingRepository;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public CsvConvertViewModel(IDialogService dialogService)
            : this(
                  dialogService,
                  new MessageService(),
                  new FileDialogService(),
                  new CsvConvertLogic(),
                  new CsvFileAccess(),
                  new SettingDrive())
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
            ICommonDialogService commonDialogService,
            ICsvConvertLogic logic,
            ICsvFileRepository csvFileRepository,
            ISettingRepository settingRepository)
        {
            _dialogService = dialogService;
            _messageService = messageService;
            _commonDialogService = commonDialogService;
            _logic = logic;
            _csvFileRepository = csvFileRepository;
            _settingRepository = settingRepository;

            ExecuteCommand = new DelegateCommand(ExecuteCommandExecute, CanExecuteCommand);
            InputCommand = new DelegateCommand(ExecuteInputCommand, CanInputCommand);
            ReplaceOutputColumnCommand = new DelegateCommand<int?>(ExecuteReplaceOutputColumnCommand);
            CreateCommand = new DelegateCommand(ExecuteCreateCommand);

            SelectInputFileCommand = new DelegateCommand(ExecuteSelectInputFileCommand);
            SelectOutputFileCommand = new DelegateCommand(ExecuteSelectOutputFileCommand);

            CreateOutputColumnFromInputCommand = new DelegateCommand(
                ExecuteCreateOutputColumnFromInputCommand, CanExecuteCreateOutputColumnFromInputCommand);
            ReadSettingCommand = new DelegateCommand(
                ExecuteReadSettingCommand, CanExecuteReadSettingCommand);
            SaveSettingCommand = new DelegateCommand(
                ExecuteSaveSettingCommand, CanExecuteSaveSettingCommand);
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

        /// <summary>
        /// 設定名
        /// </summary>
        private string _settingName = string.Empty;

        /// <summary>
        /// 設定名
        /// </summary>
        public string SettingName
        {
            get { return _settingName; }
            set { SetProperty(ref _settingName, value); }
        }

        /// <summary>
        /// 設定ファイルパス
        /// </summary>
        private string _settingFilePath = string.Empty;

        /// <summary>
        /// 設定ファイルパス
        /// </summary>
        public string SettingFilePath
        {
            get { return _settingFilePath; }
            set { SetProperty(ref _settingFilePath, value); }
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
        /// 入力ファイル選択コマンド
        /// </summary>
        public DelegateCommand SelectInputFileCommand { get; }

        /// <summary>
        /// 出力ファイル選択コマンド
        /// </summary>
        public DelegateCommand SelectOutputFileCommand { get; }

        /// <summary>
        /// 入力ファイル情報取得コマンド
        /// </summary>
        public DelegateCommand InputCommand { get; }

        /// <summary>
        /// 入力項目から出力項目生成コマンド
        /// </summary>
        public DelegateCommand CreateOutputColumnFromInputCommand { get; }

        /// <summary>
        /// 出力項目並び替えコマンド
        /// </summary>
        public DelegateCommand<int?> ReplaceOutputColumnCommand { get; }

        /// <summary>
        /// 新出力項目作成コマンド
        /// </summary>
        public DelegateCommand CreateCommand { get; }

        /// <summary>
        /// 出力ファイル作成実行コマンド
        /// </summary>
        public DelegateCommand ExecuteCommand { get; }

        /// <summary>
        /// 設定情報読み込みコマンド
        /// </summary>
        public DelegateCommand ReadSettingCommand { get; }

        /// <summary>
        /// 設定情報保存コマンド
        /// </summary>
        public DelegateCommand SaveSettingCommand { get; }

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

            CreateOutputColumnFromInputCommand.RaiseCanExecuteChanged();
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

        /// <summary>
        /// 入力ファイル取得コマンド実行
        /// </summary>
        private void ExecuteSelectInputFileCommand()
        {
            var settings = new FileDialogSettings()
            {
                Filter = new ExtensionFilter("CSVファイル", "*.csv | *"),
                Title = "CSVファイル選択",
            };
            if (_commonDialogService.ShowDialog(settings))
            {
                InputCsvFilePath = settings.FileName;
            }
        }

        /// <summary>
        /// 出力ファイル取得コマンド実行
        /// </summary>
        private void ExecuteSelectOutputFileCommand()
        {
            var settings = new FileDialogSettings()
            {
                Filter = new ExtensionFilter("CSVファイル", "*.csv | *"),
                Title = "CSVファイル選択",
            };
            if (_commonDialogService.ShowDialog(settings))
            {
                OutputCsvFilePath = settings.FileName;
            }
        }


        private void ExecuteCreateOutputColumnFromInputCommand()
        {
            var outputSetting = new OutputSettingEntity(InputHeaders);

            OutputColumns.Clear();
            foreach (var columnSetting in outputSetting.ColumnSettings)
            {
                OutputColumns.Add(new CsvConvertViewModelHeader(columnSetting));
            }
        }

        private bool CanExecuteCreateOutputColumnFromInputCommand()
        {
            return InputHeaders.Count > 0;
        }

        /// <summary>
        /// 設定情報読み込み実行
        /// </summary>
        private void ExecuteReadSettingCommand()
        {
            var fileDialogSettings = new FileDialogSettings()
            {
                Filter = new ExtensionFilter("設定ファイル", "*"),
                Title = "設定ファイル選択",
            };
            if (_commonDialogService.ShowDialog(fileDialogSettings))
            {
                var setting = _settingRepository.Read(fileDialogSettings.FileName);

                SettingFilePath = fileDialogSettings.FileName;

                SettingName = setting.SettingName;
                InputHeaders.Clear();
                foreach (var header in setting.Headers)
                {
                    InputHeaders.Add(header);
                }
                OutputColumns.Clear();
                foreach (var outputColumn in setting.OutputSetting.ColumnSettings)
                {
                    OutputColumns.Add(new CsvConvertViewModelHeader(outputColumn));
                }
            }
        }

        /// <summary>
        /// 設定情報読み込み実行可否
        /// </summary>
        /// <returns></returns>
        private bool CanExecuteReadSettingCommand()
        {
            return true;
        }

        /// <summary>
        /// 設定情報保存コマンド実行
        /// </summary>
        private void ExecuteSaveSettingCommand()
        {
            var setting = GetSettingEntity();
            var fileDialogSettings = new FileDialogSettings()
            {
                Filter = new ExtensionFilter("設定ファイル", "*"),
                Title = "設定ファイル保存先選択",
            };
            if (_commonDialogService.ShowDialog(fileDialogSettings))
            {
                _settingRepository.Save(fileDialogSettings.FileName, setting);
                _messageService.ShowDialog("設定を保存しました。");
                SettingFilePath = fileDialogSettings.FileName;
            }

        }

        /// <summary>
        /// 設定情報保存コマンド実行可否
        /// </summary>
        private bool CanExecuteSaveSettingCommand()
        {
            if (OutputColumns.Count == 0)
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// 設定情報を取得します。
        /// </summary>
        /// <returns></returns>
        private SettingEntity GetSettingEntity()
        {
            var inputHeaders = new List<HeaderEntity>();
            foreach (var header in InputHeaders)
            {
                inputHeaders.Add(header);
            }
            var outputSetting = GetOutputSetting();

            return new SettingEntity(SettingName, inputHeaders, outputSetting);
        }
    }
}
