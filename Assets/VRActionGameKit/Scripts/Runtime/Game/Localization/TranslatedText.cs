﻿using TMPro;
using UnityEngine;

namespace _3DGamekitLite.Scripts.Runtime.Game.Localization
{
    public class TranslatedText : MonoBehaviour
    {
        public TextMeshProUGUI text;
        public string phraseKey;
        public bool setTextOnStart = true;

        void Awake ()
        {
            if (text == null)
                text = GetComponent<TextMeshProUGUI> ();
        }

        void Start ()
        {
            if(setTextOnStart)
                text.text = Translator.Instance[phraseKey];
        }

        public void SetText ()
        {
            text.text = Translator.Instance[phraseKey];
        }
    }
}