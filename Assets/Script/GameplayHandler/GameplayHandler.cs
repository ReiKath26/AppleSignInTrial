using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AppleAuth;
using AppleAuth.Enums;
using AppleAuth.Extensions;
using AppleAuth.Interfaces;
using AppleAuth.Native;

public class Star
{
    public string name;
    public int neededPoint;
    public string sprite;

    public Star(string name, int neededPoint, string sprite)
    {
        this.name = name;
        this.neededPoint = neededPoint;
        this.sprite = sprite;
    }
}

public class GameplayHandler : MonoBehaviour
{
  
    private const string AppleUserIdKey = "1dCu55en74ppl3";
    private const string UserNameKey = "u53rm4n3";
    private const string UserTotalPoint = "U53rp0";
    private IAppleAuthManager _authManager;

    public GamePanel game_panel;
    private int point = 0;
    private int index = 0;

    private Star[] availableStars;

   private void Start()
   {
        availableStars = new Star[]{
            new Star("Level 1: Solid Star", 20, "solid_star"),
            new Star("Level 2: Gleam Star", 50, "bicolor_star"),
            new Star("Level 3: Radiant Star", 100, "radiant_star")
        };

        if(PlayerPrefs.HasKey(AppleUserIdKey))
        {
           SetUpGamePage(false);
        }

        else
        {
            if(AppleAuthManager.IsCurrentPlatformSupported)
            {
                var deserialize = new PayloadDeserializer();
                _authManager = new AppleAuthManager(deserialize);
            }

            SetUpGamePage(true);
        }
   }

   private void Update()
   {
        game_panel.pointSlider.value = (float)point/(float) availableStars[index].neededPoint;
        Debug.Log((float) (point/availableStars[index].neededPoint));
        game_panel.starPoint.text = point + "/" + availableStars[index].neededPoint;
   }


   private void checkStarLevel()
   {
        if(point > availableStars[index].neededPoint)
        {
            //level up effect?

            if(index + 1 < availableStars.Length)
            {
                index += 1;
                game_panel.SetStar(availableStars[index]);
            }
          
        }
   }

   public void trySigningIn()
   {
        AppleSignIn();
   }

   public void clickOnStar()
   {
        point+=1;
        checkStarLevel();
        PlayerPrefs.SetInt(UserTotalPoint, point);

         //add effect to show plus one when clicked
       
   }

   private void AppleSignIn()
    {
         var loginArgs = new AppleAuthLoginArgs(LoginOptions.IncludeEmail | LoginOptions.IncludeFullName);
        
            _authManager.LoginWithAppleId(
            loginArgs,
            credential =>
            {
                var appleCredential = credential as IAppleIDCredential;

                 PlayerPrefs.SetString(AppleUserIdKey, credential.User);

                if(appleCredential != null)
                {
                    if(appleCredential.FullName != null)
                    {
                        var name = appleCredential.FullName.ToLocalizedString(PersonNameFormatterStyle.Short);
                        PlayerPrefs.SetString(UserNameKey, name);
                    }

                    else
                    {
                        PlayerPrefs.SetString(UserNameKey, "User");
                    }
                }

                SetUpGamePage(false);

            },
            error =>
            {
                var authorizationErrorCode = error.GetAuthorizationErrorCode();
                Debug.Log("Sign In Failed " + authorizationErrorCode.ToString() + " " + error.ToString());
                SetUpGamePage(true);
            });
    }

   private void SetUpGamePage(bool isGuest)
   {
        if(PlayerPrefs.HasKey(UserTotalPoint))
        {
                point = PlayerPrefs.GetInt(UserTotalPoint);

                if(point <= 20)
                {
                    index = 0;
                    
                }

                else if(point <= 50)
                {
                    index = 1;
                }

                else
                {
                    index = 2;
                }

                game_panel.SetStar(availableStars[index]);
        }

        else
        {
                index = 0;
                game_panel.SetStar(availableStars[index]);
        }

        switch(isGuest)
        {
            case true:

            if(_authManager == null)
            {
                game_panel.SetAppleLoginAvailability(false);
            }

            else
            {
                game_panel.SetAppleLoginAvailability(true);
            }

            game_panel.SetPlayerName("Guest");
            game_panel.SetUpAppleID(false, string.Empty);
            game_panel.SetAppleLoginAvailability(true);
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
            game_panel.SetAppleLoginAvailability(false);
            return;

        }
   }
}
