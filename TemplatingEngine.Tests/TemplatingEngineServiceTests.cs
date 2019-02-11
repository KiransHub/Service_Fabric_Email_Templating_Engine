using System;
using System.Collections.Generic;
using System.Fabric;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Data;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using TemplatingEngine.External.Models;
using TemplatingEngine.Repositories;
using TemplatingEngine.Services;

namespace TemplatingEngine.Tests
{
    [TestClass]
    public class TemplatingEngineServiceTests
    {
        private readonly Dictionary<long, IModel> _dictionary = new Dictionary<long, IModel>();
        private static TemplatingEngineService _service;
        private Mock<CommunicationsTemplateModel> _mockModel;

        [TestInitialize]
        public void TestInitialize()
        {
            _mockModel = new Mock<CommunicationsTemplateModel>();
            var mockTransaction = new Mock<ITransaction>();

            var mockReliableRepository = new Mock<IReliableRepositoryAsync<CommunicationsTemplateModel>>();

            mockReliableRepository.Setup(x => x.Create(_mockModel.Object))
                .Callback(() => { _dictionary.Add(0, _mockModel.Object); })
                .Returns(Task.Run(() => (long) 0));

            mockReliableRepository.Setup(x => x.Delete(It.IsAny<long>()))
                .Callback((long id) => { _dictionary.Remove(id); })
                .Returns(Task.Run(() => new ConditionalValue<CommunicationsTemplateModel>(true, new CommunicationsTemplateModel())));

            mockReliableRepository.Setup(x => x.Update(It.IsAny<long>(), It.IsAny<CommunicationsTemplateModel>()))
                .Callback((long id, CommunicationsTemplateModel value) => { _dictionary[id] = value; })
                .Returns(() => Task.Delay(0));

            var codePackageActivationContext = new Mock<ICodePackageActivationContext>();
            var serviceContext = new StatefulServiceContext(new NodeContext("", new NodeId(8, 8), 8, "", ""), codePackageActivationContext.Object, string.Empty, new Uri("http://boo.net"), null, Guid.NewGuid(), 0L);

            _service = new TemplatingEngineService(serviceContext, mockReliableRepository.Object);
        }

        [TestCleanup]
        public void TestCleanup()
            => _dictionary.Clear();

        [TestMethod]
        public void Can_Instantiate()
            => Assert.IsNotNull(_service);

        [TestMethod]
        public void Can_Create_Items()
        {
            var tempResult = _service.CreateTemplateAsync(_mockModel.Object).Result;
            var count = _dictionary.Count;

            Assert.AreEqual(1, count);
        }

        [TestMethod]
        public void Can_Get_Number_Of_Items()
        {
            _dictionary.Add(0, _mockModel.Object);
            _dictionary.Add(1, _mockModel.Object);
            _dictionary.Add(2, _mockModel.Object);

            var tempResult = _service.GetTemplatesAsync().Result; 
            var count = _dictionary.Count;

            Assert.AreEqual(3, count);
        }

        [TestMethod]
        public void Can_Delete_Items()
        {
            _dictionary.Add(0, _mockModel.Object);
            _dictionary.Add(1, _mockModel.Object);
            _dictionary.Add(2, _mockModel.Object);

            var task = _service.DeleteTemplateById(0);

            while (task.IsCompleted == false)
                Thread.SpinWait(100);

            Assert.AreEqual(2, _dictionary.Count);
            Assert.IsFalse(_dictionary.ContainsKey(0));
        }


        [TestMethod]
        public void Can_Update_Items()
        {
            const string originalContent = "hello world";
            const string newContent = "hello { Name }";

            _dictionary.Add(0, _mockModel.Object);
            _dictionary.Add(1, _mockModel.Object);

            _dictionary.Add(2, new CommunicationsTemplateModel()
            {
                Content = originalContent
            });

            var task = _service.UpdateTemplate(2, new CommunicationsTemplateModel()
            {
                Content = newContent
            });

            while (task.IsCompleted == false)
                Thread.SpinWait(5000);

            var result = _dictionary[2] as CommunicationsTemplateModel;

            Assert.AreEqual(3, _dictionary.Count);
            Assert.IsTrue(_dictionary.ContainsKey(2));
            Assert.IsNotNull(result);
            Assert.AreEqual(newContent, result.Content);
        }

    }
}