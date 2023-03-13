using CsvConverter.Domain.Entities;
using CsvConverter.Domain.Logics;
using CsvConverter.Domain.Repositories;
using CsvConverter.WPF.Services;
using CsvConverter.WPF.ViewModels;
using Moq;
using Prism.Services.Dialogs;

namespace CsvConverterTest.Tests
{
    [TestClass]
    public class CsvConvertViewModelTest
    {

        [TestMethod]
        public void 基本画面項目確認シナリオ()
        {
            var logicMock = new Mock<ICsvConvertLogic>();
            logicMock.Setup(x => x.Execute(It.IsAny<InputCsvFileEntity>(), It.IsAny<OutputCsvFileEntity>(), It.IsAny<OutputSettingEntity>()))
                .Callback((InputCsvFileEntity inputFile, OutputCsvFileEntity outputFile, OutputSettingEntity setting) =>
                {
                    Assert.AreEqual("InputCsvFilePath", inputFile.CsvFilePath);
                    Assert.AreEqual("OutputCsvFilePath", outputFile.CsvFilePath);

                    ////実際に実装部分についても実行を行う
                    var logic = new CsvConvertLogic();
                    logic.Execute(inputFile, outputFile, setting);
                });
            var fileString =
@"Field1,Field2,Field3
A,B,C
a,b,c
";
            var csvFileMock = new Mock<ICsvFileRepository>();
            csvFileMock.Setup(x => x.GetData(It.IsAny<string>())).Returns(fileString);
            csvFileMock.Setup(x => x.WriteData(It.IsAny<string>(), It.IsAny<string>()))
                .Callback((string filePath, string outFileString) =>
                {
                    Assert.AreEqual("OutputCsvFilePath", filePath);
                    var expectFileString =
@"Field1,Field2,Field3,OutputField4,OutputField5
A,B,C,B,ABB
a,b,c,b,abb
";
                    Assert.AreEqual(expectFileString, outFileString);

                });
            var settingRepositoryMock = new Mock<ISettingRepository>();
            var dialogServiceMock = new Mock<IDialogService>();
            var messageServiceMock = new Mock<IMessageService>();
            var commonDialogServiceMock = new Mock<ICommonDialogService>();
            var viewModel = new CsvConvertViewModel(
                dialogServiceMock.Object,
                messageServiceMock.Object,
                commonDialogServiceMock.Object,
                logicMock.Object,
                csvFileMock.Object,
                settingRepositoryMock.Object);

            ////初期表示
            Assert.AreEqual(string.Empty, viewModel.InputCsvFilePath);
            Assert.AreEqual(string.Empty, viewModel.OutputCsvFilePath);
            Assert.AreEqual(0, viewModel.InputHeaders.Count);
            Assert.AreEqual(0, viewModel.OutputColumns.Count);
            Assert.AreEqual(false, viewModel.InputCommand.CanExecute());
            Assert.AreEqual(false, viewModel.CreateOutputColumnFromInputCommand.CanExecute());
            Assert.AreEqual(false, viewModel.ExecuteCommand.CanExecute());

            ////入力パス設定
            viewModel.InputCsvFilePath = "InputCsvFilePath";
            Assert.AreEqual(true, viewModel.InputCommand.CanExecute());
            Assert.AreEqual(false, viewModel.CreateOutputColumnFromInputCommand.CanExecute());
            Assert.AreEqual(false, viewModel.ExecuteCommand.CanExecute());

            ////入力ファイル読み込み
            viewModel.InputCommand.Execute();
            Assert.AreEqual(3, viewModel.InputHeaders.Count);
            Assert.AreEqual("Field1", viewModel.InputHeaders[0].HeaderName);
            Assert.AreEqual(0, viewModel.InputHeaders[0].HeaderId);
            Assert.AreEqual("Field2", viewModel.InputHeaders[1].HeaderName);
            Assert.AreEqual(1, viewModel.InputHeaders[1].HeaderId);
            Assert.AreEqual("Field3", viewModel.InputHeaders[2].HeaderName);
            Assert.AreEqual(2, viewModel.InputHeaders[2].HeaderId);
            Assert.AreEqual(0, viewModel.OutputColumns.Count);
            Assert.AreEqual(true, viewModel.CreateOutputColumnFromInputCommand.CanExecute());
            Assert.AreEqual(false, viewModel.ExecuteCommand.CanExecute());

            ////出力項目作成(入力項目から)
            viewModel.CreateOutputColumnFromInputCommand.Execute();
            Assert.AreEqual(3, viewModel.InputHeaders.Count);
            Assert.AreEqual("Field1", viewModel.InputHeaders[0].HeaderName);
            Assert.AreEqual(0, viewModel.InputHeaders[0].HeaderId);
            Assert.AreEqual("Field2", viewModel.InputHeaders[1].HeaderName);
            Assert.AreEqual(1, viewModel.InputHeaders[1].HeaderId);
            Assert.AreEqual("Field3", viewModel.InputHeaders[2].HeaderName);
            Assert.AreEqual(2, viewModel.InputHeaders[2].HeaderId);

            Assert.AreEqual(3, viewModel.OutputColumns.Count);
            Assert.AreEqual("Field1", viewModel.OutputColumns[0].FieldName);
            Assert.AreEqual(true, viewModel.OutputColumns[0].IsOutput);
            Assert.AreEqual("Field2", viewModel.OutputColumns[1].FieldName);
            Assert.AreEqual(true, viewModel.OutputColumns[1].IsOutput);
            Assert.AreEqual("Field3", viewModel.OutputColumns[2].FieldName);
            Assert.AreEqual(true, viewModel.OutputColumns[2].IsOutput);

            ////新しい項目を作成する
            dialogServiceMock.Setup(x => x.ShowDialog(It.IsAny<string>(), It.IsAny<IDialogParameters>(), It.IsAny<Action<IDialogResult>>()))
                .Callback((string name, IDialogParameters parameters, Action<IDialogResult> callback) =>
                {
                    var inputHeaders = parameters.GetValue<IReadOnlyList<HeaderEntity>>(nameof(CreateOutputColumnViewModel.InputHeaders));
                    Assert.AreEqual(3, inputHeaders.Count);
                    Assert.AreEqual("Field1", inputHeaders[0].HeaderName);
                    Assert.AreEqual("Field2", inputHeaders[1].HeaderName);
                    Assert.AreEqual("Field3", inputHeaders[2].HeaderName);

                    var columnSetting = new OutputColumnSettingEntity(0, "OutputField4", true, new InputTargetSettingEntity("Field2"));
                    var resultParameters = new DialogParameters();
                    resultParameters.Add(nameof(OutputColumnSettingEntity), columnSetting);
                    var dialogResult = new DialogResult(ButtonResult.OK, resultParameters);
                    callback?.Invoke(dialogResult);
                });
            viewModel.CreateCommand.Execute();

            ////新しい項目をもう一つ作成する
            dialogServiceMock.Setup(x => x.ShowDialog(It.IsAny<string>(), It.IsAny<IDialogParameters>(), It.IsAny<Action<IDialogResult>>()))
                .Callback((string name, IDialogParameters parameters, Action<IDialogResult> callback) =>
                {
                    var inputHeaders = parameters.GetValue<IReadOnlyList<HeaderEntity>>(nameof(CreateOutputColumnViewModel.InputHeaders));
                    Assert.AreEqual(3, inputHeaders.Count);
                    Assert.AreEqual("Field1", inputHeaders[0].HeaderName);
                    Assert.AreEqual("Field2", inputHeaders[1].HeaderName);
                    Assert.AreEqual("Field3", inputHeaders[2].HeaderName);

                    var outputHeaders = parameters.GetValue<IReadOnlyList<HeaderEntity>>(nameof(CreateOutputColumnViewModel.OutputHeaders));
                    Assert.AreEqual(4, outputHeaders.Count);
                    Assert.AreEqual("Field1", outputHeaders[0].HeaderName);
                    Assert.AreEqual("Field2", outputHeaders[1].HeaderName);
                    Assert.AreEqual("Field3", outputHeaders[2].HeaderName);
                    Assert.AreEqual("OutputField4", outputHeaders[3].HeaderName);

                    var columnSetting = new OutputColumnSettingEntity(0, "OutputField5", true, new ConcatenateTargetSettingEntity(
                        new List<HeaderEntity>() {
                                        new HeaderEntity(0, "Field1"),
                                        new HeaderEntity(1, "Field2"),
                                        new HeaderEntity(2, "OutputField4"),
                    }));
                    var resultParameters = new DialogParameters();
                    resultParameters.Add(nameof(OutputColumnSettingEntity), columnSetting);
                    var dialogResult = new DialogResult(ButtonResult.OK, resultParameters);
                    callback?.Invoke(dialogResult);
                });
            viewModel.CreateCommand.Execute();

            Assert.AreEqual(5, viewModel.OutputColumns.Count);
            Assert.AreEqual("Field1", viewModel.OutputColumns[0].FieldName);
            Assert.AreEqual(true, viewModel.OutputColumns[0].IsOutput);
            Assert.AreEqual("Field1", viewModel.OutputColumns[0].FieldContent);
            Assert.AreEqual("Field1", viewModel.OutputColumns[0].InputFieldName);
            Assert.AreEqual("Field2", viewModel.OutputColumns[1].FieldName);
            Assert.AreEqual(true, viewModel.OutputColumns[1].IsOutput);
            Assert.AreEqual("Field2", viewModel.OutputColumns[1].FieldContent);
            Assert.AreEqual("Field2", viewModel.OutputColumns[1].InputFieldName);
            Assert.AreEqual("Field3", viewModel.OutputColumns[2].FieldName);
            Assert.AreEqual(true, viewModel.OutputColumns[2].IsOutput);
            Assert.AreEqual("Field3", viewModel.OutputColumns[2].FieldContent);
            Assert.AreEqual("Field3", viewModel.OutputColumns[2].InputFieldName);
            Assert.AreEqual("OutputField4", viewModel.OutputColumns[3].FieldName);
            Assert.AreEqual(true, viewModel.OutputColumns[3].IsOutput);
            Assert.AreEqual("Field2", viewModel.OutputColumns[3].FieldContent);
            Assert.AreEqual("Field2", viewModel.OutputColumns[3].InputFieldName);
            Assert.AreEqual("OutputField5", viewModel.OutputColumns[4].FieldName);
            Assert.AreEqual(true, viewModel.OutputColumns[4].IsOutput);
            Assert.AreEqual("Field1+Field2+OutputField4", viewModel.OutputColumns[4].FieldContent);
            Assert.AreEqual("", viewModel.OutputColumns[4].InputFieldName);

            viewModel.OutputCsvFilePath = "OutputCsvFilePath";

            Assert.AreEqual(true, viewModel.ExecuteCommand.CanExecute());
            viewModel.ExecuteCommand.Execute();

            logicMock.VerifyAll();
            csvFileMock.VerifyAll();
            dialogServiceMock.VerifyAll();
        }

