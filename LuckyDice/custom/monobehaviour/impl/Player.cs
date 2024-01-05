using System;
using GameNetcodeStuff;
using LuckyDice.custom.events.implementation.player;
using UnityEngine;

namespace LuckyDice.custom.monobehaviour
{
    public class Player : MonoBehaviour
    {
        private PlayerControllerB _player = null;
        private bool _isJihad;
        private float _timeRemaining;
        
        private AudioSource _audioSource;
        
        private void Start()
        {
            _player = GetComponentInParent<PlayerControllerB>();
            _isJihad = false;
            _timeRemaining = 0f;
            
            _audioSource = Instantiate(_player.currentVoiceChatAudioSource);
            _audioSource.minDistance = 0;
        }
        
        public void SetJihad(float time)
        {
            _isJihad = true;
            _timeRemaining = time;
        }
        
        private void Update()
        {
            if (!_isJihad)
                return;
            
            if (StartOfRound.Instance.inShipPhase)
                ResetJihad();
            
            _timeRemaining -= Time.deltaTime;
            
            if (!_player.voiceMuffledByEnemy && _timeRemaining < 10f) 
                _player.voiceMuffledByEnemy = true;
            
            if (_timeRemaining < 0f) 
                Kaboom();
        }

        private void ResetJihad()
        {
            _audioSource.Stop();
            _isJihad = false;
            _player.voiceMuffledByEnemy = false;
            _timeRemaining = 0f;
        }
        
        private void Kaboom()
        {
            _player.voiceMuffledByEnemy = false;
            _isJihad = false;
            _timeRemaining = 0f;
            _audioSource.clip = HolyJihad.jihadClip;
            _audioSource.Play();
        }
    }
}