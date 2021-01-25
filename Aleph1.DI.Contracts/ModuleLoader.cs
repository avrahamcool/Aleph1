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
        /// <param name="modulesDir">relative path to the modules directory</param>
        /// <param name="assemblies">names the DLLs to load</param>
        public static void LoadModulesFromAssemblies(IModuleRegistrar registrar, string rootPath, string modulesDir, string[] assemblies)
        {
            Uri baseUri = new Uri(rootPath);

            AppDomain.CurrentDomain.AssemblyResolve += (object sender, ResolveEventArgs args) =>
            {
                string assemblyPartialName = args.Name.Substring(0, args.Name.IndexOf(',')) + ".dll";
                string assemblyPartialPath = Path.Combine(modulesDir, assemblyPartialName);
                string assemblyFullPath = new Uri(baseUri, assemblyPartialPath).LocalPath;

                return Assembly.LoadFile(assemblyFullPath);
            };

            List<string> assembliesPath = assemblies
                .Select(assName => Path.Combine(modulesDir, assName))
                .Select(assPath => new Uri(baseUri, assPath).LocalPath).ToList();
            string badPath = assembliesPath.FirstOrDefault(ap => !File.Exists(ap));
            if (badPath != null)
            {
                throw new Exception($"Could not load {badPath}. Current EXE Dir: {rootPath}. Current BaseUri {baseUri}");
            }

            try
            {
                //Creating a single catalog from all the DLL's
                using (AggregateCatalog catalog = new AggregateCatalog(assembliesPath.Select(ap => new AssemblyCatalog(ap))))
                using (CompositionContainer compositionContainer = new CompositionContainer(catalog))
                {
                    //Get all the modules and register them
                    foreach (IModule module in compositionContainer.GetExports<IModule>().Select(e => e.Value))
                    {
                        module.Initialize(registrar);
                    }
                }
            }
            catch (ReflectionTypeLoadException rtle)
            {
                throw new Exception(string.Join(Environment.NewLine, rtle.LoaderExceptions.Select(e => e.ToString())));
            }
        }
    }
}
