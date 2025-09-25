using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class DailyBonus : MonoBehaviour
{
    [Header("Bonus Settings")]
    public Button[] bonusButtons;  // Массив кнопок для всех бонусов
    public Image[] crossOutImages;  // Массив изображений для перечеркнутых бонусов
    public Image[] bonusImages;  // Массив изображений для бонусов (например, иконки бонусов)
    public int[] bonusAmounts;  // Суммы бонусов для каждой кнопки (например, 100, 200, 300 и т.д.)

    private const string PLAYER_COINS_KEY = "PlayerCoins";  // Ключ для монет игрока
    private const string LAST_LOGIN_DATE_KEY = "LastLoginDate";  // Ключ для хранения даты последнего входа
    private const string BONUS_COLLECTED_KEY = "BonusCollected_";  // Префикс для ключей, отслеживающих, был ли собран бонус
    private const string DAYS_PLAYED_KEY = "DaysPlayed";  // Ключ для отслеживания количества дней подряд, в которые игрок заходил
    private const int MAX_DAYS = 7;  // Максимальное количество дней для бонусов (цикл из 7 дней)

    void Start()
    {
        // Получаем количество дней, в которые игрок заходил в игру
        int daysPlayed = PlayerPrefs.GetInt(DAYS_PLAYED_KEY, 0);  // Если данных нет, начнем с 0

        // Загружаем дату последнего входа
        string lastLoginDate = PlayerPrefs.GetString(LAST_LOGIN_DATE_KEY, "");
        string currentDate = System.DateTime.Now.ToString("yyyy-MM-dd");

        // Если дата изменилась, это значит, что сегодня новый день
        if (lastLoginDate != currentDate)
        {
            // Если игрок пропустил день (то есть дата последнего входа не совпадает с текущей), сбрасываем счетчик на 1
            daysPlayed = 1;  // Сбросить на 1, если день пропущен

            // Обновляем дату последнего входа
            PlayerPrefs.SetString(LAST_LOGIN_DATE_KEY, currentDate);

            // Сохраняем обновленное количество дней в PlayerPrefs
            PlayerPrefs.SetInt(DAYS_PLAYED_KEY, daysPlayed);

            // Открываем бонусы в зависимости от количества дней
            for (int i = 0; i < bonusButtons.Length; i++)
            {
                if (i < daysPlayed)  // Доступен бонус для всех дней, в которые игрок заходил
                {
                    EnableBonus(i);
                }
                else  // Блокируем бонусы, которые еще не доступны
                {
                    DisableBonus(i);
                }
            }
        }
        else
        {
            // Если день не изменился, проверяем, какие бонусы были собраны
            for (int i = 0; i < bonusButtons.Length; i++)
            {
                // Мы показываем бонусы только до тех дней, которые игрок реально посещал
                if (i < daysPlayed)
                {
                    if (PlayerPrefs.GetInt(BONUS_COLLECTED_KEY + (i + 1), 0) == 0)  // Если бонус еще не собран
                    {
                        EnableBonus(i);
                    }
                    else
                    {
                        DisableBonus(i);
                        crossOutImages[i].gameObject.SetActive(true);  // Если бонус собран, показываем перечеркнутую картинку
                    }
                }
                else
                {
                    DisableBonus(i);  // Блокируем бонусы, которые еще не доступны
                }
            }
        }

        // Добавляем обработчики для каждой кнопки
        for (int i = 0; i < bonusButtons.Length; i++)
        {
            int index = i;  // Локальная копия индекса для использования в обработчике
            bonusButtons[i].onClick.AddListener(() => OnBonusButtonClick(index));
        }
    }


    // Включаем бонус (кнопка активируется, полоска скрывается)
    void EnableBonus(int index)
    {
        bonusButtons[index].interactable = true;
        crossOutImages[index].gameObject.SetActive(false);  // Если бонус доступен, не показываем перечеркнутую картинку

        // Восстанавливаем изображение бонуса в его нормальную непрозрачность
        SetBonusImageAlpha(index, 1f);  // Полная непрозрачность
    }

    // Отключаем бонус (кнопка деактивируется, полоска не показывается, если бонус еще не собран)
    void DisableBonus(int index)
    {
        bonusButtons[index].interactable = false;

        // Если бонус еще не был собран, показываем прозрачность изображения
        if (PlayerPrefs.GetInt(BONUS_COLLECTED_KEY + (index + 1), 0) == 0)
        {
            crossOutImages[index].gameObject.SetActive(false);  // Если бонус еще не собран, картинка не показывается
            SetBonusImageAlpha(index, 0.5f);  // Прозрачность 50%
        }
        else
        {
            crossOutImages[index].gameObject.SetActive(true);  // Если бонус собран, показываем картинку
            SetBonusImageAlpha(index, 1f);  // Восстанавливаем нормальную прозрачность
        }
    }

    // Метод для установки прозрачности изображения бонуса
    void SetBonusImageAlpha(int index, float alpha)
    {
        Color color = bonusImages[index].color;
        color.a = alpha;  // Устанавливаем альфа-канал
        bonusImages[index].color = color;  // Применяем новый цвет с измененной прозрачностью
    }

    // При нажатии на кнопку бонуса
    void OnBonusButtonClick(int index)
    {
        // Получаем бонус
        AddBonusToPlayer(index);

        // Сохраняем, что бонус был собран
        PlayerPrefs.SetInt(BONUS_COLLECTED_KEY + (index + 1), 1);  // Сохраняем в PlayerPrefs, что бонус получен
        PlayerPrefs.Save();  // Сохраняем изменения
        ButtonSound(2);

        // Отключаем кнопку и включаем полоску
        DisableBonus(index);
    }

    // Добавляем бонус игроку (монеты)
    void AddBonusToPlayer(int index)
    {
        // Загружаем количество монет игрока из PlayerPrefs
        int playerCoins = PlayerPrefs.GetInt(PLAYER_COINS_KEY, 0);  // Если монеты не сохранены, то 1000 по умолчанию

        // Добавляем бонус
        playerCoins += bonusAmounts[index];

        // Сохраняем обновленное количество монет обратно в PlayerPrefs
        PlayerPrefs.SetInt(PLAYER_COINS_KEY, playerCoins);

        // Логируем текущий баланс монет
        Debug.Log($"Получено {bonusAmounts[index]} монет! Текущий баланс: {playerCoins}");
    }

    public void ButtonSound(int sound)
    {
        AudioManager.Instance.PlaySFX(sound); // Воспроизводит переданный звук
    }
}
