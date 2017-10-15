using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.ComponentModel;

namespace Discover.Web.Mvc
{
    public class EnhancedDefaultModelBinder : DefaultModelBinder
    {
        protected override void BindProperty(ControllerContext controllerContext, ModelBindingContext bindingContext, PropertyDescriptor propertyDescriptor)
        {
            var metaData = bindingContext.PropertyMetadata[propertyDescriptor.Name];
            var prefix = string.IsNullOrEmpty(bindingContext.ModelName) ? propertyDescriptor.Name : string.Concat(bindingContext.ModelName, ".", propertyDescriptor.Name);

            // attempt to intercept binding of DateTime properties...
            if ((metaData.DataTypeName == "DateTime" || metaData.DataTypeName == null) && (propertyDescriptor.PropertyType == typeof(DateTime) || propertyDescriptor.PropertyType == typeof(DateTime?)))
            {
                // look for components of the date/time being split across multiple values...
                var dateValue = bindingContext.ValueProvider.GetValue(prefix + ".Date") ?? bindingContext.ValueProvider.GetValue(prefix);
                var hoursValue = bindingContext.ValueProvider.GetValue(prefix + ".Hour");
                var minutesValue = bindingContext.ValueProvider.GetValue(prefix + ".Minute");
                var secondsValue = bindingContext.ValueProvider.GetValue(prefix + ".Second");

                DateTime result;
                if (metaData.IsRequired || (dateValue != null && !string.IsNullOrWhiteSpace(dateValue.AttemptedValue)))
                {
                    if (!DateTime.TryParse(dateValue.AttemptedValue, out result))
                    {
                        bindingContext.ModelState.AddModelError(propertyDescriptor.Name, "Date component was not in a recognised format");
                        return;
                    }

                    int timeComponent = 0;
                    if (hoursValue != null)
                    {
                        if (!int.TryParse(hoursValue.AttemptedValue, out timeComponent))
                        {
                            bindingContext.ModelState.AddModelError(propertyDescriptor.Name, "Hour component was not in a recognised format");
                            return;
                        }
                        else if (timeComponent < 0 || timeComponent > 23)
                        {
                            bindingContext.ModelState.AddModelError(propertyDescriptor.Name, "Hour component was outside the valid range");
                            return;
                        }
                        result = result.AddHours(timeComponent);
                    }

                    if (minutesValue != null)
                    {
                        if (!int.TryParse(minutesValue.AttemptedValue, out timeComponent))
                        {
                            bindingContext.ModelState.AddModelError(propertyDescriptor.Name, "Minute component was not in a recognised format");
                            return;
                        }
                        else if (timeComponent < 0 || timeComponent > 59)
                        {
                            bindingContext.ModelState.AddModelError(propertyDescriptor.Name, "Minute component was outside the valid range");
                            return;
                        }
                        result = result.AddMinutes(timeComponent);
                    }

                    if (secondsValue != null)
                    {
                        if (!int.TryParse(secondsValue.AttemptedValue, out timeComponent))
                        {
                            bindingContext.ModelState.AddModelError(propertyDescriptor.Name, "Second component was not in a recognised format");
                            return;
                        }
                        else if (timeComponent < 0 || timeComponent > 59)
                        {
                            bindingContext.ModelState.AddModelError(propertyDescriptor.Name, "Second component was outside the valid range");
                            return;
                        }
                        result = result.AddSeconds(timeComponent);
                    }

                    this.SetProperty(controllerContext, bindingContext, propertyDescriptor, result);
                    return;
                }
            }

            // fall back to default binding behaviour
            base.BindProperty(controllerContext, bindingContext, propertyDescriptor);
        }

        protected override object CreateModel(ControllerContext controllerContext, ModelBindingContext bindingContext, Type modelType)
        {
            Type type = modelType;
            if (modelType.IsGenericType)
            {
                Type genericTypeDefinition = modelType.GetGenericTypeDefinition();
                if (genericTypeDefinition == typeof(IDictionary<,>))
                {
                    type = typeof(Dictionary<,>).MakeGenericType(modelType.GetGenericArguments());
                }
                else if (((genericTypeDefinition == typeof(IEnumerable<>)) || (genericTypeDefinition == typeof(ICollection<>))) || (genericTypeDefinition == typeof(IList<>)))
                {
                    type = typeof(List<>).MakeGenericType(modelType.GetGenericArguments());
                }
                return Activator.CreateInstance(type);
            }
            else if (modelType.IsAbstract)
            {
                var concreteTypeIdentifier = bindingContext.ValueProvider.GetValue(bindingContext.ModelName + ".ETID");
                if (concreteTypeIdentifier == null) concreteTypeIdentifier = bindingContext.ValueProvider.GetValue(bindingContext.ModelName + "._TypeGUID");
                if (concreteTypeIdentifier == null) concreteTypeIdentifier = bindingContext.ValueProvider.GetValue(bindingContext.ModelName + "._TypeName");
                if (concreteTypeIdentifier == null) throw new Exception("Concrete type for abstract class not specified");

                // Fetch model type using type Guid (or full name) and instantiate & bind
                var allAppDomainTypes = AppDomain.CurrentDomain.GetAssemblies().Where(a => !a.IsDynamic).SelectMany(a => a.GetTypes());
                Type elementType = null; 

                Guid typeGuid;
                if (Guid.TryParse(concreteTypeIdentifier.AttemptedValue, out typeGuid))
                {
                    elementType = allAppDomainTypes.Where(t => t.GUID == typeGuid).FirstOrDefault();
                }
                else
                {
                    elementType = allAppDomainTypes.Where(t => t.FullName.Equals(concreteTypeIdentifier.AttemptedValue, StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault();
                }

                if (elementType == null)
                    throw new Exception(String.Format("Concrete model type {0} not found", concreteTypeIdentifier.AttemptedValue));

                var instance = Activator.CreateInstance(elementType);
                //bindingContext.ModelMetadata = ModelMetadataProviders.Current.GetMetadataForType(() => instance, type);
                bindingContext.ModelMetadata = ModelMetadataProviders.Current.GetMetadataForType(null, elementType);
                bindingContext.ModelMetadata.Model = instance;
                return instance;
            }
            else
            {
                return Activator.CreateInstance(modelType);
            }
        }
    }
}
