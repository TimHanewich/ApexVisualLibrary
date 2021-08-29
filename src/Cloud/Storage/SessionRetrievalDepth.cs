using System;

namespace ApexVisual.Cloud.Storage
{
    public enum SessionRetrievalDepth
    {
        Summary = 0, //Just the details about the session (track, team, driver, created at, etc)
        Lap = 1, //Everything above, and then details about each lap (Sector times, lap invalid, etc.)
        Full = 2 //ALL DATA
    }
}