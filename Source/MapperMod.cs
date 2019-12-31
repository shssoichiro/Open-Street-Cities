using ICities;

namespace Mapper
{
    public class MapperMod : IUserMod
    {
        public string Name
        {
            get
            {
                return "Open Street Cities";
            }
        }
        public string Description
        {
            get
            {
                return "Import roads from Open Street Map";
            }
        }
    }
}
