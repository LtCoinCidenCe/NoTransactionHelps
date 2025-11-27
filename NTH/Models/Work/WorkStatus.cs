namespace NTH.Models.Work;

public enum WorkStatus : int
{
    NeverTouched = 0,
    Assigned = 10,
    InProgress = 20,
    Ready = 30,
    Done = 40,
    Uploaded = 99
}
