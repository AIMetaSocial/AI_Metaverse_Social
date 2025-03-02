using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Thirdweb;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoginManager : MonoBehaviour
{

    public static string address;
    public async void LoginSuccess(){

        address = await ThirdwebManager.Instance.SDK.Wallet.GetAddress();
        //updater.text = address;
        
        Debug.Log(address);
        PlayerPrefs.SetString("Account",address);
        DatabaseManager.Instance.GetData();
        await UniTask.Delay(2000);
        SceneManager.LoadScene("MainMenu");
    }
}