        [TestMethod]
        public void 入力ファイルと出力ファイルを指定してそのまま出力するシナリオ()
        {
            var dialogServiceMock = new Mock<IDialogService>();
            var commonDialogServiceMock = new Mock<ICommonDialogService>();
            var messageServiceMock = new Mock<IMessageService>();
            var logicMock = new Mock<ICsvConvertLogic>();
            logicMock.Setup(x => x.Execute(It.IsAny<InputCsvFileEntity>(), It.IsAny<OutputCsvFileEntity>(), It.IsAny<OutputSettingEntity>()))
                .Callback((InputCsvFileEntity inputFile, OutputCsvFileEntity outputFile, OutputSettingEntity setting) =>
                {
                    Assert.AreEqual("InputCsvFilePath", inputFile.CsvFilePath);
                    Assert.AreEqual("OutputCsvFilePath", outputFile.CsvFilePath);
                });
            var csvFileMock = new Mock<ICsvFileRepository>();
            var settingRepositoryMock = new Mock<ISettingRepository>();
            var viewModel = new CsvConvertViewModel(
                dialogServiceMock.Object,
                messageServiceMock.Object,
                commonDialogServiceMock.Object,
                logicMock.Object,
                csvFileMock.Object,
                settingRepositoryMock.Object);

            Assert.AreEqual(string.Empty, viewModel.InputCsvFilePath);
            Assert.AreEqual(string.Empty, viewModel.OutputCsvFilePath);
            Assert.AreEqual(false, viewModel.ExecuteCommand.CanExecute());

            viewModel.InputCsvFilePath = "InputCsvFilePath";
            Assert.AreEqual(false, viewModel.ExecuteCommand.CanExecute());

            viewModel.OutputCsvFilePath = "OutputCsvFilePath";

            Assert.AreEqual(true, viewModel.ExecuteCommand.CanExecute());
            viewModel.ExecuteCommand.Execute();

            logicMock.VerifyAll();
        }

