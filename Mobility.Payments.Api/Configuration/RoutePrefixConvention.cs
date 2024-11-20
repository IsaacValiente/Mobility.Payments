namespace Mobility.Payments.Api.Configuration
{
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.ApplicationModels;
    using System;

    /// <summary>
    /// Adds a custom route prefix to all controllers, with the option to exclude specific namespaces.
    /// </summary>
    public class RoutePrefixConvention : IControllerModelConvention
    {
        /// <summary>
        /// The attribute route model that defines the route prefix.
        /// </summary>
        private readonly AttributeRouteModel attributeRouteModel;

        /// <summary>
        /// Array of namespaces to exclude from the route prefix application.
        /// </summary>
        private readonly string[] excludedNamespace;

        /// <summary>
        /// Initializes a new instance of the <see cref="RoutePrefixConvention"/> class.
        /// </summary>
        /// <param name="prefix">The route prefix to apply to controllers.</param>
        /// <param name="excluded">An optional array of namespaces to exclude from the route prefix application.</param>
        public RoutePrefixConvention(string prefix, params string[] excluded)
        {
            attributeRouteModel = new AttributeRouteModel(new RouteAttribute(prefix));
            excludedNamespace = excluded ?? [];
        }

        /// <summary>
        /// Applies the route prefix convention to a given controller model.
        /// </summary>
        /// <param name="controller">The controller model to which the convention is applied.</param>
        /// <remarks>
        /// - If the controller's namespace matches an excluded namespace, the prefix is not applied.
        /// - Combines the defined route prefix with any existing route attributes on the controller.
        /// </remarks>
        public void Apply(ControllerModel controller)
        {
            if (Array.Exists(excludedNamespace, (n) => controller.DisplayName.Contains(n)))
            {
                return;
            }

            foreach (SelectorModel selector in controller.Selectors)
            {
                selector.AttributeRouteModel = selector.AttributeRouteModel != null ? AttributeRouteModel.CombineAttributeRouteModel(attributeRouteModel, selector.AttributeRouteModel) : attributeRouteModel;
            }
        }
    }
}