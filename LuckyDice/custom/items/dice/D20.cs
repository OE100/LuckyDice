using LuckyDice.custom.network;

namespace LuckyDice.custom.items.dice
{
    public class D20 : DiceItem
    {
        public override void Start()
        {
            base.Start();
            outcomes.Add(Event.RandomizeLocks);
            // todo: add more outcomes
        }
    }
}