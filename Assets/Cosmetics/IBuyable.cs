using System;

[Serializable]
public struct LimitedTime
{
	public int limitedDay;
	public int limitedMonth;
	public int limitedYear;
	public int limitedHour;
	public int limitedMinute;
}

[Serializable]
public struct LimitedTimeStartEnd
{
    public LimitedTime timeStart;
    public LimitedTime timeEnd;
}


public interface IBuyable
{
    string ProdId { get; }
    int BeanCost { get; }
    int StarCost { get; }
    bool PaidOnMobile { get; }
    LimitedTimeStartEnd LimitedTimeAvailable { get; }
}