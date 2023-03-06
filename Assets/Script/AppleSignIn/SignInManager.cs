using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AppleAuth;
using AppleAuth.Enums;
using AppleAuth.Extensions;
using AppleAuth.Interfaces;
using AppleAuth.Native;

public class SignInManager : MonoBehaviour
{
    private const string AppleUserIdKey = "1dCu55en74ppl3";

    private IAppleAuthManager _authManager;

    public SignInPanel sign_in_panel;
    public GamePanel game_panel;

    public enum status
    {
        unavailable, available, checkingCredentials, quickLogin, authorized, signingIn, error
    }


    private void Start()
    {
        if(AppleAuthManager.IsCurrentPlatformSupported)
        {
            var deserialize = new PayloadDeserializer();
            this._authManager = new AppleAuthManager(deserialize);
        }
    }

    private void Update()
    {
        if(this._authManager != null)
        {
            this._authManager.Update();
        }

        this.sign_in_panel.ShowLoadingAnimation(Time.deltaTime);
    }

    private void SetUpMenu()
    {

    }

    private void playAsGuest()
    {

    }

    private void playAuthorized()
    {

    }
}