        [TestMethod]
        public void ファイルを参照ボタンで指定するシナリオ()
        {
            var dialogServiceMock = new Mock<IDialogService>();
            var commonDialogServiceMock = new Mock<ICommonDialogService>();
            var messageServiceMock = new Mock<IMessageService>();
            var logicMock = new Mock<ICsvConvertLogic>();
            logicMock.Setup(x => x.Execute(It.IsAny<InputCsvFileEntity>(), It.IsAny<OutputCsvFileEntity>(), It.IsAny<OutputSettingEntity>()))
                .Callback((InputCsvFileEntity inputFile, OutputCsvFileEntity outputFile, OutputSettingEntity setting) =>
                {
                    Assert.AreEqual("InputCsvFilePath", inputFile.CsvFilePath);
                    Assert.AreEqual("OutputCsvFilePath", outputFile.CsvFilePath);
                });
            var csvFileMock = new Mock<ICsvFileRepository>();
            var settingRepositoryMock = new Mock<ISettingRepository>();
            var viewModel = new CsvConvertViewModel(
                dialogServiceMock.Object,
                messageServiceMock.Object,
                commonDialogServiceMock.Object,
                logicMock.Object,
                csvFileMock.Object,
                settingRepositoryMock.Object);

            Assert.AreEqual(string.Empty, viewModel.InputCsvFilePath);
            Assert.AreEqual(string.Empty, viewModel.OutputCsvFilePath);
            Assert.AreEqual(false, viewModel.ExecuteCommand.CanExecute());

            ////入力ファイル指定
            commonDialogServiceMock.Setup(x => x.ShowDialog(It.IsAny<ICommonDialogSettings>())).Returns(true)
                .Callback((ICommonDialogSettings settings) =>
                {
                    var dialogSettings = settings as FileDialogSettings;
                    Assert.IsNotNull(dialogSettings);

                    dialogSettings.FileName = "InputCsvFilePath";
                });

            viewModel.SelectInputFileCommand.Execute();
            Assert.AreEqual("InputCsvFilePath", viewModel.InputCsvFilePath);
            Assert.AreEqual(false, viewModel.ExecuteCommand.CanExecute());

            ////出力ファイル指定
            commonDialogServiceMock.Setup(x => x.ShowDialog(It.IsAny<ICommonDialogSettings>())).Returns(true)
                .Callback((ICommonDialogSettings settings) =>
                {
                    var dialogSettings = settings as FileDialogSettings;
                    Assert.IsNotNull(dialogSettings);

                    dialogSettings.FileName = "OutputCsvFilePath";
                });
            viewModel.SelectOutputFileCommand.Execute();
            Assert.AreEqual("OutputCsvFilePath", viewModel.OutputCsvFilePath);

            Assert.AreEqual(true, viewModel.ExecuteCommand.CanExecute());
            viewModel.ExecuteCommand.Execute();

            commonDialogServiceMock.VerifyAll();
            logicMock.VerifyAll();
        }

        [TestMethod]
        public void 並び替えシナリオ()
        {
            var logicMock = new Mock<ICsvConvertLogic>();
            logicMock.Setup(x => x.Execute(It.IsAny<InputCsvFileEntity>(), It.IsAny<OutputCsvFileEntity>(), It.IsAny<OutputSettingEntity>()))
                .Callback((InputCsvFileEntity inputFile, OutputCsvFileEntity outputFile, OutputSettingEntity setting) =>
                {
                    Assert.AreEqual("InputCsvFilePath", inputFile.CsvFilePath);
                    Assert.AreEqual("OutputCsvFilePath", outputFile.CsvFilePath);

                    ////実際に実装部分についても実行を行う
                    var logic = new CsvConvertLogic();
                    logic.Execute(inputFile, outputFile, setting);
                });
            var fileString =
@"Field1,Field2,Field3
A,B,C
a,b,c
";
            var csvFileMock = new Mock<ICsvFileRepository>();
            csvFileMock.Setup(x => x.GetData(It.IsAny<string>())).Returns(fileString);
            csvFileMock.Setup(x => x.WriteData(It.IsAny<string>(), It.IsAny<string>()))
                .Callback((string filePath, string outFileString) =>
                {
                    Assert.AreEqual("OutputCsvFilePath", filePath);
                    var expectFileString =
@"Field2,Field3,Field1
B,C,A
b,c,a
";
                    Assert.AreEqual(expectFileString, outFileString);

                });

            var dialogServiceMock = new Mock<IDialogService>();
            var messageServiceMock = new Mock<IMessageService>();
            var commonDialogServiceMock = new Mock<ICommonDialogService>();
            var settingRepositoryMock = new Mock<ISettingRepository>();
            var viewModel = new CsvConvertViewModel(
                dialogServiceMock.Object,
                messageServiceMock.Object,
                commonDialogServiceMock.Object,
                logicMock.Object,
                csvFileMock.Object,
                settingRepositoryMock.Object);

            Assert.AreEqual(string.Empty, viewModel.InputCsvFilePath);
            Assert.AreEqual(string.Empty, viewModel.OutputCsvFilePath);
            Assert.AreEqual(false, viewModel.ExecuteCommand.CanExecute());
            Assert.AreEqual(0, viewModel.OutputColumns.Count);
            Assert.AreEqual(false, viewModel.InputCommand.CanExecute());

            viewModel.InputCsvFilePath = "InputCsvFilePath";
            Assert.AreEqual(false, viewModel.ExecuteCommand.CanExecute());
            Assert.AreEqual(true, viewModel.InputCommand.CanExecute());

            viewModel.InputCommand.Execute();
            viewModel.CreateOutputColumnFromInputCommand.Execute();
            Assert.AreEqual(3, viewModel.OutputColumns.Count);
            Assert.AreEqual("Field1", viewModel.OutputColumns[0].FieldName);
            Assert.AreEqual("Field2", viewModel.OutputColumns[1].FieldName);
            Assert.AreEqual("Field3", viewModel.OutputColumns[2].FieldName);

            viewModel.SelectedOutputColumnIndex = 0;
            viewModel.ReplaceOutputColumnCommand.Execute(2);
            Assert.AreEqual("Field2", viewModel.OutputColumns[0].FieldName);
            Assert.AreEqual("Field3", viewModel.OutputColumns[1].FieldName);
            Assert.AreEqual("Field1", viewModel.OutputColumns[2].FieldName);

            viewModel.OutputCsvFilePath = "OutputCsvFilePath";

            Assert.AreEqual(true, viewModel.ExecuteCommand.CanExecute());
            viewModel.ExecuteCommand.Execute();

            logicMock.VerifyAll();
            csvFileMock.VerifyAll();
        }

