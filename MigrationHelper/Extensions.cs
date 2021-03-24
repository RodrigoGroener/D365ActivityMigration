﻿using System;
using System.Linq;
using Microsoft.Xrm.Sdk;

namespace MigrationHelper
{
    public static class Extensions
    {
        /// <summary>
        /// Get the first attribute that ends with the <param name="value"></param> passed in.
        /// </summary>
        /// <returns>the logical name of the attribute or null</returns>
        public static string GetAttributeNameThatEndsBy(this AttributeCollection attributeCollection, ITracingService tracer, string value)
        {
            string attributeName = attributeCollection.Keys.FirstOrDefault(x => x.EndsWith(value, StringComparison.InvariantCultureIgnoreCase));
            string message = attributeName == null
                ? $"no attribute found that ends with \"{value}\""
                : $"attribute found: \"{attributeName}\"";
            tracer.Trace(message);
            return attributeName;
        }
    }
}