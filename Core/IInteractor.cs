namespace CodeExamples.Codebase.InteractionSystem.Core
{
    public interface IInteractor
    {
        public EEGameObject MainObject { get; }

        public void RegisterAvailableInteraction(IInteraction interaction);
        public void UnregisterAvailableInteraction(IInteraction interaction);
        public void RegisterUnavailableInteraction(IInteraction interaction);
        public void UnregisterUnavailableInteraction(IInteraction interaction);
    }
}
