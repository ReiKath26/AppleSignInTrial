using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AppleAuth;
using AppleAuth.Enums;
using AppleAuth.Extensions;
using AppleAuth.Interfaces;
using AppleAuth.Native;
using UnityEngine.SceneManagement;

public class SignInManager : MonoBehaviour
{
    private const string AppleUserIdKey = "1dCu55en74ppl3";
    private const string UserNameKey = "u53rm4n3";
    private const string EmailKey = "3n4l1";

    private IAppleAuthManager _authManager;

    public SignInPanel sign_in_panel;
  

    private void Start()
    {
        //if current platform supported
        if(AppleAuthManager.IsCurrentPlatformSupported)
        {
            var deserialize = new PayloadDeserializer();
            _authManager = new AppleAuthManager(deserialize);
        }

        initializeSignIn();
    }

    private void Update()
    {
        //update authmanager
        if(_authManager != null)
        {
            _authManager.Update();
        }

        sign_in_panel.ShowLoadingAnimation(Time.deltaTime);
    }

    private void initializeSignIn()
    {
        //if device doesn't support
        if(_authManager == null)
        {
            SetUpUnavailableLogin();
            return;
        }

        //if credentials revoked
        _authManager.SetCredentialsRevokedCallback(result =>
        {
            SetUpErrorMessage("Received revoked callback" + result);
            SetUpAvailableLogin();
            PlayerPrefs.DeleteKey(AppleUserIdKey);
        });

        if(PlayerPrefs.HasKey(AppleUserIdKey))
        {
            var storedID = PlayerPrefs.GetString(AppleUserIdKey);
            CheckCredentials(storedID);
        }

        else
        {
            QuickLoginAttempt();
        }
    }

    private void SetUpUnavailableLogin()
    {
        sign_in_panel.SetUpSignInAvailability(false, false);
        sign_in_panel.SetLoadingMessage(true, "Apple Sign In Unsupported");
    }

    private void SetUpAvailableLogin()
    {
        sign_in_panel.SetUpSignInAvailability(true, true);
        sign_in_panel.SetLoadingMessage(false, string.Empty);
    }

    private void SetUpErrorMessage(string errorMessage)
    {
        sign_in_panel.ShowErrorMessage(errorMessage);
        StartCoroutine(DisplayErrorMessage());
    }

    IEnumerator DisplayErrorMessage()
    {
        sign_in_panel.ErrorMessageHolder.SetActive(true);
        yield return new WaitForSeconds(3f);
        sign_in_panel.ErrorMessageHolder.SetActive(false);
    }

    public void PlayAsGuest()
    {
        PlayerPrefs.DeleteKey(AppleUserIdKey);
        SceneManager.LoadScene(1);
    }

    public void TryPlayAuthorized()
    {
        sign_in_panel.SetUpSignInAvailability(true, false);
        sign_in_panel.SetLoadingMessage(true, "Signing in...");
        AppleSignIn();
    }

    private void CheckCredentials(string appleID)
    {
        sign_in_panel.SetUpSignInAvailability(true, false);
        sign_in_panel.SetLoadingMessage(true, "Checking credentials...");
        _authManager.GetCredentialState( appleID,
        state =>
        {
            switch(state)
            {
                case CredentialState.Authorized:
                PlayerPrefs.SetString(AppleUserIdKey, appleID);
                SceneManager.LoadScene(1);
                return;

                case CredentialState.Revoked:
                case CredentialState.NotFound:
                SetUpAvailableLogin();
                PlayerPrefs.DeleteKey(AppleUserIdKey);
                return;
            }
        },
        error =>
        {
            var authorizationErrorCode = error.GetAuthorizationErrorCode();
            SetUpAvailableLogin();
            SetUpErrorMessage("Check Credentials Failed " + authorizationErrorCode.ToString() + " " + error.ToString());
        });
    }

    private void QuickLoginAttempt()
    {
        sign_in_panel.SetUpSignInAvailability(true, false);
        sign_in_panel.SetLoadingMessage(true, "Trying to Log User In...");
        var quickLoginArgs = new AppleAuthQuickLoginArgs();

        this._authManager.QuickLogin(quickLoginArgs, 
            credential =>
            {
                var appleCredential = credential as IAppleIDCredential;
                if(appleCredential != null)
                {
                    PlayerPrefs.SetString(AppleUserIdKey, credential.User);
                }

                SceneManager.LoadScene(1);
            },
            error =>
            {
                var authorizationErrorCode = error.GetAuthorizationErrorCode();
                SetUpAvailableLogin();
                SetUpErrorMessage("Quick Login Failed " + authorizationErrorCode.ToString() + " " + error.ToString());
            });
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

                    if(appleCredential.Email != null)
                    {
                        PlayerPrefs.SetString(EmailKey, appleCredential.Email);
                    }

                }

                SceneManager.LoadScene(1);

            },
            error =>
            {
                var authorizationErrorCode = error.GetAuthorizationErrorCode();
                SetUpAvailableLogin();
                SetUpErrorMessage("Sign In Failed " + authorizationErrorCode.ToString() + " " + error.ToString());
            });
    }
}

