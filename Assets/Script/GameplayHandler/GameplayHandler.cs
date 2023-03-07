using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameplayHandler : MonoBehaviour
{
  
    private const string AppleUserIdKey = "1dCu55en74ppl3";
    private const string UserNameKey = "u53rm4n3";

   public GamePanel game_panel;

   private void Start()
   {
        if(PlayerPrefs.HasKey(AppleUserIdKey))
        {
           SetUpGamePage(false);
        }

        else
        {
            SetUpGamePage(true);
        }
   }


   private void SetUpGamePage(bool isGuest)
   {
        switch(isGuest)
        {
            case true:
            game_panel.SetPlayerName("Guest");
            game_panel.SetUpAppleID(false, string.Empty);
            game_panel.SetTryLoginVisibility(true);
            return;

            case false:
            if(PlayerPrefs.HasKey(UserNameKey))
            {
                game_panel.SetPlayerName(PlayerPrefs.GetString(UserNameKey));
               
            }

            else
            {
                game_panel.SetPlayerName("User");
            }

            game_panel.SetUpAppleID(true, PlayerPrefs.GetString(AppleUserIdKey));
            game_panel.SetTryLoginVisibility(false);
            return;

        }
   }
}
