using UnityEngine.UI;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Exit : MonoBehaviour
{
    [SerializeField] private Button exit;

    void Start()
    {
        exit.onClick.AddListener(OnClick);
    }

    private void OnClick()
    {
        SceneManager.LoadSceneAsync(GameInfo.Instance.GetPreviousScene());
    }
}
