using CodeExamples.Codebase.InteractionSystem.Core;
using UnityEngine;
using UnityEngine.Events;

namespace CodeExamples.Codebase.InteractionSystem.Interactions
{
    public abstract class MonoInputInteraction : EEMonoBehaviour, IInputInteraction
    {
        [TabGroup("Interaction Blockers")]
        [SerializeField, Required]
        private InterfaceReference<IInteractionBlocker>[] m_blockers;

        [TabGroup("Input")] 
        [SerializeField] 
        private InteractionInput m_input;

        [TabGroup("Interaction Events")]
        [SerializeField, ES3NonSerializable]
        protected UnityEvent OnFirstInteraction;

        [TabGroup("Interaction Events")]
        [SerializeField, ES3NonSerializable]
        protected UnityEvent OnInteractionStart;

        [TabGroup("Interaction Events")]
        [SerializeField, ES3NonSerializable]
        protected UnityEvent OnInteractionEnd;

        [TabGroup("Debug")]
        [SerializeField, ReadOnly]
        protected bool m_notInteracted = true;

        [TabGroup("Debug")] 
        [SerializeField]
        protected ValueReference<bool> m_isAvailable;

        public InteractionInput Input => m_input;
        public ValueReference<bool> IsAvailable => m_isAvailable;

        #region Initialization

        public override bool InitializationEnabled => true;

        public override void EarlyInitialize(EESceneInitializationManager manager)
        {
            m_isAvailable = new ValueReference<bool>(true, CheckAvailability(), this);
        }

        #endregion

        #region Input

        public virtual void InteractionInputDown(IInteractor interactor) { }

        public virtual void InteractionInputPressed(IInteractor interactor) { }

        public virtual void InteractionInputUp(IInteractor interactor) { }

        #endregion

        protected virtual bool CanBeInteracted(IInteractor interactor)
        {
            return m_isAvailable.Value;
        }

        protected virtual bool CheckAvailability()
        {
            for (int i = 0; i < m_blockers.Length; i++)
                if (m_blockers[i].Value.IsBlocking.Value)
                    return false;
            
            return true;
        }

        public virtual void TryToInteract(IInteractor interactor)
        {
            if (ShouldIgnoreInteractor(interactor))
                return;

            if (CanBeInteracted(interactor))
                interactor.RegisterAvailableInteraction(this);
            else
                interactor.RegisterUnavailableInteraction(this);

            for (int i = 0; i < m_blockers.Length; i++)
                m_blockers[i].Value.IsBlocking.AddChangedListener(OnBlockerValueChanged);
        }

        public virtual void RemoveInteraction(IInteractor interactor)
        {
            if (ShouldIgnoreInteractor(interactor))
                return;

            for (int i = 0; i < m_blockers.Length; i++)
                m_blockers[i].Value.IsBlocking.RemoveChangedListener(OnBlockerValueChanged);

            if (CanBeInteracted(interactor))
                interactor.UnregisterAvailableInteraction(this);
            else
                interactor.UnregisterUnavailableInteraction(this);
        }

        public virtual void InteractionStart(IInteractor interactor)
        {
            OnInteractionStart.Invoke();

            if (!m_notInteracted)
            {
                OnFirstInteraction.Invoke();
                m_notInteracted = false;
            }
        }

        public virtual void InteractionEnd(IInteractor interactor)
        {
            OnInteractionEnd.Invoke();
        }

        //TODO: IT IS GOOD FOR NOW
        //TODO: this is good for "try to interact", but should not be used in removed, there should be an optimization in the interactor way to somehow add interactions in dictionary with its InteractableObject
        protected abstract bool ShouldIgnoreInteractor(IInteractor interactor);

        protected virtual void OnBlockerValueChanged(ValueReferenceBase value)
        {
            m_isAvailable.Value = CheckAvailability();
        }
    }
}
