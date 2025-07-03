namespace CodeExamples.Codebase.InteractionSystem.Core
{
    public interface IInteraction
    {
        public ValueReference<bool> IsAvailable { get; }

        public void TryToInteract(IInteractor interactor);
        public void RemoveInteraction(IInteractor interactor);

        public void InteractionStart(IInteractor interactor);
        public void InteractionEnd(IInteractor interactor);
    }
}
