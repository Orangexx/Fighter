/****************************************************************************
 * 2019.5 DESKTOP-ERI3UES
 ****************************************************************************/

public partial class CharacterConfig : IConfig
{
	[ConfigField("Level")]
	public int Level  { get; set; }

	[ConfigField("Poise")]
	public float Poise  { get; set; }

	[ConfigField("HP")]
	public int HP  { get; set; }

	[ConfigField("Power")]
	public float Power  { get; set; }

	[ConfigField("MoveSpeed")]
	public float MoveSpeed  { get; set; }

	[ConfigField("AttackSpeed")]
	public float AttackSpeed  { get; set; }

}
