using UnityEngine;
using UnityEngine.UI; // Для работы с UI элементами
using System.Collections.Generic; // Для использования HashSet
using System.Xml;
using Unity.VisualScripting.Antlr3.Runtime.Misc;

public class ButtonSelection : MonoBehaviour
{
    // Ссылки на элементы UI
    public Image buttonImage; // Изображение фона кнопки
    public Sprite plusSprite; // Спрайт для пустого слота (с плюсом)
    public Sprite greenSprite; // Спрайт для выбранного слота (зелёный фон)
    public Sprite yellowSprite; // Спрайт для занятого слота (жёлтый фон с персонажем)
    public Image characterImage; // Изображение персонажа

    // Слайдеры для отображения HP и Damage
    public Slider hpSlider; // Слайдер для здоровья
    public Slider damageSlider; // Слайдер для урона

    // Переменные для внутреннего состояния
    private bool isSelected = false; // Флаг, выбран ли этот слот
    private bool isCharacterPlaced = false; // Флаг, занят ли слот персонажем

    // Статический список для всех слотов
    public static List<ButtonSelection> slots = new List<ButtonSelection>();

    // Статический HashSet для отслеживания использованных ID персонажей
    public static HashSet<int> usedCharacterIds = new HashSet<int>();

    // Уникальный ID персонажа в этом слоте
    public int characterId;
    public int SlotId;

    // Статическая переменная для хранения текущего выбранного слота
    private static ButtonSelection selectedSlot = null;
    int a = 0;

    private void Awake()
    {
        if (a == 0)
        {
            usedCharacterIds.Clear();
            slots.Clear();
            a = 1;
            Debug.Log("Почистили");
        }
    }

    void Start()
    {
        // Добавляем этот слот в список, если его ещё нет
        if (!slots.Contains(this))
        {
            slots.Add(this);
            Debug.Log($"Слот с ID {characterId} добавлен в список слотов.");
        }

        // Загружаем все персонажи при старте игры
        LoadCharacters();

        Debug.Log("Вызван СТАРТ");
    }

    public bool IsCharacterPlaced  // Свойство для доступа к полю isCharacterPlaced
    {
        get { return isCharacterPlaced; }
    }

    // Метод для выбора слота (вызывается при нажатии на кнопку)
    public void OnSlotSelected()
    {
        // Если слот пустой или занят другим персонажем
        if (!isCharacterPlaced)
        {
            // Если выбран другой слот, сбрасываем его выделение
            if (selectedSlot != null && selectedSlot != this)
            {
                selectedSlot.Deselect();
            }

            // Выбираем этот слот
            isSelected = true;
            buttonImage.sprite = greenSprite; // Меняем фон на зелёный
            selectedSlot = this; // Устанавливаем этот слот как выбранный
        }
        else
        {
            // Если слот занят, удаляем текущего персонажа, чтобы разместить нового
            RemoveCharacter();
        }
    }

    // Метод для снятия выделения
    public void Deselect()
    {
        isSelected = false;

        // Если слот пустой, возвращаем на спрайт с плюсом
        if (!isCharacterPlaced)
        {
            buttonImage.sprite = plusSprite;
        }
    }

    // Метод для установки персонажа в слот
    public void PlaceCharacter(Sprite characterSprite, int hp, int damage, GameObject characterEquippedText, int uniqueId)
    {
        // Если слот уже занят, то удаляем текущего персонажа
        if (isCharacterPlaced)
        {
            RemoveCharacter();
        }

        // Проверяем, не был ли этот персонаж уже использован
        if (usedCharacterIds.Contains(uniqueId))
        {
            Debug.Log("Этот персонаж уже размещён в другом слоте!");
            return; // Если персонаж уже использовался, не даём его поставить
        }

        // Устанавливаем персонажа в слот
        buttonImage.sprite = yellowSprite; // Фон становится жёлтым
        characterImage.sprite = characterSprite; // Устанавливаем спрайт персонажа
        characterImage.gameObject.SetActive(true); // Показываем изображение персонажа
        isCharacterPlaced = true; // Слот теперь занят

        // Добавляем ID персонажа в список использованных
        usedCharacterIds.Add(uniqueId);

        // Устанавливаем значения для здоровья и урона
        if (hpSlider != null && damageSlider != null)
        {
            hpSlider.value = hp;
            damageSlider.value = damage;
        }

        // Показываем текст "Equipped" на кнопке
        characterEquippedText.SetActive(true);

        // Устанавливаем уникальный ID персонажа в этот слот
        this.characterId = uniqueId;

        // Сохраняем данные персонажа в PlayerPrefs с учётом SlotId
        SaveCharacterData();

        // Устанавливаем уровень персонажа из PlayerPrefs
        int level = PlayerPrefs.GetInt("Character_" + characterId + "_level", 0);
        Debug.Log("Установили лвл емае");

        // Сохраняем данные HP и Damage
        PlayerPrefs.SetInt("Character_" + characterId + "_hp", hp);
        PlayerPrefs.SetInt("Character_" + characterId + "_damage", damage);
        PlayerPrefs.SetInt("Character_" + characterId + "_level", level);

        // Выводим информацию о сохранённом персонаже
        Debug.Log($"Персонаж с ID {characterId} сохранён в слоте {SlotId}: HP = {hp}, Damage = {damage}, Level = {level}");
    }

