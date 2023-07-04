using Microsoft.Xrm.Sdk.Query;
using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyPlugin
{
    public class TownPlugin : PluginBase
    {
        public TownPlugin(string unsecureConfiguration, string secureConfiguration)
            : base(typeof(PaymentPlugin))
        {
        }

        protected override void ExecuteCdsPlugin(ILocalPluginContext localPluginContext)
        {
            IPluginExecutionContext _context = localPluginContext.PluginExecutionContext;
            Entity town = (Entity)_context.InputParameters["Target"];
            var regionRef = localPluginContext.Current.GetAttributeValue<EntityReference>("crfe2_region");
            Entity region = localPluginContext.CurrentUserService.Retrieve(regionRef.LogicalName, regionRef.Id, new ColumnSet("crfe2_people"));

            var people = localPluginContext.Latest.GetAttributeValue<int?>("crfe2_people");
            var sum = people ?? 0;
            if (region.Attributes.Contains("crfe2_people") && (int)region.Attributes["crfe2_people"] >= 0)
            {
                sum += (int)region.Attributes["crfe2_people"];
                region.Attributes["crfe2_people"] = sum;
            }
            else
            {
                region.Attributes.Add("crfe2_people", sum);
            }
            localPluginContext.CurrentUserService.Update(region);
            if (localPluginContext.Target.Contains("crfe2_region"))
            {
                Entity image = new Entity();
                if (_context.PreEntityImages.ContainsKey("PreImage"))
                {
                    image = (Entity)_context.PreEntityImages["PreImage"];
                    people = image.GetAttributeValue<int>("crfe2_people");
                    regionRef = image.GetAttributeValue<EntityReference>("crfe2_region");
                    region = localPluginContext.CurrentUserService.Retrieve(regionRef.LogicalName, regionRef.Id, new ColumnSet("crfe2_people", "crfe2_name"));
                    
                    if (region.Attributes.Contains("crfe2_people"))
                    {
                        region.Attributes["crfe2_people"] = (int)region.Attributes["crfe2_people"] - people;
                    }
                    localPluginContext.CurrentUserService.Update(region);
                }
            }


        }
    }
}
