using System.Linq;
using UnityEngine;

namespace CodeExamples.Codebase.InteractionSystem.Core.Handlers
{
    public abstract class NestedInputInteractionHandlerBase : EEMonoBehaviour
    {
        [TabGroup("Blockers")]
        [SerializeField, Required]
        private InterfaceReference<IInteractionBlocker>[] m_blockers;

        [TabGroup("SOAP")]
        [SerializeField, Required, AssetsOnly, AssetSelector]
        protected ScriptableEventInputDisplayData m_showInputEvent;

        [TabGroup("SOAP")]
        [SerializeField, Required, AssetsOnly, AssetSelector]
        protected ScriptableEventString m_hideInputEvent;

        [TabGroup("Debug")]
        [SerializeField]
        protected ValueReference<bool> m_isAvailable;

        protected InteractionHandlerWithInput m_holdableInteractionsHandler;

        #region Initialization

        public override bool InitializationEnabled => true;

        public override void EarlyInitialize(EESceneInitializationManager manager)
        {
            m_isAvailable = new ValueReference<bool>(true, CheckAvailability());

            for (var i = 0; i < m_blockers.Length; i++)
                m_blockers[i].Value.IsBlocking.AddChangedListener(OnBlockerValueChanged);
        }

        public override void Uninitialize(EESceneInitializationManager manager)
        {
            for (var i = 0; i < m_blockers.Length; i++)
                m_blockers[i].Value.IsBlocking.RemoveChangedListener(OnBlockerValueChanged);
        }

        #endregion

        #region Observers

        private void OnBlockerValueChanged(ValueReferenceBase value)
        {
            m_isAvailable.Value = CheckAvailability();
        }

        #endregion

        #region Interactor

        protected virtual bool CheckAvailability()
        {
            return m_blockers.All(blocker => !blocker.Value.IsBlocking.Value);
        }

        #region Available

        public void RegisterAvailableInteraction(IInteraction interaction)
        {
            if (interaction is not IInputInteraction inputInteraction)
                return;

            m_holdableInteractionsHandler.HandleAvailable(inputInteraction);
        }

        public void UnregisterAvailableInteraction(IInteraction interaction)
        {
            UnregisterInteraction(interaction);
        }

        #endregion

        #region Unavailable

        public void RegisterUnavailableInteraction(IInteraction interaction)
        {
            if (interaction is not IInputInteraction inputInteraction)
                return;

            m_holdableInteractionsHandler.HandleUnavailable(inputInteraction);
        }

        public void UnregisterUnavailableInteraction(IInteraction interaction)
        {
            UnregisterInteraction(interaction);
        }
        
        private void UnregisterInteraction(IInteraction interaction)
        {
            if (interaction is not IInputInteraction inputInteraction)
                return;

            m_holdableInteractionsHandler.HandleRemove(inputInteraction);
        }

        #endregion

        #endregion
    }
}