    // Метод для сохранения данных персонажа в PlayerPrefs с учётом SlotId
    private void SaveCharacterData()
    {
        PlayerPrefs.SetInt("Slot_" + SlotId + "_characterId", characterId);
        Debug.Log($"Сохранены данные персонажа с ID {characterId} в слот {SlotId} в PlayerPrefs.");
    }

    // Метод для обновления слайдеров (например, когда изменяются характеристики персонажа)
    public void UpdateSliders(int updatedHp, int updatedDamage)
    {
        if (hpSlider != null && damageSlider != null)
        {
            hpSlider.value = updatedHp;
            damageSlider.value = updatedDamage;
        }
    }

    // Статический метод для получения текущего выбранного слота
    public static ButtonSelection GetSelectedSlot()
    {
        return selectedSlot;
    }

    // Метод для удаления персонажа из слота
    // Метод для удаления персонажа из слота
    public void RemoveCharacter()
    {
        if (isCharacterPlaced)
        {
            // Убираем ID персонажа из списка использованных
            usedCharacterIds.Remove(characterId);

            // Удаляем данные персонажа из PlayerPrefs
            PlayerPrefs.DeleteKey("Slot_" + SlotId + "_characterId"); // Удаляем ID персонажа для этого слота
            PlayerPrefs.DeleteKey("Character_" + characterId + "_hp");  // Удаляем HP персонажа
            PlayerPrefs.DeleteKey("Character_" + characterId + "_damage");  // Удаляем Damage персонажа
            PlayerPrefs.DeleteKey("Character_" + characterId + "_level");  // Удаляем Level персонажа

            // Сбрасываем спрайт персонажа
            characterImage.sprite = null;
            characterImage.gameObject.SetActive(false);

            // Возвращаем фон кнопки на исходный (с плюсиком)
            buttonImage.sprite = plusSprite;

            // Сбрасываем слайдеры
            if (hpSlider != null && damageSlider != null)
            {
                hpSlider.value = 0;
                damageSlider.value = 0;
            }

            // Слот теперь пустой
            isCharacterPlaced = false;
            Debug.Log("Персонаж удалён из слота " + SlotId);
        }
    }


    // Метод для загрузки всех сохранённых персонажей из PlayerPrefs
    // Метод для загрузки персонажа
    private void LoadCharacters()
    {
        Debug.Log("Загрузка персонажей...");

        // Перебираем все слоты
        foreach (ButtonSelection slot in slots)
        {
            int savedCharacterId = PlayerPrefs.GetInt("Slot_" + slot.SlotId + "_characterId", -1);
            if (savedCharacterId != -1)
            {
                string spritePath = "";

                switch (savedCharacterId)
                {
                    case int n when (n >= 0 && n <= 99):
                        spritePath = "Squad/пірат4-3";
                        break;
                    case int n when (n >= 100 && n <= 199):
                        spritePath = "Squad/пірат4-1";
                        break;
                    case int n when (n >= 200 && n <= 299):
                        spritePath = "Squad/пірат4-2";
                        break;
                    default:
                        Debug.LogWarning($"Не найден подходящий спрайт для персонажа с ID {savedCharacterId}");
                        break;
                }

                // Загружаем спрайт по пути
                Sprite savedSprite = Resources.Load<Sprite>(spritePath);

                // Проверяем, был ли успешно загружен спрайт
                if (savedSprite == null)
                {
                    Debug.LogError($"Ошибка загрузки спрайта для персонажа с ID {savedCharacterId} из пути {spritePath}");
                }

                int savedHp = PlayerPrefs.GetInt("Character_" + savedCharacterId + "_hp", 0);
                int savedDamage = PlayerPrefs.GetInt("Character_" + savedCharacterId + "_damage", 0);
                int savedLevel = PlayerPrefs.GetInt("Character_" + savedCharacterId + "_level", 0);

                // Загружаем данные
                Debug.Log($"Загружен персонаж с ID {savedCharacterId} в слот {slot.SlotId}: HP = {savedHp}, Damage = {savedDamage}, Level = {savedLevel}");

                // Устанавливаем эти данные в слот
                slot.PlaceCharacter(savedSprite, savedHp, savedDamage, slot.characterImage.gameObject, savedCharacterId);
            }
            else
            {
                Debug.Log($"Слот {slot.SlotId} пуст.");
            }
        }
    }


    void Update()
    {
        // Например, по нажатию на клавишу "Q" выводим все ID
        if (Input.GetKeyDown(KeyCode.Q))
        {
            PrintCharacterIds();
        }
    }

    public static void PrintCharacterIds()
    {
        int emptySlotsCount = 0;

        // Перебираем все слоты в списке
        foreach (ButtonSelection slot in slots)
        {
            // Проверяем, пуст ли слот
            if (!slot.isCharacterPlaced || slot.characterId == -1)
            {
                emptySlotsCount++;
                Debug.Log($"Слот с ID {slot.characterId} пуст.");
            }
            else
            {
                // Если слот занят, выводим его ID
                Debug.Log($"Слот с ID {slot.characterId} занят персонажем.");
            }
        }

        // Выводим количество пустых слотов
        Debug.Log($"Количество пустых слотов: {emptySlotsCount}");
    }
}
