using CsvConverter.Domain.Entities;
using CsvConverter.Domain.Logics;
using CsvConverter.Domain.Repositories;
using CsvConverter.WPF.ViewModels;
using Moq;

namespace CsvConverterTest.Tests
{
    [TestClass]
    public class CsvConvertViewModelTest
    {
        [TestMethod]
        public void 入力ファイルと出力ファイルを指定してそのまま出力するシナリオ()
        {
            var logicMock = new Mock<ICsvConvertLogic>();
            logicMock.Setup(x => x.Execute(It.IsAny<InputCsvFileEntity>(), It.IsAny<OutputCsvFileEntity>(), It.IsAny<OutputSettingEntity>()))
                .Callback((InputCsvFileEntity inputFile, OutputCsvFileEntity outputFile, OutputSettingEntity setting) =>
                {
                    Assert.AreEqual("InputCsvFilePath", inputFile.CsvFilePath);
                    Assert.AreEqual("OutputCsvFilePath", outputFile.CsvFilePath);
                });
            var csvFileMock = new Mock<ICsvFileRepository>();
            var viewModel = new CsvConvertViewModel(logicMock.Object, csvFileMock.Object);

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
            var viewModel = new CsvConvertViewModel(logicMock.Object, csvFileMock.Object);

            Assert.AreEqual(string.Empty, viewModel.InputCsvFilePath);
            Assert.AreEqual(string.Empty, viewModel.OutputCsvFilePath);
            Assert.AreEqual(false, viewModel.ExecuteCommand.CanExecute());
            Assert.AreEqual(0, viewModel.OutputRows.Count);
            Assert.AreEqual(false, viewModel.InputCommand.CanExecute());

            viewModel.InputCsvFilePath = "InputCsvFilePath";
            Assert.AreEqual(false, viewModel.ExecuteCommand.CanExecute());
            Assert.AreEqual(true, viewModel.InputCommand.CanExecute());

            viewModel.InputCommand.Execute();
            Assert.AreEqual(3, viewModel.OutputRows.Count);
            Assert.AreEqual("Field1", viewModel.OutputRows[0].FieldName);
            Assert.AreEqual("Field2", viewModel.OutputRows[1].FieldName);
            Assert.AreEqual("Field3", viewModel.OutputRows[2].FieldName);

            viewModel.SelectedOutputRowIndex = 0;
            viewModel.ReplaceOutputRowCommand.Execute(2);
            Assert.AreEqual("Field2", viewModel.OutputRows[0].FieldName);
            Assert.AreEqual("Field3", viewModel.OutputRows[1].FieldName);
            Assert.AreEqual("Field1", viewModel.OutputRows[2].FieldName);

            viewModel.OutputCsvFilePath = "OutputCsvFilePath";

            Assert.AreEqual(true, viewModel.ExecuteCommand.CanExecute());
            viewModel.ExecuteCommand.Execute();

            logicMock.VerifyAll();
            csvFileMock.VerifyAll();
        }
    }
}
