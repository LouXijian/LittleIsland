using LittleIsland.Core;
using LittleIsland.Model;

namespace LittleIsland.Gameplay
{
    /// <summary>
    /// This event is fired when user input should be enabled.
    /// </summary>
    public class EnablePlayerInput : Simulation.Event<EnablePlayerInput>
    {
        LittleIslandModel model = Simulation.GetModel<LittleIslandModel>();

        public override void Execute()
        {
            var player = model.player;
            player.controlEnabled = true;
        }
    }
}