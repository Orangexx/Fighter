/****************************************************************************
 * 2018.11 DESKTOP-CFTQOJO
 ****************************************************************************/

public partial class InputSetting : IConfig
{
	[ConfigField("Type")]
	public string Type { get; set; }

	[ConfigField("Key")]
	public string Key { get; set; }

}