        [TestMethod]
        public void 出力制御シナリオ()
        {
            var logicMock = new Mock<ICsvConvertLogic>();
            logicMock.Setup(x => x.Execute(It.IsAny<InputCsvFileEntity>(), It.IsAny<OutputCsvFileEntity>(), It.IsAny<OutputSettingEntity>()))
                .Callback((InputCsvFileEntity inputFile, OutputCsvFileEntity outputFile, OutputSettingEntity setting) =>
                {
                    Assert.AreEqual("InputCsvFilePath", inputFile.CsvFilePath);
                    Assert.AreEqual("OutputCsvFilePath", outputFile.CsvFilePath);

                    ////実際に実装部分についても実行を行う
                    var logic = new CsvConvertLogic();
                    logic.Execute(inputFile, outputFile, setting);
                });
            var fileString =
@"Field1,Field2,Field3
A,B,C
a,b,c
";
            var csvFileMock = new Mock<ICsvFileRepository>();
            csvFileMock.Setup(x => x.GetData(It.IsAny<string>())).Returns(fileString);
            csvFileMock.Setup(x => x.WriteData(It.IsAny<string>(), It.IsAny<string>()))
                .Callback((string filePath, string outFileString) =>
                {
                    Assert.AreEqual("OutputCsvFilePath", filePath);
                    var expectFileString =
@"Field1,Field3
A,C
a,c
";
                    Assert.AreEqual(expectFileString, outFileString);

                });
            var settingRepositoryMock = new Mock<ISettingRepository>();
            var dialogServiceMock = new Mock<IDialogService>();
            var messageServiceMock = new Mock<IMessageService>();
            var commonDialogServiceMock = new Mock<ICommonDialogService>();
            var viewModel = new CsvConvertViewModel(
                dialogServiceMock.Object,
                messageServiceMock.Object,
                commonDialogServiceMock.Object,
                logicMock.Object,
                csvFileMock.Object,
                settingRepositoryMock.Object);

            Assert.AreEqual(string.Empty, viewModel.InputCsvFilePath);
            Assert.AreEqual(string.Empty, viewModel.OutputCsvFilePath);
            Assert.AreEqual(false, viewModel.ExecuteCommand.CanExecute());
            Assert.AreEqual(0, viewModel.OutputColumns.Count);
            Assert.AreEqual(false, viewModel.InputCommand.CanExecute());

            viewModel.InputCsvFilePath = "InputCsvFilePath";
            Assert.AreEqual(false, viewModel.ExecuteCommand.CanExecute());
            Assert.AreEqual(true, viewModel.InputCommand.CanExecute());

            viewModel.InputCommand.Execute();
            viewModel.CreateOutputColumnFromInputCommand.Execute();
            Assert.AreEqual(3, viewModel.OutputColumns.Count);
            Assert.AreEqual("Field1", viewModel.OutputColumns[0].FieldName);
            Assert.AreEqual(true, viewModel.OutputColumns[0].IsOutput);
            Assert.AreEqual("Field2", viewModel.OutputColumns[1].FieldName);
            Assert.AreEqual(true, viewModel.OutputColumns[1].IsOutput);
            Assert.AreEqual("Field3", viewModel.OutputColumns[2].FieldName);
            Assert.AreEqual(true, viewModel.OutputColumns[2].IsOutput);

            viewModel.OutputColumns[1].IsOutput = false;
            Assert.AreEqual("Field1", viewModel.OutputColumns[0].FieldName);
            Assert.AreEqual(true, viewModel.OutputColumns[0].IsOutput);
            Assert.AreEqual("Field2", viewModel.OutputColumns[1].FieldName);
            Assert.AreEqual(false, viewModel.OutputColumns[1].IsOutput);
            Assert.AreEqual("Field3", viewModel.OutputColumns[2].FieldName);
            Assert.AreEqual(true, viewModel.OutputColumns[2].IsOutput);

            viewModel.OutputCsvFilePath = "OutputCsvFilePath";

            Assert.AreEqual(true, viewModel.ExecuteCommand.CanExecute());
            viewModel.ExecuteCommand.Execute();

            logicMock.VerifyAll();
            csvFileMock.VerifyAll();
        }

        [TestMethod]
        public void 項目追加シナリオ()
        {
            var logicMock = new Mock<ICsvConvertLogic>();
            logicMock.Setup(x => x.Execute(It.IsAny<InputCsvFileEntity>(), It.IsAny<OutputCsvFileEntity>(), It.IsAny<OutputSettingEntity>()))
                .Callback((InputCsvFileEntity inputFile, OutputCsvFileEntity outputFile, OutputSettingEntity setting) =>
                {
                    Assert.AreEqual("InputCsvFilePath", inputFile.CsvFilePath);
                    Assert.AreEqual("OutputCsvFilePath", outputFile.CsvFilePath);

                    ////実際に実装部分についても実行を行う
                    var logic = new CsvConvertLogic();
                    logic.Execute(inputFile, outputFile, setting);
                });
            var fileString =
@"Field1,Field2,Field3
A,B,C
a,b,c
";
            var csvFileMock = new Mock<ICsvFileRepository>();
            csvFileMock.Setup(x => x.GetData(It.IsAny<string>())).Returns(fileString);
            csvFileMock.Setup(x => x.WriteData(It.IsAny<string>(), It.IsAny<string>()))
                .Callback((string filePath, string outFileString) =>
                {
                    Assert.AreEqual("OutputCsvFilePath", filePath);
                    var expectFileString =
@"Field1,Field2,Field3,Field4
A,B,C,B
a,b,c,b
";
                    Assert.AreEqual(expectFileString, outFileString);

                });

            var dialogServiceMock = new Mock<IDialogService>();
            dialogServiceMock.Setup(x => x.ShowDialog(It.IsAny<string>(), It.IsAny<IDialogParameters>(), It.IsAny<Action<IDialogResult>>()))
                .Callback((string name, IDialogParameters parameters, Action<IDialogResult> callback) =>
                {
                    var headers = parameters.GetValue<IReadOnlyList<HeaderEntity>>(nameof(CreateOutputColumnViewModel.InputHeaders));
                    Assert.AreEqual(3, headers.Count);
                    Assert.AreEqual("Field1", headers[0].HeaderName);
                    Assert.AreEqual("Field2", headers[1].HeaderName);
                    Assert.AreEqual("Field3", headers[2].HeaderName);

                    var columnSetting = new OutputColumnSettingEntity(0, "Field4", true, new InputTargetSettingEntity("Field2"));
                    var resultParameters = new DialogParameters();
                    resultParameters.Add(nameof(OutputColumnSettingEntity), columnSetting);
                    var dialogResult = new DialogResult(ButtonResult.OK, resultParameters);
                    callback?.Invoke(dialogResult);
                });
            var messageServiceMock = new Mock<IMessageService>();
            var commonDialogServiceMock = new Mock<ICommonDialogService>();
            var settingRepositoryMock = new Mock<ISettingRepository>();
            var viewModel = new CsvConvertViewModel(
                dialogServiceMock.Object,
                messageServiceMock.Object,
                commonDialogServiceMock.Object,
                logicMock.Object, csvFileMock.Object,
                settingRepositoryMock.Object);

            Assert.AreEqual(string.Empty, viewModel.InputCsvFilePath);
            Assert.AreEqual(string.Empty, viewModel.OutputCsvFilePath);
            Assert.AreEqual(false, viewModel.ExecuteCommand.CanExecute());
            Assert.AreEqual(0, viewModel.OutputColumns.Count);
            Assert.AreEqual(false, viewModel.InputCommand.CanExecute());

            viewModel.InputCsvFilePath = "InputCsvFilePath";
            Assert.AreEqual(false, viewModel.ExecuteCommand.CanExecute());
            Assert.AreEqual(true, viewModel.InputCommand.CanExecute());

            viewModel.InputCommand.Execute();
            viewModel.CreateOutputColumnFromInputCommand.Execute();
            Assert.AreEqual(3, viewModel.OutputColumns.Count);
            Assert.AreEqual("Field1", viewModel.OutputColumns[0].FieldName);
            Assert.AreEqual(true, viewModel.OutputColumns[0].IsOutput);
            Assert.AreEqual("Field2", viewModel.OutputColumns[1].FieldName);
            Assert.AreEqual(true, viewModel.OutputColumns[1].IsOutput);
            Assert.AreEqual("Field3", viewModel.OutputColumns[2].FieldName);
            Assert.AreEqual(true, viewModel.OutputColumns[2].IsOutput);

            ////新しい項目を作成する
            viewModel.CreateCommand.Execute();

            Assert.AreEqual("Field1", viewModel.OutputColumns[0].FieldName);
            Assert.AreEqual(true, viewModel.OutputColumns[0].IsOutput);
            Assert.AreEqual("Field2", viewModel.OutputColumns[1].FieldName);
            Assert.AreEqual(true, viewModel.OutputColumns[1].IsOutput);
            Assert.AreEqual("Field3", viewModel.OutputColumns[2].FieldName);
            Assert.AreEqual(true, viewModel.OutputColumns[2].IsOutput);
            Assert.AreEqual("Field4", viewModel.OutputColumns[3].FieldName);
            Assert.AreEqual(true, viewModel.OutputColumns[3].IsOutput);

            viewModel.OutputCsvFilePath = "OutputCsvFilePath";

            Assert.AreEqual(true, viewModel.ExecuteCommand.CanExecute());
            viewModel.ExecuteCommand.Execute();

            logicMock.VerifyAll();
            csvFileMock.VerifyAll();
            dialogServiceMock.VerifyAll();
        }

