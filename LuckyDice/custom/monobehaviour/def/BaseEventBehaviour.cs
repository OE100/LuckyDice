﻿using UnityEngine;

namespace LuckyDice.custom.monobehaviour.def
{
    public class BaseEventBehaviour : MonoBehaviour
    {
        protected virtual bool IsPhaseForbidden()
        {
            return StartOfRound.Instance.inShipPhase || StartOfRound.Instance.currentLevelID == 3;
        }

        protected virtual void Update()
        {
            if (IsPhaseForbidden())
                Destroy(this);
        }
    }
}