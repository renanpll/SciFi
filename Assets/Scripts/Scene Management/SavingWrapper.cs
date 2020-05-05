using SciFi.Saving;
using System.Collections;
using UnityEngine;

namespace SciFi.SceneManagement
{
    public class SavingWrapper : MonoBehaviour
    {
        const string _defaultSaveFile = "save";

        private void Awake()
        {
            StartCoroutine(LoadLastScene());
        }

        private IEnumerator LoadLastScene()
        {
            yield return GetComponent<SavingSystem>().LoadLastScene(_defaultSaveFile);
            Fader fader = FindObjectOfType<Fader>();
            fader.FadeOutImmediate();

            yield return fader.FadeIn(1f);
        }
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.S))
            {
                Save();
            }

            if (Input.GetKeyDown(KeyCode.L))
            {
                Load();
            }

            if (Input.GetKeyDown(KeyCode.Delete))
            {
                Delete();
            }
        }

        public void Load()
        {
            GetComponent<SavingSystem>().Load(_defaultSaveFile);
        }

        public void Save()
        {
            GetComponent<SavingSystem>().Save(_defaultSaveFile);
        }

        public void Delete()
        {
            GetComponent<SavingSystem>().Delete(_defaultSaveFile);
        }
    }

}
