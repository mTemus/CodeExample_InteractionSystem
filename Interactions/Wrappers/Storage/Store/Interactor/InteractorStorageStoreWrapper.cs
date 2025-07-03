using CodeExamples.Codebase.InteractionSystem.Core.Handlers;
using CodeExamples.Codebase.InteractionSystem.Interactions.Wrappers.Storage.Extensions;
using UnityEngine;

namespace CodeExamples.Codebase.InteractionSystem.Interactions.Wrappers.Storage.Store.Interactor
{
    public class InteractorStorageStoreWrapper : NestedInputInteractionHandlerBase, IStorageStoreInteractor
    {
        [TabGroup("Requirements")]
        [SerializeField, Required]
        private InterfaceReference<IStorage> m_storage;

        public ValueReference<bool> IsAvailable => m_isAvailable;
        public EEGameObject MainObject => Parent;

        #region Initialization

        public override bool InitializationEnabled => true;

        public override void EarlyInitialize(EESceneInitializationManager manager)
        {
            base.EarlyInitialize(manager);

            m_holdableInteractionsHandler = new InteractionHandlerWithInput(this, m_showInputEvent, m_hideInputEvent);
            m_storage.Value.OnStorageStockChanged.AddListener(OnStorageStockChanged);
        }

        public override void Uninitialize(EESceneInitializationManager manager)
        {
            base.Uninitialize(manager);
            m_storage.Value.OnStorageStockChanged.RemoveListener(OnStorageStockChanged);
        }

        #endregion

        #region Observer

        private void OnStorageStockChanged(IStorage storage)
        {
            m_isAvailable.Value = CheckAvailability();
        }

        #endregion

        #region IStorageStoreInteractor

        protected override bool CheckAvailability()
        {
            return base.CheckAvailability() && !m_storage.Value.IsFull;
        }

        public bool CanStore(IStorable storable)
        {
            return m_storage.Value.CanStore(storable);
        }

        public void Store(IStorable storable)
        {
            m_storage.Value.Store(storable);
        }

        #endregion
    }
}