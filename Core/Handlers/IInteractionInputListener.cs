namespace CodeExamples.Codebase.InteractionSystem.Core.Handlers
{
    public interface IInteractionInputListener : IInteractionListener
    {
        public void InteractionInputDown(IInputInteraction interaction);
        public void InteractionInputPressed(IInputInteraction interaction);
        public void InteractionInputUp(IInputInteraction interaction);
    }

    public interface IInteractionListener
    {
        public void InteractionStart(IInteraction interaction);
        public void InteractionEnd(IInteraction interaction);
    }
}
