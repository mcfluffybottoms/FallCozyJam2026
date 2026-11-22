using System;
using System.Collections.Generic;
using UnityEngine;

public class ItemManager : MonoBehaviour
{
    // set up show
    // items

    // hand

    // watering can

    // shovels


    // inventory
    [SerializeReference, SubclassSelector]
    private List<Item> itemList = new();
    // [SerializeReference, SubclassSelector]
    // private List<Item> itemList = new();


    private List<Item> boughtItems = new();
    private List<Item> availableItems = new();
    private int ActiveItemId = 0;
    [SerializeField] private EconomyManager economyManager;

    public void BuyItem(int itemID)
    {
        if (itemID < 0 || itemID >= itemList.Count)
        {
            return;
        }
        var item = itemList[itemID];

        if (!IsBuyable(itemID) || GetBudget() < item.Cost)
        {
            int diff = item.Cost - GetBudget();
            return;
        }
        else if (!item.Bought || item.Rebuyable)
        {
            GetMoneySaver().DecreaseMoney(item.Cost);
            item.Buy();
        }
        SetActiveItem(itemID);

        if (SoundManager.Instance != null)
            SoundManager.Instance.PlayUIButton();
    }

    public bool IsBuyable(int itemID)
    {
        var item = itemList[itemID];
        return !(item.Bought && !item.Rebuyable);
    }

    public List<Item> GetItemList()
    {
        return itemList;
    }

    public Item GetActiveItem()
    {
        return itemList[ActiveItemId];
    }

    public void SetActiveItem(int id)
    {
        ActiveItemId = id;
    }

    public MoneySaver GetMoneySaver()
    {
        return economyManager.MoneySaver;
    }
    public int GetBudget()
    {
        return economyManager.MoneySaver.Money;
    }


}