using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class ButtonFunctions : MonoBehaviour
{
    private Button button;
    private void Start()
    {
        button = gameObject.GetComponent<Button>();
        StartCoroutine(EnableButton());
    }

    IEnumerator EnableButton()
    {
        button.interactable = false;
        yield return new WaitForSeconds(0.5f);
        button.interactable = true;
    }

    public void PlayButton()
    {
        GameManager.Instance.LoadScene("Main level");
    }

    public void BackToMenuButton()
    {
        GameManager.Instance.BackToMenu();
    }

    public void QuitButton()
    {
        GameManager.Instance.QuitGame();
    }

    public void HoverButton()
    {
        SoundManager.Instance.ButtonHover();
    }

    public void ClickButton()
    {
        SoundManager.Instance.ButtonPressed();
    }

    public void RestartButton()
    {
        GameManager.Instance.RestartLevel();
    }

    public void ResumeGame()
    {
        GameManager.Instance.TogglePlayerMovement(true);
        GameManager.Instance.ToggleCursorVisibility(false);
    }
}
