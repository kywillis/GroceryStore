using GroceryStoreDataRepo.Models;
using GroceryStoreDataRepo.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace GroceryStoreAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomerController : ControllerBase
    {
        ICustomerDataAccessor _customerDataAccessor;
        public CustomerController(ICustomerDataAccessor customerDataAccessor)
        {
            _customerDataAccessor = customerDataAccessor;
        }
        // GET: api/<CustomerController>
        [HttpGet]
        public IEnumerable<Customer> Get()
        {
            return _customerDataAccessor.GetAll();
        }

        // GET api/<CustomerController>/5
        [HttpGet("{id}")]
        public Customer Get(int id)
        {            
            return _customerDataAccessor.GetByID(id);
        }

        // POST api/<CustomerController>
        [HttpPost]
        public int Post(Customer customer)
        {
            return _customerDataAccessor.Create(customer);
        }

        // PUT api/<CustomerController>/5
        [HttpPut]
        public void Put(Customer customer)
        {
            _customerDataAccessor.Update(customer);
        }
    }
}
