/****************************************************************************
 * 2018.11 DESKTOP-CFTQOJO
 ****************************************************************************/
public partial class StateMap : IConfig
{
	[ConfigField("ID")]
	public int ID  { get; set; }

	[ConfigField("StateName")]
	public string StateName { get; set; }

	[ConfigField("NextStateNames")]
	public string NextStateNames { get; set; }

	[ConfigField("StateInfoID")]
	public int StateInfoID  { get; set; }

}
