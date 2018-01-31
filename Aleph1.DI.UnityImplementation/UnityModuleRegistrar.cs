using Aleph1.DI.Contracts;
using Unity;
using Unity.Lifetime;

namespace Aleph1.DI.UnityImplementation
{
    internal class UnityModuleRegistrar : IModuleRegistrar
    {
        private readonly IUnityContainer _container;
        internal UnityModuleRegistrar(IUnityContainer container)
        {
            this._container = container;
        }

        public void RegisterType<TFrom, TTo>() where TTo : TFrom
        {
            this._container.RegisterType<TFrom, TTo>();
        }

        public void RegisterTypeAsSingelton<TFrom, TTo>() where TTo : TFrom
        {
            this._container.RegisterType<TFrom, TTo>(new ContainerControlledLifetimeManager());
        }
    }
}
