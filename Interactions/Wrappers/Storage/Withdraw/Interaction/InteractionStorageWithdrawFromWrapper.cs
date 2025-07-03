using CodeExamples.Codebase.InteractionSystem.Core;
using CodeExamples.Codebase.InteractionSystem.Interactions.Wrappers.Storage.Extensions;
using UnityEngine;

namespace CodeExamples.Codebase.InteractionSystem.Interactions.Wrappers.Storage.Withdraw.Interaction
{
    public class InteractionStorageWithdrawFromWrapper : MonoInputInteraction
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

        private IStorageStoreInteractor m_interactor;

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
            return base.CheckAvailability() && !m_storage.Value.IsEmpty;
        }

        protected override bool ShouldIgnoreInteractor(IInteractor interactor)
        {
            return interactor is not IStorageStoreInteractor;
        }

        protected override bool CanBeInteracted(IInteractor interactor)
        {
            if (!m_storage.Value.CanWithdraw(this))
                return false;

            var storageInteractor = interactor as IStorageStoreInteractor;

            return base.CanBeInteracted(interactor) && storageInteractor.CanStore(m_storage.Value.Withdraw(false));
        }

        #endregion

        #region Input Interaction

        public override void InteractionInputDown(IInteractor interactor)
        {
            m_interactor = interactor as IStorageStoreInteractor;

            m_interactor.Store(m_storage.Value.Withdraw());
        }

        public override void InteractionInputPressed(IInteractor interactor)
        {
            //TODO: inject the custom delta time here
            m_currentDelay += Time.deltaTime;

            if (m_currentDelay < m_storeDelay)
                return;

            if (m_storage.Value.IsEmpty)
                return;

            m_interactor.Store(m_storage.Value.Withdraw());
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