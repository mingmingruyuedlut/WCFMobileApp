using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Interactive.Constant;

namespace Interactive.DBManager.Entity
{
    public class IncidentEntity
    {
        public int Id { get; set; }
        public int FPIncidentId { get; set; }
        public int UserId { get; set; }
        public string SubmitterEmail { get; set; }
        public IncidentModelStatus Status { get; set; }
        public IncidentType Type { get; set; }
    }
}
