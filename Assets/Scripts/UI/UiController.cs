using System;
using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.Player;
using UnityEngine;

namespace Assets.Scripts.UI
{
    public class UiController : MonoBehaviour
    {
        public bool IsReady { get; set; }

        public InventoryController InventoryController { get; set; }
        public TimerController TimerController { get; set; }
        public MessageController MessageController { get; set; }
        public PlayerStatsController PlayerStatsController { get; set; }
        public LoadbarController LoadbarController { get; set; }
        public HighScoreController HighScoreController { get; set; }
        public ConfirmPanelController ConfirmPanelController { get; set; }
        private List<MonoBehaviour> UiControllers { get; set; }

        void Awake()
        {
            GetControllers();
        }

        void Update()
        {

        }

        private void GetControllers()
        {
            this.InventoryController = GetComponentInChildren<InventoryController>();
            this.TimerController = GetComponentInChildren<TimerController>();
            this.MessageController = GetComponentInChildren<MessageController>();
            this.PlayerStatsController = GetComponentInChildren<PlayerStatsController>();
            this.LoadbarController = GetComponentInChildren<LoadbarController>();
            this.HighScoreController = GetComponentInChildren<HighScoreController>();
            this.ConfirmPanelController = GetComponentInChildren<ConfirmPanelController>();
            this.UiControllers = new List<MonoBehaviour>() { InventoryController, TimerController, MessageController, PlayerStatsController, LoadbarController, HighScoreController};
            this.IsReady = true;
        }

        public void DisplayUi(bool enable = false)
        {
            Camera.main.cullingMask = !enable ? ~(1 << 0 | 1 << 9) : ~(1 << 9);
            GameObject.FindGameObjectWithTag("Parallax").GetComponent<Camera>().cullingMask = !enable ? 0 : 1 << 9;
            foreach (var controller in this.UiControllers)
                if (!(controller is LoadbarController) && !(controller is HighScoreController))
                {
                    controller.gameObject.SetActive(enable);
                    if (controller is IVisible)
                    {
                        ((IVisible)controller).IsVisible(enable);
                    }
                }   
                else
                    controller.gameObject.SetActive(!enable);
        }

        public void DisplayHighscore(List<PlayerBehaviour> players)
        {
            foreach (var controller in this.UiControllers)
            {
                if (controller is HighScoreController)
                {
                    ((HighScoreController)controller).gameObject.SetActive(true);
                    ((HighScoreController)controller).Set(players);
                    ((HighScoreController)controller).IsVisible(true);
                    continue;
                }

                if (!(controller is LoadbarController))
                {
                    if (controller is IVisible)
                    {
                        ((IVisible)controller).IsVisible(false);
                        StartCoroutine(DisableControllerWhenHidden(1, ((IVisible)controller)));
                    }
                    else
                        controller.gameObject.SetActive(false);
                }   
            }
        }

        private IEnumerator DisableControllerWhenHidden(float tickDuration, IVisible controller)
        {
            while (true)
            {
                if (!(controller.CheckVisibilityChanges()))
                    break;
                yield return new WaitForSeconds(tickDuration);
            }
            ((MonoBehaviour)controller).gameObject.SetActive(false);
        }

        public IEnumerator DisplayWhenReady(Action toDo, Action continueWith = null)
        {
            while (true)
            {
                if (!this.IsReady)
                {
                    yield return new WaitForSeconds(0.1f);
                    continue;
                }
                toDo();
                if (continueWith != null)
                    continueWith();
                break;
            }
        }
    }
}
