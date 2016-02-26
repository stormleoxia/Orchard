using System.Collections.Generic;
using System.Linq;
using Orchard.Environment.Descriptor.Models;
using Orchard.Environment.Extensions.Models;

namespace Orchard.Environment.Extensions {
    public interface IExtensionManager {
        IEnumerable<ExtensionDescriptor> AvailableExtensions();
        IEnumerable<FeatureDescriptor> AvailableFeatures();

        ExtensionDescriptor GetExtension(string id);

        IEnumerable<Feature> LoadFeatures(IEnumerable<FeatureDescriptor> featureDescriptors);
    }

    public static class ExtensionManagerExtensions {
		public static IEnumerable<FeatureDescriptor> EnabledFeatures(this IExtensionManager extensionManager, ShellDescriptor descriptor)
		{
			var availableFeatures = extensionManager.AvailableFeatures().ToList();
			return availableFeatures.Where(fd =>
			{
				var result = descriptor.Features.Any(sf => sf.Name == fd.Id);
				return result;
			});
        }
    }
}