        [TestMethod]
        public void 結合項目追加シナリオ()
        {
            var logicMock = new Mock<ICsvConvertLogic>();
            logicMock.Setup(x => x.Execute(It.IsAny<InputCsvFileEntity>(), It.IsAny<OutputCsvFileEntity>(), It.IsAny<OutputSettingEntity>()))
                .Callback((InputCsvFileEntity inputFile, OutputCsvFileEntity outputFile, OutputSettingEntity setting) =>
                {
                    Assert.AreEqual("InputCsvFilePath", inputFile.CsvFilePath);
                    Assert.AreEqual("OutputCsvFilePath", outputFile.CsvFilePath);

                    ////実際に実装部分についても実行を行う
                    var logic = new CsvConvertLogic();
                    logic.Execute(inputFile, outputFile, setting);
                });
            var fileString =
@"Field1,Field2,Field3
A,B,C
a,b,c
";
            var csvFileMock = new Mock<ICsvFileRepository>();
            csvFileMock.Setup(x => x.GetData(It.IsAny<string>())).Returns(fileString);
            csvFileMock.Setup(x => x.WriteData(It.IsAny<string>(), It.IsAny<string>()))
                .Callback((string filePath, string outFileString) =>
                {
                    Assert.AreEqual("OutputCsvFilePath", filePath);
                    var expectFileString =
@"Field1,Field2,Field3,Field4
A,B,C,ABC
a,b,c,abc
";
                    Assert.AreEqual(expectFileString, outFileString);

                });

            var dialogServiceMock = new Mock<IDialogService>();
            dialogServiceMock.Setup(x => x.ShowDialog(It.IsAny<string>(), It.IsAny<IDialogParameters>(), It.IsAny<Action<IDialogResult>>()))
                .Callback((string name, IDialogParameters parameters, Action<IDialogResult> callback) =>
                {
                    var headers = parameters.GetValue<IReadOnlyList<HeaderEntity>>(nameof(CreateOutputColumnViewModel.InputHeaders));
                    Assert.AreEqual(3, headers.Count);
                    Assert.AreEqual(0, headers[0].HeaderId);
                    Assert.AreEqual("Field1", headers[0].HeaderName);
                    Assert.AreEqual(1, headers[1].HeaderId);
                    Assert.AreEqual("Field2", headers[1].HeaderName);
                    Assert.AreEqual(2, headers[2].HeaderId);
                    Assert.AreEqual("Field3", headers[2].HeaderName);

                    var columnSetting = new OutputColumnSettingEntity(0, "Field4", true, new ConcatenateTargetSettingEntity(
                        new List<HeaderEntity>() {
                            new HeaderEntity(0, "Field1"),
                            new HeaderEntity(1, "Field2"),
                            new HeaderEntity(2, "Field3"),
                        }));
                    var resultParameters = new DialogParameters();
                    resultParameters.Add(nameof(OutputColumnSettingEntity), columnSetting);
                    var dialogResult = new DialogResult(ButtonResult.OK, resultParameters);
                    callback?.Invoke(dialogResult);
                });
            var messageServiceMock = new Mock<IMessageService>();
            var commonDialogServiceMock = new Mock<ICommonDialogService>();
            var settingRepositoryMock = new Mock<ISettingRepository>();
            var viewModel = new CsvConvertViewModel(
                dialogServiceMock.Object,
                messageServiceMock.Object,
                commonDialogServiceMock.Object,
                logicMock.Object,
                csvFileMock.Object,
                settingRepositoryMock.Object);

            Assert.AreEqual(string.Empty, viewModel.InputCsvFilePath);
            Assert.AreEqual(string.Empty, viewModel.OutputCsvFilePath);
            Assert.AreEqual(false, viewModel.ExecuteCommand.CanExecute());
            Assert.AreEqual(0, viewModel.OutputColumns.Count);
            Assert.AreEqual(false, viewModel.InputCommand.CanExecute());

            viewModel.InputCsvFilePath = "InputCsvFilePath";
            Assert.AreEqual(false, viewModel.ExecuteCommand.CanExecute());
            Assert.AreEqual(true, viewModel.InputCommand.CanExecute());

            viewModel.InputCommand.Execute();
            viewModel.CreateOutputColumnFromInputCommand.Execute();
            Assert.AreEqual(3, viewModel.OutputColumns.Count);
            Assert.AreEqual("Field1", viewModel.OutputColumns[0].FieldName);
            Assert.AreEqual(true, viewModel.OutputColumns[0].IsOutput);
            Assert.AreEqual("Field2", viewModel.OutputColumns[1].FieldName);
            Assert.AreEqual(true, viewModel.OutputColumns[1].IsOutput);
            Assert.AreEqual("Field3", viewModel.OutputColumns[2].FieldName);
            Assert.AreEqual(true, viewModel.OutputColumns[2].IsOutput);

            ////新しい項目を作成する
            viewModel.CreateCommand.Execute();

            Assert.AreEqual("Field1", viewModel.OutputColumns[0].FieldName);
            Assert.AreEqual(true, viewModel.OutputColumns[0].IsOutput);
            Assert.AreEqual("Field2", viewModel.OutputColumns[1].FieldName);
            Assert.AreEqual(true, viewModel.OutputColumns[1].IsOutput);
            Assert.AreEqual("Field3", viewModel.OutputColumns[2].FieldName);
            Assert.AreEqual(true, viewModel.OutputColumns[2].IsOutput);
            Assert.AreEqual("Field4", viewModel.OutputColumns[3].FieldName);
            Assert.AreEqual(true, viewModel.OutputColumns[3].IsOutput);

            viewModel.OutputCsvFilePath = "OutputCsvFilePath";

            Assert.AreEqual(true, viewModel.ExecuteCommand.CanExecute());
            viewModel.ExecuteCommand.Execute();

            logicMock.VerifyAll();
            csvFileMock.VerifyAll();
            dialogServiceMock.VerifyAll();
        }

