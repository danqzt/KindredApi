using System.Text.Json;
using Common.Models;

namespace Server;

public class Constants
{
    public static readonly int[] CUST_IDS =
    [
        114568, 114569, 114570, 114571, 114572, 114573, 114574, 114575, 114576, 114577, 114578, 114579, 114580, 114581,
        114582, 114583, 114584, 114585, 114586, 114587, 114588, 114589
    ];
    
    public static BaseMessage[] Events => JsonSerializer.Deserialize<BaseMessage[]>(File.ReadAllText("events.json"))!;
    
}