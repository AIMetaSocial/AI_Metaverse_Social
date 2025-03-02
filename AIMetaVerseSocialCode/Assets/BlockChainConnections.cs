using Cysharp.Threading.Tasks;
using Defective.JSON;
using System;
using System.Collections.Generic;
using Thirdweb;
using UnityEngine;

public class BlockChainConnections : MonoBehaviour
{

    public static BlockChainConnections Instance;
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(this.gameObject);
        }

        Screen.sleepTimeout = SleepTimeout.NeverSleep;

    }
    // Start is called bef
    //
    // ore the first frame update

    public const string abi = "[{\"inputs\":[],\"stateMutability\":\"nonpayable\",\"type\":\"constructor\"},{\"anonymous\":false,\"inputs\":[{\"indexed\":true,\"internalType\":\"address\",\"name\":\"owner\",\"type\":\"address\"},{\"indexed\":true,\"internalType\":\"address\",\"name\":\"approved\",\"type\":\"address\"},{\"indexed\":true,\"internalType\":\"uint256\",\"name\":\"tokenId\",\"type\":\"uint256\"}],\"name\":\"Approval\",\"type\":\"event\"},{\"anonymous\":false,\"inputs\":[{\"indexed\":true,\"internalType\":\"address\",\"name\":\"owner\",\"type\":\"address\"},{\"indexed\":true,\"internalType\":\"address\",\"name\":\"operator\",\"type\":\"address\"},{\"indexed\":false,\"internalType\":\"bool\",\"name\":\"approved\",\"type\":\"bool\"}],\"name\":\"ApprovalForAll\",\"type\":\"event\"},{\"inputs\":[{\"internalType\":\"address\",\"name\":\"to\",\"type\":\"address\"},{\"internalType\":\"uint256\",\"name\":\"tokenId\",\"type\":\"uint256\"}],\"name\":\"approve\",\"outputs\":[],\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"anonymous\":false,\"inputs\":[{\"indexed\":true,\"internalType\":\"address\",\"name\":\"owner\",\"type\":\"address\"},{\"indexed\":true,\"internalType\":\"uint256\",\"name\":\"tokenId\",\"type\":\"uint256\"},{\"indexed\":false,\"internalType\":\"string\",\"name\":\"tokenURI\",\"type\":\"string\"}],\"name\":\"Minted\",\"type\":\"event\"},{\"inputs\":[{\"internalType\":\"string\",\"name\":\"tokenURI\",\"type\":\"string\"}],\"name\":\"mintNFT\",\"outputs\":[{\"internalType\":\"uint256\",\"name\":\"\",\"type\":\"uint256\"}],\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"anonymous\":false,\"inputs\":[{\"indexed\":true,\"internalType\":\"address\",\"name\":\"previousOwner\",\"type\":\"address\"},{\"indexed\":true,\"internalType\":\"address\",\"name\":\"newOwner\",\"type\":\"address\"}],\"name\":\"OwnershipTransferred\",\"type\":\"event\"},{\"inputs\":[{\"internalType\":\"uint256\",\"name\":\"_itemId\",\"type\":\"uint256\"}],\"name\":\"purchaseCoins\",\"outputs\":[],\"stateMutability\":\"payable\",\"type\":\"function\"},{\"inputs\":[],\"name\":\"renounceOwnership\",\"outputs\":[],\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"address\",\"name\":\"from\",\"type\":\"address\"},{\"internalType\":\"address\",\"name\":\"to\",\"type\":\"address\"},{\"internalType\":\"uint256\",\"name\":\"tokenId\",\"type\":\"uint256\"}],\"name\":\"safeTransferFrom\",\"outputs\":[],\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"address\",\"name\":\"from\",\"type\":\"address\"},{\"internalType\":\"address\",\"name\":\"to\",\"type\":\"address\"},{\"internalType\":\"uint256\",\"name\":\"tokenId\",\"type\":\"uint256\"},{\"internalType\":\"bytes\",\"name\":\"_data\",\"type\":\"bytes\"}],\"name\":\"safeTransferFrom\",\"outputs\":[],\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"address\",\"name\":\"operator\",\"type\":\"address\"},{\"internalType\":\"bool\",\"name\":\"approved\",\"type\":\"bool\"}],\"name\":\"setApprovalForAll\",\"outputs\":[],\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"anonymous\":false,\"inputs\":[{\"indexed\":true,\"internalType\":\"address\",\"name\":\"from\",\"type\":\"address\"},{\"indexed\":true,\"internalType\":\"address\",\"name\":\"to\",\"type\":\"address\"},{\"indexed\":true,\"internalType\":\"uint256\",\"name\":\"tokenId\",\"type\":\"uint256\"}],\"name\":\"Transfer\",\"type\":\"event\"},{\"inputs\":[{\"internalType\":\"address\",\"name\":\"from\",\"type\":\"address\"},{\"internalType\":\"address\",\"name\":\"to\",\"type\":\"address\"},{\"internalType\":\"uint256\",\"name\":\"tokenId\",\"type\":\"uint256\"}],\"name\":\"transferFrom\",\"outputs\":[],\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"address\",\"name\":\"newOwner\",\"type\":\"address\"}],\"name\":\"transferOwnership\",\"outputs\":[],\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"anonymous\":false,\"inputs\":[{\"indexed\":true,\"internalType\":\"address\",\"name\":\"from\",\"type\":\"address\"},{\"indexed\":true,\"internalType\":\"address\",\"name\":\"to\",\"type\":\"address\"},{\"indexed\":true,\"internalType\":\"uint256\",\"name\":\"tokenId\",\"type\":\"uint256\"}],\"name\":\"Transferred\",\"type\":\"event\"},{\"inputs\":[{\"internalType\":\"address\",\"name\":\"_recipient\",\"type\":\"address\"}],\"name\":\"withdraw\",\"outputs\":[],\"stateMutability\":\"payable\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"address\",\"name\":\"owner\",\"type\":\"address\"}],\"name\":\"balanceOf\",\"outputs\":[{\"internalType\":\"uint256\",\"name\":\"\",\"type\":\"uint256\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"uint256\",\"name\":\"tokenId\",\"type\":\"uint256\"}],\"name\":\"getApproved\",\"outputs\":[{\"internalType\":\"address\",\"name\":\"\",\"type\":\"address\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[],\"name\":\"getCurrentTime\",\"outputs\":[{\"internalType\":\"uint256\",\"name\":\"\",\"type\":\"uint256\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"address\",\"name\":\"owner\",\"type\":\"address\"}],\"name\":\"getOwnedNFTs\",\"outputs\":[{\"internalType\":\"uint256[]\",\"name\":\"\",\"type\":\"uint256[]\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"address\",\"name\":\"owner\",\"type\":\"address\"}],\"name\":\"getOwnedNFTURIs\",\"outputs\":[{\"internalType\":\"string[]\",\"name\":\"\",\"type\":\"string[]\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"address\",\"name\":\"owner\",\"type\":\"address\"},{\"internalType\":\"address\",\"name\":\"operator\",\"type\":\"address\"}],\"name\":\"isApprovedForAll\",\"outputs\":[{\"internalType\":\"bool\",\"name\":\"\",\"type\":\"bool\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[],\"name\":\"name\",\"outputs\":[{\"internalType\":\"string\",\"name\":\"\",\"type\":\"string\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[],\"name\":\"owner\",\"outputs\":[{\"internalType\":\"address\",\"name\":\"\",\"type\":\"address\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"uint256\",\"name\":\"tokenId\",\"type\":\"uint256\"}],\"name\":\"ownerOf\",\"outputs\":[{\"internalType\":\"address\",\"name\":\"\",\"type\":\"address\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"bytes4\",\"name\":\"interfaceId\",\"type\":\"bytes4\"}],\"name\":\"supportsInterface\",\"outputs\":[{\"internalType\":\"bool\",\"name\":\"\",\"type\":\"bool\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[],\"name\":\"symbol\",\"outputs\":[{\"internalType\":\"string\",\"name\":\"\",\"type\":\"string\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"uint256\",\"name\":\"tokenId\",\"type\":\"uint256\"}],\"name\":\"tokenURI\",\"outputs\":[{\"internalType\":\"string\",\"name\":\"\",\"type\":\"string\"}],\"stateMutability\":\"view\",\"type\":\"function\"}]";


    // address of contract
    public static string contractAdd = "0x85fb57bF40Dd60d03D79Eff5aa2C35Dbfd44D033";



    //string chainId = "97";
    //string networkRPC = "https://bsc-testnet-dataseed.bnbchain.org";
    //string contractAdd = "0x9163154758495150E10ac3AA3D9feA25B4023E32";
    Contract contract;
    public async void Start()
    {
        ThirdwebManager.Instance.Initialize("1313161555");





    }

    string address;
    CurrencyValue balanace;


    private void Update()
    {
        // if (Input.GetKeyDown(KeyCode.A)) ActionTake(0);
        // if (Input.GetKeyDown(KeyCode.B)) ActionTake(1);
        // if (Input.GetKeyDown(KeyCode.C)) ActionTake(2);
        // if (Input.GetKeyDown(KeyCode.D)) ActionTake(3);
        // if (Input.GetKeyDown(KeyCode.E)) ActionTake(4);
        // if (Input.GetKeyDown(KeyCode.F)) ActionTake(5);
        // if (Input.GetKeyDown(KeyCode.G)) ActionTake(6);
    }
    public async void ActionTake(int _no)
    {
        return;
        // Break;
        if (!await ThirdwebManager.Instance.SDK.Wallet.IsConnected())
        {
            // updater.text = "Not Connected yet";
            return;
        }

        switch (_no)
        {
            case 0:
                address = await ThirdwebManager.Instance.SDK.Wallet.GetAddress();
                // updater.text = address;
                break;
            case 1:
                balanace = await ThirdwebManager.Instance.SDK.Wallet.GetBalance();
                // updater.text = balanace.displayValue;
                break;
            case 2:
                /*  contract = ThirdwebManager.Instance.SDK.GetContract(contractAdd, abi);
                  var test = await contract.Read<int>("raceStatus");
                  updater.text = test.ToString();
                  break;*/
                // array of arguments for contract

                //updater.text = test2.ToString();
                break;
            case 3:
                // get current timestamp
                int timestamp = (int)(System.DateTime.UtcNow.Subtract(new System.DateTime(1970, 1, 1))).TotalSeconds;
                // set expiration time
                int expirationTime = timestamp + 60;
                // set message
                string message = "Welcome to AI Metaverse Social\n" + expirationTime.ToString();
                var balanaceuo = await ThirdwebManager.Instance.SDK.Wallet.Sign(message);
                //updater.text = balanaceuo;
                break;
            case 4:
                // array of arguments for contract
                object[] inputParams1 = { address };
                contract = ThirdwebManager.Instance.SDK.GetContract(contractAdd, abi);

                var test3 = await contract.Read<int[]>("getOwnedNFTs", inputParams1);


                for (int i = 0; i < test3.Length; i++)
                {
                    Debug.Log("Value " + test3[i]);
                }
                // updater.text = test3.ToString();
                break;
            case 5:




                break;
            case 6:
                /*                float _amount2 = 0.00001f;
                                float decimals2 = 1000000000000000000; // 18 decimals
                                float wei2 = _amount2 * decimals2;
                                string value2 = Convert.ToDecimal(wei2).ToString();
                                // array of arguments for contract
                                object[] inputParams3 = {  };
                                contract = ThirdwebManager.Instance.SDK.GetContract(contractAdd, abi);

                                var test4 = await contract.Write("MintRewardToken", inputParams3);
                                updater.text = test4.ToString();
                                break;*/


                // array of arguments for contract
                string jsonData = "https://metaverseaigame.fun/aigeneration/json/NFT_67bedbf28fb97.json";
                object[] inputParams3 = { jsonData };
                contract = ThirdwebManager.Instance.SDK.GetContract(contractAdd, abi);

                var test5 = await contract.Write("mintNFT", inputParams3);


                //string jsonData = "https://metaverseaigame.fun/aigeneration/json/NFT_67bedbf28fb97.json";

                // updater.text = test5.ToString();
                break;
            case 7:

                break;

        }

    }

    public async UniTask<bool> Mint(string jsonURL)
    {
        try
        {
            string jsonData = jsonURL;
            object[] inputParams3 = { jsonData };
            contract = ThirdwebManager.Instance.SDK.GetContract(contractAdd, abi);

            var test5 = await contract.Write("mintNFT", inputParams3);
            Debug.Log("MINT SUCCESS STATUS" + test5.receipt.status);

            return test5.receipt.status == 1;
        }
        catch
        {
            return false;
        }
    }

    public float[] coinPackAmount;

    public async UniTask<bool> purchaseCoins(int _no)
    {
        try
        {
            //float _amount = 0.00001f;
            float _amount = coinPackAmount[_no];
            float decimals = 1000000000000000000; // 18 decimals
            float wei = _amount * decimals;
            string value = Convert.ToDecimal(wei).ToString();
            // array of arguments for contract
            object[] inputParams2 = { 0 };
            contract = ThirdwebManager.Instance.SDK.GetContract(contractAdd, abi);

            var test4 = await contract.Write("purchaseCoins", new TransactionRequest() { value = Convert.ToDecimal(wei).ToString() }, inputParams2);
            //updater.text = test4.ToString();

            Debug.Log("Status 1 " + test4.receipt.status);
            Debug.Log("Status 01 " + test4.receipt.confirmations);
            /*
                            await UniTask.Delay(5000);
                            var check = await Transaction.WaitForTransactionResult(test4.receipt.transactionHash, await ThirdwebManager.Instance.SDK.Wallet.GetChainId());
                            Debug.Log("Status 2 " + test4.receipt.status);
                            Debug.Log("Status 02 " + test4.receipt.confirmations);*/
            if (test4.receipt.status.ToString() == "1") return true;
            return false;
        }
        catch
        {
            return false;
        }
    }

    public async UniTask<List<int>> GetMyNFT_UID()
    {
        try
        {

            object[] inputParams = { LoginManager.address };
            contract = ThirdwebManager.Instance.SDK.GetContract(contractAdd, abi);

            contract = ThirdwebManager.Instance.SDK.GetContract(contractAdd, abi);

            var test3 = await contract.Read<int[]>("getOwnedNFTs", inputParams);

            List<int> listOfUID = new List<int>();

            for (int i = 0; i < test3.Length; i++)
            {
                listOfUID.Add(test3[i]);
            }

            return listOfUID;
        }
        catch
        {
            return null;
        }

    }

    public async UniTask<List<string>> GetMyNFTS()
    {
        try
        {

            object[] inputParams = { LoginManager.address };
            contract = ThirdwebManager.Instance.SDK.GetContract(contractAdd, abi);

            var test2 = await contract.Read<string[]>("getOwnedNFTURIs", inputParams);
            List<string> jsonURLS = new List<string>();
            for (int i = 0; i < test2.Length; i++)
            {
                try
                {
                    JSONObject jSONObject = new JSONObject(test2[i]);
                    //  Debug.Log("Value " + test2[i]);
                    //jsonURLS.Add(jSONObject.GetField("json_url").stringValue);
                    jsonURLS.Add(test2[i]);
                }
                catch
                {
                    continue;
                }
            }

            return jsonURLS;

        }
        catch
        {
            return null;
        }
    }

    public async UniTask<bool> TransferNFT(string _toID, int uid)
    {
        try
        {
            object[] inputParams3 = { LoginManager.address, _toID, uid };
            contract = ThirdwebManager.Instance.SDK.GetContract(contractAdd, abi);
            var test5 = await contract.Write("transferFrom", inputParams3);

            Debug.Log("Status 1 " + test5.receipt.status);
            Debug.Log("Status 01 " + test5.receipt.confirmations);

            return test5.receipt.status == 1;
        }
        catch
        {
            return false;
        }
    }



}
