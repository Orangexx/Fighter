/****************************************************************************
 * 2019.5 DESKTOP-ERI3UES
 ****************************************************************************/

public partial class MonsterConfig : IConfig
{
	[ConfigField("Name")]
	public string Name { get; set; }

	[ConfigField("Poise")]
	public int Poise  { get; set; }

	[ConfigField("HP")]
	public int HP  { get; set; }

	[ConfigField("Power")]
	public int Power  { get; set; }

	[ConfigField("MoveSpeed")]
	public int MoveSpeed  { get; set; }

	[ConfigField("AttackSpeed")]
	public int AttackSpeed  { get; set; }

}
