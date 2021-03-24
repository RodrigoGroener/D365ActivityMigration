﻿using System;
using Microsoft.Xrm.Sdk;

namespace MigrationHelper
{
    public class AnnotationSetCreatedBy : IPlugin
    {
        #region Secure/Unsecure Configuration Setup
        private string _secureConfig = null;
        private string _unsecureConfig = null;

        public AnnotationSetCreatedBy(string unsecureConfig, string secureConfig)
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
                if (context.InputParameters["Target"] is Entity entity && entity.LogicalName == "annotation" && entity.Contains("notetext"))
                {
                    var notetext = (string) entity["notetext"];
                    if (notetext != null && notetext.StartsWith("{")) //JSON
                    {
                        var annotationDto = DataTransferObject.ParseJson(notetext);
                        tracer.Trace(notetext);
                        entity["createdby"] = new EntityReference("systemuser", annotationDto.createdby);
                        entity["notetext"] = annotationDto.originalfieldvalue;
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
