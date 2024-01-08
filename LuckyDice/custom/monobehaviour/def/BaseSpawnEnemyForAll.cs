using GameNetcodeStuff;

namespace LuckyDice.custom.monobehaviour.def
{
    public abstract class BaseSpawnEnemyForAll<TEnemy> : BaseSpawnEnemyEvent<TEnemy> where TEnemy : EnemyAI 
    {
        public override void AddPlayer(PlayerControllerB player)
        {
            foreach (PlayerControllerB p in StartOfRound.Instance.allPlayerScripts)
            {
                if (player.isPlayerDead || !player.isPlayerControlled)
                    continue;
                base.AddPlayer(p);
            }
        }
    }
}