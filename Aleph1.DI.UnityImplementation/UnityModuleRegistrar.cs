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
        public void RegisterType<TFrom, TTo>() where TTo : TFrom
        {
            this._container.RegisterType<TFrom, TTo>();
        }

        /// <summary>
        /// register a singelton type mapping between a concrete type to a Interface
        /// </summary>
        /// <typeparam name="TFrom">concrete type implementation</typeparam>
        /// <typeparam name="TTo">Interface</typeparam>
        public void RegisterTypeAsSingelton<TFrom, TTo>() where TTo : TFrom
        {
            this._container.RegisterType<TFrom, TTo>(new ContainerControlledLifetimeManager());
        }
    }
}
