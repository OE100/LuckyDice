using LuckyDice.custom.network;

namespace LuckyDice.custom.items.dice
{
    public class D4 : DiceItem
    {
        public override void Start()
        {
            base.Start();
            outcomes.Add(Event.Bleed);
            // todo: add more outcomes
        }
    }
}