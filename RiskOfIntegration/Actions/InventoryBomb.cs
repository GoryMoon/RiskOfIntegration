using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using R2API.Utils;
using RoR2;
using UnityEngine;
using Random = UnityEngine.Random;

namespace RiskOfIntegration.Actions
{
    public class InventoryBomb: BaseAction
    {
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate, PropertyName = "spread")]
        private float _spread;
        
        
        public override ActionResponse Handle()
        {
            if (Utils.GetPlayer(out _, out var player))
            {
                var items = new List<Tuple<PickupIndex, int>>();
                var inventory = player.inventory;
                var transform = player.gameObject.transform;
                for (var i = 0; i < ItemCatalog.itemCount; i++)
                {
                    var itemCount = inventory.GetItemCount((ItemIndex) i);
                    if (itemCount > 0)
                    {
                        items.Add(Tuple.Create(PickupCatalog.FindPickupIndex((ItemIndex) i), itemCount));
                    }
                }

                var equipment = inventory.GetEquipment(inventory.activeEquipmentSlot);
                if (equipment.equipmentIndex != EquipmentIndex.None)
                {
                    items.Add(Tuple.Create(PickupCatalog.FindPickupIndex(equipment.equipmentIndex), 1));
                }
                
                foreach (var (pickupIndex, count) in items)
                {
                    for (var i = 0; i < count; i++)
                    {
                        PickupDropletController.CreatePickupDroplet(pickupIndex,
                            transform.position, 
                            Vector3.forward * Random.Range(-_spread, _spread) + Vector3.right * Random.Range(-_spread, _spread) + Vector3.up * 10);
                    }
                }
                inventory.CopyItemsFrom(new GameObject().AddComponent<Inventory>());
                inventory.SetEquipmentIndex(EquipmentIndex.None);

                ChatMessage.SendColored($"{From} made you drop your items!", "#d63031");
                return ActionResponse.Done;
            }

            return ActionResponse.Done;
        }
    }
}