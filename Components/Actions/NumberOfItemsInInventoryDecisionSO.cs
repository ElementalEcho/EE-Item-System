using EE.Core;
using EE.InventorySystem.Core;
using UnityEngine;

namespace EE.InventorySystem.Decisions {
    public class NumberOfItemsInInventoryDecisionSO : GenericActionSO<NumberOfItemsInInventoryDecision> {
        [SerializeField]
        private bool isFullAndStacksAreFull = false;
        public bool IsFullAndStacksAreFull => isFullAndStacksAreFull;

        [SerializeField]
        private bool inventoryFull = false;
        public bool InventoryFull => inventoryFull;


        [SerializeField]
        private bool inventoryEmpty = false;
        public bool InventoryEmpty => inventoryEmpty;
    }
    public class NumberOfItemsInInventoryDecision : GenericAction {
        private NumberOfItemsInInventoryDecisionSO OriginSO => (NumberOfItemsInInventoryDecisionSO)_originSO;

        IInventoryComponent inventory;
        public override void Init(IHasComponents controller) {
            inventory = controller.GetComponent<IInventoryComponent>();
        }

        protected override bool Decide() {           
            if (OriginSO.IsFullAndStacksAreFull) {
                return inventory.IsFullAndStacksAreFull;
            }
            if (OriginSO.InventoryFull) {
                return inventory.InventoryFull;
            }
            if (OriginSO.InventoryEmpty) {
                return inventory.ItemCount <= 0;
            }
            return false;
        }

    }
}