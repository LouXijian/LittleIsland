using LittleIsland.Core;
using LittleIsland.Mechanics;
using LittleIsland.Model;
using UnityEngine;

namespace LittleIsland.Gameplay
{
    /// <summary>
    /// Fired when a player collides with a token.
    /// </summary>
    /// <typeparam name="PlayerCollision"></typeparam>
    public class PlayerTokenCollision : Simulation.Event<PlayerTokenCollision>
    {
        public PlayerController player;
        public TokenInstance token;

        LittleIslandModel model = Simulation.GetModel<LittleIslandModel>();

        public override void Execute()
        {
            AudioSource.PlayClipAtPoint(token.tokenCollectAudio, token.transform.position);
        }
    }
}