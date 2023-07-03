using Microsoft.Xrm.Sdk.Query;
using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyPlugin
{
    public class TownPrePlugin : PluginBase
    {
        public TownPrePlugin(string unsecureConfiguration, string secureConfiguration)
            : base(typeof(PaymentPlugin))
        {
        }

        protected override void ExecuteCdsPlugin(ILocalPluginContext localPluginContext)
        {
            IPluginExecutionContext _context = localPluginContext.PluginExecutionContext;
            if (localPluginContext.Target.Contains("crfe2_region"))
            {
                Entity town = (Entity)_context.InputParameters["Target"];
                var people = localPluginContext.Current.GetAttributeValue<int>("crfe2_people");
                var regionRef = localPluginContext.Current.GetAttributeValue<EntityReference>("crfe2_region");
                Entity region = localPluginContext.CurrentUserService.Retrieve(regionRef.LogicalName, regionRef.Id, new ColumnSet("crfe2_people", "crfe2_name"));
                if (region.Attributes.Contains("crfe2_people"))
                    region.Attributes["crfe2_people"] = (int)region.Attributes["crfe2_people"] - people;

                localPluginContext.CurrentUserService.Update(region);
            }
           



        }
    }
}
