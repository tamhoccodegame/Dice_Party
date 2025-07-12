using System.Collections;
using System.Collections.Generic;
using PlayFab.ClientModels;
using PlayFab;
using TMPro;
using UnityEngine;

public class PlayFabLogin : MonoBehaviour
{
    public TMP_InputField emailInput;
    public TMP_InputField passwordInput;

    public void OnLoginButtonClick()
    {
        var request = new LoginWithEmailAddressRequest
        {
            Email = emailInput.text,
            Password = passwordInput.text
        };

        PlayFabClientAPI.LoginWithEmailAddress(request, OnLoginSuccess, OnLoginFailure);
    }

    void OnLoginSuccess(LoginResult result)
    {
        Debug.Log("Đăng nhập thành công: " + result.PlayFabId);
    }

    void OnLoginFailure(PlayFabError error)
    {
        Debug.LogError(error.GenerateErrorReport());
    }
}