        [TestMethod]
        public void 表示確認シナリオ()
        {
            var logicMock = new Mock<ICsvConvertLogic>();
            logicMock.Setup(x => x.Execute(It.IsAny<InputCsvFileEntity>(), It.IsAny<OutputCsvFileEntity>(), It.IsAny<OutputSettingEntity>()))
                .Callback((InputCsvFileEntity inputFile, OutputCsvFileEntity outputFile, OutputSettingEntity setting) =>
                {
                    Assert.AreEqual("InputCsvFilePath", inputFile.CsvFilePath);
                    Assert.AreEqual("OutputCsvFilePath", outputFile.CsvFilePath);

                    ////実際に実装部分についても実行を行う
                    var logic = new CsvConvertLogic();
                    logic.Execute(inputFile, outputFile, setting);
                });
            var fileString =
@"Field1,Field2,Field3
A,B,C
a,b,c
";
            var csvFileMock = new Mock<ICsvFileRepository>();
            csvFileMock.Setup(x => x.GetData(It.IsAny<string>())).Returns(fileString);
            csvFileMock.Setup(x => x.WriteData(It.IsAny<string>(), It.IsAny<string>()))
                .Callback((string filePath, string outFileString) =>
                {
                    Assert.AreEqual("OutputCsvFilePath", filePath);
                    var expectFileString =
@"Field1,Field2,Field3,OutputField4,OutputField5
A,B,C,B,ABB
a,b,c,b,abb
";
                    Assert.AreEqual(expectFileString, outFileString);

                });

            var dialogServiceMock = new Mock<IDialogService>();
            var messageServiceMock = new Mock<IMessageService>();
            var commonDialogServiceMock = new Mock<ICommonDialogService>();
            var settingRepositoryMock = new Mock<ISettingRepository>();
            var viewModel = new CsvConvertViewModel(
                dialogServiceMock.Object,
                messageServiceMock.Object,
                commonDialogServiceMock.Object,
                logicMock.Object,
                csvFileMock.Object,
                settingRepositoryMock.Object);

            Assert.AreEqual(string.Empty, viewModel.InputCsvFilePath);
            Assert.AreEqual(string.Empty, viewModel.OutputCsvFilePath);
            Assert.AreEqual(false, viewModel.ExecuteCommand.CanExecute());
            Assert.AreEqual(0, viewModel.OutputColumns.Count);
            Assert.AreEqual(false, viewModel.InputCommand.CanExecute());

            viewModel.InputCsvFilePath = "InputCsvFilePath";
            Assert.AreEqual(false, viewModel.ExecuteCommand.CanExecute());
            Assert.AreEqual(true, viewModel.InputCommand.CanExecute());

            viewModel.InputCommand.Execute();
            viewModel.CreateOutputColumnFromInputCommand.Execute();
            Assert.AreEqual(3, viewModel.OutputColumns.Count);
            Assert.AreEqual("Field1", viewModel.OutputColumns[0].FieldName);
            Assert.AreEqual(true, viewModel.OutputColumns[0].IsOutput);
            Assert.AreEqual("Field2", viewModel.OutputColumns[1].FieldName);
            Assert.AreEqual(true, viewModel.OutputColumns[1].IsOutput);
            Assert.AreEqual("Field3", viewModel.OutputColumns[2].FieldName);
            Assert.AreEqual(true, viewModel.OutputColumns[2].IsOutput);

            ////新しい項目を作成する
            dialogServiceMock.Setup(x => x.ShowDialog(It.IsAny<string>(), It.IsAny<IDialogParameters>(), It.IsAny<Action<IDialogResult>>()))
                .Callback((string name, IDialogParameters parameters, Action<IDialogResult> callback) =>
                {
                    var inputHeaders = parameters.GetValue<IReadOnlyList<HeaderEntity>>(nameof(CreateOutputColumnViewModel.InputHeaders));
                    Assert.AreEqual(3, inputHeaders.Count);
                    Assert.AreEqual("Field1", inputHeaders[0].HeaderName);
                    Assert.AreEqual("Field2", inputHeaders[1].HeaderName);
                    Assert.AreEqual("Field3", inputHeaders[2].HeaderName);

                    var columnSetting = new OutputColumnSettingEntity(0, "OutputField4", true, new InputTargetSettingEntity("Field2"));
                    var resultParameters = new DialogParameters();
                    resultParameters.Add(nameof(OutputColumnSettingEntity), columnSetting);
                    var dialogResult = new DialogResult(ButtonResult.OK, resultParameters);
                    callback?.Invoke(dialogResult);
                });
            viewModel.CreateCommand.Execute();

            ////新しい項目をもう一つ作成する
            dialogServiceMock.Setup(x => x.ShowDialog(It.IsAny<string>(), It.IsAny<IDialogParameters>(), It.IsAny<Action<IDialogResult>>()))
                .Callback((string name, IDialogParameters parameters, Action<IDialogResult> callback) =>
                {
                    var inputHeaders = parameters.GetValue<IReadOnlyList<HeaderEntity>>(nameof(CreateOutputColumnViewModel.InputHeaders));
                    Assert.AreEqual(3, inputHeaders.Count);
                    Assert.AreEqual("Field1", inputHeaders[0].HeaderName);
                    Assert.AreEqual("Field2", inputHeaders[1].HeaderName);
                    Assert.AreEqual("Field3", inputHeaders[2].HeaderName);

                    var outputHeaders = parameters.GetValue<IReadOnlyList<HeaderEntity>>(nameof(CreateOutputColumnViewModel.OutputHeaders));
                    Assert.AreEqual(4, outputHeaders.Count);
                    Assert.AreEqual("Field1", outputHeaders[0].HeaderName);
                    Assert.AreEqual("Field2", outputHeaders[1].HeaderName);
                    Assert.AreEqual("Field3", outputHeaders[2].HeaderName);
                    Assert.AreEqual("OutputField4", outputHeaders[3].HeaderName);

                    var columnSetting = new OutputColumnSettingEntity(0, "OutputField5", true, new ConcatenateTargetSettingEntity(
                        new List<HeaderEntity>() {
                                        new HeaderEntity(0, "Field1"),
                                        new HeaderEntity(1, "Field2"),
                                        new HeaderEntity(2, "OutputField4"),
                    }));
                    var resultParameters = new DialogParameters();
                    resultParameters.Add(nameof(OutputColumnSettingEntity), columnSetting);
                    var dialogResult = new DialogResult(ButtonResult.OK, resultParameters);
                    callback?.Invoke(dialogResult);
                });
            viewModel.CreateCommand.Execute();

            Assert.AreEqual(5, viewModel.OutputColumns.Count);
            Assert.AreEqual("Field1", viewModel.OutputColumns[0].FieldName);
            Assert.AreEqual(true, viewModel.OutputColumns[0].IsOutput);
            Assert.AreEqual("Field1", viewModel.OutputColumns[0].FieldContent);
            Assert.AreEqual("Field1", viewModel.OutputColumns[0].InputFieldName);
            Assert.AreEqual("Field2", viewModel.OutputColumns[1].FieldName);
            Assert.AreEqual(true, viewModel.OutputColumns[1].IsOutput);
            Assert.AreEqual("Field2", viewModel.OutputColumns[1].FieldContent);
            Assert.AreEqual("Field2", viewModel.OutputColumns[1].InputFieldName);
            Assert.AreEqual("Field3", viewModel.OutputColumns[2].FieldName);
            Assert.AreEqual(true, viewModel.OutputColumns[2].IsOutput);
            Assert.AreEqual("Field3", viewModel.OutputColumns[2].FieldContent);
            Assert.AreEqual("Field3", viewModel.OutputColumns[2].InputFieldName);
            Assert.AreEqual("OutputField4", viewModel.OutputColumns[3].FieldName);
            Assert.AreEqual(true, viewModel.OutputColumns[3].IsOutput);
            Assert.AreEqual("Field2", viewModel.OutputColumns[3].FieldContent);
            Assert.AreEqual("Field2", viewModel.OutputColumns[3].InputFieldName);
            Assert.AreEqual("OutputField5", viewModel.OutputColumns[4].FieldName);
            Assert.AreEqual(true, viewModel.OutputColumns[4].IsOutput);
            Assert.AreEqual("Field1+Field2+OutputField4", viewModel.OutputColumns[4].FieldContent);
            Assert.AreEqual("", viewModel.OutputColumns[4].InputFieldName);

            viewModel.OutputCsvFilePath = "OutputCsvFilePath";

            Assert.AreEqual(true, viewModel.ExecuteCommand.CanExecute());
            viewModel.ExecuteCommand.Execute();

            logicMock.VerifyAll();
            csvFileMock.VerifyAll();
            dialogServiceMock.VerifyAll();
        }



