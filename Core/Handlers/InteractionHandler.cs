namespace CodeExamples.Codebase.InteractionSystem.Core.Handlers
{
    public abstract class InteractionHandler<T> where T : IInteraction
    {
        protected readonly IInteractor Interactor;

        public abstract void HandleAvailable(T interaction);
        public abstract void HandleUnavailable(T interaction);
        public abstract void HandleRemove(T interaction);

        protected InteractionHandler(IInteractor interactor)
        {
            Interactor = interactor;
        }
    }
}
