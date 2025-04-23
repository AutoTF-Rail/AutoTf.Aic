using System.Net.NetworkInformation;
using AutoTf.Aic.Models;
using AutoTf.Logging;

namespace AutoTf.Aic.Services;

public class NetworkConfigurator
{
	private static readonly Logger Logger = Statics.Logger;
	
	public static bool IsInternetAvailable()
	{
		try
		{
			using var ping = new Ping();

			PingReply reply = ping.Send("1.1.1.1", 1500);
			if (reply.Status == IPStatus.Success)
			{
				return true;
			}
		}
		catch
		{
			// ignored
		}

		Logger.Log("Got no internet connection.");
		return false;
	}
	
	public static void SetStaticIpAddress(string ipAddress, string subnetMask, string? newInterface = null)
	{
		try
		{
			newInterface ??= GetPrimaryEthernetInterface();
			
			if (CheckIpAddress(newInterface))
				return;
			
			Logger.Log("Setting Static IP.");
			string setIpCommand = $"ip addr add {ipAddress}/{subnetMask} dev {newInterface}";
			string bringUpInterfaceCommand = $"ip link set {newInterface} up";

			CommandExecuter.ExecuteSilent(setIpCommand, false);
			CommandExecuter.ExecuteSilent(bringUpInterfaceCommand, false);

			Logger.Log($"Set {ipAddress} on {newInterface} with subnet mask {subnetMask}");
		}
		catch (Exception ex)
		{
			Logger.Log($"An error occurred while setting IP: {ex.Message}");
			Logger.Log(ex.ToString());
			throw;
		}
	}
	
	public static bool CheckIpAddress(string interfaceName)
	{
		string checkIpCommand = $"ip addr show {interfaceName}";
		string output = CommandExecuter.ExecuteCommand(checkIpCommand);

		if (output.Contains("inet"))
		{
			Logger.Log($"Current IP settings for {interfaceName}:");
			Logger.Log(output.Split('\n').FirstOrDefault(x => x.Contains("inet"))?.Split("brd")[0].Replace("inet", "").Trim()!);
			return true;
		}

		Logger.Log($"{interfaceName} does not have an IP address set.");
		return false;
	}
	
	public static string GetPrimaryEthernetInterface()
	{
		string command = "ip -o link show | awk -F': ' '{print $2}'";
		string output = CommandExecuter.ExecuteCommand(command);
		
		string[] interfaces = output.Split('\n', StringSplitOptions.RemoveEmptyEntries);

		foreach (var iface in interfaces)
		{
			if (iface.StartsWith("en")) // Most ethernet interfaces start with 'en', e.g. on the jetson orin nano 
			{
				return iface.Trim();
			}
		}

		throw new Exception("No Ethernet interface found.");
	}
}