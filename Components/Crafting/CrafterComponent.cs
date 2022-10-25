using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
namespace EE.ItemSystem.Crafting {
    public interface ICrafterComponent {

    }
}
namespace EE.ItemSystem.Crafting.Impl {
    public class CrafterComponent : MonoBehaviour, ICrafterComponent {
        [SerializeField]
        private CraftingDataSO craftingDataSO;


        private ICrafter crafter;

        private IInventoryUser inventory;

        [SerializeField]
        private int index = 0;

        private void Awake() {
            inventory = GetComponent<IInventoryUser>();

            ICraftingData craftingData = craftingDataSO != null ? craftingDataSO.CraftingData : null;
            crafter = new Crafter(craftingData);
        }
        [Button]
        public void Craft() {
            if (crafter.Craft(inventory.GetItems(), index, out Item item)) {
                inventory.AddItem(new Item(item.ItemInfo, item.NumberOfItems));
                Debug.Log("Item crafted");
            }
            else {
                Debug.Log("Crafting failed");
            }
        }
    }
}