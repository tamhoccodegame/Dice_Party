using System.Collections;
using System.Collections.Generic;
using PlayFab.ClientModels;
using PlayFab;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class PlayFabRegister : MonoBehaviour
{
    public TMP_InputField emailInput;
    public TMP_InputField passwordInput;
    public TMP_InputField usernameInput;

    public void OnRegisterButtonClick()
    {
        var request = new RegisterPlayFabUserRequest
        {
            Email = emailInput.text,
            Password = passwordInput.text,
            Username = "Player_" + Random.Range(1000, 9999),
            RequireBothUsernameAndEmail = true
        };

        PlayFabClientAPI.RegisterPlayFabUser(request, OnRegisterSuccess, OnRegisterFail);
    }

    void OnRegisterSuccess(RegisterPlayFabUserResult result)
    {
        Debug.Log("Đăng ký thành công!");
    }

    void OnRegisterFail(PlayFabError error)
    {
        Debug.LogError("Lỗi đăng ký: " + error.ErrorMessage);
    }

}
