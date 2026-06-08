using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class DigitalKeyboard : MonoBehaviour
{
  [SerializeField] private char[] letters;
  [SerializeField] private GameObject buttonPrefab;
  [SerializeField] private AudioSource audioSource;
  [SerializeField] private AudioClip soundEffect;
  [SerializeField] private float buttonSpawnDelay = 0.03f;

  private float previousPitch = 1;

  private void Start()
  {
    StartCoroutine(SpawnButtons());
  }

  private IEnumerator SpawnButtons()
  {
    if (buttonPrefab == null)
    {
      Debug.LogError("[DigitalKeyboard] Button Prefab is not assigned!");
      yield break;
    }

    yield return new WaitForSecondsRealtime(buttonSpawnDelay);

    previousPitch = 1f;

    for (int i = 0; i < letters.Length; i++)
    {
      string letter = letters[i].ToString();

      GameObject newButton = Instantiate(buttonPrefab, transform);
      newButton.name = letter;

      TMP_Text text = newButton.GetComponentInChildren<TMP_Text>();
      if (text != null)
        text.SetText(letter);

      if (newButton.TryGetComponent<Button>(out Button button))
      {
        string capturedLetter = letter;

        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(() =>
        {
          DeathUIManager.Instance.AddCharacter(capturedLetter);
        });
      }

      if (newButton.TryGetComponent<ButtonSelect>(out var select))
        select.SetAudioSource(audioSource);

      if (newButton.TryGetComponent<RectTransform>(out var rt))
      {
        rt.localScale = Vector3.one;
        rt.anchoredPosition = Vector2.zero;
      }

      previousPitch = Mathf.Clamp(previousPitch + 0.115f, -3f, 3f);

      if (audioSource != null && soundEffect != null && audioSource.isActiveAndEnabled)
        PlayRandomSound(previousPitch);

      yield return new WaitForSecondsRealtime(0.01f);
    }
  }

  private void PlayRandomSound(float pitch)
  {
    audioSource.pitch = pitch;
    audioSource.PlayOneShot(soundEffect);
  }
}