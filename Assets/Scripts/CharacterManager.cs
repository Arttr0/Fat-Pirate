using UnityEngine;
using UnityEngine.UI; // Нужно для работы с Image и Slider
using System.Collections.Generic;
using UnityEngine.TextCore.Text;
using TMPro;

public class CharacterManager : MonoBehaviour
{
    // Переменные для 4 иконок (спрайтов)
    public Sprite icon1; // Иконка для персонажа с ID <= 100
    public Sprite icon2; // Иконка для персонажа с 101 до 200
    public Sprite icon3; // Иконка для персонажа с 201 до 300
    public Sprite defaultIcon; // Иконка по умолчанию

    // Переменные для 4 кнопок (с добавлением для спрайтов)
    public Image buttonIcon1; // Иконка для кнопки 1
    public Image buttonIcon2; // Иконка для кнопки 2
    public Image buttonIcon3; // Иконка для кнопки 3
    public Image buttonIcon4; // Иконка для кнопки 4
    public Sprite Strong; // Иконка для кнопки с ID <= 100
    public Sprite Quick; // Иконка для кнопки с 101 до 200
    public Sprite Lasting; // Иконка для кнопки с 201 до 300
    public Sprite Strong1; // Иконка удара для кнопки уровня 0
    public Sprite Quick1; // Иконка удара для кнопки уровня 1
    public Sprite Lasting1; // Иконка удара для кнопки уровня 2

    // Список Image компонентов, куда будем загружать иконки
    public List<Image> characterIcons; // Список компонентов Image для отображения иконок

    // Списки слайдеров для HP и Damage
    public List<Slider> hpSliders; // Список слайдеров HP
    public List<Slider> damageSliders; // Список слайдеров Damage

    // Переменные для выбора персонажа
    public Button characterButton1;
    public Button characterButton2;
    public Button characterButton3;
    public Button characterButton4;
    public Outline[] characterOutline;
    public TextMeshProUGUI[] Damage;
    public GameController gameController;
    // Метод для проверки, все ли персонажи мертвы
    public bool AllCharactersDead()
    {
        foreach (var slider in hpSliders)
        {
            if (slider.gameObject.activeSelf && slider.value > 0)  // Если персонаж жив
            {
                return false;
            }
        }
        return true;
    }

    // Метод для получения всех ID персонажей, которые заняли слоты
    public List<int> GetSelectedCharacterIds()
    {
        List<int> selectedCharacterIds = new List<int>();

        // Перебираем все слоты и добавляем ID занятых персонажей
        foreach (ButtonSelection slot in ButtonSelection.slots)
        {
            if (slot.IsCharacterPlaced) // Используем свойство
            {
                selectedCharacterIds.Add(slot.characterId);
            }
            else
            {
                selectedCharacterIds.Add(-1); // Если персонаж не выбран, добавляем -1
            }
        }

        return selectedCharacterIds; // Возвращаем список всех выбранных ID
    }

    // Метод для назначения иконки (картинки) в зависимости от ID
    Sprite GetIconForCharacter(int id)
    {
        if (id <= 99 && id >= 0) // Диапазон для ID 0-99
        {
            return icon1; // Возвращаем первую иконку
        }
        else if (id >= 100 && id <= 199) // Диапазон для ID 100-199
        {
            return icon2; // Возвращаем вторую иконку
        }
        else if (id >= 200 && id <= 299) // Диапазон для ID 200-299
        {
            return icon3; // Возвращаем третью иконку
        }
        else
        {
            return defaultIcon; // Возвращаем иконку по умолчанию, если ID больше 300
        }
    }

    // Метод для назначения иконки на кнопку
    void AssignIconToButton(int buttonIndex, int id)
    {
        Sprite iconToAssign = null; // Иконка, которую мы будем назначать

        if (id <= 99 && id >= 0) // Диапазон для ID 0-99
        {
            iconToAssign = Strong; // Выбираем иконку Strong
        }
        else if (id >= 100 && id <= 199) // Диапазон для ID 100-199
        {
            iconToAssign = Quick; // Выбираем иконку Quick
        }
        else if (id >= 200 && id <= 299) // Диапазон для ID 200-299
        {
            iconToAssign = Lasting; // Выбираем иконку Lasting
        }

        // Назначаем иконку для кнопки в зависимости от индекса
        switch (buttonIndex)
        {
            case 1:
                buttonIcon1.sprite = iconToAssign; // Для первой кнопки
                break;
            case 2:
                buttonIcon2.sprite = iconToAssign; // Для второй кнопки
                break;
            case 3:
                buttonIcon3.sprite = iconToAssign; // Для третьей кнопки
                break;
            case 4:
                buttonIcon4.sprite = iconToAssign; // Для четвёртой кнопки
                break;
            default:
                Debug.LogWarning("Некорректный индекс кнопки!");
                break;
        }
    }

