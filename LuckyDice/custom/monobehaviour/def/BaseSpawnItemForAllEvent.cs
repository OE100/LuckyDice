using GameNetcodeStuff;

namespace LuckyDice.custom.monobehaviour.def
{
    public abstract class BaseSpawnItemForAllEvent : BaseSpawnItemEvent
    {
        public override void AddPlayer(PlayerControllerB player)
        {
            foreach (var p in StartOfRound.Instance.allPlayerScripts)
            {
                if (player.isPlayerDead || !player.isPlayerControlled)
                    continue;
                base.AddPlayer(p);
            }
        }
    }
}