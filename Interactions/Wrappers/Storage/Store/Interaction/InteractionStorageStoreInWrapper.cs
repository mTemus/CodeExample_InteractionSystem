using CodeExamples.Codebase.InteractionSystem.Core;
using CodeExamples.Codebase.InteractionSystem.Interactions.Wrappers.Storage.Extensions;
using UnityEngine;

namespace CodeExamples.Codebase.InteractionSystem.Interactions.Wrappers.Storage.Store.Interaction
{
    public class InteractionStorageStoreInWrapper : MonoInputInteraction
    {
        [TabGroup("Requirements")]
        [SerializeField, Required]
        private InterfaceReference<IStorage> m_storage;

        [TabGroup("Delay")]
        [SerializeField]
        private float m_storeDelay;

        [TabGroup("Debug")]
        [SerializeField, ReadOnly]
        private float m_currentDelay;

        private IStorageWithdrawInteractor m_interactor;


        #region Initialization

        public override void EarlyInitialize(EESceneInitializationManager manager)
        {
            base.EarlyInitialize(manager);
            m_storage.Value.OnStorageStockChanged.AddListener(OnStockChanged);
        }

        public override void Uninitialize(EESceneInitializationManager manager)
        {
            m_storage.Value.OnStorageStockChanged.RemoveListener(OnStockChanged);
        }

        #endregion

        #region Observers

        private void OnStockChanged(IStorage storage)
        {
            m_isAvailable.Value = CheckAvailability();
        }

        #endregion

        #region Interaction

        protected override bool CheckAvailability()
        {
            return base.CheckAvailability() && !m_storage.Value.IsFull;
        }

        public override void InteractionStart(IInteractor interactor)
        {
            base.InteractionStart(interactor);
            m_interactor = interactor as IStorageWithdrawInteractor;
        }

        protected override bool ShouldIgnoreInteractor(IInteractor interactor)
        {
            return interactor is not IStorageWithdrawInteractor;
        }

        protected override bool CanBeInteracted(IInteractor interactor)
        {
            var storageInteractor = interactor as IStorageWithdrawInteractor;

            if (!storageInteractor.CanWithdraw())
                return false;

            var storable = storageInteractor.Withdraw(false);

            return base.CanBeInteracted(interactor) && m_storage.Value.CanStore(storable);
        }

        #endregion

        #region Interaction Input

        public override void InteractionInputDown(IInteractor interactor)
        {
            m_storage.Value.Store(m_interactor.Withdraw(true));
        }

        public override void InteractionInputPressed(IInteractor interactor)
        {
            //TODO: how to inject the custom delta time here?
            m_currentDelay += Time.deltaTime;

            if (m_currentDelay < m_storeDelay)
                return;

            if (!m_interactor.CanWithdraw())
                return;

            m_storage.Value.Store(m_interactor.Withdraw(true));
            m_currentDelay = 0;
        }

        public override void InteractionInputUp(IInteractor interactor)
        {
            m_currentDelay = 0f;
            m_interactor = null;
        }

        #endregion
    }
}