using System.Collections.Generic;

namespace Game.Model
{
    public class WorldInfoModel:IBaseModel
    {
        public List<World> Worlds { get; set; } = new();
        public List<Stage> Stages { get; set; } = new();
    }

    public class World
    {
        public int WolrdId { get; set; }
    }

    public class Stage
    {
        public int StageId { get; set; }
    }
}