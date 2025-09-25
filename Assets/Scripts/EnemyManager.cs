using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class EnemyManager : MonoBehaviour
{
    // Переменные для 4 иконок (спрайтов)
    public Sprite icon1; // Иконка для врага уровня 0
    public Sprite icon2; // Иконка для врага уровня 1
    public Sprite icon3; // Иконка для врага уровня 2
    public Sprite defaultIcon; // Иконка по умолчанию

    // Переменные для 4 кнопок (с добавлением для спрайтов)
    public Image buttonIcon1; // Иконка для кнопки 1
    public Image buttonIcon2; // Иконка для кнопки 2
    public Image buttonIcon3; // Иконка для кнопки 3
    public Image buttonIcon4; // Иконка для кнопки 4
    public Image buttonIcon1attack; // Иконка для кнопки 1
    public Image buttonIcon2attack; // Иконка для кнопки 2
    public Image buttonIcon3attack; // Иконка для кнопки 3
    public Image buttonIcon4attack; // Иконка для кнопки 4
    public Sprite Strong; // Иконка для кнопки уровня 0
    public Sprite Quick; // Иконка для кнопки уровня 1
    public Sprite Lasting; // Иконка для кнопки уровня 2
    public Sprite Strong1; // Иконка для кнопки уровня 0
    public Sprite Quick1; // Иконка для кнопки уровня 1
    public Sprite Lasting1; // Иконка для кнопки уровня 2

    // Списки Image компонентов для отображения иконок врагов
    public List<Image> enemyIcons; // Список иконок врагов

    // Списки слайдеров для HP и Damage
    public List<Slider> hpSliders; // Список слайдеров HP
    public List<Slider> damageSliders; // Список слайдеров Damage

    // Списки кнопок для выбора врагов
    public Button button1;
    public Button button2;
    public Button button3;
    public Button button4;
    public Outline[] EnemyOutline;
    public TextMeshProUGUI[] Damage;
    // Переменная для отслеживания уровня сложности
    private int difficultyLevel = 0;
    private int LevelUp = 0;

    // Метод для проверки, все ли враги мертвы
    public bool AllEnemiesDead()
    {
        foreach (var slider in hpSliders)
        {
            if (slider.gameObject.activeSelf && slider.value > 0)  // Если враг жив
            {
                return false;
            }
        }
        LevelUp = PlayerPrefs.GetInt("LevelUP", 0);
        LevelUp++;
        if (LevelUp % 3 == 0) {
            difficultyLevel = PlayerPrefs.GetInt("difficultyLevel", 1);
            difficultyLevel++;
            PlayerPrefs.SetInt("difficultyLevel", difficultyLevel);
        }
        PlayerPrefs.SetInt("LevelUP", LevelUp);
        return true;
    }

    
    // Метод для повышения уровня сложности
    public void IncreaseDifficulty()
    {
        difficultyLevel = PlayerPrefs.GetInt("difficultyLevel", 1);
        // difficultyLevel = PlayerPrefs.GetInt("difficultyLevel",0);
        // difficultyLevel++;
        //  PlayerPrefs.SetInt("difficultyLevel", difficultyLevel);
        // В зависимости от уровня сложности увеличиваем количество врагов и их параметры
        int numberOfEnemies = Mathf.Min(difficultyLevel, 4); // Ограничим до 4 врагов
        for (int i = 0; i < numberOfEnemies; i++)
        {
            // Генерируем врагов только для активных слотов
            GenerateRandomEnemy(i);
        }

        // Деактивируем все лишние элементы, которые не активированы на данном уровне сложности
        DeactivateExcessEnemies(numberOfEnemies);
    }

    // Метод для случайного генератора характеристик врага
    public void GenerateRandomEnemy(int index)
    {
        // Случайный уровень врага (0, 1 или 2)
        int level = Random.Range(0, 3);

        // Случайные параметры врага в зависимости от его уровня и сложности
        int hp = 0;
        int damage = 0;
        Sprite icon = defaultIcon;
        Sprite buttonIcon = null;

        // Учитываем сложность, чем выше уровень сложности, тем сильнее враги
        //int hpMultiplier = difficultyLevel + 1;
        //int damageMultiplier = difficultyLevel + 1;

        switch (level)
        {
            case 0:
                hp = Random.Range(25, 90);  // Уровень 0
                damage = Random.Range(10, 15);
                icon = icon1;  // Иконка для уровня 0
                buttonIcon = Strong;  // Иконка для кнопки уровня 0
                break;
            case 1:
                hp = Random.Range(30, 70);  // Уровень 1
                damage = Random.Range(5, 10);
                icon = icon2;  // Иконка для уровня 1
                buttonIcon = Quick;  // Иконка для кнопки уровня 1
                break;
            case 2:
                hp = Random.Range(40, 100);  // Уровень 2
                damage = Random.Range(30, 50);
                icon = icon3;  // Иконка для уровня 2
                buttonIcon = Lasting;  // Иконка для кнопки уровня 2
                break;
        }

        // Назначаем параметры на соответствующие UI элементы
        if (index < enemyIcons.Count)
        {
            enemyIcons[index].sprite = icon;  // Устанавливаем иконку для врага
            hpSliders[index].value = hp;  // Устанавливаем значение HP слайдера
            damageSliders[index].value = damage;  // Устанавливаем значение Damage слайдера

            // Назначаем иконку на кнопку в зависимости от уровня
            switch (index)
            {
                case 0:
                    buttonIcon1.sprite = buttonIcon;
                    buttonIcon1.gameObject.SetActive(true);
                    break;
                case 1:
                    buttonIcon2.sprite = buttonIcon;
                    buttonIcon2.gameObject.SetActive(true);
                    break;
                case 2:
                    buttonIcon3.sprite = buttonIcon;
                    buttonIcon3.gameObject.SetActive(true);
                    break;
                case 3:
                    buttonIcon4.sprite = buttonIcon;
                    buttonIcon4.gameObject.SetActive(true);
                    break;
                default:
                    Debug.LogWarning("Некорректный индекс кнопки!");
                    break;
            }

            // Логируем изменения
            Debug.Log($"Враг {index + 1} с уровнем {level} создан. HP: {hp}, Damage: {damage}");
        }
    }

    // Метод для деактивации лишних врагов, которые не были активированы на этом уровне сложности
    public void DeactivateExcessEnemies(int numberOfEnemies)
    {
        // Деактивируем все врагов, которые не были активированы
        for (int i = numberOfEnemies; i < enemyIcons.Count; i++)
        {
            enemyIcons[i].gameObject.SetActive(false);
            hpSliders[i].gameObject.SetActive(false);
            damageSliders[i].gameObject.SetActive(false);
        }

        // Деактивируем кнопки для тех врагов, которых нет
        if (numberOfEnemies < 1) buttonIcon1.gameObject.SetActive(false);
        if (numberOfEnemies < 2) buttonIcon2.gameObject.SetActive(false);
        if (numberOfEnemies < 3) buttonIcon3.gameObject.SetActive(false);
        if (numberOfEnemies < 4) buttonIcon4.gameObject.SetActive(false);
    }

    // Метод для деактивации UI элементов для отсутствующего врага
    public void DeactivateUIElements(int buttonIndex)
    {
        switch (buttonIndex)
        {
            case 1:
                buttonIcon1.gameObject.SetActive(false);
                hpSliders[0].gameObject.SetActive(false);
                damageSliders[0].gameObject.SetActive(false);
                enemyIcons[0].gameObject.SetActive(false);
                break;
            case 2:
                buttonIcon2.gameObject.SetActive(false);
                hpSliders[1].gameObject.SetActive(false);
                damageSliders[1].gameObject.SetActive(false);
                enemyIcons[1].gameObject.SetActive(false);
                break;
            case 3:
                buttonIcon3.gameObject.SetActive(false);
                hpSliders[2].gameObject.SetActive(false);
                damageSliders[2].gameObject.SetActive(false);
                enemyIcons[2].gameObject.SetActive(false);
                break;
            case 4:
                buttonIcon4.gameObject.SetActive(false);
                hpSliders[3].gameObject.SetActive(false);
                damageSliders[3].gameObject.SetActive(false);
                enemyIcons[3].gameObject.SetActive(false);
                break;
            default:
                Debug.LogWarning("Некорректный индекс для деактивации UI элементов.");
                break;
        }
    }

    // Пример использования
    void Start()
    {
        //Debug.Log(difficultyLevel);
        //PlayerPrefs.GetInt("difficultyLevel", 1);
        // Генерируем начальных врагов
        IncreaseDifficulty();

        // Привязка обработчиков событий для кнопок
        button1.onClick.AddListener(() => OnEnemyButtonClicked(0));
        button2.onClick.AddListener(() => OnEnemyButtonClicked(1));
        button3.onClick.AddListener(() => OnEnemyButtonClicked(2));
        button4.onClick.AddListener(() => OnEnemyButtonClicked(3));
    }

    // Обработчик события нажатия на кнопку врага
    void OnEnemyButtonClicked(int buttonIndex)
    {
        // Получаем параметры врага, связанного с данной кнопкой
        string enemyType = "";
        switch (buttonIndex)
        {
            case 0:
                enemyType = "Сильный";
                break;
            case 1:
                enemyType = "Быстрый";
                break;
            case 2:
                enemyType = "Долговечный";
                break;
            case 3:
                enemyType = "Неизвестный";
                break;
        }
        Debug.Log($"Выбран враг: {enemyType}");

        // Увеличиваем сложность, если все враги мертвы
        //if (AllEnemiesDead())
        //{
        //    IncreaseDifficulty();
        //}
    }
}
