using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interactive.DBManager.Entity
{
    public class UserEntity
    {
        public int Id { get; set; }
        public string Email { get; set; }
        public int CompanyId { get; set; }
        public string BillingId { get; set; }
    }
}
