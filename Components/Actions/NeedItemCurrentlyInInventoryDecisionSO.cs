using EE.Core.Targeting;
using EE.InventorySystem;
using EE.InventorySystem.Core;
using UnityEngine;

namespace EE.Core.Actions {
    public class NeedItemCurrentlyInInventoryDecisionSO : GenericActionSO<NeedItemCurrentlyInInventoryDecision> {

    }
    public class NeedItemCurrentlyInInventoryDecision : GenericAction {
        private NeedItemCurrentlyInInventoryDecisionSO OriginSO => (NeedItemCurrentlyInInventoryDecisionSO)_originSO;

        ItemTracker _itemTracker;
        IInventoryComponent inventory;
        ITargetScannerComponent targetScannerComponent;

        public override void Init(IHasComponents controller) {
            inventory = controller.GetComponent<IInventoryComponent>();
            targetScannerComponent = controller.GetComponent<ITargetScannerComponent>();
        }

        protected override bool Decide() {
            if (!targetScannerComponent.TryGetComponentFromTarget(out ItemTracker itemTracker)) {
                return false;
            }

            foreach (var item in itemTracker.requiredItems) {
                if (!item.completed && inventory.ContainsItem(new Item(item.itemNeeded.ItemType))) {
                    return true;
                }
            }
            return false;

        }
    }
}