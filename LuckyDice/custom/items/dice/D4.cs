using LuckyDice.custom.network;

namespace LuckyDice.custom.items.dice
{
    public class D4 : DiceItem
    {
        public override void Start()
        {
            base.Start();
            outcomes.Add(Event.Bleed);
            outcomes.Add(Event.SpawnCentipede);
            // todo: add more outcomes
        }
    }
}