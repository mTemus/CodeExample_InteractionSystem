namespace CodeExamples.Codebase.InteractionSystem.Core
{
    public interface IInteractionBlocker
    {
        public ValueReference<bool> IsBlocking { get; }
    }
}
