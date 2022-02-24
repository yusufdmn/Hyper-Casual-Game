using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MarketController : MonoBehaviour
{
    public static MarketController Current;
    public List<MarketItem> items;
    public List<Item> equippedItems;
    public GameObject marketMenu;

    public void InitializeMarketController()
    {
        Current = this;
        foreach(MarketItem item in items)
        {
            item.InitiliazeItem();
        }
    }

    public void ActivateMarketMenu()
    {
        marketMenu.SetActive(!marketMenu.gameObject.activeSelf);
    }
}
