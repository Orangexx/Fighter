/****************************************************************************
 * 2018.11 DESKTOP-CFTQOJO
 ****************************************************************************/

public partial class StateInfo : IConfig
{
	[ConfigField("ID")]
	public int ID  { get; set; }

	[ConfigField("AttackBox")]
	public string AttackBox { get; set; }

	[ConfigField("HurtBox")]
	public string HurtBox { get; set; }

	[ConfigField("AttackInfo")]
	public int AttackInfo  { get; set; }

	[ConfigField("HrutInfo")]
	public int HrutInfo  { get; set; }

}
