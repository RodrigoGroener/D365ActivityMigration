﻿using System;
using Microsoft.Xrm.Sdk;

namespace MigrationHelper
{
    public class SetModifiedOn : IPlugin
    {
        #region Secure/Unsecure Configuration Setup
        private string _secureConfig = null;
        private string _unsecureConfig = null;

        public SetModifiedOn(string unsecureConfig, string secureConfig)
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
                    string attributeName = entity.Attributes.GetAttributeNameThatEndsBy(tracer, "_overriddenmodifiedon");

                    if (attributeName != null && entity.Attributes.Contains(attributeName))
                    {
                        tracer.Trace($"{attributeName} has value: {entity[attributeName]}");

                        entity.Attributes.Add("modifiedon", entity[attributeName]);
                        tracer.Trace($"modifiedon overwritten with {attributeName}");
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
