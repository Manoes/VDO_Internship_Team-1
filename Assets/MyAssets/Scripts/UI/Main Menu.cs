using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [Header("Logo")]
    [SerializeField] private RectTransform logo;

    [Header("Controls")]
    [SerializeField] private RectTransform controlsRoot;
    [SerializeField] private RectTransform joystick;
    [SerializeField] private RectTransform[] buttons;

    void Start()
    {
        LockCursor();

        if (logo != null)
        {
            logo
                .DOScale(1.05f, 1.2f)
                .SetEase(Ease.InOutSine)
                .SetLoops(-1, LoopType.Yoyo);
        }

        AnimateControls();
    }

    private void LockCursor()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void AnimateControls()
    {
        if (controlsRoot != null)
        {
            controlsRoot
                .DOAnchorPosY(controlsRoot.anchoredPosition.y + 10, 1.5f)
                .SetEase(Ease.InOutSine)
                .SetLoops(-1, LoopType.Yoyo);
        }

        if (joystick != null)
        {
            joystick
                .DORotate(new Vector3(0, 0, 5f), 0.75f)
                .SetEase(Ease.InOutSine)
                .SetLoops(-1, LoopType.Yoyo);
        }

        for (int i = 0; i < buttons.Length; i++)
        {
            RectTransform button = buttons[i];

            button
                .DOScale(1.15f, 0.4f)
                .SetDelay(i * 0.15f)
                .SetLoops(-1, LoopType.Yoyo);
        }
    }

    public void PlayButton()
    {
        SceneManager.LoadScene("Main Game");
    }

    public void QuitButton()
    {
        Application.Quit();
    }
}
