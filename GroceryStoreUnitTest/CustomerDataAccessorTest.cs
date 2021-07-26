using GroceryStoreDataRepo;
using GroceryStoreDataRepo.Models;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Xunit;
using DA = GroceryStoreDataRepo.DataAccessors;

namespace GroceryStoreUnitTest
{
    [TestClass]
    public class CustomerDataAccessorTest
    {
        private static Random random = new Random();
        private string _emptyFileContents = null;
        private string _noneemptyFileContents = null;

        public CustomerDataAccessorTest()
        {
            //grab a copy of the starting file db's before running the test so the changes can be reverted after the test is complete
            _emptyFileContents = File.ReadAllText(getDBFilePath(true));
            _noneemptyFileContents = File.ReadAllText(getDBFilePath(false));
        }

        [TestCleanup()]
        public void Cleanup()
        {
            File.WriteAllText(getDBFilePath(true), _emptyFileContents);
            File.WriteAllText(getDBFilePath(false), _noneemptyFileContents);
        }

        [TestMethod]
        [DataRow(true, 0, 0)] //empty file, not results
        [DataRow(false, 1, int.MaxValue)] //non-empty file, results
        public void GetAll_ReturnAllCustomers_ExpectedNumberReturned(bool emptySource, int minResults, int maxResults)
        {
            //Arrange
            DA.CustomerDataAccessor customerDataAccessor = getCustomerDataAccessor(emptySource);

            //Act
            IEnumerable<Customer> all = customerDataAccessor.GetAll();

            //Assert
            Assert.IsTrue(minResults <= all.Count() && all.Count() <= maxResults);
        }

        [TestMethod]
        [DataRow(true)] //empty file
        [DataRow(false)] //non-empty file
        public void Create_AddValidCustomer_CustomerWithExpectedIDCreated(bool emptySource)
        {
            //Arrange
            DA.CustomerDataAccessor customerDataAccessor = getCustomerDataAccessor(emptySource);
            IEnumerable<Customer> allCustomers = customerDataAccessor.GetAll();

            int maxID = (allCustomers.Any()) ? allCustomers.Max(c => c.ID) : 0;
            Customer customer = new Customer() { Name = "test" };

            //Act
            int newID = customerDataAccessor.Create(customer);

            //Assert
            Assert.IsTrue(newID == maxID + 1);
        }

        [TestMethod]
        public void GetByID_GetCustomerWithExistingID_CustomerReturned()
        {
            //Arrange
            DA.CustomerDataAccessor customerDataAccessor = getCustomerDataAccessor(false);
            IEnumerable<Customer> allCustomers = customerDataAccessor.GetAll();
            int id = allCustomers.First().ID;

            //Act
            Customer customer = customerDataAccessor.GetByID(id);

            //Assert
            Assert.IsTrue(customer?.ID == id);
        }

        [TestMethod]
        [DataRow(true)] //empty file
        [DataRow(false)] //non-empty file
        public void GetByID_GetCustomerWithNonExistingID_NullReturned(bool emptySource)
        {
            //Arrange
            DA.CustomerDataAccessor customerDataAccessor = getCustomerDataAccessor(emptySource);            

            //Act
            Customer customer = customerDataAccessor.GetByID(-1);

            //Assert
            Assert.IsTrue(customer == null);
        }

        [TestMethod]
        public void Update_UpdateExistingCustomer_CustomerUpdated()
        {
            //Arrange
            DA.CustomerDataAccessor customerDataAccessor = getCustomerDataAccessor(false);
            Customer customer = customerDataAccessor.GetAll().First();
            string newName = randomString(10);
            customer.Name = newName;
            int id = customer.ID;

            //Act
            customerDataAccessor.Update(customer);

            //Assert
            Assert.IsTrue(customerDataAccessor.GetByID(id)?.Name == newName);
        }

        private DA.CustomerDataAccessor getCustomerDataAccessor(bool useEmptySource = false)
        {
            IOptions<DataRepoSettings> options = Options.Create(new DataRepoSettings());
            options.Value.JsonFilePath = getDBFilePath(useEmptySource);

            return new DA.CustomerDataAccessor(options);
        }

        private string getDBFilePath(bool emptyFile)
        {
            FileInfo info = new FileInfo(Assembly.GetExecutingAssembly().Location);
            string path = info.DirectoryName;
            if (emptyFile)
                return $@"{path}\empty.database.json";
            else
                return $@"{path}\non.empty.database.json";
        }

        private string randomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }
    }
}
