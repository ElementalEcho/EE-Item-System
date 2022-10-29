using EE.Core;
using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;

namespace EE.ItemSystem.Actions {
    internal enum ItemActionTypeSet {
        Start,
        Attack,
        Throw
    }
    public class UseItemActionSO : GenericActionSO<UseItemAction> {

        public GenericActionSO[] defaultActions = new GenericActionSO[0];
        [SerializeField,InlineEditor]
        internal ItemDataBaseSO itemDataBaseSO = null;
        [SerializeField]
        internal ItemActionTypeSet itemActionType = ItemActionTypeSet.Start;
    }
    public class UseItemAction : GenericAction {
        IInventoryUser inventory;
        private UseItemActionSO OriginSO => (UseItemActionSO)base._originSO;

        GenericAction[] defaultActions = new GenericAction[0];
        public Dictionary<IItemInfo, GenericAction[]> actionsDictonary = new Dictionary<IItemInfo, GenericAction[]>();

        public override void Init(IHasComponents controller) {
            inventory = controller.GetComponent<IInventoryUser>();
            defaultActions = OriginSO.defaultActions.GetActions(controller);

            foreach (var item in OriginSO.itemDataBaseSO.ItemList) {
                GenericAction[] actions;
                switch (OriginSO.itemActionType) {
                    case ItemActionTypeSet.Start:
                        actions = item.StartItemUseEffects.GetActions(controller);
                        break;
                    case ItemActionTypeSet.Attack:
                        actions = item.AttackItemUseEffects.GetActions(controller);
                        break;
                    case ItemActionTypeSet.Throw:
                        actions = item.ThrownItemEffects.GetActions(controller);
                        break;
                    default:
                        actions = new GenericAction[0];
                        break;
                }
                //If item doesn't have actions no need to add it to dictonary and just use default actions.
                if (actions.Length > 0) {
                    actionsDictonary.Add(item.ItemType, actions);
                }
            }
        }
        public override void Enter() {
            var itemUseEffects = inventory.CurrentItem != null && actionsDictonary.TryGetValue(inventory.CurrentItem.ItemInfo, out var genericActions) ? genericActions : defaultActions;

            foreach (var useEffect in itemUseEffects) {
                useEffect.Enter();
            }
        }

        public override void Act(float tickSpeed) {
            var itemUseEffects = inventory.CurrentItem != null && actionsDictonary.TryGetValue(inventory.CurrentItem.ItemInfo, out var genericActions) ? genericActions : defaultActions;

            foreach (var useEffect in itemUseEffects) {
                useEffect.Act(tickSpeed);
            }
        }
        public override void Exit() {
            var itemUseEffects = inventory.CurrentItem != null && actionsDictonary.TryGetValue(inventory.CurrentItem.ItemInfo, out var genericActions) ? genericActions : defaultActions;

            foreach (var useEffect in itemUseEffects) {
                useEffect.Exit();
            }
        }
        public override bool ExitCondition() {
            var itemUseEffects = inventory.CurrentItem != null && actionsDictonary.TryGetValue(inventory.CurrentItem.ItemInfo, out var genericActions) ? genericActions : defaultActions;
            int numberOfActions = itemUseEffects.Length;

            foreach (var useEffect in itemUseEffects) {
                if (useEffect.ExitCondition()) {
                    numberOfActions--;
                    if (numberOfActions <= 0) {
                        return true;
                    }
                }
            }
            return false;

        }

    }
    [System.Serializable]
    internal struct ItemGenericActionStruct {
        public ItemTypeSO itemSO;
        public GenericActionSO[] genericActionSOs;

        public ItemGenericActionStruct(ItemTypeSO itemSO, GenericActionSO[] genericActionSOs) {
            this.itemSO = itemSO;
            this.genericActionSOs = genericActionSOs;
        }
    }
}
