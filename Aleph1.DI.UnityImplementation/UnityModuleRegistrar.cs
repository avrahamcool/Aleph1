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
			_container = container;
		}

		/// <summary>register a transient type mapping between a concrete type to an Interface</summary>
		/// <typeparam name="TFrom">Interface</typeparam>
		/// <typeparam name="TTo">concrete type implementation</typeparam>
		/// <param name="name">name to use for Named Registration, use null for default</param>
		public void RegisterType<TFrom, TTo>(string name = null) where TTo : TFrom
		{
			if (name == null)
			{
				_container.RegisterType<TFrom, TTo>();
			}
			else
			{
				_container.RegisterType<TFrom, TTo>(name);
			}
		}

		/// <summary>register a singleton type mapping between a concrete type to a Interface</summary>
		/// <typeparam name="TFrom">Interface</typeparam>
		/// <typeparam name="TTo">concrete type implementation</typeparam>
		/// <param name="name">name to use for Named Registration, use null for default</param>
		public void RegisterTypeAsSingelton<TFrom, TTo>(string name = null) where TTo : TFrom
		{
			if (name == null)
			{
				_container.RegisterType<TFrom, TTo>(new ContainerControlledLifetimeManager());
			}
			else
			{
				_container.RegisterType<TFrom, TTo>(name, new ContainerControlledLifetimeManager());
			}
		}

		/// <summary>register a singleton type mapping between a concrete instance to a Interface</summary>
		/// <typeparam name="TFrom">Interface</typeparam>
		/// <typeparam name="TTo">concrete instance implementation</typeparam>
		/// <param name="instance">concrete instance implementation</param>
		/// <param name="name">name to use for Named Registration, use null for default</param>
		public void RegisterTypeAsSingelton<TFrom, TTo>(TTo instance, string name = null) where TTo : TFrom
		{
			if (name == null)
			{
				_container.RegisterInstance<TFrom>(instance, new ContainerControlledLifetimeManager());
			}
			else
			{
				_container.RegisterInstance<TFrom>(name, instance, new ContainerControlledLifetimeManager());
			}
		}
	}
}
