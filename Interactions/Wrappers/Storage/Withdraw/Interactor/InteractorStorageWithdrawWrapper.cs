using CodeExamples.Codebase.InteractionSystem.Core.Handlers;
using CodeExamples.Codebase.InteractionSystem.Interactions.Wrappers.Storage.Extensions;
using UnityEngine;

namespace CodeExamples.Codebase.InteractionSystem.Interactions.Wrappers.Storage.Withdraw.Interactor
{
    public class InteractorStorageWithdrawWrapper : NestedInputInteractionHandlerBase, IStorageWithdrawInteractor
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

        #region Observers

        private void OnStorageStockChanged(IStorage storage)
        {
            m_isAvailable.Value = CheckAvailability();
        }

        #endregion

        #region IStorageWithdrawInteractor

        protected override bool CheckAvailability()
        {
            return base.CheckAvailability() && !m_storage.Value.IsEmpty;
        }

        public bool CanWithdraw()
        {
            return !m_storage.Value.IsEmpty || m_storage.Value.CanWithdraw(this);
        }

        public IStorable Withdraw(bool remove)
        {
            return m_storage.Value.Withdraw(remove);
        }

        #endregion
    }
}