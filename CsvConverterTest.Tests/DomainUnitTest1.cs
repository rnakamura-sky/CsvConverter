using CsvConverter.Domain.Entities;
using CsvConverter.Domain.Logics;
using CsvConverter.Domain.Repositories;
using Moq;

namespace CsvConverterTest.Tests
{
    [TestClass]
    public class DomainUnitTest1
    {
        [TestMethod]
        public void CSVファイルを読み取って出力する()
        {
            var inputCsvFileMock = new Mock<IInputCsvFileRepository>();
            inputCsvFileMock.Setup(x => x.GetData("InputFilePath")).Returns("SampleData");
            var outputCsvFileMock = new Mock<IOutputCsvFileRepository>();
            outputCsvFileMock.Setup(x => x.WriteData("OutputFilePath", "SampleData")).Callback((string filePath, string data) =>
            {
                Assert.AreEqual("OutputFilePath", filePath);
                Assert.AreEqual("SampleData", data);
            });

            var inputCsvFile = new InputCsvFileEntity(inputCsvFileMock.Object, "InputFilePath");

            var data = inputCsvFile.GetData();
            Assert.AreEqual("SampleData", data.GetFileString());

            var outputCsvFile = new OutputCsvFileEntity(outputCsvFileMock.Object, "OutputFilePath");
            outputCsvFile.WriteData(data);


            inputCsvFileMock.VerifyAll();
            outputCsvFileMock.VerifyAll();

        }

        [TestMethod]
        public void ロジックのテスト()
        {
            var inputCsvFileMock = new Mock<IInputCsvFileRepository>();
            inputCsvFileMock.Setup(x => x.GetData(It.IsAny<string>())).Returns("SampleData");
            var outputCsvFileMock = new Mock<IOutputCsvFileRepository>();
            outputCsvFileMock.Setup(x => x.WriteData("OutputFilePath", "SampleData")).Callback((string filePath, string data) =>
            {
                Assert.AreEqual("OutputFilePath", filePath);
                Assert.AreEqual("SampleData", data);
            });

            var inputCsvFile = new InputCsvFileEntity(inputCsvFileMock.Object, "InputFilePath");
            var outputCsvFile = new OutputCsvFileEntity(outputCsvFileMock.Object, "OutputFilePath");


            var logic = new CsvConvertLogic();
            logic.Execute(inputCsvFile, outputCsvFile);


            inputCsvFileMock.VerifyAll();
            outputCsvFileMock.VerifyAll();

        }
    }
}