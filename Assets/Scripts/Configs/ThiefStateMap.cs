/****************************************************************************
 * 2019.5 DESKTOP-ERI3UES
 ****************************************************************************/

public partial class ThiefStateMap : IConfig
{
	[ConfigField("ID")]
	public int ID  { get; set; }

	[ConfigField("StateName")]
	public string StateName { get; set; }

	[ConfigField("StateTriggers")]
	public string StateTriggers { get; set; }

	[ConfigField("StateInfoID")]
	public int StateInfoID  { get; set; }

}
