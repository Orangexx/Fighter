/****************************************************************************
 * 2019.6 DESKTOP-0CBBMSB
 ****************************************************************************/

public partial class Test : IConfig
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
