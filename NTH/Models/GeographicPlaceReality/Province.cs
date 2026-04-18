namespace NTH.Models.GeographicPlaceReality;

public class Province
{
	public required string Name { get; set; }
	public required string Suffix { get; set; }
	public char Abbr { get; set; }
	public long Code { get; set; }
	public long Population { get; set; }
	public long Area { get; set; } // 单位：平方公里（km²）
}
