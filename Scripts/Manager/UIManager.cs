using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class UIManager : Singleton<UIManager>
{
    int _order = 10; // 현재까지 최근에 사용한 오더

    Stack<UI_Popup> _popupStack = new Stack<UI_Popup>(); // 오브젝트 말고 컴포넌트를 담음. 팝업 캔버스 UI 들을 담는다.

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
    /// 동적생성 - UISubItem
    /// Resources/Prefabs/UI/SubItem/에 있는 프리팹을 로드해서 만든다
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
    /// 동적생성 - UIScene
    /// Resources/Prefabs/UI/Scene/ 에 있는 프리팹을 로드해서 만든다
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
    /// 동적생성 - UIPopup
    /// Resources/Prefabs/UI/Popup/ 에 있는 프리팹을 로드해서 만든다
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
    /// 해당하는 캔버스 sortingOrder를 설정한다
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
    /// 팝업 끄기 
    /// 가장 위에 있는 것만 지울 수 있다 
    /// </summary>
    /// <param name="popup"></param>
    public void DestoryUIPopup(UI_Popup popup)
    {
        if (_popupStack.Count == 0)
        {
            return;
        }

        // 맨 위 팝업만 지울 수 있다
        if (_popupStack.Peek() != popup)
        {
            Debug.Log("Close Popup Failed!");
            return;
        }

        _popupStack.Pop();

        // 팝업이 널인데 스택에 남아있을 때
        if (popup == null)
        {
            _order--;
            return;
        }

        // 팝업스크립트 붙어있는 오브젝트가 삭제되었을 때
        if (popup != null && popup.gameObject == null)
        {
            popup = null;
            _order--;
            return;
        }

        // 그냥 삭제하는 일반적인 케이스
        if (popup != null && popup.gameObject != null)
        {
            Destroy(popup.gameObject);
            popup = null;
            _order--;
            return;
        }
    }

    /// <summary>
    /// 팝업 다 끄기
    /// </summary>
    public void DestoryAllUIPopup()
    {
        while (_popupStack.Count > 0)
        {
            UI_Popup popup = _popupStack.Pop();

            // 씬 넘어가는데 팝업 삭제 안하고 씬 넘겼을 때
            if (popup == null)
            {
                _order--;
                continue;
            }

            // 팝업스크립트 붙어있는 오브젝트가 삭제되었을 때
            if (popup != null && popup.gameObject == null)
            {
                popup = null;
                _order--;
                continue;
            }

            // 일반적으로 팝업 다 삭제하려고 할 떄
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
    /// 현재 스택에 있는 팝업을 찾아서 리턴한다
    /// UIManager.Instance.FindUIPopup<UIPopup_Play>()?.RefreshHpBar(); 이렇게 사용
    /// </summary>

    public T FindUIPopup<T>() where T : UI_Popup
    {
        return _popupStack.Where(x => x.GetType() == typeof(T)).FirstOrDefault() as T;
    }

    #endregion


}