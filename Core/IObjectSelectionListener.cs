namespace CodeExamples.Codebase.InteractionSystem.Core
{
    public interface IObjectSelectionListener
    {
        public void OnObjectSelected(IInteractor interactor);
        public void OnObjectDeselected(IInteractor interactor);
    }
}