        [TestMethod]
        public void 設定保存_読み込みシナリオ()
        {
            var logicMock = new Mock<ICsvConvertLogic>();
            logicMock.Setup(x => x.Execute(It.IsAny<InputCsvFileEntity>(), It.IsAny<OutputCsvFileEntity>(), It.IsAny<OutputSettingEntity>()))
                .Callback((InputCsvFileEntity inputFile, OutputCsvFileEntity outputFile, OutputSettingEntity setting) =>
                {
                    Assert.AreEqual("InputCsvFilePath", inputFile.CsvFilePath);
                    Assert.AreEqual("OutputCsvFilePath", outputFile.CsvFilePath);

                    ////実際に実装部分についても実行を行う
                    var logic = new CsvConvertLogic();
                    logic.Execute(inputFile, outputFile, setting);
                });
            var fileString =
@"Field1,Field2,Field3
A,B,C
a,b,c
";
            var csvFileMock = new Mock<ICsvFileRepository>();
            csvFileMock.Setup(x => x.GetData(It.IsAny<string>())).Returns(fileString);
            csvFileMock.Setup(x => x.WriteData(It.IsAny<string>(), It.IsAny<string>()))
                .Callback((string filePath, string outFileString) =>
                {
                    Assert.AreEqual("OutputCsvFilePath", filePath);
                    var expectFileString =
@"OutputField1,OutputField2,OutputField3,OutputField4,OutputField5
A,B,C,B,ABB
a,b,c,b,abb
";
                    Assert.AreEqual(expectFileString, outFileString);

                });
            var settingRepositoryMock = new Mock<ISettingRepository>();
            var dialogServiceMock = new Mock<IDialogService>();
            var messageServiceMock = new Mock<IMessageService>();
            var commonDialogServiceMock = new Mock<ICommonDialogService>();
            var viewModel = new CsvConvertViewModel(
                dialogServiceMock.Object,
                messageServiceMock.Object,
                commonDialogServiceMock.Object,
                logicMock.Object,
                csvFileMock.Object,
                settingRepositoryMock.Object);

            ////初期表示
            Assert.AreEqual(string.Empty, viewModel.InputCsvFilePath);
            Assert.AreEqual(string.Empty, viewModel.OutputCsvFilePath);
            Assert.AreEqual(0, viewModel.InputHeaders.Count);
            Assert.AreEqual(0, viewModel.OutputColumns.Count);
            Assert.AreEqual(false, viewModel.InputCommand.CanExecute());
            Assert.AreEqual(false, viewModel.CreateOutputColumnFromInputCommand.CanExecute());
            Assert.AreEqual(false, viewModel.ExecuteCommand.CanExecute());
            Assert.AreEqual("", viewModel.SettingName);
            Assert.AreEqual("", viewModel.SettingFilePath);
            Assert.AreEqual(true, viewModel.ReadSettingCommand.CanExecute());
            Assert.AreEqual(false, viewModel.SaveSettingCommand.CanExecute());

            ////設定ファイル読み込み
            commonDialogServiceMock.Setup(x => x.ShowDialog(It.IsAny<ICommonDialogSettings>())).Returns(true)
                .Callback((ICommonDialogSettings settings) => {
                    var fileSettings = settings as FileDialogSettings;
                    Assert.IsNotNull(fileSettings);
                    fileSettings.FileName = "SettingFile";
                });
            var settingEntity = new SettingEntity(
                "SampleSetting",
                new List<HeaderEntity>()
                {
                    new HeaderEntity(0, "Field1"),
                    new HeaderEntity(1, "Field2"),
                    new HeaderEntity(2, "Field3"),
                },
                new OutputSettingEntity(
                    new List<OutputColumnSettingEntity>()
                    {
                        new OutputColumnSettingEntity(0, "OutputField1", true, new InputTargetSettingEntity("Field1")),
                        new OutputColumnSettingEntity(1, "OutputField2", true, new InputTargetSettingEntity("Field2")),
                        new OutputColumnSettingEntity(2, "OutputField3", true, new InputTargetSettingEntity("Field3")),
                        new OutputColumnSettingEntity(3, "OutputField4", true, new InputTargetSettingEntity("Field2")),
                        new OutputColumnSettingEntity(4, "OutputField5", true, new ConcatenateTargetSettingEntity(
                            new List<HeaderEntity>()
                            {
                                new HeaderEntity(0, "OutputField1"),
                                new HeaderEntity(1, "OutputField2"),
                                new HeaderEntity(3, "OutputField4"),
                            })),
                    }));
            settingRepositoryMock.Setup(x => x.Read(It.IsAny<string>())).Returns(settingEntity);
            viewModel.ReadSettingCommand.Execute();
            Assert.AreEqual("SettingFile", viewModel.SettingFilePath);
            Assert.AreEqual("SampleSetting", viewModel.SettingName);
            Assert.AreEqual(3, viewModel.InputHeaders.Count);
            Assert.AreEqual("Field1", viewModel.InputHeaders[0].HeaderName);
            Assert.AreEqual(0, viewModel.InputHeaders[0].HeaderId);
            Assert.AreEqual("Field2", viewModel.InputHeaders[1].HeaderName);
            Assert.AreEqual(1, viewModel.InputHeaders[1].HeaderId);
            Assert.AreEqual("Field3", viewModel.InputHeaders[2].HeaderName);
            Assert.AreEqual(2, viewModel.InputHeaders[2].HeaderId);
            Assert.AreEqual(5, viewModel.OutputColumns.Count);
            Assert.AreEqual("OutputField1", viewModel.OutputColumns[0].FieldName);
            Assert.AreEqual(true, viewModel.OutputColumns[0].IsOutput);
            Assert.AreEqual("Field1", viewModel.OutputColumns[0].FieldContent);
            Assert.AreEqual("Field1", viewModel.OutputColumns[0].InputFieldName);
            Assert.AreEqual("OutputField2", viewModel.OutputColumns[1].FieldName);
            Assert.AreEqual(true, viewModel.OutputColumns[1].IsOutput);
            Assert.AreEqual("Field2", viewModel.OutputColumns[1].FieldContent);
            Assert.AreEqual("Field2", viewModel.OutputColumns[1].InputFieldName);
            Assert.AreEqual("OutputField3", viewModel.OutputColumns[2].FieldName);
            Assert.AreEqual(true, viewModel.OutputColumns[2].IsOutput);
            Assert.AreEqual("Field3", viewModel.OutputColumns[2].FieldContent);
            Assert.AreEqual("Field3", viewModel.OutputColumns[2].InputFieldName);
            Assert.AreEqual("OutputField4", viewModel.OutputColumns[3].FieldName);
            Assert.AreEqual(true, viewModel.OutputColumns[3].IsOutput);
            Assert.AreEqual("Field2", viewModel.OutputColumns[3].FieldContent);
            Assert.AreEqual("Field2", viewModel.OutputColumns[3].InputFieldName);
            Assert.AreEqual("OutputField5", viewModel.OutputColumns[4].FieldName);
            Assert.AreEqual(true, viewModel.OutputColumns[4].IsOutput);
            Assert.AreEqual("OutputField1+OutputField2+OutputField4", viewModel.OutputColumns[4].FieldContent);
            Assert.AreEqual("", viewModel.OutputColumns[4].InputFieldName);

            ////入力パス設定
            viewModel.InputCsvFilePath = "InputCsvFilePath";
            Assert.AreEqual(true, viewModel.InputCommand.CanExecute());
            Assert.AreEqual(true, viewModel.CreateOutputColumnFromInputCommand.CanExecute());
            Assert.AreEqual(false, viewModel.ExecuteCommand.CanExecute());

            ////出力パス設定
            viewModel.OutputCsvFilePath = "OutputCsvFilePath";
            Assert.AreEqual(true, viewModel.ExecuteCommand.CanExecute());
            viewModel.ExecuteCommand.Execute();

            ////設定の保存
            Assert.AreEqual(true, viewModel.SaveSettingCommand.CanExecute());
            commonDialogServiceMock.Setup(x => x.ShowDialog(It.IsAny<ICommonDialogSettings>())).Returns(true)
                .Callback((ICommonDialogSettings settings) =>
                {
                    var fileSettings = settings as FileDialogSettings;
                    Assert.IsNotNull(fileSettings);
                    fileSettings.FileName = "SaveSettingFile";
                });
            settingRepositoryMock.Setup(x => x.Save(It.IsAny<string>(), It.IsAny<SettingEntity>()))
                .Callback((string settingFilePath, SettingEntity setting) => {
                    Assert.AreEqual("SaveSettingFile", settingFilePath);

                    Assert.AreEqual("SampleSetting", setting.SettingName);
                    Assert.AreEqual(3, setting.Headers.Count);
                    Assert.AreEqual("Field1", setting.Headers[0].HeaderName);
                    Assert.AreEqual(0, setting.Headers[0].HeaderId);
                    Assert.AreEqual("Field2", setting.Headers[1].HeaderName);
                    Assert.AreEqual(1, setting.Headers[1].HeaderId);
                    Assert.AreEqual("Field3", setting.Headers[2].HeaderName);
                    Assert.AreEqual(2, setting.Headers[2].HeaderId);
                    var outputSetting = setting.OutputSetting;
                    var outputColumns = outputSetting.ColumnSettings;

                    Assert.AreEqual(5, outputColumns.Count);
                    Assert.AreEqual(0, outputColumns[0].Index);
                    Assert.AreEqual("OutputField1", outputColumns[0].OutputHeader);
                    Assert.AreEqual(true, outputColumns[0].IsOutput);
                    var outputTarget = outputColumns[0].TargetSetting as InputTargetSettingEntity;
                    Assert.IsNotNull(outputTarget);
                    Assert.AreEqual("Field1", outputTarget.InputHeaderName);

                    Assert.AreEqual(1, outputColumns[1].Index);
                    Assert.AreEqual("OutputField2", outputColumns[1].OutputHeader);
                    Assert.AreEqual(true, outputColumns[1].IsOutput);
                    outputTarget = outputColumns[1].TargetSetting as InputTargetSettingEntity;
                    Assert.IsNotNull(outputTarget);
                    Assert.AreEqual("Field2", outputTarget.InputHeaderName);

                    Assert.AreEqual(2, outputColumns[2].Index);
                    Assert.AreEqual("OutputField3", outputColumns[2].OutputHeader);
                    Assert.AreEqual(true, outputColumns[2].IsOutput);
                    outputTarget = outputColumns[2].TargetSetting as InputTargetSettingEntity;
                    Assert.IsNotNull(outputTarget);
                    Assert.AreEqual("Field3", outputTarget.InputHeaderName);

                    Assert.AreEqual(3, outputColumns[3].Index);
                    Assert.AreEqual("OutputField4", outputColumns[3].OutputHeader);
                    Assert.AreEqual(true, outputColumns[3].IsOutput);
                    outputTarget = outputColumns[3].TargetSetting as InputTargetSettingEntity;
                    Assert.IsNotNull(outputTarget);
                    Assert.AreEqual("Field2", outputTarget.InputHeaderName);

                    Assert.AreEqual(4, outputColumns[4].Index);
                    Assert.AreEqual("OutputField5", outputColumns[4].OutputHeader);
                    Assert.AreEqual(true, outputColumns[4].IsOutput);
                    var concatenateTarget = outputColumns[4].TargetSetting as ConcatenateTargetSettingEntity;
                    Assert.IsNotNull(concatenateTarget);
                    Assert.AreEqual(3, concatenateTarget.OutputHeaderEntities.Count);
                    Assert.AreEqual(0, concatenateTarget.OutputHeaderEntities[0].HeaderId);
                    Assert.AreEqual("OutputField1", concatenateTarget.OutputHeaderEntities[0].HeaderName);
                    Assert.AreEqual(1, concatenateTarget.OutputHeaderEntities[1].HeaderId);
                    Assert.AreEqual("OutputField2", concatenateTarget.OutputHeaderEntities[1].HeaderName);
                    Assert.AreEqual(3, concatenateTarget.OutputHeaderEntities[2].HeaderId);
                    Assert.AreEqual("OutputField4", concatenateTarget.OutputHeaderEntities[2].HeaderName);
                });
            messageServiceMock.Setup(x => x.ShowDialog(It.IsAny<string>())).Callback((string message) =>
            {
                Assert.AreEqual("設定を保存しました。", message);
            });
            viewModel.SaveSettingCommand.Execute();
            Assert.AreEqual("SaveSettingFile", viewModel.SettingFilePath);


            logicMock.VerifyAll();
            csvFileMock.VerifyAll();
            dialogServiceMock.VerifyAll();
            settingRepositoryMock.VerifyAll();
            commonDialogServiceMock.VerifyAll();
            messageServiceMock.VerifyAll();
        }
    }
}
