namespace Aleph1.DI.Contracts
{
    /// <summary>Allows objects implementing IModule to register types to the DI container.</summary>
    public interface IModuleRegistrar
    {
        /// <summary>register a transient type mapping between a concrete type to an Interface</summary>
        /// <typeparam name="TFrom">concrete type implementation</typeparam>
        /// <typeparam name="TTo">Interface</typeparam>
        void RegisterType<TFrom, TTo>() where TTo : TFrom;

        /// <summary>register a singelton type mapping between a concrete type to a Interface</summary>
        /// <typeparam name="TFrom">concrete type implementation</typeparam>
        /// <typeparam name="TTo">Interface</typeparam>
        void RegisterTypeAsSingelton<TFrom, TTo>() where TTo : TFrom;
    }
}
