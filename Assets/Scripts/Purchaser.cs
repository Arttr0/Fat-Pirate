using TMPro;
using UnityEngine;
using UnityEngine.Purchasing;

public class Purchaser : MonoBehaviour
{
    int playerCoins;
    public GameObject PopUpPanel;
    public TextMeshProUGUI PopUpText;
    public TextMeshProUGUI PlayerCoinsText;
    public void OnPurchaseCompleted(Product product)
    {
        switch (product.definition.id)
        {
            case "com.mainframe.FatPirate.200monet":
                Add200Coins();
                break;
            case "com.mainframe.FatPirate.500monet":
                Add500Coins();
                break;
            case "com.mainframe.FatPirate.1200monet":
                Add1200Coins();
                break;
        }
    }
    public void OnPurchaseFailid(Product product, PurchaseFailureReason reason)
    {
        string errorMessage = $"Purchase failed for product {product.definition.id}. Failure reason: {reason}";
        Debug.Log(errorMessage);

        switch (reason)
        {
            case PurchaseFailureReason.PurchasingUnavailable:
                string msg1 = "Purchasing is unavailable, please check your internet connection.";
                Debug.LogWarning(msg1);
                ShowPopUp(msg1);
                break;
            case PurchaseFailureReason.ExistingPurchasePending:
                string msg2 = "There is already a pending purchase.";
                Debug.LogWarning(msg2);
                ShowPopUp(msg2);
                break;
            case PurchaseFailureReason.ProductUnavailable:
                string msg3 = "The product is unavailable.";
                Debug.LogWarning(msg3);
                ShowPopUp(msg3);
                break;
            case PurchaseFailureReason.PaymentDeclined:
                string msg4 = "The payment was declined.";
                Debug.LogWarning(msg4);
                ShowPopUp(msg4);
                break;
            case PurchaseFailureReason.DuplicateTransaction:
                string msg5 = "Duplicate transaction detected.";
                Debug.LogWarning(msg5);
                ShowPopUp(msg5);
                break;
            default:
                string msg6 = "An unknown error occurred during the purchase.";
                Debug.LogWarning(msg6);
                ShowPopUp(msg6);
                break;
        }
    }
    public void ShowPopUp(string text) {
        PopUpText.text = text;
        PopUpPanel.SetActive(true);
    }
    public void ClosePopUp() {
        PopUpPanel.SetActive(false);
    }

    private void Add200Coins()
    {
        playerCoins = PlayerPrefs.GetInt("PlayerCoins", 0);
        playerCoins += 200;
        PlayerCoinsText.text = playerCoins.ToString();
        PlayerPrefs.SetInt("PlayerCoins", playerCoins);
        Debug.Log("Purchase: get 200 coins");
    }

    private void Add500Coins()
    {
        playerCoins = PlayerPrefs.GetInt("PlayerCoins", 0);
        playerCoins += 500;
        PlayerCoinsText.text = playerCoins.ToString();
        PlayerPrefs.SetInt("PlayerCoins", playerCoins);
        Debug.Log("Purchase: get 500 coins");
    }
    private void Add1200Coins()
    {
        playerCoins = PlayerPrefs.GetInt("PlayerCoins", 0);
        playerCoins += 1200;
        PlayerCoinsText.text = playerCoins.ToString();
        PlayerPrefs.SetInt("PlayerCoins", playerCoins);
        Debug.Log("Purchase: get 1200 coins");
    }
}