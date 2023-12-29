#region

using GameNetcodeStuff;

#endregion

namespace LuckyDice.custom.events.implementation
{
    public class SpawnItemForAllEvent : SpawnItemEvent
    {
        public SpawnItemForAllEvent(int stackValue, int numberOfItems, int itemId)
            : base(stackValue, numberOfItems, itemId) {}
        
        public override void AddPlayer(PlayerControllerB player)
        {
            foreach (PlayerControllerB p in StartOfRound.Instance.allPlayerScripts)
            {
                if (player.isPlayerDead)
                    continue;
                base.AddPlayer(p);
            }
        }
    }
}