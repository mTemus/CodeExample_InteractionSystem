namespace CodeExamples.Codebase.InteractionSystem.Interactions.Wrappers.Storage.Extensions
{
    public interface IStorageWithdrawInteractor : INestedInteractor
    {
        public bool CanWithdraw();
        public IStorable Withdraw(bool remove);
    }
}