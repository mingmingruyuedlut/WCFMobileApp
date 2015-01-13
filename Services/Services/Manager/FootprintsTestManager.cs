using Interactive.Services.Contract;
using System;
using System.Collections.Generic;

using Interactive.Footprints.Stub;
using Interactive.Footprints.Model;
using System.Xml;


namespace Services.Manager
{
    public class FootprintsTestManager
    {
        FPTestService fpService = new FPTestService();
     
        public IncidentModel CreateIncident(RegisterUser user)
        {
            throw new NotImplementedException();
        }



        public CheckUser CheckUser(string email)
        {
            throw new NotImplementedException();
        }
    }
}