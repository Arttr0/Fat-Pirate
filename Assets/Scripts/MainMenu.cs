using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
//using UnityEngine.iOS;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class MainMenu : MonoBehaviour
{
    public GameObject Settings;
    public GameObject Shop;
    public GameObject DailyRewards;
    public GameObject Leaderboard;
    public GameObject SquadOn;
    // Start is called before the first frame update
    void Start()
    {
        HideAllPanels();
    }
    private string rateUsUrl = "https://apps.apple.com/app/id6738912190";
    private string bugReportEmail = "email@gmail.com"; // Замените на ваш email
    private string bugReportSubject = "Bug Report for Fat Pirate"; // Тема письма
    private string bugReportBody = "Please describe the bug here..."; // Тело письма
    public void RateUs() {
        //Device.RequestStoreReview();
    }
    public void OnBugReportButtonClicked()
    {
        // Создание URL для отправки email
        string mailtoUrl = "mailto:" + bugReportEmail
                         + "?subject=" + UnityWebRequest.EscapeURL(bugReportSubject)
                         + "&body=" + UnityWebRequest.EscapeURL(bugReportBody);

        // Открыть почтовое приложение с заранее заполненными данными
        Application.OpenURL(mailtoUrl);
    }

    public void ShowPanel(GameObject panel)
    {
        HideAllPanels(); // Скрыть все панели перед показом новой
        panel.SetActive(true); // Активировать переданную панель
        ButtonSound(0);
    }

    public void HideAllPanels()
    {
        ButtonSound(0);
        Settings.SetActive(false);
        Shop.SetActive(false);
        DailyRewards.SetActive(false);
        Leaderboard.SetActive(false);
    }

    public void ShowAllPanels()
    {
        Settings.SetActive(true);
        Shop.SetActive(true);
        DailyRewards.SetActive(true);
        Leaderboard.SetActive(true);
    }

    public void Exit()
    {
        Application.Quit();
    }

    public void ButtonSound(int sound)
    {
        AudioManager.Instance.PlaySFX(sound); // Воспроизводит переденный звук
    }

    public void StartGame() {
        if (ButtonSelection.usedCharacterIds.Count == 0)
        {
            SquadOn.SetActive(true);
        }
        else {
            SceneManager.LoadScene(1);
            SquadOn.SetActive(false);
        }
    }


    // Update is called once per frame
    void Update()
    {

    }
}
