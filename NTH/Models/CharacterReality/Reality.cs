using System.Collections.Immutable;

namespace NTH.Models.CharacterReality;

public static class Reality
{
	static Reality()
	{
		Characters = [
			new() { fixedChara = true, Nameue = "結月", Nameshita = "ゆかり", Introduction = "大人の女性の情感あふれる声" },
			new() { fixedChara = true, Nameue = "紲星", Nameshita = "あかり", Introduction = "明るい女の子の可愛らしい中にも優しさあふれる声" },
			new() { fixedChara = true, Nameue = "琴葉", Nameshita = "茜", Introduction = "関西弁で喋る百合姉" },
			new() { fixedChara = true, Nameue = "琴葉", Nameshita = "葵", Introduction = "標準語で喋る百合妹" },
			new() { fixedChara = true, Nameue = "弦巻", Nameshita = "マキ", Introduction = "胸が大きいバンドメンバー" },
			new() { fixedChara = true, Nameue = "宮舞", Nameshita = "モカ", Introduction = "もう上しか見えないDJ" },
			new() { fixedChara = true, Nameue = "紡乃世", Nameshita = "詞音", Introduction = "角という名の、奇抜な髪型をした有名人" },
			new() { fixedChara = true, Nameue = "双葉", Nameshita = "湊音", Introduction = "青春怪人" },
			new() { fixedChara = true, Nameue = "夏色", Nameshita = "花梨", Introduction = "小樽市のパイセン" },
			new() { fixedChara = true, Nameue = "小春", Nameshita = "六花", Introduction = "中国古詩" },
			new() { fixedChara = true, Nameue = "花隈", Nameshita = "千冬", Introduction = "文芸眼鏡娘" },
			new() { fixedChara = true, Nameue = "四国", Nameshita = "めたん", Introduction = "常に金欠高等部二年生" },
			new() { fixedChara = true, Nameue = "春日部", Nameshita = "つむぎ", Introduction = "埼玉ギャル" },
		];
		CharactersByName = Characters.ToImmutableSortedDictionary(x => x.Nameue + x.Nameshita, x => x);
	}
	public readonly static ImmutableList<CharacterID> Characters;
	public readonly static ImmutableSortedDictionary<string, CharacterID> CharactersByName;
}
