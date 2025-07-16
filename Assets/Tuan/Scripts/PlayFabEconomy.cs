using System.Collections.Generic;
using PlayFab.ClientModels;
using PlayFab.EconomyModels;
using PlayFab;
using UnityEngine;

public class ItemsPurchase : MonoBehaviour
{
    public void RefreshInventory()
    {
        PlayFabClientAPI.GetUserInventory(new GetUserInventoryRequest(),
            result =>
            {
                foreach (var item in result.Inventory)
                    Debug.Log($"Item: {item.ItemId} x{item.RemainingUses}");

                Debug.Log($"Coins: {result.VirtualCurrency["CO"]}");
            },
            error => Debug.LogError(error.GenerateErrorReport()));
    }
    public void AddCoins(int amount)
    {
        var req = new AddUserVirtualCurrencyRequest
        {
            VirtualCurrency = "CO",
            Amount = amount
        };

        PlayFabClientAPI.AddUserVirtualCurrency(req,
            res => Debug.Log($"+{amount} Coins (new balance {res.Balance})"),
            err => Debug.LogError(err.GenerateErrorReport()));
    }

   /* public void BuySkin(string skinItemId)
    {
        var request = new SubmitInventoryPurchaseRequest
        {
            Item = new InventoryItemReference
            {
                Id = "SKIN_NINJA",
                Type = "catalogItem"
            },
            PriceAmounts = new List<InventoryPriceAmount>
    {
        new InventoryPriceAmount
        {
            Amount = 500,
            Item = new InventoryItemReference
            {
                Id = "CO",
                Type = "currency"
            }
        }
    }
        };

        PlayFabEconomyAPI.SubmitInventoryPurchase(request,
            res => {
                Debug.Log("Mua skin Economy V2 thành công!");
            },
            err => Debug.LogError(err.GenerateErrorReport()));
    }*/
/*    public void ShowOwnedSkins()
    {
        PlayFabClientAPI.GetUserInventory(new GetUserInventoryRequest(),
            res => {
                foreach (var item in res.Inventory)
                {
                    if (item.ItemClass == "skin")
                        Debug.Log($"Sở hữu skin: {item.DisplayName} ({item.ItemId})");
                }
            },
            err => Debug.LogError(err.GenerateErrorReport()));
    }*/
/*    public void SetEquippedSkin(string itemId)
    {
        var req = new UpdateUserDataRequest
        {
            Data = new Dictionary<string, string> {
            { "EquippedSkin", itemId }
        }
        };
        PlayFabClientAPI.UpdateUserData(req,
            res => Debug.Log("Trang phục đã chọn: " + itemId),
            err => Debug.LogError(err.GenerateErrorReport()));
    }*/

   /* public void GetEquippedSkin(System.Action<string> callback)
    {
        PlayFabClientAPI.GetUserData(new GetUserDataRequest(),
            res => {
                if (res.Data.TryGetValue("EquippedSkin", out var entry))
                    callback(entry.Value);
                else
                    callback(null);
            },
            err => Debug.LogError(err.GenerateErrorReport()));
    }*/
    /*public void TryBuySkin(string skinItemId)
    {
        PlayFabClientAPI.GetUserInventory(new GetUserInventoryRequest(),
            res => {
                bool hasSkin = res.Inventory.Any(i => i.ItemId == skinItemId);
                if (hasSkin)
                {
                    Debug.Log("Bạn đã sở hữu skin này rồi.");
                    SetEquippedSkin(skinItemId); // Hoặc hiện nút [Trang bị]
                }
                else
                {
                    BuySkin(skinItemId); // Gọi mua nếu chưa có
                }
            },
            err => Debug.LogError(err.GenerateErrorReport()));
    }*/

}
