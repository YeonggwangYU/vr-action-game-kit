﻿using UnityEngine;
using UnityEngine.Events;

namespace _3DGamekitLite.Scripts.Runtime.Game.Core
{
    public class InteractOnButton : InteractOnTrigger
    {

        public string buttonName = "X";
        public UnityEvent OnButtonPress;

        bool canExecuteButtons = false;

        protected override void ExecuteOnEnter(Collider other)
        {
            canExecuteButtons = true;
        }

        protected override void ExecuteOnExit(Collider other)
        {
            canExecuteButtons = false;
        }

        void Update()
        {
            if (canExecuteButtons && Input.GetButtonDown(buttonName))
            {
                OnButtonPress.Invoke();
            }
        }

    } 
}
