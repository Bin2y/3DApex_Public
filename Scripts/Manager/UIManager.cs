using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class UIManager : Singleton<UIManager>
{
    int _order = 10; // ������� �ֱٿ� ����� ����

    Stack<UI_Popup> _popupStack = new Stack<UI_Popup>(); // ������Ʈ ���� ������Ʈ�� ����. �˾� ĵ���� UI ���� ��´�.

    public GameObject Root
    {
        get
        {
            GameObject root = GameObject.Find("@UI_Root");
            if (root == null)
                root = new GameObject { name = "@UI_Root" };
            return root;
        }
    }

    protected override void Awake()
    {
        _isDontDestroyOnLoad = true;
        base.Awake();
    }

    public UI_Scene UIScene { get; private set; }
    public void SetUIScene(GameObject go, bool isNotOverlay = false)
    {
        Canvas canvas = go.GetOrAddComponent<Canvas>();
        if (!isNotOverlay) canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.overrideSorting = true;
        canvas.sortingOrder = -25;

        UI_Scene uiScene = go.GetOrAddComponent<UI_Scene>();
        UIScene = uiScene;

        go.transform.SetParent(Root.transform);
    }
    /// <summary>
    /// �������� - UISubItem
    /// Resources/Prefabs/UI/SubItem/�� �ִ� �������� �ε��ؼ� �����
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="parent"></param>
    /// <param name="name"></param>
    /// <returns></returns>
    public T InstanciateSubItem<T>(Transform parent = null, string name = null) where T : UI_SubItem
    {
        if (string.IsNullOrEmpty(name))
        {
            name = typeof(T).Name;
        }

        GameObject prefab = ResourceManager.Instance.Load<GameObject>($"Prefabs/UI/UISubItem/{name}");

        GameObject go = Instantiate(prefab);
        if (parent != null)
        {
            go.transform.SetParent(parent);
        }

        go.transform.localScale = Vector3.one;
        go.transform.localPosition = prefab.transform.position;

        return go.GetOrAddComponent<T>();
    }

    /// <summary>
    /// �������� - UIScene
    /// Resources/Prefabs/UI/Scene/ �� �ִ� �������� �ε��ؼ� �����
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="name"></param>
    /// <returns></returns>
    public T InstanciateUIScene<T>(string name = null) where T : UI_Scene
    {
        if (string.IsNullOrEmpty(name))
        {
            name = typeof(T).Name;
        }

        GameObject go = ResourceManager.Instance.Instantiate($"UI/UIScene/{name}");

        T sceneUI = go.GetOrAddComponent<T>();

        go.transform.SetParent(Root.transform);

        return sceneUI;
    }


    #region UIPopup

    /// <summary>
    /// �������� - UIPopup
    /// Resources/Prefabs/UI/Popup/ �� �ִ� �������� �ε��ؼ� �����
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="name"></param>
    /// <param name="parent"></param>
    /// <returns></returns>
    public T InstanciateUIPopup<T>(string name = null, Transform parent = null) where T : UI_Popup
    {
        if (string.IsNullOrEmpty(name))
        {
            name = typeof(T).Name;
        }

        GameObject go = ResourceManager.Instance.Instantiate($"UI/UIPopup/{name}");
        T popup = go.GetOrAddComponent<T>();
        _popupStack.Push(popup);

        if (parent != null)
        {
            go.transform.SetParent(parent);
        }
        else
        {
            go.transform.SetParent(Root.transform);
        }

        go.transform.localScale = Vector3.one;

        return popup;
    }

    /// <summary>
    /// �ش��ϴ� ĵ���� sortingOrder�� �����Ѵ�
    /// </summary>
    /// <param name="go"></param>
    /// <param name="sort"></param>
    public void SetUIPopup(GameObject go, bool sort = true)
    {
        Canvas canvas = go.GetOrAddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.overrideSorting = true;

        if (sort)
        {
            canvas.sortingOrder = _order;
            _order++;
        }
        else
        {
            canvas.sortingOrder = 0;
        }
    }

    /// <summary>
    /// �˾� ���� 
    /// ���� ���� �ִ� �͸� ���� �� �ִ� 
    /// </summary>
    /// <param name="popup"></param>
    public void DestoryUIPopup(UI_Popup popup)
    {
        if (_popupStack.Count == 0)
        {
            return;
        }

        // �� �� �˾��� ���� �� �ִ�
        if (_popupStack.Peek() != popup)
        {
            Debug.Log("Close Popup Failed!");
            return;
        }

        _popupStack.Pop();

        // �˾��� ���ε� ���ÿ� �������� ��
        if (popup == null)
        {
            _order--;
            return;
        }

        // �˾���ũ��Ʈ �پ��ִ� ������Ʈ�� �����Ǿ��� ��
        if (popup != null && popup.gameObject == null)
        {
            popup = null;
            _order--;
            return;
        }

        // �׳� �����ϴ� �Ϲ����� ���̽�
        if (popup != null && popup.gameObject != null)
        {
            Destroy(popup.gameObject);
            popup = null;
            _order--;
            return;
        }
    }

    /// <summary>
    /// �˾� �� ����
    /// </summary>
    public void DestoryAllUIPopup()
    {
        while (_popupStack.Count > 0)
        {
            UI_Popup popup = _popupStack.Pop();

            // �� �Ѿ�µ� �˾� ���� ���ϰ� �� �Ѱ��� ��
            if (popup == null)
            {
                _order--;
                continue;
            }

            // �˾���ũ��Ʈ �پ��ִ� ������Ʈ�� �����Ǿ��� ��
            if (popup != null && popup.gameObject == null)
            {
                popup = null;
                _order--;
                continue;
            }

            // �Ϲ������� �˾� �� �����Ϸ��� �� ��
            if (popup != null && popup.gameObject != null)
            {
                Destroy(popup.gameObject);
                popup = null;
                _order--;
                continue;
            }
        }
    }

    /// <summary>
    /// ���� ���ÿ� �ִ� �˾��� ã�Ƽ� �����Ѵ�
    /// UIManager.Instance.FindUIPopup<UIPopup_Play>()?.RefreshHpBar(); �̷��� ���
    /// </summary>

    public T FindUIPopup<T>() where T : UI_Popup
    {
        return _popupStack.Where(x => x.GetType() == typeof(T)).FirstOrDefault() as T;
    }

    #endregion


}