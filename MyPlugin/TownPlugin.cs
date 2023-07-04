using Microsoft.Xrm.Sdk.Query;
using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;

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
            var regionRef = localPluginContext.Current.GetAttributeValue<EntityReference>("crfe2_region");
            Entity region = localPluginContext.CurrentUserService.Retrieve(regionRef.LogicalName, regionRef.Id, new ColumnSet("crfe2_people"));

            region.Attributes["crfe2_people"] = GetSumOfTown(region, localPluginContext.CurrentUserService);

            localPluginContext.CurrentUserService.Update(region);


            if (localPluginContext.Target.Contains("crfe2_region"))
            {
                if (_context.PreEntityImages.ContainsKey("PreImage"))
                {
                    Entity image = (Entity)_context.PreEntityImages["PreImage"];
                    regionRef = image.GetAttributeValue<EntityReference>("crfe2_region");
                    region = localPluginContext.CurrentUserService.Retrieve(regionRef.LogicalName, regionRef.Id, new ColumnSet("crfe2_people", "crfe2_name"));

                    region.Attributes["crfe2_people"] = GetSumOfTown(region, localPluginContext.CurrentUserService);
                    localPluginContext.CurrentUserService.Update(region);
                }
            }

            
        }

        private int GetSumOfTown(Entity region, IOrganizationService _service)
        {
            int sum = 0;

            QueryExpression query = new QueryExpression()
            {
                EntityName = "crfe2_town",
                ColumnSet= new ColumnSet("crfe2_people"),
                Criteria = new FilterExpression()
                {
                    FilterOperator = LogicalOperator.And,
                    Conditions = { new ConditionExpression("crfe2_region", ConditionOperator.Equal, region.Id) }
                }
            };

            EntityCollection entityCollection = _service.RetrieveMultiple(query);
           
            foreach (var i in entityCollection.Entities)
            {
                if (region.Attributes.Contains("crfe2_people") && (int)region.Attributes["crfe2_people"] >= 0)
                {
                    sum += (int)i.Attributes["crfe2_people"];
                }
            }
            return sum;
        }

    }
}
