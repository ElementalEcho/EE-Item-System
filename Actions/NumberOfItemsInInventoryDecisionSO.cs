using EE.Core;
using UnityEngine;

namespace EE.ItemSystem.Decisions {
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

        IInventoryUser inventory;
        public override void Init(IHasComponents controller) {
            inventory = controller.GetComponent<IInventoryUser>();
        }

        protected override bool Decide() {           
            if (OriginSO.IsFullAndStacksAreFull) {
                //Debug.LogError("Not Implemented");
                //return inventory.IsFullAndStacksAreFull;
                return inventory.IsFull;

            }
            if (OriginSO.InventoryFull) {
                return inventory.IsFull;
            }
            if (OriginSO.InventoryEmpty) {
                return inventory.NumberOfFilledSlots <= 0;
            }
            return false;
        }

    }
}