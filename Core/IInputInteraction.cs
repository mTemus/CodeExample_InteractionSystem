namespace CodeExamples.Codebase.InteractionSystem.Core
{
    public interface IInputInteraction : IInteraction
    {
        public InteractionInput Input { get; }

        public void InteractionInputDown(IInteractor interactor);
        public void InteractionInputPressed(IInteractor interactor);
        public void InteractionInputUp(IInteractor interactor);
    }
}