using GroceryStoreDataRepo.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GroceryStoreDataRepo.Interfaces
{
    public interface ICustomerDataAccessor
    {
        public int Create(Customer customer);
        public IEnumerable<Customer> GetAll();
        public Customer GetByID(int id);
        public void Update(Customer customer);
    }
}
