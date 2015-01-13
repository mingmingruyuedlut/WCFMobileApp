using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Interactive.Footprints.Stub.ContractInstance;

namespace Interactive.Footprints.Stub
{
    public class FPTestService
    {
        public Customer GetCustomerDetails(string id)
        {
            foreach (Customer c in Customers.Instance.CustomerList)
            {
                if (c.Id.Equals(id))
                {
                    return c;
                }
            }
          //  WebOperationContext.Current.OutgoingResponse.SetStatusAsNotFound();
            return null;
        }
    }
}
