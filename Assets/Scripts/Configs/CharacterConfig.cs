/****************************************************************************
 * 2019.5 DESKTOP-ERI3UES
 ****************************************************************************/

public partial class CharacterConfig : IConfig
{
	[ConfigField("Level")]
	public int Level  { get; set; }

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
