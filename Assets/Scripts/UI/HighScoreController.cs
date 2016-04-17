using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Extra;
using Assets.Scripts.Player;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Assets.Scripts.UI
{
    public class HighScoreController : MonoBehaviour, IVisible, IUiControl
    {
        public GameObject EntryPrefab;
        public GameObject EntryParent;
        public GameObject ConfirmPanel;

        private List<PlayerBehaviour> _players;
        private CanvasGroup _canvasGroup;

        private float _alpha = 0.0f;

        void Awake()
        {
            _canvasGroup = GetComponent<CanvasGroup>();
            _players = new List<PlayerBehaviour>();
        }

        void Update()
        {
            CheckVisibilityChanges();
        }

        public bool CheckVisibilityChanges()
        {
            if (Mathf.Abs(_canvasGroup.alpha - _alpha) > 0.0001)
            {
                _canvasGroup.alpha = Mathf.Lerp(_canvasGroup.alpha, _alpha, 0.1f);
                return true;
            }
            return false;
        }

        public void IsVisible(bool visible)
        {
            _alpha = visible ? 1.0f : 0.0f;
            _canvasGroup.interactable = visible;
        }

        public void Set(List<PlayerBehaviour> playerBehaviours)
        {
            RemoveExisting();
            _players = playerBehaviours.OrderByDescending(pb => pb.Score.Points).ToList();
            GenerateTextBoxes();
        }

        public void NextBattle()
        {
            foreach (PlayerBehaviour player in _players)
            {
                player.SetActive(false);
                player.transform.parent = null;
                DontDestroyOnLoad(player.gameObject);
            }
            SceneManager.LoadScene("ShopScene");
        }

        

        private void RemoveExisting()
        {
            List<GameObject> objects = GameObject.FindGameObjectsWithTag("HighscoreEntry").ToList();
            objects.ForEach(Destroy);
        }

        private void GenerateTextBoxes()
        {
            bool isLast = false;

            for (int i = 0; i < _players.Count; i++)
            {
                if (i == _players.Count - 1) isLast = true;

                GenerateEntry(_players[i], isLast);
            }
        }

        private void GenerateEntry(PlayerBehaviour pb, bool isLast)
        {
            var entry = Instantiate(EntryPrefab);
            entry.transform.SetParent(this.EntryParent.transform, false);
            entry.GetComponent<RectTransform>().localScale = Vector3.one;

            Text[] fields = entry.GetComponentsInChildren<Text>();

            fields[0].text = pb.Name;
            fields[1].text = pb.Score.Kills.ToString();
            fields[2].text = pb.Score.Points.ToString();
            fields[3].text = pb.Score.Money.ToString();

            if (isLast)
            {
                entry.GetComponent<Image>().enabled = false;
            }
        }

    }
}
