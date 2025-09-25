using TMPro;
using UnityEngine;
using UnityEngine.TextCore.Text;
using UnityEngine.UI; // Для работы с UI элементами

public class CharacterButton : MonoBehaviour
{
    public Sprite characterSprite;  // Спрайт персонажа
    public int hp; // HP персонажа
    public int damage; // Урон персонажа
    public int characterId;  // Уникальный ID персонажа
    public GameObject equippedText; // Ссылка на объект "Equipped"
    public int Exp;  // Переменная для опыта
    public int level = 0; // Уровень персонажа (1, 2, 3)
    public Button UpgradeButton;  // Ссылка на кнопку
    public Image targetImage;  // Ссылка на изображение, которое будем менять
    public Sprite[] UpgradeSprites;  // Массив изображений для разных языков
    public GameObject PopUpExp;
    // Добавляем переменную для ссылки на TextMeshPro компонент
    private TMP_Text experienceText;

    void Start()
    {
        PopUpExp = GameObject.Find("Canvas/Panels/PopUpExp");
        // Загружаем HP, Damage и Level из PlayerPrefs
        if (PlayerPrefs.HasKey("Character_" + characterId + "_hp"))
        {
            hp = PlayerPrefs.GetInt("Character_" + characterId + "_hp");
            damage = PlayerPrefs.GetInt("Character_" + characterId + "_damage");
            level = PlayerPrefs.GetInt("Character_" + characterId + "_level", 0);  // Загружаем уровень, если он существует
            Debug.Log(hp);
            Debug.Log(damage);
            Debug.Log(level);
        }
        UpdateCharacterImage();
        // Загружаем значение опыта
        Exp = PlayerPrefs.GetInt("Exp", 0);
        //Exp = 1000;
        // Находим компонент TMP_Text через GameObject.Find по точному пути
        GameObject experienceTextObject = GameObject.Find("Canvas/Panels/Squad Panel/Табличка с опытом/Image/Text (TMP)");

        if (experienceTextObject != null)
        {
            experienceText = experienceTextObject.GetComponent<TMP_Text>();

            if (experienceText != null)
            {
                Debug.Log("Найден компонент TMP_Text: " + experienceText.name);
                // Обновляем текст с текущим значением Exp
                UpdateExperienceText();
            }
            else
            {
                Debug.LogError("Компонент TMP_Text не найден на объекте: " + experienceTextObject.name);
            }
        }
        else
        {
            Debug.LogError("Объект с текстом не найден по пути: Canvas/Panels/Squad Panel/Табличка с опытом/Image/Text (TMP)");
        }
    }

    // Метод обновления текста с опытом
    private void UpdateExperienceText()
    {
        if (experienceText != null)
        {
            experienceText.text = Exp.ToString();  // Обновляем текст, отображая опыт
        }
        else
        {
            Debug.LogError("TMP_Text компонент не был инициализирован.");
        }
    }

    // Метод вызывается при нажатии на кнопку персонажа
    public void OnCharacterSelect()
    {
        // Получаем выбранный слот с помощью статического метода GetSelectedSlot()
        ButtonSelection selectedSlot = ButtonSelection.GetSelectedSlot();

        if (selectedSlot != null) // Если слот был выбран
        {
            Debug.Log($"Персонаж {characterId} выбран для слота {selectedSlot.characterId}");
            selectedSlot.PlaceCharacter(characterSprite, hp, damage, equippedText, characterId);
        }
        else
        {
            Debug.Log("Слот не был выбран!");
        }
    }

    void UpdateCharacterImage()
    {
        if (level == 0)
        {
            targetImage.sprite = UpgradeSprites[0]; // Уровень 0
        }
        else if (level == 1)
        {
            targetImage.sprite = UpgradeSprites[1]; // Уровень 1
        }
        else if (level == 2)
        {
            targetImage.sprite = UpgradeSprites[2]; // Уровень 2
        }
        else if (level == 3)
        {
            targetImage.sprite = UpgradeSprites[2]; // Выключаем кнопку
            UpgradeButton.interactable = false;
        }
    }

    // Метод для улучшения персонажа
    public void UpgradeCharacter()
    {
        // Сохраняем улучшения персонажа в PlayerPrefs
        //PlayerPrefs.SetInt("Character_" + characterId + "_hp", hp);
        //PlayerPrefs.SetInt("Character_" + characterId + "_damage", damage);
        //PlayerPrefs.SetInt("Character_" + characterId + "_level", level);  // Сохраняем уровень

        // Улучшаем параметры персонажа (например, увеличиваем HP и Damage)
        //hp += 5;
        //damage += 5;
        // Улучшаем уровень персонажа
        if (level < 3)
        {
            if (level == 0 && Exp >= 50)
            {
                Exp -= 50;
                level++;
                targetImage.sprite = UpgradeSprites[level];
                hp += 5;
                damage += 5;
            }
            else if (level == 1 && Exp >= 100)
            {
                Exp -= 100;
                level++;
                hp += 10;
                damage += 7;
                targetImage.sprite = UpgradeSprites[level];
            }
            else if (level == 2 && Exp >= 200)
            {
                Exp -= 200;
                level++;
                hp += 20;
                damage += 10;
                UpgradeButton.interactable = false;
            }
            else {
                PopUpExp.SetActive(true);
            }
        }
        PlayerPrefs.SetInt("Character_" + characterId + "_hp", hp);
        PlayerPrefs.SetInt("Character_" + characterId + "_damage", damage);
        PlayerPrefs.SetInt("Character_" + characterId + "_level", level);
        // Проходим по всем слотам и ищем слот с нужным ID
        bool slotFound = false; // Для отладки, чтобы проверить, найден ли слот
        foreach (ButtonSelection slot in ButtonSelection.slots)
        {
            Debug.Log($"Проверка слота с ID: {slot.characterId}"); // Логируем ID слота
            if (slot.characterId == characterId) // Если ID персонажа в слоте совпадает
            {
                // Обновляем слайдеры в этом слоте
                slot.UpdateSliders(hp, damage);
                Debug.Log($"Слайдеры обновлены для персонажа с ID {characterId}.");
                slotFound = true; // Отметим, что слот найден
                break; // Выход из цикла, так как мы нашли нужный слот
            }
        }

        if (!slotFound)
        {
            Debug.Log($"Слот с ID {characterId} не найден!");
        }

        // Отображаем информацию об улучшении
        Debug.Log($"Character {characterId} upgraded: HP = {hp}, Damage = {damage}, Level = {level}");

        // Обновляем опыт
        // Exp += 10; // Увеличиваем опыт на 10, например

        // Сохраняем новый опыт в PlayerPrefs
        PlayerPrefs.SetInt("Exp", Exp);

        // Обновляем текст с опытом
        UpdateExperienceText();
    }

    public void Update()
    {
        if (!ButtonSelection.usedCharacterIds.Contains(characterId))
        {
            equippedText.SetActive(false);
            //Debug.Log(characterId + "Удалена надпись equippedText");
        }
        else {
            equippedText.SetActive(true);
        }
    }

}
