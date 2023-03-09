using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AppleAuth;
using AppleAuth.Enums;
using AppleAuth.Extensions;
using AppleAuth.Interfaces;
using AppleAuth.Native;

public class GameplayHandler : MonoBehaviour
{
  
    private const string AppleUserIdKey = "1dCu55en74ppl3";
    private const string UserNameKey = "u53rm4n3";
    private const string UserTotalPoint = "U53rp0";
     private const string EmailKey = "3n4l1";

    private IAppleAuthManager _authManager;

    public GamePanel game_panel;
    private int point = 0;
    private int index = 0;

    private Star[] availableStars;

    private bool isGuest = true;
    private bool canAdd = true;

    [SerializeField] private GameObject pointText;
    [SerializeField] private GameObject touchPoint;

   private void Start()
   {
        availableStars = new Star[]{
            new Star("Level 1: Solid Star", 20, "solid_star"),
            new Star("Level 2: Gleam Star", 50, "bicolor_star"),
            new Star("Level 3: Radiant Star", 100, "radiant_star")
        };

        if(PlayerPrefs.HasKey(AppleUserIdKey))
        {
           isGuest = false;
           SetUpGamePage();
        }

        else
        {
            if(AppleAuthManager.IsCurrentPlatformSupported)
            {
                var deserialize = new PayloadDeserializer();
                _authManager = new AppleAuthManager(deserialize);
            }

            isGuest = true;
            SetUpGamePage();
        }
   }

   private void Update()
   {
        if(canAdd)
        {
            game_panel.pointSlider.value = (float)point/(float) availableStars[index].neededPoint;
            game_panel.starPoint.text = point + "/" + availableStars[index].neededPoint;
        }
        
   }


   private void checkStarLevel()
   {
        if(point > availableStars[index].neededPoint)
        {

            if(index + 1 < availableStars.Length)
            {
                index += 1;
                 if(pointText)
                {
                    GameObject prefab = Instantiate(pointText, touchPoint.transform);
                    prefab.GetComponentInChildren<TextMesh>().text = "Level up!";
                }  
                game_panel.SetStar(availableStars[index]);
            }

            else
            {
                canAdd = false;
                game_panel.starPoint.text = "MAX";
            }
          
        }
   }

   public void trySigningIn()
   {
        AppleSignIn();
   }

   public void clickOnStar()
   {
        if(canAdd)
        {
            int add = UnityEngine.Random.Range(1, 3);
            point+=add;

            if(pointText)
            {
                GameObject prefab = Instantiate(pointText, touchPoint.transform);
                prefab.GetComponentInChildren<TextMesh>().text = "+" + add;
            }

            checkStarLevel();
            PlayerPrefs.SetInt(UserTotalPoint, point);
        }
        
       
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

                string userPointKey = UserTotalPoint + PlayerPrefs.GetString(AppleUserIdKey);

                PlayerPrefs.SetInt(userPointKey, point);
                PlayerPrefs.DeleteKey(UserTotalPoint);
                isGuest = false;
                SetUpGamePage();

            },
            error =>
            {
                var authorizationErrorCode = error.GetAuthorizationErrorCode();
                Debug.Log("Sign In Failed " + authorizationErrorCode.ToString() + " " + error.ToString());
                isGuest = true;
                SetUpGamePage();
            });
    }

   private void SetUpGamePage()
   {

        switch(isGuest)
        {
            case true:

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

            if(_authManager == null)
            {
                game_panel.SetAppleLoginAvailability(false);
            }

            else if(_authManager != null)
            {
                game_panel.SetAppleLoginAvailability(true);
            }

            game_panel.SetPlayerName("Guest");
            game_panel.SetUpAppleID(false, string.Empty);
            game_panel.SetEmail(false, string.Empty);
            return;

            case false:

            string userPointKey = UserTotalPoint + PlayerPrefs.GetString(AppleUserIdKey);

             if(PlayerPrefs.HasKey(userPointKey))
            {
                point = PlayerPrefs.GetInt(userPointKey);

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

            if(PlayerPrefs.HasKey(UserNameKey))
            {
                game_panel.SetPlayerName(PlayerPrefs.GetString(UserNameKey));
               
            }

            else
            {
                game_panel.SetPlayerName("User");
            }

            if(PlayerPrefs.HasKey(EmailKey))
            {
                game_panel.SetEmail(true, PlayerPrefs.GetString(EmailKey));
            }

            else
            {
                game_panel.SetEmail(true, "Undisclosed in quick login attempt");
            }

            game_panel.SetUpAppleID(true, PlayerPrefs.GetString(AppleUserIdKey));
            game_panel.SetAppleLoginAvailability(false);
            return;

        }
   }
}
