using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

[System.Serializable]
public class SignInPanel
{
   public GameObject SignInWithAppleHolder;
   public Button SignInWithAppleButton;
   public GameObject LoadingMessageHolder;
   public Transform LoadingIcon;
   public TextMeshProUGUI LoadingMessage;
   public GameObject ErrorMessageHolder;
   public TextMeshProUGUI ErrorMessage;

   public void SetUpSignInAvailability(bool visibility, bool enabler)
   {
        this.SignInWithAppleHolder.SetActive(visibility);
        this.SignInWithAppleButton.enabled = enabler;
   }

   public void SetLoadingMessage(bool visibility, string message)
   {
         this.LoadingMessageHolder.SetActive(visibility);

         if(this.LoadingMessageHolder.activeSelf)
         {
            this.LoadingMessage.text = message;
         }
   }

   public void ShowErrorMessage(string message)
   {
        if(this.LoadingMessageHolder.activeSelf)
        {
            this.LoadingMessageHolder.SetActive(false);
        }

        this.ErrorMessage.text = message;
   }

  
   public void ShowLoadingAnimation(float deltaTime)
   {
        if(!this.LoadingMessageHolder.activeSelf)
        {
            return;
        }

        this.LoadingIcon.Rotate(0.0f, 0.0f, -360.0f * deltaTime);
   }
}
