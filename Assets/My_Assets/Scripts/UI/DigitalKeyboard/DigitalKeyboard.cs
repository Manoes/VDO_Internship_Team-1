using TMPro;
using UnityEngine;
using System.Collections;
using System;

public class DigitalKeyboard : MonoBehaviour
{
  [SerializeField] char[] letters;
  [SerializeField] GameObject buttonPrefab;
  [SerializeField] AudioSource audioSource;
  [SerializeField] AudioClip soundEffect;
  [SerializeField] float buttonSpawnDelay = 0.03f;
  float previousPitch = 1;

  //  [SerializeField] TMP_Text playerName;
  //   static public TMP_Text staticPlayerName;
  void Start()
  {
    StartCoroutine(SpawnButtons());
  }

  IEnumerator SpawnButtons()
  {
    if (buttonPrefab == null)
    {
      Debug.LogError("[DigitalKeyboard] Button Prefab is not assigned!");
      yield break;
    }

    yield return new WaitForSeconds(buttonSpawnDelay);
    previousPitch = 1f;

    for (int i = 0; i < letters.Length; i++)
    {
      try
      {
        string _letter = letters[i].ToString();
        
        // Instantiate with parent immediately is more efficient for UI
        GameObject newButton = Instantiate(buttonPrefab, transform);
        newButton.name = _letter;

        // Use TryGetComponent to prevent the coroutine from crashing if a component is missing
        if (newButton.TryGetComponent<ButtonSelect>(out var select))
          select.SetAudioSource(audioSource);
          
        if (newButton.TryGetComponent<AddCharacter>(out var addChar))
          addChar.SetAudioSource(audioSource);

        TMP_Text text = newButton.GetComponentInChildren<TMP_Text>();
        if (text != null)
          text.SetText(_letter);

        // Using RectTransform for UI ensures layout groups behave correctly
        if (newButton.TryGetComponent<RectTransform>(out var rt))
        {
          rt.localScale = Vector3.one;
          rt.anchoredPosition = Vector2.zero;
        }

        // Clamp pitch to prevent audio errors and update sound playback
        previousPitch = Mathf.Clamp(previousPitch + 0.115f, -3f, 3f);
        if (audioSource != null && soundEffect != null && audioSource.isActiveAndEnabled)
        {
          PlayRandomSound(previousPitch);
        }
      }
      catch (System.Exception e)
      {
        Debug.LogError($"[DigitalKeyboard] Exception during button spawn at index {i}: {e.Message}");
      }

      // A slightly longer delay helps Unity UI and the Audio system stay in sync
      yield return new WaitForSeconds(0.01f);
    }
  }


  private void PlayRandomSound(float pitch)
  {
    audioSource.pitch = pitch;
    audioSource.PlayOneShot(soundEffect);
  }

}
