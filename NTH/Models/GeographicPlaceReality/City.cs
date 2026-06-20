namespace NTH.Models.GeographicPlaceReality;

public class City
{
	public required string Name { get; set; }
	public long Code { get; set; }
	public long Population { get; set; }
	public long Area { get; set; } // 单位：平方公里（km²）
}
