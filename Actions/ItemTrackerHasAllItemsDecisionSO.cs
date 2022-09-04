using EE.InventorySystem.Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EE.Core.Actions {
    public class ItemTrackerHasAllItemsDecisionSO : GenericActionSO<ItemTrackerHasAllItemsDecision> {

    }
    public class ItemTrackerHasAllItemsDecision : GenericAction {
        private ItemTrackerHasAllItemsDecisionSO OriginSO => (ItemTrackerHasAllItemsDecisionSO)_originSO;

        ItemTracker itemTracker;

        public override void Init(IHasComponents controller) {
            itemTracker = controller.GetComponent<ItemTracker>();
        }

        protected override bool Decide() {
            foreach (var item in itemTracker.requiredItems) {
                if (!item.completed) {
                    return false;
                }
            }
            return true;

        }
    }
}