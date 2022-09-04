using EE.Core;
using EE.Core.Actions;
using EE.Core.Targeting;
using EE.InventorySystem.Impl;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace EE.InventorySystem.Core {

    internal class ItemTracker : EEMonobehavior, IHasComponents {
        [SerializeField]
        public List<ItemTrackerData> requiredItems = new List<ItemTrackerData>();
        public List<ItemTrackerData> RequiredItems => requiredItems;

        private void Update() {
            foreach (var targetingDataSO in requiredItems) {
                var retval = true;
                foreach (var requirement in targetingDataSO.completingRequirments) {
                    if (!requirement.GetAction(this).IsTrue()) {
                        retval = false;
                        break;
                    }
                }
                if (targetingDataSO.completed != true) {
                    targetingDataSO.completed = retval;
                }
            }
        }
        [System.Serializable]
        public class ItemTrackerData {
            public ItemTypeSO itemNeeded;
            public bool completed;
            public GenericActionSO[] completingRequirments;
        }
    }

}