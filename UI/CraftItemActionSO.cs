using EE.Core;
using EE.ItemSystem;
using EE.ItemSystem.Crafting.Impl;
using EE.ItemSystem.Impl;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EE.UI.Actions {
    public class CraftItemActionSO : GenericActionSO<CraftItemAction> {
        [SerializeField]
        private CrafterSO craftingController = null;
        public CrafterSO CraftingController => craftingController;

        public ContentTypeSO ItemContentType = null;

    }
    public class CraftItemAction : GenericAction {
        private CraftItemActionSO OriginSO => (CraftItemActionSO)_originSO;
        GridElement gridElement;

        public override void Init(IHasComponents controller) {
            gridElement = controller.GetComponent<GridElement>();
        }

        public override void Enter() {
            List<Item> items = new List<Item>();
            foreach (var window in CraftingUI.Instance.additionalItemsWindows) {
                GameObject gameObject = window.GetContent(OriginSO.ItemContentType);
                if (gameObject == null) {
                    continue;
                }
                Item item = gameObject.GetComponent<HasItemComponent>().Item;
                items.Add(item);
            }
            Item craftedItem = OriginSO.CraftingController.Craft(items);

            CraftingUI.Instance.ItemChanged();
        }
    }

}