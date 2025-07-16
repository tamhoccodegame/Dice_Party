using System.Collections;
using System.Collections.Generic;
using PlayFab.ClientModels;
using PlayFab;
using UnityEngine;

public class PlayFabEconomy : MonoBehaviour
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
    public void SpendCoins(int amount)
    {
        var req = new SubtractUserVirtualCurrencyRequest
        {
            VirtualCurrency = "CO",
            Amount = amount
        };

        PlayFabClientAPI.SubtractUserVirtualCurrency(req,
            res => Debug.Log($"-{amount} Coins (bal. {res.Balance})"),
            err => Debug.LogError(err.GenerateErrorReport()));
    }
    public void GrantSword()
    {
        PlayFabClientAPI.ExecuteCloudScript(new ExecuteCloudScriptRequest
        {
            FunctionName = "Main",
            FunctionParameter = new { itemId = "Sword" }
        },
        res => Debug.Log("Đã nhận Sword"),
        err => Debug.LogError(err.GenerateErrorReport()));
    }
    public void SaveEquipped(string weaponId)
    {
        PlayFabClientAPI.UpdateUserData(new UpdateUserDataRequest
        {
            Data = new Dictionary<string, string> { { "equippedWeapon", weaponId } }
        },
        _ => Debug.Log("Đã lưu weapon"),
        err => Debug.LogError(err.GenerateErrorReport()));
    }
}
