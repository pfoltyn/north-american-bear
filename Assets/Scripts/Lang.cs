using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

public class Lang
{
	public enum LanguageIdx
	{
		Polish,
		English
	};

	public const LanguageIdx currentLanguage = LanguageIdx.English;

	private const string langRecordId = "lang_record_id";
	private const string langScoreId = "lang_score_id";
	private const string langNewGameId = "lang_new_game_id";
	private const string langContinueId = "lang_continue_id";
	private const string langTitleId = "lang_title_id";
	private const string langSoundsId = "lang_sounds_id";
	private const string langMusicId = "lang_music_id";
	private const string langBgId = "lang_bg_id";

	private static Dictionary<string, string[]> langToResourceName = new Dictionary<string, string[]>(){
		{langRecordId, new[] {"rekord", "record"}},
		{langScoreId, new[] {"wynik", "score"}},
		{langNewGameId, new[] {"nowa_gra", "new_game"}},
		{langContinueId, new[] {"kontynuuj", "continue"}},
		{langTitleId, new[] {"słówka", "words"}},
		{langSoundsId, new[] {"dźwięki", "sounds"}},
		{langMusicId, new[] {"muzyka", "music"}},
		{langBgId, new[] {"ruch", "moving"}}
	};

	public static void Translate()
	{
		foreach (var key in langToResourceName.Keys)
		{
			GameObject go = GameObject.Find(key);
			if (go)
			{
				string resName = langToResourceName[key][(int)currentLanguage];
				go.GetComponent<MeshFilter>().mesh = (Mesh)Resources.Load(resName, typeof(Mesh));
			}
		}
	}
}
