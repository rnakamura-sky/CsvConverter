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
            logicMock.Setup(x => x.Execute(It.IsAny<InputCsvFileEntity>(), It.IsAny<OutputCsvFileEntity>()))
                .Callback((InputCsvFileEntity inputFile, OutputCsvFileEntity outputFile) =>
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
        public void 出力項目をちょっといじって出力するシナリオ()
        {
            var logicMock = new Mock<ICsvConvertLogic>();
            logicMock.Setup(x => x.Execute(It.IsAny<InputCsvFileEntity>(), It.IsAny<OutputCsvFileEntity>()))
                .Callback((InputCsvFileEntity inputFile, OutputCsvFileEntity outputFile) =>
                {
                    Assert.AreEqual("InputCsvFilePath", inputFile.CsvFilePath);
                    Assert.AreEqual("OutputCsvFilePath", outputFile.CsvFilePath);
                });
            var fileString =
@"Field1,Field2,Field3
A,B,C
a,b,c
";
            var csvFileMock = new Mock<ICsvFileRepository>();
            csvFileMock.Setup(x => x.GetData(It.IsAny<string>())).Returns(fileString);
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
            Assert.AreEqual(2, viewModel.OutputRows.Count);

            viewModel.OutputCsvFilePath = "OutputCsvFilePath";

            Assert.AreEqual(true, viewModel.ExecuteCommand.CanExecute());
            viewModel.ExecuteCommand.Execute();

            logicMock.VerifyAll();
        }
    }
}
