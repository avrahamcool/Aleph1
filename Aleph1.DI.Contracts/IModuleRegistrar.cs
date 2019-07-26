namespace Aleph1.DI.Contracts
{
    /// <summary>Allows objects implementing IModule to register types to the DI container.</summary>
    public interface IModuleRegistrar
    {
        /// <summary>register a transient type mapping between a concrete type to an Interface</summary>
        /// <typeparam name="TFrom">concrete type implementation</typeparam>
        /// <typeparam name="TTo">Interface</typeparam>
        /// <param name="name">name to use for Named Registration, use null for default</param>
        void RegisterType<TFrom, TTo>(string name = null) where TTo : TFrom;


        /// <summary>register a singleton type mapping between a concrete type to a Interface</summary>
        /// <typeparam name="TFrom">concrete type implementation</typeparam>
        /// <typeparam name="TTo">Interface</typeparam>
        /// <param name="name">name to use for Named Registration, use null for default</param>
        void RegisterTypeAsSingelton<TFrom, TTo>(string name = null) where TTo : TFrom;
    }
}
