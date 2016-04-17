using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Extra
{
    public class TextReplacer : MonoBehaviour
    {
        private Text _text;

        void Awake()
        {
            _text = GetComponent<Text>();
            _text.text = _text.text.Replace("\\n", "\n");
        }
    }
}
