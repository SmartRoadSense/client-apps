using System;
namespace SmartRoadSense
{
    public interface IStatsView
    {
        StatsPage CurrentPage { get; }
    }
}
