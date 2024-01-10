using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

namespace LuckyDice.custom.items.scrap
{
    public class DiscoMonkey : GrabbableObject
    {
        public AudioSource monkeyAudio = null!; 
        
        protected Light DiscoLight;
        protected IEnumerator Current;
        protected Renderer DiscoRenderer;
        
        protected static readonly List<Color> From =
        [
            Color.blue,
            Color.cyan,
            Color.green,
            Color.magenta,
            Color.red,
            Color.yellow
        ];
        
        public override void Start()
        {
            base.Start();
            DiscoLight = gameObject.GetComponentInChildren<Light>();
            DiscoRenderer = gameObject.GetComponentInChildren<Renderer>();
        }

        public override void ItemActivate(bool used, bool buttonDown = true)
        {
            base.ItemActivate(used, buttonDown);
            SwitchPlaying();
            SwitchLight();
        }

        private void SwitchPlaying()
        {
            if (monkeyAudio.isPlaying)
                monkeyAudio.Stop();
            else
                monkeyAudio.Play();
        }
        
        private void SwitchLight()
        {
            DiscoLight.enabled = !DiscoLight.enabled;
            if (DiscoLight.enabled)
            {
                Current = ChangeColor();
                StartCoroutine(Current);
            }
            else if (Current != null)
            {
                StopCoroutine(Current);
                Current = null;
            }
        }

        private IEnumerator ChangeColor()
        {
            var initial = PickRandomPresetColor();
            DiscoLight.color = initial;
            DiscoRenderer.material.color = initial;
            
            while (DiscoLight.enabled)
            {
                var newColor = PickRandomPresetColor();
                float transition = 0;
                while (transition < 1)
                {
                    var materialColor = Color.Lerp(initial, newColor, transition);
                    DiscoLight.color = materialColor;
                    DiscoRenderer.material.color = materialColor;
                    transition += 0.03f;
                    yield return new WaitForEndOfFrame();
                }
                yield return new WaitForSeconds(0.5f);
            }
        }

        private static Color PickRandomPresetColor()
        {
            return From[Random.Range(0, From.Count)];
        }
    }
}