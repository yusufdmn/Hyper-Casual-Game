using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MarketItem : MonoBehaviour
{
    public int id , price , wearId ;
    public Text priceText;
    public Button buyButton, equipButton, unequipButton;

    public GameObject itemPrefab;

    public bool HasItem()
    {
        // 0 = Isn't bought
        // 1 = Bought, not equipped
        // 2 = Bought and equipped

        bool hasItem = PlayerPrefs.GetInt("Item" + id.ToString()) != 0;
        return hasItem;
    }
    public bool IsEquipped()
    {

        bool isEquipped = PlayerPrefs.GetInt("Item" + id.ToString()) == 2;
        return isEquipped;
    }
    public void InitiliazeItem()
    {
        buyButton.gameObject.SetActive(true);

        priceText.text = price.ToString();
        if(HasItem() == true)
        {
            Debug.Log("b");
            buyButton.gameObject.SetActive(false);

            if(IsEquipped() == true)
            {
                unequipButton.gameObject.SetActive(true);
                EquipItem();
            }
            else
            {
                equipButton.gameObject.SetActive(true);
            }
        }
        else
        {
            buyButton.gameObject.SetActive(true);
        }
    }
    public void BuyItem()
    {
        if(HasItem() == false)
        {
            int money = PlayerPrefs.GetInt("money");
            
            if(money >= price)
            {
                LevelController.Current.GiveMoneyToPlayer(-price);
                PlayerPrefs.SetInt("Item" + id.ToString(), 2);
                buyButton.gameObject.SetActive(false);
                equipButton.gameObject.SetActive(false);
                unequipButton.gameObject.SetActive(true);
                CharacterControllerScript.Current.itemAudioSource.PlayOneShot(CharacterControllerScript.Current.buyItemClip, 01f);
                EquipItem();
            }
        }
    }

    public void EquipItem()
    {
        UnEquipItem();
        MarketController.Current.equippedItems[wearId] = Instantiate(itemPrefab, CharacterControllerScript.Current.wearSpots[wearId].transform).GetComponent<Item>();
        MarketController.Current.equippedItems[wearId].id = id;
        equipButton.gameObject.SetActive(false);
        unequipButton.gameObject.SetActive(true);
        PlayerPrefs.SetInt("Item" + id.ToString(), 2);
    }

    public void UnEquipItem()
    {
        Item equippedItem = MarketController.Current.equippedItems[wearId];


        if(equippedItem != null)
        {
            MarketItem marketItem = MarketController.Current.items[equippedItem.id];
            PlayerPrefs.SetInt("Item" + marketItem.id, 1);
            marketItem.equipButton.gameObject.SetActive(true);
            marketItem.unequipButton.gameObject.SetActive(false);
            Destroy(equippedItem.gameObject);
        }

    }

    public void EquipItemButton()
    {
        CharacterControllerScript.Current.itemAudioSource.PlayOneShot(CharacterControllerScript.Current.equipItemAudioClip , 0.1f);
        EquipItem();

    }

    public void UnEquipItemButton()
    {
        CharacterControllerScript.Current.itemAudioSource.PlayOneShot(CharacterControllerScript.Current.unEquipItemAudioClip, 0.1f);
        UnEquipItem();

    }


}
