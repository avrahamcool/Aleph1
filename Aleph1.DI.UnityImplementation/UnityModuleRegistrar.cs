using Aleph1.DI.Contracts;
using Unity;
using Unity.Lifetime;

namespace Aleph1.DI.UnityImplementation
{
    /// <summary>concrete implementation of the <see cref="IModuleRegistrar" /> using Unity</summary>
    /// <seealso cref="Aleph1.DI.Contracts.IModuleRegistrar" />
    public class UnityModuleRegistrar : IModuleRegistrar
    {
        private readonly IUnityContainer _container;
        /// <summary>Initializes a new instance of the <see cref="UnityModuleRegistrar"/> class.</summary>
        /// <param name="container">The Unity container.</param>
        public UnityModuleRegistrar(IUnityContainer container)
        {
            this._container = container;
        }

        /// <summary>
        /// register a transient type mapping between a concrete type to an Interface
        /// </summary>
        /// <typeparam name="TFrom">concrete type implementation</typeparam>
        /// <typeparam name="TTo">Interface</typeparam>
        /// <param name="name">name to use for Named Registration, use null for default</param>
        public void RegisterType<TFrom, TTo>(string name = null) where TTo : TFrom
        {
            if(name == null)
                this._container.RegisterType<TFrom, TTo>();
            else
                this._container.RegisterType<TFrom, TTo>(name);
        }

        /// <summary>
        /// register a singelton type mapping between a concrete type to a Interface
        /// </summary>
        /// <typeparam name="TFrom">concrete type implementation</typeparam>
        /// <typeparam name="TTo">Interface</typeparam>
        /// <param name="name">name to use for Named Registration, use null for default</param>
        public void RegisterTypeAsSingelton<TFrom, TTo>(string name = null) where TTo : TFrom
        {
            if (name == null)
                this._container.RegisterType<TFrom, TTo>(new ContainerControlledLifetimeManager());
            else
                this._container.RegisterType<TFrom, TTo>(name, new ContainerControlledLifetimeManager());
        }
    }
}
