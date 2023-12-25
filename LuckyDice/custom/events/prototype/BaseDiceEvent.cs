﻿using System.Collections;
using System.Collections.Generic;
using GameNetcodeStuff;
using LuckyDice.custom.network;

namespace LuckyDice.custom.events.prototype
{
    public abstract class BaseDiceEvent : IDiceEvent
    {
        protected bool running = false;
        protected List<PlayerControllerB> players = new List<PlayerControllerB>();
        
        public void AddPlayer(PlayerControllerB player)
        {
            players.Add(player);
        }

        public void RemovePlayer(PlayerControllerB player)
        {
            players.Remove(player);
        }

        public void Run()
        {
            running = true;
            EventManager.Instance.StartCoroutine(EventCoroutine());
        }

        public void Stop()
        {
            running = false;
        }

        public abstract IEnumerator EventCoroutine();

        protected static bool IsPhaseForbidden()
        {
            return StartOfRound.Instance.inShipPhase || StartOfRound.Instance.currentLevelID == 3;
        }
    }
}