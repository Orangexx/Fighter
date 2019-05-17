/****************************************************************************
 * 2019.5 DESKTOP-ERI3UES
 ****************************************************************************/

public partial class LevelConfig : IConfig
{
	[ConfigField("Level")]
	public int Level  { get; set; }

	[ConfigField("StartPos")]
	public float StartPos  { get; set; }

	[ConfigField("EndPos")]
	public float EndPos  { get; set; }

	[ConfigField("Monsters")]
	public string Monsters { get; set; }

}
