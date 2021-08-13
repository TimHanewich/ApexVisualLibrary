using System;
using ApexVisual.SessionManagement;
using ApexVisual.Analysis;


namespace ApexVisual.LiveCoaching
{
    public delegate void CornerChangedEventHandler(byte new_corner);
    public delegate void CornerStageChangedEventHandler(CornerStage new_stage);
    public delegate void CornerApexComparisonAvailableHandler(CommonCarData ccd, TrackLocationOptima opt);
}