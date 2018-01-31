using Aleph1.DI.Contracts;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition.Hosting;
using System.IO;
using System.Linq;
using System.Reflection;
using Unity;

namespace Aleph1.DI.UnityImplementation
{
    /// <summary>Handles loading modules into the Unity DI container</summary>
    public static class UnityModuleLoader
    {
        /// <summary>uses MEF to load all the IModule implementations to the UnityContainer</summary>
        /// <param name="container">Unity container</param>
        /// <param name="parentPath">path to the root directory of the project</param>
        /// <param name="assemblies">retlative path to the dll to load.
        /// Modules Should be Named: Peten.ModuleName.dll</param>
        public static void LoadContainerFromAssemblies(IUnityContainer container, string parentPath, string[] assemblies)
        {
            Uri baseUri = new Uri(parentPath);
            List<string> assembliesPath = assemblies.Select(ass => new Uri(baseUri, ass).LocalPath).ToList();
            string badPath = assembliesPath.FirstOrDefault(ap => !File.Exists(ap));
            if (badPath != null)
                throw new Exception($"Could not load {badPath}. Current EXE Dir: {parentPath}. Current BaseUri {baseUri}");

            try
            {
                //Creating a single catalog from all the DLL's
                using (AggregateCatalog catalog = new AggregateCatalog(assembliesPath.Select(ap => new AssemblyCatalog(ap))))
                using (CompositionContainer compositionContainer = new CompositionContainer(catalog))
                {
                    UnityModuleRegistrar registrar = new UnityModuleRegistrar(container);

                    //Get all the modules and register them
                    foreach (IModule module in compositionContainer.GetExports<IModule>().Select(e => e.Value))
                        module.Initialize(registrar);
                }
            }
            catch (ReflectionTypeLoadException rtle)
            {
                throw new Exception(String.Join(Environment.NewLine, rtle.LoaderExceptions.Select(e => e.ToString())));
            }
        }
    }
}
