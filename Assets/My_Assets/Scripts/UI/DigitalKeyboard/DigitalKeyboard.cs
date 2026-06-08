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
    yield return new WaitForSeconds(buttonSpawnDelay);
    bool notDone = true;
    previousPitch = 1;
    while (notDone)
    {
      for (int i = 0; i < letters.Length; i++)
      {
        string _letter = letters[i].ToString();
        GameObject newButton = Instantiate(buttonPrefab);
        newButton.GetComponent<ButtonSelect>().SetAudioSource(audioSource);
        newButton.GetComponent<AddCharacter>().SetAudioSource(audioSource);
        newButton.name = _letter;
        TMP_Text text = newButton.GetComponentInChildren<TMP_Text>();
        text.SetText(_letter);
        newButton.transform.SetParent(transform);
        newButton.transform.localPosition = new Vector3(0, 0, 0);
        newButton.transform.localScale = new Vector3(1, 1, 1);
        previousPitch += 0.115f;
        PlayRandomSound(previousPitch);
        yield return new WaitForSeconds(0.003f);
      }
      notDone = false;
    }

    yield return null;
  }


  private void PlayRandomSound(float pitch)
  {
    audioSource.pitch = pitch;
    audioSource.PlayOneShot(soundEffect);
  }

}
