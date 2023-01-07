namespace Aleph1.DI.Contracts
{
	/// <summary>To register all the internal type with the DI container.</summary>
	public interface IModule
	{
		/// <summary>register all the internal types that are known to this DLL into the DI container</summary>
		/// <param name="registrar">the DI container registrar</param>
		void Initialize(IModuleRegistrar registrar);
	}
}
