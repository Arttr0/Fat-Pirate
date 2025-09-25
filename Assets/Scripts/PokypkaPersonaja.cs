using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using Unity.VisualScripting;

public class PokypkaPersonaja : MonoBehaviour
{
    public GameObject gridPanel;  // Панель с GridLayoutGroup
    public GameObject prefab1;    // Префаб для персонажа 1
    public GameObject prefab2;    // Префаб для персонажа 2
    public GameObject prefab3;    // Префаб для персонажа 3

    public TextMeshProUGUI coinText;   // Текст для отображения монет игрока
    public Button buyButton1;     // Кнопка для покупки персонажа 1
    public Button buyButton2;     // Кнопка для покупки персонажа 2
    public Button buyButton3;     // Кнопка для покупки персонажа 3

    private int playerCoins;             // Количество монет игрока

    // Стоимость персонажей
    private int characterCost1 = 100;
    private int characterCost2 = 200;
    private int characterCost3 = 300;

    // Список купленных персонажей
    private List<int> boughtCharacterIds = new List<int>();

    // Счетчики для уникальных ID каждого персонажа
    private int characterIdCounter1 = 1;   // ID для персонажа 1
    private int characterIdCounter2 = 100; // ID для персонажа 2
    private int characterIdCounter3 = 200; // ID для персонажа 3

    public GameObject PopUpMoney;
    private void Update()
    {
        playerCoins = PlayerPrefs.GetInt("PlayerCoins", 0);
        UpdateCoinDisplay();
    }
    void Start()
    {
        // Загружаем количество монет игрока из PlayerPrefs
        playerCoins = PlayerPrefs.GetInt("PlayerCoins", 0);
        UpdateCoinDisplay();  // Обновляем отображение монет

        // Загружаем список купленных персонажей из PlayerPrefs
        LoadBoughtCharacters();

        // Назначаем функции для кнопок
        buyButton1.onClick.AddListener(() => TryBuyCharacter(1, characterCost1));
        buyButton2.onClick.AddListener(() => TryBuyCharacter(2, characterCost2));
        buyButton3.onClick.AddListener(() => TryBuyCharacter(3, characterCost3));

        // Добавляем купленных персонажей в сетку
        AddBoughtCharactersToGrid();
    }

    // Обновление отображения монет на UI
    void UpdateCoinDisplay()
    {
        coinText.text = playerCoins.ToString();
    }

    // Метод для попытки купить персонажа
    void TryBuyCharacter(int characterNumber, int cost)
    {
        // += 1000;
        if (playerCoins >= cost)
        {
            // Если монет достаточно, покупаем персонажа
            playerCoins -= cost;  // Списываем монеты
            PlayerPrefs.SetInt("PlayerCoins", playerCoins);  // Сохраняем новый баланс монет в PlayerPrefs
            // Добавляем персонажа в список купленных
            AddCharacterToGrid(characterNumber);

            // Обновляем отображение монет
            UpdateCoinDisplay();
        }
        else
        {
            PopUpMoney.SetActive(true);
            // Если монет не хватает
            Debug.Log("Недостаточно монет для покупки персонажа " + characterNumber);
        }
    }

    // Добавление персонажа в сетку
    public void AddCharacterToGrid(int characterNumber)
    {
        GameObject prefabToUse = null;
        string characterName = "";
        int uniqueId = 0;

        // Выбираем соответствующий префаб, имя и уникальный ID в зависимости от номера персонажа
        if (characterNumber == 1)
        {
            prefabToUse = prefab1;
            characterName = "Personaj1";
            uniqueId = characterIdCounter1++; // Уникальный ID для первого персонажа
        }
        else if (characterNumber == 2)
        {
            prefabToUse = prefab2;
            characterName = "Personaj2";
            uniqueId = characterIdCounter2++; // Уникальный ID для второго персонажа
        }
        else if (characterNumber == 3)
        {
            prefabToUse = prefab3;
            characterName = "Personaj3";
            uniqueId = characterIdCounter3++; // Уникальный ID для третьего персонажа
        }
        else
        {
            Debug.LogWarning("Неизвестный номер персонажа: " + characterNumber);
            return;  // Возвращаемся, если номер персонажа не совпал
        }

        // Добавляем ID в список купленных персонажей
        if (!boughtCharacterIds.Contains(uniqueId))
        {
            boughtCharacterIds.Add(uniqueId);
        }

        // Сохраняем список купленных персонажей в PlayerPrefs
        SaveBoughtCharacters();

        // Создаем новый объект кнопки для выбранного персонажа
        GameObject newButton = Instantiate(prefabToUse);

        // Устанавливаем уникальный ID для кнопки персонажа
        CharacterButton characterButton = newButton.GetComponent<CharacterButton>();
        if (characterButton != null)
        {
            characterButton.characterId = uniqueId;  // Устанавливаем уникальный ID
        }

        // Устанавливаем текст на кнопке
        TextMeshProUGUI buttonText = newButton.GetComponentInChildren<TextMeshProUGUI>();
        if (buttonText != null)
        {
            buttonText.text = characterName;
        }

        // Добавляем кнопку в GridLayoutGroup
        newButton.transform.SetParent(gridPanel.transform, false);
    }

    // Метод для сохранения купленных персонажей в PlayerPrefs
    void SaveBoughtCharacters()
    {
        for (int i = 0; i < boughtCharacterIds.Count; i++)
        {
            PlayerPrefs.SetInt("BoughtCharacter_" + i, boughtCharacterIds[i]);
            Debug.Log("Saved character ID: " + boughtCharacterIds[i]);  // Выводим ID сохранённого персонажа в консоль
        }

        PlayerPrefs.SetInt("BoughtCharacterCount", boughtCharacterIds.Count);
    }

    // Метод для загрузки купленных персонажей из PlayerPrefs
    void LoadBoughtCharacters()
    {
        int count = PlayerPrefs.GetInt("BoughtCharacterCount", 0);

        boughtCharacterIds.Clear();

        for (int i = 0; i < count; i++)
        {
            int characterId = PlayerPrefs.GetInt("BoughtCharacter_" + i);
            boughtCharacterIds.Add(characterId);
        }
    }

    // Добавление купленных персонажей в сетку
    void AddBoughtCharactersToGrid()
    {
        for (int i = 0; i < boughtCharacterIds.Count; i++)
        {
            // Вычисляем номер персонажа по ID
            int characterNumber = GetCharacterNumberById(boughtCharacterIds[i]);
            if (characterNumber != -1)
            {
                AddCharacterToGrid(characterNumber);  // Добавляем персонажа в сетку по его номеру
            }
            else
            {
                Debug.LogWarning("Неизвестный ID персонажа: " + boughtCharacterIds[i]);
            }
        }
    }

    // Метод для получения номера персонажа по его уникальному ID
    int GetCharacterNumberById(int uniqueId)
    {
        if (uniqueId == characterIdCounter1 || uniqueId < characterIdCounter2)
        {
            return 1;  // Персонаж 1
        }
        else if (uniqueId >= characterIdCounter2 && uniqueId < characterIdCounter3)
        {
            return 2;  // Персонаж 2
        }
        else if (uniqueId >= characterIdCounter3)
        {
            return 3;  // Персонаж 3
        }
        return -1;  // Неизвестный ID
    }
}
