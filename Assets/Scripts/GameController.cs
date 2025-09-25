using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using System.Collections;

public class GameController : MonoBehaviour
{
    public CharacterManager characterManager;  // Ссылка на CharacterManager
    public EnemyManager enemyManager;          // Ссылка на EnemyManager

    private int currentTurn = 0;               // Индекс текущего хода (0 - персонажи, 1 - враги)
    private bool isGameOver = false;

    private int selectedCharacterId = -1;      // ID выбранного персонажа
    private int selectedEnemyIndex = -1;       // Индекс выбранного врага
    public TextMeshProUGUI ExpText;
    public TextMeshProUGUI ExpText1;
    public TextMeshProUGUI PlayerCoinsText;
    public int Exp;  // Переменная для опыта
    public int PlayerCoins;             // Количество монет игрока
    public int ExpInGame;  // Переменная для опыта за игру
    public int PlayerCoinsInGame;             // Количество монет игрока за игру
    public GameObject YouTurn, EnemiesTurn; // Картинки чей ход
    public GameObject LosePanel, WinPanel;
    public GameObject Podskazka;
    void Start()
    {
        if (PlayerPrefs.GetInt("PodskazkaPokazana", 0) != 1)
        {
            Podskazka.SetActive(true);
        }
        else Podskazka.SetActive(false);
        StartCoroutine(GameLoop());
        Exp = PlayerPrefs.GetInt("Exp", 0);
        PlayerCoins = PlayerPrefs.GetInt("PlayerCoins", 0);
    }

    // Основной цикл игры
    IEnumerator GameLoop()
    {
        ButtonSound(3);
        while (!isGameOver)
        {
            if (currentTurn == 0)
            {
                StartCoroutine(DisableOutlinesWithDelay());
                Debug.Log("Игроки ходят!");
                // Игроки выбирают свои цели
                yield return StartCoroutine(PlayerTurn());
            }
            else
            {
                StartCoroutine(DisableOutlinesWithDelay());
                Debug.Log("Враги ходят!");
                // Враги выбирают свои цели
                yield return StartCoroutine(EnemyTurn());
            }

            // После каждого хода меняем очередь
            currentTurn = 1 - currentTurn;

            // Если персонажи или враги мертвы, то завершение игры
            if (characterManager.AllCharactersDead()){
                LosePanel.SetActive(true);
                ButtonSound(6);
                isGameOver = true;
            } else if (enemyManager.AllEnemiesDead())
            {
                ButtonSound(7);
                Exp += ExpInGame;
                PlayerCoins += PlayerCoinsInGame;
                PlayerPrefs.SetInt("Exp", Exp);
                PlayerPrefs.SetInt("PlayerCoins", PlayerCoins);
                ExpText1.text = ExpInGame.ToString();
                WinPanel.SetActive(true);
                isGameOver = true;
            }
        }
        Debug.Log("Игра окончена!");
    }

    // Ход игрока (персонажи атакуют)
    IEnumerator PlayerTurn()
    {
        YouTurn.SetActive(true);
        yield return new WaitForSeconds(1.0f);
        YouTurn.SetActive(false);
        // Пока не выбран персонаж и враг
        while (selectedCharacterId == -1 || selectedEnemyIndex == -1)
        {
            yield return null;  // Ждем, пока игрок сделает выбор
        }

        // Имитируем атаку выбранного персонажа на выбранного врага
        yield return StartCoroutine(AttackEnemy(selectedCharacterId, selectedEnemyIndex));

        // Сбрасываем выбор
        selectedCharacterId = -1;
        selectedEnemyIndex = -1;
    }

    // Ход врага (враги атакуют)
    IEnumerator EnemyTurn()
    {
        EnemiesTurn.SetActive(true);
        yield return new WaitForSeconds(1.0f);
        EnemiesTurn.SetActive(false);
        // Пауза до того, как враги выберут цели
        yield return new WaitForSeconds(1.0f);

        // Фильтруем врагов с нулевым здоровьем (только живые враги)
        List<int> aliveEnemies = new List<int>();
        for (int i = 0; i < enemyManager.hpSliders.Count; i++)
        {
            if (enemyManager.hpSliders[i].value > 0)  // Если враг жив
            {
                aliveEnemies.Add(i);
            }
        }

        // Если есть живые враги, выбираем случайного для атаки
        if (aliveEnemies.Count > 0)
        {
            // Выбираем случайного врага из списка живых врагов
            int randomEnemyIndex = aliveEnemies[Random.Range(0, aliveEnemies.Count)];

            // Выбираем случайного персонажа для атаки
            List<int> characterIds = characterManager.GetSelectedCharacterIds();
            List<int> aliveCharacters = new List<int>();

            // Собираем список живых персонажей (с их индексами)
            for (int i = 0; i < characterIds.Count; i++)
            {
                if (characterIds[i] != -1) // Если персонаж выбран (не мертв)
                {
                    aliveCharacters.Add(i);
                }
            }

            // Если есть живые персонажи, выбираем случайного
            if (aliveCharacters.Count > 0)
            {
                int randomCharacterIndex = aliveCharacters[Random.Range(0, aliveCharacters.Count)];
                yield return StartCoroutine(AttackPlayer(randomEnemyIndex, randomCharacterIndex));
            }
            else
            {
                Debug.Log("Все персонажи мертвы!");
            }
        }
        else
        {
            Debug.Log("Все враги мертвы!");
        }
    }



