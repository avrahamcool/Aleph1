namespace Aleph1.DI.Contracts
{
	/// <summary>Allows objects implementing IModule to register types to the DI container.</summary>
	public interface IModuleRegistrar
	{
		/// <summary>register a transient type mapping between a concrete type to an Interface</summary>
		/// <typeparam name="TFrom">Interface</typeparam>
		/// <typeparam name="TTo">concrete type implementation</typeparam>
		/// <param name="name">name to use for Named Registration, use null for default</param>
		void RegisterType<TFrom, TTo>(string name = null) where TTo : TFrom;

		/// <summary>register a singleton type mapping between a concrete type to a Interface</summary>
		/// <typeparam name="TFrom">Interface</typeparam>
		/// <typeparam name="TTo">concrete type implementation</typeparam>
		/// <param name="name">name to use for Named Registration, use null for default</param>
		void RegisterTypeAsSingelton<TFrom, TTo>(string name = null) where TTo : TFrom;

		/// <summary>register a singleton type mapping between a concrete instance to a Interface</summary>
		/// <typeparam name="TFrom">Interface</typeparam>
		/// <typeparam name="TTo">concrete instance implementation</typeparam>
		/// <param name="instance">concrete instance implementation</param>
		/// <param name="name">name to use for Named Registration, use null for default</param>
		void RegisterTypeAsSingelton<TFrom, TTo>(TTo instance, string name = null) where TTo : TFrom;
	}
}
