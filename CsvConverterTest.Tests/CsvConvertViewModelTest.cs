using CsvConverter.Domain.Entities;
using CsvConverter.Domain.Logics;
using CsvConverter.Domain.Repositories;
using CsvConverter.WPF.Services;
using CsvConverter.WPF.ViewModels;
using Moq;
using NuGet.Frameworks;
using Prism.Services.Dialogs;

namespace CsvConverterTest.Tests
{
    [TestClass]
    public class CsvConvertViewModelTest
    {
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
            var viewModel = new CsvConvertViewModel(
                dialogServiceMock.Object,
                messageServiceMock.Object,
                commonDialogServiceMock.Object,
                logicMock.Object,
                csvFileMock.Object);

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
            var viewModel = new CsvConvertViewModel(
                dialogServiceMock.Object,
                messageServiceMock.Object,
                commonDialogServiceMock.Object,
                logicMock.Object,
                csvFileMock.Object);

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
            var viewModel = new CsvConvertViewModel(
                dialogServiceMock.Object,
                messageServiceMock.Object,
                commonDialogServiceMock.Object,
                logicMock.Object,
                csvFileMock.Object);

            Assert.AreEqual(string.Empty, viewModel.InputCsvFilePath);
            Assert.AreEqual(string.Empty, viewModel.OutputCsvFilePath);
            Assert.AreEqual(false, viewModel.ExecuteCommand.CanExecute());
            Assert.AreEqual(0, viewModel.OutputColumns.Count);
            Assert.AreEqual(false, viewModel.InputCommand.CanExecute());

            viewModel.InputCsvFilePath = "InputCsvFilePath";
            Assert.AreEqual(false, viewModel.ExecuteCommand.CanExecute());
            Assert.AreEqual(true, viewModel.InputCommand.CanExecute());

            viewModel.InputCommand.Execute();
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
            var dialogServiceMock = new Mock<IDialogService>();
            var messageServiceMock = new Mock<IMessageService>();
            var commonDialogServiceMock = new Mock<ICommonDialogService>();
            var viewModel = new CsvConvertViewModel(
                dialogServiceMock.Object,
                messageServiceMock.Object,
                commonDialogServiceMock.Object,
                logicMock.Object,
                csvFileMock.Object);

            Assert.AreEqual(string.Empty, viewModel.InputCsvFilePath);
            Assert.AreEqual(string.Empty, viewModel.OutputCsvFilePath);
            Assert.AreEqual(false, viewModel.ExecuteCommand.CanExecute());
            Assert.AreEqual(0, viewModel.OutputColumns.Count);
            Assert.AreEqual(false, viewModel.InputCommand.CanExecute());

            viewModel.InputCsvFilePath = "InputCsvFilePath";
            Assert.AreEqual(false, viewModel.ExecuteCommand.CanExecute());
            Assert.AreEqual(true, viewModel.InputCommand.CanExecute());

            viewModel.InputCommand.Execute();
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
            var viewModel = new CsvConvertViewModel(
                dialogServiceMock.Object,
                messageServiceMock.Object,
                commonDialogServiceMock.Object,
                logicMock.Object, csvFileMock.Object);

            Assert.AreEqual(string.Empty, viewModel.InputCsvFilePath);
            Assert.AreEqual(string.Empty, viewModel.OutputCsvFilePath);
            Assert.AreEqual(false, viewModel.ExecuteCommand.CanExecute());
            Assert.AreEqual(0, viewModel.OutputColumns.Count);
            Assert.AreEqual(false, viewModel.InputCommand.CanExecute());

            viewModel.InputCsvFilePath = "InputCsvFilePath";
            Assert.AreEqual(false, viewModel.ExecuteCommand.CanExecute());
            Assert.AreEqual(true, viewModel.InputCommand.CanExecute());

            viewModel.InputCommand.Execute();
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
                    Assert.AreEqual("Field1", headers[0].HeaderName);
                    Assert.AreEqual("Field2", headers[1].HeaderName);
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
            var viewModel = new CsvConvertViewModel(
                dialogServiceMock.Object,
                messageServiceMock.Object,
                commonDialogServiceMock.Object,
                logicMock.Object,
                csvFileMock.Object);

            Assert.AreEqual(string.Empty, viewModel.InputCsvFilePath);
            Assert.AreEqual(string.Empty, viewModel.OutputCsvFilePath);
            Assert.AreEqual(false, viewModel.ExecuteCommand.CanExecute());
            Assert.AreEqual(0, viewModel.OutputColumns.Count);
            Assert.AreEqual(false, viewModel.InputCommand.CanExecute());

            viewModel.InputCsvFilePath = "InputCsvFilePath";
            Assert.AreEqual(false, viewModel.ExecuteCommand.CanExecute());
            Assert.AreEqual(true, viewModel.InputCommand.CanExecute());

            viewModel.InputCommand.Execute();
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
            var viewModel = new CsvConvertViewModel(
                dialogServiceMock.Object,
                messageServiceMock.Object,
                commonDialogServiceMock.Object,
                logicMock.Object,
                csvFileMock.Object);

            Assert.AreEqual(string.Empty, viewModel.InputCsvFilePath);
            Assert.AreEqual(string.Empty, viewModel.OutputCsvFilePath);
            Assert.AreEqual(false, viewModel.ExecuteCommand.CanExecute());
            Assert.AreEqual(0, viewModel.OutputColumns.Count);
            Assert.AreEqual(false, viewModel.InputCommand.CanExecute());

            viewModel.InputCsvFilePath = "InputCsvFilePath";
            Assert.AreEqual(false, viewModel.ExecuteCommand.CanExecute());
            Assert.AreEqual(true, viewModel.InputCommand.CanExecute());

            viewModel.InputCommand.Execute();
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
    }
}
