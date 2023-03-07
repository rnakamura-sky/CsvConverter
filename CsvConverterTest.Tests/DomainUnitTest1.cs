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
            inputCsvFileMock.Setup(x => x.GetData()).Returns("SampleData");
            var outputCsvFileMock = new Mock<IOutputCsvFileRepository>();
            outputCsvFileMock.Setup(x => x.WriteData("SampleData")).Callback((string data) =>
            {
                Assert.AreEqual("SampleData", data);
            });

            var inputCsvFile = new InputCsvFileEntity(inputCsvFileMock.Object, "InputFilePath");

            var data = inputCsvFile.GetData();
            Assert.AreEqual("SampleData", data);

            var outputCsvFile = new OutputCsvFileEntity(outputCsvFileMock.Object, "OutputFilePath");
            outputCsvFile.WriteData(data);


            inputCsvFileMock.VerifyAll();
            outputCsvFileMock.VerifyAll();

        }

        [TestMethod]
        public void ロジックのテスト()
        {
            var inputCsvFileMock = new Mock<IInputCsvFileRepository>();
            inputCsvFileMock.Setup(x => x.GetData()).Returns("SampleData");
            var outputCsvFileMock = new Mock<IOutputCsvFileRepository>();
            outputCsvFileMock.Setup(x => x.WriteData("SampleData")).Callback((string data) =>
            {
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