using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using GroceryStoreDataRepo.Interfaces;
using GroceryStoreDataRepo.Models;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace GroceryStoreDataRepo.DataAccessors
{
    public class CustomerDataAccessor : ICustomerDataAccessor
    {
        private readonly DataRepoSettings _dataRepoSettings;
        private static object _fileLock = new object();

        public CustomerDataAccessor(IOptions<DataRepoSettings> settings)
        {
            _dataRepoSettings = settings.Value;

            if (String.IsNullOrEmpty(_dataRepoSettings.JsonFilePath))
                throw new Exception("json file path is null or empty");

            lock(_fileLock)//in case this object gets created by multiple threads at about the same time
            {
                if (!File.Exists(_dataRepoSettings.JsonFilePath))
                    File.CreateText(_dataRepoSettings.JsonFilePath).Dispose();
            }
        }

        public int Create(Customer customer)
        {
            if(customer.ID > 0)
                throw new ClientException($"new customer id must be less than 1");

            List<Customer> allCustomers = getAllCustomers().ToList();
            customer.ID = (allCustomers.Any()) ? allCustomers.Max(c => c.ID) + 1 : 1;
            allCustomers.Add(customer);
            saveCustomers(allCustomers);
            return customer.ID;
        }

        public IEnumerable<Customer> GetAll()
        {
            return getAllCustomers();
        }

        public Customer GetByID(int id)
        {
            IEnumerable<Customer> allCustomers = getAllCustomers();
            return allCustomers.FirstOrDefault(c => c.ID == id);
        }

        public void Update(Customer customer)
        {
            List<Customer> allCustomers = getAllCustomers().ToList();
            int index = allCustomers.FindIndex(c => c.ID == customer.ID);
            if (index < 0)
                throw new ClientException($"customer with id {customer.ID} not found");

            allCustomers[index] = customer;
            saveCustomers(allCustomers);
        }

        private IEnumerable<Customer> getAllCustomers()
        {
            string rawData = null;
            lock (_fileLock) //prevent two threads from trying to access the file at the same time
            {
                rawData = File.ReadAllText(_dataRepoSettings.JsonFilePath);
            }

            return (Newtonsoft.Json.JsonConvert.DeserializeObject<DataRepo>(rawData)).Customers;
        }

        private void saveCustomers(IEnumerable<Customer> customers)
        {
            DataRepo dataRepo = new DataRepo() { Customers = customers };
            
            DefaultContractResolver contractResolver = new DefaultContractResolver { NamingStrategy = new CamelCaseNamingStrategy() };
            JsonSerializerSettings serializerSettings = new JsonSerializerSettings { ContractResolver = contractResolver, Formatting = Formatting.Indented };

            string rawData = Newtonsoft.Json.JsonConvert.SerializeObject(dataRepo, serializerSettings);
            lock (_fileLock) //prevent two threads from trying to access the file at the same time
            {
                File.WriteAllText(_dataRepoSettings.JsonFilePath, rawData);
            }
        }

        private class DataRepo
        {
            public IEnumerable<Customer> Customers { get; set; }
        }
    }
}
