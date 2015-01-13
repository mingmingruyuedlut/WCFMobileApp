using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interactive.Footprints.Stub.ContractInstance
{
    public class Customer
    {
        public string Id { get; set; }
        
        public string Name { get; set; }
        
        public string ContactNumber { get; set; }
        
    }

    public partial class Customers
    {
        private static readonly Customers _instance = new Customers();
        private Customers() { }

        public static Customers Instance
        {
            get { return _instance; }
        }

        public List<Customer> CustomerList
        {
            get { return customers; }
        }

        private List<Customer> customers = new List<Customer>() 
        { 
            new Customer() { Id = "1", Name = "Fabric Group", ContactNumber = "0433 123 123"}, 
            new Customer() { Id = "2", Name = "Pizza Palace", ContactNumber = "0433 123 222"}, 
            new Customer() { Id = "3", Name = "Interactive", ContactNumber = "0433 123 333"}
        };

    }
}
