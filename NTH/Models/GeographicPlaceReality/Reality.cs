using System.Collections.Immutable;

namespace NTH.Models.GeographicPlaceReality;

public static class Reality
{
	static Reality()
	{
		Provinces = [
			new() { Name = "北京", Suffix = "市", Abbr = '京', Code = 110000, Population = 2154_0000, Area = 16410 },
			new() { Name = "天津", Suffix = "市", Abbr = '津', Code = 120000, Population = 1562_0000, Area = 11917 },
			new() { Name = "河北", Suffix = "省", Abbr = '冀', Code = 130000, Population = 7461_0000, Area = 188800 },
			new() { Name = "山西", Suffix = "省", Abbr = '晋', Code = 140000, Population = 3729_0000, Area = 156700 },
			new() { Name = "内蒙古", Suffix = "自治区", Abbr = '蒙', Code = 150000, Population = 2470_0000, Area = 1183000 },
			new() { Name = "辽宁", Suffix = "省", Abbr = '辽', Code = 210000, Population = 4259_0000, Area = 148000 },
			new() { Name = "吉林", Suffix = "省", Abbr = '吉', Code = 220000, Population = 2407_0000, Area = 187400 },
			new() { Name = "黑龙江", Suffix = "省", Abbr = '黑', Code = 230000, Population = 3185_0000, Area = 473000 },
			new() { Name = "上海", Suffix = "市", Abbr = '沪', Code = 310000, Population = 2487_0000, Area = 6340 },
			new() { Name = "江苏", Suffix = "省", Abbr = '苏', Code = 320000, Population = 8518_0000, Area = 107200 },
			new() { Name = "浙江", Suffix = "省", Abbr = '浙', Code = 330000, Population = 6457_0000, Area = 105500 },
			new() { Name = "安徽", Suffix = "省", Abbr = '皖', Code = 340000, Population = 6103_0000, Area = 139400 },
			new() { Name = "福建", Suffix = "省", Abbr = '闽', Code = 350000, Population = 4190_0000, Area = 121400 },
			new() { Name = "江西", Suffix = "省", Abbr = '赣', Code = 360000, Population = 4517_0000, Area = 166900 },
			new() { Name = "山东", Suffix = "省", Abbr = '鲁', Code = 370000, Population = 10043_0000, Area = 157900 },
			new() { Name = "河南", Suffix = "省", Abbr = '豫', Code = 410000, Population = 9785_0000, Area = 167000 },
			new() { Name = "湖北", Suffix = "省", Abbr = '鄂', Code = 420000, Population = 5775_0000, Area = 185900 },
			new() { Name = "湖南", Suffix = "省", Abbr = '湘', Code = 430000, Population = 6644_0000, Area = 211800 },
			new() { Name = "广东", Suffix = "省", Abbr = '粤', Code = 440000, Population = 12859_0000, Area = 179700 },
			new() { Name = "广西", Suffix = "自治区", Abbr = '桂', Code = 450000, Population = 4989_0000, Area = 236700 },
			new() { Name = "海南", Suffix = "省", Abbr = '琼', Code = 460000, Population = 1020_0000, Area = 35400 },
			new() { Name = "重庆", Suffix = "市", Abbr = '渝', Code = 500000, Population = 3124_0000, Area = 82400 },
			new() { Name = "四川", Suffix = "省", Abbr = '川', Code = 510000, Population = 8318_0000, Area = 486000 },
			new() { Name = "贵州", Suffix = "省", Abbr = '贵', Code = 520000, Population = 3857_0000, Area = 176100 },
			new() { Name = "云南", Suffix = "省", Abbr = '云', Code = 530000, Population = 4830_0000, Area = 394100 },
			new() { Name = "西藏", Suffix = "自治区", Abbr = '藏', Code = 540000, Population = 365_0000, Area = 1228400 },
			new() { Name = "陕西", Suffix = "省", Abbr = '陕', Code = 610000, Population = 3954_0000, Area = 205800 },
			new() { Name = "甘肃", Suffix = "省", Abbr = '甘', Code = 620000, Population = 2443_0000, Area = 425900 },
			new() { Name = "青海", Suffix = "省", Abbr = '青', Code = 630000, Population = 592_0000, Area = 722300 },
			new() { Name = "宁夏", Suffix = "自治区", Abbr = '宁', Code = 640000, Population = 720_0000, Area = 66400 },
			new() { Name = "新疆", Suffix = "自治区", Abbr = '新', Code = 650000, Population = 2585_0000, Area = 1664900 },
			new() { Name = "台湾", Suffix = "省", Abbr = '台', Code = 710000, Population = 2360_0000, Area = 36193 },
			new() { Name = "香港", Suffix = "特别行政区", Abbr = '港', Code = 810000, Population = 750_0000, Area = 1114 },
			new() { Name = "澳门", Suffix = "特别行政区", Abbr = '澳', Code = 820000, Population = 68_0000, Area = 33 },
		];
		ProvincesByPopulation = Provinces.OrderBy(x => x.Population).ToImmutableList();
		ProvincesByArea = Provinces.OrderBy(x => x.Area).ToImmutableList();
	}
	public readonly static ImmutableList<Province> Provinces;
	public readonly static ImmutableList<Province> ProvincesByPopulation;
	public readonly static ImmutableList<Province> ProvincesByArea;
}
