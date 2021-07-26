using GroceryStoreAPI.Controllers;
using GroceryStoreDataRepo;
using GroceryStoreDataRepo.Interfaces;
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
    public class CustomerControllerTest
    {
        private static Random random = new Random();

        public CustomerControllerTest()
        {
            ////grab a copy of the starting file db's before running the test so the changes can be reverted after the test is complete
            //_emptyFileContents = File.ReadAllText(getDBFilePath(true));
            //_noneemptyFileContents = File.ReadAllText(getDBFilePath(false));
        }

        [TestCleanup()]
        public void Cleanup()
        {
        }

        [TestMethod]
        public void Get_ReturnAllCustomers_ExpectedNumberReturned()
        {
            //Arrange
            ICustomerDataAccessor mockRepo = getCustomerDataAccessor();
            CustomerController customerController = new CustomerController(mockRepo);

            //Act
            IEnumerable<Customer> allCustomers = customerController.Get();

            //Assert
            Assert.IsTrue(allCustomers.Count() == mockRepo.GetAll().Count());
        }

        [TestMethod]
        public void Get_ReturnCustomerByID_ReturnsCustomer()
        {
            //Arrange
            MockCustomerDataAccessor mockRepo = getCustomerDataAccessor();
            CustomerController customerController = new CustomerController(mockRepo);

            //Act
            Customer customer = customerController.Get(mockRepo.SampleCustomerID);

            //Assert
            Assert.IsTrue(customer != null && customer.ID == mockRepo.SampleCustomerID);
        }

        [TestMethod]
        public void Post_CreateNewCustomer_NewIDIsReturned()
        {
            string name = "new customer";
            //Arrange
            MockCustomerDataAccessor mockRepo = getCustomerDataAccessor();
            CustomerController customerController = new CustomerController(mockRepo);

            //Act
            int id = customerController.Post(new Customer() { Name = name });

            //Assert
            Assert.IsTrue(mockRepo.GetAll().Any(c => c.ID == id && c.Name.Equals(name)));
        }

        [TestMethod]
        public void Put_UpdateCustomer_CustomerIsUpdated()
        {
            //Arrange
            MockCustomerDataAccessor mockRepo = getCustomerDataAccessor();
            Customer customer = mockRepo.GetAll().First();
            customer.Name = randomString(10);
            CustomerController customerController = new CustomerController(mockRepo);

            //Act
            customerController.Put(customer);

            //Assert
            Assert.IsTrue(mockRepo.GetAll().Any(c => c.ID == customer.ID && c.Name.Equals(customer.Name)));
        }

        private MockCustomerDataAccessor getCustomerDataAccessor()
        {
            return new MockCustomerDataAccessor();
        }

        private string randomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        private class MockCustomerDataAccessor : ICustomerDataAccessor
        {
            private List<Customer> _customers = new List<Customer>()
                                                {
                                                    new Customer()
                                                    {
                                                        ID = 1,
                                                        Name = "One"
                                                    },
                                                    new Customer()
                                                    {
                                                        ID = 2,
                                                        Name = "Two"
                                                    }
                                                };

            /// <summary>
            /// returns the id of the first sample customer
            /// </summary>
            public int SampleCustomerID 
            {
                get
                {
                    return GetAll().First().ID;
                }
            }

            public MockCustomerDataAccessor()
            {
            }

            public int Create(Customer customer)
            {
                customer.ID = _customers.Max(c => c.ID) + 1;
                _customers.Add(customer);
                return customer.ID;
            }

            public IEnumerable<Customer> GetAll()
            {
                return _customers;
            }

            public Customer GetByID(int id)
            {
                return _customers.FirstOrDefault(c => c.ID == id);
            }

            public void Update(Customer customer)
            {
                int index = _customers.FindIndex(c => c.ID == customer.ID);
                if(index >= 0)
                    _customers[index] = customer;
            }
        }
    }
}
