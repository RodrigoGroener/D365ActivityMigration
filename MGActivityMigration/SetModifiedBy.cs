using Microsoft.Xrm.Sdk;

using System;

namespace DeltaN.BusinessSolutions.ActivityMigration
{
    public class SetModifiedBy : IPlugin
    {
        #region Secure/Unsecure Configuration Setup
        private string _secureConfig = null;
        private string _unsecureConfig = null;

        public SetModifiedBy(string unsecureConfig, string secureConfig)
        {
            _secureConfig = secureConfig;
            _unsecureConfig = unsecureConfig;
        }
        #endregion
        public void Execute(IServiceProvider serviceProvider)
        {
            ITracingService tracer = (ITracingService)serviceProvider.GetService(typeof(ITracingService));
            IPluginExecutionContext context = (IPluginExecutionContext)serviceProvider.GetService(typeof(IPluginExecutionContext));
            IOrganizationServiceFactory factory = (IOrganizationServiceFactory)serviceProvider.GetService(typeof(IOrganizationServiceFactory));
            IOrganizationService service = factory.CreateOrganizationService(context.UserId);


            try
            {
                if (context.InputParameters["Target"] is Entity entity && entity.LogicalName != "annotation")
                {
                    string attributeName = entity.Attributes.GetAttributeNameThatEndsBy(tracer, "_overriddenmodifiedby");

                    if (attributeName != null && entity.Contains(attributeName))
                    {
                        tracer.Trace($"{attributeName} has value: {(entity[attributeName] as EntityReference)?.Name} | {(entity[attributeName] as EntityReference)?.Id}");

                        entity["modifiedby"] = entity[attributeName];
                        tracer.Trace($"modifiedby overwritten with {attributeName}");
                    }
                }
            }
            catch (Exception exception)
            {
                throw new InvalidPluginExecutionException(exception.Message, exception);
            }
        }
    }
}
