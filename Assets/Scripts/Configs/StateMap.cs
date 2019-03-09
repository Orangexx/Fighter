/****************************************************************************
 * 2018.12 DESKTOP-CFTQOJO
 ****************************************************************************/

public partial class StateMap : IConfig
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
