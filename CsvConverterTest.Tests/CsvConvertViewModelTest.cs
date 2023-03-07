using CsvConverter.Domain.Logics;
using CsvConverter.WPF.ViewModels;
using Moq;

namespace CsvConverterTest.Tests
{
    [TestClass]
    public class CsvConvertViewModelTest
    {
        [TestMethod]
        public void シナリオ()
        {
            var logicMock = new Mock<ICsvConvertLogic>();
            var viewModel = new CsvConvertViewModel(logicMock.Object);

            Assert.AreEqual(string.Empty, viewModel.InputCsvFilePath);
            Assert.AreEqual(string.Empty, viewModel.OutputCsvFilePath);

            viewModel.InputCsvFilePath = "InputCsvFilePath";
            viewModel.OutputCsvFilePath = "OutputCsvFilePath";

            viewModel.ExecuteCommand.Execute();

        }
    }
}
