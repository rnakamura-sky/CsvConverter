using CsvConverter.Domain.Entities;
using CsvConverter.Domain.Repositories;
using Moq;

namespace CsvConverterTest.Tests
{
    [TestClass]
    public class DomainUnitTest1
    {
        [TestMethod]
        public void CSV�t�@�C����ǂݎ���ďo�͂���()
        {
            var inputCsvFileMock = new Mock<ICsvFileRepository>();
            inputCsvFileMock.Setup(x => x.GetData()).Returns("SampleData");
            var outputCsvFileMock = new Mock<ICsvFileRepository>();
            outputCsvFileMock.Setup(x => x.WriteData("SampleData")).Callback((string data) =>
            {
                Assert.AreEqual("SampleData", data);
            });

            var inputCsvFile = new CsvFileEntity(inputCsvFileMock.Object, "InputFilePath");

            var data = inputCsvFile.GetData();
            Assert.AreEqual("SampleData", data);

            var outputCsvFile = new CsvFileEntity(outputCsvFileMock.Object, "OutputFilePath");
            outputCsvFile.WriteData(data);


            inputCsvFileMock.VerifyAll();
            outputCsvFileMock.VerifyAll();

        }
    }
}