using System;
namespace SmartRoadSense
{
    public class MainMenuItem
    {
        public MainMenuItem(Type type)
        {
            TargetType = type; // typeof(DetailPage);
        }
        public int Id { get; set; }
        public string Title { get; set; }

        public object[] PageArgs { get; set; }
        public Type TargetType { get; set; }
    }
}
