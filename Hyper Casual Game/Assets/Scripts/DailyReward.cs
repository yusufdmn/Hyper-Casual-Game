using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DailyReward : MonoBehaviour
{

    public bool initialized;

    public long rewardGivingTimeTicks;
    public GameObject dailyRewardMenu;
    public Text remainingRewardTime;

    public void InitializeDailyReward()
    {
        if (PlayerPrefs.HasKey("lastDailyReward"))
        {
            rewardGivingTimeTicks = long.Parse(PlayerPrefs.GetString("lastDailyReward")) + 864000000000;
            long currentTime = System.DateTime.Now.Ticks;

            if(currentTime >= rewardGivingTimeTicks)
            {
                GiveReward();
            }
        }
        else
        {
            GiveReward();
        }
        initialized = true;
    }


    public void GiveReward()
    {
        LevelController.Current.GiveMoneyToPlayer(100);
        dailyRewardMenu.SetActive(true);
        PlayerPrefs.SetString("lastDailyReward", System.DateTime.Now.Ticks.ToString());
        rewardGivingTimeTicks = long.Parse(PlayerPrefs.GetString("lastDailyReward")) + 864000000000;
    }


    void Update()
    {
        if (initialized == true)
        {
            if(LevelController.Current.startMenu.activeInHierarchy == true)
            {
                long currentTime = System.DateTime.Now.Ticks;
                long remainingTime = rewardGivingTimeTicks - currentTime;
                if(remainingTime <= 0)
                {
                    GiveReward();
                }
                else
                {
                    System.TimeSpan timeSpan = System.TimeSpan.FromTicks(remainingTime);
                    remainingRewardTime.text = string.Format("{0}:{1}:{2}", timeSpan.Hours.ToString("D2"), timeSpan.Minutes.ToString("D2"), timeSpan.Seconds.ToString("D2"));
                }
            }
        }

    } 
    public void TapToReturnButton(){

        dailyRewardMenu.SetActive(false);

    }
}
