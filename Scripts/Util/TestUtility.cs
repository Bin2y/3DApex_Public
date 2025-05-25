using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

namespace yoon
{
    public class TestUtility : MonoBehaviour
    {
        public static Canvas canvas; // UI를 표시할 캔버스
        public static GameObject buttonPrefab; // 버튼 프리팹

        private void Awake()
        {
            if (canvas == null)
            {
                canvas = GameObject.Find("TestUI").GetComponent<Canvas>();
            }
            buttonPrefab = Resources.Load<GameObject>("Button");
        }

        public static void ButtonUI(Vector2 position, string text, UnityAction action)
        {
            if (buttonPrefab == null)
            {
                Debug.LogError("Button Prefab is not assigned.");
                return;
            }

            GameObject newButton = Instantiate(buttonPrefab, canvas.transform);
            RectTransform rectTransform = newButton.GetComponent<RectTransform>();
            rectTransform.anchoredPosition = position;

            Text buttonText = newButton.GetComponentInChildren<Text>();
            if (buttonText != null)
            {
                buttonText.text = text;
            }

            Button btn = newButton.GetComponent<Button>();
            btn.onClick.AddListener(action);
            
        }
    }
}