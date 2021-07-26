using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GroceryStoreDataRepo
{
    /// <summary>
    /// Message of this Exception is appropriate to be returned to client
    /// </summary>
    public class ClientException : Exception
    {        
        public ClientException(string message) : base(message)
        {
        }

        public ClientException(string message, Exception inner) : base(message, inner)
        {
        }
    }
}
