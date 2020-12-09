using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Secured_Docusign_API
{
    public class settings
    {
        public class OktaSettings
        {
            public string Instance { get; set; }
            public string TenantId { get; set; }
            public string Domain { get; set; }
            public string ClientId { get; set; }
        }
    }
}