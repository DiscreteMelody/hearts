using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace Hearts_AI
{
    static class Simulator
    {
        public static object cloneGame(Game game_in_progress)
        {
            object clonedGame = null;
            MemoryStream memoryStream = new MemoryStream();
            BinaryFormatter binaryFormatter = new BinaryFormatter();

            binaryFormatter.Serialize(memoryStream, game_in_progress);
            memoryStream.Position = 0;

            clonedGame = (Game)binaryFormatter.Deserialize(memoryStream);

            return clonedGame;
        }
    }
}
