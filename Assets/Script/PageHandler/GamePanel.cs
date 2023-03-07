using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using AppleAuth.Enums;
using AppleAuth.Extensions;
using AppleAuth.Interfaces;
using TMPro;

[System.Serializable]
public class GamePanel
{
   public TextMeshProUGUI userName;
   public Button star;
   public TextMeshProUGUI starName;
   public Slider pointSlider;
   public TextMeshProUGUI starPoint;
   public GameObject appleIDHolder;
   public TextMeshProUGUI appleID;
   public GameObject guestTryLogIn;

   public void SetPlayerName(string name)
   {
      userName.text = name + "'s Star";
   }

   public void SetUpAppleID(bool visibility, string id)
   {
      appleIDHolder.SetActive(visibility);
      if(appleIDHolder.activeSelf)
      {
         appleID.text = "Apple ID: " + id;
      }
   }

   public void SetTryLoginVisibility(bool visibility)
   {
      guestTryLogIn.SetActive(visibility);
   }



}
