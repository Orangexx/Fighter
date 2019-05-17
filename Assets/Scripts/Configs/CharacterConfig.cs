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

	[ConfigField("Speed")]
	public float Speed  { get; set; }

	[ConfigField("Exp")]
	public int Exp  { get; set; }

}