    // Метод для деактивации UI элементов (иконка, слайдеры) для ID отсутствующего или невалидного
    public void DeactivateUIElements(int buttonIndex)
    {
        // Проверяем, какой индекс передан, и выключаем соответствующие элементы
        switch (buttonIndex)
        {
            case 1:
                buttonIcon1.gameObject.SetActive(false); // Выключаем иконку кнопки 1
                hpSliders[0].gameObject.SetActive(false); // Выключаем слайдер HP кнопки 1
                damageSliders[0].gameObject.SetActive(false); // Выключаем слайдер Damage кнопки 1
                characterIcons[0].gameObject.SetActive(false); // Выключаем иконку для персонажа 1
                Debug.Log("Отключены элементы UI для кнопки 1: Иконка, HP слайдер, Damage слайдер и иконка персонажа.");
                break;
            case 2:
                buttonIcon2.gameObject.SetActive(false); // Выключаем иконку кнопки 2
                hpSliders[1].gameObject.SetActive(false); // Выключаем слайдер HP кнопки 2
                damageSliders[1].gameObject.SetActive(false); // Выключаем слайдер Damage кнопки 2
                characterIcons[1].gameObject.SetActive(false); // Выключаем иконку для персонажа 2
                Debug.Log("Отключены элементы UI для кнопки 2: Иконка, HP слайдер, Damage слайдер и иконка персонажа.");
                break;
            case 3:
                buttonIcon3.gameObject.SetActive(false); // Выключаем иконку кнопки 3
                hpSliders[2].gameObject.SetActive(false); // Выключаем слайдер HP кнопки 3
                damageSliders[2].gameObject.SetActive(false); // Выключаем слайдер Damage кнопки 3
                characterIcons[2].gameObject.SetActive(false); // Выключаем иконку для персонажа 3
                Debug.Log("Отключены элементы UI для кнопки 3: Иконка, HP слайдер, Damage слайдер и иконка персонажа.");
                break;
            case 4:
                buttonIcon4.gameObject.SetActive(false); // Выключаем иконку кнопки 4
                hpSliders[3].gameObject.SetActive(false); // Выключаем слайдер HP кнопки 4
                damageSliders[3].gameObject.SetActive(false); // Выключаем слайдер Damage кнопки 4
                characterIcons[3].gameObject.SetActive(false); // Выключаем иконку для персонажа 4
                Debug.Log("Отключены элементы UI для кнопки 4: Иконка, HP слайдер, Damage слайдер и иконка персонажа.");
                break;
            default:
                Debug.LogWarning("Некорректный индекс для деактивации UI элементов.");
                break;
        }
    }

    // Пример использования
    void Start()
    {
        List<int> selectedIds = GetSelectedCharacterIds();

        // Выводим все выбранные ID персонажей и назначаем иконки и слайдеры
        for (int i = 0; i < selectedIds.Count; i++)
        {
            int id = selectedIds[i];
            Debug.Log("Персонаж" + id);
            int buttonId;
            Sprite assignedIcon = GetIconForCharacter(id);
            switch (i)
            {
                case 0:
                    buttonId = 0;
                    characterButton1.onClick.AddListener(() => gameController.OnCharacterSelected(id, buttonId));
                    break;
                case 1:
                    buttonId = 1;
                    characterButton2.onClick.AddListener(() => gameController.OnCharacterSelected(id, buttonId));
                    break;
                case 2:
                    buttonId = 2;
                    characterButton3.onClick.AddListener(() => gameController.OnCharacterSelected(id, buttonId));
                    break;
                case 3:
                    buttonId = 3;
                    characterButton4.onClick.AddListener(() => gameController.OnCharacterSelected(id, buttonId));
                    break;
                default: break;
            }

            // Выводим ID и соответствующую иконку для проверки
            Debug.Log("Для персонажа с ID " + id + " присваивается иконка: " + assignedIcon.name + " в слот: " + i);

            // Если ID не передан (id == -1), то отключаем все UI элементы для этого слота
            if (id == -1)
            {
                DeactivateUIElements(i + 1); // Передаем индекс кнопки и ID персонажа
            }
            else
            {
                // Применяем иконку к компоненту Image для отображения
                if (i < characterIcons.Count)
                {
                    characterIcons[i].sprite = assignedIcon; // Применяем иконку
                }
                else
                {
                    Debug.LogWarning("Нет достаточного количества Image компонентов для отображения персонажей.");
                }

                // Обновляем слайдеры HP и Damage для каждого персонажа
                if (i < hpSliders.Count && i < damageSliders.Count)
                {
                    int hp = PlayerPrefs.GetInt("Character_" + id + "_hp", 0); // Пытаемся получить HP
                    int damage = PlayerPrefs.GetInt("Character_" + id + "_damage", 0); // Пытаемся получить Damage

                    // Логируем полученные значения для отладки
                    Debug.Log($"Для персонажа с ID {id} загружен HP: {hp}, Damage: {damage}");

                    // Применяем значения HP и Damage к соответствующим слайдерам
                    hpSliders[i].value = hp; // Устанавливаем значение слайдера HP
                    damageSliders[i].value = damage; // Устанавливаем значение слайдера Damage

                    // Выводим обновленные значения для отладки
                    Debug.Log("Обновлен HP для персонажа с ID " + id + ": " + hp);
                    Debug.Log("Обновлен Damage для персонажа с ID " + id + ": " + damage);
                }
                else
                {
                    Debug.LogWarning("Нет достаточного количества слайдеров для обновления HP и Damage.");
                }

                // Применяем иконки на кнопки в зависимости от ID
                AssignIconToButton(i + 1, id); // Передаем индекс кнопки и ID персонажа
            }
        }
    }

}