using System;
using System.Collections.Generic;
using System.ComponentModel.Composition.Hosting;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Aleph1.DI.Contracts
{
    /// <summary>Handles loading <see cref="IModule"/> from DLL's into the <see cref="IModuleRegistrar"/></summary>
    public static class ModuleLoader
    {
        /// <summary>uses MEF to load all the IModule implementations to the Registrar</summary>
        /// <param name="registrar">IModuleRegistrar</param>
        /// <param name="rootPath">path to the root directory of the project</param>
        /// <param name="assemblies">relative path to the DLL to load.</param>
        public static void LoadModulesFromAssemblies(IModuleRegistrar registrar, string rootPath, string[] assemblies)
        {
            Uri baseUri = new Uri(rootPath);
            List<string> assembliesPath = assemblies.Select(ass => new Uri(baseUri, ass).LocalPath).ToList();
            string badPath = assembliesPath.FirstOrDefault(ap => !File.Exists(ap));
            if (badPath != null)
                throw new Exception($"Could not load {badPath}. Current EXE Dir: {rootPath}. Current BaseUri {baseUri}");

            try
            {
                //Creating a single catalog from all the DLL's
                using (AggregateCatalog catalog = new AggregateCatalog(assembliesPath.Select(ap => new AssemblyCatalog(ap))))
                using (CompositionContainer compositionContainer = new CompositionContainer(catalog))
                {
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
