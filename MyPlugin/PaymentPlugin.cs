using Entities;
using Microsoft.Crm.Services.Utility;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Cryptography.Xml;
using System.Security.Policy;
using System.ServiceModel;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace MyPlugin
{
    public class PaymentPlugin : PluginBase
    {
        public PaymentPlugin(string unsecureConfiguration, string secureConfiguration)
            : base(typeof(PaymentPlugin))
        {
        }

        protected override void ExecuteCdsPlugin(ILocalPluginContext localPluginContext)
        {
            IPluginExecutionContext _context = localPluginContext.PluginExecutionContext;

            Entity payment = (Entity)_context.InputParameters["Target"];
            if (localPluginContext.Target.Contains("crfe2_contact") || 
                localPluginContext.Target.Contains("crfe2_date") || 
                localPluginContext.Target.Contains("crfe2_sum"))
            {
                var contactRef = localPluginContext.Latest.GetAttributeValue<EntityReference>("crfe2_contact");
                Entity contact = localPluginContext.CurrentUserService.Retrieve(contactRef.LogicalName, contactRef.Id, new ColumnSet("firstname"));
                
                var date = localPluginContext.Latest.GetAttributeValue<DateTime?>("crfe2_date");
                var sum = (localPluginContext.Latest.GetAttributeValue<Money>("crfe2_sum") ?? new Money(0)).Value;

                var name = contact.Attributes["firstname"] + "-" + date?.ToString("dd.mm.yyyy") + "-" +
                    sum.ToString("N", CultureInfo.CreateSpecificCulture("fr-CA"));

                localPluginContext.Set("crfe2_name", name);
            }
        }
    }



}
