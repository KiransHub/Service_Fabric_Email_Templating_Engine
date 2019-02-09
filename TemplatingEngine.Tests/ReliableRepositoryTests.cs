using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Data;
using Microsoft.ServiceFabric.Data.Collections;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Client;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using TemplatingEngine.External.Models;
using TemplatingEngine.Repositories;

namespace TemplatingEngine.Tests
{
    [TestClass]
    public class ReliableRepositoryTests
    {
        private readonly Dictionary<long, IModel> _dictionary = new Dictionary<long, IModel>();
        private ReliableRepositoryAsync<IModel> _repo;
        private Mock<IModel> _mockModel;

        [TestInitialize]
        public void TestInitialize()
        {
            _mockModel = new Mock<IModel>();
            var mockTransaction = new Mock<ITransaction>();

            var mockReliableDict = new Mock<IReliableDictionary<long, IModel>>();

            mockReliableDict.Setup(x => x.AddAsync(mockTransaction.Object, 0, _mockModel.Object))
                .Callback(() => { _dictionary.Add(0, _mockModel.Object); })
                .Returns(Task.Delay(0));

            mockReliableDict.Setup(x => x.TryRemoveAsync(mockTransaction.Object, It.IsAny<long>()))
                .Callback((ITransaction tx, long id) => { _dictionary.Remove(id); });

            mockReliableDict.Setup(x => x.SetAsync(mockTransaction.Object, It.IsAny<long>(), It.IsAny<IModel>()))
                .Callback((ITransaction tx, long id, IModel value) => { _dictionary[id] = value; });

            var mockStateManager = new Mock<IReliableStateManager>();

            mockStateManager
                .Setup(x => x.GetOrAddAsync<IReliableDictionary<long, IModel>>(Settings.Constants
                    .ReliableDictionaryNames.TemplatingEngineReliableDictionaryName))
                .Returns(Task.Run(() => mockReliableDict.Object));

            mockStateManager
                .Setup(x => x.CreateTransaction())
                .Returns(mockTransaction.Object);

            _repo = new ReliableRepositoryAsync<IModel>(mockStateManager.Object);
        }

        [TestCleanup]
        public void TestCleanup()
            => _dictionary.Clear();

        [TestMethod]
        public void Can_Instantiate()
            => Assert.IsNotNull(_repo);

        [TestMethod]
        public void Can_Create_Items()
        {
            var tempResult = _repo.CreateAsync(_mockModel.Object).Result;
            var count = _dictionary.Count;

            Assert.AreEqual(1, count);
        }

        [TestMethod]
        public void Can_Get_Number_Of_Items()
        {
            _dictionary.Add(0, _mockModel.Object);
            _dictionary.Add(1, _mockModel.Object);
            _dictionary.Add(2, _mockModel.Object);

            var tempResult = _repo.GetItemsCount().Result;
            var count = _dictionary.Count;

            Assert.AreEqual(3, count);
        }

        [TestMethod]
        public void Can_Delete_Items()
        {
            _dictionary.Add(0, _mockModel.Object);
            _dictionary.Add(1, _mockModel.Object);
            _dictionary.Add(2, _mockModel.Object);

            var task = _repo.Delete(0);

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

            var task = _repo.Update(2, new CommunicationsTemplateModel()
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