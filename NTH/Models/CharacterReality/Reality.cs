using System.Collections.Immutable;

namespace NTH.Models.CharacterReality;

public static class Reality
{
	static Reality()
	{
		Characters = [
			new() { Nameue = "結月", Nameshita = "ゆかり", Introduction = "大人の女性の情感あふれる声" },
			new() { Nameue = "紲星", Nameshita = "あかり", Introduction = "明るい女の子の可愛らしい中にも優しさあふれる声" },
			new() { Nameue = "琴葉", Nameshita = "茜", Introduction = "関西弁で喋る百合姉" },
			new() { Nameue = "琴葉", Nameshita = "葵", Introduction = "標準語で喋る百合妹" },
		];
		CharactersByName = Characters.ToImmutableSortedDictionary(x => x.Nameue + x.Nameshita, x => x);
	}
	public readonly static ImmutableList<CharacterID> Characters;
	public readonly static ImmutableSortedDictionary<string, CharacterID> CharactersByName;
}
