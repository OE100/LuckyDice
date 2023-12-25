using LuckyDice.Patches.custom.events;
using LuckyDice.Patches.custom.network;

namespace LuckyDice.Patches.custom.items.dice
{
    public class D20 : DiceItem
    {
        public override void Start()
        {
            base.Start();
            outcomes.Add(Event.BleedEvent);
        }
    }
}