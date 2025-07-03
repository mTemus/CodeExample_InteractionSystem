using CodeExamples.Codebase.InteractionSystem.Core;
using CodeExamples.Codebase.InteractionSystem.Core.Finders;
using CodeExamples.Codebase.InteractionSystem.Core.Handlers;
using CodeExamples.Codebase.InteractionSystem.Other.Raycast;
using UnityEngine;

namespace CodeExamples.Codebase.InteractionSystem.Player
{
    public class PlayerInteractionAbility : PlayerAbilityBase, IInteractor
    {
        [TabGroup("Raycast")] 
        [SerializeField] 
        private LayerMask m_interactionMask;

        [TabGroup("Raycast")] 
        [SerializeField] 
        private float m_interactionRange = 5f;

        [TabGroup("SOAP")] 
        [SerializeField, Required, AssetsOnly, AssetSelector]
        private ScriptableEventInteractionSelection m_onInteractionSelectEvent;

        [TabGroup("SOAP")]
        [SerializeField, Required, AssetsOnly, AssetSelector]
        private ScriptableEventInteractionSelection m_onInteractionDeselectEvent;

        [TabGroup("SOAP")]
        [SerializeField, Required, AssetsOnly, AssetSelector]
        private ScriptableEventInputDisplayData m_showInputEvent;

        [TabGroup("SOAP")]
        [SerializeField, Required, AssetsOnly, AssetSelector]
        private ScriptableEventString m_hideInputEvent;

#if UNITY_EDITOR
        [TabGroup("Debug")]
        [SerializeField, ReadOnly]
        private InteractableObject m_currentInteractable;
# endif
        //TODO: handler also, but not sure if it needs to be another type
        [TabGroup("Debug")]
        [SerializeField, ReadOnly]
        private InteractionHandlerWithInput m_handler;

        //TODO: finder can be from a factory
        [TabGroup("Debug")]
        [SerializeField, ReadOnly]
        private InteractionFinder m_interactionFinder;

        private void Awake()
        {
            enabled = false;
        }

        private void FixedUpdate()
        {
            m_interactionFinder.TryToFindInteraction();
        }

        #region Initialization

        public override bool InitializationEnabled => true;

        public override void EarlyInitialize(EESceneInitializationManager manager)
        {
            m_handler = new InteractionHandlerWithInput(this, m_showInputEvent, m_hideInputEvent);
            m_interactionFinder = new InteractionFPPFinder(new RayCasterData(UnityEngine.Camera.main, m_interactionMask, m_interactionRange));

            m_interactionFinder.Initialize(this);
            m_interactionFinder.CurrentInteractable.AddChangedListener(OnInteractableChanged, false);
        }

        public override void Uninitialize(EESceneInitializationManager manager)
        {
            m_interactionFinder.Uninitialize(this);
        }

        #endregion

        #region IPlayerAbility

        public override void Enable()
        {
            enabled = true;
        }

        public override void Disable()
        {
            enabled = false;
        }

        #endregion

        #region IInteractor

        public EEGameObject MainObject => Parent;

        public void RegisterAvailableInteraction(IInteraction interaction)
        {
            if (interaction is not IInputInteraction inputInteraction)
                return;

            m_handler.HandleAvailable(inputInteraction);
        }

        public void UnregisterAvailableInteraction(IInteraction interaction)
        {
            UnregisterInteraction(interaction);
        }

        public void RegisterUnavailableInteraction(IInteraction interaction)
        {
            if (interaction is not IInputInteraction inputInteraction)
                return;

            m_handler.HandleUnavailable(inputInteraction);
        }

        public void UnregisterUnavailableInteraction(IInteraction interaction)
        {
            UnregisterInteraction(interaction);
        }

        private void UnregisterInteraction(IInteraction interaction)
        {
            if (interaction is not IInputInteraction inputInteraction)
                return;

            m_handler.HandleRemove(inputInteraction);
        }

        #endregion

        #region Observers

        private void OnInteractableChanged(ValueReferenceBase value)
        {
            var interactable = value.GetValueAs<InteractableObject>();

#if UNITY_EDITOR
            m_currentInteractable = interactable;
#endif

            if (interactable == null)
            {
                var previousInteractable = value.GetPreviousValueAs<InteractableObject>();

                // shouldn't be null but will see

                // if (previousInteractable == null)
                //     return;

                previousInteractable.Deselect(this);
                previousInteractable.RemoveFromInteractor(this);

                m_onInteractionDeselectEvent.Raise(new InteractionSelectionData(previousInteractable, this));
            }
            else
            {
                interactable.Select(this);
                interactable.AddToInteractor(this);

                m_onInteractionSelectEvent.Raise(new InteractionSelectionData(interactable, this));
            }
        }

        #endregion
    }

    public struct InteractionSelectionData
    {
        public InteractableObject InteractableObject { get; }
        public IInteractor Interactor { get; }

        public InteractionSelectionData(InteractableObject interactableObject, IInteractor interactor)
        {
            InteractableObject = interactableObject;
            Interactor = interactor;
        }
    }
}
