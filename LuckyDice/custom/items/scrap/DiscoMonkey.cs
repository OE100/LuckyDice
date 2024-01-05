using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LuckyDice.custom.items.scrap
{
    public class DiscoMonkey : GrabbableObject
    {
        public AudioSource monkeyAudio; 
        
        private Light light;
        private IEnumerator current;
        private Renderer renderer;
        
        static List<Color> from = new List<Color>
        {
            Color.blue,
            Color.cyan,
            Color.green,
            Color.magenta,
            Color.red,
            Color.yellow
        };
        
        public override void Start()
        {
            base.Start();
            light = gameObject.GetComponentInChildren<Light>();
            renderer = gameObject.GetComponentInChildren<Renderer>();
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
            light.enabled = !light.enabled;
            if (light.enabled)
            {
                current = ChangeColor();
                StartCoroutine(current);
            }
            else if (current != null)
            {
                StopCoroutine(current);
                current = null;
            }
        }

        private IEnumerator ChangeColor()
        {
            Color initial = PickRandomPresetColor();
            light.color = initial;
            renderer.material.color = initial;
            
            while (light.enabled)
            {
                Color newColor = PickRandomPresetColor();
                float transition = 0;
                while (transition < 1)
                {
                    Color materialColor = Color.Lerp(initial, newColor, transition);
                    light.color = materialColor;
                    renderer.material.color = materialColor;
                    transition += 0.03f;
                    yield return new WaitForEndOfFrame();
                }
                yield return new WaitForSeconds(0.5f);
            }
        }

        private static Color PickRandomPresetColor()
        {
            return from[Random.Range(0, from.Count)];
        }
    }
}