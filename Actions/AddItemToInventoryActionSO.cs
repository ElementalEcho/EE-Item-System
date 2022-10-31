using EE.Core;
using UnityEngine;


namespace EE.ItemSystem.Actions {
    public class AddItemToInventoryActionSO : GenericActionSO<AddItemToInventoryAction> {

        [SerializeField]
        private InspectorItem[] itemsToAdd = new InspectorItem[0];
        public Item[] ItemsToAdd => itemsToAdd.GetItems();
    }

    public class AddItemToInventoryAction : GenericAction {
        IInventoryUser inventory;
        private AddItemToInventoryActionSO OriginSO => (AddItemToInventoryActionSO)_originSO;

        public override void Init(IHasComponents controller) {
            inventory = controller.GetComponent<IInventoryUser>();
        }
        public override void Enter() {
            if (OriginSO.Reverse) {
                foreach (var item in OriginSO.ItemsToAdd) {
                    inventory.Remove(item.ItemInfo, item.NumberOfItems);
                }
            }
            else {
                foreach (var item in OriginSO.ItemsToAdd) {
                    inventory.Add(item);
                }
            }

        }
        protected override bool Decide() {
            return !inventory.IsFull;
        }
    }
}