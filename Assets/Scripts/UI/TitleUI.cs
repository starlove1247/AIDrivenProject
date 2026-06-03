using UnityEngine;
using UnityEngine.UI;

public class TitleUI : MonoBehaviour
{
    [SerializeField] Button startButton;

    void Start()
    {
        startButton.onClick.AddListener(() =>
            SceneLoader.Instance.LoadSceneWithFade("MainScene"));
    }
}
