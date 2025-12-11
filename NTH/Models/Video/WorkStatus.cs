namespace NTH.Models.Video;

/// <summary>
/// Work status, in the first glance, is a work property
/// However, mostly, it describes video status with one work type
/// </summary>
public enum WorkStatus : int
{
    NeverTouched = 0,
    Assigned = 10,
    InProgress = 20,
    Ready = 30,
    Done = 40,
    Uploaded = 99
}
