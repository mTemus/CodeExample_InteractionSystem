namespace CodeExamples.Codebase.InteractionSystem.Interactions.Wrappers.Storage.Extensions
{
    public interface IStorageStoreInteractor : INestedInteractor
    {
        public bool CanStore(IStorable storable);
        public void Store(IStorable storable);
    }
}