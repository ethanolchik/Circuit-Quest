using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadLevel : MonoBehaviour
{
    private bool isLocked = true;
    private Button levelButton;
    void Start()
    {
        levelButton = GetComponent<Button>();
        levelButton.onClick.AddListener(OnClick);
    }

    void OnClick()
    {
        if (isLocked)
        {
            return;
        }

        // Reset the bridge manager json file
        string bridgeManagerPath = Application.dataPath + "/Resources/BridgeManager.json";
        if (System.IO.File.Exists(bridgeManagerPath))
        {
            System.IO.File.WriteAllText(bridgeManagerPath, "{}");
        }

        // Reset the health system file
        string healthSystemPath = Application.dataPath + "/Resources/HealthSystem.json";
        if (System.IO.File.Exists(healthSystemPath))
        {
            System.IO.File.WriteAllText(healthSystemPath, "{}");
        }

        SceneManager.LoadSceneAsync("Scenes/Levels/"+gameObject.name);
    }

    public void Unlock()
    {
        isLocked = false;
    }
}