    // Функция атаки персонажа на врага
    IEnumerator AttackEnemy(int characterId, int enemyIndex)
    {
        int damage = PlayerPrefs.GetInt("Character_" + characterId + "_damage");
        Slider enemyHpSlider = enemyManager.hpSliders[enemyIndex];

        // Наносим урон врагу
        enemyHpSlider.value -= damage;
        for (int i = 0; i < enemyManager.EnemyOutline.Length; i++)
        {
            if (enemyManager.EnemyOutline[i] == null)
            {
                Debug.LogError("Ошибка: элемент в массиве EnemyOutline не инициализирован.");
                continue; // Пропускаем этот элемент, если он не инициализирован
            }

            // Включаем/выключаем EnemyOutline для выбранного врага
            enemyManager.EnemyOutline[i].enabled = (i == enemyIndex);

            // Изменяем текст в TextMeshPro, если элемент выбран
            if (i == enemyIndex)
            {
                // Пример изменения текста в компоненте TextMeshPro
                enemyManager.Damage[i].text = "-" + damage;
                StartCoroutine(AnimateTextWithFloatEffect(enemyManager.Damage[i]));
            }
            else
            {
                // Возвращаем исходный текст, если элемент не выбран
                enemyManager.Damage[i].text = "";  // Или другой дефолтный текст
            }
        }

        Debug.Log($"Персонаж {characterId} атакует врага {enemyIndex} на {damage} урона!");
        ButtonSound(4);
        // Изменяем иконку персонажа с использованием метода ChangeCharacterIcon
        yield return StartCoroutine(ChangeCharacterIcon(characterId));
        // Если враг умирает
        if (enemyHpSlider.value <= 0)
        {
            ButtonSound(5);
            ExpInGame += 25;
            PlayerCoinsInGame += 25;
            ExpText.text = ExpInGame.ToString();
            PlayerCoinsText.text = PlayerCoinsInGame.ToString();
            enemyManager.DeactivateUIElements(enemyIndex + 1);  // Отключаем UI врага
            Debug.Log($"Враг {enemyIndex} убит!");
        }
        //StartCoroutine(DisableOutlinesWithDelay());
        yield return null;
    }
    IEnumerator AnimateTextWithFloatEffect(TextMeshProUGUI textObject)
    {
        Vector3 originalPosition = textObject.transform.position; // Сохраняем исходную позицию текста
        Vector3 targetPosition = originalPosition + new Vector3(0, 25, 0); // Целевая позиция (поднимем текст по оси Y на 10 единиц)

        float duration = 1f; // Длительность анимации
        float elapsedTime = 0f; // Время, прошедшее с начала анимации

        while (elapsedTime < duration)
        {
            // Линейно изменяем позицию текста с использованием интерполяции
            textObject.transform.position = Vector3.Lerp(originalPosition, targetPosition, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null; // Ждем следующий кадр
        }

        // После завершения анимации вернем текст на исходную позицию
        textObject.transform.position = originalPosition;
    }
    IEnumerator ChangeCharacterIcon(int characterId)
    {
        // Массив для всех кнопок (иконок персонажей)
        Image[] characterButtons = new Image[] { characterManager.buttonIcon1, characterManager.buttonIcon2, characterManager.buttonIcon3, characterManager.buttonIcon4 };

        // Массив для всех типов спрайтов: Strong, Quick, Lasting и их 1-ые версии
        Sprite[][] characterSprites = new Sprite[][]
        {
        new Sprite[] { characterManager.Strong, characterManager.Strong1 },  // для "Strong" и "Strong1"
        new Sprite[] { characterManager.Quick, characterManager.Quick1 },    // для "Quick" и "Quick1"
        new Sprite[] { characterManager.Lasting, characterManager.Lasting1 } // для "Lasting" и "Lasting1"
        };
        // Проверяем, какой тип персонажа и его индекс
        int spriteTypeIndex = -1;

        if (characterId >= 0 && characterId <= 99) spriteTypeIndex = 0;  // Strong
        else if (characterId >= 100 && characterId <= 199) spriteTypeIndex = 1;  // Quick
        else if (characterId >= 200 && characterId <= 299) spriteTypeIndex = 2;  // Lasting

        // Проверяем, попадает ли characterId в корректный диапазон
        if (spriteTypeIndex != -1)
        {
            // Определяем, какой персонаж менять
            int characterIndex = buttonId1;  // Это дает 0 для 0-99, 1 для 100-199, 2 для 200-299

            // Меняем спрайт кнопки персонажа в зависимости от characterId и типа спрайта
            characterButtons[characterIndex].sprite = characterSprites[spriteTypeIndex][1]; // Устанавливаем "X1" (например, Strong1, Quick1, Lasting1)

            yield return new WaitForSeconds(1.0f);  // Пауза на 1 секунду

            characterButtons[characterIndex].sprite = characterSprites[spriteTypeIndex][0]; // Устанавливаем "X" (например, Strong, Quick, Lasting)
        }
        else
        {
            Debug.Log("Некорректный characterId!");
        }
    }


    // Функция атаки врага на персонажа
    // Функция атаки врага на персонажа
    IEnumerator AttackPlayer(int enemyIndex, int characterIndex)
    {
        List<int> characterIds = characterManager.GetSelectedCharacterIds();
        int characterId = characterIds[characterIndex];

        if (characterId == -1) yield break;  // Если персонаж мертв, пропускаем

        int damage = (int)enemyManager.damageSliders[enemyIndex].value; // Приведение float к int
        Slider playerHpSlider = characterManager.hpSliders[characterIndex];

        // Наносим урон персонажу
        playerHpSlider.value -= damage;
        for (int i = 0; i < characterManager.characterOutline.Length; i++)
        {
            if (characterManager.characterOutline[i] == null)
            {
                Debug.LogError("Ошибка: элемент в массиве EnemyOutline не инициализирован.");
                continue; // Пропускаем этот элемент, если он не инициализирован
            }
            characterManager.characterOutline[i].enabled = (i == characterIndex);
            characterManager.Damage[i].enabled = (i == characterIndex);
            if (i == characterIndex)
            {
                // Пример изменения текста в компоненте Damage
                characterManager.Damage[i].text = "-" + damage;
                StartCoroutine(AnimateTextWithFloatEffect(characterManager.Damage[i]));
            }
            else
            {
                // Возвращаем исходный текст, если элемент не выбран
                characterManager.Damage[i].text = "";  // Или другой дефолтный текст
            }
           // characterManager.characterOutline[i].enabled = (i == characterIndex);
        }
        Debug.Log($"Враг {enemyIndex} атакует персонажа {characterIndex} на {damage} урона!");
        ButtonSound(4);
        // Изменяем иконку кнопки с использованием метода ChangeButtonIcon
        yield return StartCoroutine(ChangeButtonIcon(enemyIndex));
        // Если персонаж умирает
        if (playerHpSlider.value <= 0)
        {
            ButtonSound(5);
            if (ExpInGame >= 25) {
                ExpInGame-= 25;
                PlayerCoinsInGame -= 25;
                ExpText.text = ExpInGame.ToString();
                PlayerCoinsText.text = PlayerCoinsInGame.ToString();
            }
            characterManager.DeactivateUIElements(characterIndex + 1);
            Debug.Log($"Персонаж {characterIndex} убит!");
        }

        //StartCoroutine(DisableOutlinesWithDelay());
        yield return null;
    }


    IEnumerator ChangeButtonIcon(int enemyIndex)
    {
        // Массив для всех кнопок
        Image[] buttonIcons = new Image[] { enemyManager.buttonIcon1, enemyManager.buttonIcon2, enemyManager.buttonIcon3, enemyManager.buttonIcon4 };

        // Массив для всех типов спрайтов: Strong, Quick, Lasting и их 1-ые версии
        Sprite[][] enemySprites = new Sprite[][]
        {
        new Sprite[] { enemyManager.Strong, enemyManager.Strong1 }, // для "Strong" и "Strong1"
        new Sprite[] { enemyManager.Quick, enemyManager.Quick1 },   // для "Quick" и "Quick1"
        new Sprite[] { enemyManager.Lasting, enemyManager.Lasting1 } // для "Lasting" и "Lasting1"
        };
        // Для всех элементов в characterOutline
        for (int i = 0; i < enemyManager.EnemyOutline.Length; i++)
        {
            Debug.Log(enemyIndex);
            // Отключаем все, кроме выбранного
            enemyManager.EnemyOutline[i].enabled = (i == enemyIndex);
        }
        Debug.Log("Анимация атаки");

        // Проверяем имя спрайта врага и определяем, какой тип и спрайт использовать
        string enemySpriteName = enemyManager.enemyIcons[enemyIndex].sprite.name;
        Debug.Log("Текущий спрайт врага: " + enemySpriteName);

        // Определяем тип врага
        int spriteTypeIndex = -1;
        if (enemySpriteName == "Враг1") spriteTypeIndex = 0;  // Strong
        else if (enemySpriteName == "Враг2") spriteTypeIndex = 1;  // Quick
        else if (enemySpriteName == "Враг3") spriteTypeIndex = 2;  // Lasting

        if (spriteTypeIndex != -1)
        {
            // Выводим в консоль информацию о том, какой тип спрайта будет использоваться
            Debug.Log("Тип спрайта: " + (spriteTypeIndex == 0 ? "Strong" : spriteTypeIndex == 1 ? "Quick" : "Lasting"));

            // Меняем спрайт кнопки в зависимости от индекса врага и типа спрайта
            Debug.Log($"Меняем спрайт кнопки {enemyIndex} с {buttonIcons[enemyIndex].sprite.name} на {enemySprites[spriteTypeIndex][1].name}");
            buttonIcons[enemyIndex].sprite = enemySprites[spriteTypeIndex][1]; // Устанавливаем "X1" (например, Strong1, Quick1, Lasting1)

            yield return new WaitForSeconds(1.0f);  // Пауза на 1 секунду

            // Выводим информацию о том, что спрайт кнопки изменится обратно
            Debug.Log($"Меняем спрайт кнопки {enemyIndex} с {buttonIcons[enemyIndex].sprite.name} на {enemySprites[spriteTypeIndex][0].name}");
            buttonIcons[enemyIndex].sprite = enemySprites[spriteTypeIndex][0]; // Устанавливаем "X" (например, Strong, Quick, Lasting)
        }
        else
        {
            Debug.LogError("Неизвестный тип спрайта врага." + spriteTypeIndex);
        }
    }
    int buttonId1 = -1;
    // Этот метод будет вызываться при нажатии на кнопку персонажа
    public void OnCharacterSelected(int characterId, int buttonId)
    {
        buttonId1 = buttonId;
        selectedCharacterId = characterId;
        // Для всех элементов в characterOutline
        for (int i = 0; i < characterManager.characterOutline.Length; i++)
        {
            // Отключаем все, кроме выбранного
            characterManager.characterOutline[i].enabled = (i == buttonId);
        }

        Debug.Log($"Персонаж {characterId} выбран для атаки.");
    }
    IEnumerator DisableOutlinesWithDelay()
    {
        // Пауза 0.3 секунды
        //yield return new WaitForSeconds(0.3f);

        // Отключаем все элементы в characterOutline
        for (int i = 0; i < characterManager.characterOutline.Length; i++)
        {
            characterManager.characterOutline[i].enabled = false;
            characterManager.Damage[i].text = "";
        }

        Debug.Log("Все outlines персонажей отключены.");

        // Отключаем все элементы в EnemyOutline
        for (int i = 0; i < enemyManager.EnemyOutline.Length; i++)
        {
            enemyManager.EnemyOutline[i].enabled = false;
            enemyManager.Damage[i].text = "";
        }

        Debug.Log("Все outlines врагов отключены.");
        yield return null;
    }

    // Этот метод будет вызываться при нажатии на кнопку врага
    public void OnEnemySelected(int enemyIndex)
    {
        selectedEnemyIndex = enemyIndex;

        // Получаем спрайт врага из enemyManager
        Sprite selectedEnemySprite = enemyManager.enemyIcons[enemyIndex].sprite;

        // Для всех элементов в characterOutline
        for (int i = 0; i < enemyManager.EnemyOutline.Length; i++)
        {
            Debug.Log(enemyIndex);
            // Отключаем все, кроме выбранного
            enemyManager.EnemyOutline[i].enabled = (i == enemyIndex);
        }

        // Выводим информацию в консоль
        Debug.Log($"Враг {enemyIndex + 1} выбран в качестве цели. Спрайт: {selectedEnemySprite.name}");
    }
    public void OnApplicationPause(bool pause)
    {
        if (pause) {
            Time.timeScale = 0.0f;
        }else Time.timeScale = 1.0f;
    }
    public void ButtonSound(int sound)
    {
        AudioManager.Instance.PlaySFX(sound); // Воспроизводит переденный звук
    }
    
    public void PodskazkaOff() {
        PlayerPrefs.SetInt("PodskazkaPokazana", 1);
        Podskazka.SetActive(false);
    }
}
