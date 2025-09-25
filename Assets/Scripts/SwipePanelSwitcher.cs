using UnityEngine;

public class SwipePanelSwitcher : MonoBehaviour
{
    public GameObject[] panels; // Массив панелей
    private Vector2 touchStartPos; // Стартовая позиция для свайпа
    private Vector2 touchEndPos; // Конечная позиция для свайпа

    private int currentPanel = 0; // Индекс текущей активной панели

    void Update()
    {
        if (Input.touchCount > 0)  // Для мобильных устройств
        {
            Touch touch = Input.GetTouch(0);
            HandleTouch(touch.position, touch.phase);
        }
        else if (Input.GetMouseButtonDown(0))  // Для ПК (с мышью)
        {
            touchStartPos = Input.mousePosition;
        }
        else if (Input.GetMouseButtonUp(0))
        {
            touchEndPos = Input.mousePosition;
            HandleTouch(touchEndPos, TouchPhase.Ended);
        }
    }

    // Обрабатываем свайпы
    void HandleTouch(Vector2 position, TouchPhase phase)
    {
        if (phase == TouchPhase.Began)
        {
            touchStartPos = position; // Запоминаем начальную позицию
        }
        else if (phase == TouchPhase.Ended)
        {
            touchEndPos = position; // Запоминаем конечную позицию

            // Рассчитываем разницу между начальной и конечной позициями
            float swipeDistance = touchEndPos.x - touchStartPos.x;

            // Если свайп влево (сильно в правую сторону)
            if (swipeDistance < -100f)
            {
                ButtonSound(1);
                ShowNextPanel();
            }
            // Если свайп вправо (сильно в левую сторону)
            else if (swipeDistance > 100f)
            {
                ButtonSound(1);
                ShowPreviousPanel();
            }
        }
    }

    void ShowNextPanel()
    {
        // Отключаем текущую панель
        panels[currentPanel].SetActive(false);

        // Переключаем на следующую панель (если не достигнут конец)
        currentPanel = (currentPanel + 1) % panels.Length;

        // Включаем следующую панель
        panels[currentPanel].SetActive(true);
    }

    void ShowPreviousPanel()
    {
        // Отключаем текущую панель
        panels[currentPanel].SetActive(false);

        // Переключаем на предыдущую панель (если не достигнут начало)
        currentPanel = (currentPanel - 1 + panels.Length) % panels.Length;

        // Включаем предыдущую панель
        panels[currentPanel].SetActive(true);
    }
    public void ButtonSound(int sound)
    {
        AudioManager.Instance.PlaySFX(sound); // Воспроизводит переденный звук
    }
}
