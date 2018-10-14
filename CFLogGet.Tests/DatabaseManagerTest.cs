using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CFLogGet.Tests
{
    [TestClass]
    public class DatabaseManagerTest
    {
        [Ignore]
        [TestMethod]
        public void CreateTable()
        {
            var manager = new DatabaseManager("Data Source=.\\sqlexpress;Initial Catalog=cflogget;Integrated Security=True;Connection Timeout=10");
            manager.CreateTable($"test_{DateTime.Now:yyyyMMddHHmmss}");
        }
    }
}
